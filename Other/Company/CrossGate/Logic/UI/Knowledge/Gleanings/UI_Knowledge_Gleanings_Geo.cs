using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Lib.Core;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Knowledge_Gleanings_Geo
    {
        public class CellGeo
        {
            private Transform transform;

            private Button _btn;
            private Image _imgItem;
            private Text _textName;

            private Transform _transLock;
            private Transform _transRed;

            private System.Action<uint> _action;
            private uint _knowledgeId;
            private bool _isActive;

            public void Init(Transform trans)
            {
                transform = trans;

                _btn = transform.Find("Image_Icon").GetComponent<Button>();
                _btn.onClick.AddListener(OnClick);

                _imgItem = transform.Find("Image_Icon").GetComponent<Image>();
                _textName = transform.Find("Image_label/Label").GetComponent<Text>();
                _transLock = transform.Find("Image_lockitem");
                _transRed = transform.Find("Image_Red");
            }

            public void OnShow()
            {
                transform.gameObject.SetActive(true);
            }

            public void OnHide()
            {
                transform.gameObject.SetActive(false);
            }

            public void OnDestroy()
            {

            }

            private void OnClick()
            {
                if (_isActive)
                {
                    UIManager.OpenUI(EUIID.UI_Knowledge_Gleanings_Info, false, _knowledgeId);

                    Sys_Knowledge.Instance.OnDelNewKnowledge(Sys_Knowledge.ETypes.Gleanings, _knowledgeId);
                    _transRed.gameObject.SetActive(false);
                }
                else
                {
                    CSVGleanings.Data data = CSVGleanings.Instance.GetConfData(_knowledgeId);
                    if (data != null)
                    {
                        KnowledgeUnlockParam param = new KnowledgeUnlockParam();
                        param.knowledgeType = Sys_Knowledge.ETypes.Gleanings;
                        param.sourceId = data.Source;

                        UIManager.OpenUI(EUIID.UI_Knowledge_Unlock, false, param);
                    }
                }
                //_action?.Invoke(_knowledgeId);
            }

            //public void Register(System.Action<uint> action)
            //{
            //    _action = action;
            //}

            public void UpdateInfo(uint knowledgeId)
            {
                _knowledgeId = knowledgeId;
                _isActive = Sys_Knowledge.Instance.IsKnowledgeActive(_knowledgeId);

                CSVGleanings.Data data = CSVGleanings.Instance.GetConfData(_knowledgeId);
                if (data != null)
                {
                    ImageHelper.SetIcon(_imgItem, data.icon_id, false);
                    _textName.text = LanguageHelper.GetTextContent(data.name_id);

                    _transLock.gameObject.SetActive(!_isActive);
                }

                bool isRedPoint = Sys_Knowledge.Instance.IsRedPointByKnowledge(_knowledgeId);
                _transRed.gameObject.SetActive(isRedPoint);
            }
        }

        private Transform transform;

        private InfinityGrid _infinityGrid;
        //private InfinityGridLayoutGroup gridGroup;
        //private Dictionary<GameObject, CellGeo> dicCells = new Dictionary<GameObject, CellGeo>();
        //private int visualGridCount;

        private List<uint> listGeos = new List<uint>();

        public void Init(Transform trans)
        {
            transform = trans;

            _infinityGrid = transform.Find("Scroll View0").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnChangeCell;
        }

        public void OnShow()
        {
            transform.gameObject.SetActive(true);
        }

        public void OnHide()
        {
            transform.gameObject.SetActive(false);
        }

        public void OnDestroy()
        {
            //foreach (var data in dicCells)
            //    data.Value.OnDestroy();
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            CellGeo geo = new CellGeo();
            geo.Init(cell.mRootTransform);
            cell.BindUserData(geo);
        }

        private void OnChangeCell(InfinityGridCell cell, int index)
        {
            CellGeo geo = cell.mUserData as CellGeo;
            geo.UpdateInfo(listGeos[index]);
        }

        public void UpdateInfo(uint subTypeId)
        {
            listGeos = Sys_Knowledge.Instance.GetGleanings(1u, subTypeId);

            _infinityGrid.CellCount = listGeos.Count;
            _infinityGrid.ForceRefreshActiveCell();
        }
    }
}


