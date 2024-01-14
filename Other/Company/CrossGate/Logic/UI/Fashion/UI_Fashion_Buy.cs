using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Framework;
using Lib.Core;
using Table;
using UnityEngine.EventSystems;
using Logic;
using Packet;

public class UI_Fashion_Buy : UIBase
{
    private Toggle m_UnlockToggle;
    private Button m_BuyButton;
    private Button m_CloseButton;
    private CP_ToggleRegistry m_CP_ToggleRegistry;
    private Text m_Cost;
    private Image m_CostIcon;
    private Text m_ItemName;
    private PropItem m_PropItem;
    private CSVItem.Data m_CSVItemData;//默认永久的道具
    private Text m_Limit1;
    private Text m_Limit2;
    private UI_CurrencyTitle m_UI_CurrencyTitle;
    private uint m_PropId;
    private List<uint> itemIds = new List<uint>();

    protected override void OnOpen(object arg)
    {
        m_PropId = (uint)arg;
        m_CSVItemData = CSVItem.Instance.GetConfData(m_PropId);
        UpdateItemIds();
    }

    private void UpdateItemIds()
    {
        itemIds.Clear();
        uint fashionId = m_CSVItemData.fun_value[0];
        CSVFashionClothes.Data cSVFashionClothesData = CSVFashionClothes.Instance.GetConfData(fashionId);
        if (cSVFashionClothesData != null)
        {
            itemIds = cSVFashionClothesData.FashionItem;
        }
        CSVFashionWeapon.Data cSVFashionWeaponData = CSVFashionWeapon.Instance.GetConfData(fashionId);
        if (cSVFashionWeaponData != null)
        {
            itemIds = cSVFashionWeaponData.FashionItem;
        }
        CSVFashionAccessory.Data cSVFashionAccessoryData = CSVFashionAccessory.Instance.GetConfData(fashionId);
        if (cSVFashionAccessoryData != null)
        {
            itemIds = cSVFashionAccessoryData.AccItem;
        }
    }

    protected override void OnLoaded()
    {
        m_UnlockToggle = transform.Find("Animator/View_BuyDetail/Toggle").GetComponent<Toggle>();
        m_BuyButton = transform.Find("Animator/View_BuyDetail/BtnBuy").GetComponent<Button>();
        m_CloseButton = transform.Find("Animator/View_TipsBgNew06/Btn_Close").GetComponent<Button>();
        m_CP_ToggleRegistry = transform.Find("Animator/View_BuyDetail/TabList").GetComponent<CP_ToggleRegistry>();
        m_CostIcon = transform.Find("Animator/View_BuyDetail/Price/Cost_Coin").GetComponent<Image>();
        m_Cost = transform.Find("Animator/View_BuyDetail/Price/Cost_Coin/Text_Cost").GetComponent<Text>();
        m_ItemName = transform.Find("Animator/View_BuyDetail/PropItem/Text_Name").GetComponent<Text>();
        m_Limit1 = transform.Find("Animator/View_BuyDetail/TabList/ListItem/Btn_Dark/Text").GetComponent<Text>();
        m_Limit2 = transform.Find("Animator/View_BuyDetail/TabList/ListItem/Image_Light/Text").GetComponent<Text>();

        m_UI_CurrencyTitle = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
        m_UI_CurrencyTitle.SetData(new List<uint>() { 1 });

        m_PropItem = new PropItem();
        m_PropItem.BindGameObject(transform.Find("Animator/View_BuyDetail/PropItem").gameObject);

        m_CP_ToggleRegistry.onToggleChange += OnToggleChanged;

        m_BuyButton.onClick.AddListener(OnBuyClicked);
        m_CloseButton.onClick.AddListener(OnCloseClicked);
    }

    protected override void OnShow()
    {
        UpdateIcon();
        UpdateInfo();
        m_CP_ToggleRegistry.SwitchTo(2);
    }

    private void OnToggleChanged(int cur, int old)
    {
        if (cur == 1)
        {
            m_PropId = itemIds[1];
        }
        else if (cur == 2)
        {
            m_PropId = itemIds[0];
        }
        m_CSVItemData = CSVItem.Instance.GetConfData(m_PropId);
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        TextHelper.SetText(m_ItemName, m_CSVItemData.name_id);
        PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
         (_id: m_CSVItemData.id,
         _count: 0,
         _bUseQuailty: true,
         _bBind: false,
         _bNew: false,
         _bUnLock: false,
         _bSelected: false,
         _bShowCount: false,
         _bShowBagCount: false,
         _bUseClick: true,
         _onClick: null,
         _bshowBtnNo: false);
        m_PropItem.SetData(new MessageBoxEvt(EUIID.UI_Cooking_Multiple, showItem));
        uint price = CSVDressUnlock.Instance.GetConfData(m_PropId).price_now;
        TextHelper.SetText(m_Cost, price.ToString());
        uint currencyId = CSVDressUnlock.Instance.GetConfData(m_PropId).price_type;
        ImageHelper.SetIcon(m_CostIcon, CSVItem.Instance.GetConfData(currencyId).small_icon_id);
    }

    private void UpdateInfo()
    {
        if (m_CSVItemData.fun_parameter == "fashion")
        {
            uint itemId_limit = itemIds[1];
            CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(itemId_limit);
            uint time = cSVItemData.fun_value[1];
            if (time > 0)
            {
                int day = (int)time / 86400;
                TextHelper.SetText(m_Limit1, 2009571, day.ToString());
                TextHelper.SetText(m_Limit2, 2009571, day.ToString());
            }
        }
    }

    private void OnBuyClicked()
    {
        uint fashionId = m_CSVItemData.fun_value[0];

        DyeScheme dyeScheme = new DyeScheme();
        FashionClothes fashionClothes = Sys_Fashion.Instance.GetFashionClothes(fashionId);
        if (fashionClothes != null)
        {
            for (uint i = 0; i < 4; i++)
            {
                DyeInfo dyeInfo = new DyeInfo();
                dyeInfo.DyeIndex = i;
                dyeInfo.Value = ((Color32)fashionClothes.GetFirstColor((ETintIndex)i)).ToUInt32();
                dyeScheme.DyeInfo.Add(dyeInfo);
            }
        }

        FashionWeapon fashionWeapon = Sys_Fashion.Instance.GetFashionWeapon(fashionId);
        if (fashionWeapon != null)
        {
            for (uint i = 0; i < 1; i++)
            {
                DyeInfo dyeInfo = new DyeInfo();
                dyeInfo.DyeIndex = i;
                dyeInfo.Value = ((Color32)fashionWeapon.GetFirstColor((ETintIndex)i)).ToUInt32();
                dyeScheme.DyeInfo.Add(dyeInfo);
            }
        }

        FashionAccessory fashionAccessory = Sys_Fashion.Instance.GetFashionAcce(fashionId);
        if (fashionAccessory != null)
        {
            for (uint i = 0; i < 1; i++)
            {
                DyeInfo dyeInfo = new DyeInfo();
                dyeInfo.DyeIndex = i;
                dyeInfo.Value = ((Color32)fashionAccessory.GetFirstColor((ETintIndex)i)).ToUInt32();
                dyeScheme.DyeInfo.Add(dyeInfo);
            }
        }

        Sys_Fashion.Instance.BuyPropAndUnlockReq(fashionId, m_PropId, m_UnlockToggle.isOn, dyeScheme);
        UIManager.CloseUI(EUIID.UI_Fashion_Buy);
    }

    private void OnCloseClicked()
    {
        UIManager.CloseUI(EUIID.UI_Fashion_Buy);
    }

    protected override void OnDestroy()
    {
        m_UI_CurrencyTitle.Dispose();
    }
}
