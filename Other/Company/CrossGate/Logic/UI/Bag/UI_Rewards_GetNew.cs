using System.Collections;
using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Table;
using Random = UnityEngine.Random;

namespace Logic
{

    public class UI_Rewards_GetNew : UIBase
    {
        public class ItemFxBagParms
        {
            public List<uint> itemIds = new List<uint>();
            public List<uint> itemCounts = new List<uint>();
        }
        private List<uint> m_ItemIds = new List<uint>();
        private List<uint> m_ItemCounts = new List<uint>();
        private CoroutineHandler m_handler;

        private Button btn_Close;
        private Button btn_Sure;
        private InfinityGrid m_InfintyGrid;
        private GameObject go_FxPanel;
        private GameObject go_FxItem;
        private RectTransform panelTrans;
        private static int fxCount=6;
        private int nowFxCount;
        private bool canClose;
        protected override void OnOpen(object arg)
        {
            if (arg!=null)
            {
                ItemFxBagParms itemParms = arg as ItemFxBagParms;
                m_ItemIds = itemParms.itemIds;
                m_ItemCounts = itemParms.itemCounts;
            }
        }

        protected override void OnLoaded()
        {
            btn_Close = transform.Find("Animator/Common/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
            btn_Sure = transform.Find("Animator/Common/Btn_01").GetComponent<Button>();
            m_InfintyGrid = transform.Find("Animator/Common/Image_bg03/Rect").GetComponent<InfinityGrid>();
            go_FxPanel = transform.Find("Animator/FX_UIFireWork").gameObject;
            go_FxItem = transform.Find("Animator/FX_UIFireWork/01").gameObject;
            panelTrans = transform.Find("Animator/Common/View_TipsBg02_Big").GetComponent<RectTransform>();
            m_InfintyGrid.onCreateCell = OnCellCreate;
            m_InfintyGrid.onCellChange = OnCellChange;
            btn_Close.onClick.AddListener(OnButtonCloseClicked);
            btn_Sure.onClick.AddListener(OnButtonSureClicked);
            canClose = false;
            nowFxCount = 1;
        }

        protected override void OnShow()
        {
            FxGameObejctShow();
            SetReward(m_ItemIds.Count);
        }
        protected override void OnHide()
        {
            FrameworkTool.DestroyChildren(go_FxPanel, go_FxItem.transform.name);
        }
        private void SetReward(int count)
        {
            m_InfintyGrid.CellCount = count;
            m_InfintyGrid.ForceRefreshActiveCell();
            m_InfintyGrid.MoveToIndex(0);

        }
        private void FxGameObejctShow()
        {
            go_FxPanel.SetActive(true);
            //go_FxItem.transform.position = RandomPosition();
            if (m_handler != null)
            {
                CoroutineManager.Instance.Stop(m_handler);
                m_handler = null;
            }
            CoroutineManager.Instance.StartHandler(ShowFxItem());

        }

        private IEnumerator ShowFxItem()
        {
            while(nowFxCount<=fxCount)
            {
                FxItemInit(nowFxCount%4);
                nowFxCount++;
                yield return new WaitForSeconds(0.25f);
            }
            canClose = true;
        }

        private void FxItemInit(int _index)
        {
            GameObject temp_go= FrameworkTool.CreateGameObject(go_FxItem, go_FxPanel);
            temp_go.transform.position = RandomPosition(_index);
        }
        private Vector3 RandomPosition(int _index)
        {
            Vector2 s_vec = new Vector2();
            switch (_index)
            {
                case 1:
                    s_vec = new Vector2(Random.Range(Screen.width / 25, 12 * Screen.width / 25), Random.Range(14 * Screen.height / 25, 24 * Screen.height / 25));
                    break;
                case 2:
                    s_vec = new Vector2(Random.Range(14 * Screen.width / 25, 24 * Screen.width / 25), Random.Range(14 * Screen.height / 25, 24 * Screen.height / 25));
                    break;
                case 3:
                    s_vec = new Vector2(Random.Range(14 * Screen.width / 25, 24 * Screen.width / 25), Random.Range(Screen.height / 25, 12 * Screen.height / 25));
                    break;
                case 0:
                    s_vec = new Vector2(Random.Range(Screen.width / 25, 12 * Screen.width / 25), Random.Range(Screen.height / 25, 12 * Screen.height / 25));
                    break;
                default:
                    break;
            }
            RectTransformUtility.ScreenPointToWorldPointInRectangle(panelTrans, s_vec, CameraManager.mUICamera, out Vector3 pos);
            return pos;
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            FxRawardItem item = cell.mUserData as FxRawardItem;
            if (item != null && m_ItemIds.Count > index)
                item.SetItem(m_ItemIds[index],m_ItemCounts[index]);
        }

        private void OnCellCreate(InfinityGridCell cell)
        {
            FxRawardItem rawardItem = new FxRawardItem();

            rawardItem.Load(cell.mRootTransform);

            cell.BindUserData(rawardItem);
        }
        private void OnButtonCloseClicked()
        {
            if (canClose)
            {
                UIManager.CloseUI(EUIID.UI_Rewards_GetNew);
            }
            
        }
        private void OnButtonSureClicked()
        {
            if (canClose)
            {
                UIManager.CloseUI(EUIID.UI_Bag);
                UIManager.CloseUI(EUIID.UI_Rewards_GetNew);
            }

        }

        public class FxRawardItem : IntClickItem
        {
            PropItem m_Item;

            PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, true, false);

            public override void Load(Transform root)
            {
                base.Load(root);

                m_Item = new PropItem();

                m_Item.BindGameObject(root.gameObject);
            }

            public override ClickItem Clone()
            {
                return Clone<FxRawardItem>(this);
            }

            public void SetItem(uint itemId, uint itemCount)
            {
                m_ItemData.id = itemId;
                m_ItemData.count = itemCount;

                m_Item.SetData(new MessageBoxEvt(EUIID.UI_Rewards_GetNew, m_ItemData));
            }
        }

    }

}
