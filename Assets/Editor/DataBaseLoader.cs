using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class DataBaseLoader : Singleton<DataBaseLoader>
{
    public MonoCoroutine monoCoroutine = null;
    public void LoadAsset<T>(string resPath, Action<UnityEngine.Object, string> loadCallBack) where T : UnityEngine.Object
    {
        T obj = AssetDatabase.LoadAssetAtPath<T>(resPath);
        if (obj == null)
        {
            UtilLog.Error("Load Asset Is Null: " + resPath);
            return;
        }
        if (loadCallBack != null)
        {
            loadCallBack(obj, resPath);
        }

    }

    public void LoadAssetAsync<T>(string resPath, Action<UnityEngine.Object, string> loadCallBack) where T : UnityEngine.Object
    {
        T obj = AssetDatabase.LoadAssetAtPath<T>(resPath);
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
