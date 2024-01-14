using Lib.Core;
using Logic.Core;
using Packet;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Achievement_ShareList : UIBase
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
        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                clickData = arg as ClickShowPositionData;
            }
        }
        protected override void OnDestroy()
        {
            clickData = null;
            shareCellList.Clear();
        }
        #endregion
        #region 组件
        Button closeBtn;
        Transform content;
        #endregion
        #region 数据
        ClickShowPositionData clickData;
        List<ShareCell> shareCellList = new List<ShareCell>();
        #endregion
        #region 查找组件、注册事件
        private void OnParseComponent()
        {
            closeBtn = transform.Find("Image_BG").GetComponent<Button>();
            content = transform.Find("bg");

            closeBtn.onClick.AddListener(() => { CloseSelf(); });
        }
        #endregion
        #region 初始化
        private void InitView()
        {
            SetPosition();
            RefreshShareCell();
        }
        #endregion
        #region 界面显示
        public void SetPosition()
        {
            if (clickData != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(clickData.parent, clickData.clickTarget.position, null, out Vector2 localPos);
                Vector3 newPos = new Vector3(localPos.x - clickData.clickTarget.sizeDelta.x * 2, localPos.y <= -13f ? -13f : localPos.y, 0);
                content.localPosition = newPos;
            }
        }
        private void RefreshShareCell()
        {
            for (int i = 0; i < shareCellList.Count; i++)
            {
                ShareCell cell = shareCellList[i];
                PoolManager.Recycle(cell);
            }
            shareCellList.Clear();
            int count = Sys_Achievement.Instance.achShareDataList.Count;
            FrameworkTool.CreateChildList(content, count);
            for (int i = 0; i < count; i++)
            {
                Transform tran = content.GetChild(i);
                ShareCell cell = PoolManager.Fetch<ShareCell>();
                cell.Init(tran);
                cell.SetData(Sys_Achievement.Instance.achShareDataList[i], OnClickShare);
                shareCellList.Add(cell);
            }
        }
        private void OnClickShare(EAchievementShareType type)
        {
            if (type == EAchievementShareType.Friend)
            {
                UIManager.OpenUI(EUIID.UI_Achievement_Share, false, clickData.data);
            }
            else
            {
                Sys_Chat.Instance.mInputCache.AddAchievement(clickData.data);
                int errorCode =Sys_Chat.Instance.SendContent((ChatType)type, Sys_Chat.Instance.mInputCache);
                Sys_Chat.Instance.mInputCache.Clear();
                if (errorCode != Sys_Chat.Chat_Success)
                {
                    Sys_Chat.Instance.PushErrorTip(errorCode);
                }
                else
                    Sys_Achievement.Instance.isShare = true;
            }
            CloseSelf();
        }
        #endregion

        public class ShareCell
        {
            Text title;
            Button btn;
            AchShareData data;
            Action<EAchievementShareType> action;
            public void Init(Transform tran)
            {
                title = tran.Find("Text_01").GetComponent<Text>();
                btn=tran.GetComponent<Button>();

                btn.onClick.AddListener(SelectedShare);
            }
            public void SetData(AchShareData data,Action<EAchievementShareType> action)
            {
                this.data = data;
                this.action = action;
                title.text = LanguageHelper.GetTextContent(data.languageId);
            }
            private void SelectedShare()
            {
                action?.Invoke(data.shareType);
            }
        }
    }
}