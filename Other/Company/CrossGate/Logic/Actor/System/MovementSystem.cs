using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using Lib.Core;

namespace Logic
{
    public class MovementSystem : LevelSystemBase
    {
        public override void OnUpdate()
        {
            if (GameCenter.mainHero != null)
            {
                Excute(GameCenter.mainHero.movementComponent);
            }

            for (int i = 0, len = GameCenter.otherActorList.Count; i < len; ++i)
            {
                Excute(GameCenter.otherActorList[i].movementComponent);
            }

            for (int i = 0, len = GameCenter.npcsList.Count; i < len; ++i)
            {
                Excute(GameCenter.npcsList[i].movementComponent);
            }

            foreach (var virtualNpc in Sys_Escort.Instance.escortVirtualNpcs.Values)
            {
                Excute(virtualNpc.movementComponent);
            }

            foreach (var virtualNpc in Sys_Track.Instance.trackVirtualNpcs.Values)
            {
                Excute(virtualNpc.movementComponent);
            }

            foreach (var virtualNpc in Sys_NpcFollow.Instance.npcFollowVirtualNpcs.Values)
            {
                Excute(virtualNpc.movementComponent);
            }            

            foreach (var virtualNpc in VirtualShowManager.Instance.virtualSceneActors.Values)
            {
                Excute(virtualNpc.movementComponent);
            }
        }

        private void Excute(MovementComponent movementComponent)
        {
            movementComponent.Update();
        }
    }
}