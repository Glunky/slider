using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class manager : MonoBehaviour
{
    VideoPlayer videoPlayer;
    VideoSource videoSource;
    AudioSource audioSource;

    List<Mediafile> MediaFiles = new List<Mediafile>();
    bool mediasIsDownloaded = false;
    public static bool videoIsPlaying = false;
    public static int indexOfMediaFile = 0;
    public static int countOfMedias;

    private void Awake()
    {
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        StartCoroutine(Upload());
    }

    
    void Update()
    {
        if (mediasIsDownloaded)
        {
            if(MediaFiles[indexOfMediaFile].id_type == "1")
            {
                GetComponent<RawImage>().texture = MediaFiles[indexOfMediaFile].texture;
            }

            else if(MediaFiles[indexOfMediaFile].id_type == "2" && videoIsPlaying)
            {
                StartCoroutine(PlayVideo());
                videoIsPlaying = false;
            }
        } 
    }


    IEnumerator Upload()
    {
        //http://192.168.0.84/view/external.php?action=enumerate&data={"class":"mediafile","properties":["file", "id_type", "length"]}
        WWWForm form = new WWWForm();
        form.AddField("action", "enumerate");
        form.AddField("data", "{\"class\":\"mediafile\",\"properties\":[\"file\", \"length\", \"id_type\"]}");

        UnityWebRequest www = UnityWebRequest.Post("http://minos84.ru/view/external.php", form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }

        else
        {
            string response = www.downloadHandler.text;
            Debug.Log(response);
            var N = JSON.Parse(response);
            int countMedias = N["objects"].Count;
            countOfMedias = countMedias;
            Debug.Log(countMedias);
            

            for (int i = 0; i < countMedias; i++)
            {
                MediaFiles.Add(new Mediafile());
                int lastElement = MediaFiles.Count - 1;
                Mediafile mediafile = MediaFiles[i];
                mediafile.nameMedias = N["objects"][i]["name"].Value;
                mediafile.ID = N["objects"][i]["id"].Value;
                mediafile.HTTP = "http://minos84.ru" + N["objects"][i]["properties"]["file"].Value;
                mediafile.waitingTime = double.Parse(N["objects"][i]["properties"]["length"].Value);
                mediafile.id_type = N["objects"][i]["properties"]["id_type"].Value;
                mediafile.web = new WWW(mediafile.HTTP);
                yield return mediafile.web;

                if (mediafile.id_type == "1")
                    mediafile.texture = mediafile.web.texture;

                else
                    File.WriteAllBytes("E:/Test/" + mediafile.nameMedias, mediafile.web.bytes);

                Debug.Log(mediafile.nameMedias);
            }

            mediasIsDownloaded = true;
            //GetComponent<RawImage>().texture = MediaFiles[0].texture;

        }
    }


    IEnumerator PlayVideo() 
    {
        videoPlayer.playOnAwake = false;
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = "E:/Test/" + MediaFiles[indexOfMediaFile].nameMedias;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);
        videoPlayer.Prepare();
        AudioListener.volume = 1;
        //text.text = videoPlayer.url;

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        //File.WriteAllBytes("/home/platform/video.ogv",);

        GetComponent<RawImage>().texture = videoPlayer.texture;
        videoPlayer.Play();
    }

}
