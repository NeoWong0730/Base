using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Packet;
using Table;
using Logic.Core;

namespace Logic
{
    public class UI_Treasure_Display : UIParseCommon
    {
        //UI
        private Image imgBadge;
        private Text mTextLv;
        private Slider mSlider;
        private Text mTextPercent;

        private InfinityGridLayoutGroup gridGroup;
        private Dictionary<GameObject, UI_Treasure_Display_Cell> dictCells = new Dictionary<GameObject, UI_Treasure_Display_Cell>();
        private int visualGridCount;

        private List<uint> listIds = new List<uint>();

        protected override void Parse()
        {
            imgBadge = transform.Find("Image_Badge").GetComponent<Image>();
            mTextLv = transform.Find("Text_Number").GetComponent<Text>();
            mSlider = transform.Find("Slider_Hp").GetComponent<Slider>();
            mTextPercent = transform.Find("Slider_Hp/Text_Percent").GetComponent<Text>();

            gridGroup = transform.Find("Collect_Object/Scroll_View/Viewport").GetComponent<InfinityGridLayoutGroup>();
            gridGroup.minAmount = 7;
            gridGroup.updateChildrenCallback += UpdateChildrenCallback;

            for (int i = 0; i < gridGroup.transform.childCount; ++i)
            {
                Transform trans = gridGroup.transform.GetChild(i);
                UI_Treasure_Display_Cell cell = new UI_Treasure_Display_Cell();
                cell.Init(trans);
                dictCells.Add(trans.gameObject, cell);
            }
        }

        public override void Show()
        {
            UpdateInfo();
        }

        public override void Hide()
        {
            
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= visualGridCount)
                return;

            if (dictCells.ContainsKey(trans.gameObject))
            {
                UI_Treasure_Display_Cell cell = dictCells[trans.gameObject];
                cell.UpdateInfo(listIds[index]);
            }
        }

        public void UpdateInfo()
        {
            //treasure level
            uint level = Sys_Treasure.Instance.Level;
            uint exp = Sys_Treasure.Instance.Exp;
            CSVTreasuresLevel.Data levelData = CSVTreasuresLevel.Instance.GetConfData(level);
            CSVTreasuresLevel.Data nextLevelData = CSVTreasuresLevel.Instance.GetConfData(level + 1);

            ImageHelper.SetIcon(imgBadge, levelData.bg);
            mTextLv.text = LanguageHelper.GetTextContent(2009211u, level.ToString());

            if (nextLevelData != null)
            {
                float sliderPercent = exp / (nextLevelData.upgrade_exp * 1.0f);
                mSlider.value = sliderPercent;
                mTextPercent.text = string.Format("{0}/{1}", exp.ToString(), nextLevelData.upgrade_exp.ToString());
            }
            else
            {
                mSlider.value = 1.0f;
                mTextPercent.text = "Max";
            }

            listIds = Sys_Treasure.Instance.GetSlotList();
            visualGridCount = listIds.Count;
            gridGroup.SetAmount(visualGridCount);
        }
    }
}
