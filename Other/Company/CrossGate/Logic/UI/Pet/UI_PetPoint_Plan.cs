using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;
using Packet;
using System;

namespace Logic
{
    public class UI_PetPoint_Plan_Item
    {
        private Transform transform;
        public CP_Toggle toggle;
        private Button addBtn;
        private Button reNameBtn;
        private Text nameLight;
        private Text nameDark;
        public int index;
        private int type;
        public Action<int> addAction;
        public Action<int> renameAction;

        public void Init(Transform _transform)
        {
            transform = _transform;
            toggle = transform.Find("TabItem").GetComponent<CP_Toggle>();
            toggle.onValueChanged.AddListener(onValueChanged);
            addBtn = transform.Find("Button_Add").GetComponent<Button>();
            addBtn.onClick.AddListener(OnAddBtnClicked);
            reNameBtn = transform.Find("TabItem/Button_Rename").GetComponent<Button>();
            reNameBtn.onClick.AddListener(OnReNameBtnClicked);
            nameLight = transform.Find("TabItem/Image_Menu_Light/Text_Menu_Dark").GetComponent<Text>();
            nameDark = transform.Find("TabItem/Btn_Menu_Dark/Text_Menu_Dark").GetComponent<Text>();
        }

        public void Refersh(int _index, string _name, bool _isAdd, int _type)
        {
            index = _index;
            type = _type;
            if (_name == "")
            {
                if (type == (int)Sys_Plan.EPlanType.PetAttribute)
                {
                    nameLight.text = LanguageHelper.GetTextContent(10013504, (index + 1).ToString());
                    nameDark.text = LanguageHelper.GetTextContent(10013504, (index + 1).ToString());
                }
                else
                {
                    nameLight.text = LanguageHelper.GetTextContent(10013505, (index + 1).ToString());
                    nameDark.text = LanguageHelper.GetTextContent(10013505, (index + 1).ToString());
                }
            }
            else
            {
                nameLight.text = _name;
                nameDark.text = _name;
            }
            addBtn.gameObject.SetActive(_isAdd);
            toggle.gameObject.SetActive(!_isAdd);
        }

        public void Destroy()
        {
            addAction = null;
            renameAction = null;
        }

        private  void OnAddBtnClicked()
        {
            addAction?.Invoke(index);
        }

        private void OnReNameBtnClicked()
        {
            renameAction?.Invoke(index);
        }

        private void onValueChanged(bool isOn)
        {
            if (isOn)
            {
                Sys_Pet.Instance.eventEmitter.Trigger<int, int>(Sys_Pet.EEvents.OnSelectAddPointPlan,index,type);
            }
        }
    }

    public class UI_PetPoint_Plan
    {
        private Transform transform;
        private Transform item;
        private PetUnit petUnit;
        private Sys_Plan.EPlanType type;
        private List<UI_PetPoint_Plan_Item> cells = new List<UI_PetPoint_Plan_Item>(); 

        public void Init(Transform _transform)
        {
            transform = _transform;
            item = transform.Find("TabList/Item");
        }

        public void PocessEvent(bool isRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle<uint, uint>(Sys_Pet.EEvents.OnAllocPointPlanRename, OnAllocPointPlanRename,isRegister);
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnAllocPointPlanAdd, OnAllocPointPlanAdd, isRegister);
            Sys_Pet.Instance.eventEmitter.Handle<uint, uint>(Sys_Pet.EEvents.OnCorrectPlanRename, OnCorrectPlanRename, isRegister);
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnCorrectPointPlanAdd, OnCorrectPointPlanAdd, isRegister);
        }

        private void OnCorrectPointPlanAdd(uint petUid)
        {
            if (type != Sys_Plan.EPlanType.PetAttributeCorrect)
            {
                return;
            }
            if (petUnit.Uid == petUid)
            {
                petUnit = Sys_Pet.Instance.GetPetByUId(petUid).petUnit;
                Default();
                AddItems();
            }
        }

        private void OnCorrectPlanRename(uint petUid, uint index)
        {
            if (type != Sys_Plan.EPlanType.PetAttributeCorrect)
            {
                return;
            }
            if (petUnit.Uid == petUid)
            {
                petUnit = Sys_Pet.Instance.GetPetByUId(petUid).petUnit;
                for (int i = 0; i < cells.Count; ++i)
                {
                    if (cells[i].index == index)
                    {
                        cells[i].Refersh((int)index, petUnit.EnhancePlansData.Plans[(int)index].PlanName.ToStringUtf8(), i == (cells.Count - 1), (int)Sys_Plan.EPlanType.PetAttributeCorrect);
                    }
                }
            }
        }

        private void OnAllocPointPlanAdd(uint petUid)
        {
            if (type != Sys_Plan.EPlanType.PetAttribute)
            {
                return;
            }
            if (petUnit.Uid == petUid)
            {
                petUnit = Sys_Pet.Instance.GetPetByUId(petUid).petUnit;
                Default();
                AddItems();
            }
        }

        private void OnAllocPointPlanRename(uint petUid, uint index)
        {
            if (type != Sys_Plan.EPlanType.PetAttribute)
            {
                return;
            }
            if (petUnit.Uid== petUid)
            {
                petUnit = Sys_Pet.Instance.GetPetByUId(petUid).petUnit;
                for(int i = 0; i < cells.Count; ++i)
                {
                    if (cells[i].index == index)
                    {
                        cells[i].Refersh((int)index, petUnit.PetPointPlanData.Plans[(int)index].PlanName.ToStringUtf8(), i == (cells.Count - 1), (int)Sys_Plan.EPlanType.PetAttribute);
                    }
                }
            }
        }

        public void Refresh(PetUnit _petUnit, Sys_Plan.EPlanType _type)
        {
            petUnit = _petUnit;
            type = _type;
            Default();
            AddItems();
        }

        private void AddItems()
        {
            cells.Clear();
            if (type == Sys_Plan.EPlanType.PetAttribute)
            {
                FrameworkTool.CreateChildList(item.parent.transform, petUnit.PetPointPlanData.Plans.Count + 1);
                for (int i = 0; i <= petUnit.PetPointPlanData.Plans.Count; ++i)
                {
                    if (i != 0)
                    {
                        item.parent.transform.name = i.ToString();
                    }
                    UI_PetPoint_Plan_Item cell = new UI_PetPoint_Plan_Item();
                    cell.Init(item.parent.transform.GetChild(i));
                    if (i == petUnit.PetPointPlanData.Plans.Count)
                    {
                        cell.Refersh(i, string.Empty, true, (int) type);
                    }
                    else
                    {
                        cell.Refersh(i, petUnit.PetPointPlanData.Plans[i].PlanName.ToStringUtf8(), false, (int) type);
                    }
                    cell.addAction = OnAddAction;
                    cell.renameAction = OnRenamection;
                    cells.Add(cell);
                    cell.toggle.SetSelected(i == petUnit.PetPointPlanData.CurrentPlanIndex, true);
                }
            }
            else if (type == Sys_Plan.EPlanType.PetAttributeCorrect)
            {
                FrameworkTool.CreateChildList(item.parent.transform, petUnit.EnhancePlansData.Plans.Count + 1);
                for (int i = 0; i <= petUnit.EnhancePlansData.Plans.Count; ++i)
                {
                    if (i != 0)
                    {
                        item.parent.transform.name = i.ToString();
                    }
                    UI_PetPoint_Plan_Item cell = new UI_PetPoint_Plan_Item();
                    cell.Init(item.parent.transform.GetChild(i));
                    if (i == petUnit.EnhancePlansData.Plans.Count)
                    {
                        cell.Refersh(i, string.Empty, true, (int)type);
                    }
                    else
                    {
                        cell.Refersh(i, petUnit.EnhancePlansData.Plans[i].PlanName.ToStringUtf8(), false, (int)type);
                    }
                    cell.addAction = OnAddAction;
                    cell.renameAction = OnRenamection;
                    cells.Add(cell);
                    cell.toggle.SetSelected(i == petUnit.EnhancePlansData.CurrentPlanIndex, true);
                }
            }
        }

        private void Default()
        {
            for(int i = 0; i < cells.Count; ++i)
            {
                cells[i].Destroy();
            }
            if (item != null)
            {
                FrameworkTool.DestroyChildren(item.parent.gameObject, item.name);
            }
        }

        private void OnRenamection(int index)
        {
            void OnRename(int schIndex, int __, string newName)
            {
                if (type == Sys_Plan.EPlanType.PetAttribute)
                {
                    Sys_Pet.Instance.AllocPointPlanRenameReq(petUnit.Uid, (uint)schIndex, newName);
                }
                else if (type == Sys_Plan.EPlanType.PetAttributeCorrect)
                {
                    Sys_Pet.Instance.AllocEnhancePlanRenameReq(petUnit.Uid, (uint)schIndex, newName);
                }
            }

            if (type == Sys_Plan.EPlanType.PetAttribute)
            {
                UI_ChangeSchemeName.ChangeNameArgs arg = new UI_ChangeSchemeName.ChangeNameArgs()
                {
                    arg1 = index,
                    arg2 = 0,
                    oldName = petUnit.PetPointPlanData.Plans[index].PlanName.ToStringUtf8(),
                    onYes = OnRename
                };
                UIManager.OpenUI(EUIID.UI_ChangeSchemeName, false, arg);
            }
            else if (type == Sys_Plan.EPlanType.PetAttributeCorrect)
            {
                UI_ChangeSchemeName.ChangeNameArgs arg = new UI_ChangeSchemeName.ChangeNameArgs()
                {
                    arg1 = index,
                    arg2 = 0,
                    oldName = petUnit.EnhancePlansData.Plans[index].PlanName.ToStringUtf8(),
                    onYes = OnRename
                };
                UIManager.OpenUI(EUIID.UI_ChangeSchemeName, false, arg);
            }
        }

        private void OnAddAction(int index)
        {
            bool valid = false;
            IniElement_IntArray rlt = new IniElement_IntArray();
            uint lan = 0;
            void OnConform()
            {
                if (type == Sys_Plan.EPlanType.PetAttribute)
                {
                    if (Sys_Bag.Instance.GetItemCount(2) < rlt.value[2])
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5602));
                    }
                    else
                    {
                        // 请求server新增方案，同时给新方案设置空的数据
                        Sys_Pet.Instance.AllocPointPlanAddReq(petUnit.Uid);
                    }
                }
                else if (type == Sys_Plan.EPlanType.PetAttributeCorrect)
                {
                     if (Sys_Bag.Instance.GetItemCount(2) < rlt.value[2])
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5602));
                    }
                    else
                    {
                        // 请求server新增方案，同时给新方案设置空的数据
                        Sys_Pet.Instance.AllocEnhancePlanAddReq(petUnit.Uid);
                    }
                }
            }

            bool isOpen = CSVCheckseq.Instance.GetConfData(12102).IsValid();
            if (isOpen)
            {
                if (type == Sys_Plan.EPlanType.PetAttribute)
                {
                    lan = 10013809;
                    valid = (Sys_Ini.Instance.Get<IniElement_IntArray>(1434, out rlt) && rlt.value.Length >= 3);
                    if (petUnit.PetPointPlanData.Plans.Count >= rlt.value[1])
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10013704));
                        return;
                    }
                }
                else if (type == Sys_Plan.EPlanType.PetAttributeCorrect)
                {
                    lan = 10013810;
                    valid = (Sys_Ini.Instance.Get<IniElement_IntArray>(1435, out rlt) && rlt.value.Length >= 3);
                    if (petUnit.EnhancePlansData.Plans.Count >= rlt.value[1])
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10013705));
                        return;
                    }
                }

                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(lan, rlt.value[2].ToString());
                PromptBoxParameter.Instance.SetConfirm(true, OnConform);
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10013901));
            }
        }
    }   
}
