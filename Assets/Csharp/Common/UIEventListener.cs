using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIEventListener : MonoBehaviour, IPointerClickHandler
{
    public static UIEventListener Get(GameObject obj)
    {
        UIEventListener comp = obj.GetComponent<UIEventListener>();
        if (comp == null)
        {
            comp = obj.AddComponent<UIEventListener>();
        }
        return comp;
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }
}
