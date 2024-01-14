using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// 虚拟场景SceneActor基类///
    /// </summary>
    public abstract class VirtualSceneActor : SceneActor, IAnimatorActor, IMovementComponent
    {
        public GameObject AnimatorGameObject
        {
            get;
            set;
        }

        public AnimationComponent AnimationComponent = new AnimationComponent();
        public MovementComponent movementComponent = new MovementComponent();

        protected override void OnConstruct()
        {
            base.OnConstruct();

            AnimationComponent.actor = this;
            AnimationComponent.Construct();

            movementComponent.actor = this;
            movementComponent.Construct();
        }

        protected override void OnDispose()
        {
            AnimationComponent.Dispose();
            movementComponent.Dispose();

            AnimatorGameObject = null;
            base.OnDispose();
        }

        //protected override void OnSetParent()
        //{
        //    SetParent(GameCenter.npcRoot.transform);
        //}

        public override void SetLayer(Transform transform)
        {
            transform.Setlayer(ELayerMask.NPC);
            cacheELayerMask = ELayerMask.NPC;
        }

        protected override void OnOtherSet()
        {
            AnimatorGameObject = modelGameObject.transform.GetChild(0).gameObject;
        }

        public abstract uint ID
        {
            get;
        }

        public MovementComponent GetMovementComponent()
        {
            return movementComponent;
        }
    }
}
