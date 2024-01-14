using System;
using UnityEngine;

// 动画状态的监听器
[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class AnimationEndTrigger : MonoBehaviour
{
    public Action<string> onAnimationStart;
    public Action<string> onAnimationEnd;

    // 动画播放开始的回调
    private void OnAnimationStart(string stateName) {
        if (Application.isPlaying) {
            onAnimationStart?.Invoke(stateName);
        }
    }
    // 动画播放结束的回调
    private void OnAnimationEnd(string stateName) {
        if (Application.isPlaying) {
            onAnimationEnd?.Invoke(stateName);
        }
    }
    public void Clear() {
        onAnimationStart = null;
        onAnimationEnd = null;
    }
}
