using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
namespace Logic
{
    public class UI_Pet_MountConversion_Layout
    {
        private Button closeBtn;
        private Button changeBtn;
        private GameObject noneView;
        private GameObject rightView;
        private Image skillImage;
        private Text skillNameText;
        private Text skillCostText;
        private Text skillDescText;
        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;
        
        public void Init(Transform transform)
        {
            infinityGrid = transform.Find("Animator/Scroll_View_Pets").GetComponent<InfinityGrid>();
            changeBtn = transform.Find("Animator/View_Right/Btn_Conversion").GetComponent<Button>();
            closeBtn = transform.Find("Animator/View_TipsBgNew07/Btn_Close").GetComponent<Button>();
            noneView = transform.Find("Animator/View_Npc022").gameObject;
            rightView = transform.Find("Animator/View_Right").gameObject;
            skillImage = rightView.transform.Find("Image_bg/Image_bg/Image_Icon").GetComponent<Image>();
            skillNameText = rightView.transform.Find("Image_bg/Text_Name").GetComponent<Text>();
            skillCostText = rightView.transform.Find("Image_bg/Text_Consume").GetComponent<Text>();
            skillDescText = rightView.transform.Find("View_SkillTips/Text_Tips").GetComponent<Text>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            changeBtn.onClick.AddListener(listener.OnChangeBtnClicked);
            infinityGrid.onCreateCell += listener.OnCreateCell;
            infinityGrid.onCellChange += listener.OnCellChange;
        }

        public void SetInfinityGridCell(int count)
        {
            infinityGrid.CellCount = count;
            infinityGrid.ForceRefreshActiveCell();
            bool hasPet = count > 0;
            rightView.SetActive(hasPet);
            noneView.SetActive(!hasPet);
        }

        public void SetRightView(uint skillId)
        {
            bool isActiveSkill = Sys_Skill.Instance.IsActiveSkill(skillId);

            //skillCostText.gameObject.SetActive(isActiveSkill);

            if (isActiveSkill) //主动技能
            {
                CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                if (skillInfo != null)
                {
                    ImageHelper.SetIcon(skillImage, skillInfo.icon);

                    skillNameText.text = LanguageHelper.GetTextContent(skillInfo.name);
                    TextHelper.SetText(skillDescText, Sys_Skill.Instance.GetSkillDesc(skillId));
                    CSVActiveSkill.Data cSVActiveSkillData = CSVActiveSkill.Instance.GetConfData(skillId);
                    if (null != cSVActiveSkillData)
                    {
                        TextHelper.SetText(skillCostText, 2011502u, cSVActiveSkillData.energy_cost.ToString());
                    }
                    else
                    {
                        Debug.LogErrorFormat("not found skillId={0} in activeSkill", skillId);
                    }
                }
                else
                {
                    Debug.LogErrorFormat("not found skillId={0} in skillInfo", skillId);
                }
            }
            else
            {

                CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                if (skillInfo != null)
                {
                    ImageHelper.SetIcon(skillImage, skillInfo.icon);
                    skillNameText.text = LanguageHelper.GetTextContent(skillInfo.name);
                    TextHelper.SetText(skillDescText, skillInfo.desc);
                    TextHelper.SetText(skillCostText, 2011502u, skillInfo.cost_energy.ToString());
                }
                else
                {
                    Debug.LogErrorFormat("not found skillId={0}", skillId);
                }
            }
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void OnChangeBtnClicked();
            void OnCreateCell(InfinityGridCell cell);
            void OnCellChange(InfinityGridCell cell, int index);
        }
    }

    public class UI_Pet_MountConversion : UIBase, UI_Pet_MountConversion_Layout.IListener
    {
        private UI_Pet_MountConversion_Layout layout = new UI_Pet_MountConversion_Layout();
        private List<PetMountCell> cells = new List<PetMountCell>();
        private int selectIndex;
        private List<ClientPet> mountPets = new List<ClientPet>();
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnNumberChangePet, AbandonPet, toRegister);
        }

        protected override void OnOpen(object arg = null)
        {
        }

        protected override void OnShow()
        {
            mountPets = Sys_Pet.Instance.GetMountConversionList();
            layout.SetInfinityGridCell(mountPets.Count);
            
            if(mountPets.Count == 0)
            {
                selectIndex = -1;
            }
            else
            {
                SelectState();
                SetSkillInfo();
            }
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            selectIndex = -1;
        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            PetMountCell entry = new PetMountCell();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            entry.AddListen(OnCellClicked);
            cell.BindUserData(entry);
            cells.Add(entry);
        }

        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= mountPets.Count)
                return;
            PetMountCell entry = cell.mUserData as PetMountCell;
            entry.ReSetData(mountPets[index], 0, index);
            entry.SetSelect(selectIndex == index);
        }

        private void OnCellClicked(int index)
        {
            if (selectIndex != index)
            {
                selectIndex = index;
                SelectState();
                SetSkillInfo();
            }
        }

        private void SelectState()
        {
            for (int i = 0; i < cells.Count; i++)
            {
                cells[i].SetSelect(cells[i].index == selectIndex);
            }
        }

        private void SetSkillInfo()
        {
            if (selectIndex >= 0)
            {
                var petId = mountPets[selectIndex].petData.id;
                CSVPetMount.Data mountData = CSVPetMount.Instance.GetConfData(petId);
                if (null != mountData)
                {
                    if (null != mountData.mount_skills && mountData.mount_skills.Count >= 0 && null != mountData.mount_skills[0])
                    {
                        layout.SetRightView(mountData.mount_skills[0][0]);
                    }
                }
            }
        }

        private void AbandonPet()
        {
            mountPets = Sys_Pet.Instance.GetMountConversionList();
            if (mountPets.Count == 0)
            {
                selectIndex = -1;
            }
            else
            {
                selectIndex = 0;
            }
            layout.SetInfinityGridCell(mountPets.Count);
            SelectState();
            SetSkillInfo();
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_MountConversion);
        }

        public void OnChangeBtnClicked()
        {
            if (selectIndex >= 0)
            {
                var clientPet = mountPets[selectIndex];
                CSVPetMount.Data mountData = CSVPetMount.Instance.GetConfData(clientPet.petData.id);
                if (null != mountData)
                {
                    if (null != mountData.mount_skills && mountData.mount_skills.Count >= 0 && null != mountData.mount_skills[0])
                    {
                        layout.SetRightView(mountData.mount_skills[0][0]);
                    }
                }
                if (Sys_Pet.Instance.IsUniquePet(clientPet.petData.id))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12469));
                    return;
                }
                else if (Sys_Pet.Instance.IsPetBeEffectWithSecureLock(clientPet.petUnit))
                {
                    return;
                }
                else if (clientPet.petUnit.SimpleInfo.ExpiredTick > 0)//限时
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000212));
                    return;
                }
                else if (Sys_Pet.Instance.IsLastPetEntExpiredTick(clientPet.petUnit.SimpleInfo.ExpiredTick > 0))//最后一个
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000213));
                    return;
                }
                else if (Sys_Pet.Instance.fightPet.IsSamePet(clientPet.GetPetUid()))//出战
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000214));
                    return;
                }
                else if (clientPet.GetPetUid() == Sys_Pet.Instance.mountPetUid)//骑乘
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000215));
                    return;
                }
                else if(!clientPet.GetPetIsDomestication())
                {
                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680000216);
                    PromptBoxParameter.Instance.SetConfirm(true, () =>
                    {
                        CSVPetNewParam.Data cSVPetParameterData = CSVPetNewParam.Instance.GetConfData(37u);
                        if (null != cSVPetParameterData)
                        {
                            ActionCtrl.Instance.MoveToTargetNPCAndInteractive(cSVPetParameterData.value);
                            UIManager.CloseUI(EUIID.UI_Pet_MountConversion);
                        }
                    });
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                    return;
                }
                else if(clientPet.HasSubPet())//契约了其他宠物-自己是主宠
                {
                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680000217);
                    PromptBoxParameter.Instance.SetConfirm(true, () =>
                    {
                        var indexs = new List<uint>();
                        var uids = clientPet.GetSubsPetUid();
                        for (uint i = 0; i < uids.Count; i++)
                        {
                            if(uids[(int)i] != 0)
                            {
                                indexs.Add(i);
                            }
                        }
                        Sys_Pet.Instance.OnPetContractCancleReq(clientPet.GetPetUid(), indexs);
                    });
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                    return;
                }
                else if (clientPet.HasPartnerPet())//是否是其他宠物的契约宠物-自己是副宠
                {
                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680000218);
                    PromptBoxParameter.Instance.SetConfirm(true, () =>
                    {
                        ClientPet partnerPet = Sys_Pet.Instance.GetPetByUId(clientPet.petUnit.SimpleInfo.ContractPetUid);
                        if(null != partnerPet)
                        {
                            int index = partnerPet.GetIndexByPetUid(clientPet.GetPetUid());
                            if(index >= 0)
                            {
                                Sys_Pet.Instance.OnPetContractCancleReq(partnerPet.GetPetUid(), new List<uint>() { (uint)index});
                            }
                        }
                    });
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                    return;
                }
                else if(clientPet.HasManyMountSkill())//多技能
                {
                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680000219);
                    PromptBoxParameter.Instance.SetConfirm(true, () =>
                    {
                        Sys_Pet.Instance.OnPetMountExchangeReq(clientPet.GetPetUid());
                    });
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                    return;
                }
                else if (clientPet.HasEquipDemonSpiritSphere())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002042));
                    return;
                }

                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680000220);
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    Sys_Pet.Instance.OnPetMountExchangeReq(clientPet.GetPetUid());
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
        }
    }
}