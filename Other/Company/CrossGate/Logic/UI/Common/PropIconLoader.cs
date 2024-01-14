using System.Collections;
using System.Collections.Generic;
using Lib.AssetLoader;
using UnityEngine.UI;
using UnityEngine;
using System;
using Table;
using UnityEngine.EventSystems;
using Logic.Core;
using Framework;

namespace Logic
{

    public class MessageBoxEvt
    {
        public EUIID sourceUiId;
        public PropIconLoader.ShowItemData itemData;
        public bool b_ForceShowScource;
        public bool b_ShowItemInfo = true;
        public bool b_changeSourcePos = false;
        public Vector3 pos;
        public MessageBoxEvt() { }
        public MessageBoxEvt(EUIID _sourceUiid, PropIconLoader.ShowItemData _itemData)
        {
            Reset(_sourceUiid, _itemData);
        }
        public MessageBoxEvt Reset(EUIID _sourceUiid, PropIconLoader.ShowItemData _itemData)
        {
            sourceUiId = _sourceUiid;
            itemData = _itemData;
            b_ForceShowScource = false;
            b_ShowItemInfo = true;
            return this;
        }
    }

    public static class PropIconLoader
    {
        public class ShowItemDataExt : ShowItemData
        {
            public bool useStack;
            public uint neededCount;

            public ShowItemDataExt() { }
            public ShowItemDataExt(ulong guid, uint id, uint count, uint neededCount, bool useStack)
            {
                this.guid = guid;
                this.id = id;
                this.count = count;
                this.neededCount = neededCount;
                this.useStack = useStack;
            }
        }
        public class ShowItemData : ItemGuidCount
        {
            public bool bUseQuailty;        //是否需要显示品质
            public bool bBind;
            public bool bNew;
            public bool bUnLock;
            public bool bSelected;
            public bool bShowCount;
            public bool bShowBagCount;
            public bool bUseClick;
            public Action<PropItem> onclick;
            public bool bShowBtnNo;
            public bool bUseTips;
            public bool bCopyBagGridCount;
            public int bagGridCount;

            // 背包cell数据
            public ItemData bagData;

            public uint Quality { get; private set; }               //需要数据, item传入
            public bool bMarketedEnd { get; private set; } = true;         //禁售期是否结束 (ture代表结束 不显示锁)

            public long otherCount { get; private set; }        //其他数量 (烹饪 需要显示其他玩家数量)

            public bool bSetOtherCount { get; private set; } = false;   //其他数量(烹饪 需要显示其他玩家数量)

            public int ceilIndex { get;  set; }        //格子对应下标
            public ShowItemData() { }

            public uint EquipPara { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="_id"> infoId  </param>
            /// <param name="_count"> 显示个数(如果是？/？格式 代表右侧得个数) </param>
            /// <param name="_bUseQuailty"> 是否使用品质</param>
            /// <param name="_bBind"> 是否显示绑定标志 </param>
            /// <param name="_bNew"> 是否显示新 标志 </param>
            /// <param name="_bUnLock">是否显示锁标志 </param>
            /// <param name="_bSelected">是否显示选中特效</param>
            /// <param name="_bShowCount">是否显示个数</param>
            /// <param name="_bShowBagCount">是否显示背包中道具个数（一般用于 ()/() 格式）</param>
            /// <param name="_bUseClick">是否可以触发点击</param>
            /// <param name="_onClick">点击回调</param>
            /// <param name="_bshowBtnNo">是否显示+号</param>
            /// <param name="_bUseTips">点击之后是否需要弹tips</param>
            public ShowItemData(uint _id, long _count, bool _bUseQuailty, bool _bBind, bool _bNew, bool _bUnLock, bool _bSelected,
                bool _bShowCount = false, bool _bShowBagCount = false, bool _bUseClick = true, Action<PropItem> _onClick = null, bool _bshowBtnNo = true,
                bool _bUseTips = true)
            {
                id = _id;
                count = _count;
                bUseQuailty = _bUseQuailty;
                bBind = _bBind;
                bNew = _bNew;
                bUnLock = _bUnLock;
                bSelected = _bSelected;
                bShowCount = _bShowCount;
                bShowBagCount = _bShowBagCount;
                bUseClick = _bUseClick;
                onclick = _onClick;
                bShowBtnNo = _bshowBtnNo;
                bUseTips = _bUseTips;
            }

            public ShowItemData Reset()
            {
                bUseQuailty = false;
                bBind = false;
                bNew = false;
                bUnLock = false;
                bSelected = false;
                bShowCount = false;
                bShowBagCount = false;
                bUseClick = false;
                onclick = null;
                bShowBtnNo = false;
                bUseTips = false;
                Quality = 0;
                bMarketedEnd = true;
                return this;
            }

            public ShowItemData Refresh(uint _id, long _count, bool _bUseQuailty, bool _bBind, bool _bNew, bool _bUnLock, bool _bSelected,
                bool _bShowCount = false, bool _bShowBagCount = false, bool _bUseClick = true, Action<PropItem> _onClick = null,
                bool _bshowBtnNo = true, bool _bUseTips = true)
            {

                id = _id;
                count = _count;
                bUseQuailty = _bUseQuailty;
                bBind = _bBind;
                bNew = _bNew;
                bUnLock = _bUnLock;
                bSelected = _bSelected;
                bShowCount = _bShowCount;
                bShowBagCount = _bShowBagCount;
                bUseClick = _bUseClick;
                onclick = _onClick;
                bShowBtnNo = _bshowBtnNo;
                bUseTips = _bUseTips;
                return this;
            }

            public void SetBagData(ItemData itemData)
            {
                bagData = itemData;
                SetQuality(itemData.Quality);
                SetMarketEnd(itemData.bMarketEnd);
            }

            public void SetQuality(uint quality)
            {
                Quality = quality;
            }

            public void SetMarketEnd(bool marketEnd)
            {
                bMarketedEnd = marketEnd;
            }

            public void SetOtherCount(long count)
            {
                bSetOtherCount = true;
                otherCount = count;
            }


            public void SetUnSpawnEquipQuality(uint dropId)
            {
                CSVDrop.Data cSVDropData = CSVDrop.Instance.GetDropItemData(dropId);
                if (cSVDropData != null)
                {
                    uint equipParmId = cSVDropData.equip_para;
                    CSVEquipmentParameter.Data cSVEquipmentParameterData = CSVEquipmentParameter.Instance.GetConfData(equipParmId);
                    if (cSVEquipmentParameterData != null)
                    {
                        equipPara = equipParmId;
                        int maxQualityWeight = 0;
                        int maxQuality = 0;
                        for (int i = 0; i < cSVEquipmentParameterData.quality_weight.Count; i++)
                        {
                            if (maxQualityWeight < (int)cSVEquipmentParameterData.quality_weight[i])
                            {
                                maxQualityWeight = (int)cSVEquipmentParameterData.quality_weight[i];
                                maxQuality = i + 1;
                            }
                        }
                        if (maxQualityWeight > 0)
                        {
                            SetQuality((uint)maxQuality);
                        }
                    }
                }
            }

            public void SetUnSpawnEquipQualityByEquipParamId(uint equipParmId)
            {
                CSVEquipmentParameter.Data cSVEquipmentParameterData = CSVEquipmentParameter.Instance.GetConfData(equipParmId);
                if (cSVEquipmentParameterData != null)
                {
                    equipPara = equipParmId;
                    int maxQualityWeight = 0;
                    int maxQuality = 0;
                    for (int i = 0; i < cSVEquipmentParameterData.quality_weight.Count; i++)
                    {
                        if (maxQualityWeight < (int)cSVEquipmentParameterData.quality_weight[i])
                        {
                            maxQualityWeight = (int)cSVEquipmentParameterData.quality_weight[i];
                            maxQuality = i + 1;
                        }
                    }
                    if (maxQualityWeight > 0)
                    {
                        SetQuality((uint)maxQuality);
                    }
                }

            }
        }


        public static PropItem GetAsset(ShowItemData _itemData, Transform parent)
        {
            return GetAsset(_itemData, parent, EUIID.Invalid);
        }

        public static PropItem GetAsset(ShowItemData _itemData, Transform parent, EUIID uIID)
        {
            GameObject obj = GlobalAssets.GetAsset<GameObject>(GlobalAssets.sPrefab_PropIcon);
            GameObject clone = GameObject.Instantiate<GameObject>(obj);
            clone.transform.SetParent(parent);
            clone.transform.localScale = Vector3.one;
            clone.transform.localPosition = Vector3.zero;

            PropItem propIconWrap = new PropItem();
            propIconWrap.BindGameObject(clone);
            propIconWrap.SetData(_itemData, uIID);
            return propIconWrap;
        }

    }



}


