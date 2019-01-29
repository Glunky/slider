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
using System;

public class Platform : MonoBehaviour
{

    Text text;

    Vector2 startTouchPosition, endTouchPosition;

    protected VideoPlayer videoPlayer;
    VideoSource videoSource;
    protected AudioSource audioSource;

    protected string queryDetails;
    string[] HTTP;
    string[] nameMedias;
    string[] typeMedias;
    WWW[] web;
    Texture2D[] texture;
    int[] waitingTime;

    int monitor;
    public static int index = 0;
    protected const int TimeNeedForResponse = 5;

    bool flag = true;
    bool imagesIsDownloaded;
    bool updateIsDone;
    public static bool playVideo = true;

    private void Awake()
    {
        text = (Text)FindObjectOfType(typeof(Text));
        text.enabled = false;
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    protected void initServer(string URL)
    {
        GraphQuery.url = URL;
        imagesIsDownloaded = false;
        updateIsDone = false;
    }
    protected void initMonitor(int ID)
    {
        monitor = ID;
        queryDetails = queryDetails.Replace("$", ID.ToString());
    }
    protected void getDataFromServer()
    {
        GraphQuery.onQueryComplete += getResult;
        GraphQuery.POST(queryDetails);
    }
    protected void getResult()
    {
        Debug.Log(GraphQuery.queryReturn);
        var N = JSON.Parse(GraphQuery.queryReturn);
        int countMedias = N["data"]["monitor"]["medias"].Count; // получаем количество medias
        HTTP = new string[countMedias];
        web = new WWW[countMedias];
        texture = new Texture2D[countMedias];
        typeMedias = new string[countMedias];
        waitingTime = new int[countMedias];
        nameMedias = new string[countMedias];
        for (int i = 0; i < countMedias; i++)
        {
            string name = N["data"]["monitor"]["medias"][i]["url_file"].Value;
            HTTP[i] = "http://192.168.0.84" + name; // заполняем массив http ссылками
            waitingTime[i] = int.Parse(N["data"]["monitor"]["medias"][i]["length"].Value); // заполняем массив временем показа картинок
            typeMedias[i] = N["data"]["monitor"]["medias"][i]["type"].Value;
            nameMedias[i] = N["data"]["monitor"]["medias"][i]["name"].Value;
        }
        OnDisable();
    }

    void OnDisable()
    {
        GraphQuery.onQueryComplete -= getResult;
    }

    protected IEnumerator downloadMedias()
    {
        text.enabled = true;
            
        for (int i = 0; i < web.Length; i++)
        {

            text.text = "Downoading: " + (i + 1).ToString() + "/" + web.Length;
            web[i] = new WWW(HTTP[i]);
            yield return web[i];
            if (typeMedias[i] == "video")
                File.WriteAllBytes(Application.persistentDataPath + nameMedias[i] + ".mp4", web[i].bytes);


            else if (typeMedias[i] == "image")
                texture[i] = web[i].texture;


        }

        text.enabled = false;
        imagesIsDownloaded = true;
        //updateIsDone = true;
    }

    //придумать норм название
    IEnumerator showTextureThroughTime(int waitTime)
    {
        // это для видео (вызывается сопрограмма Play())
        if (typeMedias[index] == "video")
        {
            if (playVideo) StartCoroutine(PlayVideo());
            playVideo = false;
        }

        // это для картинок, просто присваиваем текстуру
        else if (typeMedias[index] == "image" && texture[index] != null)
            GetComponent<RawImage>().texture = texture[index];
        

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

    void showTextureWithTouches()
    {

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            startTouchPosition = Input.GetTouch(0).position;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            endTouchPosition = Input.GetTouch(0).position;
            if (endTouchPosition.x > startTouchPosition.x) index--;
            if (endTouchPosition.x < startTouchPosition.x) index++;
        }


        if (index > web.Length - 1)
            index = 0;

        else if (index < 0)
            index = web.Length - 1;

        if (typeMedias[index] == "video")
        {
            if (playVideo) StartCoroutine(PlayVideo());
            playVideo = false;
        }

        else if (typeMedias[index] == "image"&& texture[index] != null)
            GetComponent<RawImage>().texture = texture[index];


        /*if (updateIsDone)
        {
            index = -1;
            updateIsDone = false;
        }*/
    }

    protected void showMediasUsingTime(int waitTime)
    {
        if (imagesIsDownloaded)
        {
            if (flag)
            {
                StartCoroutine(showTextureThroughTime(waitTime));
            }
        }
        
    }

    protected void showMediasUsingTouches()
    {
            if (imagesIsDownloaded)
                showTextureWithTouches();
        
    }

    protected IEnumerator updateContent(int repeatTime)
    {
        for (; ; )
        {
            yield return new WaitForSeconds(repeatTime);
            Debug.Log("Start delete");
            for (int i = 0; i < web.Length; i++)
            {
                File.Delete(Application.persistentDataPath + nameMedias[index] + ".mp4");
            }

            //initServer("http://192.168.0.84/control/graphql.php");
            initMonitor(monitor);
            getDataFromServer();
            yield return new WaitForSeconds(TimeNeedForResponse);
            yield return downloadMedias();
        }
    }

    IEnumerator PlayVideo()
    {
        AudioListener.volume = 1;
        videoPlayer.playOnAwake = false;
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = Application.persistentDataPath + nameMedias[index] + ".mp4";
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);
        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        GetComponent<RawImage>().texture = videoPlayer.texture;
        videoPlayer.Play();
        audioSource.Play();
    }

}