using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;

namespace Logic
{
    public partial class UI_GoddnessTrial_Layout
    {
        class ChapterItem:ClickItem
        {
            private Image m_IIcon;

            private CP_Toggle m_Toggle;

            private Action<uint> OnclickAc;

            public uint ID { get; set; }
            public override void Load(Transform root)
            {
                base.Load(root);

                m_IIcon = root.Find("Image_ICON").GetComponent<Image>();

                m_Toggle = root.GetComponent<CP_Toggle>();
            }

            public void SetListener(IListener listener)
            {
                m_Toggle.onValueChanged.AddListener(OnToggleValChangle);

                OnclickAc = listener.OnClickChapter;
            }

            private void OnToggleValChangle(bool state)
            {
                if (OnclickAc != null&&state)
                    OnclickAc.Invoke(ID);
            }

            public void SetIcon(uint icon)
            {
                ImageHelper.SetIcon(m_IIcon, icon);
            }

            public void SetToggleOn(bool b,bool sendmsg)
            {

                m_Toggle.SetSelected(b, sendmsg);
            }
        }
    }

    public partial class UI_GoddnessTrial_Layout
    {
        class DropItem : ClickItem
        {

            private PropItem mPropItem = new PropItem();

            private PropIconLoader.ShowItemData  mShowItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, true, false);
            public override void Load(Transform root)
            {
                base.Load(root);

                mPropItem.BindGameObject(root.gameObject);
            }

            public override ClickItem Clone()
            {
                return Clone<DropItem>(this);
            }


            public void SetData(ItemIdCount data)
            {
                mShowItemData.id = data.id;
                mShowItemData.count = data.count;

                mPropItem.SetData(new MessageBoxEvt(EUIID.UI_GoddessTrial, mShowItemData));

            }
        }

        class DifficItem : ClickItem
        {
            private CP_Toggle m_Toggle;

            private Action<int,int> m_OnToggleValChangeAc;
            public uint ID { get; set; }
            public int Index { get; set; }

            private Text m_TNormal;
            private Text m_TSelect;

            private Transform m_TransLock;
            public override void Load(Transform root)
            {
                base.Load(root);

                m_Toggle = root.GetComponent<CP_Toggle>();

                m_TNormal = root.Find("Text01").GetComponent<Text>();
                m_TSelect = root.Find("Text_Select").GetComponent<Text>();

                m_TransLock = root.Find("lock");
            }

            public override ClickItem Clone()
            {
                return Clone<DifficItem>(this);
            }

            public void SetListener(IListener listener)
            {
                m_OnToggleValChangeAc = listener.OnClickDifficulty;

                m_Toggle.onValueChanged.AddListener(OnToggleValChangle);
            }

            private void OnToggleValChangle(bool state)
            {
                if (m_OnToggleValChangeAc != null && state)
                    m_OnToggleValChangeAc.Invoke((int)ID,Index);
            }

            public void SetText(uint langue)
            {
                TextHelper.SetText(m_TNormal, langue);
                TextHelper.SetText(m_TSelect, langue);
            }

            public void SetFocus(bool b,bool sendmsg)
            {
                m_Toggle.SetSelected(b, sendmsg);
            }

            public void SetInteractable(bool b)
            {
                m_Toggle.interactable = b;

                m_TransLock.gameObject.SetActive(!b);
            }
        }
    }
    public partial class UI_GoddnessTrial_Layout
    {
        private RawImage m_ITitle;
        private Text m_TLevel;
        private Text m_TCDInfo;


        private Button m_BtnCharacter;
        private Button m_BtnEnding;

        ClickItemGroup<ChapterItem> m_ChapterGroup = new ClickItemGroup<ChapterItem>() { AutoClone = false };

        private Button m_BtnClose;
        public void SetChapter(int index, uint id, uint icon)
        {
            var item = m_ChapterGroup.getAt(index);

            if (item == null)
                return;

            item.ID = id;
            item.SetIcon(icon);
        }


        public void SetTitle(string name)
        {
            ImageHelper.SetTexture(m_ITitle, name+".png");
        }

        public void SetLevelStr(uint minlevel, uint maxlevel)
        {
            m_TLevel.text = LanguageHelper.GetTextContent(2022302, minlevel.ToString(), maxlevel.ToString());
        }

        public void SetFocusCapter(int index,bool sendmsg)
        {
            var item = m_ChapterGroup.getAt(index);

            if (item == null)
                return;

            item.SetToggleOn(true, sendmsg);
        }
    }


    public partial class UI_GoddnessTrial_Layout
    {
        private Transform m_TransDetail;
        private Transform m_TransDetailBG;

        Button m_BtnRank;
        Button m_BtnFastTeam;
        Button m_BtnGoWithTeam;
        Button m_BtnDetailClose;
        Button m_BtnDeDetail;

        ClickItemGroup<DropItem> m_DropGroup = new ClickItemGroup<DropItem>();
        ClickItemGroup<DifficItem> m_DifficGroup = new ClickItemGroup<DifficItem>();

        private Text m_TInstanceTitle;
        private Text m_TInInfo;
        private Text m_TRecommend;
        private Text m_TOneself;

        private Image m_IDTitle;

        
        public void SetShowDetail(bool bshow)
        {
            if (m_TransDetailBG.gameObject.activeSelf != bshow)
            {
                m_TransDetail.gameObject.SetActive(bshow);
                m_TransDetailBG.gameObject.SetActive(bshow);
            }
        }

        public void SetDetailName(uint id)
        {
            if (id == 0)
                return;

            TextHelper.SetText(m_TInstanceTitle, id);
        }

        public void SetDetailTitleImage(string path)
        {
            ImageHelper.SetIcon(m_IDTitle, path);
        }

        public void SetDetailInfo(uint langueID)
        {
            TextHelper.SetText(m_TInInfo, langueID);
        }
        public void SetDetailScore(uint recommand, uint ownself)
        {
            m_TRecommend.text = recommand.ToString();
            m_TOneself.text = ownself.ToString();

            Color color = recommand > ownself ? Color.red : Color.green;// new Color(147 / 255f, 104 / 255f, 66 / 255f);

            m_TOneself.color = color;
        }

        public void SetDifficlySize(int size)
        {
            m_DifficGroup.SetChildSize(size);
        }

        public void SetDifficlyID(int index, uint id, uint langueID)
        {
           var item =  m_DifficGroup.getAt(index);

            if (item == null)
                return;

            item.ID = id;
            item.Index = index;

            item.SetText(langueID);
        }

        public void SetDifficlyFocuse(int index,bool sendmsg = true)
        {
            var item = m_DifficGroup.getAt(index);

            if (item == null)
                return;

            item.SetFocus(true,sendmsg);
        }

        public void SetDifficInteractable(int index,bool b)
        {
            var item = m_DifficGroup.getAt(index);

            if (item == null)
                return;

            item.SetInteractable(b);
        }
        public void SetDetailDrop(List<ItemIdCount> dropItems)
        {
            int count = dropItems.Count;
            m_DropGroup.SetChildSize(count);

            for (int i = 0; i < count; i++)
            {
                var item = m_DropGroup.getAt(i);

                if (item != null)
                    item.SetData(dropItems[i]);
            }
        }

    }
    public partial class UI_GoddnessTrial_Layout 
    {
        IListener m_Listener;
        public void Load(Transform root)
        {

            m_ITitle = root.Find("Animator/View_Message/Image_Title").GetComponent<RawImage>();
            m_TLevel = root.Find("Animator/View_Message/Text_Level").GetComponent<Text>();
            m_TCDInfo = root.Find("Animator/View_Message/Text_Tips01").GetComponent<Text>();

            m_BtnCharacter = root.Find("Animator/View_Message/Btn_Character").GetComponent<Button>();
            m_BtnEnding = root.Find("Animator/View_Message/Btn_Ending").GetComponent<Button>();

            Transform toggleGroupTrans = root.Find("Animator/Toggle_List/Toggle_Group");

            int count = toggleGroupTrans.childCount;

            for (int i = 1; i <= count; i++)
            {
                var item = toggleGroupTrans.Find("Toggle0" + i);

                if (item != null)
                {
                    m_ChapterGroup.AddChild(item);
                }
            }



            m_TransDetailBG = root.Find("Animator/Image_Blank");

            m_TransDetail = root.Find("Animator/View_Details");

            m_BtnRank = m_TransDetail.Find("Btn_03").GetComponent<Button>();
            m_BtnFastTeam = m_TransDetail.Find("Btn_02").GetComponent<Button>();
            m_BtnGoWithTeam = m_TransDetail.Find("Btn_01").GetComponent<Button>();

            Transform difficitem = m_TransDetail.Find("Scroll_View/Toggle_List/Toggle01");
            m_DifficGroup.AddChild(difficitem);


            Transform dropitem = m_TransDetail.Find("Scroll_View_drop/Grid_Item/PropItem");
            m_DropGroup.AddChild(dropitem);

            m_IDTitle = m_TransDetail.Find("Image_Title").GetComponent<Image>();

            m_TInstanceTitle = m_TransDetail.Find("Text_Title").GetComponent<Text>();

            m_TInInfo = m_TransDetail.Find("Text_Discrib").GetComponent<Text>();

            m_TRecommend = m_TransDetail.Find("Image_Frame01/Text_Num").GetComponent<Text>();

            m_TOneself = m_TransDetail.Find("Image_Frame02/Text_Num").GetComponent<Text>();

            m_BtnDetailClose = m_TransDetail.Find("Btn_Close").GetComponent<Button>();
            m_BtnDeDetail = m_TransDetail.Find("Btn_Detail").GetComponent<Button>();


            m_BtnClose = root.Find("Animator/View_Title08/Btn_Close").GetComponent<Button>();
        }

        public void SetListener(IListener listener)
        {
            m_Listener = listener;

            m_BtnCharacter.onClick.AddListener(listener.OnClickCharacter);

            m_BtnEnding.onClick.AddListener(listener.OnClickEnding);

            m_BtnClose.onClick.AddListener(listener.OnClickClose);

            int count = m_ChapterGroup.Size;

            for (int i = 0; i < count; i++)
            {
               var item = m_ChapterGroup.getAt(i);

                if (item != null)
                    item.SetListener(listener);
            }


            m_BtnDeDetail.onClick.AddListener(listener.OnClickDeDetail);
            m_BtnDetailClose.onClick.AddListener(listener.OnClickCloseDetail);
            m_BtnRank.onClick.AddListener(listener.OnClickRand);
            m_BtnFastTeam.onClick.AddListener(listener.OnClickFastTeam);
            m_BtnGoWithTeam.onClick.AddListener(listener.OnClickGoWithTeam);

            m_DifficGroup.SetAddChildListenter(OnAddDifficlyItem);

        }

        private void OnAddDifficlyItem(DifficItem item)
        {
            item.SetListener(m_Listener);
        }
    }



    public partial class UI_GoddnessTrial_Layout
    {
        public interface IListener
        {
            void OnClickClose();
            void OnClickCharacter();
            void OnClickEnding();

            void OnClickChapter(uint id);

            void OnClickDifficulty(int id,int index);//困难度

            void OnClickRand();
            void OnClickFastTeam();
            void OnClickGoWithTeam();

            void OnClickCloseDetail();

            void OnClickDeDetail();
        }
    }


}
