using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using System;
using Lib.Core;
using Packet;

namespace Logic
{
    /// <summary>
    /// 挂饰
    /// </summary>
    public class FashionAccessory
    {
        public CSVFashionAccessory.Data cSVFashionAccessoryData
        {
            get;
            private set;
        }

        public EHeroModelParts AcceType
        {
            get { return (EHeroModelParts)cSVFashionAccessoryData.Acctype; }
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
                    if (unlock)
                    {

                    }
                    else
                    {
                        //ClearColorCache();
                        curUseScheme = -1;
                        fashionTimer.bNeedUpdateTime = false;
                        ownerType = OwnerType.None;
                    }
                    Sys_Fashion.Instance.CheckUnlockedList(Id, unlock);
                    Sys_Fashion.Instance.eventEmitter.Trigger(Sys_Fashion.EEvents.OnUpdatePropRoot);
                    Sys_Fashion.Instance.eventEmitter.Trigger(Sys_Fashion.EEvents.OnSetColorBtnActive);
                }
                Sys_Fashion.Instance.eventEmitter.Trigger<uint>(Sys_Fashion.EEvents.OnUpdateAcceLockState, Id);
            }
        }
        private bool dress;
        public bool Dress
        {
            get
            {
                return dress;
            }
            set
            {
                if (dress != value)
                {
                    dress = value;
                    Sys_Fashion.Instance.eventEmitter.Trigger<uint>(Sys_Fashion.EEvents.OnUpdateAcceDressState, Id);
                }
            }
        }
        public ulong ExpireTime { get; set; }

        public Dictionary<ETintIndex, ColorCache> OwnTintWarp = new Dictionary<ETintIndex, ColorCache>();
        public Dictionary<ETintIndex, Color> curUseColor = new Dictionary<ETintIndex, Color>();
        public List<uint> schemes = new List<uint>();//方案1 2 长度为2， 0/1代表是否有方案
        public int curUseScheme { get; private set; }
        public FashionTimer fashionTimer;
        public OwnerType ownerType = OwnerType.None;
        public bool hasScheme
        {
            get
            {
                return curUseScheme != -1;
            }
        }
        public int SchemeCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < schemes.Count; i++)
                {
                    if (schemes[i] > 0)
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        public FashionAccessory(uint id, bool _unlock, bool dress)
        {
            Id = id;
            UnLock = _unlock;
            Dress = dress;
            cSVFashionAccessoryData = CSVFashionAccessory.Instance.GetConfData(id);
            OwnTintWarp.Add(ETintIndex.R, new ColorCache());

            schemes.Add(0);
            schemes.Add(0);
            curUseScheme = -1;

            fashionTimer = new FashionTimer(() =>
            {
                Sys_Fashion.Instance.FashionExpireReq(Id);
            });
        }

        public uint PropItem;

        public bool CanLock()
        {
            if (cSVFashionAccessoryData.LimitedTime == 0)
                return true;
            List<uint> items = cSVFashionAccessoryData.AccItem;
            for (int i = 0; i < items.Count; i++)
            {
                if (Sys_Bag.Instance.GetItemCount(items[i]) > 0)
                {
                    PropItem = items[i];
                    return true;
                }
            }
            return false;
        }

        public void CalTimeOut(uint ExpireTimeLen)
        {
            if (cSVFashionAccessoryData.LimitedTime == 0 || ExpireTime == 0)
            {
                ownerType = OwnerType.Forever;
                fashionTimer.bNeedUpdateTime = false;
            }
            else
            {
                ownerType = OwnerType.Limit;
                fashionTimer.bNeedUpdateTime = true;
                //fashionTimer.ExpireDateTime = Sys_Time.ConvertFromTimeStamp((uint)ExpireTime);
                fashionTimer.EndTime = ExpireTime;
                fashionTimer.CalRemainTime(ExpireTimeLen);

                Sys_Fashion.Instance.AddActiveFashionAccessory(this);
            }
            Sys_Fashion.Instance.eventEmitter.Trigger<uint>(Sys_Fashion.EEvents.OnUpdateAcceLimitTime, Id);
        }

        public void ReCalTime(ulong _ExpireTime)
        {
            ExpireTime = _ExpireTime;
            fashionTimer.bNeedUpdateTime = true;
            fashionTimer.EndTime = ExpireTime;
            //fashionTimer.ExpireDateTime = Sys_Time.ConvertFromTimeStamp((uint)ExpireTime);
        }

        public void Update()
        {
            fashionTimer?.Update();
        }

        public void CheckDirty(ETintIndex eTintIndex, Color32 color32)
        {
            Color color = GetLastUseColor(eTintIndex);
            Color nextColor = color32;
            ColorCache colorCache = OwnTintWarp[eTintIndex];
            bool dirty = false;
            colorCache.SetDirty(color != nextColor);
            foreach (var item in OwnTintWarp)
            {
                if (item.Value.Dirty)
                {
                    dirty = true;
                }
            }
            if (dirty && SchemeCount == 2)
            {
                bool rEqual = GetCurUseColor(ETintIndex.R) == GetColor(1 - curUseScheme, ETintIndex.R);
                if (rEqual)
                {
                    dirty = false;
                }
            }
            Sys_Fashion.Instance.eventEmitter.Trigger<bool>(Sys_Fashion.EEvents.OnUpdateDyeButtonState, dirty);
        }

        //时装染色
        public void TryDyeFashion(bool IsSwatches, int _curUseScheme)
        {
            Dictionary<uint, Color32> colors = new Dictionary<uint, Color32>();
            DyeScheme dyeScheme = new DyeScheme();
            foreach (var item in OwnTintWarp)
            {
                //if (item.Value.Dirty)
                //{
                //    colors.Add((uint)item.Key, (Color32)GetCurUseColor(item.Key));
                //}
                DyeInfo dyeInfo = new DyeInfo();
                dyeInfo.DyeIndex = (uint)item.Key;
                dyeInfo.Value = ((Color32)GetCurUseColor(item.Key)).ToUInt32();
                dyeScheme.DyeInfo.Add(dyeInfo);
            }
            Sys_Fashion.Instance.FashionNewDyeFashionReq(Id, dyeScheme, IsSwatches, (uint)_curUseScheme);
        }

        public void ClearColorCache()
        {
            OwnTintWarp[ETintIndex.R] = new ColorCache();
            curUseColor.Clear();
            for (int i = 0; i < schemes.Count; i++)
            {
                schemes[i] = 0;
            }
            curUseScheme = -1;
        }

        public void SetCurrentUseScheme(int index)
        {
            curUseScheme = index;
        }

        public Color GetColor(int index, ETintIndex eTintIndex)
        {
            if (schemes[index] == 0)
            {
                DebugUtil.LogErrorFormat("方案{0}不存在", index);
                return Color.white;
            }
            return OwnTintWarp[eTintIndex].GetColor(index);
        }

        public int GetAcceType()
        {
            return (int)cSVFashionAccessoryData.Acctype;
        }

        public bool HasColorCache(ETintIndex eTintIndex)
        {
            return OwnTintWarp[eTintIndex].hasColorCache;
        }


        public Color GetLastUseColor(ETintIndex tintIndex)
        {
            if (HasColorCache(tintIndex))
            {
                if (curUseScheme == -1)
                {
                    return GetFirstColor(tintIndex);
                }
                return GetColor(curUseScheme, tintIndex);
            }
            else
            {
                return GetFirstColor(tintIndex);
            }
        }

        public Color GetNextSchemeColor(ETintIndex tintIndex)
        {
            Color color = GetColor(1 - curUseScheme, tintIndex);

            return color;
        }

        public Color GetFirstColor(ETintIndex tintIndex)
        {
            return Sys_Fashion.Instance.GetAcceFirstColor(cSVFashionAccessoryData.id, tintIndex);
        }

        public void SetCurUseColor(ETintIndex eTintIndex, Color color)
        {
            curUseColor[eTintIndex] = color;
        }

        public Color GetCurUseColor(ETintIndex eTintIndex)
        {
            if (curUseColor.Count == 0)
            {
                ResetCurUseColor();
            }
            return curUseColor[eTintIndex];
        }

        public void ResetCurUseColor()
        {
            Color color_R = GetLastUseColor(ETintIndex.R);
            SetCurUseColor(ETintIndex.R, color_R);
            ResetDirty();
        }

        public void ResetCurUseColorToScheme(int scheme)
        {
            Color color_R = GetColor(scheme, ETintIndex.R);
            SetCurUseColor(ETintIndex.R, color_R);
            ResetDirty();
        }

        public void ResetDirty()
        {
            foreach (var item in OwnTintWarp)
            {
                item.Value.SetDirty(false);
            }
        }
    }

}


