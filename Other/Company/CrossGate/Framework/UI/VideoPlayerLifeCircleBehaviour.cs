using Lib.Core;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerLifeCircleBehaviour : MonoBehaviour
{
    public static readonly Lib.Core.EventEmitter<EVideoPlayerCircle> eventEmitter = new EventEmitter<EVideoPlayerCircle>();
    public VideoPlayer videoPlayer;

    private void OnEnable()
    {
        videoPlayer.started += OnGraphStart;
        videoPlayer.loopPointReached += OnGraphStop;
    }
    private void OnDisable()
    {
        videoPlayer.started -= OnGraphStart;
        videoPlayer.loopPointReached -= OnGraphStop;
    }

    public void OnGraphStart(VideoPlayer source)
    {
        if (Application.isPlaying)
        {
            eventEmitter.Trigger(EVideoPlayerCircle.OnGraphStart);
        }
    }

    public void OnGraphStop(VideoPlayer source)
    {
        if (Application.isPlaying)
        {
            eventEmitter.Trigger(EVideoPlayerCircle.OnGraphStop);
        }
    }
}
