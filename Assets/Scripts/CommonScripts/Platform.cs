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

public class Platform : MonoBehaviour
{ 

    Vector2 startTouchPosition, endTouchPosition;

    protected VideoPlayer videoPlayer;
    VideoSource videoSource;
    protected AudioSource audioSource;

    protected string queryDetails;
    string[] HTTP;
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
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    protected void initServer(string URL) {
        GraphQuery.url = URL;
        imagesIsDownloaded = false;
        updateIsDone = false;
    }
    protected void initMonitor(int ID) {
        monitor = ID;
        queryDetails = queryDetails.Replace("$", ID.ToString());
    }
    protected void getDataFromServer() {
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

    protected IEnumerator downloadMedias()
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
                    File.WriteAllBytes(Application.persistentDataPath + "_" + i.ToString() + "_" + "testTexture.png", bytes);

            }
            //}   
        }
        imagesIsDownloaded = true;
        //updateIsDone = true;
    }

    //придумать норм название
    IEnumerator showTextureThroughTime(int waitTime)
    {
        // это для видео (вызывается сопрограмма Play())
        if (HTTP[index].Contains(".mp4"))
        {

            if (playVideo) StartCoroutine(PlayVideo());
            playVideo = false;
        }

        // это для картинок, просто присваиваем текстуру
        else if ((HTTP[index].Contains(".jpg") || HTTP[index].Contains(".JPG")) && texture[index] != null)
        {
            GetComponent<RawImage>().texture = texture[index];
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

        if (HTTP[index].Contains(".mp4"))
        {
            if (playVideo) StartCoroutine(PlayVideo());
            playVideo = false;
        }

        else if ((HTTP[index].Contains(".jpg") || HTTP[index].Contains(".JPG")) && texture[index] != null)
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
                File.Delete(Application.persistentDataPath + "_" + i.ToString() + "_" + "testTexture.png");
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
