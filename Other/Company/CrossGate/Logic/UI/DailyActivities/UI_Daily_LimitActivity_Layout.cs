using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Logic.Core;

namespace Logic
{
    public partial class UI_Daily_LimitActivity_Layout
    {
        public class LimitItem : ClickItem
        {
            private Image mIIcon;
            private Text mName;
            private Text mTime;

            private Text m_TexType;

            private Button m_BtnGoto;
            public uint Config { get; set; }

            private IListerner m_IListener;

            private float m_OpenTime;
            public override void Load(Transform root)
            {
                base.Load(root);

                mIIcon = root.Find("Image_ICON").GetComponent<Image>();

                mName = root.Find("Text_Name").GetComponent<Text>();

                mTime = root.Find("Text_TIme").GetComponent<Text>();

                m_BtnGoto = root.Find("Btn_01_Small").GetComponent<Button>();

                m_TexType = root.Find("Text_Tips").GetComponent<Text>();
            }

            public override ClickItem Clone()
            {
                return Clone<LimitItem>(this);
            }

            public void SetIcon(Sprite sprite)
            {
                mIIcon.sprite = sprite;
            }

            public void SetIcon(uint sprite)
            {
                ImageHelper.SetIcon(mIIcon, sprite);
            }

            public void SetName(string name)
            {
                mName.text = name;
            }

            public void SetTime(string tex)
            {
                mTime.text = tex;
            }

            public void SetListener(IListerner listerner)
            {
                m_BtnGoto.onClick.AddListener(OnClickItem);

                m_IListener = listerner;
            }

            private void OnClickItem()
            {
                m_IListener?.OnClickItem(Config);
            }

            public void SetOpenTime(float time)
            {
                m_OpenTime = time;

                SetTime(GetTimeString(m_OpenTime));
            }

            public void UpdateTime(float offset)
            {
                var time  = m_OpenTime - offset;

                SetTime(GetTimeString(time));
            }

            private string GetTimeString(float time)
            {
                if (time <= 0)
                    return string.Empty;

                var mins = (int) (time / 60);
                var sceond = (int)(time - mins * 60);

                string strtime = time < 10 ? "0" + mins.ToString() : mins.ToString();
                strtime += ":";

                strtime += (sceond < 10 ? "0" + sceond.ToString() : sceond.ToString());

                return strtime;
            }

            public void SetActiveType(uint langue)
            {
                m_TexType.text = LanguageHelper.GetTextContent(langue);
            }
        }
    }
    public partial class UI_Daily_LimitActivity_Layout
    {
        private Button mBtnClose;

        private Button mBtnClear;

        private IListerner mListerner;

        private ClickItemGroup<LimitItem> mGroup = new ClickItemGroup<LimitItem>();
        public void Load(Transform transform)
        {
            mBtnClose = transform.Find("Animator/View_TipsBgNew03/Btn_Close").GetComponent<Button>();

            Transform item = transform.Find("Animator/View_Content/Scroll_View01/Viewport/Item01");

            mGroup.AddChild(item);

            mBtnClear = transform.Find("Animator/Btn_01").GetComponent<Button>();
        }

        private void OnAddItem(LimitItem item)
        {
            
            item.SetListener(mListerner);
        }
        public interface IListerner
        {
            void OnClickClose();

            void OnClickItem(uint configid);

            void OnClickClear();
        }
        public void SetLisitener(IListerner listerner)
        {
            mListerner = listerner;

            mBtnClose.onClick.AddListener(listerner.OnClickClose);

            mBtnClear.onClick.AddListener(listerner.OnClickClear);

            mGroup.SetAddChildListenter(OnAddItem);
        }

        private void OnClickItem(int index)
        {
            var item = mGroup.getAt(index);

            if (item == null)
                return;

            mListerner.OnClickItem(item.Config);
        }
    }

    public partial class UI_Daily_LimitActivity_Layout
    {
        public int GetSize()
        {
           return mGroup.Count;
        }
        public void SetSize(int count)
        {
            mGroup.SetChildSize(count);
        }
        public void SetName(int index, uint langueID)
        {
            var item = mGroup.getAt(index);

            if (item == null)
                return;

            item.SetName(LanguageHelper.GetTextContent(langueID));
        }

        public void SetIcon(int index, uint icon)
        {
            var item = mGroup.getAt(index);

            if (item == null)
                return;

            item.SetIcon(icon);
        }

        public void SetTime(int index, string tex)
        {
            var item = mGroup.getAt(index);

            if (item == null)
                return;

            item.SetTime(tex);
        }

        public void SetOpenTime(int index, float time)
        {
            var item = mGroup.getAt(index);

            if (item == null)
                return;

            item.SetOpenTime(time);
        }

        public void SetActityType(int index, uint langue)
        {
            var item = mGroup.getAt(index);

            if (item == null)
                return;

            item.SetActiveType(langue);
        }
        public void UpdateOpenTime(int index, float offsettime)
        {
            var item = mGroup.getAt(index);

            if (item == null)
                return;

            item.UpdateTime(offsettime);
        }
        public void SetConfigID(int index, uint configid)
        {
            var item = mGroup.getAt(index);

            if (item == null)
                return;

            item.Config = configid;
        }
    }
}
