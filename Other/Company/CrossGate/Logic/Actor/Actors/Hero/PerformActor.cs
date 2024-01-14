using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.AI;

namespace Logic
{
    // 用于创角表演的Actor
    public class PerformActor : SceneActor, IAnimatorActor
    {
        public GameObject AnimatorGameObject { get; set; }

        public AnimationComponent animationComponent = new AnimationComponent();

        protected override void OnConstruct()
        {
            base.OnConstruct();

            animationComponent.actor = this;
            animationComponent.Construct();
        }

        protected override void OnDispose()
        {
            animationComponent.Dispose();

            base.OnDispose();
        }

        protected override void OnOtherSet()
        {
            AnimatorGameObject = modelGameObject.transform.GetChild(0).gameObject;
        }
    }

    public class ShowParnter : SceneActor, IAnimatorActor
    {        
        public GameObject AnimatorGameObject { get; set; }

        public CSVPartner.Data cSVPartnerData
        {
            get;
            set;
        }

        public uint WeaponID
        {
            get;
            set;
        } = Constants.UMARMEDID;

        public AnimationComponent animationComponent = new AnimationComponent();

        protected override void OnConstruct()
        {
            base.OnConstruct();

            animationComponent.actor = this;
            animationComponent.Construct();
        }

        protected override void OnDispose()
        {
            animationComponent.Dispose();

            cSVPartnerData = null;
            WeaponID = Constants.UMARMEDID;

            base.OnDispose();
        }

        protected override void SetGameObject()
        {
            NavMeshAgent navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = false;
            }
            base.SetGameObject();
        }

        protected override void OnOtherSet()
        {
            AnimatorGameObject = modelGameObject.transform.GetChild(0).gameObject;
        }

        //protected override void OnSetName()
        //{
        //    gameObject.name = $"ShowParnter_{uID.ToString()}";
        //}

        public override void SetLayer(Transform transform)
        {
            transform.Setlayer(ELayerMask.ModelShow);
            cacheELayerMask = ELayerMask.ModelShow;
        }

        protected override void LoadDepencyAssets()
        {
            assetsGroupLoader = new AssetsGroupLoader();

            List<string> animationPaths;
            AnimationComponent.GetAnimationPaths(cSVPartnerData.id + 100, WeaponID, out animationPaths, Constants.UIModelShowAnimationClipHashSet);
            if (animationPaths != null)
            {
                for (int index = 0, len = animationPaths.Count; index < len; index++)
                {
                    assetsGroupLoader.AddLoadTask(animationPaths[index]);
                }
            }
            else
            {
                DebugUtil.LogError($"animationPaths is null cSVPartnerData.id: {cSVPartnerData.id}");
            }
        }
    }


    public class ShowBrave : SceneActor, IAnimatorActor
    {       
        public GameObject AnimatorGameObject { get; set; }

        public CSVBrave.Data cSVBraveData
        {
            get;
            set;
        }

        public uint WeaponID
        {
            get;
            set;
        } = Constants.UMARMEDID;

        public AnimationComponent animationComponent = new AnimationComponent();

        protected override void OnConstruct()
        {
            base.OnConstruct();

            animationComponent.actor = this;
            animationComponent.Construct();
        }

        protected override void OnDispose()
        {
            animationComponent.Dispose();
            cSVBraveData = null;
            WeaponID = Constants.UMARMEDID;

            base.OnDispose();
        }

        protected override void SetGameObject()
        {
            NavMeshAgent navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = false;
            }
            base.SetGameObject();
        }

        protected override void OnOtherSet()
        {
            AnimatorGameObject = modelGameObject.transform.GetChild(0).gameObject;
        }

        //protected override void OnSetName()
        //{
        //    gameObject.name = $"ShowBrave_{uID.ToString()}";
        //}

        public override void SetLayer(Transform transform)
        {
            transform.Setlayer(ELayerMask.ModelShow);
            cacheELayerMask = ELayerMask.ModelShow;
        }

        protected override void LoadDepencyAssets()
        {
            assetsGroupLoader = new AssetsGroupLoader();

            List<string> animationPaths;
            AnimationComponent.GetAnimationPaths(cSVBraveData.id, WeaponID, out animationPaths, Constants.UIModelShowAnimationClipHashSet);
            if (animationPaths != null)
            {
                for (int index = 0, len = animationPaths.Count; index < len; index++)
                {
                    assetsGroupLoader.AddLoadTask(animationPaths[index]);
                }
            }
            else
            {
                DebugUtil.LogError($"animationPaths is null cSVBraveData.id: {cSVBraveData.id}");
            }
        }
    }
}
