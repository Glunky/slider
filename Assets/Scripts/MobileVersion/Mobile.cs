using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Mobile : Platform {

    IEnumerator Start () {

        queryDetails = "query { monitor(id:9) { medias { length, type, name, url_file } } }"; // формируем запрос
        initServer("http://192.168.0.84/control/graphql.php"); // инициализируем сервер
        //initMonitor(9); // инициализируем монитор
        getDataFromServer(); //получаем данные от сервера
        yield return new WaitForSeconds(TimeNeedForResponse); // ждём некоторе время, чтобы не вылезло исключение
        yield return downloadMedias(); // скачиваем медиафайлы
        //server.StartCoroutine(server.updateContent(40)); // если хотим обновлять контент
    }

    void Update () {
        showMediasUsingTouches();
    }
}
