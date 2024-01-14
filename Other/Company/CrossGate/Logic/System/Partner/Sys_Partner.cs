using Packet;
using Logic.Core;
using System.Collections.Generic;
using System;
using Net;
using Lib.Core;
using Table;

namespace Logic
{
    public partial class Sys_Partner : SystemModuleBase<Sys_Partner>
    {
        private List<uint> listInfoIds = new List<uint>();

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        private List<uint> listOccupationIds = new List<uint>();
        public enum EEvents : int
        {
            OnNewPartnerNotification,
            OnAttrChangeNotification,
            OnFormSelectNotification,
            OnFormRefreshNotification,
            OnFormationSelectedNtf,
            OnRuneDressCallBack,
            OnRuneUnLoadCallBack,
            OnRuneLoadALlCallBack,
            OnRuneComposeCallBack,
            OnRuneDecomposeCallBack,
            
            OnFetterToRuneEquip,
            OnTelPartnerTab,
            
            OnPartnerLevelChanged, // 伙伴等级变化
            OnPartnerUnlock, // 解锁新伙伴
            OnPartnerRuneStatusChanged,
            
            //伙伴夹点刷新
            OnPartnerPointUpdateNtf,
            OnPartnerDistrutePointNtf,
            OnPartnerPointChangeNtf,
            OnPartnerPointSkillNtf,
        }

        private PartnerInfo partnerInfo = null;

        /// <summary>
        /// 伙伴列表，左边索引
        /// </summary>
        public int SelectIndex = 0;
        /// <summary>
        /// 伙伴详情，选中伙伴
        /// </summary>
        public uint SelectPartnerId = 0u;

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPartner.DataNtf, OnPartnerDataNtf, CmdPartnerDataNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPartner.InfoNtf, OnInfoNtf, CmdPartnerInfoNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPartner.ChangeFormationReq, (ushort)CmdPartner.ChangeFormationRes, OnFormationRes, CmdPartnerChangeFormationRes.Parser);
            //EventDispatcher.Instance.AddEventListener((ushort)CmdPartner.InfoReq, (ushort)CmdPartner.InfoRes, OnInfoRes, CmdPartnerInfoRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPartner.UnlockReq, (ushort)CmdPartner.UnlockRes, OnPartnerUnlockRes, CmdPartnerUnlockRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPartner.AddExpReq, (ushort)CmdPartner.AddExpRes, OnAddExpRes, CmdPartnerAddExpRes.Parser);
            // 伙伴符文
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPartner.RuneAddNtf, OnPartnerRuneAddNtf, CmdPartnerRuneAddNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPartner.RuneDressReq, (ushort)CmdPartner.RuneDressRes, OnPartnerRuneDressRes, CmdPartnerRuneDressRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPartner.RuneUnloadReq, (ushort)CmdPartner.RuneUnloadRes, OnPartnerRuneUnloadRes, CmdPartnerRuneUnloadRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPartner.RuneComposeReq, (ushort)CmdPartner.RuneComposeRes, OnPartnerRuneComposeRes, CmdPartnerRuneComposeRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPartner.RuneDecomposeReq, (ushort)CmdPartner.RuneDecomposeRes, OnPartnerRuneDecomposeRes, CmdPartnerRuneDecomposeRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPartner.RuneUnloadAllReq, (ushort)CmdPartner.RuneUnloadAllRes, OnCmdPartnerRuneUnloadAllRes, CmdPartnerRuneUnloadAllRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPartner.CurrentTeamNtf, OnCurFormationNtf, CmdPartnerCurrentTeamNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdPartner.RuneDressOneKeyReq, (ushort)CmdPartner.RuneDressOneKeyRes, OnCmdPartnerRuneLoadAllRes, CmdPartnerRuneDressOneKeyRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPartner.BondChangeNtf, this.OnBondChangeNtf, CmdPartnerBondChangeNtf.Parser);

            Sys_Plan.Instance.eventEmitter.Handle<uint, uint>(Sys_Plan.EEvents.ChangePlan, (playType, index) => {
                if (playType == (uint)Sys_Plan.EPlanType.Partner)
                {
                    //切换阵容
                    CmdPartnerChangeFormationReq req = new CmdPartnerChangeFormationReq();
                    req.CurrentIndex = index;
                    req.FmList.AddRange(partnerInfo.FmList);

                    NetClient.Instance.SendMessage((ushort)CmdPartner.ChangeFormationReq, req);                 
                }
            }, true);

            var occDict = CSVPartnerOccupation.Instance.GetAll();
            listOccupationIds.Clear();
            foreach (var data in occDict)
            {
                listOccupationIds.Add(data.list_occupation);
            }

            RegExchangePoint();
        }

        // 运行时 增量数据
        private void OnPartnerDataNtf(NetMsg msg)
        {
            CmdPartnerDataNtf ntf = NetMsgUtil.Deserialize<CmdPartnerDataNtf>(CmdPartnerDataNtf.Parser, msg);
            
            var remoteParList = ntf.PaList;
            //更新partnerlist
            if (partnerInfo != null && remoteParList != null)
            {
                var localParList = partnerInfo.PaList;
                for (int i = 0; i < remoteParList.Count; ++i) {
                    var remote = remoteParList[i];
                    bool isExist = false;
                    for (int j = 0; j < localParList.Count; ++j)
                    {
                        var local = localParList[j];
                        if (local.InfoId == remote.InfoId)
                        {
                            local.Exp = remote.Exp;

                            var oldLevel = local.Level;
                            if (oldLevel != remote.Level) {
                                local.Level = remote.Level;
                                
                                // 伙伴等级变化
                                OnPartnerLevelChanged(remote, local, oldLevel);
                            }

                            isExist = true;
                            eventEmitter.Trigger(EEvents.OnAttrChangeNotification, remote.InfoId);
                            break;
                        }
                    }

                    // 新解锁伙伴
                    if (!isExist)
                    {
                        localParList.Add(remote);

                        OnPartnerUnlock(remote);
                        
                        eventEmitter.Trigger(EEvents.OnNewPartnerNotification, remote.InfoId);
                        UIManager.OpenUI(EUIID.UI_PartnerGet, false, remote.InfoId);
                    }
                }
            }
        }

        // 登录 全量数据
        // 伙伴等级变化 这里通知
        private void OnInfoNtf(NetMsg msg)
        {
            CmdPartnerInfoNtf ntf = NetMsgUtil.Deserialize<CmdPartnerInfoNtf>(CmdPartnerInfoNtf.Parser, msg);
            var remoteParList = ntf.Pi.PaList;
            // 更新partnerlist
            if (partnerInfo != null)
            {
                var localParList = partnerInfo.PaList;
                bool hasChenged = false;
                for (int i = 0; i < remoteParList.Count; ++i) {
                    var remote = remoteParList[i];
                    for (int j = 0; j < localParList.Count; ++j)
                    {
                        var local = localParList[j];
                        if (local.InfoId == remote.InfoId)
                        {
                            local.Exp = remote.Exp;
                            var oldLevel = local.Level;
                            if (oldLevel != remote.Level) {
                                local.Level = remote.Level;

                                hasChenged = true;
                                // 伙伴等级变化
                                OnPartnerLevelChanged(remote, local, oldLevel);
                            }
                            break;
                        }
                    }
                }

                // 等级变化
                if (hasChenged) {
                    this.eventEmitter.Trigger(EEvents.OnPartnerLevelChanged);
                }
            }
            else {
                // 第一次获取数据
                this.partnerInfo = ntf.Pi;
                this.Correct(ntf.Pi);
            }

            ConstructBondsData();
            BondsDataValuation(ntf.Pi);
            uint point = ntf.Pi != null ? ntf.Pi.TotalPoint : 0;
            BindExchangePoint(point);
        }

        public void PartnerFormationReq()
        {
            ClearSelectState();

            CmdPartnerChangeFormationReq req = new CmdPartnerChangeFormationReq();
            req.CurrentIndex = partnerInfo.CurrentIndex;
            req.FmList.AddRange(partnerInfo.FmList);

            NetClient.Instance.SendMessage((ushort)CmdPartner.ChangeFormationReq, req);
        }

        private void OnFormationRes(NetMsg msg)
        {
            CmdPartnerChangeFormationRes res = NetMsgUtil.Deserialize<CmdPartnerChangeFormationRes>(CmdPartnerChangeFormationRes.Parser, msg);

            partnerInfo.FmList.Clear();
            partnerInfo.FmList.AddRange(res.FmList);
            partnerInfo.CurrentIndex = res.CurrentIndex;

            if (Sys_Plan.Instance.allPlans[(uint)Sys_Plan.EPlanType.Partner].ContainsKey(partnerInfo.CurrentIndex))
            {
                Sys_Plan.Instance.eventEmitter.Trigger<uint, uint>(Sys_Plan.EEvents.OnChangePlanSuccess, (uint)Sys_Plan.EPlanType.Partner, partnerInfo.CurrentIndex);
            }
            else
            {
                Sys_Plan.Instance.eventEmitter.Trigger<uint, uint>(Sys_Plan.EEvents.AddNewPlan, (uint)Sys_Plan.EPlanType.Partner, partnerInfo.CurrentIndex);
            }
        }

        /// <summary>
        /// 同步伙伴阵容
        /// </summary>
        /// <param name="msg"></param>
        private void OnCurFormationNtf(NetMsg msg)
        {
            CmdPartnerCurrentTeamNtf res = NetMsgUtil.Deserialize<CmdPartnerCurrentTeamNtf>(CmdPartnerCurrentTeamNtf.Parser, msg);

            PartnerFormation temp = new PartnerFormation();
            temp.Pa.AddRange(res.Team);

            partnerInfo.FmList[(int)partnerInfo.CurrentIndex] = temp;
        }

        public void PartnerUnlockReq(uint _infoId)
        {
            CmdPartnerUnlockReq req = new CmdPartnerUnlockReq();
            req.InfoId = _infoId;
            NetClient.Instance.SendMessage((ushort)CmdPartner.UnlockReq, req);
        }

        private void OnPartnerUnlockRes(NetMsg msg)
        {
            //CmdPartnerUnlockRes res = NetMsgUtil.Deserialize<CmdPartnerUnlockRes>(CmdPartnerUnlockRes.Parser, msg);
        }

        public void AddExpReq(uint _infoId, uint _itemInfoId, uint _itemNum)
        {
            CmdPartnerAddExpReq req = new CmdPartnerAddExpReq();
            req.InfoId = _infoId;
            req.ItemInfoId = _itemInfoId;
            req.ItemNum = _itemNum;
            NetClient.Instance.SendMessage((ushort)CmdPartner.AddExpReq, req);
        }

        private void OnAddExpRes(NetMsg msg)
        {
            //CmdPartnerChangeFormationRes res = NetMsgUtil.Deserialize<CmdPartnerChangeFormationRes>(CmdPartnerChangeFormationRes.Parser, msg);
        }

        #region Logic
        public List<uint> GetPartnerListInfoIds(uint _typeId, List<uint> _listIds)
        {
            listInfoIds.Clear();

            //Dictionary<uint, CSVPartnerData> dictPartner = CSVPartner.Instance.GetDictData();

            if (_typeId == 0) //all
            {
                //int count = CSVPartner.Instance.Count;
                var datas = CSVPartner.Instance.GetAll();
                for (int i = 0, len = datas.Count; i < len; ++i)
                {
                    var data = datas[i];
                    if ((_listIds.Contains(data.occupation)|| _listIds.Contains(data.belonging)) 
                        && data.list_show)
                    {
                        listInfoIds.Add(data.id);
                    }
                }
            }
            else
            {
                //int count = CSVPartner.Instance.Count;
                var datas = CSVPartner.Instance.GetAll();
                for (int i = 0, len = datas.Count; i < len; ++i)
                {
                    var data = datas[i];
                    if ((_typeId == data.occupation || _typeId == data.belonging) 
                        && data.list_show)
                    {
                        listInfoIds.Add(data.id);
                    }
                }
            }

            //sort
            List<uint> unlockList = new List<uint>();
            List<uint> lockList = new List<uint>();
            for (int i = 0; i < listInfoIds.Count; ++i)
            {
                if (IsUnLock(listInfoIds[i]))
                    unlockList.Add(listInfoIds[i]);
                else
                    lockList.Add(listInfoIds[i]);
            }
            unlockList.Sort();
            lockList.Sort();

            listInfoIds.Clear();
            listInfoIds.AddRange(unlockList);
            listInfoIds.AddRange(lockList);

            return listInfoIds;
        }

        public bool IsUnLock(uint infoId)
        {
            bool lockState = false;
            if (partnerInfo != null)
            {
                for(int i = 0; i < partnerInfo.PaList.Count; ++i)
                {
                    if (partnerInfo.PaList[i].InfoId == infoId)
                    {
                        lockState = true;
                        break;
                    }
                }
            }
            return lockState;
        }

        public Partner GetPartnerInfo(uint infoId)
        {
            if (partnerInfo != null)
            {
                for (int i = 0; i < partnerInfo.PaList.Count; ++i)
                {
                    if (partnerInfo.PaList[i].InfoId == infoId)
                    {
                        return partnerInfo.PaList[i];
                    }
                }
            }

            return null;
        }

        public List<Partner> GetUnlockPartners()
        {
            List<Partner> list = new List<Partner>();
            if (partnerInfo != null)
                list.AddRange(partnerInfo.PaList);

            return list;
        }

        public List<AttributeRow> GetPartnerAttr(uint infoId, uint level, EAttryType attrType, bool isIgnoreBounds = false)
        {
            List<AttributeRow> list = new List<AttributeRow>();
            Dictionary<uint, uint> addAttrs = GetAllAttrs();

            CSVPartnerLevel.Data levelData = CSVPartnerLevel.Instance.GetUniqData(infoId, level);
            if (levelData != null)
            {
                Dictionary<uint, uint> partnerRuneAttrDic = GetPartnerRuneAttrByPartnerId(infoId);
                if(levelData.attribute != null)
                {
                    for (int i = 0; i < levelData.attribute.Count; ++i)
                    {
                        AttributeRow row = new AttributeRow();
                        row.Id = levelData.attribute[i][0];
                        if (partnerRuneAttrDic.ContainsKey(row.Id))
                            row.Value = (int)levelData.attribute[i][1] + (int)partnerRuneAttrDic[row.Id];
                        else
                            row.Value = (int)levelData.attribute[i][1];

                        if (!isIgnoreBounds)
                        {
                            uint addValue = 0;
                            addAttrs.TryGetValue(row.Id, out addValue);
                            row.Value += (int)addValue;
                        }

                        CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(row.Id);
                        if (attrData != null && attrData.attr_show == (uint)attrType)
                        {
                            list.Add(row);
                        }
                    }
                }
            }

            return list;
        }

        public List<uint> GetRuneEquipPartnerList()
        {
            List<uint> tempList = new List<uint>();
            int count = CSVPartner.Instance.Count;
            for (int i = 0; i < count; ++i) {
                uint partnerId = CSVPartner.Instance.GetByIndex(i).id;
                if (IsUnLock(partnerId))
                {
                    tempList.Add(partnerId);
                }
            }
            List<uint> formList = new List<uint>();
            for(int i = 0; i < tempList.Count; ++i)
            {
                if (IsInForm(tempList[i]))
                    formList.Add(tempList[i]);
            }

            for (int i = 0; i < formList.Count; ++i)
                tempList.Remove(formList[i]);

            List<uint> result = new List<uint>();
            result.AddRange(formList);
            result.AddRange(tempList);

            return result;
        }

        public List<uint> GetPartnerReviewList(uint infoId)
        {
            List<uint> tempList = new List<uint>();
            tempList.Add(infoId);

            List<uint> unlock = new List<uint>();
            List<uint> locked = new List<uint>();

            if (listInfoIds.Count <= 0)
            {
                GetPartnerListInfoIds(0, listOccupationIds);
            }
            for (int i = 0; i < listInfoIds.Count; ++i)
            {
                if (listInfoIds[i] != infoId)
                {
                    if (IsUnLock(listInfoIds[i]))
                    {
                        unlock.Add(listInfoIds[i]);
                    }
                    else
                    {
                        locked.Add(listInfoIds[i]);
                    }
                }
            }

            unlock.Sort(OnSortPartner);
            locked.Sort(OnSortPartner);

            tempList.AddRange(unlock);
            tempList.AddRange(locked);

            return tempList;
        }

        private int OnSortPartner(uint id1, uint id2)
        {
            CSVPartner.Data data1 = CSVPartner.Instance.GetConfData(id1);
            CSVPartner.Data data2 = CSVPartner.Instance.GetConfData(id2);

            if (data1 != null && data2 != null)
            {
                return data2.sort.CompareTo(data1.sort);
            }
            return 0;
        }

        public bool CheckPartnerOpenUnlock(List<uint> condition)
        {
            bool canUnlock = true;
            if (condition != null)
            {
                switch (condition[0])
                {
                    case 1:     //任务
                        canUnlock = Sys_Task.Instance.IsSubmited(condition[1]);
                        break;
                    case 2:    //等级
                        canUnlock = Sys_Role.Instance.Role.Level >= condition[1];
                        break;
                }
            }

            return canUnlock;
        }
        /// <summary> 清空缓存的伙伴id列表 </summary>
        public void ClearPartnerlistIds()
        {
            listInfoIds.Clear();
        }
        #endregion
    }
}