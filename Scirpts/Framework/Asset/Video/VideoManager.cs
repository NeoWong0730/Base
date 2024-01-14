using UnityEngine;
using UnityEngine.Video;
using Lib;

namespace Framework
{
    public static class VideoManager
    {
        private static GameObject mRoot = null;
        private static VideoPlayer mVideoPlayer = null;
        private static float fVolume = 0f;

        public static void Init(GameObject root)
        {
            mRoot = root;
            mVideoPlayer = mRoot.GetComponent<VideoPlayer>();
            SetVolume(0);
            mRoot.SetActive(false);
        }

        public static void SetVolume(float volume)
        {
            fVolume = volume;
            for (ushort i = 0, len = mVideoPlayer.controlledAudioTrackCount; i < len; ++i)
            {
                DebugUtil.Log(ELogType.eNone, "volume = " + fVolume.ToString());
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
    }
}
