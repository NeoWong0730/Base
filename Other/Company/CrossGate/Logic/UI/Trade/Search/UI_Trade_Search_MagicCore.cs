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
    public class UI_Trade_Search_MagicCore : UI_Trade_Search_MagicCore.MagicAttr.IListener
    {
        private class MagicAssign
        {
            private Transform transform;

            public Button m_BtnType;
            private Text m_TextType;
            private Button m_BtnTip;

            //private List<PetType> listTypes = new List<PetType>(2);
            private uint _coreId = 0u;
            public uint CoreId {
                get {
                    return _coreId;
                }
                set {
                    _coreId = value;
                    SetName();
                }
            }

            public void Init(Transform trans)
            {
                transform = trans;

                m_BtnType = transform.Find("Image_input").GetComponent<Button>();
                m_TextType = transform.Find("Image_input/Text").GetComponent<Text>();
                m_BtnTip = transform.Find("Button").GetComponent<Button>();
                m_BtnTip.onClick.AddListener(OnClickTip);

                InitInfo();
            }

            private void OnClickTip()
            {
                if (Sys_Trade.Instance.SelectedCoreId == 0u)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011286));
                    return;
                }
                
                CSVPetEquip.Data equipData = CSVPetEquip.Instance.GetConfData(Sys_Trade.Instance.SelectedCoreId);
                if (equipData != null)
                {
                    UIRuleParam param = new UIRuleParam();
                    
                    param.TitlelanId = CSVItem.Instance.GetConfData(equipData.id).name_id;
                    
                    //计算id去重属
                    List<uint> ids = new List<uint>();
                    List<uint> temps = new List<uint>();
                    int count = CSVPetEquipAttr.Instance.Count;
                    for (int i = 0; i < count; ++i)
                    {
                        CSVPetEquipAttr.Data data = CSVPetEquipAttr.Instance.GetByIndex(i);
                        if (data.group_id == equipData.attr_id)
                        {
                            if (temps.IndexOf(data.attr_id) < 0)
                            {
                                temps.Add(data.attr_id);
                                ids.Add(data.id);
                            }
                        }
                    }
                    //content
                    System.Text.StringBuilder builder = StringBuilderPool.GetTemporary();
                    builder.Clear();
                    for(int i = 0; i < ids.Count; ++i)
                    {
                        CSVPetEquipAttr.Data equipAttrData = CSVPetEquipAttr.Instance.GetConfData(ids[i]);
                        uint attrId = equipAttrData.attr_id;
                        CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attrId);
                        if (attrData != null)
                            builder.Append(LanguageHelper.GetTextContent(attrData.name));
                        builder.Append("    ");

                        uint minValue = equipAttrData.min_attr;
                        uint maxValue = equipAttrData.max_attr;

                        builder.AppendFormat("{0}-{1}", Sys_Attr.Instance.GetAttrValue(attrData, minValue), Sys_Attr.Instance.GetAttrValue(attrData, maxValue));
                        builder.Append("\n");
                    }
                    param.StrContent = StringBuilderPool.ReleaseTemporaryAndToString(builder);

                    param.Pos = param.Pos = CameraManager.mUICamera.WorldToScreenPoint(m_BtnTip.GetComponent<RectTransform>().position);

                    UIManager.OpenUI(EUIID.UI_Rule, false, param);
                }
            }

            private void OnSelectType(uint type)
            {
                CoreId = type;
            }

            private void InitInfo()
            {
                _coreId = 0;
            }

            private void SetName()
            {
                CSVItem.Data data = CSVItem .Instance.GetConfData(_coreId);
                if (data != null)
                    m_TextType.text = LanguageHelper.GetTextContent(data.name_id);
            }

            public void Reset()
            {
                _coreId = 0;
                m_TextType.text = "";
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
                if (Sys_Trade.Instance.CorePriceRange[1] != 0
                    && num > Sys_Trade.Instance.CorePriceRange[1]) //不能小于最小值
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011198));
                    num = Sys_Trade.Instance.CorePriceRange[1];
                }

                Sys_Trade.Instance.CorePriceRange[0] = num;
                m_PriceMin.SetData(num);
            }

            private void OnInputEndMax(uint num)
            {
                if (num < Sys_Trade.Instance.CorePriceRange[0]) //不能小于最小值
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011198));
                    num = Sys_Trade.Instance.CorePriceRange[0];
                }

                Sys_Trade.Instance.CorePriceRange[1] = num;
                m_PriceMax.SetData(num);
            }

            public void Reset()
            {
                m_PriceMin.Reset();
                m_PriceMax.Reset();
            }
        }

        public class MagicAttr : MagicAttr.Attr.IListener
        {
            public class Attr
            {
                private Transform transform;

                private Button m_BtnAttr;
                private Text m_TextAttr;

                private Transform m_TransArrow;

                private UI_Common_Num m_InputNum;
                private Text _textSymbol;

                private Button m_BtnMinus;
                private Button m_BtnAdd;

                private Sys_Trade.CoreAttrData _attrData;

                private IListener _listener;

                public void Init(Transform trans)
                {
                    transform = trans;

                    m_BtnAttr = transform.Find("Image_input0").GetComponent<Button>();
                    m_BtnAttr.onClick.AddListener(OnClickQuality);

                    m_TextAttr = transform.Find("Image_input0/Text").GetComponent<Text>();

                    m_TransArrow = transform.Find("Image_greater");

                    m_InputNum = new UI_Common_Num();
                    m_InputNum.Init(transform.Find("Image_input1"));
                    m_InputNum.RegEnd(OnInputEndNum);
                    
                    _textSymbol = transform.Find("Image_input1/Text/Text_Symbol").GetComponent<Text>();

                    m_BtnMinus = transform.Find("Image_Right/Button_Minus").GetComponent<Button>();
                    m_BtnMinus.onClick.AddListener(OnClickMinus);

                    m_BtnAdd = transform.Find("Image_Right/Button_Add").GetComponent<Button>();
                    m_BtnAdd.onClick.AddListener(OnClickAdd);

                    _attrData = new Sys_Trade.CoreAttrData();
                    Sys_Trade.Instance.SelectedCoreAttrs.Add(_attrData);
                }

                public void Show()
                {
                    transform.gameObject.SetActive(true);
                }

                public void Hide()
                {
                    transform.gameObject.SetActive(false);

                    Sys_Trade.Instance.SelectedCoreAttrs.Remove(_attrData);
                }

                public void Destroy()
                {
                    GameObject.DestroyImmediate(transform.gameObject, true);
                }

                public void Reset()
                {
                    _attrData.attrId = 0u;
                    _attrData.attrValue = 0u;

                    m_TextAttr.text = "";
                    m_InputNum.Reset();
                }

                private void OnClickQuality()
                {
                    _listener?.OnClickAttr(this);
                }

                private void OnInputEndNum(uint num)
                {
                    _attrData.attrValue = num;
                }

                private void OnClickMinus()
                {
                    _listener?.OnClickMinus(this);
                }

                private void OnClickAdd()
                {
                    _listener?.OnClickAdd(this);
                }

                public void Register(IListener listener)
                {
                    _listener = listener;
                }

                public void SetAttrId(uint attrId)
                {
                    _attrData.attrId = attrId;
                    CSVAttr.Data data = CSVAttr.Instance.GetConfData(attrId);
                    m_TextAttr.text = LanguageHelper.GetTextContent(data.name);
                    _textSymbol.gameObject.SetActive(data.show_type == 2u);
                }

                public void EnableMinus(bool enable)
                {
                    m_BtnMinus.gameObject.SetActive(enable);
                }

                public void EnableAdd(bool enable)
                {
                    m_BtnAdd.gameObject.SetActive(enable);
                }

                public interface IListener
                {
                    void OnClickAttr(Attr cell);
                    void OnClickAdd(Attr cell);
                    void OnClickMinus(Attr cell);
                }
            }

            private Transform transform;

            private GameObject _template;

            private List<Attr> _list = new List<Attr>();

            private IListener _Listener;

            private int _curIndex;

            public void Init(Transform trans)
            {
                transform = trans;

                _template = transform.Find("Image_line2").gameObject;
                _template.SetActive(false);

                Reset();
            }

            private void GenCell()
            {
                GameObject go = GameObject.Instantiate(_template, transform);
                Attr cell = new Attr();
                cell.Init(go.transform);
                cell.Register(this);
                cell.Show();
                _list.Add(cell);
            }

            public void Reset()
            {
                FrameworkTool.DestroyChildren(transform.gameObject, _template.name);

                _list.Clear();
                GenCell();
                _list[0].EnableMinus(false);
                _list[0].EnableAdd(true);

                _Listener?.OnRebuildLayout();
            }

            public void Register(IListener listener)
            {
                _Listener = listener;
            }

            public void OnClickAttr(Attr cell)
            {
                if (Sys_Trade.Instance.SelectedCoreId == 0u)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011286));
                    return;
                }

                _Listener?.OnClickAttr(cell);
            }

            public void OnClickMinus(Attr cell)
            {
                cell.Reset();
                cell.Hide();
                _list.Remove(cell);
                _list[_list.Count - 1].EnableAdd(true);

                //如果剩一个
                if (_list.Count == 1)
                    _list[0].EnableMinus(false);

                cell.Destroy();
                _Listener?.OnRebuildLayout();
            }

            public void OnClickAdd(Attr cell)
            {
                if (Sys_Trade.Instance.SelectedCoreId == 0u)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011286));
                    return;
                }

                CSVPetEquip.Data data = CSVPetEquip.Instance.GetConfData(Sys_Trade.Instance.SelectedCoreId);
                if (_list.Count >= data.attr_num)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011287));
                    return;
                }

                _list[_list.Count - 1].EnableAdd(false);
                _list[_list.Count - 1].EnableMinus(true);
                GenCell();
                _list[_list.Count - 1].EnableAdd(true);

                _Listener?.OnRebuildLayout();
            }

            public interface IListener
            {
                void OnClickAttr(MagicAttr.Attr cellAttr);
                void OnRebuildLayout();
            }
        }

        private class CoreSuit
        {
            private Transform transform;

            public Button m_BtnSuit;
            private Text m_TextSuit;

            public void Init(Transform trans)
            {
                transform = trans;

                m_BtnSuit = transform.GetComponent<Button>();
                m_TextSuit = transform.Find("Text").GetComponent<Text>();
            }

            public void Reset()
            {
                m_TextSuit.text = "";
            }

            public void UpdateInfo(uint suitSkillId)
            {
                Sys_Trade.Instance.SelectedCoreSuitSkillId = suitSkillId;
                CSVPetEquipSuitSkill.Data suitSkillData = CSVPetEquipSuitSkill.Instance.GetConfData(suitSkillId);
                if (suitSkillData != null)
                {
                    CSVPassiveSkillInfo.Data info = CSVPassiveSkillInfo.Instance.GetConfData(suitSkillData.base_skill);
                    if (info != null)
                    {
                        m_TextSuit.text = LanguageHelper.GetTextContent(info.name);
                    }
                }
            }
        }
        
        private class CoreDress
        {
            private Transform transform;

            public Button m_BtnDress;
            private Text m_TextDress;

            public void Init(Transform trans)
            {
                transform = trans;

                m_BtnDress = transform.GetComponent<Button>();
                m_TextDress = transform.Find("Text").GetComponent<Text>();
            }

            public void Reset()
            {
                m_TextDress.text = "";
                Sys_Trade.Instance.SelectedCoreDressId = 0;
            }

            public void UpdateInfo(uint dressId)
            {
                Sys_Trade.Instance.SelectedCoreDressId = dressId;
                CSVPetEquipSuitAppearance.Data data = CSVPetEquipSuitAppearance.Instance.GetConfData(dressId);
                if (data != null)
                    m_TextDress.text = LanguageHelper.GetTextContent(data.name);
            }
        }

        private class CoreEffect
        {
            private Transform transform;

            public Button m_BtnEffect;
            private Text m_TextEffect;

            public void Init(Transform trans)
            {
                transform = trans;

                m_BtnEffect = transform.Find("Image_input").GetComponent<Button>();
                m_TextEffect = transform.Find("Image_input/Text").GetComponent<Text>();
            }

            public void Reset()
            {
                m_TextEffect.text = "";
            }

            public void UpdateInfo(uint effectId)
            {
                Sys_Trade.Instance.SelectedCoreEffectId = effectId;
                CSVPetEquipEffect.Data data = CSVPetEquipEffect.Instance.GetConfData(effectId);
                if (data != null)
                {
                    CSVPassiveSkillInfo.Data info = CSVPassiveSkillInfo.Instance.GetConfData(data.effect);
                    if (info != null)
                        m_TextEffect.text = LanguageHelper.GetTextContent(info.name);
                }
            }
        }

        private class StateComponent
        {
            private class StateToggle
            {
                private Transform transform;

                private uint m_State;
                private CP_Toggle m_Toggle;
                private System.Action<uint> m_Action;

                public void Init(Transform trans)
                {
                    transform = trans;

                    m_Toggle = trans.GetComponent<CP_Toggle>();
                    m_Toggle.onValueChanged.AddListener(OnClick);
                }

                public void SetState(uint state)
                {
                    m_State = state;
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
                Sys_Trade.Instance.CoreTradeState = state;
            }

            public void UpdateState()
            {
                Sys_Trade.Instance.CoreTradeState = Sys_Trade.Instance.CurPageType == Sys_Trade.PageType.Buy ? (uint)0 : 1;
                int index = Sys_Trade.Instance.CurPageType == Sys_Trade.PageType.Buy ? 0 : 1;
                toggles[index].OnSelect(true);
            }
        }

        private Transform transform;

        private MagicAssign m_MagicAssign;
        private PriceRange m_PriceRange;
        private MagicAttr m_MagicAttr;
        private CoreSuit m_CoreSuit;
        private CoreDress m_CoreDress;
        private CoreEffect m_CoreEffect;
        private StateComponent m_StateCom;

        private UI_Trade_Search_MagicCore_Sort _CoreSort;
        private UI_Trade_Search_MagicCore_Attr _CoreAttr;
        private UI_Trade_Search_MagicCore_Suit _CoreSuit;
        private UI_Trade_Search_MagicCore_Dress _CoreDress;
        private UI_Trade_Search_MagicCore_Effect _CoreEffect;

        private Button m_BtnRest;
        private Button m_BtnSearch;
        private Image m_ImgSearchCost;
        private Text m_TxtSearchCost;

        private MagicAttr.Attr _attrCell;

        public void Init(Transform trans)
        {
            transform = trans;

            m_MagicAssign = new MagicAssign();
            m_MagicAssign.Init(transform.Find("Rect/Rectlist/Image_line0"));
            m_MagicAssign.m_BtnType.onClick.AddListener(() => { _CoreSort.Show(); });

            m_PriceRange = new PriceRange();
            m_PriceRange.Init(transform.Find("Rect/Rectlist/Image_line1"));

            m_MagicAttr = new MagicAttr();
            m_MagicAttr.Init(transform.Find("Rect/Rectlist/Image_Lines"));
            m_MagicAttr.Register(this);

            m_CoreSuit = new CoreSuit();
            m_CoreSuit.Init(transform.Find("Rect/Rectlist/Image_line8/Image_input"));
            m_CoreSuit.m_BtnSuit.onClick.AddListener(OnClickSuit);
            
            m_CoreDress = new CoreDress();
            m_CoreDress.Init(transform.Find("Rect/Rectlist/Image_line8/Image_input2"));
            m_CoreDress.m_BtnDress.onClick.AddListener(OnClickDress);

            m_CoreEffect = new CoreEffect();
            m_CoreEffect.Init(transform.Find("Rect/Rectlist/Image_line12"));
            m_CoreEffect.m_BtnEffect.onClick.AddListener(OnClickEffect);

            m_StateCom = new StateComponent();
            m_StateCom.Init(transform.Find("Rect/Rectlist/Image_line11"));

            _CoreSort = new UI_Trade_Search_MagicCore_Sort();
            _CoreSort.Init(transform.Find("View_Sort"));
            _CoreSort.Hide();

            _CoreAttr = new UI_Trade_Search_MagicCore_Attr();
            _CoreAttr.Init(transform.Find("View_Attribute"));
            _CoreAttr.Hide();

            _CoreSuit = new UI_Trade_Search_MagicCore_Suit();
            _CoreSuit.Init(transform.Find("View_Suit"));
            _CoreSuit.Hide();

            _CoreDress = new UI_Trade_Search_MagicCore_Dress();
            _CoreDress.Init(transform.Find("View_Appearance"));
            _CoreDress.Hide();
            
            _CoreEffect = new UI_Trade_Search_MagicCore_Effect();
            _CoreEffect.Init(transform.Find("View_Special"));
            _CoreEffect.Hide();

            m_BtnSearch = transform.Find("Bottom_button/Btn_01").GetComponent<Button>();
            m_BtnSearch.onClick.AddListener(OnClickSearch);
            m_ImgSearchCost = transform.Find("Bottom_button/Btn_01/Layout/Icon").GetComponent<Image>();
            m_TxtSearchCost = transform.Find("Bottom_button/Btn_01/Layout/Text_Num").GetComponent<Text>();
            SetSearchCostData();

            m_BtnRest = transform.Find("Bottom_button/Btn_02").GetComponent<Button>();
            m_BtnRest.onClick.AddListener(OnClickReset);

            ProcessEvents(true);
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
            ProcessEvents(false);
        }

        private void ProcessEvents(bool register)
        {
            Sys_Trade.Instance.eventEmitter.Handle<uint>(Sys_Trade.EEvents.OnSelectCore, OnSelectCore, register);
            Sys_Trade.Instance.eventEmitter.Handle<uint>(Sys_Trade.EEvents.OnSelectCoreAttr, OnSelectCoreAttr, register);
            Sys_Trade.Instance.eventEmitter.Handle<uint>(Sys_Trade.EEvents.OnSelectCoreSuit, OnSelectCoreSuit, register);
            Sys_Trade.Instance.eventEmitter.Handle<uint>(Sys_Trade.EEvents.OnSelectCoreDress, OnSelectCoreDress, register);
            Sys_Trade.Instance.eventEmitter.Handle<uint>(Sys_Trade.EEvents.OnSelectCoreEffect, OnSelectCoreEffect, register);
        }

        private void SetSearchCostData()
        {
            CSVItem.Data itemData = CSVItem.Instance.GetConfData(Sys_Trade.Instance.GetSearchCostId());
            ImageHelper.SetIcon(m_ImgSearchCost, itemData.small_icon_id);
            m_TxtSearchCost.text = Sys_Trade.Instance.GetSearchCostNum().ToString();
        }

        private void OnClickSearch()
        {
            if (Sys_Trade.Instance.SelectedCoreId == 0u)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011286));
                return;
            }

            CSVCommodity.Data data = CSVCommodity.Instance.GetConfData(Sys_Trade.Instance.SelectedCoreId);
            if (null == data)
            {
                Debug.LogErrorFormat("CSVCommodity 未配置 id = {0}", Sys_Trade.Instance.SelectedCoreId);
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
            if ((TradeShowType)Sys_Trade.Instance.PetTradeState == TradeShowType.Publicity)
            {
                if(!data.publicity)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011204));
                    return;
                }
            }

            //议价
            if ((TradeShowType)Sys_Trade.Instance.PetTradeState == TradeShowType.Discuss)
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
                searchParam.showType = (TradeShowType)Sys_Trade.Instance.CoreTradeState;
                searchParam.searchType = TradeSearchType.PetEquip;
                searchParam.infoId = Sys_Trade.Instance.SelectedCoreId;
                searchParam.Category = cateData.list;
                searchParam.SubCategory = categoryId;
                searchParam.SubClass = subClass;
                searchParam.coreParam = Sys_Trade.Instance.CalCoreParam();

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
            m_MagicAssign.Reset();
            m_PriceRange.Reset();
            m_MagicAttr.Reset();
            m_CoreSuit.Reset();
            m_CoreDress.Reset();
            m_CoreEffect.Reset();
            m_StateCom.Reset();

            Sys_Trade.Instance.ClearSearchCoreData();
        }

        private void OnSelectCore(uint coreId)
        {
            Sys_Trade.Instance.SelectedCoreId = coreId;
            m_MagicAssign.CoreId = coreId;

            _CoreSort.Hide();
        }

        private void OnSelectCoreAttr(uint attrId)
        {
            _attrCell.SetAttrId(attrId);
            _CoreAttr.Hide();
        }

        private void OnSelectCoreSuit(uint suitId)
        {
            m_CoreSuit.UpdateInfo(suitId);
            _CoreSuit.Hide();
            m_CoreDress.Reset(); //选中套装，重置外观
        }

        private void OnSelectCoreDress(uint dressId)
        {
            m_CoreDress.UpdateInfo(dressId);
            _CoreDress.Hide();
        }
        
        private void OnSelectCoreEffect(uint effectId)
        {
            m_CoreEffect.UpdateInfo(effectId);
            _CoreEffect.Hide();
        }

        public void OnClickAttr(MagicAttr.Attr attr)
        {
            _attrCell = attr;
            _CoreAttr.Show();
        }

        public void OnClickSuit()
        {
            if (Sys_Trade.Instance.SelectedCoreId == 0u)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011286));
                return;
            }
            _CoreSuit.Show();
        }

        public void OnClickDress()
        {
            if (Sys_Trade.Instance.SelectedCoreId == 0u)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011286));
                return;
            }
            
            CSVPetEquipSuitSkill.Data suitSkillData = CSVPetEquipSuitSkill.Instance.GetConfData(Sys_Trade.Instance.SelectedCoreSuitSkillId);
            if (suitSkillData != null)
            {
                if (suitSkillData.appearance_id == 0)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011291));
                    return;
                }
            }
            
            _CoreDress.Show();
        }

        public void OnClickEffect()
        {
            if (Sys_Trade.Instance.SelectedCoreId == 0u)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011286));
                return;
            }
            _CoreEffect.Show();
        }

        public void OnRebuildLayout()
        {
            FrameworkTool.ForceRebuildLayout(transform.Find("Rect/Rectlist").gameObject);
        }
    }
}


