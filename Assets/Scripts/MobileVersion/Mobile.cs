using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Mobile : Platform {

    IEnumerator Start () {
        queryDetails = "query { monitor(id:$) { medias { length, type, name,  url_file } } }";
        initServer("http://192.168.0.84/control/graphql.php");
        initMonitor(9);
        getDataFromServer(); 
        yield return new WaitForSeconds(TimeNeedForResponse);
        yield return downloadMedias();
        //server.StartCoroutine(server.updateContent(40));

    }

    void Update () {
        showMediasUsingTouches();
    }


}
