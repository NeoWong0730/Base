using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Collections;
using Table;
using System;
using Framework;

namespace Logic
{
    public partial class Sys_Fashion : SystemModuleBase<Sys_Fashion>, ISystemModuleUpdate
    {
        public List<FashionClothes> _FashionClothes = new List<FashionClothes>();
        public List<FashionAccessory> _FashionAccessories = new List<FashionAccessory>();
        public List<FashionWeapon> _FashionWeapons = new List<FashionWeapon>();
        public List<FashionSuit> _FashionSuits = new List<FashionSuit>();

        public List<FashionClothes> _activeFashionClothes = new List<FashionClothes>();
        public List<FashionAccessory> _activeFashionAccessories = new List<FashionAccessory>();
        public List<FashionWeapon> _activeFashionWeapons = new List<FashionWeapon>();
        public List<FashionSuit> _activeFashionSuits = new List<FashionSuit>();

        public List<FashionAccessory> _FashionAcce_head_2 = new List<FashionAccessory>();
        public List<FashionAccessory> _FashionAcce_back_3 = new List<FashionAccessory>();
        public List<FashionAccessory> _FashionAcce_waist_4 = new List<FashionAccessory>();
        public List<FashionAccessory> _FashionAcce_face_5 = new List<FashionAccessory>();

        private List<uint> _UnLockedFashions = new List<uint>(); //解锁部件id
        public List<uint> _UnLockedSuits = new List<uint>(); //解锁套装id
        public List<uint> _UnLockedSuitAttr = new List<uint>(); //解锁套装属性id

        private List<uint> expireList = new List<uint>(); //过期部件id

        // public List<uint> dressedList = new List<uint>();          //已经穿戴的部件id
        public uint[] dressedList = new uint[6]; //已经穿戴的部件id
        public uint curSuit;
        public uint curUseSuit;

        public Dictionary<uint, EHeroModelParts> parts = new Dictionary<uint, EHeroModelParts>();

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        private List<uint> _expireFashionInFight = new List<uint>();

        public uint fashionPoint;

        public List<uint> rewardsGet = new List<uint>();

        public enum EEvents
        {
            OnLoadModelParts, //加载部件
            OnUnLoadModelParts, //卸载部件
            OnDyeModelParts, //染色部件
            OnUpdateClothesLockState,
            OnUpdateWeaponLockState,
            OnUpdateAcceLockState,
            OnUpdateSuitLockState,
            OnUpdateClothesDressState,
            OnUpdateWeaponDressState,
            OnUpdateAcceDressState,
            OnUpdateSuitDressState,
            OnUpdateDyeButtonState, //更新染色按钮状态
            OnUpdateClothesLimitTime,
            OnUpdateWeaponLimitTime,
            OnUpdateAcceLimitTime,
            OnUpdateSuitAsso, //更新套装关联列表
            OnUpdatePropRoot, //更新解锁道具ui
            OnUpdateUnLockButton, //更新解锁按钮状态
            OnUpdateDyePropRoot, //更新染色道接节点状态
            OnCheckDyeColorDirty,
            OnSetColorBtnActive, //染色按钮显示隐藏
            OnUpdateSuitChange,
            UpdateCompareLastShowOrHide,
            RevertToFirstColor, //时装过期回滚颜色
            SetToLastColor, //解锁时装需要回滚至之前的染色方案(如果有的话)
            OnUpdateSuit, //解锁刷新套装
            OnUpdateFashionPoint, //更新时装值
            OnGetFashionReward, //领取时装值
            OnSaveDyeSuccess, //保存方案成功
            OnDrawLucky, //抽奖完成 更新时装抽奖积分、抽奖道具
            OnRefreshDrawLuckyResult,
            OnExchangeDraw, //魔币兑换道具完成
            OnRefreshFreeDrawState, //更新免费抽奖状态
            OnRefreshLuckyDrawActiveState, //更新抽奖活动开启状态
            OnStartLuckyDrawFromRes, //结算界面继续抽奖
        }


        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdFashion.FashionListNtf, OnFashionListNtf, CmdFashionFashionListNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdFashion.LockStateNtf, OnLockStateNtf, CmdFashionLockStateNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdFashion.DyeStateNtf, OnDyeStateNtf, CmdFashionDyeStateNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdFashion.DressStateNtf, OnDressStateNtf, CmdFashionDressStateNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdFashion.SuitStateNtf, OnSuitStateNtf, CmdFashionSuitStateNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdFashion.UnlockFashionReq, (ushort) CmdFashion.UnlockFashionRes, UnlockFashionRes, CmdFashionUnlockFashionRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdFashion.DressFashionReq, (ushort) CmdFashion.DressFashionRes, DressFashionRes, CmdFashionDressFashionRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdFashion.UnfixReq, (ushort) CmdFashion.UnfixRes, FashionUnfixRes, CmdFashionUnfixRes.Parser);

            EventDispatcher.Instance.AddEventListener((ushort) CmdFashion.FashionExpireReq, (ushort) CmdFashion.FashionExpireRes, FashionExpireRes, CmdFashionFashionExpireRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdFashion.SuitSelReq, (ushort) CmdFashion.SuitSelRes, SuitSelRes, CmdFashionSuitSelRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdFashion.SuitAddrReq, (ushort) CmdFashion.SuitAddrRes, SuitAddrRes, CmdFashionSuitAddrRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdFashion.BuyPropAndUnlockReq, (ushort) CmdFashion.BuyPropAndUnlockRes, BuyPropAndUnlockRes, CmdFashionSuitAddrRes.Parser);

            EventDispatcher.Instance.AddEventListener((ushort) CmdFashion.DrawReq, (ushort) CmdFashion.DrawRes, OnDrawRes, CmdFashionDrawRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdFashion.ExchangeDrawItemReq, (ushort) CmdFashion.ExchangeDrawItemRes, FashionExchangeDrawItemRes, CmdFashionExchangeDrawItemRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdFashion.AutoBuyDrawItemReq, (ushort) CmdFashion.AutoBuyDrawItemRes, OnAutoBuyDrawCoinRes, CmdFashionAutoBuyDrawItemRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdFashion.DrawActiveInfoNtf, DrawActiveInfoNtf, CmdFashionDrawActiveInfoNtf.Parser);

            NetEventRegister();
            ParseClientData();
        }

        //解析客户端配置数据
        private void ParseClientData()
        {
            foreach (var item in CSVFashionSuit.Instance.GetAll())
            {
                if (item.Hide)
                {
                    continue;
                }

                FashionSuit fashionSuit = new FashionSuit(item.id, false);
                _FashionSuits.Add(fashionSuit);
            }

            foreach (var item in CSVFashionClothes.Instance.GetAll())
            {
                if (item.Hide)
                {
                    continue;
                }

                FashionClothes fashionClothes = new FashionClothes(item.id, false, false);
                _FashionClothes.Add(fashionClothes);
                parts.Add(item.id, EHeroModelParts.Main);
            }

            foreach (var item in CSVFashionAccessory.Instance.GetAll())
            {
                if (item.Hide)
                {
                    continue;
                }

                FashionAccessory fashionAccessory = new FashionAccessory(item.id, false, false);
                _FashionAccessories.Add(fashionAccessory);
                parts.Add(item.id, (EHeroModelParts) item.Acctype);
            }

            foreach (var item in CSVFashionWeapon.Instance.GetAll())
            {
                if (item.Hide)
                {
                    continue;
                }

                FashionWeapon fashionWeapon = new FashionWeapon(item.id, false, false);
                _FashionWeapons.Add(fashionWeapon);
                parts.Add(item.id, EHeroModelParts.Weapon);
            }

            _FashionAcce_head_2 = GetFashionAcce(2);
            _FashionAcce_back_3 = GetFashionAcce(3);
            _FashionAcce_waist_4 = GetFashionAcce(4);
            _FashionAcce_face_5 = GetFashionAcce(5);
        }

        public void OnUpdate()
        {
            for (int i = 0; i < _activeFashionClothes.Count; i++)
            {
                _activeFashionClothes[i].Update();
            }

            for (int i = 0; i < _activeFashionWeapons.Count; i++)
            {
                _activeFashionWeapons[i].Update();
            }

            for (int i = 0; i < _activeFashionAccessories.Count; i++)
            {
                _activeFashionAccessories[i].Update();
            }

            if (m_ActiveId != 0)
            {
                uint currentTime = Sys_Time.Instance.GetServerTime();
                if (currentTime > endTime)
                {
                    activeId = 0;
                }
            }

            if (m_ActiveId != 0)
            {
                if (cSVFashionActivityData.Free)
                {
                    if (!fashionFreeRedInfo.played)
                    {
                        freeDraw = true;
                    }
                    else
                    {
                        freeDraw = false;
                    }
                }
                else
                {
                    freeDraw = !Sys_Time.IsServerSameDay5(Sys_Time.Instance.GetServerTime(), Sys_Fashion.Instance.lastFreeDrawTime);
                }
            }
            else
            {
                freeDraw = false;
            }
        }

        private void ClearData()
        {
            expireList.Clear();
            _expireFashionInFight.Clear();

            for (int i = 0; i < dressedList.Length; i++)
            {
                dressedList[i] = 0;
            }

            _UnLockedFashions.Clear();
            _UnLockedSuits.Clear();
            _UnLockedSuitAttr.Clear();
            rewardsGet.Clear();
            foreach (var item in _FashionClothes)
            {
                item.UnLock = false;
                item.Dress = false;
                item.ClearColorCache();
            }

            foreach (var item in _FashionWeapons)
            {
                item.UnLock = false;
                item.Dress = false;
                item.ClearColorCache();
            }

            foreach (var item in _FashionAccessories)
            {
                item.UnLock = false;
                item.Dress = false;
                item.ClearColorCache();
            }

            foreach (var item in _FashionSuits)
            {
                item.UnLock = false;
                item.Dress = false;
            }
        }

        //上线通知
        private void OnFashionListNtf(NetMsg netMsg)
        {
            ClearData();

            LoadedMemory();

            CmdFashionFashionListNtf res = NetMsgUtil.Deserialize<CmdFashionFashionListNtf>(CmdFashionFashionListNtf.Parser, netMsg);

            #region LockData

            for (int k = 0; k < res.FashionList.Count; k++)
            {
                for (int i = 0; i < _FashionClothes.Count; i++)
                {
                    if (res.FashionList[k].FashionId == _FashionClothes[i].Id)
                    {
                        _FashionClothes[i].ExpireTime = res.FashionList[k].ExpireTime;
                        DebugUtil.Log(ELogType.eFashion, $"OnFashionListNtf id: {_FashionClothes[i].Id} ExpireTimeLen: {res.FashionList[k].ExpireTimeLen}");
                        _FashionClothes[i].CalTimeOut(res.FashionList[k].ExpireTimeLen);
                        _FashionClothes[i].UnLock = true;
                    }
                }
            }

            for (int k = 0; k < res.PendantList.Count; k++)
            {
                for (int i = 0; i < _FashionAccessories.Count; i++)
                {
                    if (res.PendantList[k].FashionId == _FashionAccessories[i].Id)
                    {
                        _FashionAccessories[i].ExpireTime = res.PendantList[k].ExpireTime;
                        _FashionAccessories[i].CalTimeOut(res.PendantList[k].ExpireTimeLen);
                        _FashionAccessories[i].UnLock = true;
                    }
                }
            }

            for (int k = 0; k < res.WeaponList.Count; k++)
            {
                for (int i = 0; i < _FashionWeapons.Count; i++)
                {
                    if (res.WeaponList[k].FashionId == _FashionWeapons[i].Id)
                    {
                        _FashionWeapons[i].ExpireTime = res.WeaponList[k].ExpireTime;
                        _FashionWeapons[i].CalTimeOut(res.WeaponList[k].ExpireTimeLen);
                        _FashionWeapons[i].UnLock = true;
                    }
                }
            }

            #endregion

            #region DressData

            for (int i = 0; i < res.DressList.Count; i++)
            {
                FashionClothes fashionClothes = _FashionClothes.Find(x => x.Id == res.DressList[i]);
                if (fashionClothes != null)
                {
                    fashionClothes.Dress = true;
                }

                FashionAccessory fashionAccessory = _FashionAccessories.Find(x => x.Id == res.DressList[i]);
                if (fashionAccessory != null)
                {
                    fashionAccessory.Dress = true;
                }

                FashionWeapon fashionWeapon = _FashionWeapons.Find(x => x.Id == res.DressList[i]);
                if (fashionWeapon != null)
                {
                    fashionWeapon.Dress = true;
                }

                dressedList[i] = res.DressList[i];
            }

            DebugUtil.LogFormat(ELogType.eFashion, DressListToString());

            #endregion


            #region SuitData

            curSuit = res.CurSuitId;
            curUseSuit = res.CurUseSuitId;
            for (int i = 0; i < _FashionSuits.Count; i++)
            {
                if (_FashionSuits[i].Id == curSuit)
                {
                    _FashionSuits[i].Dress = true;
                }
                else
                {
                    _FashionSuits[i].Dress = false;
                }
            }

            #endregion

            #region LuckyDraw

            activeId = res.ActiveId;
            lastFreeDrawTime = res.FreeDrawTime;
            startTime = res.StartTime;
            endTime = res.EndTime;


            DebugUtil.Log(ELogType.eFashion, lastFreeDrawTime.ToString());

            autoBuyDraw = res.AutoBuyDrawItem > 0;

            #endregion

            fashionPoint = res.FashionTotalValue;
            for (int i = 0; i < res.Id.Count; i++)
            {
                rewardsGet.Add(res.Id[i]);
            }

            #region DyeData

            for (int i = 0; i < res.DyeList.Count; i++)
            {
                for (int k = 0; k < _FashionClothes.Count; k++)
                {
                    if (res.DyeList[i].FashionId == _FashionClothes[k].Id)
                    {
                        FashionClothes fashionClothes = _FashionClothes[k];
                        fashionClothes.SetCurrentUseScheme((int) res.DyeList[i].UseIndex);
                        SetFashionClothesDyeSheme(fashionClothes, res.DyeList[i].DyeScheme); //方案  (服务器那边设计上固定是2长度)
                    }
                }

                for (int k = 0; k < _FashionWeapons.Count; k++)
                {
                    if (res.DyeList[i].FashionId == _FashionWeapons[k].Id)
                    {
                        FashionWeapon fashionWeapon = _FashionWeapons[k];
                        fashionWeapon.SetCurrentUseScheme((int) res.DyeList[i].UseIndex);
                        SetFashionWeaponDyeSheme(fashionWeapon, res.DyeList[i].DyeScheme); //方案  (服务器那边设计上固定是2长度)
                    }
                }

                for (int k = 0; k < _FashionAccessories.Count; k++)
                {
                    if (res.DyeList[i].FashionId == _FashionAccessories[k].Id)
                    {
                        FashionAccessory fashionAccessory = _FashionAccessories[k];
                        fashionAccessory.SetCurrentUseScheme((int) res.DyeList[i].UseIndex);
                        SetFashionAcceDyeSheme(fashionAccessory, res.DyeList[i].DyeScheme); //方案  (服务器那边设计上固定是2长度)
                    }
                }
            }

            #endregion
        }

        #region 角色视野变更通知

        //时装部件解锁状态状态通知(给角色本身发送或者给角色视野列表发送)
        private void OnLockStateNtf(NetMsg netMsg)
        {
            CmdFashionLockStateNtf res = NetMsgUtil.Deserialize<CmdFashionLockStateNtf>(CmdFashionLockStateNtf.Parser, netMsg);
            //for (int i = 0; i < _FashionClothes.Count; i++)
            //{
            //    if (res.FashionId == _FashionClothes[i].Id)
            //    {
            //        if (res.LockState == 1)
            //        {
            //            _FashionClothes[i].Unlock = false;
            //        }
            //        if (res.LockState == 2)
            //        {
            //            _FashionClothes[i].Unlock = true;
            //        }
            //    }
            //}
            //for (int i = 0; i < _FashionAccessories.Count; i++)
            //{
            //    if (res.FashionId == _FashionAccessories[i].Id)
            //    {
            //        if (res.LockState == 1)
            //        {
            //            _FashionAccessories[i].Unlock = false;
            //        }
            //        if (res.LockState == 2)
            //        {
            //            _FashionAccessories[i].Unlock = true;
            //        }
            //    }
            //}
            //for (int i = 0; i < _FashionWeapons.Count; i++)
            //{
            //    if (res.FashionId == _FashionWeapons[i].Id)
            //    {
            //        if (res.LockState == 1)
            //        {
            //            _FashionWeapons[i].Unlock = false;
            //        }
            //        if (res.LockState == 2)
            //        {
            //            _FashionWeapons[i].Unlock = true;
            //        }
            //    }
            //}
        }

        //时装部件颜色变换通知(当此部件穿戴时,给角色视野列表广播)
        private void OnDyeStateNtf(NetMsg netMsg)
        {
            CmdFashionDyeStateNtf res = NetMsgUtil.Deserialize<CmdFashionDyeStateNtf>(CmdFashionDyeStateNtf.Parser, netMsg);
            //Hero hero = GameCenter.mainWorld.GetActor(Hero.Type, res.RoleId) as Hero;
            Hero hero = GameCenter.GetSceneHero(res.RoleId);
            if (hero != null)
            {
                if (hero.heroBaseComponent != null && hero.heroBaseComponent.fashData != null)
                {
                    if (hero.heroBaseComponent.fashData.ContainsKey(res.FashionId))
                    {
                        hero.heroBaseComponent.fashData[res.FashionId] = CalDressData(res.FashionId, res.DyeScheme, hero.heroBaseComponent.HeroID);
                    }
                }

                hero.ChangeModel();
            }
            else
            {
                Debug.LogErrorFormat("FashionDyeStateNtf not found hero = {0}", res.RoleId);
            }
        }

        //时装部件穿戴更换变换通知(给角色视野列表广播)
        private void OnDressStateNtf(NetMsg netMsg)
        {
            CmdFashionDressStateNtf res = NetMsgUtil.Deserialize<CmdFashionDressStateNtf>(CmdFashionDressStateNtf.Parser, netMsg);
            //Hero hero = GameCenter.mainWorld.GetActor(Hero.Type, res.RoleId) as Hero;
            Hero hero = GameCenter.GetSceneHero(res.RoleId);
            if (hero != null)
            {
                EHeroModelParts eHeroModelParts = parts[res.FashionId];
                //替换
                if (res.Type == 0)
                {
                    if (hero.heroBaseComponent != null && hero.heroBaseComponent.fashData != null)
                    {
                        Dictionary<EHeroModelParts, uint> src = new Dictionary<EHeroModelParts, uint>();
                        foreach (var item in hero.heroBaseComponent.fashData)
                        {
                            EHeroModelParts part = parts[item.Key];
                            src.Add(part, item.Key);
                        }

                        if (src.ContainsKey(eHeroModelParts))
                        {
                            uint srcFashionId = src[eHeroModelParts];
                            hero.heroBaseComponent.fashData.Remove(srcFashionId);
                            hero.heroBaseComponent.fashData[res.FashionId] = CalDressData(res.FashionId, res.DyeScheme, hero.heroBaseComponent.HeroID);
                        }
                        else
                        {
                            hero.heroBaseComponent.fashData[res.FashionId] = CalDressData(res.FashionId, res.DyeScheme, hero.heroBaseComponent.HeroID);
                        }
                    }
                }
                //卸下
                else if (res.Type == 1)
                {
                    if (hero.heroBaseComponent != null && hero.heroBaseComponent.fashData != null)
                    {
                        hero.heroBaseComponent.fashData.Remove(res.FashionId);
                        hero.UnloadModelPart(eHeroModelParts);
                    }
                }

                hero.ChangeModel();
            }
            else
            {
                Debug.LogErrorFormat("OnDressStateNtf not found hero = {0}", res.RoleId);
            }
        }

        //套装穿戴更换通知(当此套装穿戴时,给角色视频列表广播)
        private void OnSuitStateNtf(NetMsg netMsg)
        {
            CmdFashionSuitStateNtf res = NetMsgUtil.Deserialize<CmdFashionSuitStateNtf>(CmdFashionSuitStateNtf.Parser, netMsg);
            //Hero hero = GameCenter.mainWorld.GetActor(Hero.Type, res.RoleId) as Hero;
            Hero hero = GameCenter.GetSceneHero(res.RoleId);
            if (hero != null)
            {
                if (hero.heroBaseComponent != null && hero.heroBaseComponent.fashData != null)
                {
                    //卸载比套装多出来的部位
                    List<EHeroModelParts> src = new List<EHeroModelParts>();
                    foreach (CmdFashionSuitStateNtf.Types.SuitInfo suitInfo in res.SuitInfo)
                    {
                        EHeroModelParts eHeroModelParts = parts[suitInfo.FashionId];
                        src.Add(eHeroModelParts);
                    }

                    foreach (var item in hero.heroBaseComponent.fashData)
                    {
                        EHeroModelParts eHeroModelParts = parts[item.Key];
                        if (!src.Contains(eHeroModelParts))
                        {
                            hero.UnloadModelPart(eHeroModelParts);
                        }
                    }

                    //更新时装数据 加载
                    hero.heroBaseComponent.fashData.Clear();
                    foreach (CmdFashionSuitStateNtf.Types.SuitInfo suitInfo in res.SuitInfo)
                    {
                        hero.heroBaseComponent.fashData[suitInfo.FashionId] = CalDressData(suitInfo.FashionId, suitInfo.DyeScheme, hero.heroBaseComponent.HeroID);
                    }
                }

                hero.ChangeModel();
            }
            else
            {
                Debug.LogErrorFormat("OnSuitStateNtf not found hero = {0}", res.RoleId);
            }
        }

        #endregion

        //穿戴时装请求
        public void DressFashionReq(uint fashionId)
        {
            CmdFashionDressFashionReq cmdFashionDressFashionReq = new CmdFashionDressFashionReq();
            cmdFashionDressFashionReq.OldFashionId = GetOldFashionId(fashionId);
            cmdFashionDressFashionReq.NewFashionId = fashionId;
            NetClient.Instance.SendMessage((ushort) CmdFashion.DressFashionReq, cmdFashionDressFashionReq);
        }

        //穿戴时装回应
        private void DressFashionRes(NetMsg netMsg)
        {
            CmdFashionDressFashionRes res = NetMsgUtil.Deserialize<CmdFashionDressFashionRes>(CmdFashionDressFashionRes.Parser, netMsg);

            DebugUtil.LogFormat(ELogType.eFashion, "DressFashionRes");
            if (res.OldFashionId != 0)
            {
                FashionClothes _fashionClothes = _FashionClothes.Find(x => x.Id == res.OldFashionId);
                if (_fashionClothes != null)
                {
                    _fashionClothes.Dress = false;
                    dressedList[0] = 0;
                }

                FashionAccessory _fashionAccessory = _FashionAccessories.Find(x => x.Id == res.OldFashionId);
                if (_fashionAccessory != null)
                {
                    _fashionAccessory.Dress = false;
                    dressedList[(int) _fashionAccessory.AcceType] = 0;
                }

                FashionWeapon _fashionWeapon = _FashionWeapons.Find(x => x.Id == res.OldFashionId);
                if (_fashionWeapon != null)
                {
                    _fashionWeapon.Dress = false;
                    dressedList[1] = 0;
                }
            }

            bool changeAnim = false;
            FashionClothes fashionClothes = _FashionClothes.Find(x => x.Id == res.NewFashionId);
            if (fashionClothes != null)
            {
                fashionClothes.Dress = true;
                changeAnim = true;
                dressedList[0] = res.NewFashionId;
            }

            FashionAccessory fashionAccessory = _FashionAccessories.Find(x => x.Id == res.NewFashionId);
            if (fashionAccessory != null)
            {
                fashionAccessory.Dress = true;
                dressedList[(int) fashionAccessory.AcceType] = res.NewFashionId;
            }

            FashionWeapon fashionWeapon = _FashionWeapons.Find(x => x.Id == res.NewFashionId);
            if (fashionWeapon != null)
            {
                fashionWeapon.Dress = true;
                changeAnim = true;
                dressedList[1] = res.NewFashionId;
            }

            if (res.SuitId != 0)
            {
                foreach (var item in _FashionSuits)
                {
                    if (item.Id == res.SuitId)
                    {
                        item.Dress = true;
                        //dressedList.AddOnce<uint>(item.cSVFashionSuitData.FashionId);
                        //dressedList.AddOnce<uint>(item.cSVFashionSuitData.WeaponId);
                        dressedList[0] = item.cSVFashionSuitData.FashionId;
                        dressedList[1] = item.cSVFashionSuitData.WeaponId;
                        for (int i = 0; i < item.cSVFashionSuitData.AccId.Count; i++)
                        {
                            uint accid = item.cSVFashionSuitData.AccId[i];
                            FashionAccessory _fashionAccessory = _FashionAccessories.Find(x => x.Id == accid);
                            if (_fashionAccessory != null)
                            {
                                dressedList[(int) _fashionAccessory.AcceType] = accid;
                            }

                            //dressedList.AddOnce<uint>(item.cSVFashionSuitData.AccId[i]);
                        }
                    }
                    else
                    {
                        item.Dress = false;
                    }
                }

                curSuit = res.SuitId;
                curUseSuit = res.UseSuitId;
            }
            else
            {
                foreach (var item in _FashionSuits)
                {
                    item.Dress = false;
                }
            }

            GameCenter.mainHero.heroBaseComponent.fashData = GetDressData();
            GameCenter.mainHero.ChangeModel(changeAnim);

            DebugUtil.LogFormat(ELogType.eFashion, DressListToString());
        }

        //卸下时装请求
        public void FashionUnfixReq(uint fashionId)
        {
            CmdFashionUnfixReq cmdFashionUnfixReq = new CmdFashionUnfixReq();
            cmdFashionUnfixReq.FashionId = fashionId;
            NetClient.Instance.SendMessage((ushort) CmdFashion.UnfixReq, cmdFashionUnfixReq);
        }

        //卸下时装回应
        private void FashionUnfixRes(NetMsg netMsg)
        {
            CmdFashionUnfixRes res = NetMsgUtil.Deserialize<CmdFashionUnfixRes>(CmdFashionUnfixRes.Parser, netMsg);
            EHeroModelParts modelParts = parts[res.FashionId];
            FashionAccessory fashionAccessory = _FashionAccessories.Find(x => x.Id == res.FashionId);
            if (fashionAccessory != null)
            {
                fashionAccessory.Dress = false;
                dressedList[(int) fashionAccessory.AcceType] = 0;
            }

            FashionWeapon fashionWeapon = _FashionWeapons.Find(x => x.Id == res.FashionId);
            if (fashionWeapon != null)
            {
                fashionWeapon.Dress = false;
                dressedList[1] = 0;
            }

            GameCenter.mainHero.UnloadModelPart(modelParts);
            if (res.SuitId == 0)
            {
                foreach (var item in _FashionSuits)
                {
                    item.Dress = false;
                }

                curUseSuit = 0;
                curSuit = res.SuitId;
            }

            GameCenter.mainHero.heroBaseComponent.fashData = GetDressData();
            GameCenter.mainHero.ChangeModel();
            DebugUtil.LogFormat(ELogType.eFashion, DressListToString());
        }

        //时装过期请求
        public void FashionExpireReq(uint fashionId)
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
            {
                _expireFashionInFight.AddOnce<uint>(fashionId);
                return;
            }

            CmdFashionFashionExpireReq cmdFashionFashionExpireReq = new CmdFashionFashionExpireReq();
            cmdFashionFashionExpireReq.FashionId = fashionId;
            NetClient.Instance.SendMessage((ushort) CmdFashion.FashionExpireReq, cmdFashionFashionExpireReq);
        }

        public void OnExitFight()
        {
            for (int i = _expireFashionInFight.Count - 1; i >= 0; --i)
            {
                FashionExpireReq(_expireFashionInFight[i]);
                _expireFashionInFight.RemoveAt(i);
            }
        }

        //时装过期应答
        private void FashionExpireRes(NetMsg netMsg)
        {
            CmdFashionFashionExpireRes res = NetMsgUtil.Deserialize<CmdFashionFashionExpireRes>(CmdFashionFashionExpireRes.Parser, netMsg);
            if (res.Res)
            {
                FashionClothes fashionClothes = _FashionClothes.Find(x => x.Id == res.FashionId);
                FashionWeapon fashionWeapon = _FashionWeapons.Find(x => x.Id == res.FashionId);
                FashionAccessory fashionAccessory = _FashionAccessories.Find(x => x.Id == res.FashionId);
                if (res.ExpireTime == 0)
                {
                    if (fashionClothes != null)
                    {
                        fashionClothes.UnLock = false;
                        RemoveActiveFashionClothes(fashionClothes);
                        if (fashionClothes.Dress)
                        {
                            fashionClothes.Dress = false;
                            EHeroModelParts modelParts = parts[res.FashionId];
                            if (modelParts == EHeroModelParts.Main)
                            {
                                dressedList[0] = GetFirstFashionClothesId();
                                FashionClothes _fashionClothes = _FashionClothes.Find(x => x.Id == dressedList[0]);
                                if (_fashionClothes != null)
                                {
                                    _fashionClothes.Dress = true;
                                }
                            }
                        }
                    }

                    if (fashionWeapon != null)
                    {
                        fashionWeapon.UnLock = false;
                        RemoveActiveFashionWeapon(fashionWeapon);
                        if (fashionWeapon.Dress)
                        {
                            fashionWeapon.Dress = false;
                            dressedList[1] = 0;
                            GameCenter.mainHero.UnloadModelPart(EHeroModelParts.Weapon);
                        }
                    }

                    if (fashionAccessory != null)
                    {
                        fashionAccessory.UnLock = false;
                        RemoveActiveFashionAccessory(fashionAccessory);
                        if (fashionAccessory.Dress)
                        {
                            fashionAccessory.Dress = false;
                            dressedList[(int) fashionAccessory.AcceType] = 0;
                            EHeroModelParts modelParts = parts[res.FashionId];
                            GameCenter.mainHero.UnloadModelPart(modelParts);
                        }
                    }

                    eventEmitter.Trigger<uint>(EEvents.RevertToFirstColor, res.FashionId); //过期之后需要回滚颜色
                }
                else
                {
                    if (fashionClothes != null)
                    {
                        fashionClothes.ReCalTime(res.ExpireTime);
                    }

                    if (fashionWeapon != null)
                    {
                        fashionWeapon.ReCalTime(res.ExpireTime);
                    }

                    if (fashionAccessory != null)
                    {
                        fashionAccessory.ReCalTime(res.ExpireTime);
                    }
                }

                if (res.SuitId == 0)
                {
                    foreach (var item in _FashionSuits)
                    {
                        item.Dress = false;
                    }
                }

                if (res.UseSuitId == 0)
                {
                    Sys_Fashion.Instance.eventEmitter.Trigger<bool>(Sys_Fashion.EEvents.OnUpdateSuitChange, false);
                }
            }

            GameCenter.mainHero.heroBaseComponent.fashData = GetDressData();
            GameCenter.mainHero.ChangeModel();
        }

        //解锁时装Req
        public void UnlockFashionReq(uint fashionId, uint propId, DyeScheme dyeScheme)
        {
            UnlockReq(fashionId, propId, dyeScheme);
        }

        public void BuyPropAndUnlockReq(uint fashionId, uint propId, bool unlock, DyeScheme dyeScheme)
        {
            if (!unlock)
            {
                BuyAndUnlockReq(fashionId, propId, false, dyeScheme);
            }
            else
            {
                BuyAndUnlockReq(fashionId, propId, true, dyeScheme);
            }
        }

        private void UnlockReq(uint fashionId, /*bool recoveryDye,*/ uint propId, DyeScheme dyeScheme)
        {
            CmdFashionUnlockFashionReq cmdFashionUnlockFashionReq = new CmdFashionUnlockFashionReq();
            cmdFashionUnlockFashionReq.FashionId = fashionId;
            //cmdFashionUnlockFashionReq.RecoverDyeValue = recoveryDye;
            cmdFashionUnlockFashionReq.DyeScheme = dyeScheme;
            cmdFashionUnlockFashionReq.PropId = propId;
            NetClient.Instance.SendMessage((ushort) CmdFashion.UnlockFashionReq, cmdFashionUnlockFashionReq);
        }

        private void BuyAndUnlockReq(uint fashionId, /* bool recoveryDye,*/ uint propId, bool isUnlock, DyeScheme dyeScheme)
        {
            CmdFashionBuyPropAndUnlockReq cmdFashionBuyPropAndUnlockReq = new CmdFashionBuyPropAndUnlockReq();
            cmdFashionBuyPropAndUnlockReq.FashionId = fashionId;
            //cmdFashionBuyPropAndUnlockReq.RecoverDyeValue = recoveryDye;
            cmdFashionBuyPropAndUnlockReq.PropId = propId;
            cmdFashionBuyPropAndUnlockReq.IsUnlock = isUnlock;
            cmdFashionBuyPropAndUnlockReq.DyeScheme = dyeScheme;
            NetClient.Instance.SendMessage((ushort) CmdFashion.BuyPropAndUnlockReq, cmdFashionBuyPropAndUnlockReq);
        }

        //解锁时装Res
        private void UnlockFashionRes(NetMsg netMsg)
        {
            CmdFashionUnlockFashionRes cmdFashionDyeFashionRes = NetMsgUtil.Deserialize<CmdFashionUnlockFashionRes>(CmdFashionUnlockFashionRes.Parser, netMsg);
            //bool needDyeColor = false;   //过期染色继承

            for (int i = 0; i < _FashionClothes.Count; i++)
            {
                if (_FashionClothes[i].Id == cmdFashionDyeFashionRes.FashionId)
                {
                    FashionClothes fashionClothes = _FashionClothes[i];
                    fashionClothes.ExpireTime = cmdFashionDyeFashionRes.ExpireTime;
                    fashionClothes.CalTimeOut(cmdFashionDyeFashionRes.ExpireTimeLen);
                    fashionClothes.UnLock = true;
                    fashionClothes.SetCurrentUseScheme((int) cmdFashionDyeFashionRes.UseIndex);
                    SetFashionClothesDyeSheme(fashionClothes, cmdFashionDyeFashionRes.DyeScheme);
                    eventEmitter.Trigger<uint>(EEvents.SetToLastColor, 0);
                }
            }

            for (int i = 0; i < _FashionAccessories.Count; i++)
            {
                if (_FashionAccessories[i].Id == cmdFashionDyeFashionRes.FashionId)
                {
                    FashionAccessory fashionAccessory = _FashionAccessories[i];
                    fashionAccessory.ExpireTime = cmdFashionDyeFashionRes.ExpireTime;
                    fashionAccessory.CalTimeOut(cmdFashionDyeFashionRes.ExpireTimeLen);
                    fashionAccessory.UnLock = true;
                    fashionAccessory.SetCurrentUseScheme((int) cmdFashionDyeFashionRes.UseIndex);
                    SetFashionAcceDyeSheme(fashionAccessory, cmdFashionDyeFashionRes.DyeScheme);
                    eventEmitter.Trigger<uint>(EEvents.SetToLastColor, 1);
                }
            }

            for (int i = 0; i < _FashionWeapons.Count; i++)
            {
                if (_FashionWeapons[i].Id == cmdFashionDyeFashionRes.FashionId)
                {
                    FashionWeapon fashionWeapon = _FashionWeapons[i];
                    fashionWeapon.ExpireTime = cmdFashionDyeFashionRes.ExpireTime;
                    fashionWeapon.CalTimeOut(cmdFashionDyeFashionRes.ExpireTimeLen);
                    fashionWeapon.UnLock = true;
                    fashionWeapon.SetCurrentUseScheme((int) cmdFashionDyeFashionRes.UseIndex);
                    SetFashionWeaponDyeSheme(fashionWeapon, cmdFashionDyeFashionRes.DyeScheme);
                    eventEmitter.Trigger<uint>(EEvents.SetToLastColor, (uint) parts[_FashionWeapons[i].Id]);
                }
            }

            //if (needDyeColor)
            //{
            //    eventEmitter.Trigger(EEvents.RevertToLastColor);
            //}
            eventEmitter.Trigger(EEvents.OnUpdateSuit);
        }

        public void SuitSelReqReq(uint suitid)
        {
            CmdFashionSuitSelReq cmdFashionSuitSelReq = new CmdFashionSuitSelReq();
            cmdFashionSuitSelReq.SuitId = suitid;
            NetClient.Instance.SendMessage((ushort) CmdFashion.SuitSelReq, cmdFashionSuitSelReq);
        }

        private void SuitSelRes(NetMsg netMsg)
        {
            CmdFashionSuitSelRes cmdFashionSuitSelRes = NetMsgUtil.Deserialize<CmdFashionSuitSelRes>(CmdFashionSuitSelRes.Parser, netMsg);
            //dressedList.Clear();
            for (int i = 0; i < dressedList.Length; i++)
            {
                uint fashionId = dressedList[i];
                if (dressedList[i] != 0)
                {
                    EHeroModelParts modelParts = parts[fashionId];
                    if (modelParts == EHeroModelParts.Main)
                    {
                        continue;
                    }

                    FashionAccessory fashionAccessory = _FashionAccessories.Find(x => x.Id == fashionId);
                    if (fashionAccessory != null)
                    {
                        fashionAccessory.Dress = false;
                        dressedList[(int) fashionAccessory.AcceType] = 0;
                    }

                    FashionWeapon fashionWeapon = _FashionWeapons.Find(x => x.Id == fashionId);
                    if (fashionWeapon != null)
                    {
                        fashionWeapon.Dress = false;
                        dressedList[1] = 0;
                    }

                    GameCenter.mainHero.UnloadModelPart(modelParts);
                }

                dressedList[i] = 0;
            }

            foreach (var item in _FashionSuits)
            {
                if (item.Id == cmdFashionSuitSelRes.SuitId)
                {
                    item.Dress = true;
                    dressedList[0] = item.cSVFashionSuitData.FashionId;
                    dressedList[1] = item.cSVFashionSuitData.WeaponId;
                    for (int i = 0; i < item.cSVFashionSuitData.AccId.Count; i++)
                    {
                        uint accid = item.cSVFashionSuitData.AccId[i];
                        FashionAccessory _fashionAccessory = _FashionAccessories.Find(x => x.Id == accid);
                        if (_fashionAccessory != null)
                        {
                            dressedList[(int) _fashionAccessory.AcceType] = accid;
                        }
                    }

                    //dressedList.AddRange(item.cSVFashionSuitData.AccId);
                }
                else
                {
                    item.Dress = false;
                }
            }

            GameCenter.mainHero.heroBaseComponent.fashData = GetDressData();
            GameCenter.mainHero.ChangeModel();
            curSuit = cmdFashionSuitSelRes.SuitId;
            curUseSuit = cmdFashionSuitSelRes.UseSuitId;
            eventEmitter.Trigger<bool>(EEvents.OnUpdateSuitChange, true);
        }

        public void SuitAddrReq(uint suitId, uint useSuitId)
        {
            CmdFashionSuitAddrReq cmdFashionSuitAddrReq = new CmdFashionSuitAddrReq();
            cmdFashionSuitAddrReq.SuitId = suitId;
            cmdFashionSuitAddrReq.UseSuitId = useSuitId;
            NetClient.Instance.SendMessage((ushort) CmdFashion.SuitAddrReq, cmdFashionSuitAddrReq);
        }

        private void SuitAddrRes(NetMsg netMsg)
        {
            CmdFashionSuitAddrRes cmdFashionSuitSelRes = NetMsgUtil.Deserialize<CmdFashionSuitAddrRes>(CmdFashionSuitAddrRes.Parser, netMsg);
            curUseSuit = cmdFashionSuitSelRes.UseSuitId;
            eventEmitter.Trigger<bool>(EEvents.OnUpdateSuitChange, true);
        }

        private void BuyPropAndUnlockRes(NetMsg netMsg)
        {
            CmdFashionBuyPropAndUnlockRes cmdFashionBuyPropAndUnlockRes = NetMsgUtil.Deserialize<CmdFashionBuyPropAndUnlockRes>(CmdFashionBuyPropAndUnlockRes.Parser, netMsg);
        }

        public void AddActiveFashionClothes(FashionClothes fashionClothes)
        {
            _activeFashionClothes.AddOnce<FashionClothes>(fashionClothes);
        }

        public void RemoveActiveFashionClothes(FashionClothes fashionClothes)
        {
            _activeFashionClothes.TryRemove<FashionClothes>(fashionClothes);
        }

        public void AddActiveFashionWeapon(FashionWeapon fashionWeapon)
        {
            _activeFashionWeapons.AddOnce<FashionWeapon>(fashionWeapon);
        }

        public void RemoveActiveFashionWeapon(FashionWeapon fashionWeapon)
        {
            _activeFashionWeapons.TryRemove<FashionWeapon>(fashionWeapon);
        }

        public void AddActiveFashionAccessory(FashionAccessory fashionAccessory)
        {
            _activeFashionAccessories.AddOnce<FashionAccessory>(fashionAccessory);
        }

        public void RemoveActiveFashionAccessory(FashionAccessory fashionAccessory)
        {
            _activeFashionAccessories.TryRemove<FashionAccessory>(fashionAccessory);
        }
    }
}