using Logic.Core;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Table;

namespace Logic
{
    /// <summary> 探索奖励 </summary>
    public class UI_ExploreReward : UIBase
    {
        #region 界面组件
        /// <summary> 标题 </summary>
        private Text text_Title;
        /// <summary> 内容 </summary>
        private Text text_Content;
        /// <summary> 奖励 </summary>
        private List<PropItem> list_RewardItem = new List<PropItem>();
        #endregion
        #region 变量
        /// <summary> 奖励数据 </summary>
        private CSVMapExplorationReward.Data cSVMapExplorationRewardData
        {
            get;
            set;
        }
        #endregion
        #region 系统函数        
        protected override void OnLoaded()
        {            
            OnParseComponent();
        }        
        protected override void OnOpen(object arg)
        {            
            uint id = arg == null ? 0 : System.Convert.ToUInt32(arg);
            cSVMapExplorationRewardData = CSVMapExplorationReward.Instance.GetConfData(id);
        }
        protected override void OnShow()
        {
            RefreshView();
        }                
        #endregion
        #region 初始化
        /// <summary>
        /// 检测组件 
        /// </summary>
        private void OnParseComponent()
        {
            text_Title = transform.Find("Animator/View_Content/Text_Title").GetComponent<Text>();
            text_Content = transform.Find("Animator/View_Content/Text_Tips").GetComponent<Text>();

            Transform tr_Node = transform.Find("Animator/Scroll_View_Item/TabList");
            for (int i = 0, count = tr_Node.childCount; i < count; i++)
            {
                PropItem propItem = new PropItem();
                propItem.BindGameObject(tr_Node.GetChild(i).gameObject);
                list_RewardItem.Add(propItem);
            }

            transform.Find("Animator/View_TipsBg01_Small/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        public void RefreshView()
        {
            if (null == cSVMapExplorationRewardData) return;
           
            text_Content.text = LanguageHelper.GetTextContent(cSVMapExplorationRewardData.lan, LanguageHelper.GetTextContent(cSVMapExplorationRewardData.title_lan));

            //设置奖励
            List<ItemIdCount> list_drop = CSVDrop.Instance.GetDropItem(cSVMapExplorationRewardData.DropId);

            uint x = 0;
            long y = 0;
            for (int i = 0, count = list_RewardItem.Count; i < count; i++)
            {
                if (i < list_drop.Count)
                {
                    x = list_drop[i].id;
                    y = list_drop[i].count;
                }
                else
                {
                    x = 0; y = 0;
                }
                SetRewardItem(list_RewardItem[i], x, y);
            }
        }
        /// <summary>
        /// 设置奖励模版
        /// </summary>
        /// <param name="propItem"></param>
        /// <param name="id"></param>
        /// <param name="Num"></param>
        public void SetRewardItem(PropItem propItem, uint id, long Num)
        {
            CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(id);
            if (null == cSVItemData)
            {
                propItem.SetActive(false);
                return;
            }
            propItem.SetActive(true);
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(id, Num, true, false, false, false, false, true);
            propItem.SetData(new MessageBoxEvt( EUIID.UI_ExploreReward, itemData));

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(propItem.transform.Find("Btn_Item/Image_BG").gameObject);
            eventListener.triggers.Clear();
            eventListener.AddEventListener(UnityEngine.EventSystems.EventTriggerType.PointerClick, ret => 
            { UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt( EUIID.UI_ExploreReward, itemData));});
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
        #endregion
        #region 提供功能
        #endregion
    }
}
