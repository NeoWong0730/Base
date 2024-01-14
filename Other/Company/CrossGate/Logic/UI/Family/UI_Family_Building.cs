using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine;
using UnityEngine.UI;
using Packet;
using Lib.Core;

namespace Logic
{
    /// <summary> 家族建筑子界面 </summary>
    public class UI_Family_Building : UIComponent
    {
        #region 界面组件
        /// <summary> 建筑列表 </summary>
        private List<GameObject> list_Building = new List<GameObject>();
        #endregion
        #region 数据定义
        /// <summary> 建筑数据 </summary>
        private List<GuildDetailInfo.Types.Building> list_AllBuildings = new List<GuildDetailInfo.Types.Building>();
        #endregion
        #region 系统函数
        protected override void Loaded()
        {
            OnParseComponent();
        }
        public override void Show()
        {
            SetData();
            RefreshView();
        }
        public override void Hide()
        {
        }
        protected override void Update()
        {
        }
        protected override void Refresh()
        {

        }
        #endregion
        #region 初始化
        /// <summary>
        /// 检测组件
        /// </summary>
        private void OnParseComponent()
        {
            /// <summary> 城堡 </summary>
            GameObject go_Hall = transform.Find("Scroll_View/Viewport/View_Hall").gameObject;
            list_Building.Add(go_Hall);
            /// <summary> 银行 </summary>
            GameObject go_Storage = transform.Find("Scroll_View/Viewport/View_Storage").gameObject;
            list_Building.Add(go_Storage);
            /// <summary> 科学院 </summary>
            GameObject go_Military = transform.Find("Scroll_View/Viewport/View_Military").gameObject;
            list_Building.Add(go_Military);
            /// <summary> 农舍 </summary>
            GameObject go_People = transform.Find("Scroll_View/Viewport/View_Camp").gameObject;
            list_Building.Add(go_People);
            /// <summary> 军事院 </summary>
            GameObject go_Research = transform.Find("Scroll_View/Viewport/View_Research").gameObject;
            list_Building.Add(go_Research);
            /// <summary> 驯兽栏 </summary>
            GameObject go_Training = transform.Find("Scroll_View/Viewport/View_Training").gameObject;
            list_Building.Add(go_Training);
            /// <summary> 军营 </summary>
            GameObject go_Camp = transform.Find("Scroll_View/Viewport/View_People").gameObject;
            list_Building.Add(go_Camp);

            go_Hall.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => { OnClick_Building(go_Hall); });
            go_People.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => { OnClick_Building(go_People); });
            go_Research.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => { OnClick_Building(go_Research); });
            go_Storage.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => { OnClick_Building(go_Storage); });
            go_Military.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => { OnClick_Building(go_Military); });
            go_Training.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => { OnClick_Building(go_Training); });
            go_Camp.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => { OnClick_Building(go_Camp); });
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        private void SetData()
        {
            if (null != Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info &&
                null != Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllBuildings)
            {
                list_AllBuildings.Clear();
                list_AllBuildings.AddRange(Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllBuildings);
            }
        }

        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            for (int i = 0; i < list_Building.Count; i++)
            {
                Transform tr = list_Building[i].transform;
                GuildDetailInfo.Types.Building building = i < list_AllBuildings.Count ? list_AllBuildings[i] : null;
                SetBuildingItem(tr, building, (Sys_Family.FamilyData.EBuildingIndex)i);
            }
        }
        /// <summary>
        /// 设置建筑模版
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="building"></param>
        /// <param name="index"></param>
        private void SetBuildingItem(Transform tr, GuildDetailInfo.Types.Building building, Sys_Family.FamilyData.EBuildingIndex index)
        {
            uint level = building == null ? 0 : building.Lvl;
            /// <summary> 建筑等级 </summary>
            Text text_Lv = tr.Find("Button/Image_Grade/Text_Grade").GetComponent<Text>();
            text_Lv.text = level.ToString();
            /// <summary> 建筑是否解锁 </summary>
            GameObject go_Lock = tr.Find("Button/Image_Lock").gameObject;
            GameObject go_LvNode = tr.Find("Button/Image_Grade").gameObject;

            go_Lock.SetActive(false);
            go_LvNode.SetActive(true);
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 点击建筑
        /// </summary>
        /// <param name="go"></param>
        private void OnClick_Building(GameObject go)
        {
            int index = 0;
            
            switch (go.name)
            {
                case "View_Hall":
                    index = (int)Sys_Family.FamilyData.EBuildingIndex.Castle;
                    break;
                case "View_Storage":
                    index = (int)Sys_Family.FamilyData.EBuildingIndex.Bank;
                    break;
                case "View_Military":
                    index = (int)Sys_Family.FamilyData.EBuildingIndex.SciencesAcademy;
                    break;
                case "View_Camp":
                    index = (int)Sys_Family.FamilyData.EBuildingIndex.Grange;
                    break;
                case "View_Research":
                    index = (int)Sys_Family.FamilyData.EBuildingIndex.MilitaryAcademy;
                    break;
                case "View_Training":
                    index = (int)Sys_Family.FamilyData.EBuildingIndex.TrainingAnimals;
                    break;
                case "View_People":
                    index = (int)Sys_Family.FamilyData.EBuildingIndex.Barracks;
                    break;
            }
            UIManager.HitButton(EUIID.UI_Family, "OnClick_Building:" + index.ToString());
            if (index == (int)Sys_Family.FamilyData.EBuildingIndex.TrainingAnimals ||
                index == (int)Sys_Family.FamilyData.EBuildingIndex.Barracks)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10624));
                return;
            }
            switch ((Sys_Family.FamilyData.EBuildingIndex)index)
            {
                case Sys_Family.FamilyData.EBuildingIndex.Castle:
                    {
                        UIManager.OpenUI(EUIID.UI_Family_BuildLvUpgrade, false, index);
                    }
                    break;
                case Sys_Family.FamilyData.EBuildingIndex.Bank:
                    {
                        UIManager.OpenUI(EUIID.UI_Family_Bank);
                    }
                    break;
                case Sys_Family.FamilyData.EBuildingIndex.SciencesAcademy:
                    {
                        UIManager.OpenUI(EUIID.UI_Family_BuildSkillUpgrade, false, index);
                    }
                    break;
                case Sys_Family.FamilyData.EBuildingIndex.Grange:
                    {
                        UIManager.OpenUI(EUIID.UI_Family_BuildSkillUpgrade, false, index);
                    }
                    break;
                case Sys_Family.FamilyData.EBuildingIndex.MilitaryAcademy:
                    {
                        UIManager.OpenUI(EUIID.UI_Family_BuildSkillUpgrade, false, index);
                    }
                    break;
            }
        }
        #endregion
        #region 提供功能
        #endregion
    }
}