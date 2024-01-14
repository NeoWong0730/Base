using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
using System.Text;

namespace Logic
{
    public class UI_Pet_LevelDown_Layout
    {
        private Button closeBtn;
        private Button cancelBtn;
        private Button sureBtn;
        private Text nameText;
        private Text levelText;
        private Text basePointText;
        private Text enhancePointText;
        private Text maxSkillLeveText;
        private Text advancePointText;
        private Text advanceSkillGridText;
        private Text delectSkillText;
        private GameObject noAdvanceTipsGo;
        private GameObject AdvanceItemGo;
        public void Init(Transform transform)
        {
            closeBtn = transform.Find("Animator/View_TipsBgNew03/Btn_Close").GetComponent<Button>();
            sureBtn = transform.Find("Animator/Btn_01").GetComponent<Button>();
            cancelBtn = transform.Find("Animator/Btn_02").GetComponent<Button>();
            nameText = transform.Find("Animator/Text_Title1").GetComponent<Text>();
            levelText = transform.Find("Animator/Text_Tips1").GetComponent<Text>();
            basePointText = transform.Find("Animator/Text_Tips2").GetComponent<Text>();
            enhancePointText = transform.Find("Animator/Text_Tips3").GetComponent<Text>();
            maxSkillLeveText = transform.Find("Animator/Text_Tips4").GetComponent<Text>();
            advancePointText = transform.Find("Animator/Text_Tips5").GetComponent<Text>();
            advanceSkillGridText = transform.Find("Animator/Text_Tips6").GetComponent<Text>();
            delectSkillText = transform.Find("Animator/Text_Tips7").GetComponent<Text>();
            noAdvanceTipsGo = transform.Find("Animator/Type2").gameObject;
            AdvanceItemGo = transform.Find("Animator/Type1/Grid").gameObject;
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            sureBtn.onClick.AddListener(listener.SureBtnClicked);
            cancelBtn.onClick.AddListener(listener.CancelBtnClicked);
        }

        public void SetPetName(string name)
        {
            TextHelper.SetText(nameText, 8005, name);
        }

        public void SetlevelInfo(uint currentLevel, uint tagLevel, uint styleId)
        {
            TextHelper.SetText(levelText, LanguageHelper.GetTextContent(8006, currentLevel.ToString(), tagLevel.ToString()), CSVWordStyle.Instance.GetConfData(styleId));
        }

        public void SetBasePointInfo(uint current, uint tag, uint styleId)
        {
            TextHelper.SetText(basePointText, LanguageHelper.GetTextContent(8007, current.ToString(), tag.ToString()), CSVWordStyle.Instance.GetConfData(styleId));
        }

        public void SetEnhancePointInfo(uint current, uint tag, uint styleId)
        {
            TextHelper.SetText(enhancePointText, LanguageHelper.GetTextContent(8009, current.ToString(), tag.ToString()), CSVWordStyle.Instance.GetConfData(styleId));
        }

        public void SetMaxSkillLevelInfo(uint current, uint styleId)
        {
            TextHelper.SetText(maxSkillLeveText, LanguageHelper.GetTextContent(8011, current.ToString(), current.ToString(), current.ToString()), CSVWordStyle.Instance.GetConfData(styleId));
        }

        public void SetInfo(uint currentPetUid)
        {
            ClientPet pet = Sys_Pet.Instance.GetPetByUId(currentPetUid);
            if (null != pet)
            {
                SetPetName(pet.GetPetNmae());

                //bool isBack = false;
                //此处tag 为等级
                var tag = pet.petUnit.SimpleInfo.Level;
                uint styleId = 0;
                ///等级回退显示
                {
                    var levelLimit = Sys_Pet.Instance.LevelBackLimit;
                    for (int i = levelLimit.Length - 1; i >= 0; i--)
                    {
                        if (tag > levelLimit[i])
                        {
                            tag = levelLimit[i];
                            //isBack = true;
                            styleId = 3;
                            break;
                        }
                    }
                    SetlevelInfo(pet.petUnit.SimpleInfo.Level, tag, styleId);
                }
                styleId = 0;
                //进阶回退显示
                {
                    int currentAdvanceNum = (int)pet.GetAdvancedNum();
                    var advancelevels = Sys_Pet.Instance.AdvancedLevel;
 
                    //可以保留的进阶次数
                    int saveAdvanceNum = 0;
                    for (int i = 0; i < advancelevels.Count; i++)
                    {
                        if(advancelevels[i] <= tag)
                        {
                            saveAdvanceNum++;
                        }
                    }
                    //扣除得进阶次数
                    saveAdvanceNum = currentAdvanceNum - saveAdvanceNum;
                    noAdvanceTipsGo.gameObject.SetActive(currentAdvanceNum < 1);
                    if (saveAdvanceNum <= 0)
                    {
                        SetAdvancePointInfo(LanguageHelper.GetTextContent(5122), styleId);
                        SetAdvanceGridInfo(LanguageHelper.GetTextContent(5122), styleId);
                        AdvanceItemGo.transform.parent.gameObject.SetActive(false);
                        delectSkillText.gameObject.SetActive(false);
                    }
                    else
                    {
                        if (currentAdvanceNum != 0)
                        {
                            styleId = 3;
                            var delectSkillGridNum = pet.petData.forward_adv_num * saveAdvanceNum;
                            SetAdvanceGridInfo(delectSkillGridNum.ToString(), styleId);
                            var skillGrid = pet.GetPetSkillGridsCount();
                            var petSkills = pet.GetNorolSkills();
                            if(petSkills.Count > skillGrid - delectSkillGridNum)
                            {
                                var willDelectNum = petSkills.Count - (skillGrid - delectSkillGridNum);
                                SetDelectSkillInfo(petSkills.GetRange((int)(skillGrid - delectSkillGridNum), (int)willDelectNum));
                                delectSkillText.gameObject.SetActive(true);
                            }
                            else
                            {
                                delectSkillText.gameObject.SetActive(false);
                            }
                            uint delectPoint = 0;
                            saveAdvanceNum = currentAdvanceNum - saveAdvanceNum;
                            if (null != pet.petData.required_skills_count)
                            {
                                for (int i = currentAdvanceNum - 1; i >= saveAdvanceNum; i--)
                                {
                                    if(pet.petData.required_skills_count.Count > i && i >= 0)
                                    {
                                        delectPoint += pet.petData.required_skills_count[i];
                                    }
                                }
                            }

                            if (null != pet.petData.lv_back_money)
                            {
                                List<List<uint>> items = new List<List<uint>>(saveAdvanceNum);
                                for (int i = currentAdvanceNum - 1; i >= saveAdvanceNum; i--)
                                {
                                    if (pet.petData.lv_back_money.Count > i && i >= 0)
                                    {
                                        items.Add(pet.petData.lv_back_money[i]);
                                    }
                                }

                                FrameworkTool.CreateChildList(AdvanceItemGo.transform, items.Count);

                                for (int i = 0; i < items.Count; i++)
                                {
                                    Transform trans = AdvanceItemGo.transform.GetChild(i);
                                    PropItem cell = new PropItem();
                                    cell.BindGameObject(trans.gameObject);
                                    PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(items[i][0], items[i][1], true, false, false, false, false, true, false, true);
                                    cell.SetData(itemData, EUIID.UI_Pet_LevelDown);
                                }
                            }


                            noAdvanceTipsGo.gameObject.SetActive(false);
                            AdvanceItemGo.transform.parent.gameObject.SetActive(true);
                            SetAdvancePointInfo(delectPoint.ToString(), styleId);
                        }
                        else
                        {
                            SetAdvancePointInfo(LanguageHelper.GetTextContent(5122), styleId);
                            SetAdvanceGridInfo(LanguageHelper.GetTextContent(5122), styleId);
                            delectSkillText.gameObject.SetActive(false);
                            AdvanceItemGo.transform.parent.gameObject.SetActive(false);
                        }
                    }
                }
                styleId = 3;
                //普通加点显示
                {
                    var levelPointParam = CSVPetNewParam.Instance.GetConfData(8u);
                    if(null != levelPointParam)
                    {
                        SetBasePointInfo(levelPointParam.value * (pet.petUnit.SimpleInfo.Level - 1), levelPointParam.value * (tag - 1), styleId);
                    }
                }
                styleId = 0;
                //技能最高等级显示
                {
                    var count = CSVPetNewSkillsLv.Instance.Count;
                    uint lv = 1;
                    for (int i = 0; i < count; i++)
                    {
                        var skillLvInfo = CSVPetNewSkillsLv.Instance.GetByIndex(i);
                        if(null != skillLvInfo)
                        {
                            if(skillLvInfo.pet_level <= tag)
                            {
                                lv = skillLvInfo.id;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    var petSkills = pet.GetNorolSkills();
                    for (int i = 0; i < petSkills.Count; i++)
                    {
                        var skillId = petSkills[i];
                        bool isActiveSkill = Sys_Skill.Instance.IsActiveSkill(skillId);
                        if (isActiveSkill) //主动技能
                        {
                            CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                            if (skillInfo != null)
                            {
                                if (skillInfo.level > lv)
                                {
                                    styleId = 3;
                                    break;
                                }
                            }
                            else
                            {
                                DebugUtil.LogFormat(ELogType.eNone, "not found skillId={0} in skillInfo", skillId);
                            }
                        }
                        else
                        {
                            CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                            if (skillInfo != null)
                            {
                                if (skillInfo.level > lv)
                                {
                                    styleId = 3;
                                    break;
                                }
                            }
                            else
                            {
                                DebugUtil.LogFormat(ELogType.eNone, "not found skillId={0}", skillId);
                            }

                        }
                    }
                    SetMaxSkillLevelInfo(lv, styleId);
                }

                //强化等级显示
                // 显示上只需要判断已有强化等级是否需要回退,确定时才需要判断是否有加点需要多使用货币
                {
                    var enhanceLvl = pet.petUnit.EnhancePlansData.TotalPoint;
                    //强化等级对应的宠物等级上限表格配置
                    var csvEnhanceInfo = CSVPetNewEnhance.Instance.GetConfData(enhanceLvl);
                    if(null != csvEnhanceInfo)
                    {
                        var petLevelLimit = csvEnhanceInfo.pet_lv;
                        //当前强化等级对应宠物等级大于即将回退等级 需要搜寻强化表需要回退的等级id, 目前回退只是一阶段回退
                        if (petLevelLimit > tag)
                        {
                            var enhanceLevelParam = CSVPetNewParam.Instance.GetConfData(78u);
                            if (null != enhanceLevelParam)
                            {
                                var enhanceLevelList = ReadHelper.ReadArray_ReadUInt(enhanceLevelParam.str_value, '|');
                                if(null != enhanceLevelList)
                                {
                                    for (int i = enhanceLevelList.Count - 1; i >= 0; i--)
                                    {
                                        var id = enhanceLevelList[i];
                                        var tempEnhanceInfo = CSVPetNewEnhance.Instance.GetConfData(id);
                                        if (null != tempEnhanceInfo)
                                        {
                                            if (tempEnhanceInfo.pet_lv < tag)
                                            {
                                                tag = id;
                                                styleId = 3;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            styleId = 0;
                            tag = enhanceLvl;
                        }
                    }
                    else
                    {
                        styleId = 0;
                        tag = enhanceLvl;
                    }

                    SetEnhancePointInfo(enhanceLvl, tag, styleId);
                }

                
            }
        }

        public void SetAdvancePointInfo(string current, uint styleId)
        {
            TextHelper.SetText(advancePointText, LanguageHelper.GetTextContent(8013, current), CSVWordStyle.Instance.GetConfData(styleId));
        }

        public void SetAdvanceGridInfo(string current, uint styleId)
        {
            TextHelper.SetText(advanceSkillGridText, LanguageHelper.GetTextContent(8014, current), CSVWordStyle.Instance.GetConfData(styleId));
        }

        public void SetDelectSkillInfo(List<uint> skillIds)
        {
            bool willSkillBeDelect = skillIds.Count > 0;
            if (willSkillBeDelect)
            {
                StringBuilder stringBuilder = StringBuilderPool.GetTemporary();
                for (int i = 0; i < skillIds.Count; i++)
                {
                    var skillId = skillIds[i];
                    bool isActiveSkill = Sys_Skill.Instance.IsActiveSkill(skillId);
                    if (isActiveSkill) //主动技能
                    {
                        CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                        if (skillInfo != null)
                        {
                            if (stringBuilder.Length > 0)
                            {
                                stringBuilder.Append("，");
                            }
                            stringBuilder.Append(LanguageHelper.GetTextContent(skillInfo.name));
                        }
                        else
                        {
                            DebugUtil.LogFormat(ELogType.eNone, "not found skillId={0} in skillInfo", skillId);
                        }
                    }
                    else
                    {
                        CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                        if (skillInfo != null)
                        {
                            if (stringBuilder.Length > 0)
                            {
                                stringBuilder.Append("，");
                            }
                            stringBuilder.Append(LanguageHelper.GetTextContent(skillInfo.name));
                        }
                        else
                        {
                            DebugUtil.LogFormat(ELogType.eNone, "not found skillId={0}", skillId);
                        }
                    }
                }
                TextHelper.SetText(delectSkillText, LanguageHelper.GetTextContent(8015, StringBuilderPool.ReleaseTemporaryAndToString(stringBuilder)));
            }
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void CancelBtnClicked();
            void SureBtnClicked();
        }
    }

    public class UI_Pet_LevelDown : UIBase, UI_Pet_LevelDown_Layout.IListener
    {
        private UI_Pet_LevelDown_Layout layout = new UI_Pet_LevelDown_Layout();
        private uint currentPetUid;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
        }

        protected override void OnOpen(object arg = null)
        {
            currentPetUid = Convert.ToUInt32(arg);
        }

        protected override void OnShow()
        {
            layout.SetInfo(currentPetUid);
        }

        protected override void OnHide()
        {
        }

        protected override void OnDestroy()
        {

        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_LevelDown);
        }

        public void CancelBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_LevelDown);
        }

        public void SureBtnClicked()
        {
            ClientPet pet = Sys_Pet.Instance.GetPetByUId(currentPetUid);
            if (null != pet)
            {
                if (Sys_SecureLock.Instance.lockState)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(15189));
                    return;
                    //Sys_SecureLock.Instance.JumpToSecureLock();
                }
                var costParam = CSVPetNewParam.Instance.GetConfData(77u);
                if (null != costParam)
                {
                    List<List<uint>> costList = ReadHelper.ReadArray2_ReadUInt(costParam.str_value, '|', '&');

                    ItemIdCount itemIdCount = new ItemIdCount();
                    if (costList.Count > 0)
                    {
                        itemIdCount.id = costList[0][0];
                        itemIdCount.count = costList[0][1];
                    }
                    bool isEnough = itemIdCount.Enough;
                    if (!isEnough)
                    {
                        Sys_Bag.Instance.TryOpenExchangeCoinUI((ECurrencyType)itemIdCount.id, itemIdCount.count);
                        return;
                    }

                    if(pet.IsHasEquip())
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(15190));
                        return;
                    }


                    PromptBoxParameter.Instance.OpenPromptBox(LanguageHelper.GetTextContent(15191, itemIdCount.count.ToString(), pet.GetPetNmae()), 0, () =>
                     {
                         PromptBoxParameter.Instance.OpenPromptBox(LanguageHelper.GetTextContent(15192), 0, () =>
                         {
                             Sys_Pet.Instance.OnPetLevelDownReq(pet.GetPetUid());
                             UIManager.CloseUI(EUIID.UI_Pet_LevelDown);
                         });
                         PromptBoxParameter.Instance.needDingTo = true;
                         Sys_Hint.Instance.eventEmitter.Trigger(Sys_Hint.EEvents.RefreshPromptBoxData);
                     });
                }
            }
        }
    }
}