using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
namespace Logic
{
    public class UI_Pet_Demon_Layout
    {
        private Button closeBtn;
        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;
        private Button previewBtn;
        private Button activeBtn;
        public Image demonSpiritSkillImage;
        public Image demonSpiritSkillQualityImage;
        public Text demonSpiritSkillNameText;
        public Text demonSpiritSkillLevelText;
        public Text demonSpiritSkillDescText;
        public Text needTipsText;
        /// <summary> 激活魔魂界面 </summary>
        private GameObject activeGo;
        /// <summary> 魔魂激活后展示界面 </summary>
        private GameObject detailGo;
        public PropItem activePropItem;
        public List<Transform> transforms = new List<Transform>(4);
        public Animator activeAni;
        public void Init(Transform transform)
        {
            infinityGrid = transform.Find("Animator/View_Content/Detail/ScrollView").GetComponent<InfinityGrid>();
            closeBtn = transform.Find("Animator/View_TipsBgNew07/Btn_Close").GetComponent<Button>();
            previewBtn = transform.Find("Animator/View_Content/Image/Button_Preview").GetComponent<Button>();
            activeBtn = transform.Find("Animator/View_Content/Activate/Btn_01").GetComponent<Button>();
            demonSpiritSkillImage = transform.Find("Animator/View_Content/Image/PetSkillItem01/Image_Skill").GetComponent<Image>();
            demonSpiritSkillQualityImage = transform.Find("Animator/View_Content/Image/PetSkillItem01/Image_Quality").GetComponent<Image>();
            demonSpiritSkillNameText = transform.Find("Animator/View_Content/Image/Image_Name/Text_Name").GetComponent<Text>();
            demonSpiritSkillLevelText = transform.Find("Animator/View_Content/Image/Image_Name/Text_Name/Text").GetComponent<Text>();
            demonSpiritSkillDescText = transform.Find("Animator/View_Content/Image/Image_Name/TextTotal").GetComponent<Text>();
            needTipsText = transform.Find("Animator/View_Content/Activate/Text_01").GetComponent<Text>();

            activeGo = transform.Find("Animator/View_Content/Activate").gameObject;
            detailGo = transform.Find("Animator/View_Content/Detail").gameObject;
            activePropItem = new PropItem();
            activePropItem.BindGameObject(transform.Find("Animator/View_Content/Activate/PropItem").gameObject);
            transforms.Add(transform.Find("Animator/View_Content/Detail/Des/DI"));
            transforms.Add(transform.Find("Animator/View_Content/Detail/Des/Tian"));
            transforms.Add(transform.Find("Animator/View_Content/Detail/Des/Long"));
            transforms.Add(transform.Find("Animator/View_Content/Detail/Des/Shen"));
            activeAni = transform.Find("Animator/View_Content").GetComponent<Animator>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            previewBtn.onClick.AddListener(listener.OnPreViewBtnClicked);
            activeBtn.onClick.AddListener(listener.OnActiveBtnClicked);
            infinityGrid.onCreateCell += listener.OnCreateCell;
            infinityGrid.onCellChange += listener.OnCellChange;
        }

        public void SetInfinityGridCell(int count)
        {
            infinityGrid.CellCount = count;
            infinityGrid.ForceRefreshActiveCell();
        }

        public void SetDetailView(bool active)
        {
            detailGo.SetActive(active);
            activeGo.SetActive(!active);
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void OnCreateCell(InfinityGridCell cell);
            void OnCellChange(InfinityGridCell cell, int index);
            void OnPreViewBtnClicked();
            void OnActiveBtnClicked();
        }
    }

    /// <summary>
    /// 专属魔魂界面
    /// </summary>
    public class UI_Pet_Demon : UIBase, UI_Pet_Demon_Layout.IListener
    {
        private UI_Pet_Demon_Layout layout = new UI_Pet_Demon_Layout();
        List<uint> levels;
        private ClientPet currentPet;
        //当选择的宠物激活需要另一个宠物时
        private uint selectPetUid;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);

            PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(0, 0, false, false, false, false, false, false, false, true, ItemGridBeClicked, _bUseTips: false);
            layout.activePropItem.SetData(itemN, EUIID.UI_Pet_Demon);
            layout.activePropItem.btnNone.gameObject.SetActive(true);
            layout.activePropItem.Layout.imgQuality.gameObject.SetActive(false);

            levels = Sys_Pet.Instance.EquipActiveLevelLimit;
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnOwnDemonSpiritPetSelect, OnSelectPet, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnActiveDemonSpirit, OnActiveDemonSpirit, toRegister);
        }

        protected override void OnOpen(object arg = null)
        {
            var currentPetUid = Convert.ToUInt32(arg);
            currentPet = Sys_Pet.Instance.GetPetByUId(currentPetUid);
        }

        protected override void OnShow()
        {
            RefreshView();
        }

        protected override void OnHide()
        {
            timer?.Cancel();
        }

        Timer timer;
        private void OnActiveDemonSpirit()
        {
            AnimationClip an = layout.activeAni.runtimeAnimatorController.animationClips[0];
            float _timer = an.averageDuration;
            layout.activeAni.Play("Fx", -1, 0f);
            timer?.Cancel();
            
            timer = Timer.Register(_timer, () =>
            {
                //Sys_Pet.Instance.eventEmitter.Trigger<uint>(Sys_Pet.EEvents.OnActiveOwnDemonSpiritOrRemakeAniUiAnimatorEnd, 1u);
                Sys_Pet.Instance.eventEmitter.Trigger<uint>(Sys_Pet.EEvents.OnActiveOwnDemonSpiritOrRemakeAniUiAnimatorEnd, 1u);
                UIManager.CloseUI(EUIID.UI_Pet_Demon);
            });
        }

        private void RefreshView()
        {
            if(null != currentPet)
            {
                var skillId = currentPet.petData.soul_skill_id;
                bool isActive = currentPet.GetDemonSpiritIsActive();
                if(isActive)
                {
                    skillId = currentPet.petUnit.PetSoulUnit.SkillId;
                }
                if (Sys_Skill.Instance.IsActiveSkill(skillId)) //主动技能
                {
                    CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                    if (skillInfo != null)
                    {
                        ImageHelper.SetIcon(layout.demonSpiritSkillImage, skillInfo.icon);
                        ImageHelper.GetPetSkillQuality_Frame(layout.demonSpiritSkillQualityImage, (int)skillInfo.quality);
                        layout.demonSpiritSkillNameText.text = LanguageHelper.GetTextContent(skillInfo.name);
                        TextHelper.SetText(layout.demonSpiritSkillDescText, Sys_Skill.Instance.GetSkillDesc(skillId));
                        TextHelper.SetText(layout.demonSpiritSkillLevelText, 680003020, skillInfo.level.ToString());
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
                        ImageHelper.SetIcon(layout.demonSpiritSkillImage, skillInfo.icon);
                        ImageHelper.GetPetSkillQuality_Frame(layout.demonSpiritSkillQualityImage, (int)skillInfo.quality);
                        layout.demonSpiritSkillNameText.text = LanguageHelper.GetTextContent(skillInfo.name);
                        layout.demonSpiritSkillDescText.text = LanguageHelper.GetTextContent(skillInfo.desc);
                        TextHelper.SetText(layout.demonSpiritSkillLevelText, 680003020, skillInfo.level.ToString());
                    }
                    else
                    {
                        Debug.LogErrorFormat("not found skillId={0} in CSVPassiveSkillInfoData", skillId);
                    }
                }
                
                
                if (isActive)
                {
                    SetDetailItem();
                }
                else
                {
                    SetActiveItem();
                }
                layout.SetDetailView(isActive);
            }
        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            GameObject go = cell.mRootTransform.gameObject;
            
            cell.BindUserData(go);
        }

        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= levels.Count)
                return;
            GameObject entry = cell.mUserData as GameObject;
            TextHelper.SetText(entry.transform.Find("Text_Name").GetComponent<Text>(), LanguageHelper.GetTextContent(680003000u + (uint)index, levels[index].ToString()), CSVWordStyle.Instance.GetConfData(currentPet.GetEquipSphereTotalLevel() >= levels[index] ? 74u : 152u)); 
        }

        private void OnSelectPet(uint petUid)
        {
            selectPetUid = petUid;
            var cp = Sys_Pet.Instance.GetPetByUId(selectPetUid);
            if (null != cp)
            {
                //宠物id 同道具id
                PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(cp.petData.id, 0, true, false, false, false, false, false, false, true, ItemGridBeClicked, _bUseTips: false);
                layout.activePropItem.SetData(itemN, EUIID.UI_Pet_Demon);
                layout.activePropItem.btnNone.gameObject.SetActive(false);
                layout.activePropItem.Layout.imgIcon.enabled = true;
            }
        }

        private void SetActiveItem()
        {
            if(null != currentPet.petData.soul_activate_cost && currentPet.petData.soul_activate_cost.Count >= 3)
            {
                var costType = currentPet.petData.soul_activate_cost[0];
                layout.needTipsText.gameObject.SetActive(costType == 1);
                if (costType == 1)//消耗宠物
                {
                    TextHelper.SetText(layout.needTipsText, currentPet.petData.soul_activate_cost[1] == 0 ?680002030u : 680002029u, currentPet.petData.soul_activate_cost[2].ToString());
                    if (selectPetUid == 0)
                    {
                        PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(0, 0, false, false, false, false, false, false, false, true, ItemGridBeClicked, _bUseTips: false);
                        layout.activePropItem.SetData(itemN, EUIID.UI_Pet_Demon);
                        layout.activePropItem.txtNumber.gameObject.SetActive(false);
                        layout.activePropItem.btnNone.gameObject.SetActive(true);
                        layout.activePropItem.Layout.imgIcon.enabled = false;
                        layout.activePropItem.Layout.imgQuality.gameObject.SetActive(false);
                    }
                    else
                    {
                        var cp = Sys_Pet.Instance.GetPetByUId(selectPetUid);
                        if(null != cp)
                        {
                            //宠物id 同道具id
                            PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(cp.petData.id, 0, true, false, false, false, false, false, false, true, ItemGridBeClicked, _bUseTips: false);
                            layout.activePropItem.SetData(itemN, EUIID.UI_Pet_Demon);
                            layout.activePropItem.btnNone.gameObject.SetActive(false);
                            layout.activePropItem.Layout.imgIcon.enabled = true;
                        }
                    }
                }
                else if(costType == 2)// 消耗道具
                {
                    var itemId = currentPet.petData.soul_activate_cost[1];
                    var itemNum = currentPet.petData.soul_activate_cost[2];
                    PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(itemId, itemNum, true, false, false, false, false, true, true, true);
                    layout.activePropItem.SetData(itemN, EUIID.UI_Pet_Demon);
                    layout.activePropItem.Layout.imgIcon.enabled = true;
                }

                layout.needTipsText.gameObject.SetActive(costType == 1);
            }
        }

        private void SetDetailItem()
        {
            for (int i = 0; i < layout.transforms.Count; i++)
            {
                var level = currentPet.GetEquipSphereLevelByType((uint)i + 1);
                var trans = layout.transforms[i];
                trans.Find("Lock").gameObject.SetActive(level == 0);
                var text = trans.Find("Text").GetComponent<Text>();
                if (level > 0)
                {
                    TextHelper.SetText(text, level.ToString());
                    text.gameObject.SetActive(true);
                }
                else
                {
                    text.gameObject.SetActive(false);
                }
                
                
            }
            layout.SetInfinityGridCell(levels.Count);
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_Demon);
        }

        public void OnPreViewBtnClicked()
        {
            if (null != currentPet)
            {
                UIManager.OpenUI(EUIID.UI_Pet_DemonPreview, false, currentPet.petData.soul_skill_id);
            }
        }

        public void OnActiveBtnClicked()
        {
            if (null != currentPet)
            {
                var costType = currentPet.petData.soul_activate_cost[0];
                if (costType == 1)//消耗宠物
                {
                    if (selectPetUid == 0)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002000u));
                    }
                    else
                    {
                        var cp = Sys_Pet.Instance.GetPetByUId(selectPetUid);
                        if (null != cp)
                        {
                            if (cp.petUnit != null && cp.petUnit.Islocked)
                            {
                                PromptBoxParameter.Instance.Clear();
                                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(15248, cp.GetPetNmae(), cp.GetPetNmae());
                                PromptBoxParameter.Instance.SetConfirm(true, () =>
                                {
                    
                                    Sys_Pet.Instance.OnPetLockReq(cp.GetPetUid(), false);
                                });
                                PromptBoxParameter.Instance.SetCancel(true, null);
                                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                                return;
                            }
                            
                            PromptBoxParameter.Instance.Clear();
                            PromptBoxParameter.Instance.tipType = PromptBoxParameter.TipType.Text;
                            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680002057, cp.GetPetNmae(), cp.petUnit.SimpleInfo.Score.ToString());
                            PromptBoxParameter.Instance.SetConfirm(true, () =>
                            {
                                if (null != cp)
                                {
                                    Sys_Pet.Instance.PetSoulActiveReq(currentPet.GetPetUid(), selectPetUid);
                                }
                            });
                            PromptBoxParameter.Instance.SetCancel(true, null);
                            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                        }
                    }
                }
                else if (costType == 2)// 消耗道具
                {
                    var itemId = currentPet.petData.soul_activate_cost[1];
                    var itemNum = currentPet.petData.soul_activate_cost[2];
                    var itemCount = new ItemIdCount(itemId, itemNum);
                    if(!itemCount.Enough)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002025u));
                    }
                    else
                    {
                        Sys_Pet.Instance.PetSoulActiveReq(currentPet.GetPetUid(), 0);
                    }
                }
            }
        }

        private void ItemGridBeClicked(PropItem bulidItem)
        {
            if (null != currentPet)
            {
                UIManager.OpenUI(EUIID.UI_Pet_DemonPet, false,
                    new UI_Pet_DemonParam
                    {
                        type = 0,
                        tuple = currentPet.GetPetUid(),
                    });
            }
        }
    }
}