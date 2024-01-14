//using Logic.Core;
//using Lib.Core;
//using System;
//using System.Collections.Generic;
//using Table;
//using UnityEngine;
//using UnityEngine.UI;

//namespace Logic
//{
//    public partial class UI_LottoRewardPreview_Layout
//    {
//        class RewardItem:ClickItem
//        {
//            private ClickItemGroup<RectRewardItem> m_RewardGroup = new ClickItemGroup<RectRewardItem>();

//            public override void Load(Transform root)
//            {
//                base.Load(root);

//                var rewardTrans = root.Find("Viewport01/Item");
//                m_RewardGroup.AddChild(rewardTrans);
//            }

//            public void SetReward(List<uint> rewardlist)
//            {
//                int count = rewardlist.Count;

//                m_RewardGroup.SetChildSize(count);

//                for (int i = 0; i < count; i++)
//                {
//                    var item = m_RewardGroup.getAt(i);

//                    if (item != null)
//                    {
//                        var data = CSVAward.Instance.GetConfData(rewardlist[i]);
//                        if(data != null)
//                        {
//                            item.SetReward(data.itemId, data.itemNum);
//                        }
//                        else
//                        {
//                            DebugUtil.LogError("Data is null i = " + i.ToString() + " id = " + rewardlist[i].ToString());
//                        }

//                    }
//                }
//            }
//        }
//    }
//    public partial class UI_LottoRewardPreview_Layout
//    {
//        private Text m_TexTips;
//        private Button m_BtnClose;
//        private CP_ToggleRegistry m_toggleRegistry;
//        private InfinityGrid[] m_InfinityGrids;

//        private ClickItemGroup<RewardItem> m_RewardGroup = new ClickItemGroup<RewardItem>();

//        // private ClickItemGroup<RectPrecentItem> m_PrecentGroup = new ClickItemGroup<RectPrecentItem>();

//        //private VerticalLayoutGroup m_RewardLayoutGroup;

//        //private ContentSizeFitter m_GridSizeFitter;
//        public void Load(Transform root)
//        {
//            m_BtnClose = root.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();

//            //m_RewardLayoutGroup = root.Find("Animator/Tips01/Scroll_View01/Grid").GetComponent<VerticalLayoutGroup>();
//            m_toggleRegistry = root.Find("Animator/Tips/Toggles").GetComponent<CP_ToggleRegistry>();
//            m_toggleRegistry.onToggleChange = OnToggleChanged;
            
//            int toggleCount = root.Find("Animator/Tips/Toggles").childCount;
//            for(int i = 0; i < toggleCount; ++i)
//            {
//                CP_Toggle toggle = root.Find("Animator/Tips/Toggles/Toggle" + i.ToString()).GetComponent<CP_Toggle>();
//                toggle.id = i;
//            }

//            if(m_InfinityGrids == null)
//            {
//                m_InfinityGrids = new InfinityGrid[5];
//                for(int i = 0; i < m_InfinityGrids.Length; ++i)
//                {
//                    m_InfinityGrids[i] = root.Find("Animator/Tips/Rectlist/Image_line" + i.ToString() + "/Scroll View").GetComponent<InfinityGrid>();
//                    m_InfinityGrids[i].onCreateCell = OnCreateCell;
//                    m_InfinityGrids[i].onCellChange = OnCellChange;
//                }
//            }


//            //m_TexTips = root.Find("Animator/Tips01/Image_Tips/Text").GetComponent<Text>();

//            //m_GridSizeFitter = m_RewardLayoutGroup.transform.GetComponent<ContentSizeFitter>();

//            m_toggleRegistry.SwitchTo(0);

//            Transform gird = root.Find("Animator/Tips01/Scroll_View01/Grid");

//            int count = gird.childCount;

//            for (int i = 0; i < count; i++)
//            {
//                var item = gird.Find("AwardItem" + i.ToString());
//                if (item != null)
//                    m_RewardGroup.AddChild(item);
//            }
//        }

//        public void SetListener(IListener listener)
//        {
//            m_BtnClose.onClick.AddListener(listener.OnClickClose);
//        }

//        public void SetRewardCount(int count)
//        {
//            m_RewardGroup.SetChildSize(count);
//        }
//        public void SetReward(int index, CSVAwardPreviewData rewardlist)
//        {
//            var item = m_RewardGroup.getAt(index);

//            if (item == null)
//                return;

//            item.SetReward(rewardlist.awardId);   
            
//        }

//        public void RefreshReward()
//        {
           
//        }

//        private void OnToggleChanged(int current, int old)
//        {
//            if (current == 0)
//            {

//            }
//            else
//            {

//            }
//        }

//        private void OnCreateCell(InfinityGridCell cell)
//        {
//            RectRewardItem item = new RectRewardItem();

//            item.Load(cell.mRootTransform);
//            cell.BindUserData(item);
//        }

//        private void OnCellChange(InfinityGridCell cell, int index)
//        {
//            RectRewardItem item = cell.mUserData as RectRewardItem;
//            item.SetReward(id, count);
//        }
//    }


//    public partial class UI_LottoRewardPreview_Layout
//    {
//        public interface IListener
//        {
//            void OnClickClose();
//        }
//    }

//    public partial class UI_LottoRewardPreview_Layout
//    {
//        private class RectRewardItem : IntClickItem
//        {
//            PropItem m_Item;
//            PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, true, false);
//            public override void Load(Transform root)
//            {
//                base.Load(root);

//                m_Item = new PropItem();

//                m_Item.BindGameObject(root.gameObject);
//            }

//            public override ClickItem Clone()
//            {
//                return Clone<RectRewardItem>(this);
//            }

//            public void SetReward(uint id, uint count)
//            {
//                m_ItemData.id = id;
//                m_ItemData.count = count;

//                m_Item.SetData(new MessageBoxEvt(EUIID.UI_Lotto_Preview, m_ItemData));

//                var data = CSVItem.Instance.GetConfData(id);
//                if (data != null)
//                    m_Item.txtName.text = LanguageHelper.GetTextContent(data.name_id);
//            }
//        }
//    }

//    //public partial class UI_LottoRewardPreview_Layout
//    //{
//    //    private class RectPrecentItem : IntClickItem
//    //    {

//    //        private Text m_TexName;
//    //        private Text m_TexPrecent;
//    //        public override void Load(Transform root)
//    //        {
//    //            base.Load(root);

//    //            m_TexName = root.Find("Text_Name").GetComponent<Text>();

//    //            m_TexPrecent = root.Find("Text_Num").GetComponent<Text>();
//    //        }
//    //        public override ClickItem Clone()
//    //        {
//    //            return Clone<RectPrecentItem>(this);
//    //        }

//    //        public void SetInfo(uint id, uint precent)
//    //        {
//    //            var data = CSVItem.Instance.GetConfData(id);
//    //            m_TexName.text = LanguageHelper.GetTextContent(data.name_id);

//    //            m_TexPrecent.text = ((precent / 100f)).ToString()+"%";
//    //        }
//    //    }


//    //}
//}
