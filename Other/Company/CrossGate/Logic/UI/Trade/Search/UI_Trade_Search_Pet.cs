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
    public class UI_Trade_Search_Pet : UI_Trade_Search_Pet.PetQuality.IListener
    {
        private class PetAssign
        {
            //private class PetType
            //{
            //    private Transform transform;

            //    private int _petType;
            //    private CP_Toggle _toggle;
            //    private System.Action<int> _action;

            //    public void Init(Transform trans)
            //    {
            //        transform = trans;

            //        _toggle = trans.GetComponent<CP_Toggle>();
            //        _toggle.onValueChanged.AddListener(OnClickToggle);
            //    }

            //    public void SetPetType(int index)
            //    {
            //        _petType = index;
            //    }

            //    private void OnClickToggle(bool isOn)
            //    {
            //        if (isOn)
            //            _action?.Invoke(_petType);
            //    }

            //    public void Register(System.Action<int> action)
            //    {
            //        _action = action;
            //    }

            //    public void OnSelect(bool isOn)
            //    {
            //        _toggle.SetSelected(isOn, true);
            //    }
            //}

            private Transform transform;

            public Button m_BtnType;
            private Text m_TextType;
            private Button m_BtnTip;

            //private List<PetType> listTypes = new List<PetType>(2);
            private uint _petId = 0u;
            public uint PetId {
                get {
                    return _petId;
                }
                set {
                    _petId = value;
                    SetName();
                }
            }
            public int IPetType { get; set; }

            public void Init(Transform trans)
            {
                transform = trans;

                m_BtnType = transform.Find("Image_input").GetComponent<Button>();
                m_TextType = transform.Find("Image_input/Text").GetComponent<Text>();
                m_BtnTip = transform.Find("Button").GetComponent<Button>();
                m_BtnTip.onClick.AddListener(OnClickTip);

                //Transform typeTrans = transform.Find("Toggle");
                //int count = typeTrans.childCount;
                //for (int i = 0; i < count; ++i)
                //{
                //    PetType type = new PetType();
                //    type.Init(typeTrans.GetChild(i));
                //    type.SetPetType(i);
                //    type.Register(OnSelectType);
                //    type.OnSelect(false);
                //    listTypes.Add(type);
                //}

                InitInfo();
            }

            private void OnClickTip()
            {
                if (Sys_Trade.Instance.SelectedPetId == 0u)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011178));
                    return;
                }

                UIManager.OpenUI(EUIID.UI_Trade_Search_Pet_Tips, false, Sys_Trade.Instance.SelectedPetId);
            }

            private void OnSelectType(int type)
            {
                IPetType = type;
            }

            private void InitInfo()
            {
                IPetType = 0;
                //listTypes[0].OnSelect(true);
            }

            private void SetName()
            {
                CSVPetNew.Data data = CSVPetNew .Instance.GetConfData(_petId);
                if (data != null)
                    m_TextType.text = LanguageHelper.GetTextContent(data.name);
            }

            public void Reset()
            {
                _petId = 0;
                m_TextType.text = "";

                InitInfo();
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
                if (Sys_Trade.Instance.PetPriceRange[1] != 0
                    && num > Sys_Trade.Instance.PetPriceRange[1]) //不能小于最小值
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011198));
                    num = Sys_Trade.Instance.PetPriceRange[1];
                }

                Sys_Trade.Instance.PetPriceRange[0] = num;
                m_PriceMin.SetData(num);
            }

            private void OnInputEndMax(uint num)
            {
                if (num < Sys_Trade.Instance.PetPriceRange[0]) //不能小于最小值
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011198));
                    num = Sys_Trade.Instance.PetPriceRange[0];
                }

                Sys_Trade.Instance.PetPriceRange[1] = num;
                m_PriceMax.SetData(num);
            }

            public void Reset()
            {
                m_PriceMin.Reset();
                m_PriceMax.Reset();
            }
        }

        public class PetQuality : PetQuality.Quality.IListener
        {
            public class Quality
            {
                private Transform transform;

                private Button m_BtnQuality;
                private Text m_TextQuality;

                private Transform m_TransArrow;

                private UI_Common_Num m_InputNum;

                private Button m_BtnMinus;
                private Button m_BtnAdd;

                private Sys_Trade.PetQualityData _qualityData;

                private IListener _listener;

                public void Init(Transform trans)
                {
                    transform = trans;

                    m_BtnQuality = transform.Find("Image_input0").GetComponent<Button>();
                    m_BtnQuality.onClick.AddListener(OnClickQuality);

                    m_TextQuality = transform.Find("Image_input0/Text").GetComponent<Text>();

                    m_TransArrow = transform.Find("Image_greater");

                    m_InputNum = new UI_Common_Num();
                    m_InputNum.Init(transform.Find("Image_input1"));
                    m_InputNum.RegEnd(OnInputEndNum);

                    //temp
                    transform.Find("Image_Right").gameObject.SetActive(false);
                    
                    m_BtnMinus = transform.Find("Image_Right/Button_Minus").GetComponent<Button>();
                    m_BtnMinus.onClick.AddListener(OnClickMinus);

                    m_BtnAdd = transform.Find("Image_Right/Button_Add").GetComponent<Button>();
                    m_BtnAdd.onClick.AddListener(OnClickAdd);

                    _qualityData = new Sys_Trade.PetQualityData();
                    Sys_Trade.Instance.SelectedQualitys.Add(_qualityData);
                }

                public void Show()
                {
                    transform.gameObject.SetActive(true);
                }

                public void Hide()
                {
                    transform.gameObject.SetActive(false);

                    Sys_Trade.Instance.SelectedQualitys.Remove(_qualityData);
                }

                public void Destroy()
                {
                    GameObject.DestroyImmediate(transform.gameObject, true);
                    Sys_Trade.Instance.SelectedQualitys.Remove(_qualityData);
                }

                public void Reset()
                {
                    // _qualityData.qualityId = 0u;
                    // _qualityData.qualityValue = 0u;
                    //
                    // m_TextQuality.text = "";
                    // m_InputNum.Reset();
                    SetQualityId((uint)Sys_Trade.EPetQuality.lostGrade);
                }

                private void OnClickQuality()
                {
                    _listener?.OnClickQuality(this);
                }

                private void OnInputEndNum(uint num)
                {
                    _qualityData.qualityValue = num;
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

                public void SetQualityId(uint qualityId)
                {
                    _qualityData.qualityId = qualityId;
                    m_TextQuality.text = LanguageHelper.GetTextContent(qualityId + 6);
                    if (qualityId == (uint)Sys_Trade.EPetQuality.lostGrade)
                    {
                        m_TransArrow.localScale = new Vector3(-1, 1, 1);
                        _qualityData.qualityValue = 20;//掉档数默认20
                        m_InputNum.SetData(20);
                    }
                    else
                    {
                        m_TransArrow.localScale = Vector3.one;
                    }
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
                    void OnClickQuality(Quality cell);
                    void OnClickAdd(Quality cell);
                    void OnClickMinus(Quality cell);
                }
            }

            private Transform transform;

            private GameObject _template;

            //private List<Quality> _list = new List<Quality>();
            private Quality tempQuality;

            private IListener _Listener;

            private int _curIndex;

            public void Init(Transform trans)
            {
                transform = trans;

                _template = transform.Find("Image_line2").gameObject;
                //_template.SetActive(false);
                tempQuality = new Quality();
                tempQuality.Init(_template.transform);

                Reset();
            }

            private void GenCell()
            {
                // GameObject go = GameObject.Instantiate(_template, transform);
                // Quality cell = new Quality();
                // cell.Init(go.transform);
                // cell.Register(this);
                // cell.Show();
                // _list.Add(cell);
            }

            public void Reset()
            {
                tempQuality.Reset();
                // FrameworkTool.DestroyChildren(transform.gameObject, _template.name);
                //
                // _list.Clear();
                // GenCell();
                // _list[0].EnableMinus(false);
                // _list[0].EnableAdd(true);
                //
                // _Listener?.OnRebuildLayout();
            }

            public void Register(IListener listener)
            {
                _Listener = listener;
            }

            public void OnClickQuality(Quality cell)
            {
                // if (Sys_Trade.Instance.SelectedPetId == 0u)
                // {
                //     Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011178));
                //     return;
                // }
                //
                // _Listener?.OnClickQaulity(cell);
            }

            public void OnClickMinus(Quality cell)
            {
                // cell.Reset();
                // cell.Hide();
                // _list.Remove(cell);
                // _list[_list.Count - 1].EnableAdd(true);
                //
                // //如果剩一个
                // if (_list.Count == 1)
                //     _list[0].EnableMinus(false);
                //
                // cell.Destroy();
                // _Listener?.OnRebuildLayout();
            }

            public void OnClickAdd(Quality cell)
            {
                // if (Sys_Trade.Instance.SelectedPetId == 0u)
                // {
                //     Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011178));
                //     return;
                // }
                //
                // if (_list.Count >= 6) //最多6条
                // {
                //     Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011179));
                //     return;
                // }
                //
                // _list[_list.Count - 1].EnableAdd(false);
                // _list[_list.Count - 1].EnableMinus(true);
                // GenCell();
                // _list[_list.Count - 1].EnableAdd(true);
                //
                // _Listener?.OnRebuildLayout();
            }

            public interface IListener
            {
                void OnClickQaulity(PetQuality.Quality cellAttr);
                void OnRebuildLayout();
            }
        }

        private class PetSkill
        {
            private Transform transform;

            public Button m_BtnSkill;
            private Text m_TextSkill;

            public void Init(Transform trans)
            {
                transform = trans;

                m_BtnSkill = transform.Find("Image_input").GetComponent<Button>();
                m_TextSkill = transform.Find("Image_input/Text").GetComponent<Text>();
            }

            public void Reset()
            {
                m_TextSkill.text = "";
            }

            public void UpdateInfo()
            {
                List<uint> skillIds = Sys_Trade.Instance.SelectedSkillIds;
                System.Text.StringBuilder strBuilder = StringBuilderPool.GetTemporary();
                for (int i = 0; i < skillIds.Count; ++i)
                {
                    if (Sys_Skill.Instance.IsActiveSkill(skillIds[i]))
                    {
                        CSVActiveSkillInfo.Data currentSkillData = CSVActiveSkillInfo.Instance.GetConfData(skillIds[i]);
                        if (currentSkillData != null)
                            strBuilder.Append(LanguageHelper.GetTextContent(currentSkillData.name));
                    }
                    else
                    {
                        CSVPassiveSkillInfo.Data currentSkillData = CSVPassiveSkillInfo.Instance.GetConfData(skillIds[i]);
                        if (currentSkillData != null)
                            strBuilder.Append(LanguageHelper.GetTextContent(currentSkillData.name));
                    }

                    strBuilder.Append(";");
                }
                m_TextSkill.text = StringBuilderPool.ReleaseTemporaryAndToString(strBuilder);
            }
        }

        private class SkillNum
        {
            private Transform transform;

            private UI_Common_Num m_InputNum;

            public void Init(Transform trans)
            {
                transform = trans;

                m_InputNum = new UI_Common_Num();
                m_InputNum.Init(transform.Find("Image_input"));
                m_InputNum.RegEnd(OnInputEndNum);
            }

            public void Reset()
            {
                m_InputNum.Reset();
            }

            private void OnInputEndNum(uint num)
            {
                Sys_Trade.Instance.PetSkillsNum = num;
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
                Sys_Trade.Instance.PetScore = num;
            }
        }

        private class PetSkillNew
        {
            private Transform transform;

            public Button m_BtnSkill;
            private Text m_TextSkill;

            public void Init(Transform trans)
            {
                transform = trans;

                m_BtnSkill = transform.Find("Image_input").GetComponent<Button>();
                m_TextSkill = transform.Find("Image_input/Text").GetComponent<Text>();
            }

            public void Reset()
            {
                m_TextSkill.text = "";
            }

            public void UpdateInfo()
            {
                List<uint> skillIds = Sys_Trade.Instance.SelectedNewSkillIds;
                System.Text.StringBuilder strBuilder = StringBuilderPool.GetTemporary();
                for (int i = 0; i < skillIds.Count; ++i)
                {
                    CSVPetNewSkillSummary.Data data = CSVPetNewSkillSummary.Instance.GetConfData(skillIds[i]);
                    if (data != null)
                    {
                        strBuilder.Append(LanguageHelper.GetTextContent(data.name));
                    }
                    else
                    {
                        strBuilder.Append(" ");
                    }

                    strBuilder.Append(";");
                }
                m_TextSkill.text = StringBuilderPool.ReleaseTemporaryAndToString(strBuilder);
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
                Sys_Trade.Instance.PetTradeState = state;
            }

            public void UpdateState()
            {
                Sys_Trade.Instance.PetTradeState = Sys_Trade.Instance.CurPageType == Sys_Trade.PageType.Buy ? (uint)0 : 1;
                int index = Sys_Trade.Instance.CurPageType == Sys_Trade.PageType.Buy ? 0 : 1;
                toggles[index].OnSelect(true);
            }
        }

        private Transform transform;

        private PetAssign m_PetAssign;
        private PriceRange m_PriceRange;
        private PetQuality m_PetQuality;
        private PetSkill m_PetSkill; //包含技能
        private SkillNum m_SkillNum;
        private ScoreAssign m_ScoreAssign;
        private PetSkillNew m_PetSkillNew; //新的宠物技能
        private StateComponent m_StateCom;

        private UI_Trade_Search_Pet_Sort _PetSort;
        private UI_Trade_Search_Pet_Quality _PetQuality;
        private UI_Trade_Search_Pet_Skill _PetSkill;
        private UI_Trade_Search_Pet_SkillNew _PetSkillNew;

        private Button m_BtnRest;
        private Button m_BtnSearch;
        private Image m_ImgSearchCost;
        private Text m_TxtSearchCost;

        private PetQuality.Quality _qualityCell;

        public void Init(Transform trans)
        {
            transform = trans;

            m_PetAssign = new PetAssign();
            m_PetAssign.Init(transform.Find("Rect/Rectlist/Image_line0"));
            m_PetAssign.m_BtnType.onClick.AddListener(() => { _PetSort.Show(); });

            m_PriceRange = new PriceRange();
            m_PriceRange.Init(transform.Find("Rect/Rectlist/Image_line1"));

            m_PetQuality = new PetQuality();
            m_PetQuality.Init(transform.Find("Rect/Rectlist/Image_Lines"));
            m_PetQuality.Register(this);

            m_PetSkill = new PetSkill();
            m_PetSkill.Init(transform.Find("Rect/Rectlist/Image_line8"));
            m_PetSkill.m_BtnSkill.onClick.AddListener(OnClickSkill);

            m_SkillNum = new SkillNum();
            m_SkillNum.Init(transform.Find("Rect/Rectlist/Image_line9"));

            //m_ScoreAssign = new ScoreAssign();
            //m_ScoreAssign.Init(transform.Find("Rect/Rectlist/Image_line10"));

            m_PetSkillNew = new PetSkillNew();
            m_PetSkillNew.Init(transform.Find("Rect/Rectlist/Image_line12"));
            m_PetSkillNew.m_BtnSkill.onClick.AddListener(OnClickSkillNew);

            m_StateCom = new StateComponent();
            m_StateCom.Init(transform.Find("Rect/Rectlist/Image_line11"));

            _PetSort = new UI_Trade_Search_Pet_Sort();
            _PetSort.Init(transform.Find("View_Sort"));
            _PetSort.Hide();

            _PetQuality = new UI_Trade_Search_Pet_Quality();
            _PetQuality.Init(transform.Find("View_GrowUp"));
            _PetQuality.Hide();

            _PetSkill = new UI_Trade_Search_Pet_Skill();
            _PetSkill.Init(transform.Find("View_Skill"));
            _PetSkill.Hide();

            _PetSkillNew = new UI_Trade_Search_Pet_SkillNew();
            _PetSkillNew.Init(transform.Find("View_PetSkill"));
            _PetSkillNew.Hide();

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
            Sys_Trade.Instance.eventEmitter.Handle<uint>(Sys_Trade.EEvents.OnSelectPet, OnSelectPet, register);
            Sys_Trade.Instance.eventEmitter.Handle<uint>(Sys_Trade.EEvents.OnSelectPetQuality, OnSelectPetQuality, register);
            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnSelectPetSkills, OnSelectPetSkills, register);
            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnSelectNewPetSkills, OnSelectNewPetSkills, register);
        }

        private void SetSearchCostData()
        {
            CSVItem.Data itemData = CSVItem.Instance.GetConfData(Sys_Trade.Instance.GetSearchCostId());
            ImageHelper.SetIcon(m_ImgSearchCost, itemData.small_icon_id);
            m_TxtSearchCost.text = Sys_Trade.Instance.GetSearchCostNum().ToString();
        }

        private void OnClickSearch()
        {
            if (Sys_Trade.Instance.SelectedPetId == 0u)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011178));
                return;
            }

            CSVCommodity.Data data = CSVCommodity.Instance.GetConfData(Sys_Trade.Instance.SelectedPetId);
            if (null == data)
            {
                Debug.LogErrorFormat("CSVCommodity 未配置 id = {0}", Sys_Trade.Instance.SelectedPetId);
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
                searchParam.showType = (TradeShowType)Sys_Trade.Instance.PetTradeState;
                searchParam.searchType = TradeSearchType.Pet;
                searchParam.infoId = Sys_Trade.Instance.SelectedPetId;
                searchParam.Category = cateData.list;
                searchParam.SubCategory = categoryId;
                searchParam.SubClass = subClass;
                searchParam.petParam = Sys_Trade.Instance.CalPetParam();

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
            m_PetAssign.Reset();
            m_PriceRange.Reset();
            m_PetQuality.Reset();
            m_PetSkill.Reset();
            m_SkillNum.Reset();
            //m_ScoreAssign.Reset();
            m_PetSkillNew.Reset();
            m_StateCom.Reset();

            Sys_Trade.Instance.ClearSearchPetData();
        }

        private void OnSelectPet(uint petId)
        {
            Sys_Trade.Instance.SelectedPetId = petId;
            m_PetAssign.PetId = petId;

            _PetSort.Hide();
        }

        private void OnSelectPetQuality(uint petQualityId)
        {
            _qualityCell.SetQualityId(petQualityId);
            _PetQuality.Hide();
        }

        private void OnSelectPetSkills()
        {
            m_PetSkill.UpdateInfo();
            _PetSkill.Hide();
        }

        private void OnSelectNewPetSkills()
        {
            m_PetSkillNew.UpdateInfo();
            _PetSkillNew.Hide();
        }

        public void OnClickQaulity(PetQuality.Quality quality)
        {
            _qualityCell = quality;
            _PetQuality.Show();
        }

        public void OnClickSkill()
        {
            if (Sys_Trade.Instance.SelectedPetId == 0u)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011178));
                return;
            }
            _PetSkill.Show();
        }

        public void OnClickSkillNew()
        {
            if (Sys_Trade.Instance.SelectedPetId == 0u)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011178));
                return;
            }
            _PetSkillNew.Show();
        }

        public void OnRebuildLayout()
        {
            FrameworkTool.ForceRebuildLayout(transform.Find("Rect/Rectlist").gameObject);
        }
    }
}


