using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Framework;
using Logic.Core;
using Packet;
using UnityEngine.Playables;

namespace Logic
{
    public class UI_LevelGift : UI_OperationalActivityBase
    {
        #region 界面组件
        private List<uint> lstGiftIds;
        private InfinityGrid infinity;
        private Dictionary<GameObject, UI_LevelGiftCellView> CeilGrids = new Dictionary<GameObject, UI_LevelGiftCellView>();
        #endregion
        #region 系统函数
        protected override void Loaded()
        {
        }
        protected override void InitBeforOnShow()
        {
            Parse();
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        public override void Show()
        {
            base.Show();
            UpdateView();
        }
        public override void Hide()
        {
            base.Hide();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateLevelGiftData, UpdateView, toRegister);
        }
        #endregion

        #region function
        private void Parse()
        {
            infinity = transform.Find("Scroll View").gameObject.GetNeedComponent<InfinityGrid>();
            infinity.onCreateCell += OnCreateCell;
            infinity.onCellChange += OnCellChange;

            lstGiftIds = Sys_OperationalActivity.Instance.GetLevelGiftIds();
        }
        private void UpdateView()
        {
            infinity.CellCount = lstGiftIds.Count;
            infinity.ForceRefreshActiveCell();
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_LevelGiftCellView mCell = new UI_LevelGiftCellView();
            mCell.Init(cell.mRootTransform.transform);
            cell.BindUserData(mCell);
            CeilGrids.Add(cell.mRootTransform.gameObject, mCell);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_LevelGiftCellView mCell = cell.mUserData as UI_LevelGiftCellView;
            mCell.UpdateCellView(lstGiftIds[index]);
        }
        #endregion


        public class UI_LevelGiftCellView //: UIComponent
        {
            private Transform transform;
            private uint giftId;
            private Text txtName;
            private Text txtBgLv;
            private Button btnGet;
            private GameObject goBtnGet;
            private GameObject goFinish;
            private GameObject goLock;
            private GameObject goContent;
            private Text txtLock;
            private Dictionary<GameObject, PropItem> dicItems = new Dictionary<GameObject, PropItem>();

            public void Init(Transform _transform)
            {
                transform = _transform;
                txtName = transform.Find("Text").GetComponent<Text>();
                txtBgLv = transform.Find("bg_Text").GetComponent<Text>();
                btnGet = transform.Find("Btn_01").GetComponent<Button>();
                btnGet.onClick.AddListener(OnBtnGetClick);
                goBtnGet = transform.Find("Btn_01").gameObject;
                goFinish = transform.Find("Finish").gameObject;
                goLock = transform.Find("Lock").gameObject;
                txtLock = transform.Find("Lock").GetComponent<Text>();
                goContent = transform.Find("Reward").gameObject;
                InitItems();
            }

            public void UpdateCellView(uint id)
            {
                giftId = id;
                CSVLevelGift.Data giftData = CSVLevelGift.Instance.GetConfData(giftId);
                Sys_OperationalActivity.Instance.DictGifts.TryGetValue(giftId, out LevelGiftUnit giftState);
                bool isCareer = !(Sys_Role.Instance.Role.Career == (uint)ECareerType.None);
                if (giftData != null)
                {
                    txtName.text = LanguageHelper.GetTextContent(giftData.Name);
                    txtBgLv.text = giftData.Level.ToString();
                    txtLock.text = LanguageHelper.GetTextContent(4703, giftData.Level.ToString());
                    List<ItemData> items = Sys_OperationalActivity.Instance.GetLevelGiftItems(giftData.Reward);
                    for (int i = 0; i < goContent.transform.childCount; i++)
                    {
                        if (dicItems.TryGetValue(goContent.transform.GetChild(i).gameObject, out PropItem propItem))
                        {
                            if (i < items.Count)
                            {
                                CSVItem.Data item = CSVItem.Instance.GetConfData(items[i].Id);
                                var itemData = new PropIconLoader.ShowItemData(items[i].Id, items[i].Count, true, false, false, false, false, true, false, true, OnClick, true, false);
                                if (item.type_id == (uint)EItemType.Equipment)
                                {
                                    itemData.SetUnSpawnEquipQuality(giftData.Reward);
                                }
                                propItem.SetData(itemData, EUIID.UI_Family);
                            }
                            else
                            {
                                propItem.SetEmpty();
                            }
                        }
                    }
                }
                if (giftState != null)
                {
                    bool canGet = Sys_Role.Instance.Role.Level >= giftData.Level;
                    bool IsGet = giftState.IsGet;
                    if (canGet)
                    {
                        if (IsGet)
                        {
                            goBtnGet.SetActive(false);
                            goFinish.SetActive(true);
                            goLock.SetActive(false);
                        }
                        else
                        {
                            goBtnGet.SetActive(isCareer);
                            goLock.SetActive(!isCareer);
                            goFinish.SetActive(false);
                            if (!isCareer)
                            {
                                txtLock.text = LanguageHelper.GetTextContent(4717);
                            }
                        }
                    }
                    else
                    {
                        goBtnGet.SetActive(false);
                        goFinish.SetActive(false);
                        goLock.SetActive(true);
                    }
                }
            }
            private void InitItems()
            {
                Transform parent = goContent.transform;
                for (int i = 0; i < parent.childCount; i++)
                {
                    GameObject go = parent.GetChild(i).gameObject;
                    PropItem itemCell = new PropItem();
                    itemCell.BindGameObject(go);
                    dicItems.Add(go, itemCell);
                }
            }
            private void OnBtnGetClick()
            {
                Sys_OperationalActivity.Instance.LevelGiftGetGiftReq(giftId);
                UIManager.HitButton(EUIID.UI_OperationalActivity, giftId.ToString(), EOperationalActivity.LevelGift.ToString());
            }
            private void OnClick(PropItem item)
            {
                ItemData mItemData = new ItemData(0, 0, item.ItemData.id, (uint)item.ItemData.count, 0, false, false, null, null, 0);
                uint typeId = mItemData.cSVItemData.type_id;
                if (typeId == (uint)EItemType.Equipment)
                {
                    mItemData.EquipParam = item.ItemData.equipPara;
                    UIManager.OpenUI(EUIID.UI_Equipment_Preview, false, mItemData);
                }
                else
                {
                    PropMessageParam propParam = new PropMessageParam();
                    propParam.itemData = mItemData;
                    propParam.showBtnCheck = false;
                    propParam.sourceUiId = EUIID.UI_OperationalActivity;
                    UIManager.OpenUI(EUIID.UI_Prop_Message, false, propParam);
                }
            }

        }
    }
}
