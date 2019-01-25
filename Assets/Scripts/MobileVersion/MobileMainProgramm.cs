using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MobileMainProgramm : MonoBehaviour {

    MobileFunctionality Mobile;

    IEnumerator Start () {

        Mobile = GetComponent<MobileFunctionality>();
        Mobile.initServer("http://192.168.0.84/control/graphql.php");
        Mobile.initMonitor(9);
        Mobile.getDataFromServer(); 
        yield return new WaitForSeconds(MobileFunctionality.TimeNeedForResponse);
        yield return Mobile.downloadImages();
        //server.StartCoroutine(server.updateContent(40));

    }

    void Update () {
        Mobile.showImages();
    }


}
