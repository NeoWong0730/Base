using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Packet;
using Table;
using Logic.Core;
using System;

namespace Logic
{
    public class UI_Treasure_List_Top : UIParseCommon
    {
        private Button mBtnArrow;
        private Image mImgArrow;
        private Text mTxtSeleted;
        private GameObject grid;
        private GameObject cellTemplate;
        private List<Toggle> listToggles = new List<Toggle>();

        private Sys_Treasure.ETreasureSortType mSortType = Sys_Treasure.ETreasureSortType.All;
        private IListener mlistener;

        protected override void Parse()
        {
            mBtnArrow = transform.Find("View_Top/View_Select01/Btn_Arrow").GetComponent<Button>();
            mBtnArrow.onClick.AddListener(OnClickArrow);

            mImgArrow = transform.Find("View_Top/View_Select01/Btn_Arrow/Image_Arrow").GetComponent<Image>();
            mTxtSeleted = transform.Find("View_Top/View_Select01/Text").GetComponent<Text>();
            grid = transform.Find("View_Top/View_Select01/Lab_Select").gameObject;
            cellTemplate = transform.Find("View_Top/View_Select01/Lab_Select/SelectNow01").gameObject;

            int childCount = grid.transform.childCount;
            int sortTypeCount = Enum.GetValues(typeof(Sys_Treasure.ETreasureSortType)).Length;
            for (int i = 0; i < sortTypeCount; ++i)
            {
                Toggle toggle = null;
                if (i < childCount)
                {
                    toggle = grid.transform.GetChild(i).GetComponent<Toggle>();
                }
                else
                {
                    GameObject go = Lib.Core.FrameworkTool.CreateGameObject(cellTemplate, grid);
                    toggle = go.GetComponent<Toggle>();
                }
                listToggles.Add(toggle);
                toggle.onValueChanged.AddListener((isOn) =>
                {
                    OnToggleClick(isOn, listToggles.IndexOf(toggle));
                });
                toggle.isOn = false;

                Text txt = toggle.transform.Find("Text").GetComponent<Text>();
                txt.text = GetText((Sys_Treasure.ETreasureSortType)i);
            }

            grid.SetActive(false);
        }

        public override void Show()
        {
            OnSelectedSortType(mSortType);
            listToggles[0].isOn = true;
        }

        public override void Hide()
        {
            grid.gameObject.SetActive(false);
        }

        private void OnClickArrow()
        {
            bool isExpand = grid.activeSelf;

            grid.SetActive(!isExpand);

            float rotateZ = isExpand ? 90f : 0f;
            mImgArrow.rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotateZ);
        }

        private void OnToggleClick(bool isOn, int index)
        {
            if (isOn)
            {
                grid.gameObject.SetActive(false);

                mSortType = (Sys_Treasure.ETreasureSortType)index;
                OnSelectedSortType(mSortType);
                switch (mSortType)
                {
                    case Sys_Treasure.ETreasureSortType.All:
                        Sys_Adventure.Instance.ReportClickEventHitPoint("Treasure_TypeSelect_All");
                        break;
                    case Sys_Treasure.ETreasureSortType.Unlock:
                        Sys_Adventure.Instance.ReportClickEventHitPoint("Treasure_TypeSelect_Unlock");
                        break;
                    case Sys_Treasure.ETreasureSortType.Lock:
                        Sys_Adventure.Instance.ReportClickEventHitPoint("Treasure_TypeSelect_Lock");
                        break;
                }
            }
        }

        public void RegisterListener(IListener listener)
        {
            mlistener = listener;
        }

        public void OnSelectedSortType(Sys_Treasure.ETreasureSortType sortType)
        {
            mlistener?.OnSelectType(sortType);

            //set ui show
            mTxtSeleted.text = GetText(sortType);
            mImgArrow.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        }

        private string GetText(Sys_Treasure.ETreasureSortType type)
        {
            return LanguageHelper.GetTextContent(2009207 + (uint)type);
        }

        public interface IListener
        {
            void OnSelectType(Sys_Treasure.ETreasureSortType sortType);
        }
    }
}
