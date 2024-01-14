using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.EventSystems;
using Net;
using Packet;

namespace Logic
{
    public class UI_Knowledge_RecipeCooking : UIBase
    {
        private CP_ToggleRegistry m_CP_ToggleRegistry;
        private Transform m_ItemParent;
        private List<ItemContent> m_ItemContents = new List<ItemContent>();
        private int m_CurSelectLab;
        private int m_CurSelectIndex;
        private Button m_CloseButton;

        private Image m_ItemIcon;
        private Text m_ItemName;
        private Text m_ItemDes;

        private Text m_Line0;
        private Text m_Line1;
        private Text m_Line2;
        private Button m_Button0;
        private Button m_Button1;
        private Button m_Button2;
        private GameObject m_Con0;
        private GameObject m_Con1;
        private GameObject m_Con2;

        private uint m_SkipToItemId;
        private CSVItem.Data m_CurrentItemData;
        private CSVFoodAtlas.Data m_CurrentFoodAtlasData;

        protected override void OnInit()
        {
            m_CurSelectLab = 0;
            m_CurSelectIndex = 0;
        }

        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                m_SkipToItemId = (uint)arg;
                if (m_SkipToItemId != 0)
                {
                    if (Sys_Bag.Instance.meat.IndexOf(m_SkipToItemId) > -1)
                    {
                        m_CurSelectLab = 1;
                        m_CurSelectIndex = Sys_Bag.Instance.meat.IndexOf(m_SkipToItemId);
                    }
                    else if (Sys_Bag.Instance.seaFood.IndexOf(m_SkipToItemId) > -1)
                    {
                        m_CurSelectLab = 2;
                        m_CurSelectIndex = Sys_Bag.Instance.seaFood.IndexOf(m_SkipToItemId);
                    }
                    else if (Sys_Bag.Instance.fruit.IndexOf(m_SkipToItemId) > -1)
                    {
                        m_CurSelectLab = 3;
                        m_CurSelectIndex = Sys_Bag.Instance.fruit.IndexOf(m_SkipToItemId);
                    }
                    else if (Sys_Bag.Instance.ingredients.IndexOf(m_SkipToItemId) > -1)
                    {
                        m_CurSelectLab = 4;
                        m_CurSelectIndex = Sys_Bag.Instance.ingredients.IndexOf(m_SkipToItemId);
                    }
                }
            }
        }

        protected override void OnClose()
        {
            m_SkipToItemId = 0;
        }

        protected override void OnLoaded()
        {
            m_CP_ToggleRegistry = transform.Find("Animator/Bottom/Image_BG/Image_BG (2)/Left/Toggle_Tab").GetComponent<CP_ToggleRegistry>();
            m_ItemParent = transform.Find("Animator/Bottom/Image_BG (1)/Center/Scroll View/Viewport");
            m_CP_ToggleRegistry.onToggleChange = OnToggleChanged;
            m_CloseButton = transform.Find("Animator/View_Title07/Btn_Close").GetComponent<Button>();
            m_ItemIcon = transform.Find("Animator/Bottom/Image_BG (1)/Right/Item/Image_icon_bg/Image_icon").GetComponent<Image>();
            m_ItemName = transform.Find("Animator/Bottom/Image_BG (1)/Right/Item/Text").GetComponent<Text>();
            m_ItemDes = transform.Find("Animator/Bottom/Image_BG (1)/Right/Text_Detail").GetComponent<Text>();

            m_Line0 = transform.Find("Animator/Bottom/Image_BG (1)/Right/Line0/Text").GetComponent<Text>();
            m_Line1 = transform.Find("Animator/Bottom/Image_BG (1)/Right/Line1/Text").GetComponent<Text>();
            m_Line2 = transform.Find("Animator/Bottom/Image_BG (1)/Right/Line2/Text").GetComponent<Text>();

            m_Button0 = transform.Find("Animator/Bottom/Image_BG (1)/Right/Line0/Button").GetComponent<Button>();
            m_Button1 = transform.Find("Animator/Bottom/Image_BG (1)/Right/Line1/Button").GetComponent<Button>();
            m_Button2 = transform.Find("Animator/Bottom/Image_BG (1)/Right/Line2/Button").GetComponent<Button>();

            m_Con0 = transform.Find("Animator/Bottom/Image_BG (1)/Right/Line0").gameObject;
            m_Con1 = transform.Find("Animator/Bottom/Image_BG (1)/Right/Line1").gameObject;
            m_Con2 = transform.Find("Animator/Bottom/Image_BG (1)/Right/Line2").gameObject;

            m_Button0.onClick.AddListener(OnButton0Clicked);
            m_Button1.onClick.AddListener(OnButton1Clicked);
            m_Button2.onClick.AddListener(OnButton2Clicked);

            for (int i = 0, count = m_ItemParent.childCount; i < count; i++)
            {
                ItemContent itemContent = new ItemContent();
                itemContent.BindGameObject(m_ItemParent.GetChild(i).gameObject);
                itemContent.AddEvent(OnGridSelected);
                itemContent.index = i + 1;
                m_ItemContents.Add(itemContent);
            }
            m_ItemContents[0].SetData(Sys_Bag.Instance.meat);
            m_ItemContents[1].SetData(Sys_Bag.Instance.seaFood);
            m_ItemContents[2].SetData(Sys_Bag.Instance.fruit);
            m_ItemContents[3].SetData(Sys_Bag.Instance.ingredients);

            FrameworkTool.ForceRebuildLayout(m_ItemParent.gameObject);

            m_CloseButton.onClick.AddListener(OnCloseButtonClicked);
        }

        protected override void OnShow()
        {
            m_CP_ToggleRegistry.SwitchTo(m_CurSelectLab);
            OnInitGridSelect(m_CurSelectLab, m_CurSelectIndex);
        }

        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Knowledge_RecipeCooking);
        }

        private void OnToggleChanged(int current, int old)
        {
            if (current == old)
            {
                return;
            }
            m_CurSelectLab = current;
            UpdateContent(current);
            if (m_SkipToItemId == 0)
            {
                m_CurSelectIndex = 0;
            }
            OnInitGridSelect(m_CurSelectLab, m_CurSelectIndex);
        }

        private void UpdateContent(int lab)
        {
            if (lab == 0)
            {
                for (int i = 0, count = m_ItemContents.Count; i < count; i++)
                {
                    m_ItemContents[i].SetActive(true);
                }
            }
            else
            {
                for (int i = 0, count = m_ItemContents.Count; i < count; i++)
                {
                    if (i == lab - 1)
                    {
                        m_ItemContents[i].SetActive(true);
                    }
                    else
                    {
                        m_ItemContents[i].SetActive(false);
                    }
                }
            }
            FrameworkTool.ForceRebuildLayout(m_ItemParent.gameObject);
        }

        private void OnInitGridSelect(int contentIndex, int gridIndex)
        {
            int real = 0;
            if (contentIndex > 0)
            {
                real = contentIndex - 1;
            }
            for (int i = 0; i < m_ItemContents.Count; i++)
            {
                if (real != i)
                {
                    m_ItemContents[i].ReleaseAll();
                }
                else
                {
                    m_ItemContents[i].Select(gridIndex);
                    RefreshRightView(m_ItemContents[i].gridWarps[gridIndex].itemId);
                }
            }
            m_SkipToItemId = 0;
        }

        private void OnGridSelected(int contentIndex, int gridIndex)
        {
            if (contentIndex == m_CurSelectLab && gridIndex == m_CurSelectIndex)
            {
                return;
            }

            int real = contentIndex - 1;

            m_CurSelectIndex = gridIndex;
            for (int i = 0; i < m_ItemContents.Count; i++)
            {
                if (real != i)
                {
                    m_ItemContents[i].ReleaseAll();
                }
                else
                {
                    m_ItemContents[i].Select(gridIndex);
                    RefreshRightView(m_ItemContents[i].gridWarps[gridIndex].itemId);
                }
            }
        }

        private void RefreshRightView(uint itemId)
        {
            m_CurrentItemData = CSVItem.Instance.GetConfData(itemId);
            TextHelper.SetText(m_ItemName, m_CurrentItemData.name_id);
            ImageHelper.SetIcon(m_ItemIcon, m_CurrentItemData.icon_id);
            TextHelper.SetText(m_ItemDes, m_CurrentItemData.describe_id);

            m_CurrentFoodAtlasData = CSVFoodAtlas.Instance.GetConfData(itemId);
            if (m_CurrentFoodAtlasData != null)
            {
                if (m_CurrentFoodAtlasData.access1 == 0)
                {
                    TextHelper.SetText(m_Line0, string.Empty);
                    m_Con0.gameObject.SetActive(false);
                }
                else if (m_CurrentFoodAtlasData.access1 == 1)//寻找npc  //寻找npc并且打开商店界面
                {
                    m_Con0.gameObject.SetActive(true);
                    m_Button0.gameObject.SetActive(true);
                    bool active = false;
                    for (int i = 0; i < m_CurrentFoodAtlasData.parameter1[0].Count; i++)
                    {
                        uint npcId = m_CurrentFoodAtlasData.parameter1[0][i];
                        if (Sys_Npc.Instance.IsActivatedNpc(npcId))
                        {
                            active = true;
                            break;
                        }
                    }
                    if (!active)
                    {
                        TextHelper.SetText(m_Line0, 1003097);//问号
                        ButtonHelper.Enable(m_Button0, false);
                    }
                    else
                    {
                        TextHelper.SetText(m_Line0, m_CurrentFoodAtlasData.desc1);
                        ButtonHelper.Enable(m_Button0, true);
                    }
                }
                else if (m_CurrentFoodAtlasData.access1 == 2)
                {
                    m_Con0.gameObject.SetActive(true);
                    m_Button0.gameObject.SetActive(true);
                    ButtonHelper.Enable(m_Button0, true);
                    TextHelper.SetText(m_Line0, m_CurrentFoodAtlasData.desc1);
                }
                else if (m_CurrentFoodAtlasData.access1 == 3)
                {
                    m_Con0.gameObject.SetActive(true);
                    TextHelper.SetText(m_Line0, m_CurrentFoodAtlasData.desc1);
                    ButtonHelper.Enable(m_Button0, false);
                    m_Button0.gameObject.SetActive(false);
                }

                //------------------------------------------------------------------------------//

                if (m_CurrentFoodAtlasData.access2 == 0)
                {
                    TextHelper.SetText(m_Line1, string.Empty);
                    m_Con1.gameObject.SetActive(false);
                }
                else if (m_CurrentFoodAtlasData.access2 == 1)//寻找npc  
                {
                    m_Con1.gameObject.SetActive(true);
                    m_Button1.gameObject.SetActive(true);

                    bool active = false;
                    for (int i = 0; i < m_CurrentFoodAtlasData.parameter2[0].Count; i++)
                    {
                        uint npcId = m_CurrentFoodAtlasData.parameter2[0][i];
                        if (Sys_Npc.Instance.IsActivatedNpc(npcId))
                        {
                            active = true;
                            break;
                        }
                    }
                    if (!active)
                    {
                        TextHelper.SetText(m_Line1, 1003097);//问号
                        ButtonHelper.Enable(m_Button1, false);
                    }
                    else
                    {
                        TextHelper.SetText(m_Line1, m_CurrentFoodAtlasData.desc2);
                        ButtonHelper.Enable(m_Button1, true);
                    }
                }
                else if (m_CurrentFoodAtlasData.access2 == 2)//寻找npc并且打开商店界面
                {
                    m_Con1.gameObject.SetActive(true);
                    m_Button1.gameObject.SetActive(true);
                    TextHelper.SetText(m_Line1, m_CurrentFoodAtlasData.desc2);
                    ButtonHelper.Enable(m_Button1, true);
                }
                else if (m_CurrentFoodAtlasData.access2 == 3)
                {
                    m_Con1.gameObject.SetActive(true);
                    m_Button1.gameObject.SetActive(false);
                    TextHelper.SetText(m_Line1, m_CurrentFoodAtlasData.desc2);
                    ButtonHelper.Enable(m_Button1, false);
                }

                //------------------------------------------------------------------------------//

                if (m_CurrentFoodAtlasData.access3 == 0)
                {
                    TextHelper.SetText(m_Line2, string.Empty);
                    m_Con2.gameObject.SetActive(false);
                }
                else if (m_CurrentFoodAtlasData.access3 == 1)//寻找npc  
                {
                    m_Con2.gameObject.SetActive(true);
                    m_Button2.gameObject.SetActive(true);
                    bool active = false;
                    for (int i = 0; i < m_CurrentFoodAtlasData.parameter3[0].Count; i++)
                    {
                        uint npcId = m_CurrentFoodAtlasData.parameter3[0][i];
                        if (Sys_Npc.Instance.IsActivatedNpc(npcId))
                        {
                            active = true;
                            break;
                        }
                    }
                    if (!active)
                    {
                        TextHelper.SetText(m_Line2, 1003097);//问号
                        ButtonHelper.Enable(m_Button2, false);
                    }
                    else
                    {
                        TextHelper.SetText(m_Line2, m_CurrentFoodAtlasData.desc3);
                        ButtonHelper.Enable(m_Button2, true);
                    }
                }
                else if (m_CurrentFoodAtlasData.access3 == 2)//寻找npc并且打开商店界面
                {
                    m_Con2.gameObject.SetActive(true);
                    m_Button2.gameObject.SetActive(true);
                    ButtonHelper.Enable(m_Button2, true);
                    TextHelper.SetText(m_Line2, m_CurrentFoodAtlasData.desc3);
                }
                else if (m_CurrentFoodAtlasData.access3 == 3)
                {
                    m_Con2.gameObject.SetActive(true);
                    m_Button2.gameObject.SetActive(false);
                    TextHelper.SetText(m_Line2, m_CurrentFoodAtlasData.desc3);
                    ButtonHelper.Enable(m_Button2, false);
                }
            }
        }

        private void OnButton0Clicked()
        {
            if (m_CurrentFoodAtlasData != null && m_CurrentItemData != null)
            {
                if (m_CurrentFoodAtlasData.access1 == 1)
                {
                    ActionCtrl.Instance.MoveToTargetNPC(m_CurrentFoodAtlasData.parameter1[1][0]);
                }
                else if (m_CurrentFoodAtlasData.access1 == 2)
                {
                    //Sys_Mall.Instance.skip2MallFromItemSource = new MallPrama();
                    //Sys_Mall.Instance.skip2MallFromItemSource.mallId = m_CurrentFoodAtlasData.parameter1[0];
                    //Sys_Mall.Instance.skip2MallFromItemSource.shopId = m_CurrentFoodAtlasData.parameter1[1];
                    //Sys_Mall.Instance.skip2MallFromItemSource.itemId = m_CurrentItemData.id;
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(m_CurrentFoodAtlasData.parameter1[2][0]);
                }
                UIManager.CloseUI(EUIID.UI_Knowledge_RecipeCooking);
                if (UIManager.IsOpen(EUIID.UI_Cooking_Collect))
                {
                    UIManager.CloseUI(EUIID.UI_Cooking_Collect);
                }
                if (UIManager.IsOpen(EUIID.UI_Knowledge_Main))
                {
                    UIManager.CloseUI(EUIID.UI_Knowledge_Main);
                }
            }
        }

        private void OnButton1Clicked()
        {
            if (m_CurrentFoodAtlasData != null && m_CurrentItemData != null)
            {
                if (m_CurrentFoodAtlasData.access2 == 1)
                {
                    ActionCtrl.Instance.MoveToTargetNPC(m_CurrentFoodAtlasData.parameter2[1][0]);
                }
                else if (m_CurrentFoodAtlasData.access2 == 2)
                {
                    //Sys_Mall.Instance.skip2MallFromItemSource = new MallPrama();
                    //Sys_Mall.Instance.skip2MallFromItemSource.mallId = m_CurrentFoodAtlasData.parameter2[0];
                    //Sys_Mall.Instance.skip2MallFromItemSource.shopId = m_CurrentFoodAtlasData.parameter2[1];
                    //Sys_Mall.Instance.skip2MallFromItemSource.itemId = m_CurrentItemData.id;
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(m_CurrentFoodAtlasData.parameter2[2][0]);
                }
                UIManager.CloseUI(EUIID.UI_Knowledge_RecipeCooking);
                if (UIManager.IsOpen(EUIID.UI_Cooking_Collect))
                {
                    UIManager.CloseUI(EUIID.UI_Cooking_Collect);
                }
                if (UIManager.IsOpen(EUIID.UI_Knowledge_Main))
                {
                    UIManager.CloseUI(EUIID.UI_Knowledge_Main);
                }
            }
        }

        private void OnButton2Clicked()
        {
            if (m_CurrentFoodAtlasData != null && m_CurrentItemData != null)
            {
                if (m_CurrentFoodAtlasData.access3 == 1)
                {
                    ActionCtrl.Instance.MoveToTargetNPC(m_CurrentFoodAtlasData.parameter3[1][0]);
                }
                else if (m_CurrentFoodAtlasData.access3 == 2)
                {
                    //Sys_Mall.Instance.skip2MallFromItemSource = new MallPrama();
                    //Sys_Mall.Instance.skip2MallFromItemSource.mallId = m_CurrentFoodAtlasData.parameter3[0];
                    //Sys_Mall.Instance.skip2MallFromItemSource.shopId = m_CurrentFoodAtlasData.parameter3[1];
                    //Sys_Mall.Instance.skip2MallFromItemSource.itemId = m_CurrentItemData.id;
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(m_CurrentFoodAtlasData.parameter3[2][0]);
                }
                UIManager.CloseUI(EUIID.UI_Knowledge_RecipeCooking);
                if (UIManager.IsOpen(EUIID.UI_Cooking_Collect))
                {
                    UIManager.CloseUI(EUIID.UI_Cooking_Collect);
                }
                if (UIManager.IsOpen(EUIID.UI_Knowledge_Main))
                {
                    UIManager.CloseUI(EUIID.UI_Knowledge_Main);
                }
            }
        }

        public class ItemContent
        {
            private GameObject m_Go;
            private Transform m_ContentParent;
            public int index;
            public List<GridWarp> gridWarps = new List<GridWarp>();
            private Action<int, int> m_Selection;

            public void BindGameObject(GameObject gameObject)
            {
                m_Go = gameObject;

                m_ContentParent = m_Go.transform.Find("View_Item");
            }

            public void AddEvent(Action<int, int> action)
            {
                m_Selection = action;
            }

            public void SetData(List<uint> itemIds)
            {
                FrameworkTool.CreateChildList(m_ContentParent, itemIds.Count);
                for (int i = 0; i < itemIds.Count; i++)
                {
                    GridWarp gridWarp = new GridWarp();
                    GameObject gameObject = m_ContentParent.GetChild(i).gameObject;
                    gridWarp.BindGameObject(gameObject);
                    gridWarp.SetData(index, itemIds[i], i);
                    gridWarp.AddEvent(m_Selection);
                    gridWarps.Add(gridWarp);
                }
            }

            public void Select(int index)
            {
                for (int i = 0; i < gridWarps.Count; i++)
                {
                    if (index == i)
                    {
                        gridWarps[i].Select();
                    }
                    else
                    {
                        gridWarps[i].Release();
                    }
                }
            }


            public void ReleaseAll()
            {
                for (int i = 0; i < gridWarps.Count; i++)
                {
                    gridWarps[i].Release();
                }
            }

            public void SetActive(bool flag)
            {
                m_Go.SetActive(flag);
            }
        }

        public class GridWarp
        {
            public int ownerContent;
            public uint itemId;
            public int dataIndex;

            private Image m_Icon;
            private Text m_Name;
            public GameObject gameObject;
            private Button m_ClickButton;
            private Action<int, int> m_SelectAction;
            private GameObject m_Select;


            public void BindGameObject(GameObject @object)
            {
                gameObject = @object;
                ParseCp();
            }

            private void ParseCp()
            {
                m_ClickButton = gameObject.transform.Find("Image_bottom").GetComponent<Button>();
                m_Icon = gameObject.transform.Find("Image_icon_bg/Image_icon").GetComponent<Image>();
                m_Name = gameObject.transform.Find("Text").GetComponent<Text>();
                m_Select = gameObject.transform.Find("Image").gameObject;
                m_ClickButton.onClick.AddListener(OnButtonClicked);
            }

            public void SetData(int ownerContent, uint itemId, int dataIndex)
            {
                this.ownerContent = ownerContent;
                this.itemId = itemId;
                this.dataIndex = dataIndex;
                Refresh();
            }

            public void Refresh()
            {
                TextHelper.SetText(m_Name, CSVItem.Instance.GetConfData(itemId).name_id);
                ImageHelper.SetIcon(m_Icon, CSVItem.Instance.GetConfData(itemId).icon_id);
            }

            public void AddEvent(Action<int, int> action)
            {
                m_SelectAction = action;
            }

            private void OnButtonClicked()
            {
                m_SelectAction.Invoke(ownerContent, dataIndex);
            }

            public void Release()
            {
                if (m_Select.activeSelf)
                {
                    m_Select.SetActive(false);
                }
            }

            public void Select()
            {
                m_Select.SetActive(true);
            }
        }
    }
}


