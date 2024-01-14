using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine;
using UnityEngine.UI;
using Lib.Core;
using Packet;
using System;

namespace Logic
{
    /// <summary> 家族建设信息 </summary>
    public class UI_Family_Construct_Info : UIComponent
    {
        #region 界面组件
        
        /// <summary> 无限滚动 </summary>
        private InfinityGrid _infinityGrid;
        #endregion
        #region 数据定义
        /// <summary> 家族建设列表数量 </summary>
        //private List<CSVIndustryActivity.Data>  constructInfos;
        #endregion
        #region 系统函数        
        protected override void Loaded()
        {
            OnParseComponent();
        }
        public override void Show()
        {
            base.Show();
            SetData();
            RefreshView();
        }
        public override void Hide()
        {
            base.Hide();
        }
        protected override void Update()
        {
        }
        protected override void Refresh()
        {

        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        #endregion
        #region 初始化
        /// <summary>
        /// 检测组件
        /// </summary>
        private void OnParseComponent()
        {
            _infinityGrid = transform.Find("Scroll_View_Gem").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {

        }
        /// <summary>
        /// 设置数据
        /// </summary>
        private void SetData()
        {
            //constructInfos = new List<CSVIndustryActivity.Data>(CSVIndustryActivity.Instance.Count);
            //for (int i = 0; i < CSVIndustryActivity.Instance.Count; i++)
            //{
            //    constructInfos.Add(CSVIndustryActivity.Instance[i]);
            //}  
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            //_infinityGrid.CellCount = constructInfos == null ? 0 : constructInfos.Count;
            _infinityGrid.CellCount = CSVIndustryActivity.Instance.Count;
            _infinityGrid.ForceRefreshActiveCell();
        }
        /// <summary>
        /// 设置建设子物体
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="constructInfo"></param>
        private void SetConstruct(Transform tr, CSVIndustryActivity.Data constructInfo)
        {
            tr.name = constructInfo.id.ToString();
            if(null != constructInfo)
            {
                ImageHelper.SetIcon(tr.Find("Image2").GetComponent<Image>(), constructInfo.industryIcon);
                TextHelper.SetText(tr.Find("Text1").GetComponent<Text>(), constructInfo.industryName);
                TextHelper.SetText(tr.Find("Text2").GetComponent<Text>(), constructInfo.playName);
                TextHelper.SetText(tr.Find("Text3").GetComponent<Text>(), constructInfo.playIntroduce);
                tr.Find("Lable").gameObject.SetActive(Sys_Family.Instance.IsShowConstructRedPoint((EConstructs)constructInfo.id));
            }
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        private void OnCreateCell(InfinityGridCell cell)
        {
            GameObject go = cell.mRootTransform.gameObject;
            Button tipsBtn = go.transform.Find("Button_Tips").GetComponent<Button>();
            Button gotoBtn = go.transform.Find("Button_Go").GetComponent<Button>();
            Button imageBtn = go.transform.Find("Image1").GetComponent<Button>();
            imageBtn.onClick.AddListener(() => { OnClickConstructTipBtn(go.transform); });
            tipsBtn.onClick.AddListener(() => { OnClickConstructTipBtn(go.transform); });
            gotoBtn.onClick.AddListener(() => { OnClickConstructGotoBtn(go.transform); });
        }

        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            //var constructInfo = constructInfos[index];
            var constructInfo = CSVIndustryActivity.Instance.GetByIndex(index);
            SetConstruct(cell.mRootTransform.transform, constructInfo);
        }

        /// <summary>
        /// 行业提示按钮点击事件
        /// </summary>
        /// <param name="go"></param>
        private void OnClickConstructTipBtn(Transform tr)
        {
            uint.TryParse(tr.name, out uint id);
            UIManager.HitButton(EUIID.UI_Family_Construct, "OnClickConstructTipBtn:"+ id.ToString());
            Sys_CommonCourse.Instance.OpenCommonCourse(11, 1101);
            /*var uiid = EUIID.UI_Construct_Tips_Agri;
            switch (id)
            {
                case 17:
                    uiid = EUIID.UI_Construct_Tips_Agri; break;
                case 18:
                    uiid = EUIID.UI_Construct_Tips_Business; break;
                case 19:
                    uiid = EUIID.UI_Construct_Tips_Safe; break;
                case 20:
                    uiid = EUIID.UI_Construct_Tips_Rei; break;
                case 21:
                    uiid = EUIID.UI_Construct_Tips_Science; break;
            }
            UIManager.OpenUI(uiid, false, new Tuple<uint, object>(0, id));*/
        }
        /// <summary>
        /// 行业前往按钮点击事件
        /// </summary>
        /// <param name="go"></param>
        private void OnClickConstructGotoBtn(Transform tr)
        {
            uint.TryParse(tr.name, out uint id);
            CSVIndustryActivity.Data constructInfo = CSVIndustryActivity.Instance.GetConfData(id);
            if (null != constructInfo)
            {
                UIManager.HitButton(EUIID.UI_Family_Construct, "OnClickConstructGotoBtn:" + id.ToString());
                ActionCtrl.Instance.MoveToTargetNPCAndInteractive(constructInfo.findNPC);
                UIManager.CloseUI(EUIID.UI_Family);
                UIManager.CloseUI(EUIID.UI_Family_Construct);
            }
            else
            {
                DebugUtil.Log(ELogType.eNone, $"Table CSVIndustryActivity not find id {id}");
            }
        }
        #endregion
        #region 提供功能
        #endregion
    }
}