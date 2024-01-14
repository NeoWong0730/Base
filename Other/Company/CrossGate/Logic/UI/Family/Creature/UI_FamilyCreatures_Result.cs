using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using Lib.Core;
using Packet;

namespace Logic
{
    /// <summary>家族训练战结果 </summary>
    public class UI_FamilyCreatures_Result : UIBase
    {
        #region 界面组件
        private Text currentValueText;
        private Text maxValueText;
        #endregion
        #region 数据定义
        /// <summary> 家族训练战数据结果 </summary>
        public CmdGuildPetFightEndNtf cmdGuildPetFightEndNtf;
        #endregion
        #region 系统函数
        protected override void OnInit()
        {
        }
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnDestroy()
        {

        }
        protected override void OnOpen(object arg)
        {
            cmdGuildPetFightEndNtf = null == arg ? new CmdGuildPetFightEndNtf() : (CmdGuildPetFightEndNtf)arg;
        }
        protected override void OnOpened()
        {

        }
        protected override void OnShow()
        {
            RefreshView();
        }
        protected override void OnHide()
        {
        }
        protected override void OnUpdate()
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
            currentValueText = transform.Find("Animator/This Time/Value").GetComponent<Text>();
            maxValueText = transform.Find("Animator/Total/Value").GetComponent<Text>();
            transform.Find("Image_Black").GetComponent<Button>().onClick.AddListener(OnClick_Close);
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            
        }
        #endregion
        #region 数据处理
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            currentValueText.text = cmdGuildPetFightEndNtf.CurrScore.ToString();
            maxValueText.text = cmdGuildPetFightEndNtf.MaxScore.ToString();
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_Rename, "OnClick_Close");
            CloseSelf();
        }
        #endregion
        #region 提供功能
        #endregion
    }
}