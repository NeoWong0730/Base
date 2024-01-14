using Lib.Core;
using Logic.Core;
using UnityEngine.UI;
using UnityEngine;
using Table;
using System;
using Packet;
using System.Collections.Generic;

namespace Logic
{
    public class UI_Pet_DemonSkill_Refresh : UIBase
    {
        private Button btnClose;
        private Button previewBtn;
        private Button refreshBtn;
        private Button saveBtn;
        private Button cancelBtn;
        private Transform costItemParent;
        private GameObject skill1Go;
        private GameObject skill2Go;
        private Animator ani;
        private uint sphereType = 0;
        private int skillIndex = 0;
        private uint sphereIndex = 0;
        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                if (arg is Tuple<uint, uint, uint> tuple)
                {
                    sphereType = tuple.Item1;
                    sphereIndex = tuple.Item2;
                    skillIndex = (int)tuple.Item3;
                }
                else
                {
                    sphereType = 0;
                    sphereIndex = 0;
                    skillIndex = 0;
                }
            }
            else
            {
                sphereType = 0;
                sphereIndex = 0;
                skillIndex = 0;
            }
        }
        private List<List<uint>> refreshCost;
        private List<List<uint>> RefreshCost
        {
            get
            {
                if(null == refreshCost)
                {
                    CSVPetNewParam.Data param = CSVPetNewParam.Instance.GetConfData(87u);
                    if(null != param)
                    {
                        refreshCost = ReadHelper.ReadArray2_ReadUInt(param.str_value, '|', '&');
                    }
                    else
                    {
                        refreshCost = new List<List<uint>>();
                        DebugUtil.LogError("CSVPetNewParam Not Find id = 87");
                    }
                    
                }
                return refreshCost;
            }
        }
        protected override void OnLoaded()
        {
            btnClose = transform.Find("Animator/View_TipsBgNew07/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnClickClose);
            refreshBtn = transform.Find("Animator/View_SkillRebuild/View_Rebuild/Btn_Rebuild").GetComponent<Button>();
            refreshBtn.onClick.AddListener(OnClickRefresh);
            saveBtn = transform.Find("Animator/View_SkillRebuild/View_Rebuild/Btn_Save").GetComponent<Button>();
            saveBtn.onClick.AddListener(OnClickSave);
            cancelBtn = transform.Find("Animator/View_SkillRebuild/View_Rebuild/Btn_Delete").GetComponent<Button>();
            cancelBtn.onClick.AddListener(OnClickCancel);
            previewBtn = transform.Find("Animator/View_SkillRebuild/View_Rebuild/Skill_1/Btn_Preview").GetComponent<Button>();
            previewBtn.onClick.AddListener(OnClickPreview);
            costItemParent = transform.Find("Animator/View_SkillRebuild/View_Rebuild/Cost");
            skill1Go = transform.Find("Animator/View_SkillRebuild/View_Rebuild/Skill_1").gameObject;
            skill2Go = transform.Find("Animator/View_SkillRebuild/View_Rebuild/Skill2").gameObject;
            ani = transform.Find("Animator/View_SkillRebuild/View_Rebuild").GetComponent<Animator>();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnRefreshDemonSpiritSkill, OnRefreshDemonSpiritSkill, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnPetRemakeRecastTipsEntry, OnRightBtnClicked, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnSaveOrDelectDemonSpiritSphereSkill, UpdatePanel, toRegister);
        }
        
        private void OnRightBtnClicked(uint type)
        {
            if (type == 6)
            {
                Sys_Pet.Instance.PetSoulBeadOperateReq((uint)PetSoulBeadOpType.SoulBeadOpTypeRandSkill, sphereIndex, sphereType, (uint)skillIndex - 1);
            }
        }
        protected override void OnShow()
        {
            UpdatePanel();
        }

        private void OnRefreshDemonSpiritSkill()
        {
            ani.Play("Fx", -1, 0f);
            UpdatePanel();
        }

        private void UpdatePanel()
        {
            var items = RefreshCost;
            var count = items.Count;
            FrameworkTool.CreateChildList(costItemParent, count);
            for (int i = 0; i < count; i++)
            {
                if(null != items[i] && items[i].Count >= 2)
                {
                    var itemId = items[i][0];
                    var itemCount = items[i][1];
                    var trans = costItemParent.GetChild(i);
                    PropItem item = new PropItem();
                    item.BindGameObject(trans.gameObject);
                    PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(itemId, itemCount, true, false, false, false, false, true, true, true);
                    item.SetData(itemN, EUIID.UI_Pet_DemonSkill_Refresh);
                    item.Layout.imgIcon.enabled = true;
                }
            }
            PetSoulBeadInfo petSoulBeadInfo = Sys_Pet.Instance.GetPetDemonSpiritSphereInfo(sphereType, sphereIndex);
            var tempSkillId = petSoulBeadInfo.TempSkillIds[(int)skillIndex - 1];
            SetSkillInfo(skill1Go, petSoulBeadInfo.SkillIds[(int)skillIndex - 1]);
            SetSkillInfo(skill2Go, petSoulBeadInfo.TempSkillIds[(int)skillIndex - 1]);
            saveBtn.gameObject.SetActive(tempSkillId != 0);
            cancelBtn.gameObject.SetActive(tempSkillId != 0);
        }

        private void SetSkillInfo(GameObject skillGo, uint skillId)
        {
            bool hasSkill = skillId > 0;
            skillGo.SetActive(hasSkill);
            if (hasSkill)
            {
                if (Sys_Skill.Instance.IsActiveSkill(skillId)) //主动技能
                {
                    CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                    if (skillInfo != null)
                    {
                        skillGo.transform.Find("Text_Name").GetComponent<Text>().text = LanguageHelper.GetTextContent(skillInfo.name);
                        TextHelper.SetText(skillGo.transform.Find("Text_Description").GetComponent<Text>(), Sys_Skill.Instance.GetSkillDesc(skillId));
                        TextHelper.SetText(skillGo.transform.Find("Skillbg/Image_Levelbg/Level").GetComponent<Text>(), 680003033u, skillInfo.level.ToString());
                        ImageHelper.GetPetSkillQuality_Frame(skillGo.transform.Find("Skillbg/Image_Quality").GetComponent<Image>(), (int)skillInfo.quality);
                        ImageHelper.SetIcon(skillGo.transform.Find("Skillbg/Icon").GetComponent<Image>(), skillInfo.icon);
                    }
                    else
                    {
                        Debug.LogErrorFormat("not found skillId={0} in  CSVActiveSkillInfoData", skillId);
                    }
                }
                else
                {
                    CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                    if (skillInfo != null)
                    {
                        skillGo.transform.Find("Text_Name").GetComponent<Text>().text = LanguageHelper.GetTextContent(skillInfo.name);
                        TextHelper.SetText(skillGo.transform.Find("Text_Description").GetComponent<Text>(), LanguageHelper.GetTextContent(skillInfo.desc));
                        TextHelper.SetText(skillGo.transform.Find("Skillbg/Image_Levelbg/Level").GetComponent<Text>(), 680003033u, skillInfo.level.ToString());
                        ImageHelper.GetPetSkillQuality_Frame(skillGo.transform.Find("Skillbg/Image_Quality").GetComponent<Image>(), (int)skillInfo.quality);
                        ImageHelper.SetIcon(skillGo.transform.Find("Skillbg/Icon").GetComponent<Image>(), skillInfo.icon);
                    }
                    else
                    {
                        Debug.LogErrorFormat("not found skillId={0} in CSVPassiveSkillInfoData", skillId);
                    }
                }
            }
            else
            {

            }
        }


        private void OnClickClose()
        {
            UIManager.CloseUI(EUIID.UI_Pet_DemonSkill_Refresh);
        }


        private void OnClickRefresh()
        {
            var items = RefreshCost;
            var count = items.Count;
            for (int i = 0; i < count; i++)
            {
                if (null != items[i] && items[i].Count >= 2)
                {
                    var itemId = items[i][0];
                    var itemCount = items[i][1];
                    ItemIdCount itemIdCount = new ItemIdCount(itemId, itemCount);
                    if (!itemIdCount.Enough)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002019));
                        return;
                    }
                }
            }
            PetSoulBeadInfo petSoulBeadInfo = Sys_Pet.Instance.GetPetDemonSpiritSphereInfo(sphereType, sphereIndex);

            if (petSoulBeadInfo.TempSkillIds[(int)skillIndex - 1] != 0 && !Sys_Pet.Instance.isDemonSpiritSkillRefresh)
            {
                UIManager.OpenUI(EUIID.UI_Pet_RemakeTips, false, 6u);
            }
            else
            {
                Sys_Pet.Instance.PetSoulBeadOperateReq((uint)PetSoulBeadOpType.SoulBeadOpTypeRandSkill, sphereIndex,sphereType, (uint)skillIndex - 1);
            }
        }

        private void OnClickCancel()
        {
            PetSoulBeadInfo petSoulBeadInfo = Sys_Pet.Instance.GetPetDemonSpiritSphereInfo(sphereType, sphereIndex);
            var skillId = petSoulBeadInfo.TempSkillIds[(int)skillIndex - 1];
            string nameStr = string.Empty;
            if (Sys_Skill.Instance.IsActiveSkill(skillId)) //主动技能
            {
                CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                if (skillInfo != null)
                {
                    nameStr = LanguageHelper.GetTextContent(skillInfo.name);
                }
                else
                {
                    Debug.LogErrorFormat("not found skillId={0} in  CSVActiveSkillInfoData", skillId);
                }
            }
            else
            {
                CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                if (skillInfo != null)
                {
                    nameStr = LanguageHelper.GetTextContent(skillInfo.name);
                }
                else
                {
                    Debug.LogErrorFormat("not found skillId={0} in CSVPassiveSkillInfoData", skillId);
                }
            }

            PromptBoxParameter.Instance.OpenPromptBox(LanguageHelper.GetTextContent(680002023, nameStr),
                              0, () =>
                              {
                                  Sys_Pet.Instance.PetSoulBeadOperateReq((uint)PetSoulBeadOpType.SoulBeadOpTypeDeleteSkill, sphereIndex, sphereType, (uint)skillIndex - 1);

                              });
        }

        private void OnClickSave()
        {
            PetSoulBeadInfo petSoulBeadInfo = Sys_Pet.Instance.GetPetDemonSpiritSphereInfo(sphereType, sphereIndex);
            var skillId = petSoulBeadInfo.SkillIds[(int)skillIndex - 1];
            string saveNameStr = string.Empty;
            string delectNameStr = string.Empty;
            if (Sys_Skill.Instance.IsActiveSkill(skillId)) //主动技能
            {
                CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                if (skillInfo != null)
                {
                    delectNameStr = LanguageHelper.GetTextContent(skillInfo.name);
                }
                else
                {
                    Debug.LogErrorFormat("not found skillId={0} in  CSVActiveSkillInfoData", skillId);
                }
            }
            else
            {
                CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                if (skillInfo != null)
                {
                    delectNameStr = LanguageHelper.GetTextContent(skillInfo.name);
                }
                else
                {
                    Debug.LogErrorFormat("not found skillId={0} in CSVPassiveSkillInfoData", skillId);
                }
            }
            skillId = petSoulBeadInfo.TempSkillIds[(int)skillIndex - 1];
            if (Sys_Skill.Instance.IsActiveSkill(skillId)) //主动技能
            {
                CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                if (skillInfo != null)
                {
                    saveNameStr = LanguageHelper.GetTextContent(skillInfo.name);
                }
                else
                {
                    Debug.LogErrorFormat("not found skillId={0} in  CSVActiveSkillInfoData", skillId);
                }
            }
            else
            {
                CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                if (skillInfo != null)
                {
                    saveNameStr = LanguageHelper.GetTextContent(skillInfo.name);
                }
                else
                {
                    Debug.LogErrorFormat("not found skillId={0} in CSVPassiveSkillInfoData", skillId);
                }
            }

            PromptBoxParameter.Instance.OpenPromptBox(LanguageHelper.GetTextContent(680002021, saveNameStr, delectNameStr),
                              0, () =>
                              {
                                  Sys_Pet.Instance.PetSoulBeadOperateReq((uint)PetSoulBeadOpType.SoulBeadOpTypeSaveSkill, sphereIndex,sphereType, (uint)skillIndex - 1);

                              });
        }


        private void OnClickPreview()
        {
            UIManager.OpenUI(EUIID.UI_Pet_Demon_SkillPreview, false, new Tuple<uint,uint>(sphereType, (uint)skillIndex));
        }
    }
}


