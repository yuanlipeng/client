using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void LoadCb(UnityEngine.Object obj, string resPath);

public enum ResEnum
{
    Texture2D,
    Prefab,
    Shader,
    SpriteAsset,
}
public class ResLoadInfo
{
    public ResLoadInfo()
    {
        LoadFinishCb = new List<LoadCb>();
    }

    public string ResPath;
    public List<LoadCb> LoadFinishCb;
    public ResEnum resEnum;

    public void CallLoadCb(UnityEngine.Object obj)
    {
        int count = LoadFinishCb.Count;
        for(int i=0; i<count; i++)
        {
            LoadFinishCb[i](obj, ResPath);
        }
    }

    public void Clear()
    {
        LoadFinishCb.Clear();
    }
}
