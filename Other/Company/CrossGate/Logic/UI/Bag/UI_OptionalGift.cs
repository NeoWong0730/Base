using System.Collections;
using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Table;
using UnityEngine.EventSystems;

namespace Logic
{
    public class UI_OptionalGift : UIBase
    {
        private Button m_CloseButton;
        private Button m_ConfireButton;
        private Button m_CancelButton;
        private Text m_Title;
        private Transform m_TemplteParent;
        private List<OptionGiftCeil> m_Ceils = new List<OptionGiftCeil>();
        private ulong m_ItemUuId;
        private ItemData m_ItemData;
        private uint m_CustomId;
        private uint m_TotalCount;
        private uint m_UsedCount;
        private List<int> m_Options = new List<int>();          //选择索引

        protected override void OnOpen(object arg)
        {
            Tuple<ItemData, uint> tuple = arg as Tuple<ItemData, uint>;
            ItemData itemData = tuple.Item1;
            m_ItemUuId = itemData.Uuid;

            m_ItemData = Sys_Bag.Instance.GetItemDataByUuid(m_ItemUuId);
            m_TotalCount = tuple.Item2;
            m_CustomId = m_ItemData.cSVItemData.fun_value[0];
        }

        protected override void OnLoaded()
        {
            m_CloseButton = transform.Find("Animator/View_TipsBgNew07/Btn_Close").GetComponent<Button>();
            m_ConfireButton = transform.Find("Animator/Btn_01").GetComponent<Button>();
            m_CancelButton = transform.Find("Animator/Btn_02").GetComponent<Button>();
            m_TemplteParent = transform.Find("Animator/Scroll View/Viewport/Content");
            m_Title = transform.Find("Animator/Tips").GetComponent<Text>();

            CSVCustomizeBag.Data cSVCustomizeBagData = CSVCustomizeBag.Instance.GetConfData(m_CustomId);
            if (cSVCustomizeBagData != null)
            {
                int entryCount = cSVCustomizeBagData.gift_content.Count;
                FrameworkTool.CreateChildList(m_TemplteParent, entryCount);
                for (int i = 0; i < entryCount; i++)
                {
                    GameObject gameObject = m_TemplteParent.GetChild(i).gameObject;
                    OptionGiftCeil optionGiftCeil = new OptionGiftCeil();
                    optionGiftCeil.BindGameObject(gameObject);
                    optionGiftCeil.AddEventListener(OnAddClicked, OnSubClicked, OnInputFieldChanged, OnMaxClicked, CanAdd, GetFreeCount);
                    uint itemId = (uint)cSVCustomizeBagData.gift_content[i][0];
                    uint itemCount = (uint)cSVCustomizeBagData.gift_content[i][1];
                    uint boxId = CSVItem.Instance.GetConfData(itemId).box_id;
                    int preSec = (int)cSVCustomizeBagData.gift_content[i][2];
                    if (preSec > 0)
                    {
                        preSec += (int)Sys_Time.Instance.GetServerTime() / 86400;
                    }
                    optionGiftCeil.SetData(boxId, itemId, itemCount, preSec, i);
                    m_Ceils.Add(optionGiftCeil);
                }
            }

            m_CancelButton.onClick.AddListener(OnCancelButtonClicked);
            m_CloseButton.onClick.AddListener(OnCancelButtonClicked);
            m_ConfireButton.onClick.AddListener(OnConfireButtonClicked);
        }

        private void OnAddClicked(OptionGiftCeil optionGiftCeil)
        {
            if (m_UsedCount == m_TotalCount)
            {
                return;
            }
            m_UsedCount++;
            DebugUtil.Log(ELogType.eBag, "OnAddClicked : " + m_UsedCount);
            UpdateTitle();
            UpdateOptions();
            ButtonHelper.Enable(m_ConfireButton, m_UsedCount > 0);
        }

        private void OnSubClicked(OptionGiftCeil optionGiftCeil)
        {
            if (m_UsedCount == 0)
            {
                return;
            }
            m_UsedCount--;
            DebugUtil.Log(ELogType.eBag, "OnSubClicked: " + m_UsedCount);
            UpdateTitle();
            UpdateOptions();
            ButtonHelper.Enable(m_ConfireButton, m_UsedCount > 0);
        }

        private void OnInputFieldChanged(OptionGiftCeil optionGiftCeil)
        {
            m_UsedCount = 0;
            int index = m_Ceils.IndexOf(optionGiftCeil);
            for (int i = 0; i < m_Ceils.Count; i++)
            {
                if (i == index)
                {
                    continue;
                }
                m_UsedCount += m_Ceils[i].selectCount;
            }
            m_UsedCount += optionGiftCeil.selectCount;
            DebugUtil.Log(ELogType.eBag, "OnInputFieldChanged: " + m_UsedCount);
            UpdateTitle();
            UpdateOptions();
            ButtonHelper.Enable(m_ConfireButton, m_UsedCount > 0);
        }

        private void OnMaxClicked(OptionGiftCeil optionGiftCeil)
        {
            m_UsedCount = m_TotalCount;
            DebugUtil.Log(ELogType.eBag, "OnMaxClicked: " + m_UsedCount);
            UpdateTitle();
            UpdateOptions();
            ButtonHelper.Enable(m_ConfireButton, m_UsedCount > 0);
        }

        private void OnRecovery(OptionGiftCeil optionGiftCeil)
        {
            if (m_UsedCount < optionGiftCeil.selectCount)
            {
                DebugUtil.LogErrorFormat("已用数量低于已选数量 usedCount<selectCount");
                return;
            }
            m_UsedCount -= optionGiftCeil.selectCount;
            UpdateTitle();
            UpdateOptions();
        }

        private void OnAddOption(int optionId)
        {
            m_Options.AddOnce<int>(optionId);
        }

        private void OnRemoveOption(int optionId)
        {
            m_Options.TryRemove<int>(optionId);
            ButtonHelper.Enable(m_ConfireButton, m_Options.Count > 0);
        }

        private bool CanAdd()
        {
            return m_UsedCount < m_TotalCount;
        }

        private uint GetFreeCount()
        {
            return m_TotalCount - m_UsedCount;
        }

        private void UpdateTitle()
        {
            TextHelper.SetText(m_Title, LanguageHelper.GetTextContent(4912, m_UsedCount.ToString(), m_TotalCount.ToString()));
        }

        private void UpdateOptions()
        {
            for (int i = 0; i < m_Ceils.Count; i++)
            {
                m_Ceils[i].UpdateButtonState();
            }
        }

        private void OnCancelButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_OptionalGift);
        }

        private void OnConfireButtonClicked()
        {
            Dictionary<uint, uint> singleOptional = new Dictionary<uint, uint>();
            for (int i = 0; i < m_Ceils.Count; i++)
            {
                if (m_Ceils[i].selectCount > 0)
                {
                    singleOptional.Add((uint)m_Ceils[i].index, m_Ceils[i].selectCount);
                }
            }
            Sys_Bag.Instance.OptionalGiftPackReq(m_ItemUuId, singleOptional);
            UIManager.CloseUI(EUIID.UI_OptionalGift);
        }

        protected override void OnShow()
        {
            UpdateTitle();
            ButtonHelper.Enable(m_ConfireButton, m_UsedCount > 0);
        }

        public class OptionGiftCeil
        {
            private GameObject m_Go;
            private GameObject m_PropGo;
            private Toggle m_Toggle;
            private Text m_Name;
            private InputField m_CountText;
            private Button m_ButtonSub;
            private Button m_ButtonAdd;
            private PropItem m_PropItem;
            public uint itemId;
            public uint showItemCount;
            private uint boxId;
            private int prohibitionSec;
            private Action<OptionGiftCeil> m_OnAddClick;
            private Action<OptionGiftCeil> m_OnSubClick;
            private Action<OptionGiftCeil> m_InputFieldChanged;
            private Action<OptionGiftCeil> m_OnMaxClick;
            private Func<bool> m_CanAdd;
            private Func<uint> m_GetFreeCount;
            //private Action<OptionGiftCeil> m_OnRecovery;
            //private Action<int> m_OnAddOption;
            //private Action<int> m_OnRemoveOption;
            public int index;
            public uint selectCount;
            private UI_LongPressButton m_LongPressAddButton;
            private UI_LongPressButton m_LongPressSubButton;
            private Button m_Max;

            public void BindGameObject(GameObject gameObject)
            {
                m_Go = gameObject;

                m_Toggle = m_Go.transform.Find("Toggle").GetComponent<Toggle>();
                m_Name = m_Go.transform.Find("Content/Name").GetComponent<Text>();
                m_CountText = m_Go.transform.Find("Content/Number/InputField").GetComponent<InputField>();
                m_PropGo = m_Go.transform.Find("Content/PropItem").gameObject;

                m_Toggle.onValueChanged.AddListener(OnToggleValueChanged);
                m_ButtonSub = m_Go.transform.Find("Content/Number/Btn_Min").GetComponent<Button>();
                m_LongPressSubButton = m_ButtonSub.gameObject.AddComponent<UI_LongPressButton>();
                m_LongPressSubButton.interval = 0.3f;
                m_LongPressSubButton.bPressAcc = true;
                m_LongPressSubButton.onRelease.AddListener(OnSubButtonClicked);
                m_LongPressSubButton.OnPressAcc.AddListener(OnSubButtonClicked);

                m_ButtonAdd = m_Go.transform.Find("Content/Number/Btn_Add").GetComponent<Button>();
                m_Max = m_Go.transform.Find("Content/Number/Btn_Max").GetComponent<Button>();
                m_Max.onClick.AddListener(OnMaxButtonClicked);

                m_LongPressAddButton = m_ButtonAdd.gameObject.AddComponent<UI_LongPressButton>();
                m_LongPressAddButton.interval = 0.3f;
                m_LongPressAddButton.bPressAcc = true;
                m_LongPressAddButton.onRelease.AddListener(OnAddButtonClicked);
                m_LongPressAddButton.OnPressAcc.AddListener(OnAddButtonClicked);

                m_CountText.onValueChanged.AddListener(OnValueChanged);

                m_PropItem = new PropItem();
                m_PropItem.BindGameObject(m_PropGo);
            }

            public void SetData(uint boxId, uint itemId, uint count, int prohibitionSec, int index)
            {
                this.itemId = itemId;
                this.showItemCount = count;
                this.boxId = boxId;
                this.itemId = itemId;
                this.index = index;
                this.prohibitionSec = prohibitionSec;
                Refresh();
            }

            public void AddEventListener(Action<OptionGiftCeil> onAddClick, Action<OptionGiftCeil> onSubClick, Action<OptionGiftCeil> onInputFieldChanged, Action<OptionGiftCeil> onMaxClick, Func<bool> canAdd, Func<uint> getFreeCount)
            {
                m_OnAddClick = onAddClick;
                m_OnSubClick = onSubClick;
                m_InputFieldChanged = onInputFieldChanged;
                m_OnMaxClick = onMaxClick;
                m_CanAdd = canAdd;
                m_GetFreeCount = getFreeCount;
                //m_OnAddOption = onAddOption;
                //m_OnRemoveOption = onRemoveOption;
                //m_OnRecovery = onRecovery;
            }

            private void OnValueChanged(string value)
            {
                int count = int.Parse(value);
                int minCount = 0;
                int maxCount = (int)selectCount + (int)m_GetFreeCount();
                count = Mathf.Clamp(count, minCount, maxCount);
                m_CountText.text = count.ToString();
                selectCount = (uint)count;
                m_InputFieldChanged?.Invoke(this);
            }

            private void OnSubButtonClicked()
            {
                if (selectCount == 0)
                {
                    return;
                }
                selectCount--;

                m_OnSubClick?.Invoke(this);
                UpdateButtonState();
                UpdateCount();
            }

            private void OnAddButtonClicked()
            {
                if (!m_CanAdd())
                {
                    return;
                }
                selectCount++;

                m_OnAddClick?.Invoke(this);
                UpdateButtonState();
                UpdateCount();
            }

            private void OnToggleValueChanged(bool arg)
            {
                //if (arg)
                //{
                //    m_OnAddOption.Invoke(index);
                //    ButtonHelper.Enable(m_ButtonAdd, m_CanAdd());
                //    m_LongPressAddButton.enabled = m_CanAdd();
                //}
                //else
                //{
                //    m_OnRemoveOption.Invoke(index);
                //    m_OnRecovery?.Invoke(this);
                //    Disable();
                //}
            }

            private void OnMaxButtonClicked()
            {
                selectCount += m_GetFreeCount();
                m_CountText.text = selectCount.ToString();
                m_OnMaxClick?.Invoke(this);
                UpdateButtonState();
            }

            private void Disable()
            {
                ButtonHelper.Enable(m_ButtonSub, false);
                ButtonHelper.Enable(m_ButtonAdd, false);
                m_LongPressAddButton.enabled = false;
                m_LongPressSubButton.enabled = false;
                selectCount = 0;
                m_CountText.text = 0.ToString();
            }
            
            public void UpdateButtonState()
            {
                ButtonHelper.Enable(m_ButtonAdd, m_CanAdd());
                ButtonHelper.Enable(m_Max, m_CanAdd());
                ButtonHelper.Enable(m_ButtonSub, selectCount > 0);
                m_LongPressSubButton.enabled = selectCount > 0;
                m_LongPressAddButton.enabled = m_CanAdd();
            }
            
            private void Refresh()
            {
                TextHelper.SetText(m_Name, CSVItem.Instance.GetConfData(itemId).name_id);
                UpdateCount();

                ButtonHelper.Enable(m_ButtonAdd, m_CanAdd());
                m_LongPressAddButton.enabled = m_CanAdd();

                //ButtonHelper.Enable(m_ButtonAdd, false);
                ButtonHelper.Enable(m_ButtonSub, false);
                //m_LongPressAddButton.enabled = false;
                m_LongPressSubButton.enabled = false;
                PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                              (_id: itemId,
                              _count: showItemCount,
                              _bUseQuailty: true,
                              _bBind: false,
                              _bNew: false,
                              _bUnLock: false,
                              _bSelected: false,
                              _bShowCount: true,
                              _bShowBagCount: false,
                              _bUseClick: true,
                              _onClick: OnClicked,
                              _bshowBtnNo: false,
                              _bUseTips: false);
                m_PropItem.SetData(new MessageBoxEvt(EUIID.UI_OptionalGift, showItem));
            }

            private void UpdateCount()
            {
                m_CountText.text = selectCount.ToString();
            }

            private void OnClicked(PropItem propItem)
            {
                ItemData itemData = new ItemData((int)boxId, 0, itemId, showItemCount, 0, false,
                           false, null, null, (int)prohibitionSec);
                PropMessageParam propParam = new PropMessageParam();
                propParam.itemData = itemData;
                propParam.needShowInfo = false;
                propParam.needShowMarket = true;
                propParam.sourceUiId = EUIID.UI_OptionalGift;
                UIManager.OpenUI(EUIID.UI_Prop_Message, false, propParam);
            }
            
        }
    }
}


