using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class Test : MonoBehaviour {

    // Use this for initialization
    public Image txtImage;
    public Image altasImage;

    void Start () {

    }

    public void BeginLoad()
    {
        ResManager.Instance.SetUITexture(txtImage.gameObject, "Assets/Res/Texture/Icon_Order_1.png");
        ResManager.Instance.SetUITexture(altasImage.gameObject, "Assets/Res/Texture/Icon_Order_1.png");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
