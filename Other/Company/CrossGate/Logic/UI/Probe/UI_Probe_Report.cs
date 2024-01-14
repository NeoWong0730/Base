using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using Table;
using Lib.Core;

namespace Logic
{  
    public class UI_Probe_Report_Layout
    {
        private Transform transform;
        private Button btnClose;
        private Button sure;
        public GameObject goodFeeling;
        public GameObject thread;
        public GameObject items;
        public GameObject item;
        public GameObject scroll;
        public GameObject prop;
        public VerticalLayoutGroup VerticalLayoutGroup;
        public VerticalLayoutGroup VerticalLayoutGroup1;
        public Text reportTitle;
        public Text npcName; //npc名称
        public Text npcAppellation; //npc称谓
        public Text reportView;
        public Text threadText;
        public uint npcId;
        public uint testId; //测试用
        public uint dropId;  //掉落Id
        public uint counter; //计数器
        public string reportViewResult;
        public CSVDetect.Data cSVDetectData;
        public CSVNpc.Data npcData;

        public void Init(Transform transform)
        {
            this.transform = transform;
            btnClose = transform.Find("Animator/View_TipsBg01_Small/Btn_Close").GetComponent<Button>();
            sure = transform.Find("Animator/Button_Sure").GetComponent<Button>();
            reportTitle = transform.Find("Animator/Image_Title/Text").GetComponent<Text>();
            reportView = transform.Find("Animator/Text").GetComponent<Text>();
            threadText = transform.Find("Animator/Scroll_View/View/Image_Title1/Text_Title").GetComponent<Text>();
            goodFeeling = transform.Find("Animator/Scroll_View/View/Image_Title").gameObject;
            thread = transform.Find("Animator/Scroll_View/View/Image_Title1").gameObject;
            items = transform.Find("Animator/Scroll_View/View/Prop/Viewport").gameObject;
            item = transform.Find("Animator/Scroll_View/View/Prop/Viewport/Item").gameObject;

            scroll = transform.Find("Animator/Scroll_View").gameObject;
            prop = transform.Find("Animator/Scroll_View/View/Prop").gameObject;
            VerticalLayoutGroup = transform.Find("Animator/Scroll_View").GetComponent<VerticalLayoutGroup>();
            VerticalLayoutGroup1 = transform.Find("Animator/Scroll_View/View").GetComponent<VerticalLayoutGroup>();
        }
        public void ProbeText()
        { 
            reportViewResult = LanguageHelper.GetTextContent(cSVDetectData.Result);
            reportTitle.text = LanguageHelper.GetNpcTextContent(npcData.appellation) + LanguageHelper.GetNpcTextContent(npcData.name);
            if (cSVDetectData.ClueText != 0)
            {
                threadText.text = LanguageHelper.GetTextContent(2007614, LanguageHelper.GetTextContent(cSVDetectData.ClueText));
            }
            reportView.text = LanguageHelper.GetTextContent(2007616, Sys_Role.Instance.sRoleName, reportViewResult);
            dropId = cSVDetectData.Reward; //掉落Id
            UpdateGrids(dropId);
        }
        public void UpdateData(uint _npcId, uint _testId)
        {
            npcId = _npcId;
            testId = _testId;
            cSVDetectData = CSVDetect.Instance.GetConfData(_testId);
            npcData = CSVNpc.Instance.GetConfData(_npcId);

        }

        public void UpdateGrids(uint dropId)
        {
            List<ItemIdCount> itemIdCounts = CSVDrop.Instance.GetDropItem(dropId);
            for (int i = 0; i < itemIdCounts.Count; i++)
            {
                GameObject go = GameObject.Instantiate<GameObject>(item, items.transform);
                PropItem propItem = new PropItem();
                propItem.BindGameObject(go);
                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(itemIdCounts[i].id, itemIdCounts[i].count, true, false, false, false, false, true);
                propItem.SetData(new MessageBoxEvt(EUIID.UI_Probe_Report, itemData));
            }
            item.SetActive(false);
        }

        public void DefaultItem()
        {
            item.SetActive(true);
            FrameworkTool.DestroyChildren(items, item.transform.name);
        }

        public void RegisterEvents(IListener listener)
        {
            btnClose.onClick.AddListener(listener.OnClickClose);
            sure.onClick.AddListener(listener.OnClickClose);
        }

        //判断显示调查结果种类
        public void CheckEmpty()
        {
            counter = 0;
            if (cSVDetectData.ClueText == 0)
            {
                thread.gameObject.SetActive(false);
            }
            else
            {
                thread.gameObject.SetActive(true);
                counter += 1;
            }
            if (cSVDetectData.Favorabilityid == 0)
            {
                goodFeeling.SetActive(false);

            }
            else
            {
                goodFeeling.SetActive(true);
                counter += 2;
            }
            if (dropId == 0)
            {
                prop.SetActive(false);
            }
            else
            {
                prop.SetActive(true);
                counter += 3;
            }
        }

        //条件更改UI界面
        public void SetUISize()
        {
            if (counter == 1)
            {
                VerticalLayoutGroup.padding.bottom = 31;
                VerticalLayoutGroup1.spacing = 26;
            }
            else
            {
                VerticalLayoutGroup.padding.bottom = 6;
                VerticalLayoutGroup1.spacing = 7;
            }
            if (counter == 0)
            {
                scroll.gameObject.SetActive(false);
            }

            ForceRebuildLayout(VerticalLayoutGroup1.gameObject);
        }

        private void ForceRebuildLayout(GameObject go)
        {
            ContentSizeFitter[] fitter = go.GetComponentsInChildren<ContentSizeFitter>();
            for (int i = fitter.Length - 1; i >= 0; --i)
            {
                RectTransform trans = fitter[i].gameObject.GetComponent<RectTransform>();
                if (trans != null)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(trans);
            }
        }

        public interface IListener
        {
            void OnClickClose();
          
        }

    }

    public class UI_Probe_Report : UIBase, UI_Probe_Report_Layout.IListener
    {
        private UI_Probe_Report_Layout layout = new UI_Probe_Report_Layout();

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);

        }

        protected override void OnOpen(object arg)
        {
            Sys_Inquiry.InquiryData inquiryData = arg as Sys_Inquiry.InquiryData;
            if (inquiryData != null)
            {
                layout.UpdateData(inquiryData.npcID, inquiryData.inquiryID); //填测试数据
            }

            if (Sys_Team.Instance.isCaptain(Sys_Role.Instance.RoleId) || !Sys_Team.Instance.HaveTeam) {// 进入交互状态
                GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterInteractive);
                ActionCtrl.ActionExecuteLockFlag = true;
            }
        }

        protected override void OnShow()
        {            
            layout.scroll.gameObject.SetActive(true);
            layout.ProbeText();
            layout.CheckEmpty();
            layout.SetUISize();
        }

        protected override void OnClose() 
        {
            if (Sys_Team.Instance.isCaptain(Sys_Role.Instance.RoleId) || !Sys_Team.Instance.HaveTeam) {// 进入交互状态
                GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
                ActionCtrl.ActionExecuteLockFlag = false;
            }

            layout.DefaultItem();
        }


        public void OnClickClose()
        {
            CloseSelf();
        }
    }
}