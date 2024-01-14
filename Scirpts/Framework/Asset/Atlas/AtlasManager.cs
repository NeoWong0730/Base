#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.U2D;
using Lib;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

namespace Framework
{
    public static class AtlasManager
    {
#if UNITY_EDITOR
        private static string sHotFixAtlas = "ImageAtlas";
#endif

        public static void Register()
        {
#if UNITY_EDITOR
            //SpriteAtlasManager.atlasRequested += OnAtlasRequestedAsync;
#endif
        }

        public static void UnRegister()
        {
#if UNITY_EDITOR
            //SpriteAtlasManager.atlasRequested -= OnAtlasRequestedAsync;
#endif
        }

#if UNITY_EDITOR
        //private static async void OnAtlasRequestedAsync(string tag, System.Action<SpriteAtlas> action)
        //{
        //    DebugUtil.LogFormat(ELogType.eAtlas, "<color=#ff00ff>OnAtlasRequested({0}, {1}) {2}</color>", tag, action.Target, Time.frameCount.ToString());
        //
        //    //TODO:¡Ÿ ±¥¶¿Ì
        //    if (sHotFixAtlas.Equals(tag))
        //    {
        //        string assetPath = string.Format("Atlas/{0}", tag);
        //        SpriteAtlas spriteAtlas = Resources.Load<SpriteAtlas>(assetPath);
        //        action?.Invoke(spriteAtlas);
        //    }
        //    else
        //    {
        //        string assetPath = string.Format("Atlas/{0}.spriteatlas", tag);
        //
        //        AsyncOperationHandle<SpriteAtlas> atlasHandle = Addressables.LoadAssetAsync<SpriteAtlas>(assetPath);
        //        await atlasHandle.Task;
        //        action?.Invoke(atlasHandle.Result);
        //        AddressablesUtil.Release<SpriteAtlas>(ref atlasHandle, null);
        //    }
        //}
#endif
    }
}