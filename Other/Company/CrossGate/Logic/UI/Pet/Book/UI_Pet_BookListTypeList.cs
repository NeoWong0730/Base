using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Table;
using Lib.Core;

namespace Logic
{

    public enum EPetBookType
    {
        Area = 0,
        Race = 1,
        Genus = 2,
        Mount = 3,
    }

    public class UI_Pet_BookListTypeList
    {
        private Transform transform;
        private CP_ToggleRegistry toggleGroup;
        private CP_Toggle allToggle;
        private InfinityGridLayoutGroup gridGroup;
        private Dictionary<GameObject, UI_Pet_TypeToggle> dicCells = new Dictionary<GameObject, UI_Pet_TypeToggle>();
        private List<UI_Pet_TypeToggle> cellLists = new List<UI_Pet_TypeToggle>();
        private int visualGridCount;
        private EPetBookType type;
        private IListener listener;
        private int curSelectIndex;
        private bool isSealSetting;
        

        private List<uint> listIds = new List<uint>();
        public void Init(Transform _transform, bool _isSealSetting=false)
        {
            transform = _transform;
            toggleGroup = transform.Find("Scroll01/Content").gameObject.GetComponent<CP_ToggleRegistry>();
            gridGroup = transform.Find("Scroll01/Content").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            gridGroup.minAmount = 11;
            gridGroup.updateChildrenCallback = UpdateChildrenCallback;
            allToggle = transform.Find("Toggle_All").GetComponent<CP_Toggle>();
            allToggle.onValueChanged.AddListener(AllToggleValueChange);
            isSealSetting = _isSealSetting;
            for (int i = 0; i < gridGroup.transform.childCount; ++i)
            {
                Transform tran = gridGroup.transform.GetChild(i);

                UI_Pet_TypeToggle cell = new UI_Pet_TypeToggle(); 
                cell.Init(tran);
                cell.AddListener(OnSelectIndex);
                dicCells.Add(tran.gameObject, cell);
                cellLists.Add(cell);
            }
            curSelectIndex = -1;
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= visualGridCount)
                return;

            if (dicCells.ContainsKey(trans.gameObject))
            {
                UI_Pet_TypeToggle cell = dicCells[trans.gameObject];
                cell.SetInfo(listIds[index], type, index, isSealSetting);
                cell.OnSelect(curSelectIndex == index);
            }
        }

        public void RefreshCellData()
        {
            for (int i = 0; i < cellLists.Count; i++)
            {
                cellLists[i].RefreshRedState();
            }
        }

        private void AllToggleValueChange(bool isOn)
        {
            if(isOn)
            {
                curSelectIndex = -1;
                listener?.OnSelectListIndex(curSelectIndex);
            }            
        }

        private void OnSelectIndex(UI_Pet_TypeToggle cell)
        {
            curSelectIndex = cell.index;
            listener?.OnSelectListIndex((int)cell.currentTypeId);
        }

        public void Hide()
        {
            List<UI_Pet_TypeToggle> dataList = new List<UI_Pet_TypeToggle>(dicCells.Values);
            for (int i = 0; i < dataList.Count; i++)
            {
                dataList[i].ToggleOff();
            }
        }


        public void SetListByType(EPetBookType _type)
        {
            curSelectIndex = -1;
            
            type = _type;
            listIds.Clear();
            if(_type == EPetBookType.Genus)
            {
                //listIds = new List<uint>(CSVGenus.Instance.Count);
                //for (int i = 0; i < CSVGenus.Instance.Count; i++)
                //{
                //    listIds.Add(CSVGenus.Instance[i].id);
                //}
                listIds.AddRange(CSVGenus.Instance.GetKeys());
                listIds.Sort();
            }
            else if(_type == EPetBookType.Area)
            {
                listIds = Sys_Pet.Instance.GetHauntedAreaList();
            }
            else if(_type == EPetBookType.Race)
            {
                //listIds = new List<uint>(CSVPetNewMapRarity.Instance.Count);
                //for (int i = 0; i < CSVPetNewMapRarity.Instance.Count; i++)
                //{
                //    listIds.Add(CSVPetNewMapRarity.Instance[i].id);
                //}
                listIds.AddRange(CSVPetNewMapRarity.Instance.GetKeys());
                listIds.Sort((a, b) => (int)b - (int)a);
            }
            else if (_type == EPetBookType.Mount)
            {
                //listIds = new List<uint>(CSVPetNewIsMount.Instance.Count);
                //for (int i = 0; i < CSVPetNewIsMount.Instance.Count; i++)
                //{
                //    listIds.Add(CSVPetNewIsMount.Instance[i].id);
                //}
                listIds.AddRange(CSVPetNewIsMount.Instance.GetKeys());
                listIds.Sort((a, b) => (int)b - (int)a);
            }


            visualGridCount = listIds.Count;
            gridGroup.SetAmount(visualGridCount);
            allToggle.SetSelected(true, true);
        }

        public void RegisterListener(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void OnSelectListIndex(int _typeId);
        }
    }

}

