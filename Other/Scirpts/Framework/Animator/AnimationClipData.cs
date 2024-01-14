using UnityEngine;
using System;

namespace Framework
{
    [Serializable]
    public class AnimationClipData
    {
        public string name;
        public uint layer;
        public AnimationClip clip;
    }
}
