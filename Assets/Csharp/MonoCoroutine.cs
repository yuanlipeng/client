using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoCoroutine : MonoBehaviour {

    void Awake()
    {
        AssetLoader.Instance.monoCoroutine = this;
        AssetBundlManager.Instance.monoCoroutine = this;
    }

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
