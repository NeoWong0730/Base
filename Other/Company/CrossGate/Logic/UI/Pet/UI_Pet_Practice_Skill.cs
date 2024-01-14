using Lib.Core;
using Logic.Core;
using Packet;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Logic
{

    public class UI_Pet_Paractice_Skill
    {
        private ClientPet clientPet;
        private Transform transform;
        private Text skillNumText;
        private Button studyBtn;
        private GameObject skillSelectFxGo;
        public GameObject petSkillGo;
        private Dictionary<GameObject, PetSkillCeil> skillCeilGrids = new Dictionary<GameObject, PetSkillCeil>();
        private List<PetSkillCeil> skillCeilList = new List<PetSkillCeil>();
        private List<uint> skillIdList = new List<uint>();
        private int infinityCount;
        private uint itemId;

        private Timer timer;

        private InfinityGrid _infinityGrid;

        private PetSkillCeil guidBtn;//隐藏 给引导点击

        private GameObject effectGo;
        private GameObject hasItemTips;
        private GameObject noItemTips;

        private bool hasItem
        {
            get
            {
                return ItemId != 0;
            }
        }

        public uint ItemId
        {
            get => itemId;
            set
            {
                itemId = value;
                hasItemTips.SetActive(hasItem);
                noItemTips.SetActive(!hasItem);
                effectGo.SetActive(hasItem);
            }
        }

        public void Init(Transform transform)
        {
            this.transform = transform;
            skillSelectFxGo = transform.Find("Scroll View/Viewport/Fx_ui_Select03").gameObject;
            skillNumText = transform.Find("Image/Text_Number/Text (1)").GetComponent<Text>();

            _infinityGrid = transform.Find("Scroll View").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
            studyBtn = transform.Find("Btn_Study").GetComponent<Button>();
            studyBtn.onClick.AddListener(OnPetLearnSkillBtnClicked);
            effectGo = studyBtn.transform.Find("Image_RedTips").gameObject;
            hasItemTips = transform.Find("Text_Tips_2").gameObject;
            noItemTips = transform.Find("Text_Tips").gameObject;
            guidBtn = new PetSkillCeil();
            guidBtn.BingGameObject(transform.Find("Scroll View/Viewport/Content/Image_Bottom_Guid").gameObject);
            guidBtn.AddClickListener(GuidBtnEvent);
        }

        private void GuidBtnEvent(PetSkillCeil petSkillCeil)
        {
            OpenSelectView();
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            PetSkillCeil entry = new PetSkillCeil();
            GameObject go = cell.mRootTransform.gameObject;
            entry.BingGameObject(go);
            entry.AddClickListener(OnSkillSelect);
            skillCeilGrids.Add(go, entry);
            skillCeilList.Add(entry);
            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= infinityCount)
                return;
            PetSkillCeil entry = cell.mUserData as PetSkillCeil;
            if (index < skillIdList.Count)
            {
                uint skillId = skillIdList[index];
                entry.SetData(skillId, clientPet.IsUniqueSkill(skillId), clientPet.IsBuildSkill(skillId));
            }
            else
            {
                if (index == skillIdList.Count)
                {
                    entry.SetData(0, false, false, ItemId);
                }
                else
                {
                    entry.SetData(0, false, false);
                }

            }
        }


        private void OnPetLearnSkillBtnClicked()
        {
            if (!hasItem)
            {
                if(null != clientPet && clientPet.GetPetSkillGridsCount() == clientPet.GetPetSkillCount())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12273));
                    return;
                }
                OpenSelectView();
            }
            else
            {
                Sys_Pet.Instance.OnPetLearnSkillReq(clientPet.GetPetUid(), ItemId);
            }
        }

        public void Hide()
        {
            ItemId = 0;
            skillSelectFxGo.SetActive(false);
            transform.gameObject.SetActive(false);
        }

        public void Show()
        {
            ItemId = 0;
            skillSelectFxGo.SetActive(false);
            transform.gameObject.SetActive(true);
        }

        public void SetView(ClientPet clientPet)
        {
            ItemId = 0;
            this.clientPet = clientPet;
            skillIdList.Clear();
            infinityCount = 0;
            if (clientPet != null)
            {
                skillIdList = clientPet.GetPetSkillList();
                infinityCount = clientPet.GetPetSkillGridsCount();
                _infinityGrid.CellCount = infinityCount;
                _infinityGrid.ForceRefreshActiveCell();
                skillNumText.text = string.Format("{0}/{1}", clientPet.GetPetSkillCount().ToString(), clientPet.GetPetSkillGridsCount().ToString());
            }
            else
            {
                _infinityGrid.CellCount = infinityCount;
                _infinityGrid.ForceRefreshActiveCell();
                skillNumText.text = "0/0";
            }
        }

        public void SetLearnSkillEffect()
        {
            if(null != skillIdList && null != skillCeilList)
            {
                int index = skillIdList.Count - 1;
                if (index >= 0 && index < skillCeilList.Count - 1)
                {
                    skillSelectFxGo.transform.SetParent(skillCeilList[index].skillItem.skillImage.transform, false);
                    skillSelectFxGo.SetActive(true);
                    timer?.Cancel();
                    timer = Timer.Register(0.5f, PetSkillEffect);
                }
            }
        }

        private void PetSkillEffect()
        {
            skillSelectFxGo.SetActive(false);
        }


        public void OnSelectItem(uint itemId)
        {
            this.ItemId = itemId;
            _infinityGrid.ForceRefreshActiveCell();
        }

        private void OnSkillSelect(PetSkillCeil petSkillCeil)
        {
            if (clientPet.petUnit.SimpleInfo.ExpiredTick > 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000655));
                return;
            }
            uint skillId = petSkillCeil.petSkillBase.skillId;
            if (skillId != 0)
            {
                UIManager.OpenUI(EUIID.UI_Pet_SkillStudy, false, new UI_Pet_SkillStudyParam { petUid = clientPet.GetPetUid(), skillId = skillId });
            }
            else
            {
                uint currentItemId = petSkillCeil.itemId;
                if (currentItemId == 0)
                {
                    OpenSelectView();
                }
                else
                {
                    ItemId = 0;
                    petSkillCeil.SetItemInfo(ItemId);
                    _infinityGrid.CellCount = infinityCount;
                    _infinityGrid.ForceRefreshActiveCell();
                }
            }                
        }

        private void OpenSelectView()
        {
            UIManager.OpenUI(EUIID.UI_SelectItem, false,
                new UI_SelectItemParam
                {
                    tittle_langId = 10944,
                    getAwayId = (uint)EItemType.PetSkillBook,
                    petUid = clientPet.GetPetUid()
                },EUIID.UI_Pet_Message);
        }
    }
}
