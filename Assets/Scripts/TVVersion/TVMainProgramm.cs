using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TVMainProgramm : MonoBehaviour
{

    TVFunctionality TV;

    IEnumerator Start()
    {

        TV = GetComponent<TVFunctionality>();
        TV.initServer("http://192.168.0.84/control/graphql.php");
        TV.initMonitor(9);
        TV.getDataFromServer();
        yield return new WaitForSeconds(TVFunctionality.TimeNeedForResponse);
        yield return TV.downloadMedias();
        //server.StartCoroutine(server.updateContent(40));

    }

    void Update()
    {
        TV.showImages(3);
    }


}
