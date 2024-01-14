using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Packet;
using Table;
using Logic.Core;

namespace Logic
{
    public class UI_Equipment_Inlay_JewelList : UIParseCommon
    {
        private GameObject cellParent;
        private GameObject cellTemplate;
        private List<UI_Equipment_Inlay_JewelList_Cell> cellList = new List<UI_Equipment_Inlay_JewelList_Cell>();

        protected override void Parse()
        {
            cellParent = transform.Find("TargetScroll/Tablist").gameObject;
            cellTemplate = transform.Find("TargetScroll/Tablist/TargetItem").gameObject;
            cellTemplate.SetActive(false);
        }

        public override void Show()
        {

        }

        public override void Hide()
        {

        }

        public override void OnDestroy()
        {

        }

        public void UpdateJewelList(ItemData equipItem, uint jewelInfoId)
        {
            uint jewelType = 0;
            if (jewelInfoId != 0)
            {
                CSVJewel.Data jewelData = CSVJewel.Instance.GetConfData(jewelInfoId);
                jewelType = jewelData.jewel_type;
            }

            SortJewel(equipItem.Id, jewelType);
        }

        private void SortJewel(uint equipId, uint jewelType)
        {
            Lib.Core.FrameworkTool.DestroyChildren(cellParent, cellTemplate.name);
            cellList.Clear();

            CSVEquipment.Data baseData = CSVEquipment.Instance.GetConfData(equipId);

            List<uint> types = new List<uint>();
            if (jewelType != 0)
            {
                types.Add(jewelType);
                if (baseData.jewel_type != null)
                {
                    for (int i = 0; i < baseData.jewel_type.Count; ++i)
                    {
                        if (baseData.jewel_type[i] == jewelType)
                            continue;

                        types.Add(baseData.jewel_type[i]);
                    }
                }
            }
            else
            {
                types = baseData.jewel_type;
            }

            if (types == null) //basedata.jewel_type is may null
                return;

            int index = -1;
            for (int i = 0; i < types.Count; ++i)
            {
                GameObject cellGo = GameObject.Instantiate<GameObject>(cellTemplate, cellParent.transform);
                cellGo.SetActive(true);

                UI_Equipment_Inlay_JewelList_Cell cell = new UI_Equipment_Inlay_JewelList_Cell();
                cell.Init(cellGo.transform);
                cell.OnListJewel(types[i]);
                if (index == -1 && cell.IsRedDot)
                    index = i;

                cellList.Add(cell);
            }

            //默认展开第一个类型
            if (cellList.Count > 0)
            {
                index = index < 0 ? 0 : index;
                cellList[index].OnExpand(true);
            }
        }
    }
}


