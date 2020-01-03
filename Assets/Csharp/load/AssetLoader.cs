//#define NotAdLoad
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEditor;

public class AssetLoader: Singleton<AssetLoader>
{ 
    public MonoCoroutine monoCoroutine = null;
    public void LoadAsset<T>(string resPath, Action<UnityEngine.Object, string>loadCallBack) where T : UnityEngine.Object
    {
#if NotAdLoad
        T obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(resPath);
        if (obj == null)
        {
            UtilLog.Error("Load Asset Is Null: " + resPath);
            return;
        }
        if (loadCallBack != null)
        {
            loadCallBack(obj, resPath);
        }
#else
        AssetBundlManager.Instance.LoadAsset<T>(resPath, loadCallBack);
#endif

    }

    public void LoadAssetAsync<T>(string resPath, Action<UnityEngine.Object, string> loadCallBack) where T : UnityEngine.Object
    {
#if NotAdLoad
        T obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(resPath);
        if (obj == null)
        {
            UtilLog.Error("Load Asset Is Null: " + resPath);
        }
        this.monoCoroutine.StartCoroutine(AssetLoaderAsync(() =>
        {
            if (loadCallBack != null)
            {
                loadCallBack(obj, resPath);
            }
        }));
#else
        AssetBundlManager.Instance.LoadAssetAsync<T>(resPath, loadCallBack);
#endif

    }

    public void ClearRes(string resPath)
    {
#if NotABLoad

#else
        AssetBundlManager.Instance.ClearRes(resPath);
#endif
    }

    private IEnumerator AssetLoaderAsync(Action callBack)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if (callBack != null)
        {
            callBack();
        }
    }
}
