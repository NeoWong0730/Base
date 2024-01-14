using Packet;
using Logic.Core;
using System.Collections.Generic;
using System;
using Net;
using Lib.Core;
using Table;

namespace Logic
{
    public partial class Sys_Treasure : SystemModuleBase<Sys_Treasure>
    {
        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public uint Level {
            get {
                return mTreasureData.Level;
            }
        }

        public uint Exp {
            get {
                return mTreasureData.Exp;
            }
        }

        public enum EEvents : int
        {
            OnRefreshNtf,
            OnUnlockNotify,
        }

        public enum ETreasureSortType
        {
            All = 0,
            Unlock = 1,
            Lock = 2,
        }

        private TreasureData mTreasureData;

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTreasure.DataNtf, OnTreasureDataNtf, CmdTreasureDataNtf.Parser);
            //EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTreasure.AddExpNtf, OnAddExpNtf, CmdTreasureAddExpNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTreasure.UpdateNtf, OnUpdateNtf, CmdTreasureUpdateNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdTreasure.UnlockSlotReq, (ushort)CmdTreasure.UnlockSlotRes, OnUnlockSlotRes, CmdTreasureUnlockSlotRes.Parser);
            //EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTreasure.GetTreasureNtf, OnGetTreasureNtf, CmdTreasureGetTreasureNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdTreasure.EquipReq, (ushort)CmdTreasure.EquipRes, OnEquipRes, CmdTreasureEquipRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdTreasure.UnEquipReq, (ushort)CmdTreasure.UnEquipRes, OnUnEquipRes, CmdTreasureUnEquipRes.Parser);
            
        }

        private void OnTreasureDataNtf(NetMsg msg)
        {
            CmdTreasureDataNtf ntf = NetMsgUtil.Deserialize<CmdTreasureDataNtf>(CmdTreasureDataNtf.Parser, msg);
            mTreasureData = ntf.Data;
        }

        //private void OnAddExpNtf(NetMsg msg)
        //{
        //    CmdTreasureAddExpNtf ntf = NetMsgUtil.Deserialize<CmdTreasureAddExpNtf>(CmdTreasureAddExpNtf.Parser, msg);
        //}

        private void OnUpdateNtf(NetMsg msg)
        {
            CmdTreasureUpdateNtf ntf = NetMsgUtil.Deserialize<CmdTreasureUpdateNtf>(CmdTreasureUpdateNtf.Parser, msg);

            uint newTreasureId = 0;
            if (mTreasureData != null)
            {
                if (mTreasureData.UnlockedIdList.Count < ntf.Data.UnlockedIdList.Count)
                {
                    newTreasureId = ntf.Data.UnlockedIdList[ntf.Data.UnlockedIdList.Count-1];
                }
            }

            mTreasureData = ntf.Data;

            if (newTreasureId != 0)
                eventEmitter.Trigger(EEvents.OnUnlockNotify);

            //if (newTreasureId != 0)
            //{
            //    string Name = CSVLanguage.Instance.GetConfData(CSVItem.Instance.GetConfData(newTreasureId).name_id).words;
            //    string content = string.Format(LanguageHelper.GetTextContent(1000997), Name);
            //    Sys_Hint.Instance.PushContent_GetReward(content, newTreasureId);
            //}

            eventEmitter.Trigger(EEvents.OnRefreshNtf);
        }

        public void UnlockSlotReq()
        {
            CmdTreasureUnlockSlotReq req = new CmdTreasureUnlockSlotReq();
            NetClient.Instance.SendMessage((ushort)CmdTreasure.UnlockSlotReq, req);
        }

        private void OnUnlockSlotRes(NetMsg msg)
        {
            CmdTreasureUnlockSlotRes res = NetMsgUtil.Deserialize<CmdTreasureUnlockSlotRes>(CmdTreasureUnlockSlotRes.Parser, msg);
            //eventEmitter.Trigger(EEvents.OnRrefreshDisplayNtf);
        }

        public void EquipReq(uint treasureId)
        {
            CmdTreasureEquipReq req = new CmdTreasureEquipReq();
            req.TreasureId = treasureId;

            //计算槽位
            for (int i = 0; i < mTreasureData.EquipedIdList.Count; ++i)
            {
                if (mTreasureData.EquipedIdList[i] == 0)
                {
                    req.Slot = (uint)i + 1;
                    break;
                }
            }
            
            NetClient.Instance.SendMessage((ushort)CmdTreasure.EquipReq, req);
        }

        private void OnEquipRes(NetMsg msg)
        {
            //CmdTreasureEquipRes res = NetMsgUtil.Deserialize<CmdTreasureEquipRes>(CmdTreasureEquipRes.Parser, msg);
            //eventEmitter.Trigger(EEvents.OnRrefreshDisplayNtf);

            Sys_Hint.Instance.PushEffectInNextFight();
        }

        public void UnEquipReq(uint slot)
        {
            CmdTreasureUnEquipReq req = new CmdTreasureUnEquipReq();
            req.Slot = slot;
            NetClient.Instance.SendMessage((ushort)CmdTreasure.UnEquipReq, req);
        }

        private void OnUnEquipRes(NetMsg msg)
        {
            //CmdTreasureUnEquipRes res = NetMsgUtil.Deserialize<CmdTreasureUnEquipRes>(CmdTreasureUnEquipRes.Parser, msg);

            Sys_Hint.Instance.PushEffectInNextFight();
        }

        #region Logic
        public List<uint> GetSlotList()
        {
            List<uint> list = new List<uint>();
            //Dictionary<uint, CSVTreasuresUnlock.Data> dict = CSVTreasuresUnlock.Instance.GetDictData();
            list.AddRange(CSVTreasuresUnlock.Instance.GetKeys());
            return list;
        }

        public bool IsSlotUnlock(uint slotId)
        {
            return slotId <= mTreasureData.EquipedIdList.Count;
        }

        public bool IsWaitUnlock(uint slotId)
        {
            return slotId == mTreasureData.EquipedIdList.Count + 1;
        }

        public uint GetTreasureAtSlot(uint slotId)
        {
            return mTreasureData.EquipedIdList[(int)slotId - 1];
        }

        public bool IsFixLevel(uint level)
        {
            return mTreasureData.Level >= level;
        }

        public string GetOwnShow()
        {
            return string.Format("{0}/{1}", mTreasureData.UnlockedIdList.Count.ToString(), CSVTreasures.Instance.Count.ToString());
        }

        public List<CSVTreasures.Data>  GetListBySortType(ETreasureSortType sortType)
        {
            List<CSVTreasures.Data>  list = new List<CSVTreasures.Data>();

            if (sortType == ETreasureSortType.All)
            {
                List<CSVTreasures.Data>  unlockDisplay = new List<CSVTreasures.Data>();
                List<CSVTreasures.Data>  unlockCanDisplay = new List<CSVTreasures.Data>();
                List<CSVTreasures.Data>  unlockNotCanDisplay = new List<CSVTreasures.Data>();
                List<CSVTreasures.Data>  locked = new List<CSVTreasures.Data>();

                var dict = CSVTreasures.Instance.GetAll();
                foreach (var data in dict)
                {
                    if (mTreasureData.EquipedIdList.Contains(data.id))
                    {
                        unlockDisplay.Add(data);
                    }
                    else if (mTreasureData.UnlockedIdList.Contains(data.id))
                    {
                        if (IsFixLevel(data.level))
                        {
                            unlockCanDisplay.Add(data);
                        }
                        else
                        {
                            unlockNotCanDisplay.Add(data);
                        }
                    }
                    else
                    {
                        locked.Add(data);
                    }
                }

                SortList(unlockDisplay);
                SortList(unlockCanDisplay);
                SortList(unlockNotCanDisplay);
                SortList(locked);

                list.AddRange(unlockDisplay);
                list.AddRange(unlockCanDisplay);
                list.AddRange(unlockNotCanDisplay);
                list.AddRange(locked);
            }
            else if (sortType == ETreasureSortType.Lock)
            {
                var dict = CSVTreasures.Instance.GetAll();
                foreach (var data in dict)
                {
                    if (!mTreasureData.UnlockedIdList.Contains(data.id))
                    {
                        list.Add(data);
                    }
                }

                SortList(list);
            }
            else if (sortType == ETreasureSortType.Unlock)
            {
                List<CSVTreasures.Data>  unlockDisplay = new List<CSVTreasures.Data>();
                List<CSVTreasures.Data>  unlockCanDisplay = new List<CSVTreasures.Data>();
                List<CSVTreasures.Data>  unlockNotCanDisplay = new List<CSVTreasures.Data>();

                foreach (uint infoId in mTreasureData.UnlockedIdList)
                {
                    CSVTreasures.Data infoData = CSVTreasures.Instance.GetConfData(infoId);
                    if (mTreasureData.EquipedIdList.Contains(infoId))
                    {
                        unlockDisplay.Add(infoData);
                    }
                    else
                    {
                        if (IsFixLevel(infoData.level))
                        {
                            unlockCanDisplay.Add(infoData);
                        }
                        else
                        {
                            unlockNotCanDisplay.Add(infoData);
                        }
                    }
                }

                SortList(unlockDisplay);
                SortList(unlockCanDisplay);
                SortList(unlockNotCanDisplay);

                list.AddRange(unlockDisplay);
                list.AddRange(unlockCanDisplay);
                list.AddRange(unlockNotCanDisplay);
            }

            return list;
        }

        private void SortList(List<CSVTreasures.Data>  list)
        {
            if (list.Count >= 2)
            {
                list.Sort((var1, var2) => { return var1.sort_id.CompareTo(var2.sort_id); });
            }
        }

        public bool IsDisplay(uint treasureId)
        {
            return mTreasureData.EquipedIdList.Contains(treasureId);
        }

        public bool IsUnlock(uint treasureId)
        {
            return mTreasureData.UnlockedIdList.Contains(treasureId);
        }

        public bool IsHaveTreasure()
        {
            return mTreasureData != null && mTreasureData.UnlockedIdList != null && mTreasureData.UnlockedIdList.Count > 0;
        }
        
        #endregion
    }
}