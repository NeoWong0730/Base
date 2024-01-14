using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Achievement_Share : UIBase
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
                achData = arg as AchievementDataCell;
        }
        protected override void OnDestroy()
        {
            curSelectedFirendList.Clear();
            roleInfos.Clear();
            searchTheRoleList.Clear();
            achData = null;
            tempStringBuilder?.Clear(); 
        }
        #endregion
        #region 组件
        Button closeBtn;
        InputField inputField;
        Button searchBtn;
        Button clearBtn;
        InfinityGrid infinityGrid;
        Button cancelBtn;
        Button confirmBtn;
        #endregion
        #region 数据
        List<Sys_Society.RoleInfo> curSelectedFirendList = new List<Sys_Society.RoleInfo>();
        List<Sys_Society.RoleInfo> roleInfos;
        List<Sys_Society.RoleInfo> searchTheRoleList = new List<Sys_Society.RoleInfo>();
        string curSearchContent = string.Empty;
        AchievementDataCell achData;
        bool isSearch;
        StringBuilder tempStringBuilder;
        #endregion
        #region 查找组件、注册事件
        private void OnParseComponent()
        {
            closeBtn = transform.Find("Animator/View_TipsBgNew07/Btn_Close").GetComponent<Button>();
            inputField = transform.Find("Animator/InputField").GetComponent<InputField>();
            clearBtn = transform.Find("Animator/InputField/Btn_Clear").GetComponent<Button>();
            searchBtn = transform.Find("Animator/Button_Sreach").GetComponent<Button>();
            infinityGrid = transform.Find("Animator/Scroll View").GetComponent<InfinityGrid>();
            cancelBtn = transform.Find("Animator/Btn_Cancel").GetComponent<Button>();
            confirmBtn = transform.Find("Animator/Btn_Confirm").GetComponent<Button>();

            closeBtn.onClick.AddListener(() => { CloseSelf(); });
            inputField.onValueChanged.AddListener(InputFieldChanged);
            clearBtn.onClick.AddListener(ClearContent);
            searchBtn.onClick.AddListener(SearchOnClick);
            cancelBtn.onClick.AddListener(OnCancel);
            confirmBtn.onClick.AddListener(OnConfirm);

            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;
        }
        #endregion
        #region 初始化
        private void InitView()
        {
            isSearch = false;
            roleInfos = Sys_Achievement.Instance.GetAllFriends();
            DefaultShow();
        }
        #endregion
        #region 界面显示
        private void InputFieldChanged(string content)
        {
            curSearchContent = content;
            if (isSearch)
            {
                if (!string.IsNullOrEmpty(curSearchContent))
                    SearchOnClick();
                else
                {
                    isSearch = false;
                    DefaultShow();
                }
            }
        }
        private void ClearContent()
        {
            if (!string.IsNullOrEmpty(curSearchContent))
            {
                isSearch = false;
                inputField.text = string.Empty;
                curSearchContent = string.Empty;
                DefaultShow();
            }
        }
        private void DefaultShow()
        {
            searchTheRoleList.Clear();
            searchTheRoleList.AddRange(roleInfos);
            RefreshFriendCell();
        }
        private void SearchOnClick()
        {
            if (string.IsNullOrEmpty(curSearchContent))
                return;
            isSearch = true;
            searchTheRoleList.Clear();
            for (int i = 0; i < roleInfos.Count; i++)
            {
                if (roleInfos[i].roleName.Contains(curSearchContent) || roleInfos[i].roleID.ToString().Contains(curSearchContent))
                {
                    searchTheRoleList.Add(roleInfos[i]);
                }
            }
            RefreshFriendCell();
        }
        private void OnCancel()
        {
            curSelectedFirendList.Clear();
            CloseSelf();
        }
        private void OnConfirm()
        {
            if (curSelectedFirendList.Count <= 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5881));
                return;
            }
            Sys_Society.Instance.inputCache.AddAchievement(Sys_Achievement.Instance.GetAchievementByTid(achData.tid));
            string content = string.Format("{0}[{1}]", LanguageHelper.GetTextContent(5878), achData.tid);
            if (tempStringBuilder == null)
                tempStringBuilder = StringBuilderPool.GetTemporary();
            tempStringBuilder.Clear();
            for (int i = 0; i < curSelectedFirendList.Count; i++)
            {
                Sys_Society.Instance.ReqChatSingle(curSelectedFirendList[i].roleID, content);
                tempStringBuilder.Append(curSelectedFirendList[i].roleName);
                if (i < curSelectedFirendList.Count - 1)
                    tempStringBuilder.Append("、");
            }
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5879, tempStringBuilder.ToString())) ;
            Sys_Society.Instance.inputCache.Clear();
            curSelectedFirendList.Clear();
            CloseSelf();
        }
        private void RefreshFriendCell()
        {
            infinityGrid.CellCount = searchTheRoleList.Count;
            infinityGrid.ForceRefreshActiveCell();
            infinityGrid.MoveToIndex(0);
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            FriendSearchCell entry = new FriendSearchCell();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            FriendSearchCell entry = cell.mUserData as FriendSearchCell;
            entry.SetData(searchTheRoleList[index], OnSelected);
        }
        private void OnSelected(Sys_Society.RoleInfo info, bool isSelected)
        {
            if (isSelected)
                curSelectedFirendList.Add(info);
            else
                curSelectedFirendList.Remove(info);
        }
        #endregion

        public class FriendSearchCell
        {
            Image headFram;
            Image head;
            Text textName;
            Image imgProp;
            Text textLv;
            Text textProfession;
            GameObject offObj;
            GameObject onLineObj;
            GameObject checkObj;
            Toggle toggle;
            Action<Sys_Society.RoleInfo, bool> action;
            Sys_Society.RoleInfo roleInfo;
            public void Init(Transform trans)
            {
                toggle = trans.GetComponent<Toggle>();
                onLineObj = trans.Find("Image_BG").gameObject;
                offObj = trans.Find("Image_OFF").gameObject;
                headFram = trans.Find("Head").GetComponent<Image>();
                head = trans.Find("Head/Icon").GetComponent<Image>();
                textName = trans.Find("Text_Name").GetComponent<Text>();
                imgProp = trans.Find("Image_Prop").GetComponent<Image>();
                textLv = trans.Find("Level").GetComponent<Text>();
                textProfession = trans.Find("Text_Profession").GetComponent<Text>();
                checkObj = trans.Find("Check").gameObject;
                checkObj.SetActive(false);
                toggle.isOn = false;
                toggle.onValueChanged.AddListener(OnSelected);
            }
            public void SetData(Sys_Society.RoleInfo roleInfo, Action<Sys_Society.RoleInfo, bool> action)
            {
                this.roleInfo = roleInfo;
                this.action = action;
                onLineObj.SetActive(roleInfo.isOnLine);
                offObj.SetActive(!roleInfo.isOnLine);
                ImageHelper.SetIcon(head, Sys_Head.Instance.GetHeadImageId(roleInfo.heroID, roleInfo.iconId));
                ImageHelper.SetIcon(headFram, CSVHeadframe.Instance.GetConfData(roleInfo.iconFrameId).HeadframeIcon, true);
                textName.text = roleInfo.roleName;
                textLv.text = LanguageHelper.GetTextContent(2011127, roleInfo.level.ToString());
                CSVCareer.Data careerData = CSVCareer.Instance.GetConfData(roleInfo.occ);
                ImageHelper.SetIcon(imgProp, careerData.icon);
                textProfession.text = LanguageHelper.GetTextContent(careerData.name);
            }
            private void OnSelected(bool isOn)
            {
                checkObj.SetActive(isOn);
                action?.Invoke(roleInfo, isOn);
            }
        }
    }
}