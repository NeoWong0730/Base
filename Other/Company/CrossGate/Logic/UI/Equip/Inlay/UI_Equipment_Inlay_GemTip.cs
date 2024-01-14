using System;
using System.Collections.Generic;
using Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;

namespace Logic
{
    public class UI_Equipment_Inlay_GemTip : UIBase
    {
        public class AttrCell
        {
            private Transform transform;
            private Text txtName;
            private Text txtValue;
            public void Init(Transform trans)
            {
                transform = trans;
                txtName = transform.Find("Text").GetComponent<Text>();
                txtValue = transform.Find("Text/Text1").GetComponent<Text>();
            }

            public void UpdateInfo(Int2 param)
            {
                uint attrId = (uint)param.X;
                uint attrValue = (uint) param.Y;
                CSVAttr.Data data = CSVAttr.Instance.GetConfData(attrId);
                txtName.text = LanguageHelper.GetTextContent(data.name);
                txtValue.text = Sys_Attr.Instance.GetAttrValue(data, attrValue);
            }
        }
    
        private Text txtTitle;
        private Text txtInfo;
        private InfinityGrid _infinityGrid;
        
        private List<Int2> listDatas = new List<Int2>();
        protected override void OnLoaded()
        {
            Button btnClose = transform.Find("Animator/Button_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(() =>
            {
                this.CloseSelf();
            });
            
            txtTitle = transform.Find("Animator/View_Content/Image_Title/Text_Title").GetComponent<Text>();
            txtInfo = transform.Find("Animator/View_Content/Text").GetComponent<Text>();
            
            _infinityGrid = transform.Find("Animator/View_Content/Scroll View").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
        }

        protected override void OnOpen(object arg)
        {            

        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            

        }

        protected override void OnShow()
        {
            UpdateInfo();
        }        

        protected override void OnDestroy()
        {

        }
        
        private void OnCreateCell(InfinityGridCell cell)
        {
            AttrCell lev = new AttrCell();
            lev.Init(cell.mRootTransform);
            cell.BindUserData(lev);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            AttrCell lev = cell.mUserData as AttrCell;
            lev.UpdateInfo(listDatas[index]);
        }

        private void UpdateInfo()
        {
            ItemData curOpEquip = Sys_Bag.Instance.GetItemDataByUuid(Sys_Equip.Instance.CurOpEquipUId);
            CSVEquipment.Data equipData = CSVEquipment.Instance.GetConfData(curOpEquip.Id);
            
            
            uint gemSumLev = 0;
            for (int i = 0; i < curOpEquip.Equip.JewelinfoId.Count; ++i)
            {
                uint jewelInfoId = curOpEquip.Equip.JewelinfoId[i];
                if (jewelInfoId != 0)
                {
                    CSVJewel.Data jewInfo = CSVJewel.Instance.GetConfData(jewelInfoId);
                    gemSumLev += jewInfo.level;
                }
            }

            int nextIndex = -1;
            if (equipData.jew_lev_score != null) //没配置 就没有选项
            {
                for (int i = 0; i < equipData.jew_lev_score.Count; ++i)
                {
                    if (gemSumLev < equipData.jew_lev_score[i][0])
                    {
                        nextIndex = i;
                        break;
                    }
                }
            }
            
            listDatas.Clear();
            if (nextIndex >= 0) //需要显示属性
            {
                txtTitle.text = LanguageHelper.GetTextContent(4253, equipData.jew_lev_score[nextIndex][0].ToString());
                txtInfo.text = LanguageHelper.GetTextContent(4254);
                
                List<uint> attrList = equipData.jew_lev_attr[nextIndex];
                int count = attrList.Count / 2;
                for (int i = 0; i < count; ++i)
                {
                    int startIndex = i * 2;
                    Int2 temp = new Int2();
                    temp.X = (int)attrList[startIndex];
                    temp.Y = (int)attrList[startIndex + 1];
                    listDatas.Add(temp);
                }
            }
            else
            {
                txtTitle.text = LanguageHelper.GetTextContent(4266);
                txtInfo.text = "";
            }
            
            _infinityGrid.CellCount = listDatas.Count;
            _infinityGrid.ForceRefreshActiveCell();
        }
    }
}

