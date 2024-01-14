using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Framework;
using Lib.Core;
using Table;
using UnityEngine.EventSystems;
using System;

namespace Logic
{
    public class FashSuitChangeGrid
    {
        private Transform transform;
        private Transform attrParent;
        private GameObject select;
        private Text attrName1;
        private Text attrValue1;
        private Text attrName2;
        private Text attrValue2;
        private Action<FashSuitChangeGrid> onClick;
        private uint attrId;
        public int dataIndex;
        private CSVFashionAttr.Data cSVFashionAttrData;

        public void BindGameObject(GameObject go)
        {
            transform = go.transform;
            ParseGo();
        }

        private void ParseGo()
        {
            attrName1 = transform.Find("Grid_Attr/Attr01/Text").GetComponent<Text>();
            attrValue1 = attrName1.transform.Find("Text_Num").GetComponent<Text>();
            attrName2 = transform.Find("Grid_Attr/Attr02/Text").GetComponent<Text>();
            attrValue2 = attrName2.transform.Find("Text_Num").GetComponent<Text>();
            select = transform.Find("Image_Check").gameObject;
            attrParent = transform.Find("Grid_Attr");
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(transform.Find("Image"));
            eventListener.AddEventListener(EventTriggerType.PointerClick, OnGridClicked);
        }

        public void SetData( uint suitid, int _dataIndex)
        {
            attrId = CSVFashionSuit.Instance.GetConfData(suitid).attr_id;
            cSVFashionAttrData = CSVFashionAttr.Instance.GetConfData(attrId);
            dataIndex = _dataIndex;
            Refresh();
        }

        private void Refresh()
        {
            int needCount = cSVFashionAttrData.attr_id.Count;
            FrameworkTool.CreateChildList(attrParent, needCount);
            for (int i = 0; i < needCount; i++)
            {
                Transform child = attrParent.GetChild(i);
                Text attrName = child.Find("Text").GetComponent<Text>();
                Text attrValue = child.Find("Text/Text_Num").GetComponent<Text>();
                uint suitAttr1 = cSVFashionAttrData.attr_id[i][0];
                uint suitValue1 = cSVFashionAttrData.attr_id[i][1];
                SetAttr(suitAttr1, suitValue1, 0, 0, attrName, attrValue, null, null);
            }
        }

        private void SetAttr(uint attr1, uint value1, uint attr2, uint value2, Text attrName1, Text attrValue1, Text attrName2, Text attrValue2)
        {
            if (attrName1 != null)
            {
                CSVAttr.Data cSVAttrData = CSVAttr.Instance.GetConfData(attr1);
                TextHelper.SetText(attrName1, cSVAttrData.name);
                if (cSVAttrData.show_type == 1)
                {
                    attrValue1.text = string.Format("+{0}", value1);
                }
                else
                {
                    attrValue1.text = string.Format("+{0}%", value1 / 100f);
                }
            }
            if (attrName2 != null)
            {

                CSVAttr.Data cSVAttrData = CSVAttr.Instance.GetConfData(attr2);
                TextHelper.SetText(attrName2, cSVAttrData.name);
                if (cSVAttrData.show_type == 1)
                {
                    attrValue2.text = string.Format("+{0}", value2);
                }
                else
                {
                    attrValue2.text = string.Format("+{0}%", value2 / 100f);
                }
            }
        }

        public void AddClickListener(Action<FashSuitChangeGrid> _onClick)
        {
            onClick = _onClick;
        }

        public void Release()
        {
            select.SetActive(false);
        }

        public void Select()
        {
            select.SetActive(true);
        }

        private void OnGridClicked(BaseEventData baseEventData)
        {
            onClick.Invoke(this);
        }
    }

    public class UI_Fashion_SuitChange : UIBase
    {
        private Button closeBtn;
        private Button okBtn;
        
        private Transform scrollcontentParent;
        private InfinityGridLayoutGroup infinity;
        private Dictionary<GameObject, FashSuitChangeGrid> suitGrids = new Dictionary<GameObject, FashSuitChangeGrid>();

        private List<uint> changeList ;

        private int curSuitSelectIndex;

        protected override void OnLoaded()
        {
            scrollcontentParent = transform.Find("Animator/Scroll/Content");
            okBtn = transform.Find("Animator/Btn_01").GetComponent<Button>();
            closeBtn = transform.Find("Animator/View_TipsBg01_Small/Btn_Close").GetComponent<Button>();
            ConstructInfinityGroup();
            closeBtn.onClick.AddListener(() => UIManager.CloseUI(EUIID.UI_Fashion_SuitChange));
            okBtn.onClick.AddListener(()=> 
            {
                uint toUseAttrId = changeList[curSuitSelectIndex];
                Sys_Fashion.Instance.SuitAddrReq(Sys_Fashion.Instance.curSuit, toUseAttrId);
                UIManager.CloseUI(EUIID.UI_Fashion_SuitChange);
            });
        }

        protected override void OnShow()
        {
            changeList = new List<uint>(Sys_Fashion.Instance._UnLockedSuits);
            if (!changeList.Remove(Sys_Fashion.Instance.curUseSuit))
            {
                Debug.LogErrorFormat("套装解锁列表未包含当前穿戴套装{0}", Sys_Fashion.Instance.curUseSuit);
            }
            infinity.SetAmount(changeList.Count);
        }

        private void ConstructInfinityGroup()
        {
            infinity = scrollcontentParent.gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            infinity.minAmount = 4;
            infinity.updateChildrenCallback = UpdateChildrenCallback;
            for (int i = 0; i < scrollcontentParent.childCount; i++)
            {
                GameObject go = scrollcontentParent.GetChild(i).gameObject;
                FashSuitChangeGrid fashSuitChangeGrid = new FashSuitChangeGrid();
                fashSuitChangeGrid.BindGameObject(go);
                fashSuitChangeGrid.AddClickListener(OnGridSelected);
                suitGrids.Add(go, fashSuitChangeGrid);
            }
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            FashSuitChangeGrid fashSuitChangeGrid = suitGrids[trans.gameObject];
            fashSuitChangeGrid.SetData(changeList[index], index);
            if (index != curSuitSelectIndex)
            {
                fashSuitChangeGrid.Release();
            }
            else
            {
                fashSuitChangeGrid.Select();
            }
        }

        private void OnGridSelected(FashSuitChangeGrid fashSuitChangeGrid)
        {
            curSuitSelectIndex = fashSuitChangeGrid.dataIndex;
            foreach (var item in suitGrids)
            {
                if (item.Value.dataIndex == curSuitSelectIndex)
                {
                    item.Value.Select();
                }
                else
                {
                    item.Value.Release();
                }
            }
        }
    }
}

