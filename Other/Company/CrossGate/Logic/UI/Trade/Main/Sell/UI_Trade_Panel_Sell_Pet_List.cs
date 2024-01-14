//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using Logic.Core;
//using Table;
//using Packet;
//using Lib.Core;

//namespace Logic
//{
//    public class UI_Trade_Panel_Sell_Pet_List
//    {
//        private class SellPetCell
//        {
//            private Transform transform;

//            private PetItem01 _petItem;

//            private PetUnit _PetUnit;
//            public void Init(Transform trans)
//            {
//                transform = trans;

//                _petItem = new PetItem01();
//                _petItem.Bind(transform.Find("PetItem01"));
//                _petItem.BtnBg.onClick.AddListener(OnClickPet);
//            }

//            private void OnClickPet()
//            {
//                Sys_Trade.Instance.OnSalePetCheck(_PetUnit);
//            }

//            public void UpdateInfo(PetUnit petUnit)
//            {
//                _PetUnit = petUnit;
//                _petItem.TextLevel.text = petUnit.Level.ToString();

//                CSVPet.Data petData = CSVPet.Instance.GetConfData(petUnit.PetId);
//                if (petData != null)
//                {
//                    _petItem.SetData(petData);
//                }
//            }
//        }

//        private Transform transform;

//        private InfinityGridLayoutGroup gridGroup;
//        private int visualGridCount;
//        private Dictionary<GameObject, SellPetCell> dicCells = new Dictionary<GameObject, SellPetCell>();

//        private List<PetUnit> _petList = new List<PetUnit>();

//        public bool IsShow = false;

//        public void Init(Transform trans)
//        {
//            transform = trans;

//            gridGroup = transform.Find("TabList").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
//            gridGroup.minAmount = 24;
//            gridGroup.updateChildrenCallback = UpdateChildrenCallback;

//            for (int i = 0; i < gridGroup.transform.childCount; ++i)
//            {
//                Transform tran = gridGroup.transform.GetChild(i);

//                SellPetCell cell = new SellPetCell();
//                cell.Init(tran);
//                dicCells.Add(tran.gameObject, cell);
//            }
//        }

//        public void Show()
//        {
//            transform.gameObject.SetActive(true);
//            IsShow = true;
//        }

//        public void Hide()
//        {
//            transform.gameObject.SetActive(false);
//            IsShow = false;
//        }

//        private void UpdateChildrenCallback(int index, Transform trans)
//        {
//            if (index < 0 || index >= visualGridCount)
//                return;

//            if (dicCells.ContainsKey(trans.gameObject))
//            {
//                SellPetCell cell = dicCells[trans.gameObject];
//                cell.UpdateInfo(_petList[index]);
//            }
//        }

//        public void UpdateList()
//        {
//            _petList = Sys_Trade.Instance.GetSellPetList();
//            visualGridCount = _petList.Count;
//            gridGroup.SetAmount(visualGridCount);
//        }
//    }
//}


