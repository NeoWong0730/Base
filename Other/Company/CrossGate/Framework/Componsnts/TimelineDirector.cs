using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// https://forum.unity.com/threads/change-animationtrack-clip-at-runtime.545671/
// https://answers.unity.com/questions/1506041/dynamically-change-animations-in-timeline.html
[RequireComponent(typeof(PlayableDirector))]
[DisallowMultipleComponent]
public class TimelineDirector : MonoBehaviour
{
    public PlayableDirector playableDirector;
    private readonly Dictionary<string, PlayableBinding> bindings = new Dictionary<string, PlayableBinding>();

    public static readonly TimelineClip[] ZERO_CLIPS = new TimelineClip[0];

    private void Awake()
    {
        if (playableDirector == null)
        {
            playableDirector = GetComponent<PlayableDirector>();
        }
    }
    public void RebuildGraphAndPlay()
    {
        playableDirector.RebuildGraph();
        playableDirector.Play();
    }
    public void Collect()
    {
        bindings.Clear();
        foreach (PlayableBinding binding in playableDirector.playableAsset.outputs)
        {
            string trackName = binding.streamName;
            if (!bindings.ContainsKey(trackName))
            {
                bindings.Add(trackName, binding);
            }
        }
    }

    public void SetBinding(string trackName, UnityEngine.Object o)
    {
        PlayableBinding binding = default;
        if (!string.IsNullOrEmpty(trackName) && bindings.TryGetValue(trackName, out binding))
        {
            playableDirector.SetGenericBinding(binding.sourceObject, o);
        }
    }
    public void Pause() {
        if (Application.isPlaying) {
            playableDirector.Pause();
        }
    }
    public void Resume() {
        if (Application.isPlaying) {
            playableDirector.Resume();
        }
    }
    public bool Exist(string trackName)
    {
        return Exist(trackName, out PlayableBinding binding);
    }
    public bool Exist(string trackName, out PlayableBinding binding)
    {
        bool ret = false;
        if (!string.IsNullOrEmpty(trackName) && bindings.TryGetValue(trackName, out binding))
        {
            ret = true;
        }
        return ret;
    }
    public T GetBinding<T>(string trackName) where T : TrackAsset
    {
        T ret = default;
        if (!string.IsNullOrEmpty(trackName) && bindings.TryGetValue(trackName, out PlayableBinding binding))
        {
            ret = binding.sourceObject as T;
        }
        return ret;
    }
    public TimelineClip[] GetBindingTrackClips<T>(string trackName) where T : TrackAsset
    {
        T at = GetBinding<T>(trackName);
        if (at == null) {
            return ZERO_CLIPS;
        }
        TimelineClip[] rlt = at.GetClips() as TimelineClip[];
        Array.Sort<TimelineClip>(rlt, (TimelineClip left, TimelineClip right)=> {
            return (int)(left.start - right.start);
        });
        return rlt;
    }
}
