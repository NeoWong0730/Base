using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using UnityEngine.UI;
using Lib.Core;
using UnityEngine.EventSystems;

namespace Logic
{
    // collect
    public partial class UI_LifeSkill_Message : UIBase
    {
        private GameObject view3;
        private GameObject view3dropDownParent;
        private GameObject view3Arrow_down;
        private GameObject view3Arrow_up;
        private Text m_TextCost;
        private Text view3dropDowmLevel;
        private Image icon;
        private Text name;
        private Text tip;
        private Text area;
        private Text active;
        private Button colectBtn;
        private Button collect_view3MenuBtn;
        private CP_ToggleRegistry CP_ToggleRegistry_Collect;
        private InfinityGrid infinity_collect;
        private Transform infinityParent_collect;
        private GameObject m_CloseBG3;
        private bool _bDropDown_collect;
        private bool bDropDown_collect
        {
            get { return _bDropDown_collect; }
            set
            {
                if (_bDropDown_collect != value)
                {
                    _bDropDown_collect = value;
                    SetView3Drop(_bDropDown_collect);
                }
            }
        }
        private int curView3MenuParentLable;
        private int CurView3MenuParentLable
        {
            get { return curView3MenuParentLable; }
            set
            {
                if (curView3MenuParentLable != value)
                {
                    curView3MenuParentLable = value;
                    curLifeSkillSelectIndex_collect = 0;
                    OnView3MenuParentLableChanged();
                    UpdateView3dropDowmLevel();
                    bDropDown_collect = false;
                }
            }
        }
        private List<uint> dropDownSkillitems = new List<uint>();
        private Dictionary<GameObject, LifeSkillCeil1> ceil1s_collect = new Dictionary<GameObject, LifeSkillCeil1>();
        private int curLifeSkillSelectIndex_collect;
        private uint curSelectedItem;

        private void ParseView3Component()
        {
            view3 = transform.Find("Animator/View03").gameObject;
            m_CloseBG3 = view3.transform.Find("Right/GridList/Image_Close").gameObject;
            m_TextCost = view3.transform.Find("Center/Text_Cost").GetComponent<Text>();
            colectBtn = view3.transform.Find("Center/Btn_01").GetComponent<Button>();
            icon = view3.transform.Find("Center/Image_ICON").GetComponent<Image>();
            name = view3.transform.Find("Center/Text_Name").GetComponent<Text>();
            tip = view3.transform.Find("Center/Text_Tips").GetComponent<Text>();
            active = view3.transform.Find("Center/Text_Stage/Text").GetComponent<Text>();
            area = view3.transform.Find("Center/Text_Tips01").GetComponent<Text>();
            view3dropDownParent = view3.transform.Find("Right/GridList").gameObject;
            CP_ToggleRegistry_Collect = view3dropDownParent.GetComponent<CP_ToggleRegistry>();
            collect_view3MenuBtn = view3.transform.Find("Right/Btn_Menu_Dark").GetComponent<Button>();
            view3Arrow_down = collect_view3MenuBtn.transform.Find("Image_fold01").gameObject;
            view3dropDowmLevel = collect_view3MenuBtn.transform.Find("Text_Menu_Dark").GetComponent<Text>();
            view3Arrow_up = collect_view3MenuBtn.transform.Find("Image_fold").gameObject;
            infinityParent_collect = view3.transform.Find("Right/TargetScroll");
            infinity_collect = infinityParent_collect.gameObject.GetNeedComponent<InfinityGrid>();
            infinity_collect.onCreateCell += OnCreateCell_Collect;
            infinity_collect.onCellChange += OnCellChange_Collect;

            //for (int i = 0; i < infinityParent_collect.childCount; i++)
            //{
            //    GameObject go = infinityParent_collect.GetChild(i).gameObject;
            //    LifeSkillCeil1 lifeSkillCeil1 = new LifeSkillCeil1();
            //    lifeSkillCeil1.BindGameObject(go);
            //    lifeSkillCeil1.AddClickListener(OnCeilCollectSelected);
            //    ceil1s_collect.Add(go, lifeSkillCeil1);
            //}
        }

        private void RegistCollectEvent()
        {
            CP_ToggleRegistry_Collect.onToggleChange = OnView3MenuChildChanged;
            collect_view3MenuBtn.onClick.AddListener(OnView3MenuBtnClicked);
            colectBtn.onClick.AddListener(OnCollectButtonClicked);

            Lib.Core.EventTrigger eventTrigger2 = Lib.Core.EventTrigger.Get(m_CloseBG3);
            eventTrigger2.AddEventListener(EventTriggerType.PointerClick, OnCLoseBg3);
        }

        private void ConstructView3DropDownRoot()
        {
            FrameworkTool.CreateChildList(view3dropDownParent.transform, (int)livingSkill.cSVLifeSkillData.max_level, 1);
            for (int i = 1, count = view3dropDownParent.transform.childCount; i < count; i++)
            {
                GameObject game = view3dropDownParent.transform.GetChild(i).gameObject;
                CP_Toggle cP_Toggle = game.GetComponent<CP_Toggle>();
                cP_Toggle.id = i;
                Text text = game.transform.Find("Text_Menu_Dark").GetComponent<Text>();
                string content = string.Format(CSVLanguage.Instance.GetConfData(2010045).words, i);
                TextHelper.SetText(text, content);
            }
            OnView3MenuParentLableChanged();
            UpdateView3dropDowmLevel();
            bDropDown_collect = false;
            view3Arrow_up.SetActive(true);
            //CurView3MenuParentLable = (int)livingSkill.Level;
            //CP_ToggleRegistry_Collect.SwitchTo((int)livingSkill.Level);
        }

        private void OnRefreshCollect(uint lifeSkill, uint itemId)
        {
            m_CurSkillId = lifeSkill;
            m_CP_ToggleRegisterLeft.SwitchTo((int)m_CurSkillId);
            livingSkill = Sys_LivingSkill.Instance.livingSkills[m_CurSkillId];
            curView3MenuParentLable = (int)CSVItem.Instance.GetConfData(itemId).lv;
            UpdateView3dropDowmLevel();
            dropDownSkillitems = Sys_LivingSkill.Instance.GetUnLockSkillItems(livingSkill.SkillId, (uint)curView3MenuParentLable);
            infinity_collect.CellCount = dropDownSkillitems.Count;
            curSelectedItem = itemId;
            curLifeSkillSelectIndex_collect = dropDownSkillitems.IndexOf(curSelectedItem);
            OnSelectcollect(curLifeSkillSelectIndex_collect);
            RefreshRight_view3();
        }

        private void OnView3MenuChildChanged(int curToggle, int old)
        {
            CurView3MenuParentLable = curToggle;
        }

        private void UpdateView3dropDowmLevel()
        {
            TextHelper.SetText(view3dropDowmLevel, string.Format(CSVLanguage.Instance.GetConfData(2010045).words, CurView3MenuParentLable));
        }

        private void OnView3MenuParentLableChanged()
        {
            dropDownSkillitems = Sys_LivingSkill.Instance.GetUnLockSkillItems(livingSkill.SkillId, (uint)curView3MenuParentLable);
            infinity_collect.CellCount = dropDownSkillitems.Count;
            infinity_collect.ForceRefreshActiveCell();
            curSelectedItem = dropDownSkillitems[curLifeSkillSelectIndex_collect];
            OnSelectcollect(curLifeSkillSelectIndex_collect);
            RefreshRight_view3();
        }

        private void OnSelectcollect(int selectIndex)
        {
            foreach (var item in ceil1s_collect)
            {
                if (item.Value.dataIndex != selectIndex)
                {
                    item.Value.Release();
                }
                else
                {
                    item.Value.Select();
                }
            }
        }

        private void OnCLoseBg3(BaseEventData baseEventData)
        {
            m_CloseBG3.SetActive(false);
            bDropDown_collect = false;
        }

        private void OnView3MenuBtnClicked()
        {
            bDropDown_collect = !bDropDown_collect;
        }

        private void SetView3Drop(bool value)
        {
            view3dropDownParent.SetActive(value);
            view3Arrow_down.SetActive(value);
            view3Arrow_up.SetActive(!value);
            m_CloseBG3.SetActive(value);
            if (value)
            {
                CP_ToggleRegistry_Collect.SetHighLight(CurView3MenuParentLable);
            }
        }

        private void OnCreateCell_Collect(InfinityGridCell cell)
        {
            LifeSkillCeil1 lifeSkillCeil1 = new LifeSkillCeil1();
            lifeSkillCeil1.BindGameObject(cell.mRootTransform.gameObject);
            lifeSkillCeil1.AddClickListener(OnCeilCollectSelected);
            cell.BindUserData(lifeSkillCeil1);
            ceil1s_collect.Add(cell.mRootTransform.gameObject, lifeSkillCeil1);
        }

        private void OnCellChange_Collect(InfinityGridCell cell, int index)
        {
            LifeSkillCeil1 lifeSkillCeil1 = cell.mUserData as LifeSkillCeil1;
            lifeSkillCeil1.SetData(dropDownSkillitems[index], livingSkill.category, index, curView3MenuParentLable <= livingSkill.Level);
            if (index != curLifeSkillSelectIndex_collect)
            {
                lifeSkillCeil1.Release();
            }
            else
            {
                lifeSkillCeil1.Select();
            }
        }

        private void UpdateChildrenCallback1(int index, Transform trans)
        {
            LifeSkillCeil1 lifeSkillCeil1 = ceil1s_collect[trans.gameObject];
            lifeSkillCeil1.SetData(dropDownSkillitems[index], livingSkill.category, index, curView3MenuParentLable <= livingSkill.Level);
            if (index != curLifeSkillSelectIndex_collect)
            {
                lifeSkillCeil1.Release();
            }
            else
            {
                lifeSkillCeil1.Select();
            }
        }

        private void OnCeilCollectSelected(LifeSkillCeil1 lifeSkillCeil1)
        {
            curLifeSkillSelectIndex_collect = lifeSkillCeil1.dataIndex;
            foreach (var item in ceil1s_collect)
            {
                if (item.Value.dataIndex == curLifeSkillSelectIndex_collect)
                {
                    item.Value.Select();
                }
                else
                {
                    item.Value.Release();
                }
            }
            curSelectedItem = lifeSkillCeil1.id;
            RefreshRight_view3();
        }

        private void RefreshRight_view3()
        {
            ImageHelper.SetIcon(icon, CSVItem.Instance.GetConfData(curSelectedItem).icon_id);
            TextHelper.SetText(name, CSVItem.Instance.GetConfData(curSelectedItem).name_id);
            TextHelper.SetText(tip, CSVItem.Instance.GetConfData(curSelectedItem).describe_id);
            string content = CSVLanguage.Instance.GetConfData(2010018).words;
            CSVLifeSkillLevel.Data _cSVLifeSkillLevelData = Sys_LivingSkill.Instance.
                GetLifeSkillLevelData(livingSkill.cSVLifeSkillData.id, (uint)curView3MenuParentLable);
            string mapName = CSVLanguage.Instance.GetConfData(CSVMapInfo.Instance.GetConfData(_cSVLifeSkillLevelData.map_id).name).words;
            TextHelper.SetText(area, string.Format(content, mapName));

            if (_cSVLifeSkillLevelData.cost_vitality != 0)
            {
                m_TextCost.gameObject.SetActive(true);
                TextHelper.SetText(m_TextCost, LanguageHelper.GetTextContent(2010140, _cSVLifeSkillLevelData.cost_vitality.ToString()));
            }
            else
            {
                m_TextCost.gameObject.SetActive(false);
            }
            uint activeId = 0;
            bool contains = false;
            if (_cSVLifeSkillLevelData.active_npc == null)
            {
                contains = false;
            }
            else
            {
                foreach (var item in _cSVLifeSkillLevelData.active_npc)
                {
                    if (Sys_Npc.Instance.IsActivatedNpc(item))
                    {
                        contains = true;
                        break;
                    }
                }
            }
            if (!contains)
            {
                activeId = 2010138;
            }
            else
            {
                activeId = 2010139;
            }
            TextHelper.SetText(active, activeId);
        }

        private void OnCollectButtonClicked()
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
            {
                Sys_Hint.Instance.PushForbidOprationInFight();  //战斗内提示：当前处于战斗中，无法进行该操作
                return;
            }
            if (curView3MenuParentLable > livingSkill.Level)
            {
                //string content = CSVLanguage.Instance.GetConfData(2010113).words;
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010113, CSVLanguage.Instance.GetConfData(livingSkill.cSVLifeSkillData.name_id).words, curView3MenuParentLable.ToString()));
                return;
            }
            CSVLifeSkillLevel.Data _cSVLifeSkillLevelData = Sys_LivingSkill.Instance.
               GetLifeSkillLevelData(livingSkill.cSVLifeSkillData.id, (uint)curView3MenuParentLable);
            bool contains = false;
            if (_cSVLifeSkillLevelData.active_npc == null)
            {
                contains = false;
            }
            else
            {
                foreach (var item in _cSVLifeSkillLevelData.active_npc)
                {
                    if (Sys_Npc.Instance.IsActivatedNpc(item))
                    {
                        contains = true;
                        break;
                    }
                }
            }
            if (!contains)
            {
                string content = CSVLanguage.Instance.GetConfData(2010112).words;
                Sys_Hint.Instance.PushContent_Normal(content);
                return;
            }
            if (Sys_Bag.Instance.GetItemCount(5) < _cSVLifeSkillLevelData.cost_vitality)
            {
                string content = LanguageHelper.GetTextContent(2010114);
                Sys_Hint.Instance.PushContent_Normal(content);
                return;
            }
            UIManager.CloseUI(EUIID.UI_LifeSkill_Message);
            //UIManager.CloseUI(EUIID.UI_LifeSkill);
            CollectionCtrl.Instance.StartCollection(_cSVLifeSkillLevelData.collection_npc);
        }
    }
}

