using System.Collections.Generic;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Logic.Core;
using System.Text;
using Framework;
using System.Linq;

namespace Logic
{
    public class UI_FightTreasure_HundredDollar : UI_OperationalActivityBase
    {
        #region 界面显示
        private InfinityGrid PanelScrollGrid;
        private Timer m_timer;
        private uint _id;
        Sys_OperationalActivity.SingleFightType sft;
        private Dictionary<int, UI_FightTreasure_Ceil> entrydic = new Dictionary<int, UI_FightTreasure_Ceil>();
        #endregion
        #region 系统函数
        protected override void Update()
        {
            base.Update();
            if (entrydic.Count == 0)
            {
                return;
            }
            foreach (var item in entrydic)
            {
                item.Value.OnRefresh();
            }
            //for (int i = 0; i < entrydic.Count; i++)
            //{
            //    KeyValuePair<int, UI_FightTreasure_Ceil> kv = entrydic.ElementAt(i);
            //    kv.Value.OnRefresh();
            //}
        }
        protected override void InitBeforOnShow()
        {
            Parse();
        }
        public override void Show()
        {
            base.Show();
            OnDollar();

        }
        public override void Hide()
        {
            base.Hide();
            m_timer?.Cancel();
            entrydic.Clear();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_OperationalActivity.Instance.eventEmitter.Handle<uint>(Sys_OperationalActivity.EEvents.UpdateFightTreasureData, OnFightTreasure, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateOperatinalActivityShowOrHide, OnOperatinalActivityShowOrHide, toRegister);
            Sys_ActivityOperationRuler.Instance.eventEmitter.Handle(Sys_ActivityOperationRuler.EEvents.OnRefreshActivityInfo, JumpDayRefresh, toRegister);
        }
        #endregion
        #region Function

        private void Parse()
        {
            PanelScrollGrid = transform.Find("Scroll View").GetComponent<InfinityGrid>();
            Sys_OperationalActivity.Instance.ScrollViewVect = CameraManager.mUICamera.WorldToScreenPoint(PanelScrollGrid.gameObject.GetComponent<RectTransform>().position);
            PanelScrollGrid.onCreateCell += OnCreateCell;
            PanelScrollGrid.onCellChange += OnCellChange;
        }
        private void OnDollar()
        {
            Sys_OperationalActivity.Instance.CheckFightActivityDictionary();
            if (Sys_OperationalActivity.Instance.FightActivityDic.ContainsKey(EActivityRulerType.HundredDollarTreasure))
            {
                _id = Sys_OperationalActivity.Instance.FightActivityDic[EActivityRulerType.HundredDollarTreasure].aId;
                Sys_OperationalActivity.Instance.FightTreasureRedPointDic[_id] = false;
                if (Sys_OperationalActivity.Instance.fightTreasureDic.ContainsKey(_id))
                {
                    Sys_OperationalActivity.Instance.FightTreasureRedPointDic[_id] = false;
                    Sys_OperationalActivity.Instance.OnFightTreasureDataReq(_id);//请求数据

                }
            }
            else
            {
                ClearTemporaryHandle();
            }



        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_FightTreasure_Ceil entry = new UI_FightTreasure_Ceil();
            entry.BindGameObject(cell.mRootTransform);
            entry.AddRefreshListener(OnRefreshSingleGrid);
            cell.BindUserData(entry);
        }
        private void ClearTemporaryHandle()
        {
            uint _id = 0;
            foreach (var item in Sys_OperationalActivity.Instance.fightTreasureDic)
            {
                if (item.Value.eType == EActivityRulerType.HundredDollarTreasure)
                {
                    if (Sys_OperationalActivity.Instance.CheckFightActivityEnd(item.Key))
                    {
                        _id = item.Key;

                    }
                    
                }
            }

            if (Sys_OperationalActivity.Instance.fightTreasureDic.TryGetValue(_id, out Sys_OperationalActivity.SingleFightType _sft))
            {
                entrydic.Clear();
                sft = _sft;
                sft.CheckNowRound();
                if (sft.InRound < 0)
                {
                    Sys_OperationalActivity.Instance.FightTreasureRedPointDic[_id] = true;
                }

                PanelScrollGrid.CellCount = sft.roundsList.Count;
                PanelScrollGrid.Apply();
                if (sft.recordRound >= 0)
                {
                    var _index = sft.recordRound - 1 < 0 ? 0 : sft.recordRound - 1;
                    PanelScrollGrid.MoveIndexToTop(_index);
                }
                PanelScrollGrid.ForceRefreshActiveCell();
            }
        }
        private void OnRefreshSingleGrid(int _round)
        {
            OnDollar();
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_FightTreasure_Ceil entry = cell.mUserData as UI_FightTreasure_Ceil;
            entry.SetCeilData(index, sft.roundsList[index], sft.eType);
            entrydic[sft.roundsList[index].thisRoundIndex] = entry;
        }
        private void JumpDayRefresh()
        {
            //OnRefreshTotalGrid();
            OnDollar();
        }
        private void OnOperatinalActivityShowOrHide()
        {
            OnDollar();
        }

        private void OnRefreshTotalGrid()
        {
            if (Sys_OperationalActivity.Instance.fightTreasureDic.TryGetValue(_id, out Sys_OperationalActivity.SingleFightType _sft))
            {
                entrydic.Clear();
                sft = _sft;
                sft.CheckNowRound();
                if (sft.InRound <0)
                {
                    Sys_OperationalActivity.Instance.FightTreasureRedPointDic[_id] = true;
                }
            }
            else
            {
                DebugUtil.LogError("夺宝活动id不匹配");
                return;
            }

            PanelScrollGrid.CellCount = sft.roundsList.Count;
            PanelScrollGrid.Apply();
            if (sft.recordRound >= 0)
            {
                var _index = sft.recordRound - 1 < 0 ? 0 : sft.recordRound - 1;
                PanelScrollGrid.MoveIndexToTop(_index);
            }
            PanelScrollGrid.ForceRefreshActiveCell();

        }

        private void OnFightTreasure(uint _type)
        {
            if (_type == 0)
            {
                OnRefreshTotalGrid();
            }
            else
            {
                entrydic[(int)_type].OnFightTreasureUpdateButton(sft.roundsList[(int)_type].applyNum);
            }

        }
        #endregion
    }

}
