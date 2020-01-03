using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
public class ResManager : Singleton<ResManager>
{
    #region　加载模块
    private Dictionary<string, UnityEngine.Object> loadedCacheDict = new Dictionary<string, UnityEngine.Object>();
    private Dictionary<string, int> cacheRefDict = new Dictionary<string, int>();
    private Dictionary<string, ResLoadInfo> waitLoadInfoDict = new Dictionary<string, ResLoadInfo>();

    private ObjectPool<ResLoadInfo> resLoadInfoPool = null;
    public void Init()
    {
        resLoadInfoPool = new ObjectPool<ResLoadInfo>(30,
            () =>
            {
                return new ResLoadInfo();
            },
            null,
            (ResLoadInfo loadInfo)=>
            {
                loadInfo.Clear();
            }
        );
    }

    public void Clear()
    {
        resLoadInfoPool.Clear();
    }

    public void LoadAsset<T>(string resPath, LoadCb loadCallBack, ResEnum resEnum) where T : UnityEngine.Object
    {
        loadAsset<T>(resPath, loadCallBack, resEnum, false);
    }

    public void LoadAssetAsync<T>(string resPath, LoadCb loadCallBack, ResEnum resEnum) where T : UnityEngine.Object
    {
        loadAsset<T>(resPath, loadCallBack, resEnum, true);
    }

    private void loadAsset<T>(string resPath, LoadCb loadCallBack, ResEnum resEnum, bool isAsync = true) where T: UnityEngine.Object 
    {
        if (loadedCacheDict.ContainsKey(resPath))
        {
            if (loadCallBack != null)
            {
                addResRef(resPath);
                loadCallBack(loadedCacheDict[resPath], resPath);
            }
            return;
        }

        if (waitLoadInfoDict.ContainsKey(resPath))
        {
            waitLoadInfoDict[resPath].LoadFinishCb.Insert(0, loadCallBack);
            return;
        }

        ResLoadInfo loadInfo = this.resLoadInfoPool.GetItem();
        loadInfo.ResPath = resPath;
        loadInfo.LoadFinishCb.Insert(0, loadCallBack);

        waitLoadInfoDict[resPath] = loadInfo;

        if(isAsync)
        {
            AssetLoader.Instance.LoadAssetAsync<T>(resPath, loadFinishCallBack);
        }
        else
        {
            AssetLoader.Instance.LoadAsset<T>(resPath, loadFinishCallBack);
        }
    }

    private void loadFinishCallBack(UnityEngine.Object obj, string resPath)
    {
        if (waitLoadInfoDict.ContainsKey(resPath))
        {
            loadedCacheDict[resPath] = obj;

            ResLoadInfo info = waitLoadInfoDict[resPath];

            addResRef(resPath);

            if (info != null)
            {
                info.CallLoadCb(obj);
                resLoadInfoPool.ReturnItem(info);
            }

            resLoadInfoPool.ReturnItem(waitLoadInfoDict[resPath]);
            waitLoadInfoDict.Remove(resPath);
        }
    }

    private void addResRef(string resPath)
    {
        if(cacheRefDict.ContainsKey(resPath) == false)
        {
            cacheRefDict[resPath] = 0;
        }
        cacheRefDict[resPath] = cacheRefDict[resPath] + 1;
    }

    private void removeResRef(string resPath)
    {
        if (cacheRefDict.ContainsKey(resPath) == false || cacheRefDict[resPath] == 0)
        {
            cacheRefDict[resPath] = 0;
        }
        else
        {
            cacheRefDict[resPath] = cacheRefDict[resPath] - 1;
        }

        if(cacheRefDict[resPath] <= 0)
        {
            AssetLoader.Instance.ClearRes(resPath);
            removeCache(resPath);
        }
    }

    public void removeCache(string resPath)
    {
        if(loadedCacheDict.ContainsKey(resPath))
        {
            Resources.UnloadAsset(loadedCacheDict[resPath]);
            loadedCacheDict.Remove(resPath);
        }
        if(waitLoadInfoDict.ContainsKey(resPath))
        {
            resLoadInfoPool.ReturnItem(waitLoadInfoDict[resPath]);
            waitLoadInfoDict.Remove(resPath);
        }
    }

#endregion

#region 外部调用模块
    public void SetUITexture(GameObject obj, string resPath)
    {
        LoadAsset<Texture2D>(resPath, (UnityEngine.Object loadObj, string path) =>
            {
                Texture2D tex = (Texture2D)loadObj;
                tex.name = path;
                Sprite sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                sp.name = path;
                obj.GetComponent<Image>().sprite = sp;

                UtilLog.Error(obj.GetComponent<Image>().sprite.texture.name);
            }, 
            ResEnum.Texture2D
        );
    }

    public void ClearUITexture(GameObject obj)
    {
        Sprite sp = obj.GetComponent<Image>().sprite;
        if(sp == null)
        {
            return;
        }
        string resPath = sp.texture.name;
        removeResRef(resPath);
        obj.GetComponent<Image>().sprite = null;
    }

    public void LoadPrefab(string resPath, LoadCb loadCallBack)
    {
        LoadAssetAsync<GameObject>(resPath, loadCallBack, ResEnum.Prefab);
    }

    public void ClearPrefab(string resPath)
    {

    }
#endregion
}