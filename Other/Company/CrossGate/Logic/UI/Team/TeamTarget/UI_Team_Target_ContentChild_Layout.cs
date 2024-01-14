using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using Logic;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using Framework;

public partial class UI_Team_Target_Layout
{
    #region LeftChildItem
    public class LeftChildItem : ClickItem
    {
        //private CP_ToggleGroup mToggleGroup;

        private Text mDarkText;
        private Text mLightText;
       

        private Button mEditBtn;

        private bool mbEdit = false;

        public string text { set { setText(value); } }
        public bool Edit { get { return mbEdit; } set { mbEdit = value; setEdit(mbEdit); } }

        public uint ID { get; set; }

        public Action<uint, int> OnClickEditAction;

        public ClickItemEvent clickItemEvent = new ClickItemEvent();
        public int Index { get; set; }
        public CP_Toggle Togg { get; set; }
        public override void Load(Transform root)
        {
            mTransform = root;

            Togg = mTransform.GetComponent<CP_Toggle>();

            //mToggleGroup = mTransform.GetComponent<CP_ToggleGroup>();


            mDarkText = mTransform.Find("Btn_Menu_Dark/Text_Menu").GetComponent<Text>();
            mLightText = mTransform.Find("Btn_Menu_Light/Text_Menu").GetComponent<Text>();

            mEditBtn = mTransform.Find("EditBtn").GetComponent<Button>();

            mEditBtn.onClick.AddListener(OnClickEdit);

            Togg.onValueChanged.AddListener(OnClick);

            Edit = false;

        }

        private void setText(string str)
        {
            mDarkText.text = str;
            mLightText.text = str;
        }

        private void setEdit(bool b)
        {
            mEditBtn.gameObject.SetActive(b);
        }
        public override ClickItem Clone()
        {
            return Clone<LeftChildItem>(this);
        }

        private void OnClickEdit()
        {
            OnClickEditAction?.Invoke(ID, Index);
        }


        private void OnClick(bool state)
        {
            if(state)
              clickItemEvent.Invoke(Index);
        }


    }

    #endregion
}
