using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Logic.Core;
using Table;
using Packet;
using Lib.Core;

namespace Logic
{
    public class UI_Trade_Search_Ornament : UI_Trade_Search_Ornament.ExtraAttr.IListener
    {
        private class OrnamentAssign
        {
            private Transform transform;

            public Button m_BtnType;
            private Text m_TextType;
            private Button m_BtnTip;

            private uint m_OraId;

            public void Init(Transform trans)
            {
                transform = trans;

                m_BtnType = transform.Find("Image_input").GetComponent<Button>();
                m_TextType = transform.Find("Image_input/Text").GetComponent<Text>();
                m_BtnTip = transform.Find("Button").GetComponent<Button>();
                m_BtnTip.onClick.AddListener(OnClickTip);
            }

            private void OnClickTip()
            {
                if (Sys_Trade.Instance.SelectedOraId == 0u)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011923));
                    return;
                }

                CSVItem.Data itemData = CSVItem.Instance.GetConfData(m_OraId);
                CSVOrnamentsUpgrade.Data oraData = CSVOrnamentsUpgrade.Instance.GetConfData(m_OraId);
                if (oraData != null)
                {
                    UIRuleParam param = new UIRuleParam();
                    
                    param.TitlelanId = itemData.name_id;

                    //content
                    System.Text.StringBuilder builder = StringBuilderPool.GetTemporary();
                    builder.Clear();
                    for(int i = 0; i < oraData.base_attr.Count; ++i)
                    {
                        uint attrId = oraData.base_attr[i][0];
                        CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attrId);
                        if (attrData != null)
                            builder.Append(LanguageHelper.GetTextContent(attrData.name));
                        builder.Append("    ");

                        uint value = oraData.base_attr[i][1];
                        //uint maxValue = equipData.attr[i][2];

                        builder.AppendFormat("{0}", Sys_Attr.Instance.GetAttrValue(attrData, value));
                        builder.Append("\n");
                    }
                    param.StrContent = StringBuilderPool.ReleaseTemporaryAndToString(builder);

                    param.Pos = param.Pos = CameraManager.mUICamera.WorldToScreenPoint(m_BtnTip.GetComponent<RectTransform>().position);
                    //m_BtnTip.GetComponent<RectTransform>().position;

                    UIManager.OpenUI(EUIID.UI_Rule, false, param);
                }
            }

            public void Reset()
            {
                m_TextType.text = "";
                m_OraId = 0u;
            }

            public void UpdateInfo()
            {
                m_OraId = Sys_Trade.Instance.SelectedOraId;
                m_TextType.text = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(m_OraId).name_id);
            }
        }

        private class PriceRange
        {
            private Transform transform;

            private UI_Common_Num m_PriceMin;
            private UI_Common_Num m_PriceMax;

            public void Init(Transform trans)
            {
                transform = trans;

                m_PriceMin = new UI_Common_Num();
                m_PriceMin.Init(transform.Find("Image_input0"));
                m_PriceMin.RegEnd(OnInputEndMin);

                m_PriceMax = new UI_Common_Num();
                m_PriceMax.Init(transform.Find("Image_input1"));
                m_PriceMax.RegEnd(OnInputEndMax);
            }

            private void OnInputEndMin(uint num)
            {
                if (Sys_Trade.Instance.OraPriceRange[1] != 0
                    && num > Sys_Trade.Instance.OraPriceRange[1])
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011198));
                    num = Sys_Trade.Instance.OraPriceRange[1];
                }
                Sys_Trade.Instance.OraPriceRange[0] = num;
                m_PriceMin.SetData(num);
            }

            private void OnInputEndMax(uint num)
            {
                if (num < Sys_Trade.Instance.OraPriceRange[0]) //不能小于最小值
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011198));
                    num = Sys_Trade.Instance.OraPriceRange[0];
                }

                Sys_Trade.Instance.OraPriceRange[1] = num;
                m_PriceMax.SetData(num);
            }

            public void Reset()
            {
                m_PriceMin.Reset();
                m_PriceMax.Reset();
            }
        }
        

        public class ExtraAttr : ExtraAttr.CellAttr.IListener
        {
            public class CellAttr
            {
                private Transform transform;
             
                //属性按钮
                private Button m_BtnAttr;
                private Text m_TextAttr;

                private UI_Common_Num m_InputAttrValue;
                private Text _textSymbol;

                // //Toggle
                // private Toggle m_Toggle;

                private Button m_BtnMinus;
                private Button m_BtnAdd;

                private Sys_Trade.OraExtraAttrData _extraAttr;

                private IListener _listener;

                public void Init(Transform trans)
                {
                    transform = trans;

                    m_BtnAttr = transform.Find("Image_input0").GetComponent<Button>();
                    m_BtnAttr.onClick.AddListener(OnClickAttr);
                    m_TextAttr = transform.Find("Image_input0/Text").GetComponent<Text>();

                    m_InputAttrValue = new UI_Common_Num();
                    m_InputAttrValue.Init(transform.Find("Image_input1"));
                    m_InputAttrValue.RegEnd(OnInputEndAttrValue);
                    _textSymbol = transform.Find("Image_input1/Text/Text_Symbol").GetComponent<Text>();

                    m_BtnMinus = transform.Find("Image_Right/Button_Minus").GetComponent<Button>();
                    m_BtnMinus.onClick.AddListener(OnClickMinus);

                    m_BtnAdd = transform.Find("Image_Right/Button_Add").GetComponent<Button>();
                    m_BtnAdd.onClick.AddListener(OnClickAdd);

                    // m_Toggle = transform.Find("Background").GetComponent<Toggle>();
                    // m_Toggle.onValueChanged.AddListener(OnToggleClick);
                    // m_Toggle.isOn = false;

                    _extraAttr = new Sys_Trade.OraExtraAttrData();
                    Sys_Trade.Instance.SelectedOraAttrs.Add(_extraAttr);
                }

                public void Show()
                {
                    transform.gameObject.SetActive(true);
                }

                public void Hide()
                {
                    transform.gameObject.SetActive(false);

                    Sys_Trade.Instance.SelectedOraAttrs.Remove(_extraAttr);
                }

                public void Destroy()
                {
                    GameObject.DestroyImmediate(transform.gameObject, true);
                }

                public void Reset()
                {
                    _extraAttr?.Reset();

                    m_TextAttr.text = "";
                    m_InputAttrValue.Reset();

                    // m_Toggle.isOn = false;

                    _textSymbol.gameObject.SetActive(false);
                }

                private void OnClickAttr()
                {
                    _listener?.OnClickAttr(this);
                }

                private void OnInputEndAttrValue(uint num)
                {
                    _extraAttr.attrValue = num;
                }

                private void OnClickMinus()
                {
                    _listener?.OnClickMinus(this);
                }

                private void OnClickAdd()
                {
                    _listener?.OnClickAdd(this);
                }

                private void OnToggleClick(bool isOn)
                {
                    // if (_extraAttr != null)
                    //     _extraAttr.addSmelt = isOn;
                }

                public void EnableMinus(bool enable)
                {
                    m_BtnMinus.gameObject.SetActive(enable);
                }

                public void EnableAdd(bool enable)
                {
                    m_BtnAdd.gameObject.SetActive(enable);
                }

                public void Register(IListener listener)
                {
                    _listener = listener;
                }

                public void SetAttrId(TradeSearchOraExtraAttrInfo info)
                {
                    _extraAttr.attrId = info.infoId;
                    _extraAttr.isSkill = info.isSkill;

                    if (info.isSkill)
                    {
                        uint skillId = info.infoId * 1000 + 1;
                        CSVPassiveSkillInfo.Data data = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                        if (data != null)
                        {
                            m_TextAttr.text = LanguageHelper.GetTextContent(data.name);
                            _textSymbol.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        CSVAttr.Data data = CSVAttr.Instance.GetConfData(info.infoId);
                        if (data != null)
                        {
                            m_TextAttr.text = LanguageHelper.GetTextContent(data.name);

                            _textSymbol.gameObject.SetActive(data.show_type == 2u);
                        }
                    }
                 
                    m_InputAttrValue.Reset();
                }

                public interface IListener
                {
                    void OnClickAttr(CellAttr cell);
                    void OnClickAdd(CellAttr cell);
                    void OnClickMinus(CellAttr cell);
                }
            }

            private Transform transform;

            private GameObject _template;

            private List<CellAttr> listCells = new List<CellAttr>(4);
            private IListener _Listener;

            public void Init(Transform trans)
            {
                transform = trans;

                _template = transform.Find("Image_line2").gameObject;
                _template.SetActive(false);

                Reset();
            }

            private void GenCellAttr()
            {
                GameObject go = GameObject.Instantiate(_template, transform);
                CellAttr cell = new CellAttr();
                cell.Init(go.transform);
                cell.Register(this);
                cell.Show();
                listCells.Add(cell);
            }

            public void Reset()
            {
                FrameworkTool.DestroyChildren(transform.gameObject, _template.name);

                Sys_Trade.Instance.SelectedOraAttrs.Clear();
                listCells.Clear();
                GenCellAttr();
                listCells[0].EnableMinus(false);
                listCells[0].EnableAdd(true);
                
                _Listener?.OnRebuildLayout();
            }

            public void Register(IListener listener)
            {
                _Listener = listener;
            }

            public void OnClickAttr(CellAttr cell)
            {
                if (Sys_Trade.Instance.SelectedOraId == 0u)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011923));
                    return;
                }

                _Listener?.OnClickAttr(cell);
            }

            public void OnClickMinus(CellAttr cell)
            {
                cell.Reset();
                cell.Hide();
                listCells.Remove(cell);
                listCells[listCells.Count - 1].EnableAdd(true);

                //如果剩一个
                if (listCells.Count == 1)
                    listCells[0].EnableMinus(false);

                cell.Destroy();
                _Listener?.OnRebuildLayout();
            }

            public void OnClickAdd(CellAttr cell)
            {
                if (Sys_Trade.Instance.SelectedOraId == 0u)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011923));
                    return;
                }

                CSVOrnamentsUpgrade.Data data = CSVOrnamentsUpgrade.Instance.GetConfData(Sys_Trade.Instance.SelectedOraId);
                if (listCells.Count >= data.extra_attr_num)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011921));
                    return;
                }
                
                listCells[listCells.Count - 1].EnableAdd(false);
                listCells[listCells.Count - 1].EnableMinus(true);
                GenCellAttr();
                listCells[listCells.Count - 1].EnableAdd(true);

                _Listener?.OnRebuildLayout();
            }

            public interface IListener
            {
                void OnClickAttr(ExtraAttr.CellAttr cellAttr);
                void OnRebuildLayout();
            }
        }

        private class StateComponent
        {
            private class StateToggle
            {
                private Transform transform;

                private uint m_State = 0;
                private CP_Toggle m_Toggle;
                private System.Action<uint> m_Action;

                public void Init(Transform trans)
                {
                    transform = trans;

                    m_Toggle = transform.GetComponent<CP_Toggle>();
                    m_Toggle.onValueChanged.AddListener(OnClick);
                }

                public void SetState(uint tradeState)
                {
                    m_State = tradeState;
                }

                private void OnClick(bool isOn)
                {
                    if (isOn)
                    {
                        m_Action?.Invoke(m_State);
                    }
                }

                public void Register(System.Action<uint> action)
                {
                    m_Action = action;
                }

                public void OnSelect(bool isOn)
                {
                    m_Toggle.SetSelected(isOn, true);
                }
            }

            private Transform transform;

            List<StateToggle> toggles = new List<StateToggle>();

            public void Init(Transform trans)
            {
                transform = trans;

                Transform toggleTrans = transform.Find("Toggle");
                int count = toggleTrans.childCount;
                for (int i = 0; i < count; ++i)
                {
                    StateToggle toggle = new StateToggle();
                    toggle.Init(toggleTrans.GetChild(i));
                    toggle.SetState((uint)i);
                    toggle.OnSelect(false);
                    toggle.Register(OnState);
                    toggles.Add(toggle);
                }
            }

            public void Reset()
            {
                UpdateState();
            }

            private void OnState(uint state)
            {
                Sys_Trade.Instance.OraTradeState = state;
            }

            public  void UpdateState()
            {
                Sys_Trade.Instance.OraTradeState = Sys_Trade.Instance.CurPageType == Sys_Trade.PageType.Buy ? (uint)0 : 1; ;
                int index = Sys_Trade.Instance.CurPageType == Sys_Trade.PageType.Buy ? 0 : 1;
                toggles[index].OnSelect(true);
            }
        }

        private Transform transform;

        private OrnamentAssign m_OrnamentAssign;
        private PriceRange m_PriceRange;
        private ExtraAttr m_ExtraAttr;
        private StateComponent m_StateCom;

        private UI_Trade_Search_Ornament_Sort m_OrnamentSort;
        // private UI_Trade_Search_Equipment_Special m_EquipSpeical;
        private UI_Trade_Search_Ornament_ExtraAttr m_OrnamentExtra;
        // private UI_Trade_Search_Equipment_AdditionAttr m_EquipAddition;

        private Button m_BtnRest;
        private Button m_BtnSearch;
        private Image m_ImgSearchCost;
        private Text m_TxtSearchCost;

        private ExtraAttr.CellAttr _CurOpCellAttr;

        public void Init(Transform trans)
        {
            transform = trans;

            m_OrnamentAssign = new OrnamentAssign();
            m_OrnamentAssign.Init(transform.Find("Rect/Rectlist/Image_line0"));
            m_OrnamentAssign.m_BtnType.onClick.AddListener(() => { m_OrnamentSort.Show(); });

            m_PriceRange = new PriceRange();
            m_PriceRange.Init(transform.Find("Rect/Rectlist/Image_line1"));

            // m_SpecialAttr = new SpecialAttr();
            // m_SpecialAttr.Init(transform.Find("Rect/Rectlist/Image_line2"));
            // m_SpecialAttr.Register(OnClickSpecialAttr);

            m_ExtraAttr = new ExtraAttr();
            m_ExtraAttr.Init(transform.Find("Rect/Rectlist/Image_Lines"));
            m_ExtraAttr.Register(this);

            // m_AdditionAttr = new AdditionAttr();
            // m_AdditionAttr.Init(transform.Find("Rect/Rectlist/Image_line6"));
            // m_AdditionAttr.Register(OnClickAdditionAttr);

            //m_ScoreAssign = new ScoreAssign();
            //m_ScoreAssign.Init(transform.Find("Rect/Rectlist/Image_line7"));

            m_StateCom = new StateComponent();
            m_StateCom.Init(transform.Find("Rect/Rectlist/Image_line11"));

            m_OrnamentSort = new UI_Trade_Search_Ornament_Sort();
            m_OrnamentSort.Init(transform.Find("View_Sort"));
            m_OrnamentSort.Hide();

            // m_EquipSpeical = new UI_Trade_Search_Equipment_Special();
            // m_EquipSpeical.Init(transform.Find("View_Special"));
            // m_EquipSpeical.Hide();
            
            m_OrnamentExtra = new UI_Trade_Search_Ornament_ExtraAttr();
            m_OrnamentExtra.Init(transform.Find("View_Attribute"));
            m_OrnamentExtra.Hide();
      
            // m_EquipAddition = new UI_Trade_Search_Equipment_AdditionAttr();
            // m_EquipAddition.Init(transform.Find("View_Special2"));
            // m_EquipAddition.Hide();

            m_BtnSearch = transform.Find("Bottom_button/Btn_01").GetComponent<Button>();
            m_BtnSearch.onClick.AddListener(OnClickSearch);
            m_ImgSearchCost = transform.Find("Bottom_button/Btn_01/Layout/Icon").GetComponent<Image>();
            m_TxtSearchCost = transform.Find("Bottom_button/Btn_01/Layout/Text_Num").GetComponent<Text>();
            SetSearchCostData();

            m_BtnRest = transform.Find("Bottom_button/Btn_02").GetComponent<Button>();
            m_BtnRest.onClick.AddListener(OnClickReset);

            Sys_Trade.Instance.eventEmitter.Handle<uint>(Sys_Trade.EEvents.OnSelectOrnament, OnSelectOrnament, true);
            Sys_Trade.Instance.eventEmitter.Handle<TradeSearchOraExtraAttrInfo>(Sys_Trade.EEvents.OnSelectOraExtraAttr, OnSelectOraExtraAttr, true);
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
            OnRebuildLayout();
            m_StateCom?.UpdateState();
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        public void Destroy()
        {
            Sys_Trade.Instance.eventEmitter.Handle<uint>(Sys_Trade.EEvents.OnSelectOrnament, OnSelectOrnament, false);
            Sys_Trade.Instance.eventEmitter.Handle<TradeSearchOraExtraAttrInfo>(Sys_Trade.EEvents.OnSelectOraExtraAttr, OnSelectOraExtraAttr, false);
        }

        private void SetSearchCostData()
        {
            CSVItem.Data itemData = CSVItem.Instance.GetConfData(Sys_Trade.Instance.GetSearchCostId());
            ImageHelper.SetIcon(m_ImgSearchCost, itemData.small_icon_id);
            m_TxtSearchCost.text = Sys_Trade.Instance.GetSearchCostNum().ToString();
        }

        private void OnClickSearch()
        {
            if (Sys_Trade.Instance.SelectedOraId == 0u)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011923));
                return;
            }

            CSVCommodity.Data data = CSVCommodity.Instance.GetConfData(Sys_Trade.Instance.SelectedOraId);
            if (null == data)
            {
                Debug.LogErrorFormat("CSVCommodity 未配置 id = {0}", Sys_Trade.Instance.SelectedOraId);
                return;
            }

            //检查消耗
            long leftNum = Sys_Bag.Instance.GetItemCount(Sys_Trade.Instance.GetSearchCostId());
            if (leftNum < Sys_Trade.Instance.GetSearchCostNum())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011256));
                return; 
            }
                
            //公示
            if ((TradeShowType)Sys_Trade.Instance.EquipTradeState == TradeShowType.Publicity)
            {
                if (!data.publicity)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011204));
                    return;
                }
            }

            //议价
            if ((TradeShowType)Sys_Trade.Instance.EquipTradeState == TradeShowType.Discuss)
            {
                if (!data.bargain)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011203));
                    return;
                }
            }

            uint categoryId = Sys_Trade.Instance.SearchParam.bCross ? data.cross_category : data.category;
            uint subClass = Sys_Trade.Instance.SearchParam.bCross ? data.cross_subclass : data.subclass;

            CSVCommodityCategory.Data cateData = CSVCommodityCategory.Instance.GetConfData(categoryId);

            if (cateData != null)
            {
                if (Sys_Trade.Instance.IsInSearchCD())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011276));
                    return;
                }

                Sys_Trade.SearchData searchParam = Sys_Trade.Instance.SearchParam;
                searchParam.Reset();
                searchParam.isSearch = true;
                searchParam.showType = (TradeShowType)Sys_Trade.Instance.OraTradeState;
                searchParam.searchType = TradeSearchType.Ornament;
                searchParam.infoId = Sys_Trade.Instance.SelectedOraId;
                searchParam.Category = cateData.list;
                searchParam.SubCategory = categoryId;
                searchParam.SubClass = subClass;
                searchParam.ornamentParam = Sys_Trade.Instance.CalOraParam();

                Sys_Trade.Instance.eventEmitter.Trigger(Sys_Trade.EEvents.OnSearchNtf);
                UIManager.CloseUI(EUIID.UI_Trade_Search, false, false);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011229));
            }
        }

        private void OnClickReset()
        {
            m_OrnamentAssign.Reset();
            m_PriceRange.Reset();
            // m_SpecialAttr.Reset();
            m_ExtraAttr.Reset();
            // m_AdditionAttr.Reset();
            //m_ScoreAssign.Reset();
            m_StateCom.Reset();

            Sys_Trade.Instance.ClearSearchOraData();
        }

        private void OnSelectOrnament(uint orId)
        {
            Sys_Trade.Instance.SelectedOraId = orId;
            m_OrnamentAssign.UpdateInfo();
            m_OrnamentSort.Hide();
            m_ExtraAttr.Reset();
        }

        private void OnSelectOraExtraAttr(TradeSearchOraExtraAttrInfo info)
        {
            _CurOpCellAttr.SetAttrId(info);
            m_OrnamentExtra.Hide();
        }

        #region Interface

        public void OnClickAttr(ExtraAttr.CellAttr cellAttr)
        {
            _CurOpCellAttr = cellAttr;
            m_OrnamentExtra.Show();
        }

        public void OnRebuildLayout()
        {
            FrameworkTool.ForceRebuildLayout(transform.Find("Rect/Rectlist").gameObject);
        }
        #endregion
    }
}


