using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RightButton : MonoBehaviour, IPointerDownHandler
{

    public void OnPointerDown(PointerEventData eventData)
    {
        MobileFunctionality.index++;
        MobileFunctionality.playVideo = true;
        AudioListener.volume = 0;
    }
}
