﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Mobile : Platform {

    IEnumerator Start () {

        queryDetails = "query { monitor(id:794619) { medias { length, type, name, url_file } } }"; // формируем запрос
        initServer("http://minos84.ru/control/graphql.php"); // инициализируем сервер
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
