using Logic.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    /// <summary> 地域防范提示 </summary>
    public class UI_AreaProtectionTips : UIBase
    {
        #region 界面组件
        /// <summary> 完成委托 </summary>
        private Transform tr_FinishNode;
        /// <summary> 接受委托 </summary>
        private Transform tr_ReceiveNode;
        /// <summary> 内容 </summary>
        private Text text_Message;
        #endregion
        #region 数据定义
        /// <summary>
        /// 事件数据
        /// </summary>
        public class EventData
        {
            /// <summary> 事件ID </summary>
            public uint eventId;
            /// <summary> 是接取 </summary>
            public bool isReceive;
            /// <summary> 界面关闭后的其他行为 </summary>
            public Action action_Close;
            public EventData()
            {

            }

            public EventData(uint eventId, bool isReceive, Action action_Close = null)
            {
                this.eventId = eventId;
                this.isReceive = isReceive;
                this.action_Close = action_Close;
            }
        }
        /// <summary> 事件数据 </summary>
        public EventData eventData;
        /// <summary> 定时关闭 </summary>
        private float time = 0;
        #endregion
        #region 系统函数
        protected override void OnInit()
        {

        }
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnUpdate()
        {
            if (time > 0)
            {
                time -= deltaTime;
                if (time <= 0)
                {
                    OnClick_Close();
                }
            }
        }
        protected override void OnOpen(object arg)
        {
            eventData = arg == null ? new EventData() : arg as EventData;
			time = 2f;
        }
        protected override void OnShow()
        {
            RefreshView();
        }
        protected override void OnHide()
        {

        }
        protected override void OnClose()
        {
            if (null != eventData.action_Close)
                eventData.action_Close();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {

        }
        #endregion
        #region 初始化
        /// <summary>
        /// 检测组件 
        /// </summary>
        private void OnParseComponent()
        {
            tr_FinishNode = transform.Find("Animator/Text_Finish").transform;
            tr_ReceiveNode = transform.Find("Animator/Text_Receive").transform;
            text_Message = transform.Find("Animator/Text").GetComponent<Text>();
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            CSVAreaProtection.Data cSVAreaProtectionData = CSVAreaProtection.Instance.GetConfData(eventData.eventId);
            if (null != cSVAreaProtectionData)
            {
                text_Message.text = LanguageHelper.GetTextContent(cSVAreaProtectionData.eventName_id);
            }
            tr_ReceiveNode.gameObject.SetActive(eventData.isReceive);
            tr_FinishNode.gameObject.SetActive(!eventData.isReceive);
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        private void OnClick_Close()
        {
            CloseSelf();
        }
        #endregion
    }
}