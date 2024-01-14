using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using Packet;
using Lib.Core;
using System;

namespace Logic
{
    /// <summary> 家族建筑升级 </summary>
    public class UI_Family_Building_LvUpgrade : UIBase
    {
        #region 界面组件

        /// <summary> 建筑名称 </summary>
        private Text text_BuildingName;
        /// <summary> 建筑等级 </summary>
        private Text text_BuildingLevel;
        /// <summary> 建筑标题名称 </summary>
        private Text text_BuildingTitleName;
        /// <summary> 建筑标题描述 </summary>
        private Text text_BuildingTitleDes;
        /// <summary> 花费节点 </summary>
        private GameObject go_Cost;
        /// <summary> 升级按钮 </summary>
        private Button button_Upgrade;
        /// <summary> 完成节点 </summary>
        private GameObject go_Finish;
        /// <summary> 建筑显示信息 </summary>
        private List<GameObject> list_ShowInfo = new List<GameObject>();
        /// <summary> 建筑线 </summary>
        private List<GameObject> list_Line = new List<GameObject>();
        /// <summary> 建筑模版 </summary>
        private Toggle toggle_BuildingItem;
        /// <summary> 建筑模版列表 </summary>
        private List<Toggle> list_BuildingItem = new List<Toggle>();
        /// <summary> 升级进度 </summary>
        private LvUpgradeProgress lvUpgradeProgress;
        #endregion
        #region 数据定义
        /// <summary> 建筑升级显示类型 </summary>
        public enum BuildingLvUpShowType
        {
            Level = 0,       //等级  
            Time = 1,        //时间
            Condition = 2,   //条件
            Progress = 3,    //进度
        }
        /// <summary> 升级进度(循环调用单独列出) </summary>
        public class LvUpgradeProgress
        {
            public Text text_Time;        //剩余时间
            public Text text_Progress;    //升级进度
            public Slider slider_Progress;//升级进度
        }
        /// <summary> 当前建筑数据 </summary>
        public Sys_Family.FamilyData.EBuildingIndex eBuildingIndex { get; set; }
        /// <summary> 更新行为 </summary>
        public Action updateAction = null;
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
            uint index = arg == null ? 0 : System.Convert.ToUInt32(arg);
            eBuildingIndex = (Sys_Family.FamilyData.EBuildingIndex)index;
        }
        protected override void OnShow()
        {
            OnUpdateBuildState();
        }
        protected override void OnHide()
        {

        }
        protected override void OnUpdate()
        {
            updateAction?.Invoke();
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
            text_BuildingName = transform.Find("Animator/View_Left/Object_Text/Text_Property0").GetComponent<Text>();
            text_BuildingLevel = transform.Find("Animator/View_Left/Object_Text/Text_Property0/Text_Num").GetComponent<Text>();
            text_BuildingTitleName = transform.Find("Animator/View_Right/Object_Des01/Text_Name").GetComponent<Text>();
            text_BuildingTitleDes = transform.Find("Animator/View_Right/Object_Des01/Text_Des").GetComponent<Text>();
            go_Cost = transform.Find("Animator/View_Right/Text_Cost").gameObject;
            button_Upgrade = transform.Find("Animator/View_Right/Button_Grade").GetComponent<Button>();
            go_Finish = transform.Find("Animator/View_Right/Image_Finished").gameObject;
            toggle_BuildingItem = transform.Find("Animator/View_Left/Scroll_Grade/Grid/Item").GetComponent<Toggle>();
            var values = System.Enum.GetValues(typeof(BuildingLvUpShowType));
            for (int i = 0, count = values.Length; i < count; i++)
            {
                BuildingLvUpShowType type = (BuildingLvUpShowType)values.GetValue(i);
                GameObject go = null;
                switch (type)
                {
                    case BuildingLvUpShowType.Level: { go = transform.Find("Animator/View_Right/Object_Des01/Scroll_View/View_Des/Object_Level").gameObject; } break;
                    case BuildingLvUpShowType.Time: { go = transform.Find("Animator/View_Right/Object_Des01/Scroll_View/View_Des/Object_Time").gameObject; } break;
                    case BuildingLvUpShowType.Condition: { go = transform.Find("Animator/View_Right/Object_Des01/Scroll_View/View_Des/Object_Condition").gameObject; } break;
                    case BuildingLvUpShowType.Progress: { go = transform.Find("Animator/View_Right/Object_Des01/Scroll_View/View_Des/Object_Progress").gameObject; } break;
                }
                list_ShowInfo.Add(go);
            }

            list_Line.Add(transform.Find("Animator/View_Right/Object_Des01/Scroll_View/View_Des/Object_Line").gameObject);
            list_Line.Add(transform.Find("Animator/View_Right/Image_Splitline1").gameObject);

            lvUpgradeProgress = new LvUpgradeProgress();
            lvUpgradeProgress.text_Time = transform.Find("Animator/View_Right/Object_Des01/Scroll_View/View_Des/Object_Progress/Image_Title/Text_Time").GetComponent<Text>();
            lvUpgradeProgress.text_Progress = transform.Find("Animator/View_Right/Object_Des01/Scroll_View/View_Des/Object_Progress/Text_Progress (1)").GetComponent<Text>();
            lvUpgradeProgress.slider_Progress = transform.Find("Animator/View_Right/Object_Des01/Scroll_View/View_Des/Object_Progress/Image_Percent").GetComponent<Slider>();

            transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
            button_Upgrade.onClick.AddListener(OnClick_Upgrade);
        }
        /// <summary>
        /// 创建模版列表
        /// </summary>
        private void CreateItemList()
        {
            var values = System.Enum.GetValues(typeof(Sys_Family.FamilyData.EBuildingIndex));
            for (int i = 0, count = values.Length; i < count; i++)
            {
                Sys_Family.FamilyData.EBuildingIndex type = (Sys_Family.FamilyData.EBuildingIndex)values.GetValue(i);

                if (type == Sys_Family.FamilyData.EBuildingIndex.TrainingAnimals ||
                   type == Sys_Family.FamilyData.EBuildingIndex.Barracks)
                    continue;//预留(未处理)

                GameObject go = type == Sys_Family.FamilyData.EBuildingIndex.Castle ? toggle_BuildingItem.gameObject : GameObject.Instantiate(toggle_BuildingItem.gameObject, toggle_BuildingItem.transform.parent);
                Toggle toggle = go.GetComponent<Toggle>();
                toggle.isOn = false;
                toggle.onValueChanged.AddListener((bool value) => { if (value) { OnClick_BuildingItem(toggle); } });
                list_BuildingItem.Add(toggle);
            }
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateFamilyInfo, OnUpdateBuildState, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateBuildState, OnUpdateBuildState, toRegister);
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            var allBuilds = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllBuildings;

            for (int i = 0, count = list_BuildingItem.Count; i < count; i++)
            {
                Transform tr = list_BuildingItem[i].transform;
                GuildDetailInfo.Types.Building building = allBuilds.Count > i ? allBuilds[i] : null;
                SetBuildingItem(tr, (Sys_Family.FamilyData.EBuildingIndex)i, building);
            }

            SetMenu(eBuildingIndex);
        }
        /// <summary>
        /// 设置选中建筑界面
        /// </summary>
        /// <param name="eBuildType"></param>
        private void SetSelectBuildingView(Sys_Family.FamilyData.EBuildingIndex eBuildType)
        {
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            var info = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info;
            var allBuilds = info.AllBuildings;
            GuildDetailInfo.Types.Building building = allBuilds.Count > (int)eBuildType ? allBuilds[(int)eBuildType] : null;
            uint id = (uint)eBuildType * 100 + (null == building ? 0 : building.Lvl);
            CSVFamilyArchitecture.Data cSVFamilyArchitectureData = CSVFamilyArchitecture.Instance.GetConfData(id);
            if (null == cSVFamilyArchitectureData || null == building) return;
            bool isMaxlevel = cSVFamilyArchitectureData.MaxLevel <= building.Lvl;
            bool isUpgrading = (uint)eBuildType == info.NowUpgrade;
            bool isOtherUpgrading = info.NowUpgrade >= 0;
            text_BuildingName.text = LanguageHelper.GetTextContent(cSVFamilyArchitectureData.Name);
            text_BuildingLevel.text = LanguageHelper.GetTextContent(10087, building.Lvl.ToString());

            text_BuildingTitleName.text = LanguageHelper.GetTextContent(cSVFamilyArchitectureData.Name);
            text_BuildingTitleDes.text = LanguageHelper.GetTextContent(cSVFamilyArchitectureData.Introduce);

            bool canActive = Sys_Family.Instance.familyData.isUpgradeByBuild(id);
            for (int i = 0; i < list_ShowInfo.Count; i++)
            {
                Transform tr = list_ShowInfo[i].transform;
                switch ((BuildingLvUpShowType)i)
                {
                    case BuildingLvUpShowType.Level:
                        {
                            tr.gameObject.SetActive(true);
                            SetShowInfo(tr, string.Format("{0}/{1}", building.Lvl, cSVFamilyArchitectureData.MaxLevel));
                        }
                        break;
                    case BuildingLvUpShowType.Time:
                        {
                            bool isShow = !isMaxlevel && !isUpgrading;
                            tr.gameObject.SetActive(isShow);
                            if (isShow)
                            {
                                float time = Sys_Family.Instance.GetBuildUpgradeTotalTime(cSVFamilyArchitectureData.UpgradeTime);
                                SetShowInfo(tr, LanguageHelper.TimeToString((uint)time, LanguageHelper.TimeFormat.Type_4));
                            }
                        }
                        break;
                    case BuildingLvUpShowType.Condition:
                        {
                            bool isShow = !isMaxlevel && !isUpgrading;
                            tr.gameObject.SetActive(isShow);
                            if (isShow)
                            {
                                if(cSVFamilyArchitectureData.demandProsperityLevel != 0 && cSVFamilyArchitectureData.DemandLevel != 0)
                                {
                                    SetShowInfo(tr, LanguageHelper.GetTextContent(cSVFamilyArchitectureData.BuildLag, cSVFamilyArchitectureData.DemandLevel.ToString(), cSVFamilyArchitectureData.demandProsperityLevel.ToString()));
                                }
                                else if(cSVFamilyArchitectureData.DemandLevel != 0)
                                {
                                    SetShowInfo(tr, LanguageHelper.GetTextContent(cSVFamilyArchitectureData.BuildLag, cSVFamilyArchitectureData.DemandLevel.ToString()));
                                }
                                else if (cSVFamilyArchitectureData.demandProsperityLevel != 0)
                                {
                                    SetShowInfo(tr, LanguageHelper.GetTextContent(cSVFamilyArchitectureData.BuildLag, cSVFamilyArchitectureData.demandProsperityLevel.ToString()));
                                }
                            }
                        }
                        break;
                    case BuildingLvUpShowType.Progress:
                        {
                            tr.gameObject.SetActive(isUpgrading);
                            if (isUpgrading)
                            {
                                updateAction = () =>
                                {
                                    float upgradeTime = Sys_Family.Instance.GetBuildUpgradeResidueTime(info.UpgradeFinishTime);
                                    float totalTime = Sys_Family.Instance.GetBuildUpgradeTotalTime(cSVFamilyArchitectureData.UpgradeTime);
                                    if (upgradeTime < 0f)
                                    {
                                        upgradeTime = 0f;
                                        updateAction = null;
                                    }
                                    float value = totalTime <= 0 ? 0 : upgradeTime / totalTime;
                                    if (value >= 1f) value = 1f;
                                    SetProgressInfo(lvUpgradeProgress, LanguageHelper.TimeToString((uint)upgradeTime, LanguageHelper.TimeFormat.Type_4), 1f - value);
                                };
                                updateAction?.Invoke();
                            }
                            else
                            {
                                updateAction = null;
                            }
                        }
                        break;
                }
            }
            if (isUpgrading || isMaxlevel)
            {
                button_Upgrade.gameObject.SetActive(false);
            }
            else if (isOtherUpgrading || !canActive)
            {
                button_Upgrade.gameObject.SetActive(true);
                ImageHelper.SetImageGray(button_Upgrade, true, true);
            }
            else
            {
                button_Upgrade.gameObject.SetActive(true);
                ImageHelper.SetImageGray(button_Upgrade, false, true);
            }
            go_Cost.SetActive(!(isMaxlevel || isUpgrading));
            go_Finish.SetActive(isMaxlevel);

            if (!isMaxlevel)
            {
                string strCost = string.Format("{0}/{1}",
                  Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.GuildCoin,
                  cSVFamilyArchitectureData.FundsRequired);
                SetCostInfo(go_Cost.transform, strCost);
            }

            list_Line[0].SetActive(isUpgrading);
            list_Line[1].SetActive(!isUpgrading);
        }
        /// <summary>
        /// 设置建筑模版
        /// </summary>
        /// <param name="tr"></param>
        private void SetBuildingItem(Transform tr, Sys_Family.FamilyData.EBuildingIndex eBuildType, GuildDetailInfo.Types.Building building)
        {
            uint lv = building.Lvl;
            uint id = (uint)eBuildType * 100 + lv;
            CSVFamilyArchitecture.Data cSVFamilyArchitectureData = CSVFamilyArchitecture.Instance.GetConfData(id);
            if (null == cSVFamilyArchitectureData || null == building) return;
            uint maxLv = cSVFamilyArchitectureData.MaxLevel;
            /// <summary> 建筑图标 </summary>
            Image image_Icon = tr.Find("Image_BG/Image_Icon").GetComponent<Image>();
            ImageHelper.SetIcon(image_Icon, cSVFamilyArchitectureData.iconId);
            /// <summary> 建筑姓名 </summary>
            Text text_Name = tr.Find("Text_Name").GetComponent<Text>();
            TextHelper.SetText(text_Name, cSVFamilyArchitectureData.Name);
            /// <summary> 建筑等级 </summary>
            GameObject go_Lv = tr.Find("Text_Level").gameObject;
            go_Lv.SetActive(lv < maxLv);
            /// <summary> 建筑等级 </summary>
            Text text_Lv = tr.Find("Text_Level/Text").GetComponent<Text>();
            text_Lv.text = string.Format("{0}/{1}", lv, maxLv);
            /// <summary> 建筑未开放 </summary>
            GameObject go_Lock = tr.Find("Text_Lock").gameObject;
            go_Lock.SetActive(false);
            /// <summary> 建筑满级 </summary>
            GameObject go_Full = tr.Find("Text_Full").gameObject;
            go_Full.SetActive(lv == maxLv);
            /// <summary> 建筑可升级标记 </summary>
            GameObject go_GradeMark = tr.Find("Image_Grade").gameObject;
            go_GradeMark.SetActive(Sys_Family.Instance.familyData.isUpgradeByBuild(id));
        }
        /// <summary>
        /// 设置显示内容
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="str"></param>
        private void SetShowInfo(Transform tr, string str)
        {
            Text text_Des = tr.Find("Image_Title/Text_Des").GetComponent<Text>();
            text_Des.text = str;
        }
        /// <summary>
        /// 设置进度内容
        /// </summary>
        /// <param name="lvUpgradeProgress"></param>
        /// <param name="str"></param>
        /// <param name="value"></param>
        private void SetProgressInfo(LvUpgradeProgress lvUpgradeProgress, string str, float value)
        {
            lvUpgradeProgress.text_Time.text = str;
            lvUpgradeProgress.text_Progress.text = string.Format("{0:F1}%", value * 100);
            lvUpgradeProgress.slider_Progress.value = value;
        }
        /// <summary>
        /// 设置花费内容
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="str"></param>
        private void SetCostInfo(Transform tr, string str)
        {
            Text text_Cost = tr.GetComponent<Text>();
            text_Cost.text = str;
            Image image_Cost = tr.Find("Image_Coin").GetComponent<Image>();
            CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData((int)ECurrencyType.FamilyCoin);
            if (null != cSVItemData)
                ImageHelper.SetIcon(image_Cost, cSVItemData.small_icon_id);
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            UIManager.HitButton(EUIID.UI_Family_BuildLvUpgrade, "OnClick_Close");
            CloseSelf();
        }
        /// <summary>
        /// 升级
        /// </summary>
        private void OnClick_Upgrade()
        {
            UIManager.HitButton(EUIID.UI_Family_BuildLvUpgrade, "OnClick_Upgrade");
            if (!Sys_Family.Instance.familyData.GetMyPostAuthority(Sys_Family.FamilyData.EFamilyAuthority.BuildingUp))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10072));
                return;
            }
            Sys_Family.Instance.SendGuildUpgradeBuildingReq((uint)eBuildingIndex);
        }
        /// <summary>
        /// 点击建筑菜单
        /// </summary>
        /// <param name="toggle"></param>
        private void OnClick_BuildingItem(Toggle toggle)
        {
            int index = list_BuildingItem.IndexOf(toggle);
            if (index < 0) return;
            UIManager.HitButton(EUIID.UI_Family_BuildLvUpgrade, "OnClick_BuildingItem:" + index.ToString());
            eBuildingIndex = (Sys_Family.FamilyData.EBuildingIndex)index;
            SetSelectBuildingView(eBuildingIndex);
        }
        /// <summary>
        /// 更新建筑状态
        /// </summary>
        private void OnUpdateBuildState()
        {
            RefreshView();
        }
        #endregion
        #region 提供功能
        /// <summary>
        /// 设置菜单
        /// </summary>
        /// <param name="menu"></param>
        private void SetMenu(Sys_Family.FamilyData.EBuildingIndex menu)
        {
            Toggle toggle = list_BuildingItem[(int)menu];
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