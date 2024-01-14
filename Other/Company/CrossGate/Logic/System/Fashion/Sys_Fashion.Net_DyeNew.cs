using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Collections;
using Table;
using System;


namespace Logic
{

    public partial class Sys_Fashion : SystemModuleBase<Sys_Fashion>, ISystemModuleUpdate
    {
        private void NetEventRegister()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdFashion.FashionTotalValueNtf, FashionTotalValueNtf, CmdFashionFashionTotalValueNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdFashion.GetTotalValueAwardReq, (ushort)CmdFashion.GetTotalValueAwardRes, FashionGetTotalValueAwardRes, CmdFashionGetTotalValueAwardRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdFashion.SwitchDyeReq, (ushort)CmdFashion.SwitchDyeRes, FashionSwitchDyeRes, CmdFashionSwitchDyeRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdFashion.NewDyeFashionReq, (ushort)CmdFashion.NewDyeFashionRes, FashionNewDyeFashionRes, CmdFashionNewDyeFashionRes.Parser);
        }

        //领取时装值奖励
        public void FashionGetTotalValueAwardReq(uint id)
        {
            CmdFashionGetTotalValueAwardReq req = new CmdFashionGetTotalValueAwardReq();
            req.Id = id;
            NetClient.Instance.SendMessage((ushort)CmdFashion.GetTotalValueAwardReq, req);
        }

        private void FashionGetTotalValueAwardRes(NetMsg netMsg)
        {
            CmdFashionGetTotalValueAwardRes res = NetMsgUtil.Deserialize<CmdFashionGetTotalValueAwardRes>(CmdFashionGetTotalValueAwardRes.Parser, netMsg);
            rewardsGet.Add(res.Id);
            eventEmitter.Trigger<uint>(EEvents.OnGetFashionReward, res.Id);
            eventEmitter.Trigger(EEvents.OnUpdateFashionPoint);
        }

        //保存染色请求
        public void FashionNewDyeFashionReq(uint fashionId, DyeScheme dyeScheme, bool IsSwatches, uint index)
        {
            Sys_Hint.Instance.PushEffectInNextFight();
            CmdFashionNewDyeFashionReq req = new CmdFashionNewDyeFashionReq();
            req.FashionId = fashionId;
            req.IsAdvanceDye = IsSwatches;
            req.Index = index;
            req.DyeScheme = dyeScheme;
            NetClient.Instance.SendMessage((ushort)CmdFashion.NewDyeFashionReq, req);
            DebugUtil.LogFormat(ELogType.eFashion, "染色保存请求：存放至方案{0}", index);
            /*if (IsSwatches)
            {
                Sys_MagicBook.Instance.eventEmitter.Trigger<EMagicAchievement>(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event18);
            }
            else
            {
                Sys_MagicBook.Instance.eventEmitter.Trigger<EMagicAchievement>(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event52);
            }*/
        }

        private void FashionNewDyeFashionRes(NetMsg netMsg)
        {
            CmdFashionNewDyeFashionRes res = NetMsgUtil.Deserialize<CmdFashionNewDyeFashionRes>(CmdFashionNewDyeFashionRes.Parser, netMsg);
            for (int i = 0; i < _FashionClothes.Count; i++)
            {
                if (_FashionClothes[i].Id == res.FashionId)
                {
                    DyeScheme dyeScheme = res.DyeScheme;
                    for (int j = 0; j < dyeScheme.DyeInfo.Count; j++)
                    {
                        DyeInfo dyeInfo = dyeScheme.DyeInfo[j];
                        ColorCache colorcache = _FashionClothes[i].OwnTintWarp[(ETintIndex)dyeInfo.DyeIndex];
                        uint value = dyeInfo.Value;
                        Color32 color32 = Color32Extensions.FromUInt32(value);
                        colorcache.SetColor((int)res.Index, color32);
                    }
                    _FashionClothes[i].schemes[(int)res.Index] = 1;
                    DebugUtil.LogFormat(ELogType.eFashion, "染色保存返回: 时装衣服: {0}有 {1}套染色方案 ,当前使用方案{2}", _FashionClothes[i].Id,
                 _FashionClothes[i].SchemeCount, _FashionClothes[i].curUseScheme);
                    if (_FashionClothes[i].curUseScheme == (int)res.Index)
                    {
                        GameCenter.mainHero.heroBaseComponent.fashData = GetDressData();
                        GameCenter.mainHero.ChangeModel();
                    }
                }
            }
            for (int i = 0; i < _FashionAccessories.Count; i++)
            {
                if (_FashionAccessories[i].Id == res.FashionId)
                {
                    DyeScheme dyeScheme = res.DyeScheme;
                    for (int j = 0; j < dyeScheme.DyeInfo.Count; j++)
                    {
                        DyeInfo dyeInfo = dyeScheme.DyeInfo[j];
                        ColorCache colorcache = _FashionAccessories[i].OwnTintWarp[(ETintIndex)dyeInfo.DyeIndex];
                        uint value = dyeInfo.Value;
                        Color32 color32 = Color32Extensions.FromUInt32(value);
                        colorcache.SetColor((int)res.Index, color32);
                    }
                    _FashionAccessories[i].schemes[(int)res.Index] = 1;
                    DebugUtil.LogFormat(ELogType.eFashion, "染色保存返回: 时装挂饰 {0}有{1}套染色方案,当前使用方案{2}", _FashionAccessories[i].Id,
               _FashionAccessories[i].SchemeCount, _FashionAccessories[i].curUseScheme);
                    if (_FashionAccessories[i].curUseScheme == (int)res.Index)
                    {
                        GameCenter.mainHero.heroBaseComponent.fashData = GetDressData();
                        GameCenter.mainHero.ChangeModel();
                    }
                }
            }
            for (int i = 0; i < _FashionWeapons.Count; i++)
            {
                if (_FashionWeapons[i].Id == res.FashionId)
                {
                    DyeScheme dyeScheme = res.DyeScheme;
                    for (int j = 0; j < dyeScheme.DyeInfo.Count; j++)
                    {
                        DyeInfo dyeInfo = dyeScheme.DyeInfo[j];
                        ColorCache colorcache = _FashionWeapons[i].OwnTintWarp[(ETintIndex)dyeInfo.DyeIndex];
                        uint value = dyeInfo.Value;
                        Color32 color32 = Color32Extensions.FromUInt32(value);
                        colorcache.SetColor((int)res.Index, color32);
                    }
                    _FashionWeapons[i].schemes[(int)res.Index] = 1;
                    DebugUtil.LogFormat(ELogType.eFashion, "染色保存返回: 时装武器 :{0} 有{1}套染色方案,当前使用方案{2}", _FashionWeapons[i].Id,
              _FashionWeapons[i].SchemeCount, _FashionWeapons[i].curUseScheme);
                    if (_FashionWeapons[i].curUseScheme == (int)res.Index)
                    {
                        GameCenter.mainHero.heroBaseComponent.fashData = GetDressData();
                        GameCenter.mainHero.ChangeModel();
                    }
                }
            }

            eventEmitter.Trigger<int>(EEvents.OnSaveDyeSuccess, (int)res.Index);
            eventEmitter.Trigger<bool>(EEvents.OnUpdateDyeButtonState, false);
            eventEmitter.Trigger(EEvents.OnUpdateDyePropRoot);
            eventEmitter.Trigger(EEvents.UpdateCompareLastShowOrHide);
        }

        //切换染色方案请求
        public void FashionSwitchDyeReq(uint fashionId, uint useIndex)
        {
            CmdFashionSwitchDyeReq req = new CmdFashionSwitchDyeReq();
            req.FashionId = fashionId;
            req.UseIndex = useIndex;
            NetClient.Instance.SendMessage((ushort)CmdFashion.SwitchDyeReq, req);
        }

        private void FashionSwitchDyeRes(NetMsg netMsg)
        {
            CmdFashionSwitchDyeRes res = NetMsgUtil.Deserialize<CmdFashionSwitchDyeRes>(CmdFashionSwitchDyeRes.Parser, netMsg);
            for (int i = 0; i < _FashionClothes.Count; i++)
            {
                if (_FashionClothes[i].Id == res.FashionId)
                {
                    _FashionClothes[i].SetCurrentUseScheme((int)res.UserIndex);
                    DebugUtil.LogFormat(ELogType.eFashion, "染色方案切换返回: 时装衣服:{0} 一共有{1}套方案,当前使用方案{2}", _FashionClothes[i].Id,
               _FashionClothes[i].SchemeCount, _FashionClothes[i].curUseScheme);
                }
            }

            for (int i = 0; i < _FashionWeapons.Count; i++)
            {
                if (_FashionWeapons[i].Id == res.FashionId)
                {
                    _FashionWeapons[i].SetCurrentUseScheme((int)res.UserIndex);
                    DebugUtil.LogFormat(ELogType.eFashion, "染色方案切换返回: 时装武器:{0} 一共有{1}套方案, 当前使用方案{2}", _FashionWeapons[i].Id,
              _FashionWeapons[i].SchemeCount, _FashionWeapons[i].curUseScheme);
                }
            }

            for (int i = 0; i < _FashionAccessories.Count; i++)
            {
                if (_FashionAccessories[i].Id == res.FashionId)
                {
                    _FashionAccessories[i].SetCurrentUseScheme((int)res.UserIndex);
                    DebugUtil.LogFormat(ELogType.eFashion, "染色方案切换返回: 时装挂饰:{0} 一共有{1}套方案, 当前使用方案{2}", _FashionAccessories[i].Id,
               _FashionAccessories[i].SchemeCount, _FashionAccessories[i].curUseScheme);
                }
            }
            //更新模型染色
            GameCenter.mainHero.heroBaseComponent.fashData = GetDressData();
            GameCenter.mainHero.ChangeModel();
        }

        //时装值更新
        private void FashionTotalValueNtf(NetMsg netMsg)
        {
            CmdFashionFashionTotalValueNtf ntf = NetMsgUtil.Deserialize<CmdFashionFashionTotalValueNtf>(CmdFashionFashionTotalValueNtf.Parser, netMsg);
            fashionPoint = ntf.TotalValue;
            eventEmitter.Trigger(EEvents.OnUpdateFashionPoint);
        }
    }
}


