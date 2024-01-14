using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Table;
using UnityEngine.UI;
using Lib.Core;
using Packet;
using Framework;

namespace Logic
{
    //资格赛挑战名额说明
    public class UI_BossTower_QualifierExplain : UIBase
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

        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        #endregion
        #region 组件
        Button btnClose;
        Text baseRankNum;
        Transform parent;
        Text extraRankNum;
        Text curQualifierNum;
        #endregion
        #region 数据
        List<DynamicRankItem> DynamicRankItemList = new List<DynamicRankItem>();
        int curSelectIndex;
        #endregion
        #region 组件查找、事件注册
        private void OnParseComponent()
        {
            btnClose = transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
            baseRankNum = transform.Find("Animator/Scroll View/Viewport/Content/Line0/Text").GetComponent<Text>();
            parent = transform.Find("Animator/Scroll View/Viewport/Content/Table");
            extraRankNum = transform.Find("Animator/Scroll View/Viewport/Content/Line3/Text").GetComponent<Text>();
            curQualifierNum = transform.Find("Animator/Scroll View/Viewport/Content/Line1/Text").GetComponent<Text>();

            btnClose.onClick.AddListener(() => { CloseSelf(); });
        }
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_ActivityBossTower.Instance.eventEmitter.Handle(Sys_ActivityBossTower.EEvents.OnRefreshSelfRankData, RefreshSelfRnak, toRegister);
        }
        private void RefreshSelfRnak()
        {
            curQualifierNum.text = LanguageHelper.GetTextContent(1009048, Sys_ActivityBossTower.Instance.curDynamicQualifierNum.ToString());
            CreateDynamicRankItem();
        }
        #endregion
        private void InitView()
        {
            Sys_ActivityBossTower.Instance.OnBossTowerSelfRankReq();
            curSelectIndex = -1;
            baseRankNum.text = LanguageHelper.GetTextContent(1009047, Sys_ActivityBossTower.Instance.GetBossTowerParameter(1));
            extraRankNum.text = LanguageHelper.GetTextContent(1009054, Sys_ActivityBossTower.Instance.GetBossTowerParameter(4));
            curQualifierNum.text= LanguageHelper.GetTextContent(1009048, Sys_ActivityBossTower.Instance.curDynamicQualifierNum.ToString());
            CreateDynamicRankItem();
        }
        private void CreateDynamicRankItem()
        {
            string param = Sys_ActivityBossTower.Instance.GetBossTowerParameter(3);
            string[] strValue = param.Split('|');
            SetCurSelectIndex(strValue);
            for (int i = 0; i < DynamicRankItemList.Count; i++)
            {
                DynamicRankItem cell = DynamicRankItemList[i];
                PoolManager.Recycle(cell);
            }
            DynamicRankItemList.Clear();
            FrameworkTool.CreateChildList(parent, strValue.Length);
            for (int i = 0; i < strValue.Length; i++)
            {
                Transform trans = parent.GetChild(i);
                DynamicRankItem cell = PoolManager.Fetch<DynamicRankItem>();
                cell.Init(trans);
                cell.SetData(strValue[i], curSelectIndex == i);
                DynamicRankItemList.Add(cell);
            }
            FrameworkTool.ForceRebuildLayout(transform.gameObject);
        }
        private void SetCurSelectIndex(string[] strValue)
        {
            uint curDynamicQualifierNum = Sys_ActivityBossTower.Instance.curDynamicQualifierNum;
            for (int i = strValue.Length - 1; i >= 0; i--)
            {
                string[] par = strValue[i].Split('&');
                if (curDynamicQualifierNum >= uint.Parse(par[1]))
                {
                    curSelectIndex = i;
                    break;
                }
            }
        }
        #region item
        public class DynamicRankItem
        {
            GameObject objSelect, objCurFinished;
            Text floorNum, curRankNum, extarRankNum;
            public void Init(Transform trans)
            {
                objSelect = trans.Find("ImageBG_Select").gameObject;
                objCurFinished = trans.Find("Image_Label").gameObject;
                floorNum = trans.Find("Floor").GetComponent<Text>();
                curRankNum = trans.Find("Num").GetComponent<Text>();
                extarRankNum = trans.Find("Quota").GetComponent<Text>();
            }
            public void SetData(string str, bool isSelect)
            {
                string[] par = str.Split('&');
                curRankNum.text = par[1];
                extarRankNum.text = par[0];
                floorNum.text = LanguageHelper.GetTextContent(1006199, par[2]);
                if (isSelect)
                {
                    objSelect.SetActive(true);
                    objCurFinished.SetActive(true);
                }
                else
                {
                    objSelect.SetActive(false);
                    objCurFinished.SetActive(false);
                }
            }
        }
        #endregion
    }
}