using Logic.Core;
using Packet;
using UnityEngine;
using UnityEngine.AI;

namespace Logic
{
    public abstract class FightActor : SceneActor, IAnimatorActor, IClick, IDoubleClick, ILongPress
    {        
        public ClickComponent clickComponent = new ClickComponent();
        public DoubleClickComponent doubleClickComponent = new DoubleClickComponent();
        public LongPressComponent longpressComponent = new LongPressComponent();

        public GameObject AnimatorGameObject { get; set; }

        public BattleUnit battleUnit
        {
            get;
            set;
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();

            clickComponent.actor = this;
            clickComponent.Construct();

            doubleClickComponent.actor = this;
            doubleClickComponent.Construct();

            longpressComponent.actor = this;
            longpressComponent.Construct();

        }

        protected override void OnDispose()
        {            
            clickComponent.Dispose();
            doubleClickComponent.Dispose();
            longpressComponent.Dispose();
                        
            battleUnit = null;

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

        //protected override void CacheSceneActorGameObjectToPool(GameObject go)
        //{
        //    base.CacheSceneActorGameObjectToPool(go);

        //    NavMeshAgent navMeshAgent = go.GetComponent<NavMeshAgent>();
        //    if (navMeshAgent != null)
        //    {
        //        navMeshAgent.enabled = true;
        //    }
        //}

        //protected override void OnSetParent()
        //{
        //    transform.parent = GameCenter.fightWorld.RootTransform;
        //}
        protected override void OnOtherSet()
        {
            AnimatorGameObject = modelGameObject.transform.GetChild(0).gameObject;
        }

        public void OnClick()
        {
            clickComponent.OnClick();
        }

        public void OnDoubleClick()
        {
            doubleClickComponent.OnDouleClick();
        }

        public void OnLongPress()
        {
            longpressComponent.OnClick();
        }
    }   
}