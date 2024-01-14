using Google.Protobuf;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using static Logic.Sys_Chat;

namespace Logic
{
    /// <summary> 家族委托 </summary>
    public partial class Sys_Family : SystemModuleBase<Sys_Family>
    {
        public List<GuildConsignInfo> consignInfos = new List<GuildConsignInfo>();
        public List<GuildConsignSelfInfo> consignSelfInfos = new List<GuildConsignSelfInfo>();

        private static readonly Comparison<GuildConsignInfo> s_DefultConsignComparer = DefultConsignComparer;
        private static readonly Comparison<GuildConsignInfo> s_IntensifyComparer = IntensifyComparer;
        private static readonly Comparison<GuildConsignInfo> s_SkillComparer = SkillComparer;
        private static readonly Comparison<GuildConsignSelfInfo> s_ConSelfInfoComparer = ConSelfInfoComparer;

        public uint helpCountToday;
        public int maxHelpCount;

        public uint consignFirstOpen;

        public class ChatMsgData
        {
            public ulong chatID;
            public uint formulaID;

            public ChatMsgData(ulong chatID, uint formulaID)
            {
                this.chatID = chatID;
                this.formulaID = formulaID;
            }
        }

        private Dictionary<uint, List<ChatMsgData>> m_ChatMsg = new Dictionary<uint, List<ChatMsgData>>();

        private void FamilyConsignDataInit()
        {
            Sys_Ini.Instance.Get<IniElement_Int>(1108, out IniElement_Int iniElement_Int);
            maxHelpCount = iniElement_Int.value;
        }

        private void ProcessEvents_Consign(bool toRegister)
        {
            if (toRegister)
            {
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.GetConsignListReq, (ushort)CmdGuild.GetConsignListAck, GetConsignListAck, CmdGuildGetConsignListAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.GetSelfConsignListReq, (ushort)CmdGuild.GetSelfConsignListAck, GetSelfConsignListAck, CmdGuildGetSelfConsignListAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.SeekHelpReq, (ushort)CmdGuild.SeekHelpAck, SeekHelpAck, CmdGuildSeekHelpAck.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.SeekHelpNtf, SeekHelpNtf, CmdGuildSeekHelpNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.PublishConsignReq, (ushort)CmdGuild.PublishConsignNtf, PublishConsignNtf, CmdGuildPublishConsignNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.PublishConsignReq, (ushort)CmdGuild.PublishConsignAck, PublishConsignAck, CmdGuildPublishConsignAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.CancelConsignReq, (ushort)CmdGuild.DeleteConsignNtf, DeleteConsignNtf, CmdGuildDeleteConsignNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.HelpBuildReq, (ushort)CmdGuild.HelpBuildAck, HelpBuildAck, CmdGuildHelpBuildAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.HelpBuildReq, (ushort)CmdGuild.HelpBuildSucNtf, HelpBuildSucNtf, CmdGuildHelpBuildSucNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.ConsignSelfBaseInfoNtf, ConsignSelfBaseInfoNtf, CmdGuildConsignSelfBaseInfoNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.ConsignMergeNtf, ConsignMergeNtf, CmdGuildConsignSelfBaseInfoNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.ConsignSelfUpdateNtf, ConsignSelfUpdateNtf, CmdGuildConsignSelfUpdateNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.LifeSkillEquipNtf, LifeSkillEquipNtf, CmdGuildLifeSkillEquipNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.ReceiveBuildItemReq, (ushort)CmdGuild.ReceiveBuildItemAck, ReceiveBuildItemAck, CmdGuildReceiveBuildItemAck.Parser);

                Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.JoinFamily, OnJoinFamily, true);
                Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.QuitFamily, OnQuitFamily, true);
                Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.CreateFamily, OnJoinFamily, true);
            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.GetConsignListAck, GetConsignListAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.GetSelfConsignListAck, GetSelfConsignListAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.SeekHelpAck, SeekHelpAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.SeekHelpNtf, SeekHelpNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.PublishConsignNtf, PublishConsignNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.PublishConsignAck, PublishConsignAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.DeleteConsignNtf, DeleteConsignNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.HelpBuildAck, HelpBuildAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.HelpBuildSucNtf, HelpBuildSucNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.ConsignSelfBaseInfoNtf, ConsignSelfBaseInfoNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.ConsignMergeNtf, ConsignMergeNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.ConsignSelfUpdateNtf, ConsignSelfUpdateNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.LifeSkillEquipNtf, LifeSkillEquipNtf);

                Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.JoinFamily, OnJoinFamily, false);
                Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.QuitFamily, OnQuitFamily, false);
                Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.CreateFamily, OnJoinFamily, false);
            }
        }

        #region Net

        private void OnLoginReq()
        {
            m_ChatMsg.Clear();
            consignInfos.Clear();
            consignSelfInfos.Clear();
            GetConsignListReq();
            GetSelfConsignListReq();
        }

        private void OnJoinFamily()
        {
            consignInfos.Clear();
            consignSelfInfos.Clear();
            GetConsignListReq();
            GetSelfConsignListReq();
        }

        private void OnQuitFamily()
        {
            if (UIManager.IsOpen(EUIID.UI_FamilyWorkshop))
            {
                UIManager.CloseUI(EUIID.UI_FamilyWorkshop);
            }
            if (UIManager.IsOpen(EUIID.UI_FamilyWorkshop_entrust))
            {
                UIManager.CloseUI(EUIID.UI_FamilyWorkshop_entrust);
            }
        }

        #region 请求

        //请求家族委托 
        public void GetConsignListReq()
        {
            CmdGuildGetConsignListReq cmdGuildGetConsignListReq = new CmdGuildGetConsignListReq();
            NetClient.Instance.SendMessage((ushort)CmdGuild.GetConsignListReq, cmdGuildGetConsignListReq);
        }

        private void GetConsignListAck(NetMsg netMsg)
        {
            CmdGuildGetConsignListAck cmdGuildGetConsignListAck = NetMsgUtil.Deserialize<CmdGuildGetConsignListAck>(CmdGuildGetConsignListAck.Parser, netMsg);
            if (cmdGuildGetConsignListAck.NeedClear)
            {
                consignInfos.Clear();
            }
            for (int i = 0; i < cmdGuildGetConsignListAck.InfoList.Count; i++)
            {
                consignInfos.Add(cmdGuildGetConsignListAck.InfoList[i]);
            }
        }

        //请求自己的委托列表
        public void GetSelfConsignListReq()
        {
            CmdGuildGetSelfConsignListReq cmdGuildGetSelfConsignListReq = new CmdGuildGetSelfConsignListReq();
            NetClient.Instance.SendMessage((ushort)CmdGuild.GetSelfConsignListReq, cmdGuildGetSelfConsignListReq);
        }

        private void GetSelfConsignListAck(NetMsg netMsg)
        {
            consignSelfInfos.Clear();
            CmdGuildGetSelfConsignListAck cmdGuildGetSelfConsignListAck = NetMsgUtil.Deserialize<CmdGuildGetSelfConsignListAck>(CmdGuildGetSelfConsignListAck.Parser, netMsg);
            for (int i = 0; i < cmdGuildGetSelfConsignListAck.InfoList.Count; i++)
            {
                consignSelfInfos.Add(cmdGuildGetSelfConsignListAck.InfoList[i]);
            }
        }

        #endregion

        #region  求助
        public void SeekHelpReq(uint uID)
        {
            CmdGuildSeekHelpReq cmdGuildSeekHelpReq = new CmdGuildSeekHelpReq();
            cmdGuildSeekHelpReq.UId = uID;
            NetClient.Instance.SendMessage((ushort)CmdGuild.SeekHelpReq, cmdGuildSeekHelpReq);
        }

        //只发给自己
        private void SeekHelpAck(NetMsg netMsg)
        {
            CmdGuildSeekHelpAck cmdGuildGetSelfConsignListAck = NetMsgUtil.Deserialize<CmdGuildSeekHelpAck>(CmdGuildSeekHelpAck.Parser, netMsg);
            uint uID = cmdGuildGetSelfConsignListAck.Uid;
            uint tick = cmdGuildGetSelfConsignListAck.Tick;//求助时间

            GuildConsignSelfInfo guildConsignSelfInfo = null;
            for (int i = 0; i < consignSelfInfos.Count; i++)
            {
                if (uID == consignSelfInfos[i].UId)
                {
                    guildConsignSelfInfo = consignSelfInfos[i];
                    break;
                }
            }
            if (guildConsignSelfInfo != null)
            {
                //CSVFormula.Data cSVFormulaData = CSVFormula.Instance.GetConfData(guildConsignSelfInfo.FormulaId);
                //uint ls = cSVFormulaData.type;
                //uint needLv = cSVFormulaData.level_skill;
                //string content = LanguageHelper.GetTextContent(590002028, Sys_Role.Instance.Role.Name.ToStringUtf8(), CSVLanguage.Instance.GetConfData(CSVItem.Instance.GetConfData(cSVFormulaData.view_item).name_id).words,
                //     CSVLanguage.Instance.GetConfData(CSVLifeSkill.Instance.GetConfData(ls).name_id).words, needLv.ToString(), uID.ToString());

                //Sys_Chat.Instance.PushMessage(ChatType.Guild, null, content, EMessageProcess.None, EExtMsgType.Normal);
            }
        }

        //发送给所有人
        private void SeekHelpNtf(NetMsg netMsg)
        {
            CmdGuildSeekHelpNtf ntf = NetMsgUtil.Deserialize<CmdGuildSeekHelpNtf>(CmdGuildSeekHelpNtf.Parser, netMsg);
            string roleName = ntf.RoleName.ToStringUtf8();
            uint formulaId = ntf.FormulaId;

            CSVFormula.Data cSVFormulaData = CSVFormula.Instance.GetConfData(formulaId);
            uint ls = cSVFormulaData.type;
            uint needLv = cSVFormulaData.level_skill;
            string content = LanguageHelper.GetTextContent(590002028, roleName, ntf.RoleId.ToString(), CSVLanguage.Instance.GetConfData(CSVItem.Instance.GetConfData(cSVFormulaData.view_item).name_id).words,
                 CSVLanguage.Instance.GetConfData(CSVLifeSkill.Instance.GetConfData(ls).name_id).words, needLv.ToString(), ntf.UId.ToString());

            ulong chatID = Sys_Chat.Instance.PushMessage(ChatType.Guild, null, content, EMessageProcess.AddUID, EExtMsgType.Normal);
            CachChatMsg(ntf.UId, chatID, formulaId);
        }

        #endregion

        #region 发布委托
        public void PublishConsignReq(uint formulaID, bool useIntensifyBuild, List<uint> inputItemID, bool shareConsign)
        {
            CmdGuildPublishConsignReq cmdGuildPublishConsignReq = new CmdGuildPublishConsignReq();
            cmdGuildPublishConsignReq.FormulaID = formulaID;
            cmdGuildPublishConsignReq.UseIntensifyBuild = useIntensifyBuild;
            for (int i = 0; i < inputItemID.Count; i++)
            {
                cmdGuildPublishConsignReq.InputItemID.Add(inputItemID[i]);
            }
            cmdGuildPublishConsignReq.ShareConsign = shareConsign;
            NetClient.Instance.SendMessage((ushort)CmdGuild.PublishConsignReq, cmdGuildPublishConsignReq);
        }

        private void PublishConsignNtf(NetMsg netMsg)
        {
            CmdGuildPublishConsignNtf ntf = NetMsgUtil.Deserialize<CmdGuildPublishConsignNtf>(CmdGuildPublishConsignNtf.Parser, netMsg);
            GuildConsignInfo guildConsignInfo = ntf.Info;

            consignInfos.Add(guildConsignInfo);
            eventEmitter.Trigger(EEvents.OnPublishSuccess);

            if (ntf.ShareConsign)
            {
                CSVFormula.Data cSVFormulaData = CSVFormula.Instance.GetConfData(guildConsignInfo.FormulaId);
                uint ls = cSVFormulaData.type;
                uint needLv = cSVFormulaData.level_skill;
                string content = LanguageHelper.GetTextContent(590002028, guildConsignInfo.Name.ToStringUtf8(), guildConsignInfo.RoleId.ToString(), CSVLanguage.Instance.GetConfData(CSVItem.Instance.GetConfData(cSVFormulaData.view_item).name_id).words,
                     CSVLanguage.Instance.GetConfData(CSVLifeSkill.Instance.GetConfData(ls).name_id).words, needLv.ToString(), guildConsignInfo.UId.ToString());
                Sys_Chat.Instance.PushMessage(ChatType.Guild, null, content, EMessageProcess.None, EExtMsgType.Normal);
            }
        }

        private void PublishConsignAck(NetMsg netMsg)
        {
            CmdGuildPublishConsignAck cmdGuildPublishConsignAck = NetMsgUtil.Deserialize<CmdGuildPublishConsignAck>(CmdGuildPublishConsignAck.Parser, netMsg);
            GuildConsignSelfInfo guildConsignSelfInfo = cmdGuildPublishConsignAck.Info;

            consignSelfInfos.Add(guildConsignSelfInfo);
            eventEmitter.Trigger(EEvents.OnPublishSuccess);
        }

        #endregion

        #region 取消委托

        //取消委托
        public void CancelConsignReq(uint uID)
        {
            CmdGuildCancelConsignReq cmdGuildCancelConsignReq = new CmdGuildCancelConsignReq();
            cmdGuildCancelConsignReq.UId = uID;
            NetClient.Instance.SendMessage((ushort)CmdGuild.CancelConsignReq, cmdGuildCancelConsignReq);
        }

        //协助 领取 下架 广播
        private void DeleteConsignNtf(NetMsg netMsg)
        {
            CmdGuildDeleteConsignNtf cmdGuildCancelConsignAck = NetMsgUtil.Deserialize<CmdGuildDeleteConsignNtf>(CmdGuildDeleteConsignNtf.Parser, netMsg);

            if (cmdGuildCancelConsignAck.DeleteType == 0)
            {
                for (int i = consignInfos.Count - 1; i >= 0; --i)
                {
                    if (cmdGuildCancelConsignAck.UIdList.Contains(consignInfos[i].UId))
                    {
                        consignInfos.RemoveAt(i);
                    }
                }
                eventEmitter.Trigger<int>(EEvents.OnDeleConsignEntry, 1);

                for (int i = consignSelfInfos.Count - 1; i >= 0; --i)
                {
                    if (cmdGuildCancelConsignAck.UIdList.Contains(consignSelfInfos[i].UId))
                    {
                        consignSelfInfos.RemoveAt(i);
                    }
                }
                eventEmitter.Trigger<int>(EEvents.OnDeleConsignEntry, 2);
            }

            else if (cmdGuildCancelConsignAck.DeleteType == 1)
            {
                for (int i = consignInfos.Count - 1; i >= 0; --i)
                {
                    if (cmdGuildCancelConsignAck.UIdList.Contains(consignInfos[i].UId))
                    {
                        consignInfos.RemoveAt(i);
                    }
                }
                eventEmitter.Trigger<int>(EEvents.OnDeleConsignEntry, 1);
            }
            else if (cmdGuildCancelConsignAck.DeleteType == 2)
            {
                for (int i = consignSelfInfos.Count - 1; i >= 0; --i)
                {
                    if (cmdGuildCancelConsignAck.UIdList.Contains(consignSelfInfos[i].UId))
                    {
                        consignSelfInfos.RemoveAt(i);
                    }
                }
                eventEmitter.Trigger<int>(EEvents.OnDeleConsignEntry, 2);
            }

            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnFamilyConsignRedCondChanged, null);
        }

        #endregion

        #region 协助

        public void HelpBuildReq(uint uID)
        {
            CmdGuildHelpBuildReq cmdGuildHelpBuildReq = new CmdGuildHelpBuildReq();
            cmdGuildHelpBuildReq.UId = uID;
            NetClient.Instance.SendMessage((ushort)CmdGuild.HelpBuildReq, cmdGuildHelpBuildReq);
        }

        //此消息只发给自己
        private void HelpBuildAck(NetMsg netMsg)
        {
            UIManager.CloseUI(EUIID.UI_FamilyWorkshop_Detail);
            CmdGuildHelpBuildAck cmdGuildHelpBuildAck = NetMsgUtil.Deserialize<CmdGuildHelpBuildAck>(CmdGuildHelpBuildAck.Parser, netMsg);
            for (int i = 0; i < cmdGuildHelpBuildAck.ItemList.Count; i++)
            {

            }

            ItemData equip = new ItemData();
            equip.SetData((int)BoxIDEnum.BoxIdNormal, cmdGuildHelpBuildAck.Equip.Uuid, cmdGuildHelpBuildAck.Equip.Id, cmdGuildHelpBuildAck.Equip.Count,
                cmdGuildHelpBuildAck.Equip.Position, cmdGuildHelpBuildAck.Equip.ShowNewIcon, cmdGuildHelpBuildAck.Equip.Bind, cmdGuildHelpBuildAck.Equip.Equipment,
                cmdGuildHelpBuildAck.Equip.Essence, cmdGuildHelpBuildAck.Equip.Marketendtime, null, cmdGuildHelpBuildAck.Equip.Crystal, cmdGuildHelpBuildAck.Equip.Ornament);

            OpenFamilyConsignTipsParm openFamilyConsignTipsParm = new OpenFamilyConsignTipsParm();
            openFamilyConsignTipsParm.formulaId = cmdGuildHelpBuildAck.FormulaId;
            openFamilyConsignTipsParm.roleName = cmdGuildHelpBuildAck.RoleName.ToStringUtf8();
            openFamilyConsignTipsParm.equip = equip;

            UIManager.OpenUI(EUIID.UI_FamilyWorkshop_Tips, false, openFamilyConsignTipsParm);
            helpCountToday = cmdGuildHelpBuildAck.Count;
        }

        //发给所有玩家(包括自己)
        private void HelpBuildSucNtf(NetMsg netMsg)
        {
            CmdGuildHelpBuildSucNtf ntf = NetMsgUtil.Deserialize<CmdGuildHelpBuildSucNtf>(CmdGuildHelpBuildSucNtf.Parser, netMsg);

            uint uID = ntf.UId;
            string helpName = ntf.HelpName.ToStringUtf8();
            string roleName = ntf.RoleName.ToStringUtf8();
            ItemData equip = new ItemData();
            equip.SetData((int)BoxIDEnum.BoxIdNormal, ntf.Equip.Uuid, ntf.Equip.Id, ntf.Equip.Count,
                ntf.Equip.Position, ntf.Equip.ShowNewIcon, ntf.Equip.Bind, ntf.Equip.Equipment,
                ntf.Equip.Essence, ntf.Equip.Marketendtime, null, ntf.Equip.Crystal, ntf.Equip.Ornament);

            string content = LanguageHelper.GetTextContent(590002029, helpName, ntf.HelperId.ToString(), roleName, ntf.RoleId.ToString());
            ChatExtMsg chatExtMsg = new ChatExtMsg();
            ItemCommonData itemCommonData = new ItemCommonData();
            itemCommonData.Uuid = equip.Uuid;
            itemCommonData.Id = equip.cSVItemData.id;
            itemCommonData.Quality = equip.Quality;
            itemCommonData.Equipment = equip.Equip;
            chatExtMsg.Item.Add(itemCommonData);
            Sys_Chat.Instance.PushMessage(ChatType.Guild, null, content, EMessageProcess.None, EExtMsgType.Normal, chatExtMsg);

            if (m_ChatMsg.TryGetValue(uID, out List<ChatMsgData> datas))
            {
                for (int i = 0; i < datas.Count; i++)
                {
                    ulong chatID = datas[i].chatID;
                    uint formulaID = datas[i].formulaID;
                    CSVFormula.Data cSVFormulaData = CSVFormula.Instance.GetConfData(formulaID);
                    uint ls = cSVFormulaData.type;
                    uint needLv = cSVFormulaData.level_skill;

                    string chatContent = LanguageHelper.GetTextContent(590002042, roleName, ntf.RoleId.ToString(),
                        CSVLanguage.Instance.GetConfData(CSVItem.Instance.GetConfData(cSVFormulaData.view_item).name_id).words,
                         CSVLanguage.Instance.GetConfData(CSVLifeSkill.Instance.GetConfData(ls).name_id).words, needLv.ToString(), uID.ToString());

                    Sys_Chat.Instance.SetChatContent(chatID, chatContent);
                }
            }


            if (ntf.RoleName == Sys_Role.Instance.Role.Name)
            {
                RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnFamilyActiveRedPoint, null);
            }
        }

        #endregion

        private void ConsignSelfUpdateNtf(NetMsg netMsg)
        {
            CmdGuildConsignSelfUpdateNtf cmdGuildConsignSelfUpdateNtf = NetMsgUtil.Deserialize<CmdGuildConsignSelfUpdateNtf>(CmdGuildConsignSelfUpdateNtf.Parser, netMsg);
            GuildConsignSelfInfo guildConsignSelfInfo = cmdGuildConsignSelfUpdateNtf.Info;
            for (int i = consignSelfInfos.Count - 1; i >= 0; --i)
            {
                if (guildConsignSelfInfo.UId == consignSelfInfos[i].UId)
                {
                    consignSelfInfos.RemoveAt(i);
                    consignSelfInfos.Add(guildConsignSelfInfo);
                    break;
                }
            }
            eventEmitter.Trigger<int>(EEvents.OnDeleConsignEntry, 2);
            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnFamilyConsignRedCondChanged, null);
        }

        private void LifeSkillEquipNtf(NetMsg netMsg)
        {
            CmdGuildLifeSkillEquipNtf ntf = NetMsgUtil.Deserialize<CmdGuildLifeSkillEquipNtf>(CmdGuildLifeSkillEquipNtf.Parser, netMsg);
            string roleName = ntf.RoleName.ToStringUtf8();
            ItemData equip = new ItemData();
            equip.SetData((int)BoxIDEnum.BoxIdNormal, ntf.Equip.Uuid, ntf.Equip.Id, ntf.Equip.Count,
                ntf.Equip.Position, ntf.Equip.ShowNewIcon, ntf.Equip.Bind, ntf.Equip.Equipment,
                ntf.Equip.Essence, ntf.Equip.Marketendtime, null, ntf.Equip.Crystal, ntf.Equip.Ornament);

            string content = LanguageHelper.GetTextContent(2010172, roleName, ntf.RoleId.ToString());
            ChatExtMsg chatExtMsg = new ChatExtMsg();
            ItemCommonData itemCommonData = new ItemCommonData();
            itemCommonData.Uuid = equip.Uuid;
            itemCommonData.Id = equip.cSVItemData.id;
            itemCommonData.Quality = equip.Quality;
            itemCommonData.Equipment = equip.Equip;
            chatExtMsg.Item.Add(itemCommonData);
            Sys_Chat.Instance.PushMessage(ChatType.Guild, null, content, EMessageProcess.None, EExtMsgType.Normal, chatExtMsg);
        }

        #region 领取

        //领取委托道具
        public void ReceiveBuildItemReq(uint uID)
        {
            CmdGuildReceiveBuildItemReq cmdGuildReceiveBuildItemReq = new CmdGuildReceiveBuildItemReq();
            cmdGuildReceiveBuildItemReq.UId = uID;
            NetClient.Instance.SendMessage((ushort)CmdGuild.ReceiveBuildItemReq, cmdGuildReceiveBuildItemReq);
        }

        private void ReceiveBuildItemAck(NetMsg netMsg)
        {
            CmdGuildReceiveBuildItemAck ack = NetMsgUtil.Deserialize<CmdGuildReceiveBuildItemAck>(CmdGuildReceiveBuildItemAck.Parser, netMsg);
            ulong roleId = ack.HelpRoleId;
            uint lanId = 0;
            uint quality = 0;
            CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(ack.Equip.Id);
            uint typeId = cSVItemData.type_id;
            if (typeId == (int)EItemType.Equipment)
            {
                if (ack.Equip.Equipment != null)
                {
                    quality = (uint)ack.Equip.Equipment.Color;
                }
                else
                {
                    quality = cSVItemData.quality;
                }
            }
            else if (typeId == (int)EItemType.Ornament)
            {
                if (ack.Equip.Ornament != null)
                {
                    quality = Sys_Ornament.Instance.GetQualityByScore(ack.Equip.Id, ack.Equip.Ornament.Score);
                }
                else
                {
                    quality = cSVItemData.quality;
                }
            }
            else
            {
                quality = cSVItemData.quality;
            }
            if (quality == (uint)EQualityType.Orange)
            {
                lanId = 590002039;
            }
            else if (quality == (uint)EQualityType.Purple)
            {
                lanId = 590002040;
            }
            else
            {
                lanId = 590002041;
            }
            string content = LanguageHelper.GetTextContent(lanId, LanguageHelper.GetTextContent(cSVItemData.name_id));
            Sys_Society.Instance.ReqChatSingle(roleId, content, false);
        }

        #endregion

        //登录发送本系统相关的信息  今日已经协助次数
        private void ConsignSelfBaseInfoNtf(NetMsg netMsg)
        {
            CmdGuildConsignSelfBaseInfoNtf cmdGuildConsignSelfBaseInfoNtf = NetMsgUtil.Deserialize<CmdGuildConsignSelfBaseInfoNtf>(CmdGuildConsignSelfBaseInfoNtf.Parser, netMsg);
            helpCountToday = cmdGuildConsignSelfBaseInfoNtf.Count;
        }

        //家族合并(合并之后需要重新请求)
        private void ConsignMergeNtf(NetMsg netMsg)
        {
            CmdGuildConsignMergeNtf cmdGuildConsignSelfBaseInfoNtf = NetMsgUtil.Deserialize<CmdGuildConsignMergeNtf>(CmdGuildConsignMergeNtf.Parser, netMsg);

            GetConsignListReq();
            GetSelfConsignListReq();
        }

        public void GuildSetConsignFirstOpenReq()
        {
            consignFirstOpen = 1;
            CmdGuildSetConsignFirstOpenReq req = new CmdGuildSetConsignFirstOpenReq();
            req.State = 1;
            NetClient.Instance.SendMessage((ushort)CmdGuild.SetConsignFirstOpenReq, req);
        }

        #endregion

        #region Util

        public bool HasConsignReward()
        {
            for (int i = 0; i < consignSelfInfos.Count; i++)
            {
                if (!consignSelfInfos[i].HelperName.IsEmpty)
                {
                    return true;
                }
            }
            return false;
        }

        public void DefultSort()
        {
            consignInfos.Sort(s_DefultConsignComparer);
        }

        public void IntensifySort()
        {
            consignInfos.Sort(s_IntensifyComparer);
        }

        public void SkillSort()
        {
            consignInfos.Sort(s_SkillComparer);
        }

        public void DefultSelfConsignSort()
        {
            consignSelfInfos.Sort(s_ConSelfInfoComparer);
        }

        private static int DefultConsignComparer(GuildConsignInfo lhs, GuildConsignInfo rhs)
        {
            return lhs.EndTick.CompareTo(rhs.EndTick);
        }

        private static int IntensifyComparer(GuildConsignInfo lhs, GuildConsignInfo rhs)
        {
            if (rhs.IntensifyBuild && !lhs.IntensifyBuild)
                return 1;
            else if (!rhs.IntensifyBuild && lhs.IntensifyBuild)
                return -1;
            else
                return 0;
        }

        private static int SkillComparer(GuildConsignInfo lhs, GuildConsignInfo rhs)
        {
            CSVFormula.Data cSVFormulaData_lhs = CSVFormula.Instance.GetConfData(lhs.FormulaId);
            CSVFormula.Data cSVFormulaData_rhs = CSVFormula.Instance.GetConfData(rhs.FormulaId);
            uint skillId_lhs = cSVFormulaData_lhs.type;
            uint skillId_rhs = cSVFormulaData_rhs.type;
            uint lv_lhs = cSVFormulaData_lhs.level_skill;
            uint lv_rhs = cSVFormulaData_rhs.level_skill;

            bool level_lhs = Sys_LivingSkill.Instance.livingSkills[skillId_lhs].Level >= lv_lhs;
            bool level_rhs = Sys_LivingSkill.Instance.livingSkills[skillId_rhs].Level >= lv_rhs;
            if (level_lhs && !level_rhs)
                return -1;
            else if (!level_lhs && level_rhs)
                return 1;
            else
                return lhs.EndTick.CompareTo(rhs.EndTick);
        }


        private static int ConSelfInfoComparer(GuildConsignSelfInfo lhs, GuildConsignSelfInfo rhs)
        {
            bool get_lhs = !lhs.HelperName.IsEmpty;
            bool get_rhs = !rhs.HelperName.IsEmpty;
            if (get_lhs && !get_rhs)
            {
                return -1;
            }
            else if (!get_lhs && get_rhs)
            {
                return 1;
            }
            else
            {
                return lhs.EndTick.CompareTo(rhs.EndTick);
            }
        }

        private void CachChatMsg(uint uID, ulong chatID, uint formulaID)
        {
            if (!m_ChatMsg.TryGetValue(uID, out List<ChatMsgData> chatMsgs) || chatMsgs == null)
            {
                chatMsgs = new List<ChatMsgData>();
                m_ChatMsg.Add(uID, chatMsgs);
            }
            chatMsgs.Add(new ChatMsgData(chatID, formulaID));
        }
        #endregion
    }
}