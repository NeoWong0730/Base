using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    /// <summary> 百人道场布局设置 </summary>
    public partial class UI_HundredPeopleAreaRank_Layout
    {
        #region 界面组件
        /// <summary> 目录节点 </summary>
        public Transform transform;
        /// <summary> 我的排行节点 </summary>
        public RectTransform rt_MyRank;
        public GameObject rt_MyRankIcon1;
        public GameObject rt_MyRankIcon2;
        public GameObject rt_MyRankIcon3;
        /// <summary> 菜单模版 </summary>
        public Toggle toggle_MenuItem;
        /// <summary> 菜单列表 </summary>
        public List<Toggle> list_MenuItem = new List<Toggle>();
        /// <summary> 排名模版 </summary>
        public RectTransform rt_RankItem;
        /// <summary> 排名列表 </summary>
        public List<RectTransform> list_RankItem = new List<RectTransform>();
        #endregion
        #region 初始化
        public void Load(Transform root)
        {
            this.transform = root;

            rt_MyRank = transform.Find("Animator/View_TipsBg02_Big/MyRank").GetComponent<RectTransform>();
            rt_MyRankIcon1 = transform.Find("Animator/View_TipsBg02_Big/MyRank/Image_Icon").gameObject;
            rt_MyRankIcon2 = transform.Find("Animator/View_TipsBg02_Big/MyRank/Image_Icon1").gameObject;
            rt_MyRankIcon3 = transform.Find("Animator/View_TipsBg02_Big/MyRank/Image_Icon2").gameObject;
            toggle_MenuItem = transform.Find("Animator/View_TipsBg02_Big/ScrollView_Menu/List/Toggle").GetComponent<Toggle>();
            rt_RankItem = transform.Find("Animator/View_TipsBg02_Big/ScrollView_Rank/TabList/RankItem") as RectTransform;
            toggle_MenuItem.isOn = false;
            toggle_MenuItem.gameObject.SetActive(false);
            rt_RankItem.gameObject.SetActive(false);
            CreateCareerItemList();
            SetDefaultView();
        }
        public void SetListener(IListener listener)
        {
            transform.Find("Animator/View_TipsBg02_Big/View_TipsBgNew05/Btn_Close").GetComponent<Button>().onClick.AddListener(listener.OnClick_Close);

            for (int i = 0, count = list_MenuItem.Count; i < count; i++)
            {
                var item = list_MenuItem[i];
                item.onValueChanged.AddListener((bool value) => { if (value) { listener.OnClick_Menu(item); } });
            }
        }
        /// <summary>
        /// 默认界面，去除prefab带有的参照信息
        /// </summary>
        private void SetDefaultView()
        {
            SetMyRank(-1, string.Empty, 0);
        }
        /// <summary>
        /// 创建列表
        /// </summary>
        private void CreateCareerItemList()
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
        }
        /// <summary>
        /// 创建动态列表
        /// </summary>
        /// <param name="count"></param>
        public void CreateRankItemList(int count)
        {
            while (count > list_RankItem.Count)
            {
                GameObject go = GameObject.Instantiate(rt_RankItem.gameObject, rt_RankItem.transform.parent);
                list_RankItem.Add(go.transform as RectTransform);
            }
        }
        #endregion
        #region 响应事件
        public interface IListener
        {
            void OnClick_Close();

            void OnClick_Menu(Toggle toggle);
        }
        #endregion
        #region 功能接口
        /// <summary>
        /// 设置菜单模版
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="eCareerType"></param>
        public void SetMenuItem(int index, uint name, uint icon, uint select_icon)
        {
            if (index < 0 || index >= list_MenuItem.Count)
                return;

            Transform tr = list_MenuItem[index].transform;

            /// <summary> 职业名字 </summary>
            Text text_Career1 = tr.Find("Object/Text").GetComponent<Text>();
            /// <summary> 职业名字 </summary>
            Text text_Career2 = tr.Find("Object_Selected/Text").GetComponent<Text>();
            /// <summary> 未选中图标 </summary>
            Image image_Icon1 = tr.Find("Object/Image_Icon").GetComponent<Image>();
            /// <summary> 选中图标 </summary>
            Image image_Icon2 = tr.Find("Object_Selected/Image_Icon").GetComponent<Image>();

            text_Career1.text = text_Career2.text = LanguageHelper.GetTextContent(name);
            ImageHelper.SetIcon(image_Icon1, icon);
            ImageHelper.SetIcon(image_Icon2, select_icon);
        }
        /// <summary>
        /// 排行模板
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <param name="passedStage"></param>
        public void SetRankItem(int index, string name, uint passedStage)
        {
            if (index < 0 || index >= list_RankItem.Count)
                return;

            Transform tr = list_RankItem[index].transform;
            /// <summary> 玩家名字 </summary>
            Text text_Name = tr.Find("Text_Name").GetComponent<Text>();
            text_Name.text = name;
            /// <summary> 通关关数 </summary>
            Text text_Amount = tr.Find("Text_Amount").GetComponent<Text>();
            CSVInstanceDaily.Data csv = CSVInstanceDaily.Instance.GetConfData(passedStage);
            if (csv != null) {
                text_Amount.text = ((csv.LayerStage - 1) * 10 + csv.Layerlevel).ToString();
            }
            else {
                TextHelper.SetText(text_Amount, 1006190);
            }

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
        /// 设置我的排行
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <param name="passedStage"></param>
        public void SetMyRank(int index, string name, uint passedStage)
        {
            /// <summary> 玩家名字 </summary>
            Text text_Name = rt_MyRank.Find("Text_Name").GetComponent<Text>();
            text_Name.text = name;
            /// <summary> 通关关数 </summary>
            Text text_Amount = rt_MyRank.Find("Text_Amount").GetComponent<Text>();
            CSVInstanceDaily.Data csv = CSVInstanceDaily.Instance.GetConfData(passedStage);
            if (csv != null) {
                text_Amount.text = ((csv.LayerStage - 1) * 10 + csv.Layerlevel).ToString();
            }
            else {
                TextHelper.SetText(text_Amount, 1006190);
            }
            /// <summary> 排名4数字</summary>
            Text text_Rank = rt_MyRank.Find("Text_Rank").GetComponent<Text>();
            rt_MyRankIcon1.SetActive(index == 0);
            rt_MyRankIcon2.SetActive(index == 1);
            rt_MyRankIcon3.SetActive(index == 2);

            if (index > 2 || index <= -1) {
                text_Rank.gameObject.SetActive(true);
                text_Rank.text = index < 0 ? LanguageHelper.GetTextContent(1006035) : (index + 1).ToString();
            }
            else {
                text_Rank.gameObject.SetActive(false);
            }
        }
        #endregion
    }
}


