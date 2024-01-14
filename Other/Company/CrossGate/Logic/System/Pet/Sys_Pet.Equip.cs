using Google.Protobuf;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using System.Json;
using Table;
using UnityEngine;
namespace Logic
{
    public partial class Sys_Pet : SystemModuleBase<Sys_Pet>
    {
        private Dictionary<uint, List<uint>> dicEquip = new Dictionary<uint, List<uint>>();
        public bool isSmeltSkillTips = false;
        public bool isSmeltFashionTips = false;
        public List<uint> GetPetEquipsByType(uint type)
        {
            if(dicEquip.TryGetValue(type, out List<uint> equipList))
            {
                return equipList;
            }
            else
            {
                equipList = new List<uint>(4);
                var configs = CSVPetEquip.Instance.GetAll();
                for (int i = 0,len = configs.Count; i < len; i++)
                {
                    var config = configs[i];
                    if (config.equipment_category == type)
                    {
                        equipList.Add(config.id);
                    }
                }
                return equipList;
            }
        }
        
        /// <summary>
        /// 通过宠物装备id 获得对应的展示属性
        /// </summary>
        /// <param name="petEquipId"></param>
        /// <returns></returns>
        public List<List<uint>> GetPetEquipPreviewAttrIdById(uint petEquipId)
        {
            List<List<uint>> vs = new List<List<uint>>();
            var petEquipData = CSVPetEquip.Instance.GetConfData(petEquipId);
            if(null != petEquipData)
            {
                var configs = CSVPetEquipAttr.Instance.GetAll();
                for (int i = 0, len = configs.Count; i < len; i++)
                {
                    var config = configs[i];
                    if (petEquipData.attr_id == config.group_id && config.isShow == 1)
                    {
                        var lst = new List<uint>(3);
                        lst.Add(config.attr_id);
                        lst.Add(config.min_attr);
                        lst.Add(config.max_attr);
                        vs.Add(lst);
                    }
                }
            }
            else
            {
                DebugUtil.LogError($"Not Find Id = {petEquipId} In CSVPetEquip");
            }

            return vs;
        }

        /// <summary>
        /// 制作宠物装备
        /// </summary>
        /// <param name="itemId">道具表id</param>
        /// <param name="forgeSpecialId">使用的特殊材料(指改造图纸)</param>
        public void ItemBuildPetEquipReq(uint itemId, uint forgeSpecialId)
        {
            CmdItemBuildPetEquipReq req = new CmdItemBuildPetEquipReq();
            req.ItemId = itemId;
            req.ForgeSpecial = forgeSpecialId;
            NetClient.Instance.SendMessage((ushort)CmdItem.BuildPetEquipReq, req);
        }

       
        private void OnItemBuildPetEquipRes(NetMsg msg)
        {
            CmdItemBuildPetEquipRes dataRes = NetMsgUtil.Deserialize<CmdItemBuildPetEquipRes>(CmdItemBuildPetEquipRes.Parser, msg);
            eventEmitter.Trigger(EEvents.OnPetEquipMakeSuccess,dataRes.Uuid);
        }

        /// <summary>
        /// 炼化宠物装备套装
        /// </summary>
        /// <param name="uuid">炼化宠物装备的uuid</param>
        /// <param name="petUid">穿着该装备的宠物uid(不在宠物身上不用填)</param>
        /// <param name="smeltItemId">炼化道具id</param>
        public void ItemSmeltPetEquipReq(ulong uuid, uint petUid, uint smeltItemId)
        {
            CmdItemSmeltPetEquipReq req = new CmdItemSmeltPetEquipReq();
            req.Uuid = uuid;
            req.PetUid = petUid;
            req.SmeltItemId = smeltItemId;
            NetClient.Instance.SendMessage((ushort)CmdItem.SmeltPetEquipReq, req);
        }

        private void OnItemSmeltPetEquipRes(NetMsg msg)
        {
            CmdItemSmeltPetEquipRes dataRes = NetMsgUtil.Deserialize<CmdItemSmeltPetEquipRes>(CmdItemSmeltPetEquipRes.Parser, msg);
            ulong uuid = dataRes.Uuid;
            uint petUid = dataRes.PetUid;
            Item item = dataRes.Item;
            var clientPet = GetPetByUId(petUid);
            if (null != clientPet)
            {
                var petEquip = CSVPetEquip.Instance.GetConfData(item.Id);
                if(null != petEquip)
                {
                    clientPet.SetPetEquip(petEquip.equipment_category, item);
                }
            }
            if(null != item)
            {
                ShowPetEquipSmeltAttrChange(item);
            }

            if (petUid == followPetUid)
            {
                ClientPet simplePet = GetFightPetClient(petUid);
                if (null != simplePet)
                {
                    GameCenter.mainHero.AddPet(simplePet.GetFollowPetInfo(), Sys_Role.Instance.RoleId * 1000000 + simplePet.petData.id * 10 + 2, GetFollowPetSuitFashionId(), (uint)GetFollowPerfectRemakeCount(), IsFollowPetShowDemonSpiritFx());
                }
            }
            else if (petUid == mountPetUid)
            {
                SimplePet simplePet = GetSimplePetByUid(petUid);
                if (null != simplePet)
                {
                    GameCenter.mainHero.animationComponent.UpdateHoldingAnimations(GameCenter.mainHero.heroBaseComponent.HeroID, GameCenter.mainHero.weaponComponent.CurWeaponID, CSVActionState.Instance.GetHeroPreLoadActions(), GameCenter.mainHero.stateComponent.CurrentState, GameCenter.mainHero.modelGameObject);

                    if (CheckMountGradeIsFullByUid(mountPetUid))
                    {
                        GameCenter.mainHero.OnMount(simplePet.PetId * 10 + 1, Sys_Role.Instance.RoleId * 1000000 + simplePet.PetId * 10 + 1, GetMountPetSuitFashionId(), (uint)GetMountPerfectRemakeCount(), IsMountPetShowDemonSpiritFx());
                    }
                    else
                    {
                        GameCenter.mainHero.OnMount(simplePet.PetId * 10, Sys_Role.Instance.RoleId * 1000000 + simplePet.PetId * 10 + 1, GetMountPetSuitFashionId(), (uint)GetMountPerfectRemakeCount(), IsMountPetShowDemonSpiritFx());
                    }
                }
            }


            eventEmitter.Trigger(EEvents.OnSmeltPetEquipEnd);
        }

        /// <summary>
        /// 装备 卸下 替换 宠物装备
        /// </summary>
        /// <param name="equipSlot">要穿戴的位置(这个目前等于元核类型)</param>
        /// <param name="uuid">要穿的装备的uuid   如果uuid=0, 则是卸下, 如果本来有穿戴, 则是替换</param>
        /// <param name="petUid">操作的宠物uid</param>
        public void ItemFitPetEquipReq(uint equipSlot, ulong uuid, uint petUid)
        {
            CmdItemFitPetEquipReq req = new CmdItemFitPetEquipReq();
            req.Uuid = uuid;
            req.EquipSlot = equipSlot;
            req.PetUid = petUid;
            NetClient.Instance.SendMessage((ushort)CmdItem.FitPetEquipReq, req);
        }

        private void OnItemFitPetEquipRes(NetMsg msg)
        {
            CmdItemFitPetEquipRes dataRes = NetMsgUtil.Deserialize<CmdItemFitPetEquipRes>(CmdItemFitPetEquipRes.Parser, msg);
            uint equipSlot = dataRes.EquipSlot;
            uint petUid = dataRes.PetUid;
            Item item = dataRes.Item;
            var clientPet = GetPetByUId(petUid);
            if(null != clientPet)
            {
                clientPet.SetPetEquip(equipSlot, item);
            }
            if(clientPet.CheckHasSuitSkill(out uint i))
            {
                UIManager.OpenUI(EUIID.UI_PetMagicCore_ActivateResult, false, new Tuple<uint, uint>(item.PetEquip.SuitSkill, clientPet.GetPetSuitFashionId() > 0 ? item.PetEquip.SuitAppearance : 0));
            }

            if (petUid == followPetUid)
            {
                ClientPet simplePet = GetFightPetClient(petUid);
                if (null != simplePet)
                {
                    GameCenter.mainHero.AddPet(simplePet.GetFollowPetInfo(), Sys_Role.Instance.RoleId * 1000000 + simplePet.petData.id * 10 + 2, GetFollowPetSuitFashionId(), (uint)GetFollowPerfectRemakeCount(), IsFollowPetShowDemonSpiritFx());
                }
            }
            else if (petUid == mountPetUid)
            {
                SimplePet simplePet = GetSimplePetByUid(petUid);
                if (null != simplePet)
                {
                    GameCenter.mainHero.animationComponent.UpdateHoldingAnimations(GameCenter.mainHero.heroBaseComponent.HeroID, GameCenter.mainHero.weaponComponent.CurWeaponID, CSVActionState.Instance.GetHeroPreLoadActions(), GameCenter.mainHero.stateComponent.CurrentState, GameCenter.mainHero.modelGameObject);

                    if (CheckMountGradeIsFullByUid(mountPetUid))
                    {
                        GameCenter.mainHero.OnMount(simplePet.PetId * 10 + 1, Sys_Role.Instance.RoleId * 1000000 + simplePet.PetId * 10 + 1, GetMountPetSuitFashionId(), (uint)GetMountPerfectRemakeCount(), IsMountPetShowDemonSpiritFx());
                    }
                    else
                    {
                        GameCenter.mainHero.OnMount(simplePet.PetId * 10, Sys_Role.Instance.RoleId * 1000000 + simplePet.PetId * 10 + 1, GetMountPetSuitFashionId(), (uint)GetMountPerfectRemakeCount(), IsMountPetShowDemonSpiritFx());
                    }
                }
            }
            eventEmitter.Trigger(EEvents.OnUpdatePetInfo);
        }

        /// <summary>
        /// 分解宠物装备
        /// </summary>
        /// <param name="uuid"></param>
        public void ItemDecomposePetEquipReq(ulong uuid)
        {
            CmdItemDecomposePetEquipReq req = new CmdItemDecomposePetEquipReq();
            req.Uuid = uuid;
            NetClient.Instance.SendMessage((ushort)CmdItem.DecomposePetEquipReq, req);
        }

        private void OnItemDecomposePetEquipRes(NetMsg msg)
        {
            CmdItemDecomposePetEquipRes dataRes = NetMsgUtil.Deserialize<CmdItemDecomposePetEquipRes>(CmdItemDecomposePetEquipRes.Parser, msg);
        }


        #region Util

        /// <summary>
        /// 返回可以炼化的宠物装备
        /// </summary>
        /// <returns></returns>
        public List<ItemData> GetSmeltPetEquips()
        {
            List<ItemData> itemDatas = new List<ItemData>(32);
            var items = Sys_Bag.Instance.GetItemDatasByItemType((uint)EItemType.PetEquipment, new List<Func<ItemData, bool>>
                        {
                            (_item) =>
                            {
                                return _item.petEquip.SuitSkill != 0;
                            }
                        });
            itemDatas.AddRange(items);
            items.Clear();
            for (int i = 0; i < petsList.Count; i++)
            {
                var petItems = petsList[i].GetPetSuitSkillEquipItem();
                if(null != petItems)
                {
                    for (int j = 0; j < petItems.Count; j++)
                    {
                        var kv = petItems[j];
                        ItemData itemData = new ItemData();
                        itemData.SetData(0, kv.Uuid, kv.Id, kv.Count, kv.Position, kv.ShowNewIcon, kv.Bind, kv.Equipment, kv.Essence, kv.Marketendtime, null, kv.Crystal, kv.Ornament, kv.PetEquip);
                        itemData.outTime = kv.OutTime;
                        itemDatas.Add(itemData);
                    }
                }
            }
            return itemDatas;
        }

        public uint GetFollowPetSuitFashionId()
        {
            return GetPetFashionIdByUid(followPetUid); 
        }

        public uint GetMountPetSuitFashionId()
        {
            return GetPetFashionIdByUid(mountPetUid);
        }

        public uint GetPetFashionIdByUid(uint uid)
        {
            var clientPet = GetPetByUId(uid);
            if (null != clientPet)
            {
                return clientPet.GetPetSuitFashionId();
            }
            return 0;
        }

        public bool IsEquiped(ulong itemUid, uint petUid)
        {
            var clientPet = GetPetByUId(petUid);
            if (null != clientPet)
            {
                return clientPet.PetEquipIsFitInPet(itemUid);
                
            }
            return false;
        }

        public bool IsShowCompare(uint infoId,uint petUid, ref ItemData _resultItem)
        {
            bool isCompare = false;

            CSVPetEquip.Data equipData = CSVPetEquip.Instance.GetConfData(infoId);
            if (equipData != null)
            {
                var clientPet = GetPetByUId(petUid);
                if (null != clientPet)
                {
                    ItemData result = clientPet.GetPetEquipByType(equipData.equipment_category);
                    if (result != null)
                    {
                        _resultItem = result;
                        isCompare = true;
                    }
                }
            }

            return isCompare;
        }

        /// <summary>
        /// 获取拥有套装技能装备所属宠物uid
        /// </summary>
        /// <returns></returns>
        public uint GetPetEquipFitPetUid(ulong uuid)
        {
            for (int i = 0; i < petsList.Count; i++)
            {
                var petItems = petsList[i].GetPetSuitSkillEquipItem();
                if (null != petItems)
                {
                    for (int j = 0; j < petItems.Count; j++)
                    {
                        var item = petItems[j];
                        if(uuid == item.Uuid)
                        {
                            return petsList[i].GetPetUid();
                        }
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// 获取装备是否已装备在宠物上
        /// </summary>
        /// <returns></returns>
        public bool IsEquip(ulong uuid)
        {
            for (int i = 0; i < petsList.Count; i++)
            {
                var petItems = petsList[i].GetPetEquipItems();
                if (null != petItems)
                {
                    for (int j = 0; j < petItems.Count; j++)
                    {
                        var item = petItems[j];
                        if (uuid == item.Uuid)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void ShowPetEquipSmeltAttrChange(Item item)
        {
            if (!UIManager.IsOpen(EUIID.UI_PetMagicCore_ArtificeResult))
            {
                UIManager.OpenUI(EUIID.UI_PetMagicCore_ArtificeResult, false, item);
            }
            else
            {
                eventEmitter.Trigger(EEvents.OnSmeltItemShow, item);
            }
        }

        /// <summary>
        /// 宠物装备是否受安全锁影响
        /// </summary>
        /// <returns></returns>
        public bool IsPetEquipmentSecureLock(ItemData pet, bool jumpTips = true)
        {
            if (pet == null || pet.petEquip == null)
                return false;
            if (Sys_SecureLock.Instance.lockState && pet.petEquip.Color >= Constants.PetPurpleNum)
            {
                if(jumpTips)
                    Sys_SecureLock.Instance.JumpToSecureLock();
                return true;
            }
            return false;
        }
        #endregion
    }
}
