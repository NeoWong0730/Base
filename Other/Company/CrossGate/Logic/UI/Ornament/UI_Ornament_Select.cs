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
    public class UI_Ornament_Select : UIBase
    {
        readonly int INFINITY_MIN_AMOUNT = 6;
        private uint ornamentId = 0;
        private ulong selectedUuid = 0;

        private Button btnBlank;
        private Button btnAdd;
        private Button btnSelect;
        private TipOrnamentRightCompare itemTips;

        private InfinityGridLayoutGroup infinity;
        private int infinityCount;
        private Dictionary<GameObject, UI_Ornament_Select_CellView> CeilGrids = new Dictionary<GameObject, UI_Ornament_Select_CellView>();
        private List<ItemData> itemList = new List<ItemData>();
        #region 系统函数
        protected override void OnOpen(object arg)
        {
            ornamentId = (uint)arg;
        }
        protected override void OnLoaded()
        {
            Parse();
        }
        protected override void OnShow()
        {
            UpdateView();
        }
        protected override void OnDestroy()
        {
            itemTips.OnDestroy();
        }        
        #endregion

        #region func
        private void Parse()
        {
            btnBlank = transform.Find("Blank").GetComponent<Button>();
            btnBlank.onClick.AddListener(OnBtnCloseClick);
            btnAdd = transform.Find("Animator/Add").GetComponent<Button>();
            btnAdd.onClick.AddListener(OnBtnAddClick);
            btnSelect = transform.Find("Animator/Btn_03").GetComponent<Button>();
            btnSelect.onClick.AddListener(OnBtnSelectClick);

            infinity = transform.Find("Animator/Scroll_View/Grid").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            infinity.minAmount = INFINITY_MIN_AMOUNT;
            infinity.updateChildrenCallback = UpdateChildrenCallback;
            Transform parent = infinity.transform;
            itemList = Sys_Ornament.Instance.GetItemListByOrnamentId(ornamentId);
            int count = itemList.Count < INFINITY_MIN_AMOUNT ? itemList.Count : INFINITY_MIN_AMOUNT;
            FrameworkTool.CreateChildList(parent, count);
            for (int i = 0; i < count; i++)
            {
                GameObject go = parent.GetChild(i).gameObject;
                UI_Ornament_Select_CellView cell = new UI_Ornament_Select_CellView();
                cell.Init(go.transform);
                cell.RegisterAction(UpdateCellSelect);
                CeilGrids.Add(go, cell);
            }
            itemTips = new TipOrnamentRightCompare();
            itemTips.Init(transform.Find("Animator/Tips01"));
            itemTips.gameObject.SetActive(false);
        }
        private void UpdateView()
        {
            infinityCount = itemList.Count;
            infinity.SetAmount(infinityCount);
        }
        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= infinityCount)
                return;
            if (CeilGrids.ContainsKey(trans.gameObject))
            {
                UI_Ornament_Select_CellView cell = CeilGrids[trans.gameObject];
                cell.UpdateCellView(itemList[index]);
            }
        }
        private void UpdateCellSelect(ItemData itemData)
        {
            selectedUuid = itemData.Uuid;
            foreach (var cell in CeilGrids)
            {
                cell.Value.UpdateCellSeleted(itemData.Uuid);
            }
            itemTips.gameObject.SetActive(true);
            itemTips.UpdateItemInfo(itemData);
        }
        #endregion

        #region event
        private void OnBtnSelectClick()
        {
            if (selectedUuid > 0)
            {
                Sys_Ornament.Instance.UpgradeTargetUuid = selectedUuid;
                Sys_Ornament.Instance.eventEmitter.Trigger(Sys_Ornament.EEvents.OnUpgradeTargetSeleted);
                this.CloseSelf();
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000555));//请先选择用于升级的饰品
            }
        }
        private void OnBtnCloseClick()
        {
            this.CloseSelf();
        }
        private void OnBtnAddClick()
        {
            //获取途径
            Sys_Trade.Instance.TradeFind(ornamentId);
        }
        #endregion

        public class UI_Ornament_Select_CellView : UIComponent
        {
            private ItemData itemData;
            private Button btnSelect;
            private Image imgIcon;
            private Image imgQuality;
            private Text txtName;
            private Text txtNum;
            private Text txtLv;
            private Text txtScore;
            private GameObject goSelected;
            private Action<ItemData> actUpdateSelected;

            protected override void Loaded()
            {
                btnSelect = transform.GetComponent<Button>();
                btnSelect.onClick.AddListener(OnBtnSelectClick);
                imgIcon = transform.Find("PropItem/Btn_Item/Image_Icon").GetComponent<Image>();
                imgQuality = transform.Find("PropItem/Btn_Item/Image_Quality").GetComponent<Image>();
                txtName = transform.Find("Text_Name").GetComponent<Text>();
                txtNum = transform.Find("PropItem/Text_Number").GetComponent<Text>();
                txtNum.text = "";
                goSelected = transform.Find("Image_Select").gameObject;
                txtLv = transform.Find("Text_Level").GetComponent<Text>();
                txtScore = transform.Find("Text_Score/Text_Value").GetComponent<Text>();
            }
            public void UpdateCellView(ItemData _itemData)
            {
                itemData = _itemData;
                ImageHelper.SetIcon(imgIcon, itemData.cSVItemData.icon_id);
                imgQuality.gameObject.SetActive(true);
                ImageHelper.GetQualityColor_Frame(imgQuality, (int)itemData.Quality);
                txtName.text = LanguageHelper.GetTextContent(itemData.cSVItemData.name_id);
                txtLv.text = LanguageHelper.GetTextContent(4811, itemData.cSVItemData.lv.ToString());
                txtScore.text = itemData.ornament.Score.ToString();
            }
            public void UpdateCellSeleted(ulong _uuid)
            {
                goSelected.SetActive(_uuid == itemData.Uuid);
            }
            public void RegisterAction(Action<ItemData> act)
            {
                actUpdateSelected = act;
            }
            private void OnBtnSelectClick()
            {
                actUpdateSelected?.Invoke(itemData);
            }
        }
    }
}
