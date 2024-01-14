using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Packet;
using Table;
using Logic.Core;

namespace Logic
{
    public class UI_Treasure_List_Cell : UIParseCommon
    {
        //UI
        private TreasureItem02 mTreasureItem;

        private uint mInfoId;
        private bool mIsDisplay;

        protected override void Parse()
        {
            mTreasureItem = new TreasureItem02();
            mTreasureItem.Bind(gameObject);
            mTreasureItem.transform.GetComponent<Button>().onClick.AddListener(OnClickTreasure);

            mTreasureItem.btn.onClick.AddListener(OnClickAdd);
        }

        public override void Show()
        {

        }

        public override void Hide()
        {

        }

        private void OnClickTreasure()
        {
            UIManager.OpenUI(EUIID.UI_Treasure_Tips, false, mInfoId);
            Sys_Adventure.Instance.ReportClickEventHitPoint("Treasure_IconTips_Open_TreasuresId" + mInfoId);
        }

        private void OnClickAdd()
        {
            Sys_Treasure.Instance.EquipReq(mInfoId);
            Sys_Adventure.Instance.ReportClickEventHitPoint("Treasure_ListCell_Add:" + mInfoId);
        }

        public void UpdateInfo(uint treasureId)
        {
            mInfoId = treasureId;
            mIsDisplay = Sys_Treasure.Instance.IsDisplay(mInfoId);

            CSVTreasures.Data data = CSVTreasures.Instance.GetConfData(mInfoId);

            ImageHelper.SetIcon(mTreasureItem.icon, data.icon_id);
            mTreasureItem.textLevel.text = data.level.ToString();

            mTreasureItem.imgLable.gameObject.SetActive(mIsDisplay);

            bool isUnlock = Sys_Treasure.Instance.IsUnlock(mInfoId);
            bool isFixLevel = Sys_Treasure.Instance.IsFixLevel(data.level);

            mTreasureItem.imgTip.gameObject.SetActive(isUnlock && !isFixLevel);

            mTreasureItem.btn.gameObject.SetActive(isUnlock && !mIsDisplay && isFixLevel);
            mTreasureItem.mask.gameObject.SetActive(!isUnlock);
        }
    }
}
