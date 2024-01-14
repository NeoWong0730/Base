using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    public class VirtualNpc : VirtualSceneActor
    {        
        public VirtualNpcHUDComponent virtualNpcHUDComponent;

        public EscortVirtualNpcTaskGoalDataComponent escortVirtualNpcTaskGoalDataComponent;
        public EscortVirtualNpcDistanceCheckComponent escortVirtualNpcDistanceCheckComponent;
        public TrackVirtualNpcDistanceOutCheckComponent trackVirtualNpcDistanceOutCheckComponent;
        public TrackVirtualNpcDistanceInCheckComponent trackVirtualNpcDistanceInCheckComponent;
        public TrackVirtualNpcTaskGoalDataComponent trackVirtualNpcTaskGoalDataComponent;
        public TrackVirtualNpcLogicComponent trackVirtualNpcLogicComponent;
        public NpcFollowVirtualNpcTaskGoalDataComponent npcFollowVirtualNpcTaskGoalDataComponent;
        public VirtualNPCFollowLogicComponent virtualNPCFollowLogicComponent;
        public StateComponent stateComponent = new StateComponent();
        public FollowComponent followComponent;

        public enum EVirtualNpcType
        {
            Common,
            Escort,
            Track,
            NpcFollow,
        }

        public EVirtualNpcType eVirtualNpcType;

        public CSVNpc.Data cSVNpcData
        {
            get;
            set;
        }        

        protected override void OnConstruct()
        {
            AnimationComponent.stateComponent = stateComponent;
            movementComponent.stateComponent = stateComponent;

            base.OnConstruct();
            stateComponent.actor = this;
            stateComponent.Construct();            
        }

        protected override void OnDispose()
        {            
            stateComponent.Dispose();            

            EffectUtil.Instance.UnloadEffectByTag(uID, EffectUtil.EEffectTag.EscortIn);
            EffectUtil.Instance.UnloadEffectByTag(uID, EffectUtil.EEffectTag.EscortOut);
            cSVNpcData = null;

            virtualNpcHUDComponent?.Dispose();
            virtualNpcHUDComponent = null;

            escortVirtualNpcDistanceCheckComponent?.Dispose();
            escortVirtualNpcDistanceCheckComponent = null;

            trackVirtualNpcDistanceOutCheckComponent?.Dispose();
            trackVirtualNpcDistanceOutCheckComponent = null;

            trackVirtualNpcDistanceInCheckComponent?.Dispose();
            trackVirtualNpcDistanceInCheckComponent = null;

            trackVirtualNpcLogicComponent?.Dispose();
            trackVirtualNpcLogicComponent = null;

            virtualNPCFollowLogicComponent?.Dispose();
            virtualNPCFollowLogicComponent = null;

            trackVirtualNpcTaskGoalDataComponent = null;

            npcFollowVirtualNpcTaskGoalDataComponent = null;

            escortVirtualNpcTaskGoalDataComponent = null;

            followComponent?.Dispose();
            PoolManager.Recycle(followComponent);
            followComponent = null;

            base.OnDispose();
        }

        //protected override void OnSetName()
        //{
        //    gameObject.name = $"TaskVirtualNPC_{uID.ToString()}";
        //}

        protected override void LoadDepencyAssets()
        {
            assetsGroupLoader = new AssetsGroupLoader();

            List<string> animationPaths;
            AnimationComponent.GetAnimationPaths(cSVNpcData.action_id, Constants.UMARMEDID, out animationPaths, null);
            if (animationPaths != null)
            {
                for (int index = 0, len = animationPaths.Count; index < len; index++)
                {
                    assetsGroupLoader.AddLoadTask(animationPaths[index]);
                }
            }
            else
            {
                DebugUtil.LogError($"animationPaths is null npcType: {cSVNpcData.id} action_id: {cSVNpcData.action_id}");
            }
        }

        public override uint ID
        {
            get
            {
                return cSVNpcData.action_id;
            }
        }
    }
}
