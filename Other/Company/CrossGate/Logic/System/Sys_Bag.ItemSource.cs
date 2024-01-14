using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using Table;
using Net;
using Packet;
using Lib.Core;
using UnityEngine;
using static Packet.CmdItemOptionalGiftPackReq.Types;
using System;
using static Logic.Sys_Equip;

namespace Logic
{
    public partial class Sys_Bag : SystemModuleBase<Sys_Bag>
    {
        public void ExecuteItemSource(uint itemId, uint sourceId, EUIID ownerTips, EUIID sourceUIID)
        {
            CSVItemSource.Data cSVItemSourceData = CSVItemSource.Instance.GetConfData(sourceId);
            if (cSVItemSourceData != null)
            {
                if (cSVItemSourceData.Type == 1)            //商城
                {
                    UIManager.CloseUI(ownerTips);

                    MallPrama mallPrama = new MallPrama();
                    mallPrama.itemId = GetGoToItemId(itemId, sourceId);
                    mallPrama.mallId = cSVItemSourceData.Parameter[0];
                    mallPrama.shopId = cSVItemSourceData.Parameter[1];
                    EUIID uiId = (EUIID)cSVItemSourceData.UI_id;
                    UIManager.OpenUI(uiId, false, mallPrama);
                }
                else if (cSVItemSourceData.Type == 2)         //日常界面
                {
                    UIManager.CloseUI(ownerTips);
                    if (cSVItemSourceData.Activity_id == 0)
                    {
                        UIManager.OpenUI(EUIID.UI_DailyActivites);
                    }
                    else
                    {
                        UIDailyActivitesParmas uIDailyActivitesParmas = new UIDailyActivitesParmas();
                        uIDailyActivitesParmas.SkipToID = cSVItemSourceData.Activity_id;
                        UIManager.OpenUI(EUIID.UI_DailyActivites, false, uIDailyActivitesParmas);
                    }
                }
                else if (cSVItemSourceData.Type == 3)         //寻路
                {
                    UIManager.CloseUI(ownerTips);
                    UIManager.CloseUI(sourceUIID);
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(cSVItemSourceData.NPC_id);
                }
                else if (cSVItemSourceData.Type == 4)         //晶石界面
                {
                    UIManager.CloseUI(ownerTips);
                    Sys_StoneSkill.Instance.eventEmitter.Trigger(Sys_StoneSkill.EEvents.EnergysparSpecilEvent, itemId);
                }
                else if (cSVItemSourceData.Type == 5)           //道具合成 
                {
                    UIManager.CloseUI(ownerTips);
                    CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(itemId);
                    if (null != cSVItemData)
                    {
                        if ((uint)sourceUIID == cSVItemSourceData.UI_id)//在道具合成本界面
                        {
                            Sys_Bag.Instance.eventEmitter.Trigger(Sys_Bag.EEvents.ComposeSpecilEvent, cSVItemData.composed);
                        }
                        else                                      //打开道具合成界面
                        {
                            UIManager.OpenUI(EUIID.UI_Compose, false, cSVItemData.composed);
                        }
                    }
                }
                else if (cSVItemSourceData.Type == 6)
                {
                    UIManager.CloseUI(ownerTips);
                    uint gotoItem = GetGoToItemId(itemId, sourceId);
                    Sys_Trade.Instance.TradeFind(gotoItem);
                }
                else if (cSVItemSourceData.Type == 7)
                {
                    UIManager.CloseUI(ownerTips);
                    uint lifeskillId = cSVItemSourceData.id - cSVItemSourceData.Type * 1000;
                    if (sourceUIID == EUIID.UI_LifeSkill_Message)
                    {
                        Sys_LivingSkill.Instance.eventEmitter.Trigger<uint, uint>(Sys_LivingSkill.EEvents.OnRefreshLifeSkillMessage, lifeskillId, itemId);
                    }
                    else
                    {
                        LifeSkillOpenParm lifeSkillOpenParm = new LifeSkillOpenParm();
                        lifeSkillOpenParm.skillId = lifeskillId;
                        lifeSkillOpenParm.itemId = itemId;
                        UIManager.OpenUI(EUIID.UI_LifeSkill_Message, false, lifeSkillOpenParm);
                    }
                    if (sourceUIID == EUIID.UI_FamilyWorkshop_entrust)
                    {
                        UIManager.CloseUI(EUIID.UI_FamilyWorkshop_entrust);
                        if (UIManager.IsOpen(EUIID.UI_FamilyWorkshop))
                        {
                            UIManager.CloseUI(EUIID.UI_FamilyWorkshop);
                        }
                        if (UIManager.IsOpen(EUIID.UI_Family))
                        {
                            UIManager.CloseUI(EUIID.UI_Family);
                        }
                    }
                }
                else if (cSVItemSourceData.Type == 8)
                {
                    UIManager.CloseUI(ownerTips);
                    UIManager.OpenUI(EUIID.UI_ClueTaskMain);
                }
                else if (cSVItemSourceData.Type == 9)
                {
                    UIManager.CloseUI(ownerTips);
                    UIManager.CloseUI(sourceUIID, true);
                    UIManager.OpenUI((EUIID)cSVItemSourceData.UI_id);
                }
                else if (cSVItemSourceData.Type == 10)
                {
                    UIManager.CloseUI(ownerTips);
                    MallPrama mallPrama = new MallPrama();
                    mallPrama.itemId = GetGoToItemId(itemId, sourceId);
                    mallPrama.mallId = cSVItemSourceData.Parameter[0];
                    mallPrama.shopId = cSVItemSourceData.Parameter[1];
                    UIManager.OpenUI(EUIID.UI_PointMall, false, mallPrama);
                }
                else if (cSVItemSourceData.Type == 11)
                {
                    UIManager.CloseUI(ownerTips);
                    UIManager.OpenUI(EUIID.UI_Adventure, false, new AdventurePrama { page = cSVItemSourceData.Parameter[0] });
                }
                else if (cSVItemSourceData.Type == 12)
                {
                    UIManager.CloseUI(ownerTips);
                    Sys_Equip.UIEquipPrama data = new Sys_Equip.UIEquipPrama();
                    data.opType = (EquipmentOperations)cSVItemSourceData.Parameter[0];
                    UIManager.OpenUI(EUIID.UI_Equipment, false, data);
                }
                else if (cSVItemSourceData.Type == 13)
                {
                    UIManager.CloseUI(ownerTips);
                    UIManager.CloseUI(sourceUIID);
                    Sys_Mall.Instance.skip2MallFromItemSource = new MallPrama();
                    Sys_Mall.Instance.skip2MallFromItemSource.mallId = cSVItemSourceData.Parameter[0];
                    Sys_Mall.Instance.skip2MallFromItemSource.shopId = cSVItemSourceData.Parameter[1];
                    Sys_Mall.Instance.skip2MallFromItemSource.itemId = itemId;
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(cSVItemSourceData.NPC_id);
                }
                else if (cSVItemSourceData.Type == 14)
                {
                    UIManager.CloseUI(ownerTips);
                    UIManager.OpenUI(EUIID.UI_Fashion_Buy, false, itemId);
                }
                else if (cSVItemSourceData.Type == 15)
                {
                    UIManager.CloseUI(ownerTips);
                    MallPrama param = new MallPrama();
                    param.mallId = 101u;
                    param.isCharge = true;
                    UIManager.OpenUI(EUIID.UI_Mall, false, param);
                }
                else if (cSVItemSourceData.Type == 16)//飘字
                {
                    uint lanId = cSVItemSourceData.Parameter[0];
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(lanId));
                }
                else if (cSVItemSourceData.Type == 17)//累充
                {
                    UIManager.CloseUI(ownerTips);
                    UIManager.OpenUI(EUIID.UI_OperationalActivity, false, (uint)EOperationalActivity.TotalCharge);
                }
                else if (cSVItemSourceData.Type == 18)
                {
                    UIManager.CloseUI(ownerTips);
                    UIManager.OpenUI(EUIID.UI_Knowledge_RecipeCooking, false, itemId);
                }
                else if (cSVItemSourceData.Type == 19)
                {
                    UIManager.CloseUI(ownerTips);
                    UIManager.CloseUI(sourceUIID);
                    UIManager.OpenUI(EUIID.UI_GoddessTrial);
                }
                else if (cSVItemSourceData.Type == 20)
                {
                    UIManager.CloseUI(ownerTips);
                    Sys_Trade.Instance.FindCategory(cSVItemSourceData.Parameter[0]);
                }
                else if (cSVItemSourceData.Type == 999)//单纯打开界面(不带参数)
                {
                    UIManager.CloseUI(ownerTips);
                    UIManager.OpenUI((EUIID)cSVItemSourceData.Parameter[0]);
                }
            }
        }

        private uint GetGoToItemId(uint itemId, uint sourceId)
        {
            var datas = CSVItem.Instance.GetConfData(itemId).Source;
            uint resId = itemId;
            for (int i = 0; i < datas.Count; i++)
            {
                if (datas[i][0] == sourceId)
                {
                    resId = datas[i][1] == 0 ? itemId : datas[i][1];
                    break;
                }
            }
            return resId;
        }
    }
}



