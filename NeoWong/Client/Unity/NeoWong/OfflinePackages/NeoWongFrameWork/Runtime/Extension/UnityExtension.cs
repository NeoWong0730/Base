using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngineInternal;

namespace NWFramework
{
    /// <summary>
    /// Unity 的扩展方法辅助类
    /// </summary>
    public static class UnityExtension
    {
        /// <summary>
        /// 获取或增加组件
        /// </summary>
        /// <typeparam name="T">要获取或增加的组件</typeparam>
        /// <param name="gameObject">目标对象</param>
        /// <returns>获取或增加的组件</returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();

            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }


            return component;
        }

        /// <summary>
        /// 获取或增加组件
        /// </summary>
        /// <param name="gameObject">目标对象</param>
        /// <param name="type">要获取或增加的组件类型</param>
        /// <returns>获取或增加的组件</returns>
        public static Component GetOrAddComponent(this GameObject gameObject, Type type)
        {
            Component component = gameObject.GetComponent(type);

            if (component == null)
            {
                component = gameObject.AddComponent(type);
            }

            return component;
        }

        /// <summary>
        /// 移除组件
        /// </summary>
        /// <param name="gameObject">目标对象</param>
        /// <param name="type">要获取或增加的组件类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        [TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
        public static void RemoveMonoBehaviour(this GameObject gameObject, Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            Component component = gameObject.GetComponent(type);

            if (component != null)
            {
                UnityEngine.Object.Destroy(component);
            }
        }

        /// <summary>
        /// 移除组件
        /// </summary>
        /// <param name="gameObject">目标对象</param>
        /// <typeparam name="T">要获取或增加的组件类型</typeparam>
        public static void RemoveMonoBehaviour<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();

            if (component != null)
            {
                UnityEngine.Object.Destroy(component);
            }
        }

        /// <summary>
        /// 获取 GameObject 是否在场景中
        /// </summary>
        /// <param name="gameObject">目标对象</param>
        /// <returns>GameObject 是否在场景中</returns>
        /// <remarks>若返回 true，表明此 GameObject 是一个场景中的实例对象；若返回 false，表明此 GameObject 是一个 Prefab。</remarks>
        public static bool InScene(this GameObject gameObject)
        {
            return gameObject.scene.name != null;
        }

        private static readonly List<Transform> CachedTransforms = new List<Transform>();

        /// <summary>
        /// 递归设置游戏对象的层次
        /// </summary>
        /// <param name="gameObject"><see cref="GameObject" />对象</param>
        /// <param name="layer">目标层次的编号</param>
        public static void SetLayerRecursively(this GameObject gameObject, int layer)
        {
            gameObject.GetComponentsInChildren(true, CachedTransforms);
            for (int i = 0; i < CachedTransforms.Count; i++)
            {
                CachedTransforms[i].gameObject.layer = layer;
            }

            CachedTransforms.Clear();
        }

        /// <summary>
        /// ActivatesDeactivates the GameObject, depending on the given true or false/ value.
        /// </summary>
        /// <param name="go">GameObject</param>
        /// <param name="value">Activate or deactivate the object, where true activates the GameObject and false deactivates the GameObject</param>
        /// <param name="cacheValue">Cache Activate or deactivate the object, where true activates the GameObject and false deactivates the GameObject</param>
        public static void SetActive(this GameObject go, bool value, ref bool cacheValue)
        {
            if (go != null && value != cacheValue)
            {
                cacheValue = value;
                go.SetActive(value);
            }
        }

        /// <summary>
        /// 设置Image组件的图片资源
        /// </summary>
        /// <param name="image">Image组件</param>
        /// <param name="spriteName">图片名称</param>
        /// <param name="isSetNativeSize">是否使用原生分辨率</param>
        /// <param name="isAsync">是否使用异步加载</param>
        /// <param name="customPackageName">指定资源包的名称。不传使用默认资源包</param>
        public static void SetSprite(this UnityEngine.UI.Image image, string spriteName, bool isSetNativeSize = false,
            bool isAsync = false, string customPackageName = "")
        {
            if (image == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(spriteName))
            {
                image.sprite = null;
            }
            else
            {
                if (!isAsync)
                {
                    var operation = GameModule.Resource.LoadAssetGetOperation<Sprite>(spriteName, customPackageName: customPackageName);
                    image.sprite = operation.AssetObject as Sprite;
                    if (isSetNativeSize)
                    {
                        image.SetNativeSize();
                    }

                    image.gameObject.GetOrAddComponent<AssetReference>().Reference(operation, spriteName);
                }
                else
                {
                    GameModule.Resource.LoadAssetAsync<Sprite>(spriteName, operation =>
                    {
                        if (image == null)
                        {
                            operation.Dispose();
                            operation = null;
                            return;
                        }

                        image.sprite = operation.AssetObject as Sprite;
                        if (isSetNativeSize)
                        {
                            image.SetNativeSize();
                        }
                        image.gameObject.GetOrAddComponent<AssetReference>().Reference(operation, spriteName);
                    }, customPackageName: customPackageName);
                }
            }
        }

        /// <summary>
        /// 设置SpriteRenderer组件的精灵资源
        /// </summary>
        /// <param name="spriteRenderer">Image组件</param>
        /// <param name="spriteName">图片名称</param>
        /// <param name="isAsync">是否使用异步加载</param>
        /// <param name="customPackageName">指定资源包的名称。不传使用默认资源包</param>
        public static void SetSprite(this SpriteRenderer spriteRenderer, string spriteName, bool isAsync = false,
            string customPackageName = "")
        {
            if (spriteRenderer == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(spriteName))
            {
                spriteRenderer.sprite = null;
            }
            else
            {
                if (!isAsync)
                {
                    var operation = GameModule.Resource.LoadAssetGetOperation<Sprite>(spriteName, customPackageName: customPackageName);
                    spriteRenderer.sprite = operation.AssetObject as Sprite;

                    spriteRenderer.gameObject.GetOrAddComponent<AssetReference>().Reference(operation, spriteName);
                }
                else
                {
                    GameModule.Resource.LoadAssetAsync<Sprite>(spriteName, operation =>
                    {
                        if (spriteRenderer == null)
                        {
                            operation.Dispose();
                            operation = null;
                            return;
                        }

                        spriteRenderer.sprite = operation.AssetObject as Sprite;
                        spriteRenderer.gameObject.GetOrAddComponent<AssetReference>().Reference(operation, spriteName);
                    }, customPackageName: customPackageName);
                }
            }
        }

        /// <summary>
        /// 查找子节点
        /// </summary>
        /// <param name="transform">位置组件</param>
        /// <param name="path">子节点路径</param>
        /// <returns>位置组件</returns>
        public static Transform FindChild(this Transform transform, string path)
        {
            var findTrans = transform.Find(path);
            return findTrans != null ? findTrans : null;
        }

        /// <summary>
        /// 根据名字找到子节点，主要用于dummy接口
        /// </summary>
        /// <param name="transform">位置组件</param>
        /// <param name="name">子节点名字</param>
        /// <returns>位置组件</returns>
        public static Transform FindChildByName(this Transform transform, string name)
        {
            if (transform == null)
            {
                return null;
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                var childTrans = transform.GetChild(i);
                if (childTrans.name == name)
                {
                    return childTrans;
                }

                var find = FindChildByName(childTrans, name);
                if (find != null)
                {
                    return find;
                }
            }

            return null;
        }

        [TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
        public static Component FindChildComponent(this Type type, Transform transform, string path)
        {
            var findTrans = transform.Find(path);
            if (findTrans != null)
            {
                return findTrans.gameObject.GetComponent(type);
            }

            return null;
        }

        public static T FindChildComponent<T>(this Transform transform, string path) where T : Component
        {
            var findTrans = transform.Find(path);
            if (findTrans != null)
            {
                return findTrans.gameObject.GetComponent<T>();
            }

            return null;
        }

        public static AssetReference GetAssetReference(this GameObject gameObject)
        {
            if (gameObject == null)
            {
                return null;
            }

            return gameObject.GetComponent<AssetReference>();
        }

        /// <summary>
        /// 加载资源并绑定资源引用到GameObject上
        /// </summary>
        /// <param name="gameObject">GameObject</param>
        /// <param name="location">资源定位地址</param>
        /// <param name="callBack">加载完成回调</param>
        /// <param name="customPackageName">自定义包</param>
        /// <typeparam name="T">资源实例类型</typeparam>
        /// <returns>资源实例</returns>
        public static T LoadAsset<T>(this GameObject gameObject, string location, Action<T> callBack = null,
            string customPackageName = "") where T : UnityEngine.Object
        {
            if (gameObject == null)
            {
                return null;
            }

            var operation = GameModule.Resource.LoadAssetGetOperation<T>(location, customPackageName: customPackageName);
            var asset = operation.AssetObject as T;
            gameObject.GetOrAddComponent<AssetReference>().Reference(operation, location);
            callBack?.Invoke(asset);
            return asset;
        }

        /// <summary>
        /// 异步加载资源并绑定资源引用到GameObject上
        /// </summary>
        /// <param name="gameObject">GameObject</param>
        /// <param name="location">资源定位地址</param>
        /// <param name="callBack">加载完成回调</param>
        /// <param name="customPackageName">自定义包</param>
        /// <typeparam name="T">资源实例类型</typeparam>
        /// <returns>资源实例</returns>
        public static async UniTask<T> LoadAssetAsync<T>(this GameObject gameObject, string location, Action<T> callBack = null,
            string customPackageName = "") where T : UnityEngine.Object
        {
            if (gameObject == null)
            {
                return null;
            }

            var operation = GameModule.Resource.LoadAssetAsyncHandle<T>(location, customPackageName: customPackageName);

            bool cancelOrFailed = await operation.ToUniTask().AttachExternalCancellation(gameObject.GetCancellationTokenOnDestroy())
                .SuppressCancellationThrow();

            if (cancelOrFailed)
            {
                return null;
            }

            gameObject.GetOrAddComponent<AssetReference>().Reference(operation, location);
            var asset = operation.AssetObject as T;
            callBack?.Invoke(asset);
            return asset;
        }
    }
}