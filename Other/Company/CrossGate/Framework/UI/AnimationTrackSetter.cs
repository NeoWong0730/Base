using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(TimelineDirector))]
[RequireComponent(typeof(PlayableDirector))]
[DisallowMultipleComponent]
public class AnimationTrackSetter : TimelineTrackSetter {
    public class AnimationInfo {
        public AnimationClip clip;
        public double speedMultify = 1f;
        public double duration = 0;

        public AnimationInfo(AnimationClip clip, double speedMultify, double duration) {
            this.clip = clip;
            this.speedMultify = speedMultify;
            this.duration = duration;
        }
    }
    public TimelineClip[] clips;
    private Dictionary<TimelineClip, AnimationInfo> dict = new Dictionary<TimelineClip, AnimationInfo>();

    // 保存
    public void Restore() {
        //Debug.LogError("查错日志，防止过滤 Restore");
        dict.Clear();
        for (int i = 0, length = clips.Length; i < length; ++i) {
            AnimationPlayableAsset animationPlayableAsset = clips[i].asset as AnimationPlayableAsset;
            AnimationInfo info = new AnimationInfo(animationPlayableAsset.clip, 1f, clips[i].duration);
            dict.Add(clips[i], info);
        }
    }
    // 还原
    public void Recovery() {
        if (clips == null) {
            return;
        }
        //Debug.LogError("查错日志，防止过滤 Recovery");
        for (int i = 0, length = clips.Length; i < length; ++i) {
            AnimationPlayableAsset animationPlayableAsset = clips[i].asset as AnimationPlayableAsset;
            if (animationPlayableAsset != null && dict.TryGetValue(clips[i], out AnimationInfo info)) {
                animationPlayableAsset.clip = info.clip;
                clips[i].timeScale = info.speedMultify;
            }
        }
    }

    public void SetClip(string trackName, int clipIndex, AnimationClip targetClip, bool includeSpeedMultify = true) {
        if (clips == null) {
            clips = timelineDirector.GetBindingTrackClips<AnimationTrack>(trackName);
        }
        SetClip(clipIndex, targetClip, includeSpeedMultify);
    }
    public void SetClip(int clipIndex, AnimationClip targetClip, bool includeSpeedMultify = true) {
        if (0 <= clipIndex && clipIndex < clips.Length)
        {
            AnimationPlayableAsset animationPlayableAsset = clips[clipIndex].asset as AnimationPlayableAsset;
            animationPlayableAsset.clip = targetClip;
            
            if(includeSpeedMultify && !targetClip.isLooping)
                clips[clipIndex].timeScale = targetClip.length / dict[clips[clipIndex]].duration;
        }
    }
}
