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
    public class UI_Trade_Search_MagicCore_Effect
    {
        public class EffectCell
        {
            private Transform transform;
            private GameObject gameObject;

            private Button _btn;
            private Text _textName;

            public uint _effectId = 0u;

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
                Sys_Trade.Instance.eventEmitter.Trigger(Sys_Trade.EEvents.OnSelectCoreEffect, _effectId);
            }

            public void UpdateInfo(uint effectId)
            {
                _effectId = effectId;
                CSVPetEquipEffect.Data data = CSVPetEquipEffect.Instance.GetConfData(effectId);
                if (data != null)
                {
                    CSVPassiveSkillInfo.Data info = CSVPassiveSkillInfo.Instance.GetConfData(data.effect);
                    if (info != null)
                        _textName.text = LanguageHelper.GetTextContent(info.name);
                }
                    
            }
        }

        private Transform transform;
        private GameObject gameObject;

        private InfinityGrid _infinityGrid;
        private Button _btnClose;
        //private Button _btnConfirm;

        private List<uint> listEffects = new List<uint>(6);

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

            UpdateInfo();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            EffectCell core = new EffectCell();
            core.Init(cell.mRootTransform);
            cell.BindUserData(core);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            EffectCell core = cell.mUserData as EffectCell;
            core.UpdateInfo(listEffects[index]);
        }

        private void UpdateInfo()
        {
            listEffects.Clear();

            int count = CSVPetEquipEffect.Instance.Count;
            for (int i = 0; i < count; ++i)
            {
                CSVPetEquipEffect.Data data = CSVPetEquipEffect.Instance.GetByIndex(i);
                if (data != null && data.group_id == 100) //固定
                    listEffects.Add(data.id);
            }

            _infinityGrid.CellCount = listEffects.Count;
            _infinityGrid.ForceRefreshActiveCell();
        }
    }
}


