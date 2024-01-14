using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using Table;

namespace Logic
{
    /*
    public class NearbyNpcComponent : Logic.Core.Component, IUpdateCmd
    {
        private Npc _npc;
        private VisualComponent _vsCom;
        private CSVNpc.Data _npcData;

        private float lastTime;
        private float cd;
        private float areaDistance;
        private bool isEnter;

        protected override void OnConstruct()
        {
            _npc = actor as Npc;
            _npcData = _npc.cSVNpcData;
            _vsCom = _npc.VisualComponent;

            if (_npc != null)
                areaDistance = _npc.cSVNpcData.TriggerScopen / 10000f;

            lastTime = Time.unscaledTime;
            cd = 0.5f;

            isEnter = false;
        }

        protected override void OnDispose()
        {
            if (isEnter)
                Sys_Npc.Instance.eventEmitter.Trigger(Sys_Npc.EEvents.OnLeaveNpc, _npc);

            _npc = null;
            lastTime = 0f;
            cd = 0f;
            isEnter = false;
        }

        public void Update()
        {
            if (Time.unscaledTime - lastTime < cd)
                return;

            lastTime = Time.unscaledTime;

            if (GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Normal)
            {
                return;
            }

            CheckNpcs();
        }

        void CheckNpcs()
        {
            if (_npc == null)
                return;

            if (_vsCom == null)
            {
                _vsCom = _npc.VisualComponent;
                return;
            }

            if (!_vsCom.Visiable)
                return;

            if (_npcData == null)
                return;

            if (_npcData.WhetherScopenTrigger != 1)
                return;


            if (ActionCtrl.Instance.actionCtrlStatus == ActionCtrl.EActionCtrlStatus.Auto)
            {
                return;
            }

            float distance = _npc.DistanceTo(GameCenter.mainHero.transform);
            if (distance < areaDistance)
            {
                if (!isEnter)
                {
                    isEnter = true;
                    Sys_Npc.Instance.eventEmitter.Trigger(Sys_Npc.EEvents.OnNearNpc, _npc);
                }
            }
            else
            {
                if (isEnter)
                {
                    isEnter = false;
                    Sys_Npc.Instance.eventEmitter.Trigger(Sys_Npc.EEvents.OnLeaveNpc, _npc);
                }
            }
        }
    }
    */
}
