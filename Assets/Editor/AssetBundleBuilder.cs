using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;

public static class AssetBundBuilder
{
    private static string resDir = "Assets/Res";
    private static readonly string[] ResourceExts = {".prefab",
                                                     ".png", ".jpg",
                                                     ".controller", ".shader", ".anim", ".mat",".spriteatlas"};

    private static List<string> allFileList = new List<string>();

    private static Dictionary<string, List<string>> depCountDict = new Dictionary<string, List<string>>();

    [MenuItem("打包/更新AbName")]
    public static void BeginBuilder()
    {
        ////MonoCoroutine.Instantiate.
        allFileList.Clear();
        depCountDict.Clear();
        GetAllSubResFiles(resDir, allFileList);

        int count = allFileList.Count;
        for (int i = 0; i < count; i++)
        {
            string[] depList = AssetDatabase.GetDependencies(allFileList[i]);
            int depCount = depList.Length;
            //UtilLog.Error(" 当前文件 " + allFileList[i] + " ========================== ");
            for (int j = 0; j < depCount; j++)
            {
                if (FileIsResource(depList[j]) && string.Equals(depList[j], allFileList[i]) == false)
                {
                    if (IsAlatsSprite(depList[j]) && IsSpriteAltas(allFileList[i]))
                    {
                        if (depCountDict.ContainsKey(depList[j]))
                        {
                            UtilLog.Error("图片" + depList[j] + "同时在两个图集里面:" + depCountDict[depList[j]][0] + " " + allFileList[i]);
                        }
                        else
                        {
                            depCountDict[depList[j]] = new List<string>();
                            depCountDict[depList[j]].Insert(0, allFileList[i]);
                        }
                    }
                    else if (IsAlatsSprite(depList[j]) == false)
                    {
                        //UtilLog.Error(" 依赖的文件 " + depList[j]);
                        if (depCountDict.ContainsKey(depList[j]))
                        {
                            depCountDict[depList[j]].Insert(0, allFileList[i]);
                        }
                        else
                        {
                            depCountDict[depList[j]] = new List<string>();
                            depCountDict[depList[j]].Insert(0, allFileList[i]);
                        }
                    }
                }
            }
        }

        XmlDocument abDict = new XmlDocument();
        XmlElement dict = abDict.CreateElement("ResMap");

        for (int i = 0; i < count; i++)
        {
            AssetImporter importer = AssetImporter.GetAtPath(allFileList[i]);
            if (depCountDict.ContainsKey(allFileList[i]))
            {
                int depCount = depCountDict[allFileList[i]].Count;
                if (depCount == 1)
                {
                    importer.assetBundleName = string.Concat(depCountDict[allFileList[i]][0], ".unity3d");
                }
                else
                {
                    importer.assetBundleName = string.Concat(allFileList[i], ".unity3d");
                }
            }
            else
            {
                importer.assetBundleName = string.Concat(allFileList[i], ".unity3d");
            }
            importer.assetBundleName = importer.assetBundleName.Replace("/", ".");
            XmlElement element = abDict.CreateElement("Res");
            element.SetAttribute("ResPath", allFileList[i]);
            element.SetAttribute("AbName", importer.assetBundleName);

            dict.AppendChild(element);
        }
        abDict.AppendChild(dict);
        UtilLog.Error(Application.streamingAssetsPath);
        string path = Application.streamingAssetsPath + "/AssetBundle/BundleDict.xml";
        path = path.Replace("/", "//");
        UtilLog.Error(path);
        abDict.Save(path);
        AssetDatabase.Refresh();
        BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/AssetBundle", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);
    }

    private static bool IsAlatsSprite(string name)
    {
        return name.Contains("res/art");
    }

    private static bool IsSpriteAltas(string name)
    {
        return name.Contains(".spriteatlas");
    }

    public static List<string> GetAllSubResFiles(string fullPath)
    {
        List<string> fileList = new List<string>();
        GetAllSubResFiles(fullPath, fileList);
        return fileList;
    }

    //递归获取当前路径下的所有资源文件路径
    private static void GetAllSubResFiles(string fullPath, List<string> fileList)
    {
        if ((fileList == null) || (string.IsNullOrEmpty(fullPath)))
            return;

        string[] files = System.IO.Directory.GetFiles(fullPath);
        if ((files != null) && (files.Length > 0))
        {
            for (int i = 0; i < files.Length; ++i)
            {
                string fileName = files[i];
                if (FileIsResource(fileName))
                {
                    fileName = fileName.Replace("\\", "/");
                    fileList.Add(fileName);
                }
            }
        }

        string[] dirs = System.IO.Directory.GetDirectories(fullPath);
        if (dirs != null)
        {
            for (int i = 0; i < dirs.Length; ++i)
            {
                GetAllSubResFiles(dirs[i], fileList);
            }
        }
    }

    public static bool FileIsResource(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return false;
        string ext = Path.GetExtension(fileName);
        if (string.IsNullOrEmpty(ext))
            return false;
        for (int i = 0; i < ResourceExts.Length; ++i)
        {
            if (string.Compare(ext, ResourceExts[i], true) == 0)
            {
                return true;
            }
        }

        return false;
    }
}
