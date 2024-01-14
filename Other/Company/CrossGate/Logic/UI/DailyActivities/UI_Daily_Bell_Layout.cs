using System;
using System.Collections.Generic;

using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Framework;

namespace Logic
{
    public partial class UI_Daily_Bell_Layout
    {
        public class ClickToggleEvent
        {
            private Action<int, bool> mAction;

            public void AddListener(Action<int, bool> action)
            {
                mAction += action;
            }

            public void Invoke(int value0, bool value1)
            {
                mAction?.Invoke(value0, value1);
            }
        }
        class RedDotItem : IntClickItem
        {


            private Text mTexName;
            private Text mTexRound;
            private Toggle mToggle;
            private Button mBtnJoin;

            private Animator mAniToggle;

            public uint ConfigID { get; set; } = 0;

            public ClickToggleEvent ToggleEvent = new ClickToggleEvent();
            public override void Load(Transform root)
            {
                base.Load(root);

                mTexName = root.Find("TitleGrid/Image_Title/Text").GetComponent<Text>();
                mTexRound = root.Find("TitleGrid/Image_Title (1)/Text").GetComponent<Text>();
                mToggle = root.Find("TitleGrid/Image_Title (2)/Tog_Lock").GetComponent<Toggle>();
                mBtnJoin = root.Find("TitleGrid/Image_Title (3)/Btn_01").GetComponent<Button>();

                mBtnJoin.onClick.AddListener(OnClickBtn);
                mToggle.onValueChanged.AddListener(OnToggleValueChange);

                mAniToggle = mToggle.GetComponent<Animator>();

            }

            public override ClickItem Clone()
            {
                return Clone<RedDotItem>(this);
            }
            private void OnClickBtn()
            {
                clickItemEvent?.Invoke((int)ConfigID);
            }

            private void OnToggleValueChange(bool state)
            {
                ToggleEvent?.Invoke((int)ConfigID, state);

                string animName = state ? "Open" : "Close";

                mAniToggle.Play(animName);
            }

            public void SetName(string tex)
            {
                mTexName.text = tex;
            }

            public void SetRound(string tex)
            {
                mTexRound.text = tex;
            }

            public void SetToggleState(bool b)
            {
                string animName = b ? "Open" : "Close";

                mAniToggle.Play(animName);

                mToggle.isOn = b;



            }

        }

        public void SetRedName(int index, uint langue)
        {
            var item = mRedGroup.getAt(index);

            if (item == null)
                return;

            item.SetName(LanguageHelper.GetTextContent(langue));
        }

        public void SetRedRound(int index, uint langue)
        {
            var item = mRedGroup.getAt(index);

            if (item == null)
                return;

            item.SetRound(LanguageHelper.GetTextContent(langue));
        }

        public void SetRedToggle(int index, bool state)
        {
            var item = mRedGroup.getAt(index);

            if (item == null)
                return;

            item.SetToggleState(state);
        }

        public void SetRedCount(int count)
        {
            mRedGroup.SetChildSize(count);
        }

        public void SetRedConfigID(int index,uint configid)
        {
            var item = mRedGroup.getAt(index);

            if (item == null)
                return;

            item.ConfigID = configid;
        }
    }

    public partial class UI_Daily_Bell_Layout
    {

        class PushMessageItem : IntClickItem
        {
            private Text mTexName;
            private Text mTexRound;
            private Toggle mToggle;
            private Text mTexTime;
            private Text mTexPeople;

            private Animator mAniToggle;
            public uint ConfigID { get; set; } = 0;

            public ClickToggleEvent ToggleEvent = new ClickToggleEvent();
            public override void Load(Transform root)
            {
                base.Load(root);

                mTexName = root.Find("TitleGrid/Image_Title/Text").GetComponent<Text>();
                mTexRound = root.Find("TitleGrid/Image_Title (1)/Text").GetComponent<Text>();

                mTexPeople = root.Find("TitleGrid/Image_Title (2)/Text").GetComponent<Text>();
                mTexTime = root.Find("TitleGrid/Image_Title (3)/Text").GetComponent<Text>();
                mToggle = root.Find("TitleGrid/Image_Title (4)/Tog_Lock").GetComponent<Toggle>();

                mToggle.onValueChanged.AddListener(OnToggleValueChange);

                mAniToggle = mToggle.GetComponent<Animator>();
            }

            public override ClickItem Clone()
            {
                return Clone<PushMessageItem>(this);
            }
            private void OnToggleValueChange(bool state)
            {
                ToggleEvent?.Invoke((int)ConfigID, state);

                string animName = state ? "Open" : "Close";

                //mAniToggle.Play(animName);
            }

            public void SetName(string tex)
            {
                mTexName.text = tex;
            }

            public void SetRound(string tex)
            {
                mTexRound.text = tex;
            }

            public void SetTime(string tex)
            {
                mTexTime.text = tex;
            }

            public void SetPeople(string tex)
            {
                mTexPeople.text = tex;
            }

            public void SetToggleState(bool b)
            {
                string animName = b ? "Open" : "Close";

               // mAniToggle.Play(animName);

                mToggle.isOn = b;
            }



        }


        public void SetMessageName(int index, uint langue)
        {
            var item = mMessageGroup.getAt(index);

            if (item == null)
                return;

            item.SetName(LanguageHelper.GetTextContent(langue));
        }

        public void SetMessageRound(int index, uint langue)
        {
            var item = mMessageGroup.getAt(index);

            if (item == null)
                return;

            item.SetRound(LanguageHelper.GetTextContent(langue));
        }

        public void SetMessageTime(int index, uint langue)
        {
            var item = mMessageGroup.getAt(index);

            if (item == null)
                return;

            item.SetTime(LanguageHelper.GetTextContent(langue));
        }

        public void SetMessageTime(int index, string langue)
        {
            var item = mMessageGroup.getAt(index);

            if (item == null)
                return;

            item.SetTime(langue);
        }

        public void SetMessagePeople(int index, uint langue)
        {
            var item = mMessageGroup.getAt(index);

            if (item == null)
                return;

            item.SetPeople(LanguageHelper.GetTextContent(langue));
        }

        public void SetMessageCount(int count)
        {
            mMessageGroup.SetChildSize(count);
        }


        public void SetMessageToggle(int index,bool b)
        {
            var item = mMessageGroup.getAt(index);

            if (item == null)
                return;

            item.SetToggleState(b);
        }

        public void SetMessageConfigID(int index, uint configid)
        {
            var item = mMessageGroup.getAt(index);

            if (item == null)
                return;

            item.ConfigID = configid;
        }
    }
    public partial class UI_Daily_Bell_Layout
    {
        private Button mBtnClose;
        private Toggle mTogRed;
        private Toggle mTogMessage;

        private ClickItemGroup<RedDotItem> mRedGroup;
        private ClickItemGroup<PushMessageItem> mMessageGroup;

        private IListener m_Listener;

        private Text mTexTips;
        public void Load(Transform root)
        {
            mTogRed = root.Find("Animator/Menu/ListItem").GetComponent<Toggle>();
            mTogMessage = root.Find("Animator/Menu/ListItem (1)").GetComponent<Toggle>();

            Transform item0 = root.Find("Animator/View_Message01/ScrollView01/TabList/RankItem");
            mRedGroup = new ClickItemGroup<RedDotItem>(item0);

            Transform item1 = root.Find("Animator/View_Message02/ScrollView01/TabList/RankItem");
            mMessageGroup = new ClickItemGroup<PushMessageItem>(item1);


            mBtnClose = root.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();

            mTexTips = root.Find("Animator/Text_Tips").GetComponent<Text>();


        }

        public void SetListener(IListener listener)
        {
            m_Listener = listener;

            mRedGroup.SetAddChildListenter(OnAddItemRed);

            mMessageGroup.SetAddChildListenter(OnAddItemMessage);

            mTogRed.onValueChanged.AddListener(m_Listener.OnRedState);
            mTogMessage.onValueChanged.AddListener(m_Listener.OnMessageState);

            mBtnClose.onClick.AddListener(m_Listener.OnClickClose);
        }
        private void OnAddItemRed(RedDotItem item)
        {
            item.clickItemEvent.AddListener(m_Listener.OnClickJoin);
            item.ToggleEvent.AddListener(m_Listener.OnRedTogg);
        }

        private void OnAddItemMessage(PushMessageItem item)
        {
            item.ToggleEvent.AddListener(m_Listener.OnMessageTogg);
        }


        public void SetTipsText(uint langueID)
        {
            TextHelper.SetText(mTexTips, langueID);
        }
        public interface IListener
        {
            void OnClickJoin(int id);
            void OnRedTogg(int id, bool state);
            void OnMessageTogg(int id, bool state);

            void OnRedState(bool state);
            void OnMessageState(bool state);

            void OnClickClose();
        }
    }
}
