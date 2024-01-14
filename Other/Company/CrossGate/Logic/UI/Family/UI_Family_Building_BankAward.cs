using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using Lib.Core;
using Packet;
using System.Text;

namespace Logic
{
    /// <summary> 家族银行奖励 </summary>
    public class UI_Family_Building_BankAward : UIBase
    {
        #region 界面组件
        /// <summary> 掉落ID </summary>
        private RectTransform rt_Bg;
        /// <summary> 模版 </summary>
        private Transform tr_PropItem;
        #endregion
        #region 数据定义
        /// <summary> 掉落ID </summary>
        private uint DropId { get; set; } = 0;
        #endregion
        #region 系统函数
        protected override void OnInit()
        {
        }
        protected override void OnLoaded()
        {
            OnParseComponent();
            CreatItemList();
        }
        protected override void OnUpdate()
        {
        }
        protected override void OnOpen(object arg)
        {
            uint Id = System.Convert.ToUInt32(arg);
            DropId = Id;
        }
        protected override void OnShow()
        {
            SetRewardView();
        }
        protected override void OnHide()
        {
        }
        protected override void OnClose()
        {

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
            rt_Bg = transform.Find("View_Award").GetComponent<RectTransform>();
            tr_PropItem = transform.Find("View_Award/Viewport/PropItem");
            transform.Find("Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
        }
        /// <summary>
        /// 创建模版列表
        /// </summary>
        private void CreatItemList()
        {
            for (int i = 0; i < 5; i++)
            {
                GameObject.Instantiate(tr_PropItem, tr_PropItem.parent);
            }
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 设置奖励界面
        /// </summary>
        public void SetRewardView()
        {
            SetRewardList(transform, DropId);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt_Bg);
        }
        /// <summary>
        /// 设置奖励列表
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="Id"></param>
        public void SetRewardList(Transform tr, uint Id)
        {
            //奖励道具
            List<ItemIdCount> list_drop = CSVDrop.Instance.GetDropItem(Id);
            uint x = 0;
            long y = 0;

            Transform tr_ItemNode = tr.Find("View_Award/Viewport");
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
                SetRewardItem(list_RewardItem[i], x, y);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(tr as RectTransform);
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
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(id, Num, true, false, false, false, false, true, false, true, null, false, true);
            propItem.SetData(new MessageBoxEvt(EUIID.UI_Map, itemData));
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            UIManager.HitButton(EUIID.UI_Family_Bank, "OnClick_Close");
            CloseSelf();
        }
        #endregion
    }
}