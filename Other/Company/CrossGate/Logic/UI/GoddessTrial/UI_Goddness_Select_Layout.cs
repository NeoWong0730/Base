using System;
using System.Collections.Generic;

using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
namespace Logic
{
    public partial class UI_Goddness_Select_Layout
    {
        class SelectItem : ClickItem
        {
            CP_Toggle m_Toggle;

            Action<uint,uint> mClickAc;

            public uint Index { get; set; }
            public uint ID { get; set; }

            private Text m_TNormal;
            private Text m_TSelect;

            private Transform m_TransResult;
            private Text m_TResult;
            private Slider m_SProcess;
            public override void Load(Transform root)
            {
                base.Load(root);

                m_Toggle = root.GetComponent<CP_Toggle>();

                m_TNormal = root.Find("Text").GetComponent<Text>();
                m_TSelect = root.Find("Text_Select").GetComponent<Text>();

                m_TransResult = root.Find("Result");
                m_TResult = root.Find("Result/Text_Picket").GetComponent<Text>();
                m_SProcess = root.Find("Result/Slider").GetComponent<Slider>();
            }

            public override ClickItem Clone()
            {
                return Clone<SelectItem>(this);
            }
            public void SetListener(IListener listener)
            {
                m_Toggle.onValueChanged.AddListener(OnClick);

                mClickAc = listener.OnClickSelect;
            }

            private void OnClick(bool state)
            {
                if (mClickAc != null&&state)
                    mClickAc.Invoke(Index,ID);
            }

            public void SetToggleOn(bool b, bool send)
            {
                m_Toggle.SetSelected(b, send);
            }
            public void SetText(uint str)
            {
                TextHelper.SetText(m_TNormal, str);
                TextHelper.SetText(m_TSelect, str);
            }

            public void SetReslutText(string str)
            {
                TextHelper.SetText(m_TResult, str);
            }

            public void SetResultActive(bool b)
            {
                if (m_TransResult.gameObject.activeSelf != b)
                    m_TransResult.gameObject.SetActive(b);
            }

            public void SetResultProcess(float process)
            {
                m_SProcess.value = process;
            }
        }
    }
    public partial class UI_Goddness_Select_Layout
    {
        private Text m_TTitle;
        private Text m_TTitle01;

        private Transform m_TransView0;
        private Text m_TDiscrib0;
        private Button m_BtnView0Close;

        private Transform m_TransView1;
        private Text m_TDiscrib1;
        private Text m_TTime;
        private ClickItemGroup<SelectItem> m_GroupSelect = new ClickItemGroup<SelectItem>();

        private CP_Toggle m_TogFollowCaptain;
        private Transform m_TransFollow;

        public void SetViewDisActive(bool b)
        {

            if (m_TransView0.gameObject.activeSelf != b)
                m_TransView0.gameObject.SetActive(b);
        }

        public void SetTitle(string strTitle0, string strTitle1)
        {
            TextHelper.SetText(m_TTitle, strTitle0);
            TextHelper.SetText(m_TTitle01, strTitle1);
        }
        public void SetViewDisInfoText(uint langue)
        {
            if (langue == 0)
                return;

            TextHelper.SetText(m_TDiscrib0, langue);
        }

        public void ShowSelectResultActive()
        {
            SetSelectActive(true);

            m_TransTime.gameObject.SetActive(false);

        }
        public void SetSelectActive(bool b)
        {
            if (m_TransView1.gameObject.activeSelf != b)
                m_TransView1.gameObject.SetActive(b);
        }

        public void SetSelectInfoText(uint langue)
        {
            TextHelper.SetText(m_TDiscrib1, langue);
        }

        public void SetSelectTime(string time)
        {
            m_TTime.text = time;
        }

        public void SelectTimeActive(bool b)
        {
            m_TTime.gameObject.SetActive(b);
        }
        public void SetSelectCount(int count)
        {
            m_GroupSelect.SetChildSize(count);
        }

        public int GetSelectItemCount()
        {
            return m_GroupSelect.Count;
        }

        public void SetSelect(int index, uint id, uint langue, bool isSelected)
        {
            var item = m_GroupSelect.getAt(index);
            if (item == null)
                return;

            item.Index = (uint)index;
            item.ID = id;

            item.SetText(langue);

            item.SetToggleOn(isSelected, false);
        }

        //public uint GetSelectID(int index)
        //{
        //    var item = m_GroupSelect.getAt(index);
        //    if (item == null)
        //        return;

        //}
        public void SetSelectToggleOn(int index, bool isSelected, bool send)
        {
            var item = m_GroupSelect.getAt(index);
            if (item == null)
                return;

            item.SetToggleOn(isSelected, false);
        }

        public void SetSelectToggleOn(uint id, bool b)
        {
            int count = m_GroupSelect.Count;
            for (int i = 0; i < count; i++)
            {
                var item = m_GroupSelect.getAt(i);
                if (item != null && item.ID == id)
                {
                    item.SetToggleOn(b, false);
                    return;
                }
                   
            }


           
        }
        public void SetFollow(bool b)
        {
            m_TogFollowCaptain.SetSelected(b, false);
        }

        public void SetFollowActive(bool b)
        {
            if (m_TransFollow.gameObject.activeSelf != b)
                m_TransFollow.gameObject.SetActive(b);
        }
        public void SetSelectResult(int index, string result, float process)
        {
            var item = m_GroupSelect.getAt(index);
            if (item == null)
                return;

            item.SetResultActive(true);

            item.SetReslutText(result);

            item.SetResultProcess(process);
        }


        public void SetSelectResultActive(int index, bool active)
        {
            var item = m_GroupSelect.getAt(index);
            if (item == null)
                return;

            item.SetResultActive(active);
        }
    }
    public partial class UI_Goddness_Select_Layout
    {
        IListener m_Listener;

        private Transform m_TransTime;

        private Button m_BtnSelectClose;
        public void Load(Transform root)
        {
            m_TTitle = root.Find("Animator/Text_Title").GetComponent<Text>();
            m_TTitle01 = root.Find("Animator/Text_Title01").GetComponent<Text>();

            m_TransView0 = root.Find("Animator/View_Type1");
            m_TDiscrib0 = m_TransView0.Find("Text_Discrib").GetComponent<Text>();
            m_BtnView0Close = m_TransView0.Find("close").GetComponent<Button>();

            m_TransView1 = root.Find("Animator/View_Type2");
            m_TDiscrib1 = m_TransView1.Find("Text_Discrib").GetComponent<Text>();

            m_TransTime = m_TransView1.Find("Text_Time");
            m_TTime = m_TransView1.Find("Text_Time/Text_Num").GetComponent<Text>();

            m_TransFollow = m_TransView1.Find("follow");
            m_TogFollowCaptain = m_TransView1.Find("follow/itemProto").GetComponent<CP_Toggle>();

           var selectItem =  m_TransView1.Find("Grid/Toggle01");
            m_GroupSelect.AddChild(selectItem);

            m_BtnSelectClose = m_TransView1.Find("close").GetComponent<Button>();

        }
        public void SetListener(IListener listener)
        {
            m_Listener = listener;

            m_GroupSelect.SetAddChildListenter(OnAddSelectItem);
            m_BtnView0Close.onClick.AddListener(listener.OnClickClose);

            m_BtnSelectClose.onClick.AddListener(listener.OnClickSelectClose);

            m_TogFollowCaptain.onValueChanged.AddListener(listener.OnClickFollow);
        }

        private void OnAddSelectItem(SelectItem item)
        {
            item.SetListener(m_Listener);
        }
    }

    public partial class UI_Goddness_Select_Layout
    {
        public interface IListener
        {
            void OnClickClose();
            void OnClickSelect(uint index,uint ID);

            void OnClickFollow(bool b);

            void OnClickSelectClose();
        }
    }
}
