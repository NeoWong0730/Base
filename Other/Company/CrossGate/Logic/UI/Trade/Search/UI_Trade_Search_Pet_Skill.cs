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
    public class UI_Trade_Search_Pet_Skill :
        UI_Trade_Search_Pet_Skill.SkillCell.IListener, 
        UI_Trade_Search_Pet_Skill.SkillType.IListener, 
        UI_Trade_Search_Pet_Skill.SkillGrade.IListener
    {
        private class SelectedShow
        {
            private Transform transform;
            private GameObject gameObject;

            private Text[] _textArray = new Text[10];

            public void Init(Transform trans)
            {
                transform = trans;
                gameObject = transform.gameObject;

                for (int i = 0; i < 2; ++i)
                {
                    for (int j = 0; j < 5; ++j)
                    {
                        _textArray[i * 5 + j] = transform.Find(string.Format("Items{0}/Item{1}/Text", i, j)).GetComponent<Text>();
                    }
                }
            }

            public void UpdateSelectInfo()
            {
                List<uint> ids = Sys_Trade.Instance.SelectedSkillIds;

                for (int i = 0; i < _textArray.Length; ++i)
                    _textArray[i].transform.parent.gameObject.SetActive(false);

                for (int i = 0; i < ids.Count; ++i)
                {
                    if (i < _textArray.Length)
                    {
                        _textArray[i].transform.parent.gameObject.SetActive(true);

                        if (Sys_Skill.Instance.IsActiveSkill(ids[i]))
                        {
                            CSVActiveSkillInfo.Data currentSkillData = CSVActiveSkillInfo.Instance.GetConfData(ids[i]);
                            if (currentSkillData != null)
                            {
                                _textArray[i].text = LanguageHelper.GetTextContent(currentSkillData.name);
                            }
                            else
                            {
                                _textArray[i].text = string.Format("{0} is Null", ids[i]);
                            }
                        }
                        else
                        {
                            CSVPassiveSkillInfo.Data currentSkillData = CSVPassiveSkillInfo.Instance.GetConfData(ids[i]);
                            if (currentSkillData != null)
                            {
                                _textArray[i].text = LanguageHelper.GetTextContent(currentSkillData.name);
                            }
                            else
                            {
                                _textArray[i].text = string.Format("{0} is Null", ids[i]);
                            }
                        }
                    }
                }

                FrameworkTool.ForceRebuildLayout(transform.gameObject);
            }
        }

        public class SkillType 
        {
            private class TypeToggle 
            {
                private Transform transform;
                private GameObject gameObject;

                private CP_Toggle m_Toggle;
                private Text _TextName;
                private Text _TextLightName;

                private System.Action<uint> m_Action;

                private uint _skillType = 0;

                public void Init(Transform trans)
                {
                    transform = trans;
                    gameObject = transform.gameObject;

                    m_Toggle = transform.GetComponent<CP_Toggle>();
                    m_Toggle.onValueChanged.AddListener(OnClick);

                    _TextName = transform.Find("Text").GetComponent<Text>();
                    _TextLightName = transform.Find("Checkmark/LightText").GetComponent<Text>();
                }

                private void OnClick(bool isOn)
                {
                    if (isOn)
                    {
                        m_Action?.Invoke((uint)_skillType);
                    }
                }

                public void Register(System.Action<uint> action)
                {
                    m_Action = action;
                }

                public void UpdateInfo(uint skillType)
                {
                    _skillType = skillType;
                    _TextName.text = _TextLightName.text = LanguageHelper.GetTextContent(2011190 + _skillType);

                    OnSelect(Sys_Trade.Instance.SelectedSkillType == skillType);
                }

                public void OnSelect(bool isOn)
                {
                    m_Toggle.SetSelected(isOn, true);
                }
            }

            private Transform transform;
            private GameObject gameObject;

            private InfinityGridLayoutGroup gridGroup;
            private int visualGridCount;
            private Dictionary<GameObject, TypeToggle> dicCells = new Dictionary<GameObject, TypeToggle>();

            private List<uint> typeIds = new List<uint> { 0, 1, 2, 3 };
            public uint SkillTypId { get; set; }
            private IListener _Listener;

            public void Init(Transform trans)
            {
                transform = trans;
                gameObject = transform.gameObject;

                gridGroup = transform.Find("Toggles").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
                gridGroup.minAmount = 6;
                gridGroup.updateChildrenCallback = UpdateChildrenCallback;

                for (int i = 0; i < gridGroup.transform.childCount; ++i)
                {
                    Transform tran = gridGroup.transform.GetChild(i);

                    TypeToggle cell = new TypeToggle();
                    cell.Init(tran);
                    cell.Register(OnSkillType);
                    dicCells.Add(tran.gameObject, cell);
                }
            }

            private void UpdateChildrenCallback(int index, Transform trans)
            {
                if (index < 0 || index >= visualGridCount)
                    return;

                if (dicCells.ContainsKey(trans.gameObject))
                {
                    TypeToggle cell = dicCells[trans.gameObject];
                    cell.UpdateInfo(typeIds[index]);
                }
            }

            private void OnSkillType(uint skillType)
            {
                SkillTypId = skillType;
                Sys_Trade.Instance.SelectedSkillType = SkillTypId;
                _Listener?.OnClickType();
            }

            public void Register(IListener listener)
            {
                _Listener = listener;
            }

            public void OnUpdateInfo()
            {
                SkillTypId = 0;
                Sys_Trade.Instance.SelectedSkillType = SkillTypId;

                visualGridCount = typeIds.Count;
                gridGroup.SetAmount(visualGridCount);
            }

            public interface IListener
            {
                void OnClickType();
            }
        }

        public class SkillCell
        {
            private Transform transform;
            private GameObject gameObject;

            private Button _btn;
            private Text _textName;
            private Text _textSelectName;
            private Image _imgSelect;

            private Image _imgSkillQuality;
            private Image _imgSkillIcon;

            public uint SkillId { set; get; } = 0u;
            public bool IsSelect { set; get; } = false;

            private IListener _listener;

            public void Init(Transform trans)
            {
                transform = trans;
                gameObject = transform.gameObject;

                _btn = transform.Find("Image_BG").GetComponent<Button>();
                _btn.onClick.AddListener(OnClick);

                _imgSelect = transform.Find("Select").GetComponent<Image>();
                _imgSelect.gameObject.SetActive(false);

                _textName = transform.Find("Text_name").GetComponent<Text>();
                _textSelectName = transform.Find("Select/Text_name").GetComponent<Text>();

                _imgSkillQuality = transform.Find("PetSkillItem01/Image_Quality").GetComponent<Image>();
                _imgSkillIcon = transform.Find("PetSkillItem01/Image_Skill").GetComponent<Image>();
            }

            private void OnClick()
            {
                _listener?.OnSelectSkill(this);
            }

            public void Register(IListener listener)
            {
                _listener = listener;
            }

            public void UpdateInfo(uint skillId)
            {
                SkillId = skillId;
                _imgSkillIcon.gameObject.SetActive(false);

                if (Sys_Skill.Instance.IsActiveSkill(SkillId))
                {
                    CSVActiveSkillInfo.Data currentSkillData = CSVActiveSkillInfo.Instance.GetConfData(SkillId);
                    if (currentSkillData != null)
                    {
                        _textName.text = _textSelectName.text = LanguageHelper.GetTextContent(currentSkillData.name);

                        //ImageHelper.SetIcon(_imgSkillQuality, currentSkillData.quality);
                        ImageHelper.SetIcon(_imgSkillIcon, currentSkillData.icon);
                        _imgSkillIcon.gameObject.SetActive(true);
                    }
                    else
                    {
                        _textName.text = _textSelectName.text = string.Format("{0} is Null", SkillId);
                    }
                }
                else
                {
                    CSVPassiveSkillInfo.Data currentSkillData = CSVPassiveSkillInfo.Instance.GetConfData(SkillId);
                    if (currentSkillData != null)
                    {
                        _textName.text = _textSelectName.text = LanguageHelper.GetTextContent(currentSkillData.name);

                        //ImageHelper.SetIcon(_imgSkillQuality, currentSkillData.quality);
                        ImageHelper.SetIcon(_imgSkillIcon, currentSkillData.icon);
                        _imgSkillIcon.gameObject.SetActive(true);
                    }
                    else
                    {
                        _textName.text = _textSelectName.text = string.Format("{0} is Null", SkillId);
                    }
                }

                OnSelect(Sys_Trade.Instance.SelectedSkillIds.Contains(SkillId));
            }

            public void OnSelect(bool isSelect)
            {
                IsSelect = isSelect;
                _imgSelect.gameObject.SetActive(isSelect);
            }

            public interface IListener
            {
                void OnSelectSkill(SkillCell cell);
            }
        }

        private class SkillGrade
        {
            private class GradeToggle
            {
                private Transform transform;
                private GameObject gameObject;

                private CP_Toggle m_Toggle;
                private Text _textGrade;

                private System.Action<uint> m_Action;
                private uint _grade;

                public void Init(Transform trans)
                {
                    transform = trans;
                    gameObject = transform.gameObject;

                    m_Toggle = transform.GetComponent<CP_Toggle>();
                    m_Toggle.onValueChanged.AddListener(OnClick);

                    _textGrade = transform.Find("Text").GetComponent<Text>();
                }

                private void OnClick(bool isOn)
                {
                    if (isOn)
                    {
                        m_Action?.Invoke(_grade);
                    }
                }

                public void SetGrade(uint grade)
                {
                    _grade = grade;

                    _textGrade.text = LanguageHelper.GetTextContent(2011194 + _grade - 1);
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
            private GameObject gameObject;

            private List<GradeToggle> toggles = new List<GradeToggle>();
            private IListener _listener;

            public uint Grade { get; set; }

            public void Init(Transform trans)
            {
                transform = trans;
                gameObject = transform.gameObject;

                Transform toggleTrans = transform.Find("Toggle");
                int count = toggleTrans.childCount;
                for (int i = 0; i < count; ++i)
                {
                    GradeToggle toggle = new GradeToggle();
                    toggle.Init(toggleTrans.GetChild(i));
                    toggle.SetGrade((uint)(i + 1));
                    toggle.OnSelect(false);
                    toggle.Register(OnSelectGrade);
                    toggles.Add(toggle);
                }
            }

            private void OnSelectGrade(uint grade)
            {
                Grade = grade;
                _listener?.OnClickGrade();
            }

            public void Register(IListener listener)
            {
                _listener = listener;
            }

            public void OnUpdateInfo()
            {
                Grade = 1;
                int index = (int)Grade - 1;
                toggles[index].OnSelect(true);
            }

            public interface IListener
            {
                void OnClickGrade();
            }
        }

        private Transform transform;
        private GameObject gameObject;

        private SelectedShow _selectedShow;
        private SkillType _skillType;
        private SkillGrade _skillGrade;

        private InfinityGridLayoutGroup gridGroup;
        private int visualGridCount;
        private Dictionary<GameObject, SkillCell> dicCells = new Dictionary<GameObject, SkillCell>();

        private Button _btnClose;
        private Button _btnConfirm;

        private List<uint> skillIds = new List<uint>(32);

        public void Init(Transform trans)
        {
            transform = trans;
            gameObject = transform.gameObject;

            _selectedShow = new SelectedShow();
            _selectedShow.Init(transform.Find("Background_Root/Image_Top"));

            _skillType = new SkillType();
            _skillType.Init(transform.Find("Background_Root/Toggles_Mask"));
            _skillType.Register(this);

            _skillGrade = new SkillGrade();
            _skillGrade.Init(transform.Find("Background_Root/Bottom"));
            _skillGrade.Register(this);

            gridGroup = transform.Find("Background_Root/Scroll_View/Rect/Rectlist").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            gridGroup.minAmount = 12;
            gridGroup.updateChildrenCallback = UpdateChildrenCallback;

            for (int i = 0; i < gridGroup.transform.childCount; ++i)
            {
                Transform tran = gridGroup.transform.GetChild(i);

                SkillCell cell = new SkillCell();
                cell.Init(tran);
                cell.Register(this);
                dicCells.Add(tran.gameObject, cell);
            }

            _btnClose = transform.Find("Interrcept_Image").GetComponent<Button>();
            _btnClose.onClick.AddListener(() => { Hide(); });

            _btnConfirm = transform.Find("Background_Root/Bottom2/Btn_04").GetComponent<Button>();
            _btnConfirm.onClick.AddListener(OnClickConfirm);
        }

        public void Show()
        {
            gameObject.SetActive(true);

            _selectedShow.UpdateSelectInfo();
            _skillType.OnUpdateInfo();
            _skillGrade.OnUpdateInfo();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnClickConfirm()
        {
            Sys_Trade.Instance.eventEmitter.Trigger(Sys_Trade.EEvents.OnSelectPetSkills);
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= visualGridCount)
                return;

            if (dicCells.ContainsKey(trans.gameObject))
            {
                SkillCell cell = dicCells[trans.gameObject];
                cell.UpdateInfo(skillIds[index]);
            }
        }

        private void CalSkillIds()
        {
            skillIds.Clear();

            foreach(var data in CSVPetNewSkillSort.Instance.GetAll())
            {
                if (data.grade == _skillGrade.Grade)
                {
                    if (_skillType.SkillTypId == 0u)
                    {
                        skillIds.Add(data.id);
                    }
                    else if (data.type == _skillType.SkillTypId)
                    {
                        skillIds.Add(data.id);
                    }
                }   
            }

            visualGridCount = skillIds.Count;
            gridGroup.SetAmount(visualGridCount);
        }

        public void OnSelectSkill(SkillCell cell)
        {
            if (!cell.IsSelect)
            {
                if (Sys_Trade.Instance.SelectedSkillIds.Count < 10)
                {
                    Sys_Trade.Instance.SelectedSkillIds.Add(cell.SkillId);
                    cell.OnSelect(true);

                    _selectedShow.UpdateSelectInfo();
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011180));
                }
            }
            else
            {
                Sys_Trade.Instance.SelectedSkillIds.Remove(cell.SkillId);
                cell.OnSelect(false);
                _selectedShow.UpdateSelectInfo();
            }
        }

        public void OnClickType()
        {
            //Debug.LogErrorFormat("OnClickType");
            CalSkillIds();
        }

        public void OnClickGrade()
        {
            //Debug.LogErrorFormat("OnClickGrade");
            CalSkillIds();
        }
    }
}


