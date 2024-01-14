using System;
using UnityEngine;
using Lib.Core;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Logic.Core
{
    public abstract class SceneActor : Actor, ISceneActor
    {
        public SceneActorWrap sceneActorWrap { get; private set; }

        public GameObject gameObject { get; private set; }

        public Transform transform { get; private set; }

        public GameObject modelGameObject { get; internal set; }

        public Transform modelTransform { get; private set; }

        public GameObject mountGameObject { get; private set; }

        public Transform mountTransform { get; private set; }

        public GameObject fxRoot { get; private set; }

        public ulong UID
        {
            get
            {
                return uID;
            }
        }

        protected ELayerMask eLayerMask;
        protected ELayerMask cacheELayerMask;

        public void SetLayerHide()
        {
            transform.Setlayer(ELayerMask.HidingSceneActor);
            fxRoot.transform.Setlayer(ELayerMask.HidingSceneActor);
        }

        public void ReturnCacheLayer()
        {
            transform.Setlayer(cacheELayerMask);
            fxRoot.transform.Setlayer(cacheELayerMask);
            fxRoot.SetActive(true);
        }

        public void Show()
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
            for (int index = 0, len = renderers.Length; index < len; index++)
            {
                renderers[index].enabled = true;
            }
        }

        public void Hide()
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
            for (int index = 0, len = renderers.Length; index < len; index++)
            {
                renderers[index].enabled = false;
            }
        }

        public AsyncOperationHandle<GameObject> mHandle;

        Action<SceneActor> loadOver;

        public AssetsGroupLoader assetsGroupLoader;

        const string FXROOTNAME = "FxRoot";

        protected override void OnConstruct()
        {
            base.OnConstruct();

            gameObject = new GameObject();
            transform = gameObject.transform;
            //OnSetParent();

            fxRoot = new GameObject(FXROOTNAME);
            fxRoot.transform.SetParent(transform);
            SetGameObject();
        }

        protected virtual void SetGameObject()
        {
            gameObject.SetActive(true);
        }

        protected override void OnDispose()
        {
            assetsGroupLoader?.UnloadAll();
            assetsGroupLoader = null;

            AddressablesUtil.ReleaseInstance(ref mHandle, MHandle_Completed);

            loadOver = null;
            OnDisposeGameObject();

            sceneActorWrap = null;
            gameObject = null;
            transform = null;

            modelGameObject = null;
            modelTransform = null;

            mountGameObject = null;
            mountTransform = null;

            fxRoot = null;
            //stateComponent = null;
            //weaponComponent = null;
            //careerComponent = null;            

            base.OnDispose();
        }

        void OnDisposeGameObject()
        {
            GameObject.DestroyImmediate(gameObject);
            gameObject = null;
        }

        #region Load

        /// <summary>
        /// 使用HeroLoader的加载方式，具体逻辑实现在子类中///
        /// </summary>
        /// <param name="LoadOver"></param>
        public virtual void LoadModel(Action<SceneActor> LoadOver)
        {
            loadOver = LoadOver;
        }

        /// <summary>
        /// 直接加载模型资源的加载方式///
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="LoadOver"></param>
        public void LoadModel(string assetPath, Action<SceneActor> LoadOver)
        {
            loadOver = LoadOver;
            AddressablesUtil.InstantiateAsync(ref mHandle, assetPath, MHandle_Completed);            

            //OnSetName();
            //OnSetParent();
        }

        /// <summary>
        /// 复制已有模板的加载方式///
        /// </summary>
        /// <param name="template"></param>
        /// <param name="LoadOver"></param>
        public void LoadModel(GameObject template, Action<SceneActor> LoadOver)
        {
            loadOver = LoadOver;
            GameObject modelGameOject = FrameworkTool.CreateGameObject(template);
            //OnSetName();
            //OnSetParent();
            SetModelGameObject(modelGameOject);
        }

        #endregion

        private void MHandle_Completed(AsyncOperationHandle<GameObject> handle)
        {            
            if(handle.Status == AsyncOperationStatus.Succeeded)
            {
                SetModelGameObject(handle.Result);
            }
            else
            {
                DebugUtil.LogError("模型加载失败 策划看看 上面的报错信息里面资源名称 检查配置");
            }
        }

        /// <summary>
        /// 设置模型///
        /// </summary>
        /// <param name="modelGameObject"></param>
        public void SetModelGameObject(GameObject modelGameObject)
        {
            if (modelGameObject == null)
            {
                OnLoadModelGameObjectFailed();
            }

            BindingModelGameObject(modelGameObject);        
            OnOtherSet();            
            SetLayer(transform);
            LoadDepencyAssets();
            if (assetsGroupLoader != null)
            {
                assetsGroupLoader.StartLoad(null, () =>
                {
                    modelGameObject.SetActive(true);
                    loadOver?.Invoke(this);
                }, null);
            }
            else
            {
                modelGameObject.SetActive(true);
                loadOver?.Invoke(this);
            }

#if UNITY_EDITOR
#if !ILRUNTIME_MODE
            AddComponentView view = gameObject.GetNeedComponent<AddComponentView>();
            view.sceneActor = this;
#endif
#endif
        }

        /// <summary>
        /// 加载ModelGameObject失败///
        /// </summary>
        protected virtual void OnLoadModelGameObjectFailed()
        {
        }

        public void BindingModelGameObject(GameObject model, bool needSetModelHide = true)
        {
            modelGameObject = model;

            if (sceneActorWrap != null)
            {
                sceneActorWrap.sceneActor = null;
                sceneActorWrap = null;
            }

            if (modelGameObject != null)
            {
                modelTransform = modelGameObject.transform;

                modelTransform.SetParent(gameObject.transform, false);
                modelGameObject.transform.SetAsFirstSibling();
                modelGameObject.transform.localPosition = Vector3.zero;
                modelGameObject.transform.localRotation = Quaternion.identity;
                modelGameObject.transform.localScale = Vector3.one;

                if (modelGameObject.transform.parent != gameObject.transform)
                {
                    modelTransform.SetParent(gameObject.transform, false);
                    modelGameObject.transform.SetAsFirstSibling();
                    modelGameObject.transform.localPosition = Vector3.zero;
                    modelGameObject.transform.localRotation = Quaternion.identity;
                    modelGameObject.transform.localScale = Vector3.one;
                }

                //设置和gameobject的链接
                sceneActorWrap = model.GetComponent<SceneActorWrap>();
                if (sceneActorWrap == null)
                {
                    sceneActorWrap = model.AddComponent<SceneActorWrap>();
                }
                sceneActorWrap.sceneActor = this;
                if (needSetModelHide)
                {
                    model.SetActive(false);
                }
            }
            else
            {
                modelTransform = null;
            }
        }

        public void SetName(string name)
        {
            gameObject.name = name;
        }

        /// <summary>
        /// 设置Name///
        /// </summary>
        //protected virtual void OnSetName()
        //{
        //
        //}

        /// <summary>
        /// 设置父物体///
        /// </summary>
        //protected virtual void OnSetParent()
        //{
        //    if(mWorld != null)
        //    {
        //        SetParent(mWorld.RootTransform);
        //    }            
        //}

        /// <summary>
        /// 其它设置///
        /// </summary>
        protected virtual void OnOtherSet()
        {

        }

        /// <summary>
        /// 加载必须依赖的资源///
        /// </summary>
        protected virtual void LoadDepencyAssets()
        {

        }

        public void SetParent(Transform parent, bool stayWorldPos = true)
        {
            if (gameObject != null)
            {
                gameObject.transform.SetParent(parent, stayWorldPos);
            }
        }

        public virtual void SetLayer(Transform transform)
        {
        }

        public float DistanceTo(Transform trans)
        {
           return MathUtlilty.Distance(this.transform, trans);
        }

        public virtual void ChangeModelScale(float x, float y, float z)
        {
            if (modelTransform != null)
                modelTransform.localScale = new Vector3(x, y, z);
        }

        public virtual void ResetModelScale()
        {
            if (modelTransform != null)
                modelTransform.localScale = Vector3.one;
        }

        public static void PetWearSet(string prefix, uint suffixID, Transform transform)
        {
            string[] strs = transform.name.Split('_');
            if (strs[0] == prefix)
            {
                transform.gameObject.SetActive(uint.Parse(strs[strs.Length - 1]) == suffixID);
            }

            if (transform.childCount > 0)
            {
                for (int index = 0, len = transform.childCount; index < len; index++)
                {
                    PetWearSet(prefix, suffixID, transform.GetChild(index));
                }
            }
        }
    }
}
   