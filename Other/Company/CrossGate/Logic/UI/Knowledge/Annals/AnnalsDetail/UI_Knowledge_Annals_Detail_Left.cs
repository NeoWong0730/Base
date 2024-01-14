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
    public class UI_Knowledge_Annals_Detail_Left
    {
        private class CellYear
        {
            private Transform transform;

            private CP_Toggle _toggle;

            private Text _textYear;
            private Text _textSelectYear;
            private Transform _transRed;

            private System.Action<uint> _action;
            private uint _yearId;

            private bool _isActive;

            public void Init(Transform trans)
            {
                transform = trans;

                _toggle = transform.GetComponent<CP_Toggle>();
                _toggle.onValueChanged.AddListener(OnClickToggle);

                _textYear = transform.Find("Text").GetComponent<Text>();
                _textSelectYear = transform.Find("Text_Select").GetComponent<Text>();
                _transRed = transform.Find("Image_Red");

                //Sys_Knowledge.Instance.eventEmitter.Handle(Sys_Knowledge.EEvents.OnAnnalsRedPointUpdate, UpdateRedPoint, false);
                Sys_Knowledge.Instance.eventEmitter.Handle(Sys_Knowledge.EEvents.OnAnnalsRedPointUpdate, UpdateRedPoint, true);
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
                Sys_Knowledge.Instance.eventEmitter.Handle(Sys_Knowledge.EEvents.OnAnnalsRedPointUpdate, UpdateRedPoint, false);
            }

            private void OnClickToggle(bool isOn)
            {
                if (isOn)
                    _action?.Invoke(_yearId);
            }

            public void Reigster(System.Action<uint> action)
            {
                _action = action;
            }

            public void UpdateInfo(uint yearId)
            {
                _yearId = yearId;

                _textYear.text = _textSelectYear.text = LanguageHelper.GetTextContent(_yearId);

                _toggle.SetSelected(_yearId == Sys_Knowledge.Instance.SelectYearId, true);

                _isActive = Sys_Knowledge.Instance.IsAnnalActive(_yearId);

                ImageHelper.SetImageGray(transform, !_isActive, true);
                _toggle.enabled = _isActive;

                UpdateRedPoint();
            }

            private void UpdateRedPoint()
            {
                bool isRed = false;
                List<uint> events = Sys_Knowledge.Instance.GetAnnalEvents(_yearId);
                if (events != null)
                {
                    foreach (var data in events)
                    {
                        if (Sys_Knowledge.Instance.IsRedPointByKnowledge(data))
                        {
                            isRed = true;
                            break;
                        }
                    }
                }

                _transRed.gameObject.SetActive(isRed);
            }
        }

        private Transform transform;

        private InfinityGrid _infinityGrid;
        private Lib.Core.CoroutineHandler handler;
        private Dictionary<GameObject, CellYear> dicCells = new Dictionary<GameObject, CellYear>();

        private IListener _listener;

        private List<uint> _listYears = new List<uint>();

        public void Init(Transform trans)
        {
            transform = trans;

            _infinityGrid = transform.GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
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
            foreach (var data in dicCells)
                data.Value.OnDestroy();
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            CellYear entry = new CellYear();
            entry.Reigster(OnSelectYear);

            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);

            dicCells.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            CellYear entry = cell.mUserData as CellYear;
            entry.UpdateInfo(_listYears[index]);
        }

        //private void UpdateChildrenCallback(int index, Transform trans)
        //{
        //    if (index < 0 || index >= visualGridCount)
        //        return;

        //    if (dicCells.ContainsKey(trans.gameObject))
        //    {
        //        CellYear cell = dicCells[trans.gameObject];
        //        cell.UpdateInfo(_listYears[index]);
        //    }
        //}

        private void OnSelectYear(uint yearId)
        {
            _listener?.OnSelectYear(yearId);
        }

        public void Register(IListener listener)
        {
            _listener = listener;
        }

        public void UpdateInfo()
        {
            _listYears.Clear();
            _listYears.AddRange(Sys_Knowledge.Instance.GetTotalAnnals().Keys);

            _infinityGrid.CellCount = _listYears.Count;
            _infinityGrid.ForceRefreshActiveCell();

            int index = 0;
            if (Sys_Knowledge.Instance.SelectYearId != 0)
                index = _listYears.IndexOf(Sys_Knowledge.Instance.SelectYearId);

            index = index < 0 ? 0 : index;
            //_infinityGrid.MoveToIndex(index);
            //Debug.LogError(index.ToString());

            if (handler != null)
            {
                CoroutineManager.Instance.Stop(handler);
                handler = null;
            }

            handler = CoroutineManager.Instance.StartHandler(CheckNeedScroll(index));
        }

        private IEnumerator CheckNeedScroll(int index)
        {
            yield return new WaitForSeconds(0.2f);

            _infinityGrid.MoveToIndex(index);
            //int index = shopIds.IndexOf(shopId);
            //_infinityGrid.MoveToIndex(index);

            //foreach (var data in dicCells)
            //{
            //    if (data.Value.mShopId == selectShopId)
            //    {
            //        data.Value.OnSelect(true);
            //    }
            //}
        }

        public interface IListener
        {
            void OnSelectYear(uint yearId);
        }
    }
}


