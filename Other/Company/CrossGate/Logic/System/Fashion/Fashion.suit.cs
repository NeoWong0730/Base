using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using System;
using Lib.Core;

namespace Logic
{

    /// <summary>
    /// 套装
    /// </summary>
    public class FashionSuit
    {
        public CSVFashionSuit.Data cSVFashionSuitData
        {
            get;
            private set;
        }
        public uint Id { get; private set; }

        private bool unlock;

        public bool UnLock
        {
            get
            {
                return unlock;
            }
            set
            {
                if (unlock != value)
                {
                    unlock = value;
                    Sys_Fashion.Instance.eventEmitter.Trigger<uint>(Sys_Fashion.EEvents.OnUpdateSuitLockState, Id);
                }
            }
        }

        private bool dress;
        public bool Dress
        {
            get { return dress; }
            set
            {
                if (dress != value)
                {
                    dress = value;
                    if (dress)
                    {
                        uint oldFashionId = Sys_Fashion.Instance.GetOldFashionId(cSVFashionSuitData.FashionId);
                        if (oldFashionId != 0)
                        {
                            Sys_Fashion.Instance._FashionClothes.Find(x => x.Id == oldFashionId).Dress = false;
                        }
                        Sys_Fashion.Instance._FashionClothes.Find(x => x.Id == cSVFashionSuitData.FashionId).Dress = true;

                        uint oldFashionId_1 = Sys_Fashion.Instance.GetOldFashionId(cSVFashionSuitData.WeaponId);
                        if (oldFashionId_1 != 0)
                        {
                            Sys_Fashion.Instance._FashionWeapons.Find(x => x.Id == oldFashionId_1).Dress = false;
                        }
                        Sys_Fashion.Instance._FashionWeapons.Find(x => x.Id == cSVFashionSuitData.WeaponId).Dress = true;

                        for (int i = 0; i < cSVFashionSuitData.AccId.Count; i++)
                        {
                            uint oldFashionId_2 = Sys_Fashion.Instance.GetOldFashionId(cSVFashionSuitData.AccId[i]);
                            if (oldFashionId_2 != 0)
                            {
                                Sys_Fashion.Instance._FashionAccessories.Find(x => x.Id == oldFashionId_2).Dress = false;
                            }
                            Sys_Fashion.Instance._FashionAccessories.Find(x => x.Id == cSVFashionSuitData.AccId[i]).Dress = true;
                        }
                    }
                    Sys_Fashion.Instance.eventEmitter.Trigger<uint>(Sys_Fashion.EEvents.OnUpdateSuitDressState, Id);
                }
            }
        }
        public List<uint> associated = new List<uint>();   //关联部件id
        public List<uint> unlockedAssoiated = new List<uint>();//已经解锁的部件

        public FashionSuit(uint _id, bool unlock)
        {
            Id = _id;
            UnLock = unlock;
            cSVFashionSuitData = CSVFashionSuit.Instance.GetConfData(Id);
            associated.Add(cSVFashionSuitData.FashionId);
            associated.Add(cSVFashionSuitData.WeaponId);
            if (cSVFashionSuitData.AccId != null)
            {

                associated.AddRange(cSVFashionSuitData.AccId);
            }
        }

        public void TryAddAsso(uint id)
        {
            unlockedAssoiated.AddOnce<uint>(id);
            Sys_Fashion.Instance.eventEmitter.Trigger<uint>(Sys_Fashion.EEvents.OnUpdateSuitAsso, Id);
        }

        public void TryRemoveAsso(uint id)
        {
            unlockedAssoiated.TryRemove<uint>(id);
            Sys_Fashion.Instance.eventEmitter.Trigger<uint>(Sys_Fashion.EEvents.OnUpdateSuitAsso, Id);
        }

        public List<uint> GetLockedAssociated()
        {
            List<uint> LockedAssociated = new List<uint>();
            for (int i = 0; i < associated.Count; i++)
            {
                if (!unlockedAssoiated.Contains(associated[i]))
                {
                    LockedAssociated.Add(associated[i]);
                }
            }
            return LockedAssociated;
        }
    }
}


