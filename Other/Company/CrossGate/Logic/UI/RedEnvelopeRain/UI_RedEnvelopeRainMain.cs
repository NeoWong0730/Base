using Logic.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Lib.Core;
using Table;

namespace Logic
{
    public class UI_RedEnvelopeRainMain : UIBase
    {
        #region 系统函数
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnShow()
        {
            InitView();
        }
        protected override void OnClose()
        {
            envelopeCellList.Clear();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        protected override void OnLateUpdate(float dt, float usdt)
        {
            OnLateUpdate();
        }
        #endregion
        #region 组件
        Transform parent;
        GameObject item;
        #endregion
        #region 数据
        List<EnvelopeCell> envelopeCellList = new List<EnvelopeCell>();
        #endregion
        #region 组件查找、事件
        private void OnParseComponent()
        {
            parent = transform.Find("Animator");
            item= transform.Find("Animator/RedEnvelopeItem").gameObject;
            item.SetActive(false);
        }
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_RedEnvelopeRain.Instance.eventEmitter.Handle(Sys_RedEnvelopeRain.EEvents.OnRefreshEnvelopeData, OnRefreshEnvelopeData, toRegister);
        }
        private void OnRefreshEnvelopeData()//EnvelopeData data
        {
            EnvelopeCell cell = null;
            if (envelopeCellList.Count > 0)
            {
                for (int i = 0; i < envelopeCellList.Count; i++)
                {
                    EnvelopeCell envelopeCell = envelopeCellList[i];
                    if (envelopeCell.isEnd)
                    {
                        cell = envelopeCellList[i];
                        break;
                    }
                }
            }
            if (cell == null)
            {
                cell = CreateEnvelope();
                envelopeCellList.Add(cell);
            }
            cell.SetData();
            cell.Move();
        }
        #endregion
        #region 初始化
        private void InitView()
        {
            Sys_RedEnvelopeRain.Instance.GetNextEnvelopeData();
        }
        #endregion
        private void OnLateUpdate()
        {

        }
        #region function
        private EnvelopeCell CreateEnvelope()
        {
            GameObject obj = FrameworkTool.CreateGameObject(item, parent.gameObject);
            EnvelopeCell cell = new EnvelopeCell();
            cell.Init(obj.transform);
            return cell;
        }
        private bool CheckIsEnd()
        {
            bool isEnd = true;
            for (int i = 0; i < envelopeCellList.Count; i++)
            {
                if (!envelopeCellList[i].isEnd)
                {
                    isEnd = false;
                    break;
                }
            }
            return isEnd;
        }
        #endregion
    }
    public class EnvelopeCell
    {
        public GameObject obj;
        GameObject normalObj;
        GameObject goldObj;
        RectTransform rectTrans;
        Button btn;
        public EnvelopeData data;
        float moveDistance;
        float duration;
        public bool isEnd;
        public void Init(Transform trans)
        {
            obj = trans.gameObject;
            normalObj = trans.Find("Item").gameObject;
            goldObj = trans.Find("Item_Golden").gameObject;
            rectTrans = trans.GetComponent<RectTransform>();
            btn = trans.GetComponent<Button>();

            btn.onClick.AddListener(OnClick);
        }
        public void SetData()
        {
            this.data = Sys_RedEnvelopeRain.Instance.GetEnvelopeData(data);
            obj.SetActive(true);
            isEnd = false;
            if (data.quality == RedEnvelopeQuality.Golden)
            {
                goldObj.SetActive(true);
                normalObj.SetActive(false);
            }
            else
            {
                goldObj.SetActive(false);
                normalObj.SetActive(true);
            }
            float curHeight = Sys_RedEnvelopeRain.Instance.GetCurWidthOrHeight(false);
            rectTrans.localPosition = new Vector3(data.posX, rectTrans.rect.height / 2 * data.scale + curHeight / 2, 0);
            rectTrans.localScale = new Vector3(data.scale, data.scale, data.scale);

            moveDistance = -(curHeight / 2 + rectTrans.rect.height/2);
            duration = 6;
        }
        public void Move()
        {
            rectTrans.DOLocalMoveY(moveDistance, duration).SetEase(Ease.Linear).onComplete += () =>
            {
                obj.SetActive(false);
                isEnd = true;
                data = null;
            };
        }
        private void OnClick()
        {
            obj.SetActive(false);
            UIManager.OpenUI(EUIID.UI_RedEnvelopeRain_Popup, false, data);
        }
    }
}