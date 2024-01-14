using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Logic.Core;

namespace Logic
{
    public abstract class SpawnVirtualActor : Action
    {
        public uint virtualActorUID;
        public EActorType actorType;
        public uint configID;

        protected Vector3 spawnPosition;

        public override void OnStart()
        {
            SetSpawnPosition();
            Spawn();
        }

        void Spawn()
        {
            ActorSpawnSystem actorSpawnSystem = LevelManager.mMainLevel.GetSystem<ActorSpawnSystem>();
            if (actorSpawnSystem != null)
            {
                actorSpawnSystem.mWaitSpawnVirtualActor.Enqueue(new VirtualActorData()
                {
                    uid = virtualActorUID,
                    eActorType = actorType,
                    infoID = configID,
                    pos = spawnPosition,
                });
            }
        }

        protected abstract void SetSpawnPosition();
    }

    public class SpawnVirtualActorOnTargetPosition : SpawnVirtualActor
    {
        public Vector3 position;

        public override string OnDrawNodeText()
        {
            NodeData.Comment = "指定位置生成虚拟角色";
            return base.OnDrawNodeText();
        }

        protected override void SetSpawnPosition()
        {
            spawnPosition = position;
        }
    }

    public class SpawnVirtualActorOnTargetVirtualActorPosition : SpawnVirtualActor
    {
        public ulong targetVirtualActorUID;

        public override string OnDrawNodeText()
        {
            NodeData.Comment = "指定虚拟角色位置生成虚拟角色";
            return base.OnDrawNodeText();
        }

        protected override void SetSpawnPosition()
        {
        }
    }

    public class SpawnVirtualActorOnMainCharacterPosition : SpawnVirtualActor
    {
        public override string OnDrawNodeText()
        {
            NodeData.Comment = "玩家角色位置生成虚拟角色";
            return base.OnDrawNodeText();
        }

        protected override void SetSpawnPosition()
        {
        }
    }

    public class SpawnVirtualActorOnNpcPosition : SpawnVirtualActor
    {
        public uint targetNPCConfigID;

        public override string OnDrawNodeText()
        {
            NodeData.Comment = "指定NPC位置生成虚拟角色";
            return base.OnDrawNodeText();
        }

        protected override void SetSpawnPosition()
        {
        }
    }
}
