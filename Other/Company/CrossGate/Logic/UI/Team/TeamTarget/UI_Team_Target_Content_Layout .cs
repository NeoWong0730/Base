using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using Logic;
using System.Collections.Generic;
using UnityEngine.Events;
using System;



public partial class UI_Team_Target_Layout
{
    #region LeftItem
    public class LeftItem : IntClickItem
    {
        public CP_Toggle mItem;

        public Transform mChild;

        public ClickItemGroup<LeftChildItem> mChildItemGroup = new ClickItemGroup<LeftChildItem>();

        private Text mDarkTex;
        private Text mLightTex;

        private Transform mMark;

        private string m_Textstr;
        public string NameText { get { return m_Textstr; } set { m_Textstr = value; SetText(value); } }

        private uint m_TextstrID;
        public uint NameTextID { get { return m_TextstrID; } set { m_TextstrID = value; SetText(value); } }

        private bool m_bHoldOn = false;
       // public bool HoldOn { get { return m_bHoldOn; } set { /*m_bHoldOn = value;*/ SetToggleValue(value); } }

        private bool m_bMark = false;

        public bool Mark { get { return m_bMark; } set { SetMark(value); } }

        public Action<int, int> ClickAction;

        public uint ID { get; set; }
        private void SetText(string str)
        {
            mDarkTex.text = str;
            mLightTex.text = str;
        }

        private void SetText(uint id)
        {
            Table.CSVLanguage.Data data = Table.CSVLanguage.Instance.GetConfData(id);

            string textContent = LanguageHelper.GetTextContent(data);
            // TextStyle textStyle = LanguageHelper.GetTextStyle(data);

            NameText = textContent;
        }

        public void SetToggleValue(bool b,bool sendmsg = false)
        {
            if (mItem != null)
                mItem.SetSelected(b, sendmsg);
        }

        private void SetMark(bool b)
        {
            m_bMark = b;

            mMark.gameObject.SetActive(b);
        }

        private void OnClickChild(int index)
        {
            ClickAction?.Invoke(Index, index);
        }
        private void OnChildAdd(LeftChildItem item)
        {
            if (item != null)
                item.clickItemEvent.AddListener(OnClickChild);
        }
        public override void Load(Transform root)
        {
            mTransform = root;

            mItem = mTransform.Find("ItemBig").GetComponent<CP_Toggle>();

            mChild = mTransform.Find("MenuSmall");

            mDarkTex = mItem.transform.Find("Btn_Menu_Dark/Text_Menu_Dark").GetComponent<Text>();
            mLightTex = mItem.transform.Find("Btn_Menu_Light/Text_Menu_Dark").GetComponent<Text>();

            mMark = mItem.transform.Find("Toggle/Checkmark");//"Toggle/Background/Checkmark"

            Transform childItem = mChild.Find("ItemSmall");

            mChildItemGroup.AddChild(childItem);
            mChildItemGroup.SetAddChildListenter(OnChildAdd);

          //  HoldOn = false;
            Mark = false;
  
            mItem.onValueChanged.AddListener(OnClickToggle);

            SetToggleValue(false,true);
        }



        private void OnClickToggle(bool state)
        {
            if (state && !m_bHoldOn)
                ShowChild();
            else
                HideChild();
        }

        public void ShowChild()
        {
            m_bHoldOn = true;

            mChild.gameObject.SetActive(true);

            if (Mark == false)
            {
                LostFocus();

                //if (mChildItemGroup != null && mChildItemGroup.items.Count > 0)
                //{
                //    mChildItemGroup.items[0].Togg.SetSelected(true,false);
                //}
            }
        }

        public void HideChild()
        {
            m_bHoldOn = false;
            mChild.gameObject.SetActive(false);


        }

        public override ClickItem Clone()
        {
            return Clone<LeftItem>(this);
        }


        private void LostFocus()
        {
            if (mChildItemGroup == null)
                return;

            for (int i = 0; i < mChildItemGroup.items.Count; i++)
            {
                mChildItemGroup.items[i].Togg.SetSelected(false, false);
            }
        }
    }



    #endregion
}
