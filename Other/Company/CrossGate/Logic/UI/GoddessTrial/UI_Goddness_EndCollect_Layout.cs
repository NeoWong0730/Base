using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Logic
{
    public partial class UI_Goddness_EndCollect_Layout
    {
        class CollectItem : ClickItem
        {
            Transform m_TransGet;
            Text m_TGetTitle;
            Text m_TGetInfo;
            Button m_BtnGet;

            Transform m_TransUnGet;
            Text m_TUnGetTitle;

            public uint ID { get; set; }

            IListener m_Listener;
            public override void Load(Transform root)
            {
                base.Load(root);

                m_TransGet = root.Find("Get");
                m_TGetTitle = m_TransGet.Find("Text_Title").GetComponent<Text>();
                m_TGetInfo = m_TransGet.Find("Text").GetComponent<Text>();
                m_BtnGet = m_TransGet.Find("Btn_01").GetComponent<Button>();

                m_TransUnGet = root.Find("Unget");
                m_TUnGetTitle = m_TransUnGet.Find("Text_Title").GetComponent<Text>();
            }

            public override ClickItem Clone()
            {
                return Clone<CollectItem>(this);
            }

            public void SetTitleName(uint langueID)
            {
                TextHelper.SetText(m_TGetTitle, langueID);
                TextHelper.SetText(m_TUnGetTitle, langueID);
            }

            public void SetTitleInfo(uint langueID)
            {
                TextHelper.SetText(m_TGetInfo, langueID);
            }

            public void SetGet(bool isGet)
            {
                if (isGet != m_TransGet.gameObject.activeSelf)
                    m_TransGet.gameObject.SetActive(isGet);

                if (isGet == m_TransUnGet.gameObject.activeSelf)
                    m_TransUnGet.gameObject.SetActive(!isGet);

            }

            public void SetListener(IListener listener)
            {
                m_Listener = listener;

                m_BtnGet.onClick.AddListener(OnClick);
            }

            private void OnClick()
            {
                m_Listener?.OnClickGet(ID);
            }
        }


        public void SetCollectCount(int count)
        {
            m_CollectGroup.SetChildSize(count);
        }

        public void SetCollectTitle(int index, uint langueID)
        {
            var item = m_CollectGroup.getAt(index);
            if (item == null)
                return;

            item.SetTitleName(langueID);
        }

        public void SetCollectInfo(int index, uint langueID)
        {
            var item = m_CollectGroup.getAt(index);
            if (item == null)
                return;

            item.SetTitleInfo(langueID);
        }

        public void SetCollectGet(int index, bool isGet)
        {
            var item = m_CollectGroup.getAt(index);
            if (item == null)
                return;

            item.SetGet(isGet);
        }

        public void SetCollectID(int index, uint id)
        {
            var item = m_CollectGroup.getAt(index);
            if (item == null)
                return;

            item.ID = id;
        }
    }
    public partial class UI_Goddness_EndCollect_Layout
    {
        Button m_BtnClose;

        Text m_TProcess;

        Slider m_SlProcess;

        Button m_BtnOneKeyGet;

        ClickItemGroup<CollectItem> m_CollectGroup = new ClickItemGroup<CollectItem>();


        private Image m_ImgRewardBox;

        public void SetProcess(int cur, int max)
        {
            m_TProcess.text = cur.ToString() + "/" + max.ToString();
            m_SlProcess.value = cur / (max * 1.0f);
        }

        public void SetFxActive(bool active)
        {
            if (m_TransFx.gameObject.activeSelf != active)
                m_TransFx.gameObject.SetActive(active);
        }


        public void SetRewardImageIcon(uint icon)
        {
            ImageHelper.SetIcon(m_ImgRewardBox, icon);
        }

    }
    public partial class UI_Goddness_EndCollect_Layout
    {
        IListener m_Listener;

        private Transform m_TransFx;
        public void Load(Transform root)
        {
            m_BtnClose = root.Find("Animator/View_TipsBgNew07/Btn_Close").GetComponent<Button>();

            m_TProcess = root.Find("Animator/Image_Collect/Text_Num").GetComponent<Text>();

            m_SlProcess = root.Find("Animator/Image_Collect/Slider").GetComponent<Slider>();

            m_BtnOneKeyGet = root.Find("Animator/Image_Collect/Btn/Image").GetComponent<Button>();

            m_CollectGroup.AddChild(root.Find("Animator/List/Grid/Item"));

            m_TransFx = root.Find("Animator/Image_Collect/Btn/Image/Fx_ui_item");

            m_ImgRewardBox = root.Find("Animator/Image_Collect/Btn/Image").GetComponent<Image>();
        }

        public void SetListener(IListener listener)
        {
            m_Listener = listener;

            m_CollectGroup.SetAddChildListenter(OnAddCollectItem);

            m_BtnOneKeyGet.onClick.AddListener(listener.OnClickAllGet);

            m_BtnClose.onClick.AddListener(listener.OnClickClose);
        }

        private void OnAddCollectItem(CollectItem item)
        {
            item.SetListener(m_Listener);
        }
    }

    public partial class UI_Goddness_EndCollect_Layout
    {
        public interface IListener
        {
            void OnClickClose();

            void OnClickGet(uint id);

            void OnClickAllGet();
        }
    }
}
