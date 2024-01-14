using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    [DisallowMultipleComponent]
    public class AudioCollectorComponent : MonoBehaviour
    {
        [Header("Dont drag")]
        [SerializeField]
        public readonly List<AudioSourceNode> ls = new List<AudioSourceNode>();
        public Func<uint, float> onGetVoloume;

        public static AudioCollectorComponent instance;

        private void Awake()
        {
            instance = this;
        }

        public void Register(AudioSourceNode node, bool toRegister)
        {
            if (toRegister)
            {
                if (!ls.Contains(node))
                {
                    ls.Add(node);
                }
            }
            else
            {
                ls.Remove(node);
            }
        }

        public void SetVolume(uint audioType, float v)
        {
            for (int i = 0, length = ls.Count; i < length; ++i)
            {
                if (ls[i] != null && ls[i].audioType == audioType)
                {
                    if (ls[i].audioSource != null)
                    {
                        ls[i].audioSource.volume = v;
                    }

#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
                     if (ls[i].phaseAudioSource != null)
                    {
                        var id = ls[i].phaseAudioSource.GetSourceId();
                        Helpers.PHASEAdjustVolume(id, v);
                    }
#endif
                }
            }
        }
    }
}
