using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    /// <summary> 地图奖励界面 </summary>
    public class UI_MapAward : UIBase
    {
        #region 界面组件
        /// <summary> 背景节点 </summary>
        private RectTransform rt_Bg;
        /// <summary> 资源标题模版 </summary>
        private Transform tr_ExplorationTitleItem;
        /// <summary> 资源模版 </summary>
        private Transform tr_ExplorationItem;
        /// <summary> 资源标题模版列表 </summary>
        private List<Transform> lis_ExplorationTitleItem = new List<Transform>();
        /// <summary> 资源模版列表 </summary>
        private List<Transform> lis_ExplorationItem = new List<Transform>();
        #endregion
        #region 数据定义
        /// <summary> 探索数据 </summary>
        private Sys_Exploration.ExplorationData explorationData { get; set; }
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
        }
        protected override void OnOpen(object arg)
        {
            uint Id = System.Convert.ToUInt32(arg);
            explorationData = Sys_Exploration.Instance.GetExplorationData(Id);
        }
        protected override void OnShow()
        {
            CreateItemList();
            SetExplorationView();
        }
        protected override void OnHide()
        {
            ClearItemList();
        }
        protected override void OnClose()
        {
            explorationData = null;
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
            rt_Bg = transform.Find("Animator/Image_Bg").GetComponent<RectTransform>();
            tr_ExplorationTitleItem = transform.Find("Animator/Image_Bg/Scroll_View01/Image_Title");
            tr_ExplorationItem = transform.Find("Animator/Image_Bg/Scroll_View01/Viewport");
            tr_ExplorationTitleItem.gameObject.SetActive(false);
            tr_ExplorationItem.gameObject.SetActive(false);
            transform.Find("Black").GetComponent<Button>().onClick.AddListener(OnClick_Close);
        }
        /// <summary>
        /// 创建模版列表
        /// </summary>
        private void CreateItemList()
        {
            for (int i = 0; i < explorationData.list_ExplorationRewardID.Count; i++)
            {
                Transform tr1 = GameObject.Instantiate(tr_ExplorationTitleItem, tr_ExplorationTitleItem.parent);
                lis_ExplorationTitleItem.Add(tr1);
                Transform tr2 = GameObject.Instantiate(tr_ExplorationItem, tr_ExplorationItem.parent);
                lis_ExplorationItem.Add(tr2);
            }
        }
        /// <summary>
        /// 清理模版列
        /// </summary>
        private void ClearItemList()
        {
            for (int i = 0, count = lis_ExplorationTitleItem.Count; i < count; i++)
            {
                var x = lis_ExplorationTitleItem[i];
                if (x != null && null != x.transform) GameObject.Destroy(x.transform.gameObject);
            }
            lis_ExplorationTitleItem.Clear();
            for (int i = 0, count = lis_ExplorationItem.Count; i < count; i++)
            {
                var x = lis_ExplorationItem[i];
                if (x != null && null != x.transform) GameObject.Destroy(x.transform.gameObject);
            }
            lis_ExplorationItem.Clear();
        }
        /// <summary>
        /// 创建足够的奖励列表
        /// </summary>
        /// <param name="node"></param>
        /// <param name="child"></param>
        /// <param name="number"></param>
        private void CreateRewardItemList(Transform node, Transform child, int number)
        {
            while (node.childCount < number)
            {
                GameObject.Instantiate(child,node);
            }
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 设置资源点信息
        /// </summary>
        public void SetExplorationView()
        {
            for (int i = 0; i < lis_ExplorationTitleItem.Count; i++)
            {
                uint Id = explorationData.list_ExplorationRewardID[i];
                bool isReceived = Sys_Npc.Instance.IsGetRewards(Id);
                SetExplorationTitleItem(lis_ExplorationTitleItem[i], Id, isReceived);
                SetExplorationItem(lis_ExplorationItem[i], Id, isReceived);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt_Bg);
        }
        /// <summary>
        /// 设置探索标题模版
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="Id"></param>
        /// <param name="isReceived"></param>
        public void SetExplorationTitleItem(Transform tr, uint Id, bool isReceived)
        {
            CSVMapExplorationReward.Data cSVMapExplorationRewardData = CSVMapExplorationReward.Instance.GetConfData(Id);
            if (null == cSVMapExplorationRewardData)
            {
                tr.gameObject.SetActive(false);
                return;
            }
            tr.gameObject.SetActive(true);
            //进度
            Text text_Process = tr.Find("Text_Num").GetComponent<Text>();
            text_Process.text = string.Format("{0}%", System.Math.Round((float)cSVMapExplorationRewardData.ExplorationDegree / (float)10000 * 100, 1));
            ImageHelper.SetImageGray(text_Process, isReceived, true);
        }
        /// <summary>
        /// 设置探索模版
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="Id"></param>
        /// <param name="isReceived"></param>
        public void SetExplorationItem(Transform tr, uint Id, bool isReceived)
        {
            CSVMapExplorationReward.Data cSVMapExplorationRewardData = CSVMapExplorationReward.Instance.GetConfData(Id);
            if (null == cSVMapExplorationRewardData)
            {
                tr.gameObject.SetActive(false);
                return;
            }
            tr.gameObject.SetActive(true);

            //奖励道具
            List<ItemIdCount> list_drop = CSVDrop.Instance.GetDropItem(cSVMapExplorationRewardData.DropId);
            uint x = 0;
            long y = 0;

            Transform tr_ItemNode = tr.Find("Item01/Scroll_View/Viewport");
            Transform tr_Item = tr.Find("Item01/Scroll_View/Viewport/Item");
            CreateRewardItemList(tr_ItemNode, tr_Item, list_drop.Count);
            List<PropItem> list_RewardItem = new List<PropItem>();
            for (int i = 0; i < tr_ItemNode.childCount; i++)
            {
                PropItem propItem = new PropItem();
                propItem.BindGameObject(tr_ItemNode.GetChild(i).gameObject);
                list_RewardItem.Add(propItem);
            }

            for (int i = 0; i < list_RewardItem.Count; i++)
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
                SetRewardItem(list_RewardItem[i], x, y, isReceived);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(tr as RectTransform);
        }
        /// <summary>
        /// 设置奖励模版
        /// </summary>
        /// <param name="propItem"></param>
        /// <param name="id"></param>
        /// <param name="Num"></param>
        /// <param name="isReceived"></param>
        public void SetRewardItem(PropItem propItem, uint id, long Num, bool isReceived)
        {
            CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(id);
            if (null == cSVItemData)
            {
                propItem.SetActive(false);
                return;
            }
            propItem.SetActive(true);
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(id, Num, true, false, false, false, false, true);
            propItem.SetData(new MessageBoxEvt(EUIID.UI_Map, itemData));
            propItem.SetGot(isReceived);
            ImageHelper.SetImageGray(propItem.Layout.transform, isReceived, true);

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(propItem.transform.Find("Btn_Item/Image_BG").gameObject);
            eventListener.triggers.Clear();
            eventListener.AddEventListener(UnityEngine.EventSystems.EventTriggerType.PointerClick, ret =>
            { UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_Map, itemData)); });
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            CloseSelf();
        }
        #endregion
    }
}

