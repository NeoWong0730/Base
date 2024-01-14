using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
using Apple.PHASE;
#endif

[DisallowMultipleComponent]
public class CP_AudioCollector : MonoBehaviour {
    [Header("别拖拽")] [SerializeField] public readonly List<AudioSourceNode> ls = new List<AudioSourceNode>();
    public Func<uint, float> onGetVolume;

    public static CP_AudioCollector instance;

    private void Awake() {
        instance = this;
    }

    public void Register(AudioSourceNode node, bool toRegister) {
        if (toRegister) {
            if (!this.ls.Contains(node)) {
                this.ls.Add(node);
            }
        }
        else {
            this.ls.Remove(node);
        }
    }

    public void SetVolume(uint audioType, float v) {
        for (int i = 0, length = this.ls.Count; i < length; ++i) {
            if (this.ls[i] != null && this.ls[i].audioType == audioType) {
                if (this.ls[i].audioSource != null) {
                    this.ls[i].audioSource.volume = v;
                }

#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
                if (this.ls[i].phaseAudioSource != null)
                {
                    // set volume必须要在PHASECreateSource之后
                    var id = this.ls[i].phaseAudioSource.GetSourceId();
                    Helpers.PHASEAdjustVolume(id, v);
                }
#endif
            }
        }
    }
}