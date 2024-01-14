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
    public class UI_Trade_Search_MagicCore_Suit
    {
        public class SuitSkillCell
        {
            private Transform transform;
            private GameObject gameObject;

            private Button _btn;
            private Text _textName;
            private Image _imgIcon;

            public uint _suitSkillId = 0u;

            public void Init(Transform trans)
            {
                transform = trans;
                gameObject = transform.gameObject;

                _btn = transform.Find("Image_BG").GetComponent<Button>();
                _btn.onClick.AddListener(OnClick);

                _textName = transform.Find("Text_name").GetComponent<Text>();
                _imgIcon = transform.Find("PetItem/Image_Icon").GetComponent<Image>();
                //_textSelectName = transform.Find("Select/Text_name").GetComponent<Text>();
            }

            private void OnClick()
            {
                Sys_Trade.Instance.eventEmitter.Trigger(Sys_Trade.EEvents.OnSelectCoreSuit, _suitSkillId);
            }

            public void UpdateInfo(uint suitSkillId)
            {
                _suitSkillId = suitSkillId;
                CSVPetEquipSuitSkill.Data suitSkillData = CSVPetEquipSuitSkill.Instance.GetConfData(suitSkillId);
                if (suitSkillData != null)
                {
                    CSVPassiveSkillInfo.Data info = CSVPassiveSkillInfo.Instance.GetConfData(suitSkillData.base_skill);
                    if (info != null)
                    {
                        _textName.text = LanguageHelper.GetTextContent(info.name);
                        ImageHelper.SetIcon(_imgIcon, info.icon);
                    }
                }
            }
        }

        private Transform transform;
        private GameObject gameObject;

        private InfinityGrid _infinityGrid;
        private Button _btnClose;
        //private Button _btnConfirm;

        private List<uint> listSuitSkills = new List<uint>(6);

        public void Init(Transform trans)
        {
            transform = trans;
            gameObject = transform.gameObject;

            _infinityGrid = transform.Find("Scroll View").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;

            _btnClose = transform.Find("Interrcept_Image").GetComponent<Button>();
            _btnClose.onClick.AddListener(() => { Hide(); });
        }

        public void Show()
        {
            gameObject.SetActive(true);

            CalAttrIds();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            SuitSkillCell core = new SuitSkillCell();
            core.Init(cell.mRootTransform);
            cell.BindUserData(core);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            SuitSkillCell core = cell.mUserData as SuitSkillCell;
            core.UpdateInfo(listSuitSkills[index]);
        }

        private void CalAttrIds()
        {
            listSuitSkills.Clear();
            CSVPetEquip.Data data = CSVPetEquip.Instance.GetConfData(Sys_Trade.Instance.SelectedCoreId);
            if (data != null)
            {
                int count = CSVPetEquipSuitSkill.Instance.Count;
                for (int i = 0; i < count; ++i)
                {
                    CSVPetEquipSuitSkill.Data suitSkillData = CSVPetEquipSuitSkill.Instance.GetByIndex(i);
                    if (suitSkillData.group_id == data.skill)
                    {
                        listSuitSkills.Add(suitSkillData.id);
                    }
                }
            }
            
            _infinityGrid.CellCount = listSuitSkills.Count;
            _infinityGrid.ForceRefreshActiveCell();
        }
    }
}


