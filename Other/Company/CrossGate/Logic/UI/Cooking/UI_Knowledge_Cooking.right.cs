using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.EventSystems;
using Net;

namespace Logic
{
    public partial class UI_Knowledge_Cooking : UIBase
    {
        private Text m_EntryName;
        private Text m_EntryDesc;
        private Image m_EntryIcon;
        private Button m_CollectionButton;
        private Button m_WatchButton;
        private GameObject m_WatchStateObj;
        private Transform m_SubmitParent;
        private Button m_CloseButton;
        private List<SubmitGrid> m_SubmitGrids = new List<SubmitGrid>();
        private int curSelectSubmitIndex = 0;

        private Button m_ActiveButton;
        private GameObject m_ActiveRedPoint;
        private GameObject m_ActiveNode;
        private GameObject m_InActiveNode;
        private Transform m_ContentParent;

        private Button m_GoCookingButton;
        private uint m_DefultCookingFun;
        private Dictionary<GameObject, PropItem> m_Props = new Dictionary<GameObject, PropItem>();

        private void RegisterRight()
        {
            m_ActiveNode = transform.Find("Animator/Bottom/Image_BG (1)/Right/Activated").gameObject;
            m_InActiveNode = transform.Find("Animator/Bottom/Image_BG (1)/Right/Unactivated").gameObject;
            m_ActiveButton = transform.Find("Animator/Bottom/Image_BG (1)/Right/Unactivated/Btn_01").GetComponent<Button>();
            m_ActiveRedPoint = transform.Find("Animator/Bottom/Image_BG (1)/Right/Unactivated/Btn_01/Image_Red").gameObject;
            m_ContentParent = transform.Find("Animator/Bottom/Image_BG (1)/Right/Activated/Scroll View/Viewport/Content");

            m_EntryName = transform.Find("Animator/Bottom/Image_BG (1)/Right/Item/Text").GetComponent<Text>();
            m_EntryDesc = transform.Find("Animator/Bottom/Image_BG (1)/Right/Text_Detail").GetComponent<Text>();
            m_EntryIcon = transform.Find("Animator/Bottom/Image_BG (1)/Right/Item/Image_icon_bg/Image_icon").GetComponent<Image>();
            m_CollectionButton = transform.Find("Animator/Bottom/Image_BG (1)/Right/Btn_01").GetComponent<Button>();
            m_SubmitParent = transform.Find("Animator/Bottom/Image_BG (1)/Right/Grid");
            m_CloseButton = transform.Find("Animator/View_Title07/Btn_Close").GetComponent<Button>();
            m_WatchButton = transform.Find("Animator/Bottom/Image_BG (1)/Right/Image_Collect_bg").GetComponent<Button>();
            m_WatchStateObj = m_WatchButton.transform.Find("Image_Collect").gameObject;
            m_GoCookingButton= transform.Find("Animator/Bottom/Image_BG (1)/Right/Btn_01_Small").GetComponent<Button>();
            m_CloseButton.onClick.AddListener(OnCloseButtonClicked);
            m_CollectionButton.onClick.AddListener(OnCollectionButtonClicked);
            m_WatchButton.onClick.AddListener(OnWatchButtonClick);
            m_ActiveButton.onClick.AddListener(OnActiveButtonClick);
            m_GoCookingButton.onClick.AddListener(OnGoCookingButtonClick);
            for (int i = 1; i < m_SubmitParent.childCount; i++)
            {
                GameObject gameObject = m_SubmitParent.GetChild(i).gameObject;
                SubmitGrid submitGrid = new SubmitGrid();
                submitGrid.BindGameObject(gameObject);
                submitGrid.AddEvent(OnSubmitGridClicked, OnSubmitGridLongPressed);
                submitGrid.SetIndex(i);
                m_SubmitGrids.Add(submitGrid);
            }
            m_DefultCookingFun = CSVCookAttr.Instance.GetConfData(12).value;
        }

        private void RefreshRightView()
        {
            TextHelper.SetText(m_EntryName, m_CurSelectCooking.cSVCookData.name);
            TextHelper.SetText(m_EntryDesc, m_CurSelectCooking.cSVCookData.desc);
            ImageHelper.SetIcon(m_EntryIcon, m_CurSelectCooking.cSVCookData.icon);
            OnUpdateSubmitState();
            ShowSubmitSelect();
            RefreshWatchButton();
        }

        private void RefreshActiveNode()
        {
            if (m_CurSelectCooking.active == 1)
            {
                m_ActiveNode.SetActive(true);
                m_InActiveNode.SetActive(false);
                
                int stageCount = (int)m_CurSelectCooking.cookType;
                FrameworkTool.CreateChildList(m_ContentParent, stageCount);

                for (int i = 0; i < stageCount; i++)
                {
                    Transform child = m_ContentParent.GetChild(i);

                    Text tool = child.Find("Text1/Text").GetComponent<Text>();

                    string toolName = string.Empty;

                    List<List<uint>> foods = new List<List<uint>>();

                    if (i == 0)
                    {
                        toolName = Sys_Cooking.Instance.GetToolName(m_CurSelectCooking.cSVCookData.tool1);
                        foods = m_CurSelectCooking.cSVCookData.food1;
                    }
                    else if (i == 1)
                    {
                        toolName = Sys_Cooking.Instance.GetToolName(m_CurSelectCooking.cSVCookData.tool2);
                        foods = m_CurSelectCooking.cSVCookData.food2;
                    }
                    else
                    {
                        toolName = Sys_Cooking.Instance.GetToolName(m_CurSelectCooking.cSVCookData.tool3);
                        foods = m_CurSelectCooking.cSVCookData.food3;
                    }
                    TextHelper.SetText(tool, toolName);

                    Transform propParent = child.Find("Grid");
                    int needCount = foods.Count;

                    FrameworkTool.CreateChildList(propParent, needCount);
                    for (int j = 0; j < needCount; j++)
                    {
                        GameObject propGo = propParent.GetChild(j).gameObject;
                        if (!m_Props.TryGetValue(propGo, out PropItem propItem))
                        {
                            propItem = new PropItem();
                            m_Props.Add(propGo, propItem);
                        }

                        uint itemId = foods[j][0];
                        uint itemCount = foods[j][1];

                        propItem.BindGameObject(propGo);

                        PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                            (_id: itemId,
                            _count: itemCount,
                            _bUseQuailty: true,
                            _bBind: false,
                            _bNew: false,
                            _bUnLock: false,
                            _bSelected: false,
                            _bShowCount: true,
                            _bShowBagCount: true,
                            _bUseClick: true,
                            _onClick: null,
                            _bshowBtnNo: false);

                        propItem.SetData(new MessageBoxEvt(EUIID.UI_Knowledge_Cooking, showItem));
                    }
                }

                FrameworkTool.ForceRebuildLayout(m_ContentParent.gameObject);
            }
            else
            {
                m_ActiveNode.SetActive(false);
                m_InActiveNode.SetActive(true);
                m_ActiveRedPoint.SetActive(m_CurSelectCooking.GetSubmitState(0) == 0);
            }

            if (m_CurSelectCooking.cookType!=3)
            {
                m_GoCookingButton.gameObject.SetActive(true);
                bool isActive = (m_CurSelectCooking.active == 1);
                m_GoCookingButton.enabled = isActive;
                ImageHelper.SetImageGray(m_GoCookingButton.GetComponent<Image>(), !isActive);
            }
            else
            {
                m_GoCookingButton.gameObject.SetActive(false);
            }
        }

        private void RefreshWatchButton()
        {
            m_WatchStateObj.SetActive(m_CurSelectCooking.watch);
        }

        private void RefreshCollectButton()
        {
            ButtonHelper.Enable(m_CollectionButton, !m_CurSelectCooking.allSubmitExceptFood);
        }

        private void ShowSubmitSelect()
        {
            for (int i = 0; i < m_SubmitGrids.Count; i++)
            {
                if (m_SubmitGrids[i].dataIndex == curSelectSubmitIndex)
                {
                    m_SubmitGrids[i].Select();
                }
                else
                {
                    m_SubmitGrids[i].Release();
                }
            }
        }

        private void OnUpdateSubmitState(uint cookingId)
        {
            if (m_CurSelectCooking == null)
            {
                return;
            }
            if (cookingId != m_CurSelectCooking.id)
            {
                return;
            }

            for (int i = 0; i < m_SubmitGrids.Count; i++)
            {
                m_SubmitGrids[i].SetData(m_CurSelectCooking);
            }
            curSelectSubmitIndex = m_CurSelectCooking.submitIndex;
            ShowSubmitSelect();
            RefreshCollectButton();
            RefreshActiveNode();
        }

        private void OnUpdateSubmitState()
        {
            OnUpdateSubmitState(m_CurSelectCooking.id);
        }


        private void OnSubmitGridClicked(SubmitGrid submitGrid)
        {
            if (curSelectSubmitIndex == submitGrid.dataIndex)
            {
                return;
            }

            int state = submitGrid.cooking.GetSubmitState(submitGrid.dataIndex);
            if (state != 0)
            {
                return;
            }

            curSelectSubmitIndex = submitGrid.dataIndex;
            ShowSubmitSelect();
        }

        private void OnSubmitGridLongPressed(SubmitGrid submitGrid)
        {
            UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_Knowledge_Cooking,
                new PropIconLoader.ShowItemData(submitGrid.itemId, 0, true, false, false, false, false)));
        }

        private void OnCollectionButtonClicked()
        {
            if (m_CurSelectCooking.submitIndex == -1)
            {
                //没有可上交的道具
                string content = LanguageHelper.GetTextContent(5901);
                Sys_Hint.Instance.PushContent_Normal(content);
                return;
            }
            m_CurSelectCooking.Submit(curSelectSubmitIndex);
        }

        private void OnWatchButtonClick()
        {
            bool active = m_WatchStateObj.activeSelf;
            m_WatchStateObj.SetActive(!active);
            Sys_Cooking.Instance.WatchReq(m_CurSelectCooking.id, !active);
        }

        private void OnActiveButtonClick()
        {
            //没有食谱
            if (m_CurSelectCooking.GetSubmitState(0) == -1)
            {
                string content = LanguageHelper.GetTextContent(5938);
                Sys_Hint.Instance.PushContent_Normal(content);
                return;
            }
            m_CurSelectCooking.Submit(0);
        }
        private void OnGoCookingButtonClick()
        {
            if (Sys_OperationalActivity.Instance.CheckSpecialCardPocketKitchenIsUnlock(true))
            {
                OpenCookingSingleParm openCookingSingleParm = new OpenCookingSingleParm();
                openCookingSingleParm.cookFunId = 4;
                openCookingSingleParm.cookId = m_CurSelectCooking.cSVCookData.id;
                UIManager.OpenUI(EUIID.UI_Cooking_Single, false, openCookingSingleParm);
            }
        }
        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Knowledge_Cooking);
        }


        public class SubmitGrid
        {
            public int dataIndex;
            public Cooking cooking;
            public uint itemId;
            private CSVItem.Data m_CSVItemData;
            private Image m_Icon;
            private Text m_Score;
            //private Text m_Name;
            private GameObject m_Image_Arrow;
            public GameObject gameObject;
            private GameObject m_Select;
            private GameObject m_Collected;
            private GameObject m_Lock;
            private Button m_ClickButton;
            private Action<SubmitGrid> m_ClickAction;
            private Action<SubmitGrid> m_OnLongPressed;


            public void BindGameObject(GameObject @object)
            {
                gameObject = @object;
                ParseCp();
            }

            private void ParseCp()
            {
                m_ClickButton = gameObject.transform.Find("Image_icon_bg").GetComponent<Button>();
                m_Icon = gameObject.transform.Find("Image_icon_bg/Image_icon").GetComponent<Image>();
                m_Score = gameObject.transform.Find("Text").GetComponent<Text>();
                m_Image_Arrow = gameObject.transform.Find("Image_Arrow").gameObject;
                m_Select = gameObject.transform.Find("Image_select").gameObject;
                m_Collected = gameObject.transform.Find("Collect").gameObject;
                m_Lock = gameObject.transform.Find("Image_unknow").gameObject;
                m_ClickButton.onClick.AddListener(OnButtonClicked);
            }

            public void SetIndex(int index)
            {
                this.dataIndex = index;
            }

            public void SetData(Cooking cooking)
            {
                this.cooking = cooking;
                this.itemId = cooking.submitConfigItems[dataIndex];
                this.m_CSVItemData = CSVItem.Instance.GetConfData(itemId);
                Refresh();
            }

            public void Refresh()
            {
                uint score = cooking.GetScore(dataIndex);
                TextHelper.SetText(m_Score, LanguageHelper.GetTextContent(1003068, score.ToString()));

                if (m_CSVItemData != null)
                {
                    ImageHelper.SetIcon(m_Icon, m_CSVItemData.icon_id);
                }

                int state = cooking.GetSubmitState(dataIndex);
                if (state == 1)         //已收藏
                {
                    m_Lock.SetActive(false);
                    m_Collected.SetActive(true);
                    m_Image_Arrow.SetActive(false);
                    Release();
                }
                else if (state == -1)  //置灰
                {
                    m_Lock.SetActive(true);
                    m_Collected.SetActive(false);
                    m_Image_Arrow.SetActive(false);
                }
                else
                {
                    m_Collected.SetActive(false);
                    m_Lock.SetActive(false);
                    m_Image_Arrow.SetActive(true);
                }
            }

            public void AddEvent(Action<SubmitGrid> action, Action<SubmitGrid> onlongPressed = null)
            {
                m_ClickAction = action;
                if (onlongPressed != null)
                {
                    m_OnLongPressed = onlongPressed;
                    UI_LongPressButton uI_LongPressButton = m_ClickButton.gameObject.GetNeedComponent<UI_LongPressButton>();
                    uI_LongPressButton.onStartPress.AddListener(OnLongPressed);
                }
            }

            private void OnButtonClicked()
            {
                m_ClickAction?.Invoke(this);
            }

            private void OnLongPressed()
            {
                m_OnLongPressed?.Invoke(this);
            }

            public void Release()
            {
                m_Select.SetActive(false);
            }

            public void Select()
            {
                m_Select.SetActive(true);
            }
        }
    }
}


