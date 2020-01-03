using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Xml;
using System;

public class AssetBundlManager:Singleton<AssetBundlManager>
{
    private Dictionary<string, string> resAbNameDict = null; //资源名字 -- ab 的映射
    private Dictionary<string, int> abRefDict = null;  //ab包的引用次数
    private Dictionary<string, AssetBundle> abDict = null; //保存所有已加载的ab包
    public MonoCoroutine monoCoroutine = null;

    private bool isXmLLoaded = false;

    public void Init()
    {
        resAbNameDict = new Dictionary<string, string>();
        abRefDict = new Dictionary<string, int>();
        abDict = new Dictionary<string, AssetBundle>();
        isXmLLoaded = false;

        this.readXML();
    }

    public bool IsXmlLoaded()
    {
        return this.isXmLLoaded;
    }

    private void readXML()
    {
        this.monoCoroutine.StartCoroutine(loadXML());
    }

    IEnumerator loadXML()
    {
        string resPath = Path.Combine(Application.streamingAssetsPath, "assetbundle/BundleDict.xml");

        resPath = resPath.Replace('\\', '/');

        resPath = "file://" + resPath;

        UtilLog.Error(resPath);

        UnityWebRequest req = new UnityWebRequest(resPath);

        req.downloadHandler = new DownloadHandlerBuffer();

        yield return req.SendWebRequest();

        XmlDocument resXml = new XmlDocument();
        resXml.LoadXml(req.downloadHandler.text);

        XmlNode root = resXml.SelectSingleNode("ResMap");

        foreach (var v in root)
        {
            XmlElement elm = (XmlElement)v;
            string resPathStr = elm.GetAttribute("ResPath");
            string abName = elm.GetAttribute("AbName");

            resAbNameDict[resPathStr] = Path.Combine(Application.streamingAssetsPath, "assetbundle/" + abName);
            resAbNameDict[resPathStr] = resAbNameDict[resPathStr].Replace("\\", "/");
            UtilLog.Error("资源" + resPathStr + " " + resAbNameDict[resPathStr]);
        }

        isXmLLoaded = true;
    }

    private string getAbName(string resPath)
    {
        if(resAbNameDict.ContainsKey(resPath) == false)
        {
            UtilLog.Error(" Cannot find abName " + resPath);
            return "";
        }

        return resAbNameDict[resPath];
    }

    public void ClearRes(string resPath)
    {
        string abName = getAbName(resPath);
        removeAbRef(abName);
    }

    public void AddRefRes(string resPath)
    {
        string abName = getAbName(resPath);
        addAbRef(abName);
    }

    private void removeAbRef(string abName)
    {
        if (abRefDict.ContainsKey(abName) == false)
        {
            abRefDict[abName] = 0;
        }
        else
        {
            abRefDict[abName] = abRefDict[abName] - 1;
        }

        if(abRefDict[abName] <= 0 && abDict.ContainsKey(abName))
        {
            if (abDict[abName] != null)
            {
                UtilLog.Error(abName);
                abDict[abName].Unload(false);
                abDict[abName] = null;
            }
        }
    }

    private void addAbRef(string abName)
    {
        if(abRefDict.ContainsKey(abName)==false)
        {
            abRefDict[abName] = 0;
        }
        abRefDict[abName] = abRefDict[abName] + 1;
    }

    public void LoadAsset<T>(string resPath, Action<UnityEngine.Object, string>loadedCallBack)where T:UnityEngine.Object
    {
        string abName = getAbName(resPath);
        if(abName == "")
        {
            return;
        }
        AssetBundle ab = null;
            
        if(abDict.ContainsKey(abName))
        {
            ab = abDict[abName];
        }
        else
        {
            ab = AssetBundle.LoadFromFile(abName);
        }

        if (ab == null)
        {
            UtilLog.Error("Load AssetBundle Is Null: " + abName);
            return;
        }

        abDict[abName] = ab;

        addAbRef(abName);

        UnityEngine.Object obj = ab.LoadAsset<T>(resPath);

        if(obj==null)
        {
            removeAbRef(abName);
            UtilLog.Error(" AssetBundle " + abName + " cannot find: " + resPath);
            return;
        }
        

        if (loadedCallBack != null)
        {
            loadedCallBack(obj, resPath);
        }
    }

    public void LoadAssetAsync<T>(string resPath, Action<UnityEngine.Object,string>loadedCallBack)where T:UnityEngine.Object
    {
        string abName = getAbName(resPath);
        if (abName == "")
        {
            return;
        }

        this.monoCoroutine.StartCoroutine(loadAb(resPath, loadedCallBack));
    }

    IEnumerator loadAb(string resPath, Action<UnityEngine.Object, string> loadCallBack)
    {
        string abName = getAbName(resPath);

        AssetBundle ab = null;

        if(abDict.ContainsKey(abName))
        {
            ab = abDict[abName];
        }
        else
        {
            UnityWebRequest www = UnityWebRequest.GetAssetBundle(abName, 0);
            yield return www.SendWebRequest();

            ab = DownloadHandlerAssetBundle.GetContent(www);
        }

        if(ab==null)
        {
            UtilLog.Error("Load AssetBundle Is Null: " + abName);
        }

        abDict[abName] = ab;
        addAbRef(abName);

        UnityEngine.Object obj = ab.LoadAsset<UnityEngine.Object>(resPath);

        if(obj==null)
        {
            removeAbRef(abName);
            UtilLog.Error(" AssetBundle " + abName + " cannot find: " + resPath);
        }
        else
        {
            

            if(loadCallBack!=null)
            {
                loadCallBack(obj, resPath);
            }
        }
    }
}
