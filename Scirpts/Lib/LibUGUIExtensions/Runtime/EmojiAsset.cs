using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Extensions
{
    [Serializable]
    public struct EmojiInfo
    {
        public uint key;
        public float x;
        public float y;
        public float size;
    }
    [Serializable]
    public class EmojiAsset : ScriptableObject
    {
        [SerializeField]
        public Material material;
        [SerializeField]
        public List<EmojiInfo> emojiList = new List<EmojiInfo>();

        private Dictionary<uint, EmojiInfo> _emojiDic = null;
        public Dictionary<uint, EmojiInfo> GetEmojiDic()
        {
            if (_emojiDic == null)
            {
                _emojiDic = new Dictionary<uint, EmojiInfo>(emojiList.Count);

                for (int i = 0; i < emojiList.Count; ++i)
                {                    
                    _emojiDic.Add(emojiList[i].key, emojiList[i]);
                }
            }
            return _emojiDic;
        }

        internal bool TryGetValue(uint key, out EmojiInfo value)
        {
            return GetEmojiDic().TryGetValue(key, out value);
        }

        public bool ContainsKey(uint key)
        {
            return GetEmojiDic().ContainsKey(key);
        }

        public int Count
        {
            get{
                return emojiList.Count;
            }
        }

        public uint GetIDByIndex(int index)
        {
            return emojiList[index].key;
        }
    }
}
