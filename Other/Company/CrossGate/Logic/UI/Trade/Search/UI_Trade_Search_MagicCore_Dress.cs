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
    public class UI_Trade_Search_MagicCore_Dress
    {
        public class DressCell
        {
            private Transform transform;
            private GameObject gameObject;

            private Button _btn;
            private Text _textName;

            public uint _dressId = 0u;

            public void Init(Transform trans)
            {
                transform = trans;
                gameObject = transform.gameObject;

                _btn = transform.Find("Image_BG").GetComponent<Button>();
                _btn.onClick.AddListener(OnClick);

                _textName = transform.Find("Text_name").GetComponent<Text>();
                //_textSelectName = transform.Find("Select/Text_name").GetComponent<Text>();
            }

            private void OnClick()
            {
                Sys_Trade.Instance.eventEmitter.Trigger(Sys_Trade.EEvents.OnSelectCoreDress, _dressId);
            }

            public void UpdateInfo(uint dressId)
            {
                _dressId = dressId;
                CSVPetEquipSuitAppearance.Data info = CSVPetEquipSuitAppearance.Instance.GetConfData(dressId);
                if (info != null)
                    _textName.text = LanguageHelper.GetTextContent(info.name);
            }
        }

        private Transform transform;
        private GameObject gameObject;

        private InfinityGrid _infinityGrid;
        private Button _btnClose;
        //private Button _btnConfirm;

        private List<uint> listDress = new List<uint>(6);

        public void Init(Transform trans)
        {
            transform = trans;
            gameObject = transform.gameObject;

            _infinityGrid = transform.Find("Scroll View").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;

            _btnClose = transform.Find("Image_Back").GetComponent<Button>();
            _btnClose.onClick.AddListener(() => { Hide(); });
        }

        public void Show()
        {
            gameObject.SetActive(true);

            UpdateInfo();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            DressCell core = new DressCell();
            core.Init(cell.mRootTransform);
            cell.BindUserData(core);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            DressCell core = cell.mUserData as DressCell;
            core.UpdateInfo(listDress[index]);
        }

        private void UpdateInfo()
        {
            listDress.Clear();
            
            CSVPetEquipSuitSkill.Data suitSkillData = CSVPetEquipSuitSkill.Instance.GetConfData(Sys_Trade.Instance.SelectedCoreSuitSkillId);
            if (suitSkillData != null)
            {
                if (suitSkillData.appearance_id != 0)
                {
                    listDress.Add(suitSkillData.appearance_id);
                }
            }
            else
            {
                int count = CSVPetEquipSuitAppearance.Instance.Count;
                for(int i = 0; i < count; ++i)
                    listDress.Add(CSVPetEquipSuitAppearance.Instance.GetByIndex(i).id);
            }
            
            _infinityGrid.CellCount = listDress.Count;
            _infinityGrid.ForceRefreshActiveCell();
        }
    }
}


