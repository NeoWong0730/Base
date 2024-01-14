using Lib.Core;
using Packet;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    public class CheckTeleporterSystem : LevelSystemBase
    {
        private float fLastTime = 0f;
        private Vector3 LastPosition = Vector3.zero;

        public override void OnUpdate()
        {
            if (ActionCtrl.Instance.actionCtrlStatus == ActionCtrl.EActionCtrlStatus.PlayerCtrl)
            {
                if (GameCenter.mainHero.movementComponent.eMoveState == MovementComponent.EMoveState.MoveTo)
                    return;
            }
            else if (ActionCtrl.Instance.actionCtrlStatus == ActionCtrl.EActionCtrlStatus.Auto)
            {
                if (ActionCtrl.Instance.currentAutoAction != null && ActionCtrl.Instance.currentAutoAction is PathFindAction)
                    return;
            }

            if (GameCenter.mainHero == null)
                return;

            Transform mainHeroTransform = GameCenter.mainHero.transform;
            if (mainHeroTransform == null)
                return;

            if(LastPosition.Equals(mainHeroTransform.position))
                return;
            LastPosition = mainHeroTransform.position;

            List<TeleporterActor> teleports = GameCenter.teleports;

            TeleporterActor newEnterTel = null;
            bool inArea = false;

            for (int i = teleports.Count - 1; i >= 0; --i)
            {
                TeleporterActor actor = teleports[i];

                inArea = actor.scopeDetection.Contains(mainHeroTransform);
                if (inArea && !actor._bInArea && actor.CheckCondition())
                {
                    newEnterTel = actor;
                }

                actor._bInArea = inArea;
            }

            if (newEnterTel != null && !isTeamMemFollowed() && Time.unscaledTime >= fLastTime)
            {
                fLastTime = Time.unscaledTime + 1.0f;
                newEnterTel.GenTransmitAction();
            }
        }

        public override void OnDestroy()
        {

        }

        private static bool isTeamMemFollowed()
        {
            if (Sys_Team.Instance.HaveTeam == false)
                return false;

            if (Sys_Team.Instance.isCaptain())
                return false;

            TeamMem teamMem = Sys_Team.Instance.getTeamMem(Sys_Role.Instance.RoleId);

            if (teamMem == null || teamMem.IsLeave())
                return false;

            return true;
        }
    }
}