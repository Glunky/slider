using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LeftButtonScript : MonoBehaviour, IPointerDownHandler
{

    public void OnPointerDown(PointerEventData eventData)
    {
        manager.indexOfMediaFile--;
        AudioListener.volume = 0;
        if (manager.indexOfMediaFile < 0)
            manager.indexOfMediaFile = manager.countOfMedias - 1;

        manager.videoIsPlaying = true;
        //manager.video = true;
        //AudioListener.volume = 0;
    }
}
