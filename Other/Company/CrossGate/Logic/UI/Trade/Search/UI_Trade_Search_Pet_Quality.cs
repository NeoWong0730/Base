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
    public class UI_Trade_Search_Pet_Quality
    {
        public class QualityCell
        {
            private Transform transform;
            private GameObject gameObject;

            private Button _btn;
            private Text _textName;

            public uint _qualityId = 0u;

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
                Sys_Trade.Instance.eventEmitter.Trigger(Sys_Trade.EEvents.OnSelectPetQuality, _qualityId);
            }

            public void UpdateInfo(uint qualityId)
            {
                _qualityId = qualityId;
                _textName.text = LanguageHelper.GetTextContent(_qualityId);
            }
        }

        private Transform transform;
        private GameObject gameObject;

        private InfinityGridLayoutGroup gridGroup;
        private int visualGridCount;
        private Dictionary<GameObject, QualityCell> dicCells = new Dictionary<GameObject, QualityCell>();

        private Button _btnClose;
        //private Button _btnConfirm;

        private List<uint> _qualityIds = new List<uint>(6);

        public void Init(Transform trans)
        {
            transform = trans;
            gameObject = transform.gameObject;

            gridGroup = transform.Find("Tips_Rect/Rectlist").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            gridGroup.minAmount = 10;
            gridGroup.updateChildrenCallback = UpdateChildrenCallback;

            for (int i = 0; i < gridGroup.transform.childCount; ++i)
            {
                Transform tran = gridGroup.transform.GetChild(i);

                QualityCell cell = new QualityCell();
                cell.Init(tran);
                dicCells.Add(tran.gameObject, cell);
            }

            _btnClose = transform.Find("Interrcept_Image").GetComponent<Button>();
            _btnClose.onClick.AddListener(() => { Hide(); });
        }

        public void Show()
        {
            gameObject.SetActive(true);

            CalQualityIds();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= visualGridCount)
                return;

            if (dicCells.ContainsKey(trans.gameObject))
            {
                QualityCell cell = dicCells[trans.gameObject];
                cell.UpdateInfo(_qualityIds[index]);
            }
        }

        private void CalQualityIds()
        {
            _qualityIds.Clear();

            _qualityIds = Sys_Trade.Instance.LeftPetQualitys();

            visualGridCount = _qualityIds.Count;
            gridGroup.SetAmount(visualGridCount);
        }
    }
}


