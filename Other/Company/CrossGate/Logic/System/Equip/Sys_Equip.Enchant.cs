using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Net;
using Packet;
using Table;

namespace Logic
{
    public partial class Sys_Equip : SystemModuleBase<Sys_Equip>
    {
        private void OnEquipmentEnchantReq(ulong equipUId, ulong runeUUID, uint enchantType)
        {
            CmdItemEnchantEquipmentReq req = new CmdItemEnchantEquipmentReq();
            req.EquipUUID = equipUId;
            req.RuneUUID = runeUUID;
            req.EnchantType = enchantType;
            NetClient.Instance.SendMessage((ushort)CmdItem.EnchantEquipmentReq, req);
        }

        private void OnEquipmentEnchantRes(NetMsg msg)
        {
            //eventEmitter.Trigger(EEvents.OnNotifySmelt);
        }

        public void OnUseRunel(ItemData runeItem)
        {
            CSVTemporary.Data enchantData = CSVTemporary.Instance.GetConfData(runeItem.Id);
            if (enchantData == null)
            {
                DebugUtil.LogErrorFormat("CSVTemporary OnUseRunel_0 找不到 id={0}", runeItem.Id);
                return;
            }

            ItemData enchantEquip = null;

            for (uint i = (uint)EquipmentSlot.EquipSlotNone; i < (uint)EquipmentSlot.EquipSlotMax; ++i)
            {
                ItemData equip = SameEquipment(i);
                if (equip != null)
                {
                    CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(equip.Id);
                    if (equipInfo == null)
                    {
                        DebugUtil.LogErrorFormat("CSVEquipment 找不到 id={0}", equip.Id);
                        continue;
                    }

                    if (enchantData.equip_type.Contains(equipInfo.equipment_type))
                    {
                        enchantEquip = equip;
                        break;
                    }
                }
            }

            if (enchantEquip != null)
            {
                bool isSameType = false;

                CSVTemporary.Data temp = null;
                for (int i = 0; i < enchantEquip.Equip.EnchantAttr.Count; ++i)
                {
                    AttributeElem enchant = enchantEquip.Equip.EnchantAttr[i];
                    if (enchant == null)
                    {
                        //DebugUtil.LogErrorFormat("AttributeElem OnUseRunel_1 找不到 id={0}", enchant.SourceItemID);
                        continue;
                    }

                    if (enchant.EndTime > Sys_Time.Instance.GetServerTime())
                    {
                        temp = CSVTemporary.Instance.GetConfData(enchant.SourceItemID);

                        if (temp == null)
                        {
                            DebugUtil.LogErrorFormat("CSVTemporary OnUseRunel_1 找不到 id={0}", enchant.SourceItemID);
                            continue;
                        }

                        if (temp.type == enchantData.type)
                        {
                            isSameType = true;
                            break;
                        }
                    }
                }

                //附魔的装备等级限制
                CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(enchantEquip.Id);
                if (equipInfo != null && enchantData.equip_lev > equipInfo.equipment_level)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4224));
                    return;
                }

                uint tipId = 4045;
                if (isSameType)
                {
                    if (enchantData.lev == temp.lev)
                    {
                        tipId = 4046;
                    }
                    else if (enchantData.lev < temp.lev)
                    {
                        tipId = 4047;
                    }
                }

                if (tipId == 4046)
                {
                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(tipId);
                    PromptBoxParameter.Instance.SetConfirm(true, () =>
                    {
                        OnEquipmentEnchantReq(enchantEquip.Uuid, runeItem.Uuid, 1);
                    }, 4087);
                    PromptBoxParameter.Instance.SetCancel(true, ()=> {
                        OnEquipmentEnchantReq(enchantEquip.Uuid, runeItem.Uuid, 0);
                    }, 4086);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                }
                else
                {
                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(tipId);
                    PromptBoxParameter.Instance.SetConfirm(true, () =>
                    {
                        OnEquipmentEnchantReq(enchantEquip.Uuid, runeItem.Uuid, 0);
                    });
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                }
            }
            else
            {
                //TODO: 没有合适的装备
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4091));
            }
        }
    }
}

