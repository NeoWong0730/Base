using Packet;
using UnityEngine;
using Logic.Core;
using Logic;
using Lib.AssetLoader;
using System.Collections.Generic;
using UnityEngine.UI;
using Table;
using System.Collections;
using System;
using Lib.Core;
using Net;

public class UI_UseItem : UIBase
{
    private Animator animator;
    private Button btnClose;
    private PropItem propItem;
    private Text textName;
    private Button btnUse;
    private Text textUse;

    private bool isExcute = false;

    private ulong curUId;
    //private ItemData curItem = null;
    private uint slot = 0;
    private uint clearSame = 0;

    private Timer _timer;
    private Timer timerCountDown; //装备穿戴定时器
    private uint levelLimit; //装备穿戴等级限制
    private uint timeCountTotal; //装备穿戴倒计时总时间时间
    private uint timeCountLeft; //装备穿戴倒计时剩余时间

    protected override void OnLoaded()
    {
        animator = transform.Find("Animator").GetComponent<Animator>();
        animator.gameObject.SetActive(false);

        btnClose = transform.Find("Animator/View_BG/Btn_Close").GetComponent<Button>();
        btnClose.onClick.AddListener(OnClickClose);

        propItem = new PropItem();
        propItem.BindGameObject(transform.Find("Animator/PropItem").gameObject);
        textName = transform.Find("Animator/Text_Name").GetComponent<Text>();
        btnUse = transform.Find("Animator/Btn_01").GetComponent<Button>();
        btnUse.onClick.AddListener(OnClickUse);
        textUse = transform.Find("Animator/Btn_01/Text_01").GetComponent<Text>();

        levelLimit = uint.Parse(CSVParam.Instance.GetConfData(237).str_value);
        timeCountTotal = uint.Parse(CSVParam.Instance.GetConfData(238).str_value);
    }

    protected override void OnShow()
    {
        StartTimer();
    }

    protected override void OnHide()
    {
        _timer?.Cancel();
        _timer = null;

        if (isExcute)
        {
            animator.Play("Open", -1, 1);
            //animator.Update(0);
        }
    }

    protected override void OnDestroy()
    {
        StopTimer();        
    }

    protected override void ProcessEvents(bool toRegister)
    {        
        Sys_Equip.Instance.eventEmitter.Handle<ulong>(Sys_Equip.EEvents.OnNotifyQuenching, OnNtfQueching, toRegister);
        Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnNtfEquiped, OnNtfEquiped, toRegister);
        Sys_Bag.Instance.eventEmitter.Handle<ItemData>(Sys_Bag.EEvents.OnNtfDelItem, OnDelItem, toRegister);
        Sys_Bag.Instance.eventEmitter.Handle<ItemData>(Sys_Bag.EEvents.OnNtfDelSameItem, OnDelSameItem, toRegister);
    }

    private void StartTimer()
    {
        _timer?.Cancel();
        _timer = Timer.Register(0.3f, CheckUseItem, null, true);
    }

    private void StopTimer()
    {
        _timer?.Cancel();
        _timer = null;
        timerCountDown?.Cancel();
        timerCountDown = null;
    }

    private void Restart()
    {
        animator.gameObject.SetActive(false);
        isExcute = false;
        StartTimer();
    }

    private void CheckUseItem()
    {
        if (!isExcute)
        {
            ItemData temp = Sys_Bag.Instance.GetUseItem(ref clearSame);
            if (temp != null)
            {
                PopItem(temp);
            }
        }
    }

    private void OnClickEquipment(PropItem propItem)
    {
        EquipTipsData tipData = new EquipTipsData();
        tipData.equip = Sys_Bag.Instance.GetItemDataByUuid(curUId);
        tipData.isCompare = false;

        UIManager.OpenUI(EUIID.UI_TipsEquipment, false, tipData);
    }

    private void OnClickOrnament(PropItem propItem)
    {
        OrnamentTipsData tipData = new OrnamentTipsData();
        tipData.equip = Sys_Bag.Instance.GetItemDataByUuid(curUId);
        tipData.isCompare = false;
        tipData.sourceUiId = EUIID.UI_UseItem;
        UIManager.OpenUI(EUIID.UI_Tips_Ornament, false, tipData);
    }

    private void OnClickClose()
    {
        ItemData curItem = Sys_Bag.Instance.GetItemDataByUuid(curUId);
        if (curItem != null && curItem.cSVItemData.type_id != (uint)EItemType.Equipment)
        {
            clearSame = 1;
        }

        timerCountDown?.Cancel();
        timerCountDown = null;

        Restart();
    }

    private void OnClickUse()
    {
        ItemData curItem = Sys_Bag.Instance.GetItemDataByUuid(curUId);
        if (curItem != null)
        {
            uint typeId = curItem.cSVItemData.type_id;
            if (typeId == (uint)EItemType.Crystal)
            {
                if (curItem.marketendTimer.foreverMarket)
                {
                    Sys_Bag.Instance.EquipCrystalReq(curItem.Uuid);
                }
                else if (curItem.cSVItemData.on_sale)
                {
                    PromptBoxParameter.Instance.Clear();
                    string content1 = CSVLanguage.Instance.GetConfData(680000513).words;
                    PromptBoxParameter.Instance.content = content1;
                    PromptBoxParameter.Instance.SetConfirm(true, () =>
                    {
                        Sys_Bag.Instance.EquipCrystalReq(curItem.Uuid);
                    });
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                }
                else
                {
                    Sys_Bag.Instance.EquipCrystalReq(curItem.Uuid);
                }
            }
            else if (typeId == (uint)EItemType.Equipment)
            {
                timerCountDown?.Cancel();
                timerCountDown = null;
                textUse.text = LanguageHelper.GetTextContent(4027);

                ItemData tempItem = curItem;
                if (!Sys_Equip.Instance.IsEquiped(curItem))
                {
                    uint tempSlot = slot;
                    //替换需要提示
                    ItemData equiped = Sys_Equip.Instance.SameEquipment(tempSlot);
                    if (equiped != null)
                    {
                        if (Sys_Equip.Instance.IsInlayJewel(equiped))
                        {
                            PromptBoxParameter.Instance.OpenPromptBox(4100, 0, () =>
                            {
                                //不满足条件，弹提示
                                if (!Sys_Equip.Instance.CanReplaceEquipJewels(equiped, tempItem))
                                {
                                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4180));
                                }
                                else
                                {
                                    Sys_Equip.Instance.OnEquipReq(tempSlot, tempItem, true);
                                }

                            }, () =>
                            {
                                Sys_Equip.Instance.OnEquipReq(tempSlot, tempItem);
                            });
                        }
                        else
                        {
                            Sys_Equip.Instance.OnEquipReq(tempSlot, tempItem);
                        }
                    }
                    else
                    {
                        Sys_Equip.Instance.OnEquipReq(tempSlot, curItem);
                    }
                }
            }
            else if (typeId == (int)EItemType.Ornament)
            {
                CSVOrnamentsUpgrade.Data ornament = CSVOrnamentsUpgrade.Instance.GetConfData(curItem.Id);
                if (Sys_Bag.Instance.GetItemDataByUuid(curItem.Uuid) != null)
                {
                    Sys_Ornament.Instance.OrnamentFitReq(ornament.type, curItem.Uuid);
                }
            }
            else if (curItem.cSVItemData.type_id != (uint)EItemType.Equipment)
            {
                clearSame = 0;
                if (curItem.cSVItemData.batch_use > 0 && Sys_Bag.Instance.GetItemCount(curItem.cSVItemData.id) > 1)
                {
                    UIManager.OpenUI(EUIID.UI_BatchUse_Box, false, curItem);
                }
                else
                {
                    Sys_Bag.Instance.UseItem(curItem);
                }
            }
        }

        Restart();
    }

    private void PopItem(ItemData item)
    {
        curUId = item.Uuid;
        isExcute = true;
        StopTimer();

        if (item == null)
        {
            Restart();
            return;
        }
        uint typeId = item.cSVItemData.type_id;
        if (typeId == (uint)EItemType.Crystal)
        {
            if (!Sys_ElementalCrystal.Instance.bEquiped)
            {
                ShowItem(item);
                textUse.text = LanguageHelper.GetTextContent(4027);
            }
            else
            {
                ItemData equipCrystal = Sys_ElementalCrystal.Instance.GetEquipedCrystal();
                if (equipCrystal.crystal.Durability == 0)
                {
                    ShowItem(item);
                    textUse.text = LanguageHelper.GetTextContent(4027);
                }
                else
                {
                    Restart();
                }
            }
        }
        else if (typeId == (int)EItemType.Ornament)
        {
            if (Sys_Ornament.Instance.CheckNeedUseItem(item))
            {
                ShowItem(item);
                animator.gameObject.SetActive(true);

                PropIconLoader.ShowItemData tempItem = new PropIconLoader.ShowItemData(item.Id, 1, true, false, false, false, false, false, false, true, OnClickOrnament, false, false);
                tempItem.SetQuality(item.Quality);
                propItem.SetData(new MessageBoxEvt(EUIID.UI_UseItem, tempItem));
                textName.text = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(item.Id).name_id);
                textUse.text = LanguageHelper.GetTextContent(4027);
            }
            else
            {
                Restart();
            }
        }
        else if (typeId != (uint)EItemType.Equipment)
        {
            ShowItem(item);
            textUse.text = LanguageHelper.GetTextContent(2007408);
        }
        else
        {
            bool replace = false;
            if (Sys_Equip.Instance.IsTipEquied(item, ref slot, ref replace))
            {
                animator.gameObject.SetActive(true);

                PropIconLoader.ShowItemData tempItem = new PropIconLoader.ShowItemData(item.Id, 1, true, false, false, false, false, false, false, true, OnClickEquipment, false, false);
                tempItem.SetQuality(item.Quality);
                propItem.SetData(new MessageBoxEvt(EUIID.UI_UseItem, tempItem));
                textName.text = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(item.Id).name_id);
                textUse.text = LanguageHelper.GetTextContent(4027);

                //穿戴提示加倒计时
                bool isNeedCountDown = Sys_Role.Instance.Role.Level <= levelLimit;
                if (isNeedCountDown)
                {
                    string strTxt = LanguageHelper.GetTextContent(4027);
                    timeCountLeft = timeCountTotal;
                    timerCountDown?.Cancel();
                    timerCountDown = Timer.Register(1f, () =>
                    {
                        timeCountLeft--;
                        textUse.text = string.Format("{0}({1})", strTxt, timeCountLeft.ToString());
                        if (timeCountLeft <= 0)
                        {
                            timerCountDown?.Cancel();
                            OnClickUse();
                        }

                    }, null, true);

                    textUse.text = string.Format("{0}({1})", strTxt, timeCountLeft.ToString());
                }
            }
            else
            {
                Restart();
            }
        }
    }

    private void ShowItem(ItemData item)
    {
        animator.gameObject.SetActive(true);
        animator.Play("Open", 0, 0f);

        PropIconLoader.ShowItemData tempItem = new PropIconLoader.ShowItemData(item.Id, 1, true, false, false, false, false, false, false, true, null, false, true);
        tempItem.SetQuality(item.Quality);
        propItem.SetData(new MessageBoxEvt(EUIID.UI_UseItem, tempItem));
        textName.text = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(item.Id).name_id);
    }

    #region ntf
    private void OnNtfQueching(ulong UId)
    {
        if (isExcute)
        {
            if (Sys_Equip.Instance.IsSelectedEquipment(curUId))
            {
                Restart();
            }
        }
    }

    private void OnNtfEquiped()
    {
        if (isExcute)
        {
            ItemData curItem = Sys_Bag.Instance.GetItemDataByUuid(curUId);
            PopItem(curItem);
        }
    }


    private void OnDelItem(ItemData itemdata)
    {
        if (isExcute && itemdata != null)
        {
            if (curUId == itemdata.Uuid)
            {
                Restart();
            }
        }
    }

    private void OnDelSameItem(ItemData itemdata)
    {
        if (isExcute && itemdata != null)
        {
            if (curUId == itemdata.Uuid)
            {
                clearSame = 1;
                Restart();
            }
        }
    }

    #endregion
}
