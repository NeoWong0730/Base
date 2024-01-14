//#define USE_AV_PRO

using System.IO;
using UnityEngine;

#if USE_AV_PRO
using RenderHeads.Media.AVProVideo;
#else
using UnityEngine.Video;
#endif

public static class VideoManager
{
    static private GameObject mRoot = null;
#if USE_AV_PRO
    static private MediaPlayer mMediaPlayer = null;

    public static void Init(GameObject root)
    {
        mRoot = root;
        mMediaPlayer = mRoot.GetComponent<MediaPlayer>();
        mMediaPlayer.PauseMediaOnAppPause = true;
        mMediaPlayer.PlayMediaOnAppUnpause = true;

        mRoot.SetActive(false);
    }

    public static bool Play(string url, bool loop)
    {
        mRoot.SetActive(true);

        if (!mMediaPlayer.MediaOpened || !string.Equals(mMediaPlayer.MediaPath.Path, url) || mMediaPlayer.MediaPath.PathType != MediaPathType.AbsolutePathOrURL)
        {
            mMediaPlayer.OpenMedia(MediaPathType.AbsolutePathOrURL, url, true);
        }

        mMediaPlayer.Loop = loop;

        return true;
    }

    public static void Stop()
    {
        mMediaPlayer.Stop();
        mMediaPlayer.CloseMedia();        
        mRoot.SetActive(false);
    }

    public static bool isPlaying
    {
        get
        {
            return mMediaPlayer.Control.IsPlaying();
        }
    }
#else    
    static private VideoPlayer mVideoPlayer = null;

    static float fVolume = 0;

    //static public MediaPlayerEvent Events
    //{
    //    get
    //    {
    //        return mMediaPlayer.Events;
    //    }
    //}

    public static void Init(GameObject root)
    {
        //mRoot = Resources.Load("VideoPlayer") as GameObject;
        //mRoot = Object.Instantiate(mRoot);
        mRoot = root;
        mVideoPlayer = mRoot.GetComponent<VideoPlayer>();
        //mMediaPlayer = mRoot.transform.Find("AVPro Video").GetComponent<MediaPlayer>();
        //mMediaPlayer.PauseMediaOnAppPause = true;
        //mMediaPlayer.PlayMediaOnAppUnpause = true;

        SetVolume(0);

        mRoot.SetActive(false);
    }

    public static void SetVolume(float volume)
    {
        fVolume = volume;
        for (ushort i = 0, len = mVideoPlayer.controlledAudioTrackCount; i < len; ++i)
        {
            Debug.Log("volume = " + fVolume.ToString());
            mVideoPlayer.SetDirectAudioMute(i, fVolume < 0.01f);
            mVideoPlayer.SetDirectAudioVolume(i, fVolume);
        }
    }

    public static bool Play(string url, bool loop)
    {
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
        url = Path.ChangeExtension(url, ".webm");
#endif
        if (mRoot.activeSelf && mVideoPlayer.isPlaying && string.Equals(mVideoPlayer.url, url, System.StringComparison.Ordinal))
            return true;

        mVideoPlayer.url = url;
        mRoot.SetActive(true);
        mVideoPlayer.isLooping = loop;
        SetVolume(fVolume);
        mVideoPlayer.Play();
        return true;
    }

    public static void Stop()
    {
        mVideoPlayer.Stop();
        mRoot.SetActive(false);
    }

    public static bool isPlaying
    {
        get
        {
            return mVideoPlayer.isPlaying;
        }
    }
#endif
    }
