using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using Lib.Core;
using Packet;

namespace Logic
{
    /// <summary> 经典Boss战结果 </summary>
    public class UI_ClassicBossWarResult : UIBase
    {
        #region 界面组件
        /// <summary> 成功节点 </summary>
        private GameObject go_SuccessNode;
        /// <summary> 奖励节点 </summary>
        private GameObject go_RewardNode;
        /// <summary> 奖励模版 </summary>
        private GameObject go_RewardItem;
        /// <summary> 未解锁功能 </summary>
        private GameObject go_LockFunction;
        /// <summary> 无次数 </summary>
        private GameObject go_NotCount;
        /// <summary> 奖励次数 </summary>
        private Text text_SuccessRewardCount;
        /// <summary> 失败节点 </summary>
        private GameObject go_FailNode;
        /// <summary> 奖励次数 </summary>
        private Text text_FailRewardCount;
        #endregion
        #region 数据定义
        /// <summary> 经典Boss战数据结果 </summary>
        private CmdClassicBossResultNtf cmdClassicBossResultNtf;
        #endregion
        #region 系统函数
        protected override void OnInit()
        {
        }
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnDestroy()
        {

        }
        protected override void OnOpen(object arg)
        {
            cmdClassicBossResultNtf = null == arg ? new CmdClassicBossResultNtf() : (CmdClassicBossResultNtf)arg;
        }
        protected override void OnOpened()
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
            go_SuccessNode = transform.Find("Animator/Image_Successbg").gameObject;
            go_RewardNode = transform.Find("Animator/Image_Successbg/View_Victory/RewardNode").gameObject;
            go_RewardItem = transform.Find("Animator/Image_Successbg/View_Victory/RewardNode/Scroll_View/Viewport/Item").gameObject;
            go_LockFunction = transform.Find("Animator/Image_Successbg/View_Victory/Lock_bg/Text").gameObject;
            go_NotCount = transform.Find("Animator/Image_Successbg/View_Victory/Lock_bg/Text1").gameObject;
            text_SuccessRewardCount = transform.Find("Animator/Image_Successbg/View_Victory/Text_Residue/Value").GetComponent<Text>();
            go_FailNode = transform.Find("Animator/Image_Failedbg").gameObject;
            text_FailRewardCount = transform.Find("Animator/Image_Failedbg/View_Fail/Text_Residue/Value").GetComponent<Text>();
            transform.Find("Image_Black").GetComponent<Button>().onClick.AddListener(OnClick_Close);
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {

        }
        /// <summary>
        /// 创建足够的列表
        /// </summary>
        /// <param name="node"></param>
        /// <param name="child"></param>
        /// <param name="number"></param>
        private void CreateItemList(Transform node, Transform child, int number)
        {
            while (node.childCount < number)
            {
                GameObject.Instantiate(child, node);
            }
        }
        #endregion
        #region 数据处理
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            uint UsedTimes = (uint)Sys_ClassicBossWar.Instance.GetDailyTimes();
            uint MaxTimes = (uint)Sys_ClassicBossWar.Instance.GetDailyTotalTimes();
            uint CurTimes = MaxTimes - UsedTimes;

            switch ((CmdClassicBossResultNtf.Types.ResultType)cmdClassicBossResultNtf.Result)
            {
                case CmdClassicBossResultNtf.Types.ResultType.Fail:
                    {
                        go_SuccessNode.SetActive(false);
                        go_FailNode.SetActive(true);
                        text_FailRewardCount.text = CurTimes.ToString();
                    }
                    break;
                case CmdClassicBossResultNtf.Types.ResultType.SuccUnlock:
                    {
                        go_SuccessNode.SetActive(true);
                        go_FailNode.SetActive(false);
                        go_RewardNode.SetActive(false);
                        go_LockFunction.SetActive(true);
                        go_NotCount.gameObject.SetActive(false);
                        text_SuccessRewardCount.text = CurTimes.ToString();
                    }
                    break;
                case CmdClassicBossResultNtf.Types.ResultType.SuccReward:
                    {
                        go_SuccessNode.SetActive(true);
                        go_FailNode.SetActive(false);
                        go_RewardNode.SetActive(true);
                        go_LockFunction.SetActive(false);
                        go_NotCount.gameObject.SetActive(false);
                        text_SuccessRewardCount.text = CurTimes.ToString();
                        Transform tr_ItemParent = go_RewardItem.transform.parent;
                        CreateItemList(tr_ItemParent, go_RewardItem.transform, cmdClassicBossResultNtf.Rewards.Count);

                        List<PropItem> list_RewardItem = new List<PropItem>();
                        //uint x = 0; long y = 0;
                        for (int i = 0; i < tr_ItemParent.childCount; i++)
                        {
                            PropItem propItem = new PropItem();
                            propItem.BindGameObject(tr_ItemParent.GetChild(i).gameObject);
                            list_RewardItem.Add(propItem);
                            SetRewardItem(propItem, cmdClassicBossResultNtf.Rewards[i]);
                        }
                        //for (int i = 0; i < list_RewardItem.Count; i++)
                        //{
                        //    if (i < cmdClassicBossResultNtf.Rewards.Count)
                        //    {
                        //        x = cmdClassicBossResultNtf.Rewards[i].ItemId;
                        //        y = cmdClassicBossResultNtf.Rewards[i].Count;
                        //    }
                        //    else
                        //    {
                        //        x = 0; y = 0;
                        //    }
                        //    SetRewardItem(list_RewardItem[i], x, y);
                        //}
                        LayoutRebuilder.ForceRebuildLayoutImmediate(tr_ItemParent as RectTransform);
                    }
                    break;
                case CmdClassicBossResultNtf.Types.ResultType.SuccNoReward:
                    {
                        go_SuccessNode.SetActive(true);
                        go_FailNode.SetActive(false);
                        go_RewardNode.SetActive(false);
                        go_LockFunction.SetActive(false);
                        go_NotCount.gameObject.SetActive(true);
                        text_SuccessRewardCount.text = CurTimes.ToString();

                    }
                    break;
            }
        }
        /// <summary>
        /// 设置奖励模版
        /// </summary>
        /// <param name="propItem"></param>
        /// <param name="id"></param>
        /// <param name="Num"></param>
        public void SetRewardItem(PropItem propItem, CmdClassicBossResultNtf.Types.Reward reward)
        {
            Item item = null;
            if (reward.Item != null)
            {
                item = reward.Item;
            }
            else
            {
                item = new Item();
                item.Id = reward.ItemId;
                item.Count = (uint)reward.Count;
            }
            CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(item.Id);
            if (null == cSVItemData)
            {
                propItem.SetActive(false);
                return;
            }
            propItem.SetActive(true);
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(item.Id, item.Count, true, false, false, false, false, true, false, true, OnClickItem, false, false);
            //装备道具,单独处理
            if (cSVItemData.type_id == (uint)EItemType.Equipment)
                itemData.SetQuality(item.Equipment.Color);
            else if (cSVItemData.type_id == (uint)EItemType.Ornament)
                itemData.SetQuality(item.Ornament.Color);
            
            itemData.bagData = new ItemData(0, item.Uuid, item.Id, item.Count, 0, false, false, item.Equipment, item.Essence, 0, null, item.Crystal, item.Ornament);
            propItem.SetData(new MessageBoxEvt(EUIID.UI_ClassicBossWarResult, itemData));
        }

        private void OnClickItem(PropItem item)
        {
            ItemData mItemData = new ItemData(0, 0, item.ItemData.id, (uint)item.ItemData.count, 0, false, false, null, null, 0);
            //ItemData.EquipParam = item.ItemData.EquipPara;
            uint typeId = mItemData.cSVItemData.type_id;


            //装备道具,单独处理
            if (typeId == (uint)EItemType.Equipment)
            {
                EquipTipsData tipData = new EquipTipsData();
                tipData.equip = item.ItemData.bagData;
                tipData.isCompare = false;
                tipData.isShowOpBtn = false;

                UIManager.OpenUI(EUIID.UI_TipsEquipment, false, tipData);
            }
            else if (typeId == (uint)EItemType.Crystal)
            {
                CrystalTipsData crystalTipsData = new CrystalTipsData();
                crystalTipsData.itemData = item.ItemData.bagData;
                //crystalTipsData.bShowOp = true;
                //crystalTipsData.bShowCompare = true;
                //crystalTipsData.bShowDrop = true;
                //crystalTipsData.bShowSale = true;
                UIManager.OpenUI(EUIID.UI_Tips_ElementalCrystal, false, crystalTipsData);
            }
            else if (typeId == (uint)EItemType.Ornament)
            {
                OrnamentTipsData tipData = new OrnamentTipsData();
                tipData.equip = item.ItemData.bagData;
                tipData.sourceUiId = EUIID.UI_ClassicBossWarResult;
                UIManager.OpenUI(EUIID.UI_Tips_Ornament, false, tipData);
            }
            else
            {
                PropMessageParam propParam = new PropMessageParam();
                propParam.itemData = item.ItemData.bagData;
                propParam.showBtnCheck = false;
                propParam.sourceUiId = EUIID.UI_ClassicBossWarResult;
                UIManager.OpenUI(EUIID.UI_Prop_Message, false, propParam);
            }    
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
        #region 提供功能
        #endregion
    }
}