using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using System;
using Framework;
using Lib.Core;
using Table;
using DG.Tweening;

namespace Logic
{
    public class Sys_LittleGame_SeekItem : SystemModuleBase<Sys_LittleGame_SeekItem>
    {
        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        private CSVSeekItem.Data _cSVSeekItemData;

        private Vector3 _startRot;
        private Vector3 _endRot;
        private Vector3 _startPos;
        private Vector3 _endPos;
        public uint _TaskId;
        private Timer timer;
        private Timer timer1;

        public enum EEvents
        {
            e_TriggerStoneIcon,  //触发石像
        }

        public void EnterSeekItem(uint taskid, uint seekitemId)
        {
            _TaskId = taskid;
            _cSVSeekItemData = CSVSeekItem.Instance.GetConfData(seekitemId);

            //var component = World.GetComponent<MovementComponent>(GameCenter.mainHero);
            //component.enableflag = false;

            if (_cSVSeekItemData != null)
            {
                CameraData cameraData = new CameraData
                {
                    pith = _cSVSeekItemData.camera[0] / 10000f,
                    yaw = _cSVSeekItemData.camera[1] / 10000f,
                    roll = _cSVSeekItemData.camera[2] / 10000f,
                    distance = _cSVSeekItemData.camera[3] / 10000f,
                    fov = _cSVSeekItemData.camera[7] / 10000f,
                    lookPointOffset = new Vector3(_cSVSeekItemData.camera[4] / 10000f, _cSVSeekItemData.camera[5] / 10000f, _cSVSeekItemData.camera[6] / 10000f)
                };
                GameCenter.mCameraController.SetCameraData(cameraData, 0);

                _startRot = GameCenter.mainHero.transform.eulerAngles;
                _endRot = new Vector3(_cSVSeekItemData.dialogueParameter[3] / 10000f, _cSVSeekItemData.dialogueParameter[4] / 10000f,
                    _cSVSeekItemData.dialogueParameter[5] / 10000f);
                _startPos = GameCenter.mainHero.transform.position;
                _endPos = new Vector3(_cSVSeekItemData.consultPosition[0] / 10000, _cSVSeekItemData.consultPosition[1] / 10000, _cSVSeekItemData.consultPosition[2] / 10000) +
                    new Vector3(_cSVSeekItemData.dialogueParameter[0] / 10000f, _cSVSeekItemData.dialogueParameter[1] / 10000f, _cSVSeekItemData.dialogueParameter[2] / 10000f);
                DOTween.To(() => GameCenter.mainHero.transform.position, x => GameCenter.mainHero.transform.position = x, _endPos, 0).SetEase(Ease.Linear);
                DOTween.To(() => GameCenter.mainHero.transform.eulerAngles, x => GameCenter.mainHero.transform.eulerAngles = x, _endRot, 0).SetEase(Ease.Linear);
            }
            else
            {
                DebugUtil.LogErrorFormat("小游戏{0}数据找不到", seekitemId);
            }

            //GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterInteractive);
            //CheckHeroVisualComponent.Enable = false;

            GameCenter.mCheckHeroVisualSystem.isActive = false;
            if (GameCenter.mainHero != null)
            {
                GameCenter.mainHero.SetLayerHide();
            }

            foreach (var actor in GameCenter.otherActorsDic.Values)
            {
                actor.SetLayerHide();
            }
        }

        public void LeaveSeekItem()
        {
            //var component = World.GetComponent<MovementComponent>(GameCenter.mainHero);
            //component.enableflag = true;

            timer1?.Cancel();
            timer1 = Timer.Register(0.3f, () =>
             {
                 GameCenter.mCameraController.RevertToLastCameraData(0f, () =>
                 {
                     DOTween.To(() => GameCenter.mainHero.transform.position, x => GameCenter.mainHero.transform.position = x, _startPos, 0).SetEase(Ease.Linear);
                     DOTween.To(() => GameCenter.mainHero.transform.eulerAngles, x => GameCenter.mainHero.transform.eulerAngles = x, _startRot, 0).SetEase(Ease.Linear);
                     GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);

                     //CheckHeroVisualComponent.Enable = true;
                     GameCenter.mCheckHeroVisualSystem.isActive = true;

                     if (GameCenter.mainHero != null)
                     {
                         GameCenter.mainHero.ReturnCacheLayer();
                     }
                     foreach (var actor in GameCenter.otherActorsDic.Values)
                     {
                         actor.ReturnCacheLayer();
                     }
                 });
             });
            timer?.Cancel();
            timer = Timer.Register(0.5f, () =>
            {
                Sys_Task.Instance.ReqStepGoalFinishEx(_TaskId);
            });


            //if (GameCenter.mainHero != null)
            //{
            //    GameCenter.mainHero.gameObject?.SetActive(true);
            //}
        }


        public void EnterPaint(uint taskid)
        {
            _TaskId = taskid;
        }
    }
}

