using Lib.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework
{
    public class ShaderManager
    {        
        //private static AsyncOperationHandle<ShaderVariantCollection> _shaderVariantCollectionHandle;
        private static AsyncOperationHandle<IList<Shader>> _shadersHandle;
        //private static ShaderVariantCollection _shaderVariantCollection = null;
        private static Dictionary<string, Shader> _shaders = null;

        public static bool bLoaded
        {
            get
            {
                return _shadersHandle.IsDone;// && _shaderVariantCollectionHandle.IsDone;
            }            
        }

        public static void StartLoad()
        {
            string assetPath = "shader";            

            //_shaderVariantCollectionHandle = Addressables.LoadAssetAsync<ShaderVariantCollection>(assetPath);
            //_shaderVariantCollectionHandle.Completed += OnShaderVariantCollectionCompleted;

            AssetLabelReference assetLabel = new AssetLabelReference();
            assetLabel.labelString = assetPath;
            _shadersHandle = Addressables.LoadAssetsAsync<Shader>(assetLabel, null);
            _shadersHandle.Completed += OnShaderCompleted;
        }

        private static void OnShaderCompleted(AsyncOperationHandle<IList<Shader>> obj)
        {
            IList<Shader> shaders = _shadersHandle.Result;

            if (_shaders == null)
            {
                _shaders = new Dictionary<string, Shader>(shaders.Count);
            }
            else
            {
                _shaders.Clear();
            }

            foreach (Shader shader in shaders)
            {
                _shaders[shader.name] = shader;
            }
        }

        /*
        private static void OnShaderVariantCollectionCompleted(AsyncOperationHandle<ShaderVariantCollection> obj)
        {
            _shaderVariantCollection = _shaderVariantCollectionHandle.Result;            
        }
        */

        /// <summary>
        /// 通过shader名获取shader
        /// </summary>
        /// <param name="name">shader name</param>
        /// <returns></returns>
        public static Shader Find(string shaderName)
        {
            Shader shader = null;

            if(_shaders != null)
            {
                _shaders.TryGetValue(shaderName, out shader);
            }
        
            if (shader == null)
            {
                shader = Shader.Find(shaderName);
#if UNITY_EDITOR
                if (shader != null)
                {
                    DebugUtil.LogWarningFormat("{0} 是通过Shader.Find找到的", shaderName);
                }
#endif
            }

            if(shader == null)
            {
                DebugUtil.LogErrorFormat("{0} 未打包", shaderName);
            }

            return shader;
        }

        public static void UnInit()
        {
            Addressables.Release(_shadersHandle);
            //Addressables.Release(_shaderVariantCollectionHandle);
        }

        public static void WarmUp()
        {
            /*
            if (_shaderVariantCollection == null || _shaderVariantCollection.isWarmedUp)
                return;

            float start = Time.realtimeSinceStartup;
            _shaderVariantCollection.WarmUp();

            Debug.LogFormat("{0} : Shader Count = {1}  Variant Count = {2}", _shaderVariantCollection.name, _shaderVariantCollection.shaderCount, _shaderVariantCollection.variantCount);
            Debug.LogFormat("Shader WarmUp 费时 {0}", (Time.realtimeSinceStartup - start).ToString());
            */
        }
    }
}
