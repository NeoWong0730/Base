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
    public class UI_Trade_Search_Equipment : UI_Trade_Search_Equipment.BasicAttr.IListener
    {
        private class EquipAssign
        {
            private Transform transform;

            public Button m_BtnType;
            private Text m_TextType;
            private Button m_BtnTip;

            private uint m_EquipId;

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
                if (Sys_Trade.Instance.SelectedEquipId == 0u)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011177));
                    return;
                }

                CSVItem.Data itemData = CSVItem.Instance.GetConfData(m_EquipId);
                CSVEquipment.Data equipData = CSVEquipment.Instance.GetConfData(m_EquipId);
                if (equipData != null)
                {
                    UIRuleParam param = new UIRuleParam();
                    
                    param.TitlelanId = itemData.name_id;

                    //content
                    System.Text.StringBuilder builder = StringBuilderPool.GetTemporary();
                    builder.Clear();
                    for(int i = 0; i < equipData.attr.Count; ++i)
                    {
                        uint attrId = equipData.attr[i][0];
                        CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attrId);
                        if (attrData != null)
                            builder.Append(LanguageHelper.GetTextContent(attrData.name));
                        builder.Append("    ");

                        uint minValue = equipData.attr[i][1];
                        uint maxValue = equipData.attr[i][2];

                        builder.AppendFormat("{0}-{1}", Sys_Attr.Instance.GetAttrValue(attrData, minValue), Sys_Attr.Instance.GetAttrValue(attrData, maxValue));
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
                m_EquipId = 0u;
            }

            public void UpdateInfo()
            {
                m_EquipId = Sys_Trade.Instance.SelectedEquipId;
                m_TextType.text = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(m_EquipId).name_id);
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
                if (Sys_Trade.Instance.EquipPriceRange[1] != 0
                    && num > Sys_Trade.Instance.EquipPriceRange[1])
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011198));
                    num = Sys_Trade.Instance.EquipPriceRange[1];
                }
                Sys_Trade.Instance.EquipPriceRange[0] = num;
                m_PriceMin.SetData(num);
            }

            private void OnInputEndMax(uint num)
            {
                if (num < Sys_Trade.Instance.EquipPriceRange[0]) //不能小于最小值
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011198));
                    num = Sys_Trade.Instance.EquipPriceRange[0];
                }

                Sys_Trade.Instance.EquipPriceRange[1] = num;
                m_PriceMax.SetData(num);
            }

            public void Reset()
            {
                m_PriceMin.Reset();
                m_PriceMax.Reset();
            }
        }

        public class SpecialAttr
        {
            private Transform transform;

            private Button m_Btn;
            private Text m_Text;

            private List<uint> SpecialIds = new List<uint>(2);
            private System.Action _action;

            public void Init(Transform trans)
            {
                transform = trans;

                m_Btn = transform.Find("Image_input").GetComponent<Button>();
                m_Btn.onClick.AddListener(OnClickAttr);

                m_Text = transform.Find("Image_input/Text").GetComponent<Text>();
            }

            public void Reset()
            {
                m_Text.text = "";
                SpecialIds.Clear();
            }

            private void OnClickAttr()
            {
                if (Sys_Trade.Instance.SelectedEquipId == 0u)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011175));
                    return;
                }

                _action?.Invoke();
            }

            public void Register(System.Action action)
            {
                _action = action;
            }

            public void UpdateInfo()
            {
                SpecialIds.Clear();
                List<uint> effectIds = Sys_Trade.Instance.EquipSpeicalAttrIds;

                System.Text.StringBuilder builder = StringBuilderPool.GetTemporary();
                builder.Clear();
                for (int i = 0; i < effectIds.Count; ++i)
                {
                    SpecialIds.Add(effectIds[i]);

                    CSVEquipmentEffect.Data data = CSVEquipmentEffect.Instance.GetConfData(effectIds[i]);
                    if (data != null)
                    {
                        builder.Append(LanguageHelper.GetTextContent(data.name));
                        builder.Append(";");
                    }
                }

                m_Text.text = StringBuilderPool.ReleaseTemporaryAndToString(builder);
            }
        }

        public class BasicAttr : BasicAttr.CellAttr.IListener
        {
            public class CellAttr
            {
                private Transform transform;
             
                //属性按钮
                private Button m_BtnAttr;
                private Text m_TextAttr;

                private UI_Common_Num m_InputAttrValue;
                private Text _textSymbol;

                //Toggle
                private Toggle m_Toggle;

                private Button m_BtnMinus;
                private Button m_BtnAdd;

                private Sys_Trade.EquipBasicAttr _basicAttr;

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

                    m_Toggle = transform.Find("Background").GetComponent<Toggle>();
                    m_Toggle.onValueChanged.AddListener(OnToggleClick);
                    m_Toggle.isOn = false;

                    _basicAttr = new Sys_Trade.EquipBasicAttr();
                    Sys_Trade.Instance.EquipBasicAttrArray.Add(_basicAttr);
                }

                public void Show()
                {
                    transform.gameObject.SetActive(true);
                }

                public void Hide()
                {
                    transform.gameObject.SetActive(false);

                    Sys_Trade.Instance.EquipBasicAttrArray.Remove(_basicAttr);
                }

                public void Destroy()
                {
                    GameObject.DestroyImmediate(transform.gameObject, true);
                }

                public void Reset()
                {
                    _basicAttr?.Reset();

                    m_TextAttr.text = "";
                    m_InputAttrValue.Reset();

                    m_Toggle.isOn = false;

                    _textSymbol.gameObject.SetActive(false);
                }

                private void OnClickAttr()
                {
                    _listener?.OnClickAttr(this);
                }

                private void OnInputEndAttrValue(uint num)
                {
                    _basicAttr.attrValue = num;
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
                    if (_basicAttr != null)
                        _basicAttr.addSmelt = isOn;
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

                public void SetAttrId(uint attrId)
                {
                    _basicAttr.attrId = attrId;

                    CSVAttr.Data data = CSVAttr.Instance.GetConfData(attrId);
                    if (data != null)
                    {
                        m_TextAttr.text = LanguageHelper.GetTextContent(data.name);

                        _textSymbol.gameObject.SetActive(data.show_type == 2u);
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

                _template = transform.Find("Image_line3").gameObject;
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
                if (Sys_Trade.Instance.SelectedEquipId == 0u)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011175));
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
                if (Sys_Trade.Instance.SelectedEquipId == 0u)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011175));
                    return;
                }

                CSVEquipment.Data data = CSVEquipment.Instance.GetConfData(Sys_Trade.Instance.SelectedEquipId);
                if (listCells.Count >= data.attr.Count)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011176));
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
                void OnClickAttr(BasicAttr.CellAttr cellAttr);
                void OnRebuildLayout();
            }
        }

        public class AdditionAttr 
        {
            private Transform transform;

            private Button m_Btn;
            private Text m_Text;

            private UI_Common_Num m_InputNum;

            private Toggle m_Toggle;

            private Sys_Trade.EquipAddition _additionData = Sys_Trade.Instance.EquipAddtionData;
            private System.Action _action;

            public void Init(Transform trans)
            {
                transform = trans;

                m_Btn = transform.Find("Image_input0").GetComponent<Button>();
                m_Btn.onClick.AddListener(OnClickAttr);

                m_Text = transform.Find("Image_input0/Text").GetComponent<Text>();

                m_InputNum = new UI_Common_Num();
                m_InputNum.Init(transform.Find("Image_input1"));
                m_InputNum.RegEnd(OnInputEndNum);

                m_Toggle = transform.Find("Background").GetComponent<Toggle>();
                m_Toggle.onValueChanged.AddListener(OnClickToggle);
                m_Toggle.isOn = _additionData.addSmelt;
            }

            public void Reset()
            {
                _additionData?.Reset();

                m_Toggle.isOn = false;

                m_Text.text = "";
                m_InputNum.Reset();
            }

            private void OnClickAttr()
            {
                if (Sys_Trade.Instance.SelectedEquipId == 0u)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011177));
                    return;
                }

                _action?.Invoke();
            }

            private void OnClickToggle(bool isOn)
            {
                if (_additionData != null)
                    _additionData.addSmelt = isOn;
            }

            private void OnInputEndNum(uint num)
            {
                _additionData.totalValue = num;
            }

            public void Register(System.Action action)
            {
                _action = action;
            }

            public void UpdateInfo()
            {
                List<uint> attrIds = Sys_Trade.Instance.EquipAddtionData.attrIds;

                System.Text.StringBuilder builder = StringBuilderPool.GetTemporary();
                builder.Clear();

                for (int i = 0; i < attrIds.Count; ++i)
                {
                    CSVAttr.Data data = CSVAttr.Instance.GetConfData(attrIds[i]);
                    if (data != null)
                    {
                        builder.Append(LanguageHelper.GetTextContent(data.name)).Append(" ");
                    }
                }

                m_Text.text = StringBuilderPool.ReleaseTemporaryAndToString(builder);
            }
        }

        private class ScoreAssign 
        {
            private Transform transform;

            private UI_Common_Num m_InputScore;

            public void Init(Transform trans)
            {
                transform = trans;

                m_InputScore = new UI_Common_Num();
                m_InputScore.Init(transform.Find("Image_input"));
                m_InputScore.RegEnd(OnInputEndScore);
            }

            public void Reset()
            {
                m_InputScore.Reset();
            }

            private void OnInputEndScore(uint num)
            {
                Sys_Trade.Instance.EquipScore = num;
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
                Sys_Trade.Instance.EquipTradeState = state;
            }

            public  void UpdateState()
            {
                Sys_Trade.Instance.EquipTradeState = Sys_Trade.Instance.CurPageType == Sys_Trade.PageType.Buy ? (uint)0 : 1; ;
                int index = Sys_Trade.Instance.CurPageType == Sys_Trade.PageType.Buy ? 0 : 1;
                toggles[index].OnSelect(true);
            }
        }

        private Transform transform;

        private EquipAssign m_EquipAssign;
        private PriceRange m_PriceRange;
        private SpecialAttr m_SpecialAttr;
        private BasicAttr m_BasicAttr;
        private AdditionAttr m_AdditionAttr;
        private ScoreAssign m_ScoreAssign;
        private StateComponent m_StateCom;

        private UI_Trade_Search_Equipment_Sort m_EquipSort;
        private UI_Trade_Search_Equipment_Special m_EquipSpeical;
        private UI_Trade_Search_Equipment_BasicAttr m_EquipBasic;
        private UI_Trade_Search_Equipment_AdditionAttr m_EquipAddition;

        private Button m_BtnRest;
        private Button m_BtnSearch;
        private Image m_ImgSearchCost;
        private Text m_TxtSearchCost;

        private BasicAttr.CellAttr _CurOpCellAttr;

        public void Init(Transform trans)
        {
            transform = trans;

            m_EquipAssign = new EquipAssign();
            m_EquipAssign.Init(transform.Find("Rect/Rectlist/Image_line0"));
            m_EquipAssign.m_BtnType.onClick.AddListener(() => { m_EquipSort.Show(); });

            m_PriceRange = new PriceRange();
            m_PriceRange.Init(transform.Find("Rect/Rectlist/Image_line1"));

            m_SpecialAttr = new SpecialAttr();
            m_SpecialAttr.Init(transform.Find("Rect/Rectlist/Image_line2"));
            m_SpecialAttr.Register(OnClickSpecialAttr);

            m_BasicAttr = new BasicAttr();
            m_BasicAttr.Init(transform.Find("Rect/Rectlist/Image_Lines"));
            m_BasicAttr.Register(this);

            m_AdditionAttr = new AdditionAttr();
            m_AdditionAttr.Init(transform.Find("Rect/Rectlist/Image_line6"));
            m_AdditionAttr.Register(OnClickAdditionAttr);

            //m_ScoreAssign = new ScoreAssign();
            //m_ScoreAssign.Init(transform.Find("Rect/Rectlist/Image_line7"));

            m_StateCom = new StateComponent();
            m_StateCom.Init(transform.Find("Rect/Rectlist/Image_line8"));

            m_EquipSort = new UI_Trade_Search_Equipment_Sort();
            m_EquipSort.Init(transform.Find("View_Sort"));
            m_EquipSort.Hide();

            m_EquipSpeical = new UI_Trade_Search_Equipment_Special();
            m_EquipSpeical.Init(transform.Find("View_Special"));
            m_EquipSpeical.Hide();

            m_EquipBasic = new UI_Trade_Search_Equipment_BasicAttr();
            m_EquipBasic.Init(transform.Find("View_Attribute"));
            m_EquipBasic.Hide();

            m_EquipAddition = new UI_Trade_Search_Equipment_AdditionAttr();
            m_EquipAddition.Init(transform.Find("View_Special2"));
            m_EquipAddition.Hide();

            m_BtnSearch = transform.Find("Bottom_button/Btn_01").GetComponent<Button>();
            m_BtnSearch.onClick.AddListener(OnClickSearch);
            m_ImgSearchCost = transform.Find("Bottom_button/Btn_01/Layout/Icon").GetComponent<Image>();
            m_TxtSearchCost = transform.Find("Bottom_button/Btn_01/Layout/Text_Num").GetComponent<Text>();
            SetSearchCostData();

            m_BtnRest = transform.Find("Bottom_button/Btn_02").GetComponent<Button>();
            m_BtnRest.onClick.AddListener(OnClickReset);

            Sys_Trade.Instance.eventEmitter.Handle<uint>(Sys_Trade.EEvents.OnSelectEquip, OnSelectEquip, true);
            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnSelectEquipSpecialAttr, OnSelectSpecialAttr, true);
            Sys_Trade.Instance.eventEmitter.Handle<uint>(Sys_Trade.EEvents.OnSelectEquipBasicAttr, OnSelectBasicAttr, true);
            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnSelectEquipAdditionAttr, OnSelectEquipAdditionAttr, true);
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
            Sys_Trade.Instance.eventEmitter.Handle<uint>(Sys_Trade.EEvents.OnSelectEquip, OnSelectEquip, false);
            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnSelectEquipSpecialAttr, OnSelectSpecialAttr, false);
            Sys_Trade.Instance.eventEmitter.Handle<uint>(Sys_Trade.EEvents.OnSelectEquipBasicAttr, OnSelectBasicAttr, false);
            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnSelectEquipAdditionAttr, OnSelectEquipAdditionAttr, false);
        }

        private void SetSearchCostData()
        {
            CSVItem.Data itemData = CSVItem.Instance.GetConfData(Sys_Trade.Instance.GetSearchCostId());
            ImageHelper.SetIcon(m_ImgSearchCost, itemData.small_icon_id);
            m_TxtSearchCost.text = Sys_Trade.Instance.GetSearchCostNum().ToString();
        }

        private void OnClickSearch()
        {
            if (Sys_Trade.Instance.SelectedEquipId == 0u)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011177));
                return;
            }

            CSVCommodity.Data data = CSVCommodity.Instance.GetConfData(Sys_Trade.Instance.SelectedEquipId);
            if (null == data)
            {
                Debug.LogErrorFormat("CSVCommodity 未配置 id = {0}", Sys_Trade.Instance.SelectedEquipId);
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
                searchParam.showType = (TradeShowType)Sys_Trade.Instance.EquipTradeState;
                searchParam.searchType = TradeSearchType.Equip;
                searchParam.infoId = Sys_Trade.Instance.SelectedEquipId;
                searchParam.Category = cateData.list;
                searchParam.SubCategory = categoryId;
                searchParam.SubClass = subClass;
                searchParam.equipParam = Sys_Trade.Instance.CalEquipParam();

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
            m_EquipAssign.Reset();
            m_PriceRange.Reset();
            m_SpecialAttr.Reset();
            m_BasicAttr.Reset();
            m_AdditionAttr.Reset();
            //m_ScoreAssign.Reset();
            m_StateCom.Reset();

            Sys_Trade.Instance.ClearSearchEquipData();
        }

        private void OnSelectEquip(uint equipId)
        {
            Sys_Trade.Instance.SelectedEquipId = equipId;
            m_EquipAssign.UpdateInfo();
            m_EquipSort.Hide();
        }

        private void OnSelectSpecialAttr()
        {
            m_SpecialAttr.UpdateInfo();
            m_EquipSpeical.Hide();
        }

        private void OnSelectBasicAttr(uint attrId)
        {
            _CurOpCellAttr.SetAttrId(attrId);
            m_EquipBasic.Hide();
        }

        private void OnSelectEquipAdditionAttr()
        {
            m_AdditionAttr.UpdateInfo();
            m_EquipAddition.Hide();
        }

        #region Interface
        private void OnClickSpecialAttr()
        {
            m_EquipSpeical.Show();
        }

        public void OnClickAttr(BasicAttr.CellAttr cellAttr)
        {
            _CurOpCellAttr = cellAttr;
            m_EquipBasic.Show();
        }

        private void OnClickAdditionAttr()
        {
            m_EquipAddition.Show();
        }

        public void OnRebuildLayout()
        {
            FrameworkTool.ForceRebuildLayout(transform.Find("Rect/Rectlist").gameObject);
        }
        #endregion
    }
}


