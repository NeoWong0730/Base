using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_Goddness_Difficult_Layout
    {
        class DifficultName : ClickItem
        {
            private Text m_Name;
            public override void Load(Transform root)
            {
                base.Load(root);

                m_Name = root.GetComponent<Text>();
            }

            public void SetName(string name)
            {
                TextHelper.SetText(m_Name, name);
            }

            public override ClickItem Clone()
            {
                return Clone<DifficultName>(this);
            }
        }
        class DifficultItem:ClickItem
        {
            private Image m_IDifficult;
            private Text m_TFristTime;

            private Transform m_TransSelected;

            private Transform m_TransLock;

            ClickItemGroup<DifficultName> m_TeamNameGroup = new ClickItemGroup<DifficultName>();

            private Button m_BtnSelect;
            public int Index { get; set; }
            public uint ID { get; set; }

            IListener m_Listener;

            private Button m_BtnFristReward;

            private Image m_ImgFristReward;
            public override void Load(Transform root)
            {
                base.Load(root);

                m_IDifficult = root.Find("Image_Difficult").GetComponent<Image>();

                m_TFristTime = root.Find("Text_Time").GetComponent<Text>();

                m_TeamNameGroup.AddChild(root.Find("Grid/Text_Name"));

                m_TransSelected = root.Find("Image_Select");
                m_TransLock = root.Find("Image_Unlock");

                m_BtnSelect = root.Find("Btn_01").GetComponent<Button>();

                m_BtnFristReward = root.Find("Btn_02").GetComponent<Button>();
                m_ImgFristReward = root.Find("Btn_02").GetComponent<Image>();

            }


            public override ClickItem Clone()
            {
                return Clone<DifficultItem>(this);
            }


            public void SetListener(IListener listener)
            {
                m_Listener = listener;

                m_BtnSelect.onClick.AddListener(OnClickSelect);
                m_BtnFristReward.onClick.AddListener(OnClickFristReward);
            }

            private void OnClickSelect()
            {
                m_Listener?.OnClickDifficult(Index, ID);
            }

            private void OnClickFristReward()
            {
                m_Listener?.OnClickDifficultFristReward(Index, ID);
            }
            public void SetNames(List<string> names)
            {
                int count = names.Count;
                m_TeamNameGroup.SetChildSize(count);

                for (int i = 0; i < count; i++)
                {
                  var item = m_TeamNameGroup.getAt(i);

                    if (item != null)
                        item.SetName(names[i]);
                }
            }

            public void SetSelectActive(bool isActive)
            {
                if (m_TransSelected.gameObject.activeSelf != isActive)
                    m_TransSelected.gameObject.SetActive(isActive);

                //m_BtnSelect.gameObject.SetActive(!isActive);
            }

            public void SetLock(bool isActive)
            {
                if (m_TransLock.gameObject.activeSelf != isActive)
                    m_TransLock.gameObject.SetActive(isActive);

               // m_BtnSelect.gameObject.SetActive(!isActive);
            }

            public void SetSelectBtnActive(bool b)
            {
                if (m_BtnSelect.gameObject.activeSelf != b)
                    m_BtnSelect.gameObject.SetActive(b);
            }
            public void SetFristTime(string time)
            {
                TextHelper.SetText(m_TFristTime, time);
            }

            public void SetDifficultIcon(uint icon)
            {
                ImageHelper.SetIcon(m_IDifficult, icon);
            }

            public void SetFristRewardActive(bool active)
            {
                if (m_BtnFristReward.gameObject.activeSelf != active)
                    m_BtnFristReward.gameObject.SetActive(active);
            }

            public void SetFristRewardIcon(uint icon)
            {
                ImageHelper.SetIcon(m_ImgFristReward, icon);
            }
        }
    }
    public partial class UI_Goddness_Difficult_Layout
    {
        private ClickItemGroup<DifficultItem> m_DifficGroup = new ClickItemGroup<DifficultItem>();

        private Button m_BtnClose;

        private Button m_BtnRank;

        IListener m_Listener;

       

        public void SetDifficultCount(int count)
        {
            m_DifficGroup.SetChildSize(count);
        }

        public void SetSelect(int index,bool bSelsect)
        {
            var item = m_DifficGroup.getAt(index);

            if (item == null)
                return;

            item.SetSelectActive(bSelsect);
        }

        public void SetLock(int index, bool block)
        {
            var item = m_DifficGroup.getAt(index);

            if (item == null)
                return;

            item.SetLock(block);
        }

        public void SetSelectBtnActive(int index, bool b)
        {
            var item = m_DifficGroup.getAt(index);

            if (item == null)
                return;

            item.SetSelectBtnActive(b);
        }
        public void SetFristName(int index, List<string> names)
        {
            var item = m_DifficGroup.getAt(index);

            if (item == null)
                return;

            item.SetNames(names);
        }

        public void SetFristTime(int index, string time)
        {
            var item = m_DifficGroup.getAt(index);

            if (item == null)
                return;

            item.SetFristTime(time);
        }

        public void SetDifficultIcon(int index, uint icon)
        {
            var item = m_DifficGroup.getAt(index);

            if (item == null)
                return;

            item.SetDifficultIcon(icon);
        }

        public void SetDifficultID(int index, uint id)
        {
            var item = m_DifficGroup.getAt(index);

            if (item == null)
                return;

            item.Index = index;
            item.ID = id;
        }

        public void SetDifficultFristReward(int index, bool active)
        {
            var item = m_DifficGroup.getAt(index);

            if (item == null)
                return;

            item.SetFristRewardActive(active);
        }

        public void SetDifficultFristRewardIcon(int index, uint icon)
        {
            var item = m_DifficGroup.getAt(index);

            if (item == null)
                return;

            item.SetFristRewardIcon(icon);
        }
    }
    public partial class UI_Goddness_Difficult_Layout
    {
       
        public void Load(Transform root)
        {
            m_DifficGroup.AddChild(root.Find("Animator/List/Grid/Item"));

            m_BtnClose = root.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>();

            m_BtnRank = root.Find("Animator/Btn_01").GetComponent<Button>();
        }

        public void SetListener(IListener listener)
        {
            m_Listener = listener;

            m_BtnClose.onClick.AddListener(listener.OnClickClose);
            m_BtnRank.onClick.AddListener(listener.OnClickRank);

            m_DifficGroup.SetAddChildListenter(OnAddDifficultItem);
        }

        private void OnAddDifficultItem(DifficultItem item)
        {
            item.SetListener(m_Listener);
        }
    }

    public partial class UI_Goddness_Difficult_Layout
    {
        public interface IListener
        {
            void OnClickClose();

            void OnClickRank();
            void OnClickDifficult(int index, uint id);

            void OnClickDifficultFristReward(int index, uint id);
        }
    }
}
