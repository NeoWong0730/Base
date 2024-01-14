using System.Collections.Generic;
using UnityEngine;
using System;

namespace Framework
{
    [Serializable]
    public class AnimationClipListData : ScriptableObject
    {
        [SerializeField]
        public uint id;

        [SerializeField]
        public List<AnimationClipData> animationClipDatas;

        public Dictionary<string, AnimationClipData> animationClipDatasDict;

        public AnimationClipListData()
        {
            animationClipDatas = new List<AnimationClipData>();
            animationClipDatasDict = new Dictionary<string, AnimationClipData>();
        }
    }
}
