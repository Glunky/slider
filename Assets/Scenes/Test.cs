//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Video;

//public class Test : MonoBehaviour
//{

//    VideoPlayer videoPlayer;
//    VideoSource videoSource;
//    AudioSource audioSource;
//    Texture2D texture;

//    // Start is called before the first frame update
//    IEnumerator Start()
//    {
//        /*//file:////home/ilya/Downloads/resources/test.webm
//        ///"/home/ilya/Downloads/test.ogv"
//        videoPlayer = gameObject.AddComponent<VideoPlayer>();  
//        audioSource = gameObject.AddComponent<AudioSource>();

//        AudioListener.volume = 1;
//        //videoPlayer.playOnAwake = false;
//        videoPlayer.source = VideoSource.Url;
//        videoPlayer.url = "https://people.xiph.org/~greg/video/ytcompare/bbb_theora_486kbit.ogv";
//        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
//        //videoPlayer.EnableAudioTrack(0, true);
//        //videoPlayer.SetTargetAudioSource(0, audioSource);
//        videoPlayer.Prepare();
//        text.text = videoPlayer.url;

//        while (!videoPlayer.isPrepared)
//        {
//            yield return null;
//        }

//        File.WriteAllBytes("/home/platform/video.ogv",);

//        GetComponent<RawImage>().texture = videoPlayer.texture;
//        videoPlayer.Play();
//        audioSource.Play();*/


//        WWW[] www = new WWW[2];

//        www[1] = new WWW("https://people.xiph.org/~greg/video/ytcompare/bbb_theora_486kbit.ogv");
//        www[0] = new WWW("https://pngimage.net/wp-content/uploads/2018/06/images-with-png-extension.png");

//        for (int i = 0; i < 2; i++)
//        {
//            yield return www[i];
//        }

//        File.WriteAllBytes("/home/platform/video.ogv", www[0].bytes);
//        File.WriteAllBytes("/home/platform/video.ogv", www[1].bytes);

//        videoPlayer = gameObject.AddComponent<VideoPlayer>();
//        audioSource = gameObject.AddComponent<AudioSource>();

//        texture = www[0].texture;
//        //Texture2D m = www.texture;
//        //Debug.Log(www.url);
//        //this.GetComponent<RawImage>().texture = m;



//        //text.text = Application.persistentDataPath;
//    }

//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.A))
//        {
//            StartCoroutine(playVideo());
//        }

//        if (Input.GetKeyDown(KeyCode.D))
//        {
//            showImage();
//        }
//    }

//    private IEnumerator playVideo()
//    {
//        videoPlayer.playOnAwake = false;
//        videoPlayer.source = VideoSource.Url;
//        videoPlayer.url = "/home/platform/video.ogv";
//        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
//        videoPlayer.EnableAudioTrack(0, true);
//        videoPlayer.SetTargetAudioSource(0, audioSource);
//        videoPlayer.Prepare();
//        //text.text = videoPlayer.url;

//        while (!videoPlayer.isPrepared)
//        {
//            yield return null;
//        }

//        //File.WriteAllBytes("/home/platform/video.ogv",);

//        GetComponent<RawImage>().texture = videoPlayer.texture;
//        videoPlayer.Play();
//        audioSource.Play();
//    }

//    private void showImage()
//    {
//        this.GetComponent<RawImage>().texture = texture;
//    }
//}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.IO;

public class Test : MonoBehaviour
{


    VideoPlayer videoPlayer;
    VideoSource videoSource;
    AudioSource audioSource;
    public Text text;

    // Use this for initialization
    IEnumerator Start()
    {
        /*videoPlayer = gameObject.AddComponent<VideoPlayer>();
        audioSource = gameObject.AddComponent<AudioSource>();

        videoPlayer.playOnAwake = false;
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = "https://dl3.webmfiles.org/big-buck-bunny_trailer.webm";
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);
        videoPlayer.Prepare();
        //text.text = videoPlayer.url;

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        //File.WriteAllBytes("/home/platform/video.ogv",);

        GetComponent<RawImage>().texture = videoPlayer.texture;
        videoPlayer.Play();
        audioSource.Play();*/


        WWW www = new WWW ("https://dl3.webmfiles.org/big-buck-bunny_trailer.webm");
        yield return www;

        File.WriteAllBytes("/home/platform/w.webm", www.bytes);

        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        audioSource = gameObject.AddComponent<AudioSource>();
         
        videoPlayer.playOnAwake = false;
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = "/home/platform/w.webm";
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);
        videoPlayer.Prepare();
        //text.text = videoPlayer.url;

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        //File.WriteAllBytes("/home/platform/video.ogv",);

        GetComponent<RawImage>().texture = videoPlayer.texture;
        videoPlayer.Play();
        audioSource.Play();
        //Texture2D texture = www.texture;
        //this.GetComponent<RawImage> ().texture = texture;
    }

    // Update is called once per frame
    void Update()
    {
       // StartCoroutine(playVideo());
    }

    IEnumerator playVideo()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            WWW www = new WWW("file:///home/platform/image.png");
            yield return www;

            this.GetComponent<RawImage>().texture = www.texture;
        }
    }
}