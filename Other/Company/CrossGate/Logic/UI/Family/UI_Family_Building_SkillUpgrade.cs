using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using Packet;
using System;

namespace Logic
{
    /// <summary> 家族建筑技能升级 </summary>
    public class UI_Family_Building_SkilUpgrade : UIBase
    {
        #region 界面组件
        /// <summary> 标题名 </summary>
        private Text text_TitleName;
        /// <summary> 建筑名称 </summary>
        private Text text_BuildingName;
        /// <summary> 建筑等级 </summary>
        private Text text_BuildingLevel;
        /// <summary> 建筑技能名称 </summary>
        private Text text_BuildingSkillName;
        /// <summary> 建筑技能等级 </summary>
        private Text text_BuildingSkillLevel;
        /// <summary> 建筑技能描述 </summary>
        private Text text_BuildingSkillDes;
        /// <summary> 建筑技能图标 </summary>
        private Image image_BuildingSkillIcon;
        /// <summary> 花费节点 </summary>
        private GameObject go_Cost;
        /// <summary> 升级按钮 </summary>
        private Button button_Upgrade;
        /// <summary> 历练按钮 </summary>
        private Button button_Training;
        /// <summary> 完成节点 </summary>
        private GameObject go_Finish;
        /// <summary> 信息界面 </summary>
        private GameObject go_InfoView;
        /// <summary> 建筑显示信息 </summary>
        private List<GameObject> list_ShowInfo = new List<GameObject>();
        /// <summary> 建筑模版 </summary>
        private Toggle toggle_BuildingItem;
        /// <summary> 建筑技能模版列表 </summary>
        private List<Toggle> list_BuildingSkillItem = new List<Toggle>();
        /// <summary> 升级进度 </summary>
        private SkillUpgradeProgress skillUpgradeProgress;
        #endregion
        #region 数据定义
        /// <summary> 建筑技能升级显示类型 </summary>
        public enum BuildingSkillUpShowType
        {
            CurEffect = 0,   //当前效果 
            NextEffect = 1,  //下个效果
            Time = 2,        //时间
            Condition = 3,   //条件
            Progress = 4,    //进度
        }
        /// <summary> 升级进度(循环调用单独列出) </summary>
        public class SkillUpgradeProgress
        {
            public Text text_Time;        //剩余时间
            public Text text_Progress;    //升级进度
            public Slider slider_Progress;//升级进度
        }
        /// <summary> 当前建筑数据 </summary>
        public Sys_Family.FamilyData.EBuildingIndex eBuildingIndex { get; set; }
        /// <summary> 建筑数据 </summary>
        public GuildDetailInfo.Types.Building building { get; set; } = null;
        /// <summary> 建筑技能Id列表 </summary>
        public List<uint> list_SkillId = new List<uint>();
        /// <summary> 当前建筑技能下标 </summary>
        public int skillIndex { get; set; } = 0;
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
            eBuildingIndex = arg == null ? Sys_Family.FamilyData.EBuildingIndex.Castle : (Sys_Family.FamilyData.EBuildingIndex)System.Convert.ToUInt32(arg);
            skillIndex = 0;
        }
        protected override void OnShow()
        {
            OnUpdateBuildSkillState();
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
            text_TitleName = transform.Find("Animator/View_TipsBg02_Big/Text_Title").GetComponent<Text>();
            text_BuildingName = transform.Find("Animator/View_Left/Object_Text/Text_Property0").GetComponent<Text>();
            text_BuildingLevel = transform.Find("Animator/View_Left/Object_Text/Text_Property0/Text_Num").GetComponent<Text>();
            text_BuildingSkillName = transform.Find("Animator/View_Right/Object_Des01/Text_Name").GetComponent<Text>();
            text_BuildingSkillLevel = transform.Find("Animator/View_Right/Object_Des01/Text_Grade/Text").GetComponent<Text>();
            text_BuildingSkillDes = transform.Find("Animator/View_Right/Object_Des01/Text_Des").GetComponent<Text>();
            image_BuildingSkillIcon = transform.Find("Animator/View_Right/Object_Des01/Image_BG/Image_Icon").GetComponent<Image>();
            go_Cost = transform.Find("Animator/View_Right/Text_Cost").gameObject;
            button_Upgrade = transform.Find("Animator/View_Right/Button_Grade").GetComponent<Button>();
            button_Training = transform.Find("Animator/View_Right/Button_Look").GetComponent<Button>();
            go_Finish = transform.Find("Animator/View_Right/Image_Finished").gameObject;
            go_InfoView = transform.Find("Animator/View_Right").gameObject;
            toggle_BuildingItem = transform.Find("Animator/View_Left/Scroll_Grade/Grid/Item").GetComponent<Toggle>();
            var values = System.Enum.GetValues(typeof(BuildingSkillUpShowType));
            for (int i = 0, count = values.Length; i < count; i++)
            {
                BuildingSkillUpShowType type = (BuildingSkillUpShowType)values.GetValue(i);
                GameObject go = null;
                switch (type)
                {
                    case BuildingSkillUpShowType.CurEffect: { go = transform.Find("Animator/View_Right/Object_Des01/Scroll_View/View_Des/Object_Current").gameObject; } break;
                    case BuildingSkillUpShowType.NextEffect: { go = transform.Find("Animator/View_Right/Object_Des01/Scroll_View/View_Des/Object_Next").gameObject; } break;
                    case BuildingSkillUpShowType.Time: { go = transform.Find("Animator/View_Right/Object_Des01/Scroll_View/View_Des/Object_Time").gameObject; } break;
                    case BuildingSkillUpShowType.Condition: { go = transform.Find("Animator/View_Right/Object_Des01/Scroll_View/View_Des/Object_Condition").gameObject; } break;
                    case BuildingSkillUpShowType.Progress: { go = transform.Find("Animator/View_Right/Object_Des01/Scroll_View/View_Des/Object_Progress").gameObject; } break;
                }
                list_ShowInfo.Add(go);
            }

            skillUpgradeProgress = new SkillUpgradeProgress();
            skillUpgradeProgress.text_Time = transform.Find("Animator/View_Right/Object_Des01/Scroll_View/View_Des/Object_Progress/Image_Title/Text_Time").GetComponent<Text>();
            skillUpgradeProgress.text_Progress = transform.Find("Animator/View_Right/Object_Des01/Scroll_View/View_Des/Object_Progress/Text_Progress (1)").GetComponent<Text>();
            skillUpgradeProgress.slider_Progress = transform.Find("Animator/View_Right/Object_Des01/Scroll_View/View_Des/Object_Progress/Image_Percent").GetComponent<Slider>();

            transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
            button_Upgrade.onClick.AddListener(OnClick_Upgrade);
            button_Training.onClick.AddListener(OnClick_Training);
        }
        /// <summary>
        /// 创建模版列表
        /// </summary>
        private void CreateItemList()
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject go = i == 0 ? toggle_BuildingItem.gameObject : GameObject.Instantiate(toggle_BuildingItem.gameObject, toggle_BuildingItem.transform.parent);
                Toggle toggle = go.GetComponent<Toggle>();
                toggle.isOn = false;
                toggle.onValueChanged.AddListener((bool value) => { if (value) { OnClick_BuildingItem(toggle); } });
                list_BuildingSkillItem.Add(toggle);
            }
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateFamilyInfo, OnUpdateBuildSkillState, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateBuildSkillState, OnUpdateBuildSkillState, toRegister);
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        private void SetData()
        {
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            list_SkillId.Clear();
            //服务器建筑数据
            var allBuilds = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllBuildings;
            building = allBuilds.Count > (int)eBuildingIndex ? allBuilds[(int)eBuildingIndex] : null;
            //建筑技能列表
            var buildSkill = Sys_Family.Instance.familyData.familyBuildInfo.list_BuildSkill[(int)eBuildingIndex];
            //获取技能列表

            List<uint> list_SkillIdKeys = new List<uint>(buildSkill.dict_SkillId.Keys);
            for (int i = 0, icount = list_SkillIdKeys.Count; i < icount; i++)
            {
                uint id = list_SkillIdKeys[i];
                var item = buildSkill.dict_SkillId[id];
                if (item.Count <= 0) continue;
                uint targetSkillId = item[0];

                for (int k = 0, kcount = building.SkillMap.Count; k < kcount; k++)
                {
                    uint skillId = building.SkillMap[k];
                    if (skillId / 1000 == targetSkillId / 1000)
                    {
                        targetSkillId = skillId; //同个技能替代
                        break;
                    }
                }
                list_SkillId.Add(targetSkillId);
            }
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            uint id = (uint)eBuildingIndex * 100 + (building == null ? 0 : building.Lvl);
            CSVFamilyArchitecture.Data cSVFamilyArchitectureData = CSVFamilyArchitecture.Instance.GetConfData(id);
            if (null == cSVFamilyArchitectureData)
            {
                text_TitleName.text = string.Empty;
                text_BuildingName.text = string.Empty;
                text_BuildingLevel.text = string.Empty;
            }
            else
            {
                text_TitleName.text = LanguageHelper.GetTextContent(cSVFamilyArchitectureData.Name);
                text_BuildingName.text = LanguageHelper.GetTextContent(cSVFamilyArchitectureData.Name);
                text_BuildingLevel.text = LanguageHelper.GetTextContent(10087, cSVFamilyArchitectureData.BuildLevel.ToString());
            }


            for (int i = 0, count = list_BuildingSkillItem.Count; i < count; i++)
            {
                Transform tr = list_BuildingSkillItem[i].transform;
                bool isShow = i < list_SkillId.Count;
                if (isShow)
                {
                    tr.gameObject.SetActive(true);
                    SetBuildingSkillItem(tr, list_SkillId[i]);
                }
                else
                {
                    tr.gameObject.SetActive(false);
                }
            }
            SetMenu(skillIndex);
        }
        /// <summary>
        /// 设置选中建筑界面
        /// </summary>
        /// <param name="id"></param>
        /// <param name="maxId"></param>
        private void SetSelectBuildingView(uint id)
        {
            CSVFamilySkillUp.Data cSVFamilySkillUpData = CSVFamilySkillUp.Instance.GetConfData(id);
            if (null == cSVFamilySkillUpData || null == building)
            {
                go_InfoView.SetActive(false);
                return;
            }
            go_InfoView.SetActive(true);

            /// <summary> 技能等级 </summary>
            uint lv = cSVFamilySkillUpData.SkillLevel;
            /// <summary> 最大等级 </summary>
            uint maxLv = cSVFamilySkillUpData.Maxlevel;
            /// <summary> 是否解锁 </summary>
            //bool isLock = building.Lvl <= 0;
            /// <summary> 是否达到最大等级 </summary>
            bool isMaxLv = lv == maxLv;
            /// <summary> 建筑等级是否达到条件 </summary>
            bool isBuildConditions = cSVFamilySkillUpData.UpgradeConditions <= building.Lvl;
            /// <summary> 能否能激活 </summary>
            bool canActive = isBuildConditions && !isMaxLv;
            /// <summary> 是否当前技能在升级 </summary>
            bool isUpgrading = building.NowUpgrade == cSVFamilySkillUpData.id;
            /// <summary> 是否有其他技能在升级 </summary>
            bool isOtherUpgrading = building.NowUpgrade >= 0;

            text_BuildingSkillName.text = LanguageHelper.GetTextContent(cSVFamilySkillUpData.SkillName);
            text_BuildingSkillLevel.text = LanguageHelper.GetTextContent(10087, cSVFamilySkillUpData.SkillLevel.ToString());
            text_BuildingSkillDes.text = LanguageHelper.GetTextContent(cSVFamilySkillUpData.SkillLag);
            ImageHelper.SetIcon(image_BuildingSkillIcon, cSVFamilySkillUpData.SkillIcon);

            for (int i = 0; i < list_ShowInfo.Count; i++)
            {
                Transform tr = list_ShowInfo[i].transform;
                switch ((BuildingSkillUpShowType)i)
                {
                    case BuildingSkillUpShowType.CurEffect:
                        {
                            float Parameter = Sys_Family.Instance.familyData.GetSkillValue(cSVFamilySkillUpData, 100.0f);
                            tr.gameObject.SetActive(true);
                            SetShowInfo(tr, LanguageHelper.GetTextContent(cSVFamilySkillUpData.BulidName, Parameter.ToString()));
                        }
                        break;
                    case BuildingSkillUpShowType.NextEffect:
                        {
                            bool isShow = !isMaxLv;
                            tr.gameObject.SetActive(isShow);
                            CSVFamilySkillUp.Data nextcSVFamilySkillUpData = CSVFamilySkillUp.Instance.GetConfData(id + 1);
                            if (isShow && null != nextcSVFamilySkillUpData)
                            {
                                float Parameter = Sys_Family.Instance.familyData.GetSkillValue(nextcSVFamilySkillUpData, 100.0f);
                                SetShowInfo(tr, LanguageHelper.GetTextContent(nextcSVFamilySkillUpData.CurrentEffectDescription, Parameter.ToString()));
                            }
                        }
                        break;
                    case BuildingSkillUpShowType.Time:
                        {
                            bool isShow = !isMaxLv && !isUpgrading;
                            tr.gameObject.SetActive(isShow);
                            if (isShow)
                            {
                                float time = Sys_Family.Instance.GetBuildUpgradeTotalTime(cSVFamilySkillUpData.UpgradeTime);
                                SetShowInfo(tr, LanguageHelper.TimeToString((uint)time, LanguageHelper.TimeFormat.Type_4));
                            }
                        }
                        break;
                    case BuildingSkillUpShowType.Condition:
                        {
                            bool isShow = !isMaxLv && !isUpgrading;
                            tr.gameObject.SetActive(isShow);
                            if (isShow)
                            {
                                SetShowInfo(tr, LanguageHelper.GetTextContent(cSVFamilySkillUpData.Upgrade, cSVFamilySkillUpData.UpgradeConditions.ToString()));
                            }
                        }
                        break;
                    case BuildingSkillUpShowType.Progress:
                        {
                            tr.gameObject.SetActive(isUpgrading);
                            if (isUpgrading)
                            {
                                updateAction = () =>
                                {
                                    float upgradeTime = Sys_Family.Instance.GetBuildUpgradeResidueTime(building.UpgradeFinishTime);
                                    float totalTime = Sys_Family.Instance.GetBuildUpgradeTotalTime(cSVFamilySkillUpData.UpgradeTime);
                                    if (upgradeTime < 0f)
                                    {
                                        upgradeTime = 0F;
                                        updateAction = null;
                                    }
                                    float value = totalTime <= 0 ? 0 : upgradeTime / totalTime;
                                    if (value >= 1f) value = 1f;
                                    SetProgressInfo(skillUpgradeProgress, LanguageHelper.TimeToString((uint)upgradeTime, LanguageHelper.TimeFormat.Type_4), 1f - value);
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
            if (isUpgrading || isMaxLv)
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
            go_Finish.SetActive(isMaxLv);
            go_Cost.SetActive(!isMaxLv);
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            if (!isMaxLv)
            {
                string strCost = string.Format("{0}/{1}",
                  Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.GuildCoin,
                  cSVFamilySkillUpData.UpgradeCost);
                SetCostInfo(go_Cost.transform, strCost);
            }
            button_Training.gameObject.SetActive(eBuildingIndex == Sys_Family.FamilyData.EBuildingIndex.MilitaryAcademy);
        }
        /// <summary>
        /// 设置建筑技能模版
        /// </summary>
        /// <param name="tr"></param>
        private void SetBuildingSkillItem(Transform tr, uint skillId)
        {
            CSVFamilySkillUp.Data cSVFamilySkillUpData = CSVFamilySkillUp.Instance.GetConfData(skillId);
            if (null == cSVFamilySkillUpData || null == building)
            {
                tr.gameObject.SetActive(false);
                return;
            }
            tr.gameObject.SetActive(true);
            tr.name = skillId.ToString();

            /// <summary> 技能等级 </summary>
            uint lv = cSVFamilySkillUpData.SkillLevel;
            /// <summary> 最大等级 </summary>
            uint maxLv = cSVFamilySkillUpData.Maxlevel;
            /// <summary> 是否解锁 </summary>
            //bool isLock = building.Lvl <= 0;
            /// <summary> 是否激活 </summary>
            bool isActived = building.SkillMap.Contains(skillId);
            /// <summary> 是否达到最大等级 </summary>
            bool isMaxLv = lv == maxLv;
            /// <summary> 建筑等级是否达到条件 </summary>
            bool isBuildConditions = cSVFamilySkillUpData.UpgradeConditions <= building.Lvl;
            /// <summary> 能否能激活 </summary>
            bool canActived =  isBuildConditions && !isMaxLv && !isActived;

            /// <summary> 技能图标 </summary>
            Image image_Icon = tr.Find("Image_BG/Image_Icon").GetComponent<Image>();
            ImageHelper.SetIcon(image_Icon, cSVFamilySkillUpData.SkillIcon);
            /// <summary> 技能名称 </summary>
            Text text_Name = tr.Find("Text_Name").GetComponent<Text>();
            text_Name.text = LanguageHelper.GetTextContent(cSVFamilySkillUpData.SkillName);
            /// <summary> 技能等级 </summary>
            GameObject go_Lv = tr.Find("Text_Level").gameObject;
            go_Lv.SetActive(isBuildConditions && !isMaxLv);
            /// <summary> 技能等级 </summary>
            Text text_Lv = tr.Find("Text_Level/Text").GetComponent<Text>();
            text_Lv.text = string.Format("{0}/{1}", lv, maxLv);
            /// <summary> 技能未开放 </summary>
            GameObject go_Lock = tr.Find("Text_Lock").gameObject;
            go_Lock.SetActive(!isBuildConditions);
            /// <summary> 技能满级 </summary>
            GameObject go_Full = tr.Find("Text_Full").gameObject;
            go_Full.SetActive(isMaxLv);
            /// <summary> 建筑可升级标记 </summary>
            GameObject go_GradeMark = tr.Find("Image_Grade").gameObject;
            go_GradeMark.SetActive(canActived);
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
        private void SetProgressInfo(SkillUpgradeProgress skillUpgradeProgress, string str, float value)
        {
            skillUpgradeProgress.text_Time.text = str;
            skillUpgradeProgress.text_Progress.text = string.Format("{0:F1}%", value * 100);
            skillUpgradeProgress.slider_Progress.value = value;
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
            UIManager.HitButton(EUIID.UI_Family_BuildSkillUpgrade, "OnClick_Close");
            CloseSelf();
        }
        /// <summary>
        /// 升级
        /// </summary>
        private void OnClick_Upgrade()
        {
            UIManager.HitButton(EUIID.UI_Family_BuildSkillUpgrade, "OnClick_Upgrade");
            if (!Sys_Family.Instance.familyData.GetMyPostAuthority(Sys_Family.FamilyData.EFamilyAuthority.BuildingUp))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10072));
                return;
            }
            Sys_Family.Instance.SendGuildUpgradeWorkshopSkillReq(list_SkillId[skillIndex]);
        }
        /// <summary>
        /// 历练界面
        /// </summary>
        private void OnClick_Training()
        {
            UIManager.HitButton(EUIID.UI_Family_BuildSkillUpgrade, "OnClick_Training");
            Sys_Experience.Instance.InfoReq();
            UIManager.OpenUI(EUIID.UI_Family_Empowerment);
        }
        /// <summary>
        /// 点击建筑菜单
        /// </summary>
        /// <param name="toggle"></param>
        private void OnClick_BuildingItem(Toggle toggle)
        {
            
            int index = list_BuildingSkillItem.IndexOf(toggle);
            skillIndex = index;
            uint skillId = index >= list_SkillId.Count ? 0 : list_SkillId[skillIndex];
            UIManager.HitButton(EUIID.UI_Family_BuildSkillUpgrade, "OnClick_BuildingItem:" + skillId.ToString());
            SetSelectBuildingView(skillId);
        }
        /// <summary>
        /// 更新升级技能状态
        /// </summary>
        private void OnUpdateBuildSkillState()
        {
            SetData();
            RefreshView();
        }
        #endregion
        #region 提供功能
        /// <summary>
        /// 设置菜单
        /// </summary>
        /// <param name="menu"></param>
        private void SetMenu(int index)
        {
            Toggle toggle = list_BuildingSkillItem[index];
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