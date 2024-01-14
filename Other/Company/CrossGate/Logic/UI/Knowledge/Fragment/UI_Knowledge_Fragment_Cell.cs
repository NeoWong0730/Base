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
    public class UI_Knowledge_Fragment_Cell
    {
        private class CellFragment
        {
            private Transform transform;

            private Button _btn;
            private Text _textName;
            private Image _imgIcon;
            private Transform _transRed;

            private uint _fragId;
            private bool _isActive;

            public void Init(Transform trans)
            {
                transform = trans;

                _btn = transform.Find("Image_UP").GetComponent<Button>();
                _btn.onClick.AddListener(OnClick);

                _textName = transform.Find("Image_Di/Text").GetComponent<Text>();
                _imgIcon = transform.Find("Image_UP/Mask/Image").GetComponent<Image>();
                _transRed = transform.Find("Image_Red");

                Sys_Knowledge.Instance.eventEmitter.Handle(Sys_Knowledge.EEvents.OnFragmentRedPointUpdate, UpdateRedPointState, true);
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
                Sys_Knowledge.Instance.eventEmitter.Handle(Sys_Knowledge.EEvents.OnFragmentRedPointUpdate, UpdateRedPointState, false);
            }

            private void OnClick()
            {
                if (_isActive)
                {
                    UIManager.OpenUI(EUIID.UI_Knowledge_Fragment_Detail, false, _fragId);
                }
                else
                {
                    List<uint> storyIds = Sys_Knowledge.Instance.GetFragStorys(_fragId);
                    if(storyIds != null && storyIds.Count > 0)
                    {
                        CSVMemoryPieces.Data data = CSVMemoryPieces.Instance.GetConfData(storyIds[0]);
                        if (data != null)
                        {
                            KnowledgeUnlockParam param = new KnowledgeUnlockParam();
                            param.knowledgeType = Sys_Knowledge.ETypes.Fragment;
                            param.sourceId = data.Source;

                            UIManager.OpenUI(EUIID.UI_Knowledge_Unlock, false, param);
                        }
                    }
                }
            }

            public void UpdateInfo(uint fragId)
            {
                _fragId = fragId;

                _isActive = Sys_Knowledge.Instance.IsFragActive(_fragId);

                _textName.text = LanguageHelper.GetTextContent(_fragId);
                ImageHelper.SetIcon(_imgIcon, Sys_Knowledge.Instance.GetFragIcon(_fragId));

                ImageHelper.SetImageGray(transform, !_isActive, true);

                UpdateRedPointState();
            }

            private void UpdateRedPointState()
            {
                bool isRedPoint = Sys_Knowledge.Instance.IsFragRedPoint(_fragId);
                _transRed.gameObject.SetActive(isRedPoint);
            }
        }

        private Transform transform;

        private List<CellFragment> listCells = new List<CellFragment>(5);

        public void Init(Transform trans)
        {
            transform = trans;

            for (int i = 0; i < 5; ++i)
            {
                Transform temp = transform.Find(string.Format("Fragment{0}", i));

                CellFragment cell = new CellFragment();
                cell.Init(temp);
                listCells.Add(cell);
            }
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
            for (int i = 0; i < listCells.Count; ++i)
                listCells[i].OnDestroy();
        }

        public void UpdateInfo(List<uint> listFragments)
        {
            for (int i = 0; i < listCells.Count; ++i)
            {
                if (i < listFragments.Count)
                {
                    listCells[i].OnShow();
                    listCells[i].UpdateInfo(listFragments[i]);
                }
                else
                {
                    listCells[i].OnHide();
                }
            }
        }
    }
}


