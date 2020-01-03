using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Login : MonoBehaviour {

    void Awake()
    {
        UtilLog.Log("Awake");
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        ResManager.Instance.Init();
        AssetBundlManager.Instance.Init();
    }

	void Update () {

    }
}
