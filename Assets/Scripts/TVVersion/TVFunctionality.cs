using UnityEngine;
using UnityEditor;
using SimpleJSON;
using someClient;
using System.Xml.Linq;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Video;

public class TVFunctionality : MonoBehaviour
{
    // ссылка на основную программу (в функции showTexture отображаем текстуру, к которой прикреплён скрипт MainProgramm)
    TVMainProgramm main;

    //для настройки и показа видео
    VideoPlayer videoPlayer;
    VideoSource videoSource;
    AudioSource audioSource;

    // GraphQl запрос (обязательно вместо ID монитора ставить $)
    string queryDetails = "query { monitor(id:$) { medias { length, url_file } } }";
    // http картинок
    string[] HTTP;

    // класс WWW имеет функционал для скачивания картинок из интернета (в его конструктор передаётся http запрос)
    public WWW[] web;

    // картинки
    Texture2D[] texture;

    int[] waitingTime;
    //ID монитора
    int monitor;
    // просто нужная штука
    public static int index = 0;
    // время, необходимое для ответа сервера на graphql запрос. если вылезают ислючения - ставим побольше
    public const int TimeNeedForResponse = 5;

    // фича для исправления багов показа картинок (тоже нужная штука)
    bool flag = true;
    // переменная говорит, скачаны ли картинки. если да, выполняет функцию показа showImages()
    bool imagesIsDownloaded;
    // пока идёт обновление контента, медиасы не отображаются(должно работать, хз)
    bool updateIsDone;
    //своего рода костыль, но нужный, суть в том,что показ видео по идее нужно вызывать 1 раз из старта
    //но вызываем мы его из update, поэтому делаем проверку, если играет, снова не воспроизводить
    public static bool playVideo = true;

    private void Awake()
    {
        // делаем ссылку на MainProgramm, чтобы строка main.Get... не кидало исключение 
        main = (TVMainProgramm)FindObjectOfType(typeof(TVMainProgramm));
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }


    //инициализация сервера; на вход передаём адрес сервера
    public void initServer(string url)
    {
        GraphQuery.url = url;
        imagesIsDownloaded = false;
        updateIsDone = false;
    }
    //на вход передаётся номер монитора
    public void initMonitor(int ID)
    {
        monitor = ID;
        queryDetails = queryDetails.Replace("$", ID.ToString());
    }

    // отправляем запрос и получаем ответ
    public void getDataFromServer()
    {
        GraphQuery.onQueryComplete += getResult;
        GraphQuery.POST(queryDetails);
    }

    //в этой функции распарсиваем данные
    public void getResult()
    {
        Debug.Log(GraphQuery.queryReturn);
        var N = JSON.Parse(GraphQuery.queryReturn);
        int countMedias = N["data"]["monitor"]["medias"].Count; // получаем количество medias
        HTTP = new string[countMedias];
        web = new WWW[countMedias];
        texture = new Texture2D[countMedias];
        waitingTime = new int[countMedias];
        for (int i = 0; i < countMedias; i++)
        {
            string name = N["data"]["monitor"]["medias"][i]["url_file"].Value;
            HTTP[i] = "http://192.168.0.84" + name; // заполняем массив http ссылками
            waitingTime[i] = int.Parse(N["data"]["monitor"]["medias"][i]["length"].Value); // заполняем массив временем показа картинок
        }
        OnDisable();
    }


    void OnDisable()
    {
        GraphQuery.onQueryComplete -= getResult;
    }

     //качаем медиасы
     public IEnumerator downloadMedias()
     {
        for (int i = 0; i < web.Length; i++)
        {
            // пока не разберёмся с сортировкой картинок и видео, запись не буду делать
            //запись на комп
            /*if (File.Exists(Application.persistentDataPath + "_" + i.ToString() + "_" + "testTexture.png"))
            {
                Debug.Log("load" + Application.persistentDataPath);
                byte[] byteArray = File.ReadAllBytes(Application.persistentDataPath + "_" + i.ToString() + "_" + "testTexture.png");
                texture[i] = new Texture2D(8, 8);
                texture[i].filterMode = FilterMode.Trilinear; // настройки для улучшения качетва изображения
                texture[i].anisoLevel = 16; // тоже настройка
                texture[i].LoadImage(byteArray);
            }*/

            //else
            //{
            Debug.Log("download");
            web[i] = new WWW(HTTP[i]);
            yield return web[i];
            if (HTTP[i].Contains(".mp4"))
            {
                File.WriteAllBytes(Application.persistentDataPath + "_" + i.ToString() + "_" + "video.mp4", web[i].bytes);
                continue;
            }
            texture[i] = web[i].texture;
            if (texture[i] != null)
            {
                texture[i].filterMode = FilterMode.Trilinear;
                texture[i].anisoLevel = 16;
            }
            byte[] bytes = texture[i].EncodeToPNG();
            if (bytes != null)
            {
                if (HTTP[i].Contains(".jpg") || HTTP[i].Contains(".JPG"))
                {
                    File.WriteAllBytes(Application.persistentDataPath + "_" + i.ToString() + "_" + "testTexture.png", bytes);
                }
            }
            //}   
        }
        imagesIsDownloaded = true;
        //updateIsDone = true;
    }

    // отображает текстуру на обекте, к которому прикремлён скрипт MainProgramm
    // в нашем случае прикремлён к объекту RawImage
    // на вход передаётся массив со временем
    IEnumerator ShowTexture(int waitTime)
    {
        // это для видео (вызывается сопрограмма Play())
        if (HTTP[index].Contains(".mp4"))
        {

            if (playVideo) StartCoroutine(Play());
            playVideo = false;
        }

        // это для картинок, просто присваиваем текстуру
        else if ((HTTP[index].Contains(".jpg") || HTTP[index].Contains(".JPG")) && texture[index] != null)
        {
            main.GetComponent<RawImage>().texture = texture[index];
        }

        flag = false;
        yield return new WaitForSeconds(waitTime);
        index++;
        playVideo = true;
        //при обновлении раскоментить
        /*if (updateIsDone)
        {
            index = -1;
            updateIsDone = false;
        }*/

        if (index == web.Length - 1) index = 0;
            flag = true;
    }
   

    // каждый фрейм в методе update показываем картинки через интервал времени, который хранится в массиве waitingTime
    public void showImages(int waitTime)
    {
        if (imagesIsDownloaded)
        {
            if (flag)
            {
                StartCoroutine(ShowTexture(waitTime));
            }
        }
    }

    public IEnumerator updateContent(int repeatTime)
    {
        for (; ; )
        {
            yield return new WaitForSeconds(repeatTime);
            Debug.Log("Start delete");
            for (int i = 0; i < web.Length; i++)
            {
                File.Delete(Application.persistentDataPath + "_" + i.ToString() + "_" + "testTexture.png");
            }

            //initServer("http://192.168.0.84/control/graphql.php");
            initMonitor(monitor);
            getDataFromServer();
            yield return new WaitForSeconds(TimeNeedForResponse);
            yield return downloadMedias();
        }
    }

    IEnumerator Play()
    {
        AudioListener.volume = 1;
        videoPlayer.playOnAwake = true;
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = Application.persistentDataPath + "_" + index.ToString() + "_" + "video.mp4";
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        //Assign the Audio from Video to AudioSource to be played
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);
        videoPlayer.Prepare();

        //Wait until video is prepared
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        GetComponent<RawImage>().texture = videoPlayer.texture;
        videoPlayer.Play();
        audioSource.Play();
    }
}
