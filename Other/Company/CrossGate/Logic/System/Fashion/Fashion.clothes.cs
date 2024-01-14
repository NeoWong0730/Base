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
    /// 时装
    /// </summary>
    public class FashionClothes
    {
        public CSVFashionClothes.Data cSVFashionClothesData { get; private set; }
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
                Sys_Fashion.Instance.eventEmitter.Trigger<uint>(Sys_Fashion.EEvents.OnUpdateClothesLockState, Id);
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
                    Sys_Fashion.Instance.eventEmitter.Trigger<uint>(Sys_Fashion.EEvents.OnUpdateClothesDressState, Id);
                }
            }
        }
        public ulong ExpireTime { get; set; }
        public Dictionary<ETintIndex, ColorCache> OwnTintWarp = new Dictionary<ETintIndex, ColorCache>();
        public List<uint> schemes = new List<uint>();//方案1 2 长度为2， 0/1代表是否有方案
        public int curUseScheme { get; private set; }
        public FashionTimer fashionTimer;
        public OwnerType ownerType = OwnerType.None;
        public Dictionary<ETintIndex, Color> curUseColor = new Dictionary<ETintIndex, Color>();
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

        public FashionClothes(uint id, bool _unlock, bool dress)
        {
            Id = id;
            UnLock = _unlock;
            Dress = dress;
            cSVFashionClothesData = CSVFashionClothes.Instance.GetConfData(id);

            OwnTintWarp.Add(ETintIndex.R, new ColorCache());
            OwnTintWarp.Add(ETintIndex.G, new ColorCache());
            OwnTintWarp.Add(ETintIndex.B, new ColorCache());
            OwnTintWarp.Add(ETintIndex.A, new ColorCache());

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
            if (cSVFashionClothesData.LimitedTime == 0)
                return true;
            List<uint> items = cSVFashionClothesData.FashionItem;
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
            if (cSVFashionClothesData.LimitedTime == 0 || ExpireTime == 0)
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

                Sys_Fashion.Instance.AddActiveFashionClothes(this);
            }
            Sys_Fashion.Instance.eventEmitter.Trigger<uint>(Sys_Fashion.EEvents.OnUpdateClothesLimitTime, Id);
        }

        public void ReCalTime(ulong _ExpireTime)
        {
            ExpireTime = _ExpireTime;
            fashionTimer.bNeedUpdateTime = true;
            //fashionTimer.ExpireDateTime = Sys_Time.ConvertFromTimeStamp((uint)ExpireTime);
            fashionTimer.EndTime = ExpireTime;
        }

        public void Update()
        {
            fashionTimer?.Update();
        }

        // 当改变选色的时候 需要调用此检测
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
                bool gEqual = GetCurUseColor(ETintIndex.G) == GetColor(1 - curUseScheme, ETintIndex.G);
                bool bEqual = GetCurUseColor(ETintIndex.B) == GetColor(1 - curUseScheme, ETintIndex.B);
                bool aEqual = GetCurUseColor(ETintIndex.A) == GetColor(1 - curUseScheme, ETintIndex.A);
                if (rEqual && gEqual && bEqual && aEqual)
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

            //if (colors.Count > 0)
            //{
            //    DebugUtil.LogFormat(ELogType.eFashion, "时装染色请求");
            //    Sys_Fashion.Instance.FashionDyeFashionReq(Id, colors, IsSwatches);
            //}
        }

        public void ClearColorCache()
        {
            OwnTintWarp[ETintIndex.R] = new ColorCache();
            OwnTintWarp[ETintIndex.G] = new ColorCache();
            OwnTintWarp[ETintIndex.B] = new ColorCache();
            OwnTintWarp[ETintIndex.A] = new ColorCache();
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

        public bool HasColorCache(ETintIndex eTintIndex)
        {
            return OwnTintWarp[eTintIndex].hasColorCache;
        }

        //获取当前使用的染色方案
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
            return Sys_Fashion.Instance.GetClothesFirstColor(cSVFashionClothesData.id, tintIndex, Sys_Role.Instance.HeroId);
        }

        public void SetCurUseColor(ETintIndex eTintIndex, Color color)
        {
            curUseColor[eTintIndex] = color;
        }

        //获取实时的，未保存的染色
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

            Color color_G = GetLastUseColor(ETintIndex.G);
            SetCurUseColor(ETintIndex.G, color_G);

            Color color_B = GetLastUseColor(ETintIndex.B);
            SetCurUseColor(ETintIndex.B, color_B);

            Color color_A = GetLastUseColor(ETintIndex.A);
            SetCurUseColor(ETintIndex.A, color_A);
            ResetDirty();
        }

        public void ResetCurUseColorToScheme(int scheme)
        {
            Color color_R = GetColor(scheme, ETintIndex.R);
            SetCurUseColor(ETintIndex.R, color_R);

            Color color_G = GetColor(scheme, ETintIndex.G);
            SetCurUseColor(ETintIndex.G, color_G);

            Color color_B = GetColor(scheme, ETintIndex.B);
            SetCurUseColor(ETintIndex.B, color_B);

            Color color_A = GetColor(scheme, ETintIndex.A);
            SetCurUseColor(ETintIndex.A, color_A);
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


