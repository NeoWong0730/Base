using Logic.Core;
using UnityEngine;
using Lib.Core;
using System;
using Table;
using System.Collections.Generic;

namespace Logic
{
    public class TeleporterActor : Actor
    {        
        private Sys_Map.TelData _mapTelData;        
        private CSVCheckseq.Data _checkseqData;

        public bool _bInArea;
        public RectScopeDetection scopeDetection = new RectScopeDetection();               

        protected override void OnDispose()
        {
            _mapTelData = null;
            _checkseqData = null;            
            _bInArea = false;   
        }

        public void SetTelData(Sys_Map.TelData telData, CSVCheckseq.Data checkseqData)
        {
            _mapTelData = telData;
            _checkseqData = checkseqData;

            Rect rectTel = Rect.zero;
            rectTel.position = new Vector2(_mapTelData.pos.X + _mapTelData.offX, _mapTelData.pos.Y + _mapTelData.offY);
            rectTel.size = new Vector2(_mapTelData.rangeX, _mapTelData.rangeY);
            rectTel.position -= rectTel.size * 0.5f;            

            //rectTel.size *= 2f;
            scopeDetection.Init(new List<Rect>() { rectTel });
        }

        public void GenTransmitAction()
        {
            TransmitAction transmitAction = ActionCtrl.Instance.CreateAction(typeof(TransmitAction)) as TransmitAction;
            if (transmitAction != null)
            {                
                transmitAction.enter = true;
                transmitAction.portID = _mapTelData.mapId;
                transmitAction.gridX = (uint)_mapTelData.pos.X;
                transmitAction.gridY = (uint)(-_mapTelData.pos.Y);

                if (ActionCtrl.Instance.actionCtrlStatus == ActionCtrl.EActionCtrlStatus.PlayerCtrl)
                {
                    ActionCtrl.Instance.ExecutePlayerCtrlAction(transmitAction);
                }
                else
                {                    
                    ActionCtrl.Instance.AddAutoAction(transmitAction);
                }
            }
        }

        //public bool TestCheckEnter(Transform tran, Vector3 pos)
        //{
        //    bool inArea = scopeDetection.Contains(tran);    
        //    
        //    if (_bInArea != inArea)
        //    {
        //        _bInArea = inArea;
        //        return _bInArea && CheckCondition();
        //    }
        //    return false;
        //}

        public bool CheckCondition()
        {
            return _checkseqData == null || _checkseqData.IsValid();
        }
    }
}