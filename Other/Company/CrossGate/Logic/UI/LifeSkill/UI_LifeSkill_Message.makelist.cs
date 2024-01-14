using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using UnityEngine.UI;
using Lib.Core;
using DG.Tweening;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using UnityEngine.EventSystems;

namespace Logic
{
    // makelist
    public partial class UI_LifeSkill_Message : UIBase
    {
        private Toggle m_MakeListToggle;
        private Toggle m_fenjieToggle;
        private Button m_MakeListButton;
        private List<ulong> m_MakeLists = new List<ulong>();
        private List<ulong> m_MakeListWillRemove = new List<ulong>();
        private GameObject m_ViewList;
        private Image m_ViewListClose;
        private InfinityGrid m_InfinityGrid;
        private Dictionary<GameObject, EquipGrid> m_EquipGrids = new Dictionary<GameObject, EquipGrid>();
        private bool b_ViewListOpen = false;
        private GameObject m_FlyObj;
        private Image m_FlyObjIcon;

        private void OnCreateCell(InfinityGridCell cell)
        {
            EquipGrid entry = new EquipGrid();
            entry.BindGameObject(cell.mRootTransform.gameObject);
            entry.AddEventListener(OnGridSelected, ActiveRedPoint);
            cell.BindUserData(entry);
            m_EquipGrids.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            EquipGrid entry = cell.mUserData as EquipGrid;
            entry.SetData(m_MakeLists[index]);
        }

        private void OnGridSelected(ulong uuid)
        {
            m_MakeListWillRemove.AddOnce<ulong>(uuid);
        }

        private bool ActiveRedPoint(ulong uuid)
        {
            return !m_MakeListWillRemove.Contains(uuid);
        }

        private void OnMakeListButtonClicked()
        {
            ClearBuff();
            if (m_MakeLists.Count == 1)
            {
                ulong equidId = m_MakeLists[0];
                ItemData itemData = Sys_Bag.Instance.GetItemDataByUuid(equidId);
                if (itemData != null)
                {
                    EquipTipsData tipData = new EquipTipsData();
                    tipData.equip = itemData;
                    UIManager.OpenUI(EUIID.UI_TipsEquipment, false, tipData);
                }
            }
            else
            {
                m_ViewList.SetActive(true);
                b_ViewListOpen = true;
                ForceRefreshViewList();
            }
        }

        private void OnCloseEquipTips(ulong uuid)
        {
            m_MakeListWillRemove.AddOnce(uuid);
        }

        private void OnViewListClose(BaseEventData baseEventData)
        {
            ClearBuff();
            m_ViewList.SetActive(false);
            b_ViewListOpen = false;
        }

        private void OnNtfDecomposeItem(ulong uuid)
        {
            if (m_MakeLists.Remove(uuid))
            {
                m_MakeListWillRemove.Remove(uuid);
                if (m_MakeLists.Count == 0)
                {
                    m_MakeListButton.gameObject.SetActive(false);
                }
                ForceRefreshViewList();
            }
        }

        private void ClearBuff()
        {
            if (m_MakeLists.Count > m_MakeListWillRemove.Count)
            {
                for (int i = m_MakeListWillRemove.Count - 1; i >= 0; --i)
                {
                    m_MakeLists.Remove(m_MakeListWillRemove[i]);
                    m_MakeListWillRemove.RemoveAt(i);
                }
            }
            else
            {
                for (int i = m_MakeListWillRemove.Count - 1; i >= 1; --i)
                {
                    m_MakeLists.Remove(m_MakeListWillRemove[i]);
                    m_MakeListWillRemove.RemoveAt(i);
                }
            }
        }

        private void ForceRefreshViewList()
        {
            if (!b_ViewListOpen)
            {
                return;
            }
            m_InfinityGrid.CellCount = m_MakeLists.Count;
            m_InfinityGrid.ForceRefreshActiveCell();
        }

        private void OnMakeSuccess(ulong equipId)
        {
            isMaking = false;
            RemoveAllButtonListeners();
            if (cSVFormulaData.forge_type == 1)
            {
                int needForgeNum = (int)cSVFormulaData.normal_forge.Count;
                SetCostRoot(needForgeNum, true, cSVFormulaData.can_intensify, cSVFormulaData.can_harden, true, true);
            }
            else if (cSVFormulaData.forge_type == 2)
            {
                int needForgeNum = (int)cSVFormulaData.forge_num;
                SetCostRoot(needForgeNum, false, false, false, false);
            }
            if (equipId != 0)
            {
                if (cSVFormulaData.isequipment)
                {
                    m_MakeLists.Add(equipId);
                    if (!m_MakeListButton.gameObject.activeSelf)
                    {
                        m_MakeListButton.gameObject.SetActive(true);
                    }
                    PerformFly();
                    ForceRefreshViewList();
                }
            }
            
            if (m_MakeListToggle.isOn)
            {
                if (CanMake())
                {
                    makeFun = false;
                    UpdateMakeButton();
                    TryMake();
                }
                else
                {
                    makeFun = true;
                    UpdateMakeButton();
                }
            }
            else
            {
                makeFun = true;
                UpdateMakeButton();
            }

            bool canDecompose = false;
            if (m_fenjieToggle.isOn) {
                canDecompose = TryDecompose(equipId);
            }
        }

        private bool TryDecompose(ulong equipId) {
            ItemData itemData = Sys_Bag.Instance.GetItemDataByUuid(equipId);
            if (itemData == null) {
                return false;
            }

            //镶嵌宝石
            // if (Sys_Equip.Instance.IsInlayJewel(itemData)) {
            //     // Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4211u));
            //     return false;
            // }

            //品质提示
            if (itemData.Quality >= Sys_Equip.Instance.QualityLimit) {
                return false;
            }

            // //安全锁
            // if (Sys_Equip.Instance.IsSecureLock(itemData)) {
            //     return false;
            // }

            Sys_Equip.Instance.OnEquipmentDecomposeReq(itemData.Uuid);
            return true;
        }

        private void PerformFly()
        {
            m_FlyObj.transform.position = furmulaIcon.transform.position;
            ImageHelper.SetIcon(m_FlyObjIcon, CSVItem.Instance.GetConfData(cSVFormulaData.view_item).icon_id);
            m_FlyObj.transform.rotation = Quaternion.identity;
            m_FlyObj.SetActive(true);
            m_FlyObj.transform.DOMove(m_MakeListButton.transform.position, 1).onComplete += PlayOver;
            m_FlyObj.transform.DOLocalRotate(new Vector3(0, 0, 1200), 1, RotateMode.FastBeyond360);
        }

        private void PlayOver()
        {
            m_FlyObj.SetActive(false);
            m_FlyObj.transform.position = furmulaIcon.transform.position;
            m_FlyObj.transform.rotation = Quaternion.identity;
        }


        public class EquipGrid
        {
            private GameObject m_Go;
            private GameObject m_RedPoint;
            private Button m_ClickButton;
            private Image m_Icon;
            private Image m_Quality;
            private Text m_ItemName;
            private ItemData m_ItemData;
            private Action<ulong> m_OnClicked;
            private Func<ulong, bool> m_ActiveRedPoint;

            public void BindGameObject(GameObject gameObject)
            {
                m_Go = gameObject;

                m_RedPoint = m_Go.transform.Find("Image_Red").gameObject;
                m_Icon = m_Go.transform.Find("PropItem/Btn_Item/Image_Icon").GetComponent<Image>();
                m_ItemName = m_Go.transform.Find("PropItem/Text_Name").GetComponent<Text>();
                m_Quality = m_Go.transform.Find("PropItem/Btn_Item/Image_BG").GetComponent<Image>();
                m_ClickButton = m_Go.transform.Find("PropItem/Btn_Item").GetComponent<Button>();

                m_ClickButton.onClick.AddListener(OnClicked);
            }
            public void AddEventListener(Action<ulong> action, Func<ulong, bool> activeRed)
            {
                m_OnClicked = action;
                m_ActiveRedPoint = activeRed;
            }

            public void OnClicked()
            {
                EquipTipsData tipData = new EquipTipsData();
                tipData.equip = m_ItemData;
                UIManager.OpenUI(EUIID.UI_TipsEquipment, false, tipData);

                m_OnClicked?.Invoke(m_ItemData.Uuid);
                m_RedPoint.SetActive(m_ActiveRedPoint(m_ItemData.Uuid));
            }


            public void SetData(ulong equipId)
            {
                m_ItemData = Sys_Bag.Instance.GetItemDataByUuid(equipId);

                ImageHelper.SetIcon(m_Icon, m_ItemData.cSVItemData.icon_id);
                ImageHelper.GetQualityColor_Frame(m_Quality, (int)m_ItemData.Quality);
                TextHelper.SetText(m_ItemName, m_ItemData.cSVItemData.name_id);
                m_RedPoint.SetActive(m_ActiveRedPoint(equipId));
            }
        }
    }
}

