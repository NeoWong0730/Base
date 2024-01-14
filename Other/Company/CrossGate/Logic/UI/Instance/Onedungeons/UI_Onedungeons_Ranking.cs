using Lib.Core;
using Logic.Core;
using Packet;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    /// <summary> 单人副本排行 </summary>
    public class UI_Onedungeons_Ranking : UIBase
    {
        #region 界面组件
        /// <summary> 我的排行节点 </summary>
        private RectTransform rt_MyRank;
        /// <summary> 菜单模版 </summary>
        private Toggle toggle_MenuItem;
        /// <summary> 菜单列表 </summary>
        private List<Toggle> list_MenuItem = new List<Toggle>();
        /// <summary> 排名模版 </summary>
        private RectTransform rt_RankItem;
        /// <summary> 排名列表 </summary>
        private List<RectTransform> list_RankItem = new List<RectTransform>();
        #endregion
        #region 数据
        /// <summary> 副本编号 </summary>
        public uint instanceid { get; private set; } = 0;
        /// <summary> 菜单下标 </summary>
        public int curIndex { get; private set; } = 0;
        #endregion
        #region 系统函数        
        protected override void OnLoaded()
        {            
            OnParseComponent();
            ClearItemList();
            CreateItemList();
        }        
        protected override void OnOpen(object arg)
        {            
            instanceid = System.Convert.ToUInt32(arg);
        }
        protected override void OnShow()
        {            
            ResetOption();
            RefreshView();
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
            rt_MyRank = transform.Find("Animator/View_TipsBg02_Big/MyRank") as RectTransform;
            toggle_MenuItem = transform.Find("Animator/View_TipsBg02_Big/ScrollView_Menu/List/Toggle").GetComponent<Toggle>();
            rt_RankItem = transform.Find("Animator/View_TipsBg02_Big/ScrollView_Rank/TabList/RankItem") as RectTransform;
            toggle_MenuItem.isOn = false;
            toggle_MenuItem.gameObject.SetActive(false);
            rt_RankItem.gameObject.SetActive(false);

            transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
        }
        /// <summary>
        /// 事件注册
        /// </summary>
        /// <param name="toRegister"></param>
        protected void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Instance.Instance.eventEmitter.Handle(Sys_Instance.EEvents.DailyInstanceRankInfoRes, OnDailyInstanceRankInfoRes, toRegister);
        }
        /// <summary>
        /// 创建列表
        /// </summary>
        private void CreateItemList()
        {
            var values = System.Enum.GetValues(typeof(ECareerType));

            for (int i = 0, count = values.Length; i < count; i++)
            {
                ECareerType type = (ECareerType)values.GetValue(i);
                if (type == ECareerType.None) continue;
                GameObject go = GameObject.Instantiate(toggle_MenuItem.gameObject, toggle_MenuItem.transform.parent);
                go.name = ((int)type).ToString();
                go.SetActive(true);
                list_MenuItem.Add(go.GetComponent<Toggle>());
            }
            for (int i = 0, count = list_MenuItem.Count; i < count; i++)
            {
                var item = list_MenuItem[i];
                item.onValueChanged.AddListener((bool value) => { if (value) { OnClick_Menu(item); } });
            }
        }
        /// <summary>
        /// 创建动态列表
        /// </summary>
        /// <param name="count"></param>
        private void SetItemList(int count)
        {
            while (count > list_RankItem.Count)
            {
                GameObject go = GameObject.Instantiate(rt_RankItem.gameObject, rt_RankItem.transform.parent);
                list_RankItem.Add(go.transform as RectTransform);
            }
        }
        /// <summary>
        /// 清理列表
        /// </summary>
        private void ClearItemList()
        {
            for (int i = 0, count = list_MenuItem.Count; i < count; i++)
            {
                var item = list_MenuItem[i];
                if (item != null && null != item.transform)
                    GameObject.Destroy(item.transform.gameObject);
            }
            list_MenuItem.Clear();

            for (int i = 0, count = list_RankItem.Count; i < count; i++)
            {
                var item = list_RankItem[i];
                if (item != null && null != item.transform)
                    GameObject.Destroy(item.transform.gameObject);
            }
            list_RankItem.Clear();
        }
        /// <summary>
        /// 更新选项
        /// </summary>
        private void UpdateOption()
        {
            Toggle toggle = curIndex >= 0 && curIndex < list_MenuItem.Count ? list_MenuItem[curIndex] : null;
            if (null == toggle)
                return;

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
        #region 数据设置
        /// <summary>
        /// 重置选项
        /// </summary>
        private void ResetOption()
        {
            curIndex = 0;
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            SetMenuView();
            SetRankView(null);
            SetMyRankView(null);
            UpdateOption();
        }
        /// <summary>
        /// 设置菜单界面
        /// </summary>
        private void SetMenuView()
        {
            ECareerType[] values = (ECareerType[])System.Enum.GetValues(typeof(ECareerType));
            List<ECareerType> list = new List<ECareerType>(values);
            list.Remove(ECareerType.None);

            for (int i = 0, count = list_MenuItem.Count; i < count; i++)
            {
                SetMenuItem(list_MenuItem[i].transform, list[i]);
            }
        }
        /// <summary>
        /// 设置菜单模版
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="eCareerType"></param>
        private void SetMenuItem(Transform tr, ECareerType eCareerType)
        {
            CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData((uint)eCareerType);
            if (null == cSVCareerData)
            {
                tr.gameObject.SetActive(false);
                return;
            }
            else
            {
                tr.gameObject.SetActive(true);
            }
            /// <summary> 职业名字 </summary>
            Text text_Career1 = tr.Find("Object/Text").GetComponent<Text>();
            /// <summary> 职业名字 </summary>
            Text text_Career2 = tr.Find("Object_Selected/Text").GetComponent<Text>();
            /// <summary> 未选中图标 </summary>
            Image image_Icon1 = tr.Find("Object/Image_Icon").GetComponent<Image>();
            /// <summary> 选中图标 </summary>
            Image image_Icon2 = tr.Find("Object_Selected/Image_Icon").GetComponent<Image>();

            text_Career1.text = text_Career2.text = LanguageHelper.GetTextContent(cSVCareerData.name);
            ImageHelper.SetIcon(image_Icon1, cSVCareerData.icon);
            ImageHelper.SetIcon(image_Icon2, cSVCareerData.select_icon);
        }
        /// <summary>
        /// 设置排行界面
        /// </summary>
        private void SetRankView(Google.Protobuf.Collections.RepeatedField<Packet.DailyInsPassRankInfo> dailyInsLayerRankInfos)
        {
            SetItemList(dailyInsLayerRankInfos == null ? 0 : dailyInsLayerRankInfos.Count);

            for (int i = 0, count = list_RankItem.Count; i < count; i++)
            {
                SetRankItem(list_RankItem[i].transform, i, i < dailyInsLayerRankInfos?.Count ? dailyInsLayerRankInfos[i] : null);
            }
        }
        /// <summary>
        /// 设置排行模版
        /// </summary>
        /// <param name="tr"></param>
        private void SetRankItem(Transform tr, int index, DailyInsPassRankInfo dailyInsPassRankInfo)
        {
            if (null == dailyInsPassRankInfo)
            {
                tr.gameObject.SetActive(false);
                return;
            }
            else
            {
                tr.gameObject.SetActive(true);
            }
            /// <summary> 背景 </summary>
            GameObject go_Bg = tr.Find("Image_Slected").gameObject;
            go_Bg.SetActive(index % 2 == 0);
            /// <summary> 玩家名字 </summary>
            Text text_Name = tr.Find("Text_Name").GetComponent<Text>();
            text_Name.text = dailyInsPassRankInfo.RoleName.ToStringUtf8();
            /// <summary> 通关关数 </summary>
            Text text_Amount = tr.Find("Text_Amount").GetComponent<Text>();
            CSVInstanceDaily.Data cSVInstanceDailyData = CSVInstanceDaily.Instance.GetConfData(dailyInsPassRankInfo.PassedStage);
            text_Amount.text = cSVInstanceDailyData == null ? string.Empty : string.Format("{0}-{1}", cSVInstanceDailyData.LayerStage, cSVInstanceDailyData.Layerlevel);
            /// <summary> 所用回合数  </summary>
            Text text_Round = tr.Find("Text_Time").GetComponent<Text>();
            text_Round.text = LanguageHelper.GetTextContent(1006000, dailyInsPassRankInfo.Round.ToString());
            /// <summary> 排名1图标 </summary>
            GameObject go_Rank1 = tr.Find("Rank/Image_Icon").gameObject;
            go_Rank1.SetActive(index == 0);
            /// <summary> 排名2图标 </summary>
            GameObject go_Rank2 = tr.Find("Rank/Image_Icon1").gameObject;
            go_Rank2.SetActive(index == 1);
            /// <summary> 排名3图标 </summary>
            GameObject go_Rank3 = tr.Find("Rank/Image_Icon2").gameObject;
            go_Rank3.SetActive(index == 2);
            /// <summary> 排名4数字</summary>
            Text text_Rank = tr.Find("Rank/Text_Rank").GetComponent<Text>();
            text_Rank.gameObject.SetActive(index >= 3);
            text_Rank.text = (index + 1).ToString();
        }
        /// <summary>
        /// 设置我的排行界面
        /// </summary>
        private void SetMyRankView(Packet.DailyInsPassRankInfo dailyInsPassRankInfo, int rankIndex = -1)
        {
            Transform tr = rt_MyRank;
            /// <summary> 排名数字</summary>
            Text text_Rank = tr.Find("Text_Rank").GetComponent<Text>();
            text_Rank.text = rankIndex < 0 ? LanguageHelper.GetTextContent(1006035) : (rankIndex + 1).ToString();
            /// <summary> 玩家名字 </summary>
            Text text_Name = tr.Find("Text_Name").GetComponent<Text>();
            text_Name.text = dailyInsPassRankInfo == null ? string.Empty : dailyInsPassRankInfo.RoleName.ToStringUtf8();
            /// <summary> 通关关数 </summary>
            Text text_Amount = tr.Find("Text_Amount").GetComponent<Text>();
            CSVInstanceDaily.Data cSVInstanceDailyData = CSVInstanceDaily.Instance.GetConfData(dailyInsPassRankInfo?.PassedStage ?? 0);
            text_Amount.text = cSVInstanceDailyData == null ? string.Empty : string.Format("{0}-{1}", cSVInstanceDailyData.LayerStage, cSVInstanceDailyData.Layerlevel);
            /// <summary> 所用回合数 </summary>
            Text text_Round = tr.Find("Text_Time").GetComponent<Text>();
            text_Round.text = dailyInsPassRankInfo == null ? string.Empty : LanguageHelper.GetTextContent(1006000, dailyInsPassRankInfo.Round.ToString());
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭界面
        /// </summary>
        public void OnClick_Close()
        {
            CloseSelf();
        }
        /// <summary>
        /// 点击菜单
        /// </summary>
        /// <param name="toggle"></param>
        public void OnClick_Menu(Toggle toggle)
        {
            uint careerId;
            if (!uint.TryParse(toggle.name, out careerId))
                return;

            Sys_Instance.Instance.DailyInstanceRankInfoReq(instanceid, careerId);
        }
        /// <summary>
        /// 排行数据刷新
        /// </summary>
        public void OnDailyInstanceRankInfoRes()
        {
            CmdDailyInstanceRankInfoRes res = Sys_Instance.Instance.cmdDailyInstanceRankInfoRes;
            int rankIndex = -1;
            for (int i = 0, count = res.RankInfos.Count; i < count; i++)
            {
                if (res.RankInfos[i].RoleId == res.SelfRankInfo.RoleId)
                {
                    rankIndex = i;
                    break;
                }
            }
            SetRankView(res.RankInfos);
            SetMyRankView(res.SelfRankInfo, rankIndex);
        }
        #endregion
        #region 提供功能
        #endregion
    }
}
