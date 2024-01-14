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
    public class UI_Equipment_Inlay_JewelList_Cell : UIParseCommon
    {
        private Button btnDark;
        private Text textDark;
        private Button btnLight;
        private Text textLight;
        private Transform dotTrans;

        private GameObject grid;

        private UI_Equipment_Inlay_JewelIconNone jewelNone;
        private GameObject jewelTemplate;
        private List<UI_Equipment_Inlay_JewelIconRoot> listJewels = new List<UI_Equipment_Inlay_JewelIconRoot>();

        public bool IsRedDot;

        protected override void Parse()
        {
            btnDark = transform.Find("ItemBig/Btn_Menu_Dark").GetComponent<Button>();
            textDark = btnDark.transform.Find("Text_Menu_Dark").GetComponent<Text>();
            btnLight = transform.Find("ItemBig/Btn_Menu_Light").GetComponent<Button>();
            textLight = btnLight.transform.Find("Text_Menu_Dark").GetComponent<Text>();
            dotTrans = transform.Find("ItemBig/Image_Dot");

            btnDark.onClick.AddListener(() => { OnExpand(true); });
            btnLight.onClick.AddListener(() => { OnExpand(false); });

            grid = transform.Find("Grid").gameObject;

            jewelNone = new UI_Equipment_Inlay_JewelIconNone();
            jewelNone.Init(transform.Find("Grid/ItemSmallNone"));

            jewelTemplate = transform.Find("Grid/ItemSmall").gameObject;
            jewelTemplate.SetActive(false);
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

        public void OnListJewel(uint jewelType)
        {
            Lib.Core.FrameworkTool.DestroyChildren(jewelTemplate.transform.parent.gameObject, jewelTemplate.name, jewelNone.gameObject.name);
            listJewels.Clear();

            List<Sys_Equip.JewelGroupData> jewelList = Sys_Equip.Instance.GetJewelTotalList((EJewelType)jewelType);

            jewelNone.gameObject.SetActive(jewelList.Count == 0);
            if (jewelList.Count == 0)
            {
                jewelNone.ConstructJewel(jewelType);
            }

            IsRedDot = false;
            foreach (Sys_Equip.JewelGroupData groupData in jewelList)
            {
                GameObject jewelGo = GameObject.Instantiate<GameObject>(jewelTemplate, jewelTemplate.transform.parent);
                jewelGo.SetActive(true);

                UI_Equipment_Inlay_JewelIconRoot jewelRoot = new UI_Equipment_Inlay_JewelIconRoot();
                jewelRoot.Init(jewelGo.transform);
                jewelRoot.UpdateJewel(groupData);
                listJewels.Add(jewelRoot);

                if (!IsRedDot)
                    IsRedDot = Sys_Equip.Instance.CheckJewelCanInlay(groupData.itemId);
            }

            textDark.text = textLight.text = LanguageHelper.GetTextContent(4161 + (uint)jewelType);
            dotTrans.gameObject.SetActive(IsRedDot);

            //默认不展开
            grid.SetActive(false);
        }

        public void OnExpand(bool expand)
        {
            btnLight.gameObject.SetActive(expand);
            btnDark.gameObject.SetActive(!expand);

            grid.SetActive(expand);
        }
    }
}


