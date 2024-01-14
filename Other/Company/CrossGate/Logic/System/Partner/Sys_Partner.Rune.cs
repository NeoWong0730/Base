using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;
using UnityEngine;

namespace Logic {
    public enum ERuneType {
        Battle = 0,
        Life = 1,
        Max,
    }

    public partial class Sys_Partner : SystemModuleBase<Sys_Partner> {

        public override void OnLogin()
        {
            queueRune.Clear();
            
            // clear
            partnerInfo = null;
        }
        
        /// <summary>
        /// 限制合成按钮的公共cd
        /// </summary>
        public uint composeTimeCd = 0;

        private void OnPartnerRuneAddNtf(NetMsg msg) {
            CmdPartnerRuneAddNtf ntf = NetMsgUtil.Deserialize<CmdPartnerRuneAddNtf>(CmdPartnerRuneAddNtf.Parser, msg);
            this.PartnerRuneNumCheck(ntf.Id, ntf.Num);
            var item = CSVItem.Instance.GetConfData(ntf.Id);
           
            if (null != item)
            {
                long count = ntf.Num;
                uint quality = item.quality;
                CSVLanguage.Data cSVLanguageData = CSVLanguage.Instance.GetConfData(item.name_id);
                if (null != cSVLanguageData)
                {
                    string Name = LanguageHelper.GetTextContent(item.name_id);
                    string content_Chat = LanguageHelper.GetTextContent(400000001, Constants.gChatColors_Items[quality - 1], Name, count.ToString());
                    Sys_Chat.Instance.PushMessage(ChatType.Person, null, content_Chat, Sys_Chat.EMessageProcess.None);
                    Sys_Hint.Instance.PushContent_GetReward(content_Chat, item.id);
                }
            }
        }

        public void PartnerRuneDressReq(uint partnerId, uint runeId, uint posType, uint pos) {
            CmdPartnerRuneDressReq req = new CmdPartnerRuneDressReq();
            req.PartnerId = partnerId;
            req.RuneId = runeId;
            req.PosType = posType;
            req.Pos = pos;
            NetClient.Instance.SendMessage((ushort)CmdPartner.RuneDressReq, req);
        }

        public void OnPartnerRuneDressRes(NetMsg msg) {
            CmdPartnerRuneDressRes res = NetMsgUtil.Deserialize<CmdPartnerRuneDressRes>(CmdPartnerRuneDressRes.Parser, msg);
            if (this.partnerInfo != null) {
                for (int j = 0; j < this.partnerInfo.PaList.Count; j++) 
                {
                    Partner pa = this.partnerInfo.PaList[j];
                    if (pa.InfoId == res.PartnerId) 
                    {
                        pa.TotalRuneLv = res.TotalRuneLv;
                        pa.TotalGoldRune = res.TotalGoldRune;

                        if (pa.Rune.Count >= 2 && res.PosType < 2) {
                            var preSkillList = PartnerSkillBeRuneForActive(res.PartnerId, (ERuneType)res.PosType);
                            pa.Rune[(int)res.PosType].RuneId[(int)res.Pos] = res.RuneId;
                            for (int i = 0; i < this.partnerInfo.RunePack.Count; i++) {
                                if (this.partnerInfo.RunePack[i].Id == res.RuneId) {
                                    this.partnerInfo.RunePack[i].Num--;
                                    break;
                                }
                            }
                            var currentSkillList = PartnerSkillBeRuneForActive(res.PartnerId, (ERuneType)res.PosType);
                            for (int i = currentSkillList.Count - 1; i >= 0; i--)
                            {
                                if(preSkillList.Contains(currentSkillList[i]))
                                {
                                    currentSkillList.RemoveAt(i);
                                }
                            }
                            //展示新激活---------- currentSkillList 内为新加技能-------------
                            if(currentSkillList.Count > 0)
                            {
                                OnShowSkillsGet(currentSkillList);
                            }
                            break;
                        }
                    }
                }
                if(res.RuneUnloadId != 0)
                {
                    this.PartnerRuneNumCheck(res.RuneUnloadId, 1u);
                }
                this.eventEmitter.Trigger(EEvents.OnRuneDressCallBack, res.Pos);
            }
        }

        private void OnShowSkillsGet(List<uint> skillist)
        {
            UIManager.OpenUI(EUIID.UI_PartnerSkillGet, false, skillist);
        }

        public List<uint> PartnerSkillBeRuneForActive(uint partner, ERuneType runeType)
        {
            List<uint> activeSkill = new List<uint>(2);
            CSVPartnerSkill.Data data = CSVPartnerSkill.Instance.GetConfData(partner);
            if(null != data)
            {
                List<List<uint>> skillParam = null;
                if (runeType == ERuneType.Battle)
                {
                    skillParam = data.Battle_PassiveSkill;
                }
                else if (runeType == ERuneType.Life)
                {
                    skillParam = data.Overall_PassiveSkill;
                }
                if(null != skillParam)
                {
                    for (int i = 0; i < skillParam.Count; i++)
                    {
                        if(IsSkillActive(partner, runeType, (int)skillParam[i][2]))
                        {
                            activeSkill.Add(skillParam[i][0]);
                        }
                    }
                }
            }
            return activeSkill;
        }

        public void PartnerRuneUnloadReq(uint partnerId, uint posType, uint pos) {
            CmdPartnerRuneUnloadReq req = new CmdPartnerRuneUnloadReq();
            req.PartnerId = partnerId;
            req.PosType = posType;
            req.Pos = pos;
            NetClient.Instance.SendMessage((ushort)CmdPartner.RuneUnloadReq, req);
        }

        public void OnPartnerRuneUnloadRes(NetMsg msg) {
            CmdPartnerRuneUnloadRes res = NetMsgUtil.Deserialize<CmdPartnerRuneUnloadRes>(CmdPartnerRuneUnloadRes.Parser, msg);
            if (this.partnerInfo != null) 
            {
                for (int j = 0; j < this.partnerInfo.PaList.Count; j++) 
                {
                    Partner pa = this.partnerInfo.PaList[j];
                    if (pa.InfoId == res.PartnerId) 
                    {
                        pa.TotalRuneLv = res.TotalRuneLv;
                        pa.TotalGoldRune = res.TotalGoldRune;
                        if (pa.Rune.Count >= 2 && res.PosType < 2) 
                        {
                            for (int i = 0; i < res.Pos.Count; i++) 
                            {
                                pa.Rune[(int)res.PosType].RuneId[(int)res.Pos[i]] = 0;
                            }

                            break;
                        }
                    }
                }
                for (int i = 0; i < res.RuneUnloadId.Count; i++) {
                    this.PartnerRuneNumCheck(res.RuneUnloadId[i], 1u);
                }
                if (res.RuneUnloadId.Count > 1) {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2006106));
                }
                this.eventEmitter.Trigger(EEvents.OnRuneUnLoadCallBack);
            }
        }
        
        #region 符文一键装载
        private void FilterCareer(List<UIRuneResultParam> runeList, List<uint> partnerUsedRunes, uint partnerId) {
            if (partnerUsedRunes == null) {
                return;
            }

            var csvPartner = CSVPartner.Instance.GetConfData(partnerId);
            if (csvPartner == null) {
                return;
            }
            
            if (csvPartner.profession == 101) {
                runeList.Sort((l, r) => {
                    var ll = CSVRuneInfo.Instance.GetConfData(l.RuneId);
                    var rr = CSVRuneInfo.Instance.GetConfData(r.RuneId);
                    return (int)rr.profession_101 - (int)ll.profession_101;
                });
            }
            else if (csvPartner.profession == 201) {
                runeList.Sort((l, r) => {
                    var ll = CSVRuneInfo.Instance.GetConfData(l.RuneId);
                    var rr = CSVRuneInfo.Instance.GetConfData(r.RuneId);
                    return (int)rr.profession_201 - (int)ll.profession_201;
                });     
            }
            else if (csvPartner.profession == 301) {
                runeList.Sort((l, r) => {
                    var ll = CSVRuneInfo.Instance.GetConfData(l.RuneId);
                    var rr = CSVRuneInfo.Instance.GetConfData(r.RuneId);
                    return (int)rr.profession_301 - (int)ll.profession_301;
                });    
            }
            else if (csvPartner.profession == 401) {
                runeList.Sort((l, r) => {
                    var ll = CSVRuneInfo.Instance.GetConfData(l.RuneId);
                    var rr = CSVRuneInfo.Instance.GetConfData(r.RuneId);
                    return (int)rr.profession_401 - (int)ll.profession_401;
                });     
            }
            else if (csvPartner.profession == 501) {
                runeList.Sort((l, r) => {
                    var ll = CSVRuneInfo.Instance.GetConfData(l.RuneId);
                    var rr = CSVRuneInfo.Instance.GetConfData(r.RuneId);
                    return (int)rr.profession_501 - (int)ll.profession_501;
                });     
            }
            else if (csvPartner.profession == 601) {
                runeList.Sort((l, r) => {
                    var ll = CSVRuneInfo.Instance.GetConfData(l.RuneId);
                    var rr = CSVRuneInfo.Instance.GetConfData(r.RuneId);
                    return (int)rr.profession_601 - (int)ll.profession_601;
                });
            }
        }
        
        private void FilterUsed(List<UIRuneResultParam> runeList, List<uint> partnerUsedRunes) {
            if (partnerUsedRunes == null) {
                return;
            }

            for (int i = runeList.Count - 1; i >= 0; --i) {
                var one = runeList[i];
                int usedCount = EleCount(partnerUsedRunes, one.RuneId);
                if (usedCount >= one.Count) {
                    runeList.RemoveAt(i);
                }
            }
        }
        
        // list中值为target的个数
        public static int EleCount(List<uint> list, uint target) {
            int count = 0;
            for (int i = 0, length = list.Count; i < length; ++i) {
                if (list[i] == target) {
                    ++count;
                }
            }

            return count;
        }

        // 获取所有的解锁的空槽
        public void GetUnloadedRunes(List<PartnerRunePos> partnerRunePos, uint partnerId, int line, List<CmdPartnerRuneDressOneKeyReq.Types.RunePos> runePoss, List<uint> partnerUsedRunes, bool record = false) {
            Partner partner = this.GetPartnerInfo(partnerId);
            for (int i = 0; i < partnerRunePos[line].RuneId.Count; i++) {
                CSVRuneSlot.Data csvRuneSlot = this.dicLineSlot[line][i];
                uint runeId = partnerRunePos[line].RuneId[i];
                bool isRune = csvRuneSlot.slot_type != 9;

                bool isUnlock = false;
                if (isRune) {
                    isUnlock = partner.Level >= csvRuneSlot.slot_unlocklevel;
                }
                else {
                    List<uint> skill = this.GetRuneSkillData(partnerId, (int)csvRuneSlot.slot_category - 1, (int)csvRuneSlot.slot_sequence);
                    if (skill.Count >= 3) {
                        bool isType = (skill[1] == 1);// 1 额外符文 0 前置镶嵌满即可
                        isUnlock = this.GetRuneSkillUnlock(partnerId, (ERuneType)csvRuneSlot.slot_category - 1, csvRuneSlot.slot_unlocknum);
                        bool isActive = (!isType) || (runeId != 0);
                    }
                }
                
                // 解锁的空槽
                if (isUnlock && runeId == 0u) {
                    var runeList = this.GetRuneSureList(csvRuneSlot.slot_type, csvRuneSlot.slot_level, partnerId);
                    this.FilterUsed(runeList, partnerUsedRunes);
                    this.FilterCareer(runeList, partnerUsedRunes, partnerId);
                    // runeList.Sort((l, r) => {
                    //     return (int)((long)l.RuneId - (long)r.RuneId);
                    // });

                    if (runeList.Count > 0) {
                        runeId = runeList[0].RuneId;
                        var pos = new CmdPartnerRuneDressOneKeyReq.Types.RunePos();
                        pos.RuneId = runeId;
                        pos.Pos = csvRuneSlot.slot_sequence - 1;
                        runePoss.Add(pos);
                        
                        if(record)
                        {
                            // 记录已经使用的符文id
                            partnerUsedRunes.Add(runeId);
                        }
                    }
                }
            }
        }

        public void GetUnloadedRunes(uint partnerId, List<CmdPartnerRuneDressOneKeyReq.Types.RuneList> runes, List<uint> partnerUsedRunes, bool record = false) {
            for (int i = 0, length = (int)ERuneType.Max; i < length; ++i) {
                var one = new CmdPartnerRuneDressOneKeyReq.Types.RuneList();
                one.Type =  (uint) i;
                var posList = new List<CmdPartnerRuneDressOneKeyReq.Types.RunePos>();
                var partnerRunePos = this.GetPartnerRunePosByPartnerId(partnerId);
                this.GetUnloadedRunes(partnerRunePos, partnerId, i, posList, partnerUsedRunes, record);
                one.Runes.AddRange(posList);
                runes.Add(one);
            }
        }

        // 符文 一键装载
        public void PartnerRuneLoadAllReq(uint partnerId) {
            CmdPartnerRuneDressOneKeyReq req = new CmdPartnerRuneDressOneKeyReq();
            req.PartnerId = partnerId;
            List<CmdPartnerRuneDressOneKeyReq.Types.RuneList> ls = new List<CmdPartnerRuneDressOneKeyReq.Types.RuneList>(2);
            List<uint> partnerUsedRunes = new List<uint>();
            this.GetUnloadedRunes(partnerId, ls, partnerUsedRunes, true);
            req.RuneList.AddRange(ls);
            NetClient.Instance.SendMessage((ushort)CmdPartner.RuneDressOneKeyReq, req);
        }

        private void OnCmdPartnerRuneLoadAllRes(NetMsg msg) {
            CmdPartnerRuneDressOneKeyRes res = NetMsgUtil.Deserialize<CmdPartnerRuneDressOneKeyRes>(CmdPartnerRuneDressOneKeyRes.Parser, msg);
            if (this.partnerInfo != null) {
                if (res.Partner != null) {
                    for (int k = 0; k < this.partnerInfo.PaList.Count; k++) {
                        Partner pa = this.partnerInfo.PaList[k];
                        if (pa.InfoId == res.Partner.InfoId) {
                            var preSkillList = PartnerSkillBeRuneForActive(res.Partner.InfoId, 0);
                            preSkillList.AddRange(PartnerSkillBeRuneForActive(res.Partner.InfoId, (ERuneType)1));

                            this.partnerInfo.PaList[k] = res.Partner;

                            var currentSkillList = PartnerSkillBeRuneForActive(res.Partner.InfoId, 0);
                            currentSkillList.AddRange(PartnerSkillBeRuneForActive(res.Partner.InfoId, (ERuneType)1));
                            for (int i = currentSkillList.Count - 1; i >= 0; i--)
                            {
                                if (preSkillList.Contains(currentSkillList[i]))
                                {
                                    currentSkillList.RemoveAt(i);
                                }
                            }
                            //展示新激活---------- currentSkillList 内为新加技能-------------
                            if (currentSkillList.Count > 0)
                            {
                                OnShowSkillsGet(currentSkillList);
                            }
                            break;
                        }
                    }
                }

                // 符文背包
                this.partnerInfo.RunePack.Clear();
                for (int i = 0; i < res.RunePack.Count; i++) {
                    this.partnerInfo.RunePack.Add(res.RunePack[i]);
                }
            }
            
            this.eventEmitter.Trigger(EEvents.OnRuneLoadALlCallBack);
        }
        #endregion
        
        public void PartnerRuneUnloadAllReq(uint partnerId) {
            CmdPartnerRuneUnloadAllReq req = new CmdPartnerRuneUnloadAllReq();
            req.PartnerId = partnerId;
            NetClient.Instance.SendMessage((ushort)CmdPartner.RuneUnloadAllReq, req);
        }

        public void OnCmdPartnerRuneUnloadAllRes(NetMsg msg) {
            CmdPartnerRuneUnloadAllRes res = NetMsgUtil.Deserialize<CmdPartnerRuneUnloadAllRes>(CmdPartnerRuneUnloadAllRes.Parser, msg);
            if (this.partnerInfo != null) {
                for (int k = 0; k < this.partnerInfo.PaList.Count; k++) {
                    Partner pa = this.partnerInfo.PaList[k];
                    if (pa.InfoId == res.PartnerId) {
                        for (int i = 0; i < pa.Rune.Count; i++) {
                            int cout = pa.Rune[i].RuneId.Count;
                            for (int j = 0; j < cout; j++) {
                                pa.Rune[i].RuneId[j] = 0;
                            }
                        }
                        pa.TotalGoldRune = 0u;
                        pa.TotalRuneLv = 0u;
                        break;
                    }
                }
                this.partnerInfo.RunePack.Clear();
                for (int i = 0; i < res.RunePack.Count; i++) {
                    this.partnerInfo.RunePack.Add(res.RunePack[i]);
                }
            }
            this.eventEmitter.Trigger(EEvents.OnRuneUnLoadCallBack);
        }

        public void PartnerRuneComposeReq(uint type, uint num) {
            composeTimeCd = Sys_Time.Instance.GetServerTime();
            CmdPartnerRuneComposeReq req = new CmdPartnerRuneComposeReq();
            req.Type = type;
            req.Num = num;
            NetClient.Instance.SendMessage((ushort)CmdPartner.RuneComposeReq, req);
        }

        private Queue<List<UIRuneResultParam>> queueRune = new Queue<List<UIRuneResultParam>>();
        public void OnPartnerRuneComposeRes(NetMsg msg) {
            CmdPartnerRuneComposeRes res = NetMsgUtil.Deserialize<CmdPartnerRuneComposeRes>(CmdPartnerRuneComposeRes.Parser, msg);
            if (this.partnerInfo != null) {
                for (int i = 0; i < res.RuneList.Count; i++) {
                    PartnerRune paRune = res.RuneList[i];
                    this.PartnerRuneNumCheck(paRune.Id, paRune.Num);
                }
                this.eventEmitter.Trigger(EEvents.OnRuneComposeCallBack);
                List<UIRuneResultParam> list = this.GetRuneBagList(0, 0, data: res.RuneList);
                for (int i = 0; i < list.Count; i++)
                {
                    var item = CSVItem.Instance.GetConfData(list[i].RuneId);

                    string Name = LanguageHelper.GetTextContent(item.name_id);
                    string content = string.Format(LanguageHelper.GetTextContent(400000001), Constants.gChatColors_Item[item.quality - 1], Name, list[i].Count.ToString());
                    Sys_Chat.Instance.PushMessage(ChatType.Person, null, content, Sys_Chat.EMessageProcess.None);
                    Sys_Hint.Instance.PushContent_GetReward(content, item.id);
                }
                if(UIManager.IsOpen(EUIID.UI_Rune_Result))
                {
                    queueRune.Enqueue(list);
                }
                else
                {
                    UIManager.OpenUI(EUIID.UI_Rune_Result, false, list);
                }
            }
        }

        public bool IsNeedShowRuneResult()
        {
            return queueRune.Count > 0;
        }

        public List<UIRuneResultParam> GetRuneResultList()
        {
            if(queueRune.Count > 0)
            {
                return queueRune.Dequeue();
            }
            return null;
        }

        public void PartnerRuneDecomposeReq(List<PartnerRune> partnerRunes) {
            CmdPartnerRuneDecomposeReq req = new CmdPartnerRuneDecomposeReq();
            for (int i = 0; i < partnerRunes.Count; i++) {
                req.Runes.Add(partnerRunes[i]);
            }
            NetClient.Instance.SendMessage((ushort)CmdPartner.RuneDecomposeReq, req);
        }

        public void OnPartnerRuneDecomposeRes(NetMsg msg) {
            CmdPartnerRuneDecomposeRes res = NetMsgUtil.Deserialize<CmdPartnerRuneDecomposeRes>(CmdPartnerRuneDecomposeRes.Parser, msg);
            if (this.partnerInfo != null) {
                this.partnerInfo.RunePack.Clear();
                for (int i = 0; i < res.RuneInfo.Count; i++) {
                    this.partnerInfo.RunePack.Add(res.RuneInfo[i]);
                }
                this.eventEmitter.Trigger(EEvents.OnRuneDecomposeCallBack);
            }
        }

        private void PartnerRuneNumCheck(uint runeId, uint num) {
            bool has = false;
            for (int i = 0; i < this.partnerInfo.RunePack.Count; i++) {
                if (this.partnerInfo.RunePack[i].Id == runeId) {
                    has = true;
                    this.partnerInfo.RunePack[i].Num += num;
                    break;
                }
            }
            if (!has) {
                PartnerRune unlockRune = new PartnerRune();
                unlockRune.Id = runeId;
                unlockRune.Num = num;
                this.partnerInfo.RunePack.Add(unlockRune);
            }

        }

        public List<PartnerRunePos> GetPartnerRunePosByPartnerId(uint partnerId) {
            List<PartnerRunePos> temp = new List<PartnerRunePos>();

            for (int j = 0; j < this.partnerInfo.PaList.Count; j++) {
                Partner partner = this.partnerInfo.PaList[j];
                if (partnerId == partner.InfoId) {
                    for (int i = 0; i < partner.Rune.Count; i++) {
                        temp.Add(partner.Rune[i]);
                    }
                }
            }
            return temp;
        }

        /// <summary>
        /// id > 3000000 为被动技能id ,非属性
        /// </summary>
        /// <param partnerId="伙伴id"></param>
        /// <returns></returns>
        public Dictionary<uint, uint> GetPartnerRuneAttrByPartnerId(uint partnerId) {
            Dictionary<uint, uint> attrDic = new Dictionary<uint, uint>();

            for (int l = 0; l < this.partnerInfo.PaList.Count; l++) {
                Partner partner = this.partnerInfo.PaList[l];

                if (partnerId == partner.InfoId) {
                    for (int i = 0; i < partner.Rune.Count; i++) {
                        for (int j = 0; j < partner.Rune[i].RuneId.Count; j++) {
                            CSVRuneInfo.Data runeInfo = CSVRuneInfo.Instance.GetConfData(partner.Rune[i].RuneId[j]);
                            if (null != runeInfo) {
                                if (runeInfo.rune_attribute.Count >= 2) {
                                    // 防止符文属性配置为多属性
                                    int num = runeInfo.rune_attribute.Count / 2;
                                    for (int k = 0; k < num; k++) {
                                        uint attrId = runeInfo.rune_attribute[k];
                                        uint attrValue = runeInfo.rune_attribute[k + 1];
                                        if (attrDic.ContainsKey(attrId)) {
                                            attrDic[attrId] += attrValue;
                                        }
                                        else {
                                            attrDic.Add(attrId, attrValue);
                                        }
                                    }
                                }
                                if (runeInfo.rune_passiveskillID != 0 && !attrDic.ContainsKey(runeInfo.rune_passiveskillID)) {
                                    attrDic.Add(runeInfo.rune_passiveskillID, 0);
                                }
                            }
                        }
                    }
                }
            }
            return attrDic.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
        }

        public List<uint> GetPartnerRuneSkillByPartnerId(uint partnerId) {
            List<uint> skillIds = new List<uint>();

            for (int l = 0; l < this.partnerInfo.PaList.Count; l++) {
                Partner partner = this.partnerInfo.PaList[l];

                if (partnerId == partner.InfoId) {
                    for (int i = 0; i < partner.Rune.Count; i++) {
                        for (int j = 0; j < partner.Rune[i].RuneId.Count; j++) {
                            CSVRuneInfo.Data runeInfo = CSVRuneInfo.Instance.GetConfData(partner.Rune[i].RuneId[j]);
                            if (null != runeInfo) {
                                if (runeInfo.rune_passiveskillID != 0 && !skillIds.Contains(runeInfo.rune_passiveskillID)) {
                                    skillIds.Add(runeInfo.rune_passiveskillID);
                                }
                            }
                        }
                    }
                }
            }
            return skillIds;
        }

        //通过符文槽筛选分堆
        public List<UIRuneResultParam> GetRuneSureList(uint type, uint level, uint partnerId = 0, Google.Protobuf.Collections.RepeatedField<PartnerRune> data = null) {
            List<UIRuneResultParam> sureList = new List<UIRuneResultParam>();
            data = (data == null && null != this.partnerInfo) ? this.partnerInfo.RunePack : data;
            if (null != data) {
                for (int i = 0; i < data.Count; i++) {
                    uint id = data[i].Id;
                    uint count = data[i].Num;
                    CSVRuneInfo.Data runeInfoData = CSVRuneInfo.Instance.GetConfData(id);
                    if (null != runeInfoData) {
                        bool add = false;
                        if (type == 9 && partnerId == runeInfoData.is_exclusive) {
                            add = true;
                        }
                        else if (type == 0 && level == runeInfoData.rune_lvl && (partnerId == runeInfoData.is_exclusive || 0 == runeInfoData.is_exclusive)) {
                            add = true;
                        }
                        else if (type == runeInfoData.rune_type && level == runeInfoData.rune_lvl) {
                            add = true;
                        }

                        if (add) {
                            uint lasetNum = count % runeInfoData.stack_max;
                            uint stackNum = count / runeInfoData.stack_max;
                            for (uint j = 0; j < stackNum; j++) {
                                UIRuneResultParam lastPa = new UIRuneResultParam();
                                lastPa.RuneId = id;
                                lastPa.Count = runeInfoData.stack_max;
                                sureList.Add(lastPa);
                            }
                            if (lasetNum != 0) {
                                UIRuneResultParam lastPa = new UIRuneResultParam();
                                lastPa.RuneId = id;
                                lastPa.Count = lasetNum;
                                sureList.Add(lastPa);
                            }
                        }
                    }
                    else {
                        //DebugUtil.LogErrorFormat("CSVRuneInfo 表 id = {0} not find", id);
                    }
                }
            }
            return sureList;
        }

        public List<UIRuneResultParam> GetRuneBagList(uint type, uint level, Google.Protobuf.Collections.RepeatedField<PartnerRune> data = null) {
            List<UIRuneResultParam> sureList = new List<UIRuneResultParam>();
            if (null != this.partnerInfo) {
                if (data == null)
                    data = this.partnerInfo.RunePack;
                if (null != data) {
                    for (int i = 0; i < data.Count; i++) {
                        uint id = data[i].Id;
                        uint count = data[i].Num;
                        CSVRuneInfo.Data runeInfoData = CSVRuneInfo.Instance.GetConfData(id);
                        if (null != runeInfoData) {
                            if ((type == runeInfoData.typeID || type == 0) && (runeInfoData.lvlID == level || level == 0)) {
                                uint lasetNum = count % runeInfoData.stack_max;
                                uint stackNum = count / runeInfoData.stack_max;
                                for (uint j = 0; j < stackNum; j++) {
                                    UIRuneResultParam lastPa = new UIRuneResultParam();
                                    lastPa.RuneId = id;
                                    lastPa.Count = runeInfoData.stack_max;
                                    sureList.Add(lastPa);
                                }
                                if (lasetNum != 0) {
                                    UIRuneResultParam lastPa = new UIRuneResultParam();
                                    lastPa.RuneId = id;
                                    lastPa.Count = lasetNum;
                                    sureList.Add(lastPa);
                                }
                            }
                        }
                        else {
                            DebugUtil.LogErrorFormat("CSVRuneInfo 表 id = {0} not find", id);
                        }
                    }
                }
            }
            return sureList;
        }

        public uint GetRuneId(uint partnerId, int line, int index) {
            if (null != this.partnerInfo) {
                for (int j = 0; j < this.partnerInfo.PaList.Count; j++) {
                    Partner partner = this.partnerInfo.PaList[j];
                    if (partnerId == partner.InfoId) {
                        return partner.Rune[line].RuneId[index];
                    }
                }
            }
            return 0;
        }

        public List<uint> GetRuneSkillData(uint partnerId, int line, int pos) {
            CSVPartnerSkill.Data partnerSkill = CSVPartnerSkill.Instance.GetConfData(partnerId);
            if (null != partnerSkill) {
                List<List<uint>> curline = (line == 0 ? partnerSkill.Battle_PassiveSkill : partnerSkill.Overall_PassiveSkill);
                if (curline != null) {
                    for (int i = 0; i < curline.Count; i++) {
                        if (curline[i].Count >= 3) {
                            if (curline[i][2] == pos) {
                                return curline[i];
                            }
                        }
                    }
                }
                else {
                    return new List<uint>();
                }
            }
            return new List<uint>();
        }

        // key:line
        // value:ls
        private Dictionary<int, List<CSVRuneSlot.Data>> _dicLineSlot = null;
        public Dictionary<int, List<CSVRuneSlot.Data>> dicLineSlot
        {
            get
            {
                if (this._dicLineSlot == null)
                {
                    this._dicLineSlot = new Dictionary<int, List<CSVRuneSlot.Data>>();

                    var runeSlotDatas = CSVRuneSlot.Instance.GetAll();
                    for (int i = 0, length = runeSlotDatas.Count; i < length; i++)
                    {
                        CSVRuneSlot.Data slot = runeSlotDatas[i];
                        int lineKey = (int)slot.slot_category - 1;
                        if (this._dicLineSlot.ContainsKey(lineKey))
                        {
                            this._dicLineSlot[lineKey].Add(slot);
                        }
                        else
                        {
                            List<CSVRuneSlot.Data>  temp = new List<CSVRuneSlot.Data>();
                            temp.Add(slot);
                            this._dicLineSlot.Add(lineKey, temp);
                        }
                    }

                    List<int> keyList = new List<int>(this._dicLineSlot.Keys);
                    for (int i = 0, length = keyList.Count; i < length; i++)
                    {
                        List<CSVRuneSlot.Data>  data = this._dicLineSlot[keyList[i]];
                        if (data.Count > 1)
                        {
                            int Comp(CSVRuneSlot.Data a, CSVRuneSlot.Data b)
                            {
                                return (int)a.slot_sequence - (int)b.slot_sequence;
                            }
                            data.Sort(Comp);
                        }
                    }
                }

                return this._dicLineSlot;
            }
        }

        // 技能是否解锁
        public bool GetRuneSkillUnlock(uint partnerId, ERuneType runeType, uint limitNumber) {
            return this.GetEquipedRuneCount(partnerId, runeType) >= limitNumber;
        }

        // 获取已经装配的符文槽个数
        public int GetEquipedRuneCount(uint partnerId, ERuneType runeType, bool includeSkillSlot = false) {
            int line = (int)runeType;
            if (null != this.partnerInfo) {
                for (int i = 0; i < this.partnerInfo.PaList.Count; i++) {
                    Partner partner = this.partnerInfo.PaList[i];
                    if (partnerId == partner.InfoId) {
                        if (partner.Rune.Count >= 2) {
                            int lineCount = partner.Rune[line].RuneId.Count;
                            int count = 0;
                            for (int j = 0; j < lineCount; j++) {
                                if (partner.Rune[line].RuneId[j] != 0) {
                                    if (includeSkillSlot) {
                                        ++count;
                                    }
                                    else {
                                        count += ((this.dicLineSlot[line][j].slot_type != 9) ? 1 : 0);
                                    }
                                }
                            }
                            return count;
                        }
                    }
                }
            }
            return 0;
        }
        public bool GetRuneSkillUnlock(uint partnerId, int line, int pos) {
            if (null != this.partnerInfo) {
                for (int i = 0; i < this.partnerInfo.PaList.Count; i++) {
                    Partner partner = this.partnerInfo.PaList[i];

                    if (partnerId == partner.InfoId) {
                        if (partner.Rune.Count >= 2) {
                            int lineCount = partner.Rune[line].RuneId.Count;
                            for (int j = 0; j < lineCount; j++) {
                                if (j < pos && partner.Rune[line].RuneId[j] == 0) {
                                    return false;
                                }
                                else if (j >= pos) {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        public bool IsSkillActive(uint partnerId, ERuneType runeType, int slotId) {
            bool isActive = false;
            int lineIndex = (int)runeType;
            int slotIndex = slotId - 1;
            List<uint> data = this.GetRuneSkillData(partnerId, lineIndex, slotId);
            if (data.Count >= 3) {
                bool isType = data[1] == 1;// 1 额外符文 0 前置镶嵌满即可
                uint runeId = this.GetRuneId(partnerId, lineIndex, slotIndex);
                // 解锁
                bool isUnlock = this.GetRuneSkillUnlock(partnerId, runeType, this.dicLineSlot[lineIndex][slotIndex].slot_unlocknum);
                // 激活
                isActive = isUnlock && ((!isType) || (isType && runeId != 0));
            }
            return isActive;
        }

        private Color yellowColor;
        private Color redColor;
        private Color blueColor;
        private Color purpleColor;
        public Color SetLineColor(int type) {
            Color color = Color.clear;
            if (type == 0) {  // 黄色
                if (this.yellowColor == Color.clear) {
                    CSVParam.Data par = CSVParam.Instance.GetConfData(911);
                    if (null != par) {
                        string[] strs = par.str_value.Split('|');
                        this.yellowColor = new Color(float.Parse(strs[0]) / 255f, float.Parse(strs[1]) / 255f, float.Parse(strs[2]) / 255f);
                    }
                }
                color = this.yellowColor;

            }
            else if (type == 1) {   // 红色
                if (this.redColor == Color.clear) {
                    CSVParam.Data par = CSVParam.Instance.GetConfData(807);
                    if (null != par) {
                        string[] strs = par.str_value.Split('|');
                        this.redColor = new Color(float.Parse(strs[0]) / 255f, float.Parse(strs[1]) / 255f, float.Parse(strs[2]) / 255f);
                    }
                }
                color = this.redColor;
            }
            else if (type == 2) { // 蓝色
                if (this.blueColor == Color.clear) {
                    CSVParam.Data par = CSVParam.Instance.GetConfData(808);
                    if (null != par) {
                        string[] strs = par.str_value.Split('|');
                        this.blueColor = new Color(float.Parse(strs[0]) / 255f, float.Parse(strs[1]) / 255f, float.Parse(strs[2]) / 255f);
                    }
                }
                color = this.blueColor;
            }
            else if (type == 3) { // 紫色
                if (this.purpleColor == Color.clear) {
                    CSVParam.Data par = CSVParam.Instance.GetConfData(912);
                    if (null != par) {
                        string[] strs = par.str_value.Split('|');
                        this.purpleColor = new Color(float.Parse(strs[0]) / 255f, float.Parse(strs[1]) / 255f, float.Parse(strs[2]) / 255f);
                    }
                }
                color = this.purpleColor;
            }

            return color;
        }

        public uint GetRuneLevelImageId(uint level) {
            uint runeLeveBaseUid = 993214u;
            return runeLeveBaseUid + level;
        }

        #region 空槽红点 // 监听 伙伴等级变化, 某个伙伴解锁

        public enum ELockType {
            Lock = 0,
            UnLock = 1,
            New = 2,
        }

        public void Correct(PartnerInfo remotePartnerInfo) {
            // 预处理红点状态
            for (int i = 0, length = remotePartnerInfo.PaList.Count; i < length; ++i) {
                var partner = remotePartnerInfo.PaList[i];
                for (int j = 0, lengthJ = partner.Rune.Count; j < lengthJ; ++j) {
                    var runeList = partner.Rune[j];
                    
                    runeList.Status.Clear();
                    for (int k = 0, lengthK = runeList.RuneId.Count; k < lengthK; ++k) {
                        ELockType status = ELockType.Lock;
                        if (partner.Level >= dicLineSlot[j][k].slot_unlocklevel) {
                            status = ELockType.UnLock;
                        }

                        runeList.Status.Add((int)status);
                    }
                }
            }
        }
        
        private void OnPartnerUnlock(Partner remotePartner) {
            for (int i = 0, length = remotePartner.Rune.Count; i < length; ++i) {
                var runeList = remotePartner.Rune[i];
                runeList.Status.Clear();
                for (int j = 0, lengthJ = runeList.RuneId.Count; j < lengthJ; ++j) {
                    ELockType status = ELockType.Lock;
                    if (dicLineSlot[i][j].slot_type != 9 && remotePartner.Level >= dicLineSlot[i][j].slot_unlocklevel) {
                        status = ELockType.New;
                    }
                    runeList.Status.Add((int)status);
                }
            }
            
            this.eventEmitter.Trigger<Partner>(EEvents.OnPartnerUnlock, remotePartner);
        }
        
        private void OnPartnerLevelChanged(Partner remotePartner, Partner localPartner, uint oldLevel) {
            for (int i = 0, length = localPartner.Rune.Count; i < length; ++i) {
                for (int j = 0, lengthJ = localPartner.Rune[i].Status.Count; j < lengthJ; ++j) {
                    if (localPartner.Rune[i].Status[j] == (int) ELockType.Lock && localPartner.Level >= dicLineSlot[i][j].slot_unlocklevel &&
                        dicLineSlot[i][j].slot_type != 9) {
                        localPartner.Rune[i].Status[j] = (int) ELockType.New;
                    }
                }
            }
        }

        public void ClearStatus(uint partnerId) {
            Partner pa = GetPartnerInfo(partnerId);
            if (pa != null) {
                for (int i = 0, length = pa.Rune.Count; i < length; ++i) {
                    for (int j = 0, lengthJ = pa.Rune[i].Status.Count; j < lengthJ; ++j) {
                        pa.Rune[i].Status[j] = (int) ELockType.Lock;
                    }
                }
                
                this.eventEmitter.Trigger(EEvents.OnPartnerRuneStatusChanged);
            }
        }
        
        public void ClearStatus(uint partnerId, uint line) {
            Partner pa = GetPartnerInfo(partnerId);
            if (pa != null) {
                for (int i = 0, length = pa.Rune.Count; i < length; ++i) {
                    if (line == i) {
                        for (int j = 0, lengthJ = pa.Rune[i].Status.Count; j < lengthJ; ++j) {
                            pa.Rune[i].Status[j] = (int) ELockType.Lock;
                        }
                        break;
                    }
                }
                
                this.eventEmitter.Trigger(EEvents.OnPartnerRuneStatusChanged);
            }
        }
        
        // line,index 从0开始
        public void ClearStatus(uint partnerId, uint line, int index) {
            Partner pa = GetPartnerInfo(partnerId);
            if (pa != null) {
                for (int i = 0, length = pa.Rune.Count; i < length; ++i) {
                    if (line == i) {
                        pa.Rune[i].Status[index] = (int) ELockType.Lock;
                        break;
                    }
                }
                
                this.eventEmitter.Trigger(EEvents.OnPartnerRuneStatusChanged);
            }
        }
        
        public bool HasNew() {
            if (this.partnerInfo != null) {
                for (int i = 0, length = partnerInfo.PaList.Count; i < length; ++i) {
                    if (this.HasNew(partnerInfo.PaList[i].InfoId)) {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool HasNew(uint partnerId) {
            var p = GetPartnerInfo(partnerId);
            if (p != null) {
                for (int i = 0, length = p.Rune.Count; i < length; ++i) {
                    var runeList = p.Rune[i].Status;
                    for (int j = 0, lengthJ = runeList.Count; j < lengthJ; ++j) {
                        if (runeList[j] == (int)ELockType.New) {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        
        public bool HasNew(uint partnerId, uint line) {
            var p = GetPartnerInfo(partnerId);
            if (p != null) {
                for (int i = 0, length = p.Rune.Count; i < length; ++i) {
                    if(line == i) {
                        var runeList = p.Rune[i].Status;
                        for (int j = 0, lengthJ = runeList.Count; j < lengthJ; ++j) {
                            if (runeList[j] == (int)ELockType.New) {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
        
        public bool HasNew(uint partnerId, uint line, uint index) {
            var p = GetPartnerInfo(partnerId);
            if (p != null) {
                for (int i = 0, length = p.Rune.Count; i < length; ++i) {
                    if(line == i) {
                        var runeList = p.Rune[i].Status;
                        for (int j = 0, lengthJ = runeList.Count; j < lengthJ; ++j) {
                            if (index == j && runeList[j] == (int)ELockType.New) {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private uint changeRuneMinCount = 0;
        private uint ChangeRuneMinCount
        {
            get
            {
                if(changeRuneMinCount == 0)
                {
                    var runeSynthetise = CSVRuneSynthetise.Instance.GetByIndex(0);
                    if(null != runeSynthetise && null != runeSynthetise.synthetise_expend && runeSynthetise.synthetise_expend.Count >= 2)
                    {
                        changeRuneMinCount = runeSynthetise.synthetise_expend[1];
                    }
                }
                return changeRuneMinCount;
            }
        }

        /// <summary>
        /// 足够一次兑换
        /// </summary>
        /// <returns></returns>
        public bool IsEnough()
        {
            var hasCount = Sys_Bag.Instance.GetItemCount((uint)ECurrencyType.Runefragment);
            return hasCount >= ChangeRuneMinCount;
        }

        public List<List<uint>> GetRuneSkillsActive(uint infoId)
        {
            List<List<uint>> skillActives = new List<List<uint>>(4);
            //伙伴技能表,战斗线，生活线，被动技能
            CSVPartnerSkill.Data skillData = CSVPartnerSkill.Instance.GetConfData(infoId);
            if (skillData != null)
            {
                //战斗线被动技能
                if (skillData.Battle_PassiveSkill != null)
                {
                    for (int i = 0; i < skillData.Battle_PassiveSkill.Count; ++i)
                    {
                        var skillId = skillData.Battle_PassiveSkill[i][0];
                        bool isActive = Sys_Partner.Instance.IsSkillActive(infoId, ERuneType.Battle, (int)skillData.Battle_PassiveSkill[i][2]);
                        List<uint> tempList = new List<uint>(2);
                        tempList.Add(skillId);
                        tempList.Add(isActive ? 1u : 0u);
                        skillActives.Add(tempList);
                    }
                }

                //生活线被动技能
                if (skillData.Overall_PassiveSkill != null)
                {
                    for (int i = 0; i < skillData.Overall_PassiveSkill.Count; ++i)
                    {
                        var skillId = skillData.Overall_PassiveSkill[i][0];
                        bool isActive = Sys_Partner.Instance.IsSkillActive(infoId, ERuneType.Life, (int)skillData.Overall_PassiveSkill[i][2]);
                        List<uint> tempList = new List<uint>(2);
                        tempList.Add(skillId);
                        tempList.Add(isActive ? 1u : 0u);
                        skillActives.Add(tempList);
                    }
                }
            }

            //金符文技能
            List<uint> skillIds = Sys_Partner.Instance.GetPartnerRuneSkillByPartnerId(infoId);
            for (int i = 0; i < skillIds.Count; ++i)
            {
                var skillId = skillIds[i];
                List<uint> tempList = new List<uint>(2);
                tempList.Add(skillId);
                tempList.Add(1u);
                skillActives.Add(tempList);
            }

            return skillActives;
        }
        #endregion
    }
}