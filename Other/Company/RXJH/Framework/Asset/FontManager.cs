using UI.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework
{
    public class FontManager
    {
        public const string sFontPathFormat = "Font/{0}";
        public const string sEmojiPathFormat = "Emoji/{0}.asset";

        private static readonly Dictionary<string, AsyncOperationHandle<Font>> mFonts = new Dictionary<string, AsyncOperationHandle<Font>>(8);
        private static readonly Dictionary<string, AsyncOperationHandle<EmojiAsset>> mEmojis = new Dictionary<string, AsyncOperationHandle<EmojiAsset>>(2);

        public static Font GetFont(string name)
        {
            if (!mFonts.TryGetValue(name, out AsyncOperationHandle<Font> handle))
            {
                string assetPath = string.Format(sFontPathFormat, name);
                handle = Addressables.LoadAssetAsync<Font>(assetPath);

                if (!handle.IsDone)
                {
                    Debug.LogWarningFormat("资源阻塞加载 {0}", assetPath);
                    handle.WaitForCompletion();
                }

                mFonts[name] = handle;
            }
            return handle.Result;
        }

        public static EmojiAsset GetEmoji(string name)
        {            
            if (!mEmojis.TryGetValue(name, out AsyncOperationHandle<EmojiAsset> handle))
            {
                string assetPath = string.Format(sEmojiPathFormat, name);
                handle = Addressables.LoadAssetAsync<EmojiAsset>(assetPath);

                if (!handle.IsDone)
                {
                    Debug.LogWarningFormat("资源阻塞加载 {0}", assetPath);
                    handle.WaitForCompletion();
                }

                mEmojis[name] = handle;
            }
            return handle.Result;
        }

        public static void UnInit()
        {
            foreach (var font in mFonts)
            {
                AsyncOperationHandle<Font> handle = font.Value;
                Addressables.Release<Font>(handle);
            }
            mFonts.Clear();

            foreach (var emoji in mEmojis)
            {
                AsyncOperationHandle<EmojiAsset> handle = emoji.Value;
                Addressables.Release<EmojiAsset>(handle);
            }
            mEmojis.Clear();
        }
    }
}


