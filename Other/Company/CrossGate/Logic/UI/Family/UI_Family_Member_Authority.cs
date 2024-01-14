using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

namespace Logic
{
    /// <summary> 家族权限 </summary>
    public class UI_Family_Member_Authority : UIBase
    {
        #region 界面组件
        /// <summary> 职位模版 </summary>
        private Toggle toggle_StatusItem;
        /// <summary> 职位模版列表 </summary>
        private List<Toggle> list_StatusItem = new List<Toggle>();
        /// <summary> 权限 </summary>
        private List<Toggle> list_AuthorityItem = new List<Toggle>();
        #endregion
        #region 数据定义
        #endregion
        #region 系统函数
        protected override void OnInit()
        {
        }
        protected override void OnLoaded()
        {
            OnParseComponent();
            CreateItemList();
        }
        protected override void OnOpen(object arg)
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
            toggle_StatusItem = transform.Find("Animator/Scroll_View_Tab/Toggle_Tab/Toggle_Status").GetComponent<Toggle>();

            Transform tr_BaseAuthority = transform.Find("Animator/Scroll_View/View_Choice/Proto/SrollItem (1)");
            Transform tr_MemberManage = transform.Find("Animator/Scroll_View/View_Choice/Proto/SrollItem (2)");
            var list_BaseAuthority = tr_BaseAuthority.GetComponentsInChildren<Toggle>();
            var list_MemberManage = tr_MemberManage.GetComponentsInChildren<Toggle>();
            list_AuthorityItem.AddRange(list_BaseAuthority);
            list_AuthorityItem.AddRange(list_MemberManage);
            transform.Find("Animator/View_TipsBg01_Big/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
        }
        /// <summary>
        /// 创建模版列表
        /// </summary>
        private void CreateItemList()
        {
            //List<uint> statusList = new List<uint>(CSVFamilyPostAuthority.Instance.Count);
            //for (int i = 0; i < CSVFamilyPostAuthority.Instance.Count; i++)
            //{
            //    statusList.Add(CSVFamilyPostAuthority.Instance[i].id);
            //}
            //statusList.Reverse();

            //for (int i = 0; i < statusList.Count; i++)

            var dataList = CSVFamilyPostAuthority.Instance.GetAll();
            for (int i = dataList.Count - 1; i >= 0; --i)
            {
                GameObject go = i == 0 ? toggle_StatusItem.gameObject : GameObject.Instantiate(toggle_StatusItem.gameObject, toggle_StatusItem.transform.parent);
                SetStatusItem(go.transform, dataList[i]);
                Toggle toggle = go.GetComponent<Toggle>();
                toggle.isOn = false;
                toggle.onValueChanged.AddListener((bool value) => { if (value) { OnClick_Status(toggle); } });
                list_StatusItem.Add(toggle);
            }
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {

        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            SetDefaultToggle();
        }
        /// <summary>
        /// 设置默认
        /// </summary>
        private void SetDefaultToggle()
        {
            SetSelectToggle(0);
        }
        /// <summary>
        /// 设置职位
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="id"></param>
        private void SetStatusItem(Transform tr, CSVFamilyPostAuthority.Data cSVFamilyPostAuthorityData)//(Transform tr, uint id)
        {
            //CSVFamilyPostAuthority.Data cSVFamilyPostAuthorityData = CSVFamilyPostAuthority.Instance.GetConfData(id);
            if (null == cSVFamilyPostAuthorityData) return;
            tr.name = cSVFamilyPostAuthorityData.id.ToString();
            /// <summary> 职位名称 </summary>
            Text text_Name = tr.Find("Text").GetComponent<Text>();
            text_Name.text = LanguageHelper.GetTextContent(cSVFamilyPostAuthorityData.PostName);
            /// <summary> 职位名称 </summary>
            Text text_SelectName = tr.Find("Text_Select").GetComponent<Text>();
            text_SelectName.text = LanguageHelper.GetTextContent(cSVFamilyPostAuthorityData.PostName);
        }
        /// <summary>
        /// 设置权限模版
        /// </summary>
        /// <param name="id"></param>
        private void SetAuthorityItem(uint id)
        {
            
            CSVFamilyPostAuthority.Data cSVFamilyPostAuthorityData = CSVFamilyPostAuthority.Instance.GetConfData(id);
            if (null == cSVFamilyPostAuthorityData) return;
            var t = typeof(CSVFamilyPostAuthority.Data);

            for (int i = 0, count = list_AuthorityItem.Count; i < count; i++)
            {
                var child = list_AuthorityItem[i];
                bool value = Sys_Family.Instance.familyData.GetPostAuthority((Sys_Family.FamilyData.EFamilyStatus)id, GetEFamilyAuthorityByString(child.name));
                SetAuthorityItem(child, value);
            }
        }

           //TODO -原反射设计无法使用临时处理
        private Sys_Family.FamilyData.EFamilyAuthority GetEFamilyAuthorityByString(string str)
        {
            switch (str)
            {
                case "IsAppointment": return Sys_Family.FamilyData.EFamilyAuthority.IsAppointment;
                case "ModifyName": return Sys_Family.FamilyData.EFamilyAuthority.ModifyName;
                case "BuildingUp": return Sys_Family.FamilyData.EFamilyAuthority.BuildingUp;
                case "ModifyDeclaration": return Sys_Family.FamilyData.EFamilyAuthority.ModifyDeclaration;
                case "GroupMessage": return Sys_Family.FamilyData.EFamilyAuthority.GroupMessage;
                case "InitiataMerger": return Sys_Family.FamilyData.EFamilyAuthority.InitiataMerger;
                case "AcceptMerger": return Sys_Family.FamilyData.EFamilyAuthority.AcceptMerger;
                case "EstablishBranch": return Sys_Family.FamilyData.EFamilyAuthority.EstablishBranch;
                case "RemoveBranch": return Sys_Family.FamilyData.EFamilyAuthority.RemoveBranch;
                case "MergeBranch": return Sys_Family.FamilyData.EFamilyAuthority.MergeBranch;
                case "Invitation": return Sys_Family.FamilyData.EFamilyAuthority.Invitation;
                case "ApplicationAcceptance": return Sys_Family.FamilyData.EFamilyAuthority.ApplicationAcceptance;
                case "ModifyApproval": return Sys_Family.FamilyData.EFamilyAuthority.ModifyApproval;
                case "ModifyApprovalLevel": return Sys_Family.FamilyData.EFamilyAuthority.ModifyApprovalLevel;
                case "Worker": return Sys_Family.FamilyData.EFamilyAuthority.Worker;
                case "IsForbiddenWords": return Sys_Family.FamilyData.EFamilyAuthority.IsForbiddenWords;
                case "Clear": return Sys_Family.FamilyData.EFamilyAuthority.Clear;
                case "BattleEnroll": return Sys_Family.FamilyData.EFamilyAuthority.BattleEnroll;
                case "FamilyPetName": return Sys_Family.FamilyData.EFamilyAuthority.FamilyPetName;
                case "FamilyPetNotice": return Sys_Family.FamilyData.EFamilyAuthority.FamilyPetNotice;
                case "FamilyPetEgg": return Sys_Family.FamilyData.EFamilyAuthority.FamilyPetEgg;
                case "FamilyPetTraining": return Sys_Family.FamilyData.EFamilyAuthority.FamilyPetTraining;
                case "FamilyBonus": return Sys_Family.FamilyData.EFamilyAuthority.FamilyBonus;
                default:
                    return Sys_Family.FamilyData.EFamilyAuthority.IsAppointment;
            }
        }
        /// <summary>
        /// 设置权限模版
        /// </summary>
        /// <param name="toggle"></param>
        /// <param name="value"></param>
        private void SetAuthorityItem(Toggle toggle, bool value)
        {
            toggle.isOn = value;
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            UIManager.HitButton(EUIID.UI_Family_ApplyList, "OnClick_Close");
            CloseSelf();
        }
        /// <summary>
        /// 点击职位
        /// </summary>
        /// <param name="toggle"></param>
        public void OnClick_Status(Toggle toggle)
        {
            uint id = 0;
            if (!uint.TryParse(toggle.name, out id)) return;
            UIManager.HitButton(EUIID.UI_Family_ApplyList, "OnClick_Status");
            SetAuthorityItem(id);
        }
        #endregion
        #region 提供功能
        /// <summary>
        /// 设置单选模版
        /// </summary>
        /// <param name="index"></param>
        private void SetSelectToggle(int index)
        {
            if (index < 0 || index >= list_StatusItem.Count)
                return;

            Toggle toggle = list_StatusItem[index];
            if (!toggle.isOn)
            {
                toggle.isOn = true;
            }
            else
            {
                toggle.onValueChanged.Invoke(true);
            }
        }
        #endregion
    }
}