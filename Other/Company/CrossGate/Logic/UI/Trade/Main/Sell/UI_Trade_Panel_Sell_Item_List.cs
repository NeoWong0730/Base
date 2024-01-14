using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;
using Lib.Core;

namespace Logic
{
    public class UI_Trade_Panel_Sell_Item_List
    {
        private class SellItemCell
        {
            private Transform transform;

            private PropSpecialItem m_PropItem;
            private Image m_ImgPetBattle;
            private Image m_ImgPetRide;
            public GameObject goDomestication_0;
            public GameObject goDomestication_1;

            private Sys_Trade.TradeItemInfo m_itemInfo;
            private ItemData m_Item;
            private CSVCommodity.Data _CommodityData;

            public void Init(Transform trans)
            {
                transform = trans;

                m_PropItem = new PropSpecialItem();
                m_PropItem.BindGameObject(transform.gameObject);

                Transform parent = transform.Find("Grid");
                parent.gameObject.SetActive(true);
                m_ImgPetBattle = transform.Find("Grid/Image_Battle").GetComponent<Image>();
                m_ImgPetRide = transform.Find("Grid/Image_Ride").GetComponent<Image>();

                goDomestication_0 = transform.Find("Image_Domestication_0").gameObject;
                goDomestication_1 = transform.Find("Image_Domestication_1").gameObject;
            }

            private bool IsCanSale()
            {
                if (null == _CommodityData)
                    return false;

                if (m_Item.cSVItemData.type_id == (int)EItemType.PetEquipment) //元核
                {
                    if (m_Item.petEquip.Color < _CommodityData.quality_range[0]
                        || m_Item.petEquip.Color > _CommodityData.quality_range[1])
                    {
                        return false;
                    }
                }
                else if (m_Item.cSVItemData.type_id == (int)EItemType.Equipment) //装备
                {
                    if (m_Item.Quality < _CommodityData.quality_range[0]
                        || m_Item.Quality > _CommodityData.quality_range[1])
                    {
                        return false;
                    }
                }
                else if (m_Item.cSVItemData.type_id == (int)EItemType.Pet)
                {
                    if (!m_Item.cSVItemData.on_sale) //不可交易
                    {
                        return false;
                    }

                    if (Sys_Pet.Instance.fightPet!=null&& m_Item.Pet.Uid == Sys_Pet.Instance.fightPet.GetUid()) //出战宠物
                    {
                        return false;
                    }

                    if (Sys_Pet.Instance.mountPetUid == m_Item.Pet.Uid) //骑乘
                        return false;

                    if (Sys_Pet.Instance.followPetUid == m_Item.Pet.Uid) //跟随
                        return false;

                    int petQuality = Sys_Pet.Instance.GetPetQuality(m_Item.Pet);
                    //判断品质
                    if (petQuality < _CommodityData.quality_range[0]
                        || petQuality > _CommodityData.quality_range[1])
                    {
                        return false;
                    }

                    //判断是否在派遣中
                    if (Sys_PetExpediton.Instance.CheckPetIsUnderway(m_Item.Pet.Uid))
                    {
                        return false;
                    }
                    
                    //判断世界等级
                    if (_CommodityData.world_level <= Sys_Role.Instance.GetWorldLv())
                    {
                        return false;
                    }
                }

                return true;
            }

            public void UpdateInfo(Sys_Trade.TradeItemInfo itemInfo)
            {
                m_itemInfo = itemInfo;
                m_Item = Sys_Trade.Instance.GetItemDataByTradeItemInfo(itemInfo);
                _CommodityData = CSVCommodity.Instance.GetConfData(m_itemInfo.infoID);

                PropSpeicalLoader.ShowItemData showItem = new PropSpeicalLoader.ShowItemData(m_Item, m_Item.Count > 1, false, false);
                showItem.AddClickAction(OnClick);
                if (m_Item.Pet != null)
                    showItem.SetLevel(m_Item.Pet.SimpleInfo.Level);
                m_PropItem.SetData(new PropSpeicalLoader.MessageBoxEvt(EUIID.UI_Trade, showItem));

                ImageHelper.SetImageGray(m_PropItem.Layout.imgIcon, !IsCanSale());

                m_ImgPetBattle.gameObject.SetActive(m_Item.Pet != null && Sys_Pet.Instance.fightPet.IsSamePet(m_Item.Pet));
                m_ImgPetRide.gameObject.SetActive(m_Item.Pet != null && Sys_Pet.Instance.mountPetUid == m_Item.Pet.Uid);

                goDomestication_0.SetActive(false);
                goDomestication_1.SetActive(false);

                if (_CommodityData != null)
                {
                    //宠物驯化判断
                    if (_CommodityData.type == 2u)
                    {
                        CSVPetNew.Data petNew = CSVPetNew.Instance.GetConfData(m_Item.Pet.SimpleInfo.PetId);
                        if (petNew != null && petNew.mount)
                        {
                            goDomestication_0.SetActive(m_Item.Pet.SimpleInfo.MountDomestication == 0);
                            goDomestication_1.SetActive(m_Item.Pet.SimpleInfo.MountDomestication == 1);
                        }
                    }
                }
                else
                {
                    //DebugUtil.LogErrorFormat("交易行商品 需配置 {0}", m_Item.Id);
                }
            }

            private void OnClick(PropSpecialItem propItem)
            {
                m_Item = Sys_Trade.Instance.GetItemDataByTradeItemInfo(m_itemInfo);

                if (m_Item.cSVItemData.type_id == (int) EItemType.PetEquipment) //元核
                {
                    if (_CommodityData != null)
                    {
                        if (m_Item.petEquip.Color < _CommodityData.quality_range[0]
                            || m_Item.petEquip.Color > _CommodityData.quality_range[1])
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011295));
                            return;
                        }
                    }
                }
                else if (m_Item.cSVItemData.type_id == (int)EItemType.Equipment) //装备需要
                {
                    if (_CommodityData != null)
                    {
                        if (m_Item.Quality < _CommodityData.quality_range[0]
                            || m_Item.Quality > _CommodityData.quality_range[1])
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011224));
                            return;
                        }
                    }
                }
                else if (m_Item.cSVItemData.type_id == (int)EItemType.Pet)
                {
                    if (_CommodityData != null)
                    {
                        //如果只有一只宠物无法上架
                        if (Sys_Pet.Instance.petsList.Count <= 1)
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011233));
                            return;
                        }

                        if (!m_Item.cSVItemData.on_sale) //不可交易
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011151));
                            return;
                        }

                        if (Sys_Pet.Instance.fightPet.IsSamePet(m_Item.Pet)) //出战宠物
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011152));
                            return;
                        }

                        //骑乘宠物
                        if (Sys_Pet.Instance.mountPetUid == m_Item.Pet.Uid)
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011261));
                            return;
                        }
                        
                        //跟随宠物
                        if (Sys_Pet.Instance.followPetUid == m_Item.Pet.Uid)
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011262));
                            return;
                        }
                        
                        //装备元核宠物
                        ClientPet pet = new ClientPet(m_Item.Pet);
                        if (pet != null && pet.IsHasEquip())
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011296));
                            return;
                        }
                        
                        //装配魂珠的宠物
                        if (Sys_Pet.Instance.HasEquipDemonSpiritSphere(m_Item.Pet))
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002038));
                            return;
                        }
                        
                        //驯养中，无法上架
                        if (Sys_PetDomesticate.Instance.CheckPetIsDomesticating(m_Item.Pet.Uid))
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2052046));
                            return;
                        }
                        
                        //在其他坐骑契约位上的宠物
                        if (pet != null && pet.HasPartnerPet())
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011297));
                            return;
                        }

                        int petQuality = Sys_Pet.Instance.GetPetQuality(m_Item.Pet);
                        if (petQuality < _CommodityData.quality_range[0] 
                            || petQuality > _CommodityData.quality_range[1])
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011223));
                            return;
                        }
                        //派遣中
                        if (Sys_PetExpediton.Instance.CheckPetIsUnderway(m_Item.Pet.Uid))
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2025653));
                            return;
                        }
                        
                        //世界等级
                        if (_CommodityData.world_level <= Sys_Role.Instance.GetWorldLv())
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011907, _CommodityData.world_level.ToString()));
                            return;
                        }
                    }
                    else
                    {
                        //未配置，不可交易
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011151));
                        return;
                    }
                }

                Sys_Trade.Instance.OnSaleItemCheck(m_itemInfo);
            }
        }

        private Transform transform;

        private InfinityGrid _infinityGrid;
        private Lib.Core.CoroutineHandler handler;
        //private Dictionary<GameObject, SellItemCell> dicCells = new Dictionary<GameObject, SellItemCell>();

        private List<Sys_Trade.TradeItemInfo> m_ItemsList = new List<Sys_Trade.TradeItemInfo>();

        public bool IsShow = false;

        public void Init(Transform trans)
        {
            transform = trans;

            _infinityGrid = transform.GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
            //gridGroup = transform.Find("TabList").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            //gridGroup.minAmount = 24;
            //gridGroup.updateChildrenCallback = UpdateChildrenCallback;

            //for (int i = 0; i < gridGroup.transform.childCount; ++i)
            //{
            //    Transform tran = gridGroup.transform.GetChild(i);
            //    SellItemCell cell = new SellItemCell();
            //    cell.Init(tran);
            //    dicCells.Add(tran.gameObject, cell);
            //}
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
            IsShow = true;
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
            IsShow = false;
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            SellItemCell entry = new SellItemCell();

            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);

            //dicCells.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            SellItemCell entry = cell.mUserData as SellItemCell;
            entry.UpdateInfo(m_ItemsList[index]);
        }

        //private void UpdateChildrenCallback(int index, Transform trans)
        //{
        //    if (index < 0 || index >= visualGridCount)
        //        return;

        //    if (dicCells.ContainsKey(trans.gameObject))
        //    {
        //        SellItemCell cell = dicCells[trans.gameObject];
        //        cell.UpdateInfo(m_ItemsList[index]);
        //    }
        //}

        public void UpdateList(List<Sys_Trade.TradeItemInfo> itemList)
        {
            m_ItemsList = itemList;

            _infinityGrid.CellCount = m_ItemsList.Count;
            _infinityGrid.ForceRefreshActiveCell();
            _infinityGrid.MoveToIndex(0);
            //visualGridCount = m_ItemsList.Count;
            //gridGroup.SetAmount(visualGridCount);
        }
    }
}


