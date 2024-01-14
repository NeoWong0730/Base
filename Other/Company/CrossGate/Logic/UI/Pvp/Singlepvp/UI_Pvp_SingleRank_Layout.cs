using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_Pvp_SingleRank_Layout
    {
        ClickItemGroup<RankItem> m_ItemGroup = new ClickItemGroup<RankItem>() ;

        OwnRankItem m_OwnRankItem = new OwnRankItem();


        Button m_BtnClose;

        IListener m_Listener;

        Transform m_TransServerSelect;
        DropdownEx m_DDDServer;
        Transform m_DropArrow;

        ClickItemGroup<RankDropdownItem> m_DrapdownItemGroup;
 
        public void Load(Transform root)
        {
            Transform rankListTrans = root.Find("Animator/ScrollView_Rank/TabList/");

            int rankcount = rankListTrans.childCount;

            for (int i = 0; i < rankcount; i++)
            {
                string itemName = i == 0 ? "RankItem" : string.Format("RankItem ({0})", i);

                var item = rankListTrans.Find(itemName);

                if (item != null)
                {
                    RankItem itemObj = new RankItem();

                    itemObj.Load(item);

                    if (i == 3)
                        m_ItemGroup.OriginItem = itemObj;

                    m_ItemGroup.AddChild(itemObj);
                }
            }

            m_BtnClose = root.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();

            m_OwnRankItem.Load(root.Find("Animator/MyRank"));

          

            m_TransServerSelect = root.Find("Animator/ServerDropDown");
            m_DDDServer = m_TransServerSelect.GetComponent<DropdownEx>();
            m_DrapdownItemGroup = new ClickItemGroup<RankDropdownItem>(m_TransServerSelect.Find("Template/Viewport/Content/SelectNow01"));

            m_DropArrow = m_TransServerSelect.Find("Arrow");
        }

        public void SetListener(IListener listener)
        {
            m_Listener = listener;

            m_BtnClose.onClick.AddListener(listener.OnClickClose);

            m_ItemGroup.SetAddChildListenter(OnAddRankItem);

            m_DDDServer.onValueChanged.AddListener(listener.OnDropdownEvent);

            m_OwnRankItem.SetOnClickListener(listener.OnClickRankItemGet);

            m_DDDServer.onShowListEvent.AddListener(OnDropdownList);


        }


        private void OnAddRankItem(RankItem item)
        {
            item.SetOnClickListener(m_Listener.OnClickRankItemGet);
        }


        private void OnDropdownList(bool b)
        {
            m_DropArrow.transform.localScale = !b ? Vector3.one : new Vector3(1, -1, 1);
        }
        public void SetRankSize(int size)
        {
            m_ItemGroup.SetChildSize(size);
        }

        public void SetMySelfInfoActive(bool b)
        {
            m_OwnRankItem.SetNoInfo(!b);
        }
        public void SetMySelfRankInfo(int num, string name, string danstr, int danStar, string rate)
        {
            m_OwnRankItem.Set(num, name, danstr, danStar, rate);

            m_OwnRankItem.Index = -1;
        }    
        public void SetRankInfo(int index, int num, string name, string danstr, int danStar, string rate)
        {
            var item = m_ItemGroup.getAt(index);

            if (item == null)
                return;

            item.Set(num, name, danstr, danStar, rate);

            item.Index = index;
        }

        public void GetRewardPosition(int index,Vector3[] points)
        {
            if (index == -1)
            {
                m_OwnRankItem.GetRewardPosition(points);
                return;
            }

            var item = m_ItemGroup.getAt(index);

            if (item == null)
                return ;

             item.GetRewardPosition(points);
        }
        public void SetServerDrop(List<string> listname)
        {
            //int count = listname.Count;
            //m_DrapdownItemGroup.SetChildSize(count);

            //for (int i = 0; i < count; i++)
            //{
            //    var item = m_DrapdownItemGroup.getAt(i);
            //    if (item != null)
            //    {
            //        item.Index = i;

            //        item.SetLable(listname[i]);
                    
            //    }
            //}

            m_DDDServer.options.Clear();
            m_DDDServer.AddOptions(listname);
        }

        public void SetServerDropFocus(int index)
        {
            m_DDDServer.value = index;
        }
    }


    public partial class UI_Pvp_SingleRank_Layout
    {
        public interface IListener
        {
            void OnClickClose();
 
            void OnClickRankItemGet(int index);

            void OnDropdownEvent(int index);

        
        }
    }

    public partial class UI_Pvp_SingleRank_Layout
    {
        class RankItem : IntClickItem
        {

          //  private Text m_TexLabel;
            protected Button m_BtnReward;

            protected Text m_TexRate;

            protected Text m_TexDan;
            protected Text m_TexDanStar;

            protected Text m_TexRoleName;

            protected Text m_TexRankNum;
            protected Transform[] m_TransRankIcon = new Transform[3];

            public override void Load(Transform root)
            {
                base.Load(root);

               // m_TexLabel = root.Find("Rank/Text_Rank").GetComponent<Text>();
                m_BtnReward = root.Find("Button_Reward").GetComponent<Button>();

                m_BtnReward.onClick.AddListener(OnClickGet);

                m_TexRate = root.Find("Text_Rate").GetComponent<Text>();

                m_TexDan = root.Find("Text_Rank").GetComponent<Text>();
                m_TexDanStar = root.Find("Text_Rank/Text_Rank2").GetComponent<Text>();

                m_TexRoleName = root.Find("Text_Name").GetComponent<Text>();

                m_TexRankNum = root.Find("Rank/Text_Rank").GetComponent<Text>();


                m_TransRankIcon[0] = root.Find("Rank/Image_Icon");
                m_TransRankIcon[1] = root.Find("Rank/Image_Icon1");
                m_TransRankIcon[2] = root.Find("Rank/Image_Icon2");
            }

            public override ClickItem Clone()
            {
                return Clone<RankItem>(this);
            }
            private void OnClickGet()
            {
                clickItemEvent.Invoke(Index);
            }


            public void SetOnClickListener(UnityAction<int>  action)
            {
                clickItemEvent.AddListener(action);
            }

            public void Set(int num, string name, string danstr, int danStar, string rate)
            {
                m_TexRankNum.text = num == 0 ? LanguageHelper.GetTextContent(10168): num.ToString();
                m_TexRoleName.text = name;
                m_TexDan.text = danstr;
                m_TexDanStar.text = "×" + danStar.ToString();
                m_TexRate.text = rate;

                m_TransRankIcon[0].gameObject.SetActive(num == 1);

                m_TransRankIcon[1].gameObject.SetActive(num == 2);

                m_TransRankIcon[2].gameObject.SetActive(num == 3);

                bool b = (num <= 100 && num >0);
                if (m_BtnReward.gameObject.activeSelf != b)
                    m_BtnReward.gameObject.SetActive(b);


            }

            public void GetRewardPosition(Vector3[] points)
            {
                RectTransform rect = m_BtnReward.transform as RectTransform;

                rect.GetWorldCorners(points);

            }
        }

        class OwnRankItem : RankItem
        {
            Transform m_TransOwnRankNo;

            Transform m_TransHaveInfo;
            public override void Load(Transform root)
            {
                m_TransHaveInfo = root.Find("HaveInfo");

                base.Load(m_TransHaveInfo);

                m_TransOwnRankNo = root.Find("Text_NoInfo");
            }

            public void SetNoInfo(bool b)
            {
                m_TransOwnRankNo.gameObject.SetActive(b);
                m_TransHaveInfo.gameObject.SetActive(!b);
            }



        }
    }

    public partial class UI_Pvp_SingleRank_Layout
    {
        class RankDropdownItem: IntClickItem
        {
            private Text m_TexLabel;
            public override void Load(Transform root)
            {
                base.Load(root);

                m_TexLabel = root.Find("Text").GetComponent<Text>();
            }

            public override ClickItem Clone()
            {
                return Clone<RankDropdownItem>(this);
            }

            public void SetLable(string str)
            {
                m_TexLabel.text = str;
            }
        }
    }


    public partial class UI_Pvp_SingleRank_Layout
    {
        class RankPropItem : IntClickItem
        {
            PropItem m_Item;
            PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, false, false);
            public override void Load(Transform root)
            {
                base.Load(root);

                m_Item = new PropItem();

                m_Item.BindGameObject(root.gameObject);
            }


            public void SetReward(ItemIdCount item)
            {
                m_ItemData.id = item.id;
                m_ItemData.count = item.count;

                // m_Item.SetData(new MessageBoxEvt(EUIID.UI_DailyActivites_Detail, m_ItemData));
            }
        }
    }
}
