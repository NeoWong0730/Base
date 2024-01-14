using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;

namespace Logic 
{
    public class UI_Awaken_Tips_Layout
    {
        public Button closeBtn;
        public Text lastTitle;
        public Text nextTitle;
        public GameObject attrItem;
        public GameObject attrPanel;
        public GameObject tipsAwardPanel;
        public GameObject tipsAwardItem;
        public void Init(Transform transform)
        {
            closeBtn = transform.Find("Animator/Image_BG").GetComponent<Button>();
            lastTitle = transform.Find("Animator/View_Content/Text_Stage").GetComponent<Text>();
            nextTitle= transform.Find("Animator/View_Content/Text_Stage/Text_Next").GetComponent<Text>();
            attrItem = transform.Find("Animator/View_Content/View_Atrri/Title_Up/View_Des/Item").gameObject;
            attrPanel=transform.Find("Animator/View_Content/View_Atrri/Title_Up/View_Des").gameObject;
            tipsAwardPanel = transform.Find("Animator/View_Content/View_Atrri/Title_Award").gameObject;
            tipsAwardItem= transform.Find("Animator/View_Content/View_Atrri/Title_Award/Award/Item").gameObject;
        }
        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OnCloseBtnClicked);
        }
        public interface IListener
        {
            void OnCloseBtnClicked();
        }
    }

    public class UI_AwakenAttrItem : UIComponent
    {
        private uint id;
        private int type;
        private Text attrName;
        private Text currValue;
        private Text nextValue;
        private CSVTravellerAwakening.Data csvData;
        private CSVTravellerAwakening.Data csvDataNext;

        public UI_AwakenAttrItem(uint _id,int _type) : base()
        {
            id = _id;
            type = _type;   
        }
        protected override void Loaded()
        {
            attrName = transform.Find("Text").GetComponent<Text>();
            currValue= transform.Find("Text_Current").GetComponent<Text>();
            nextValue= transform.Find("Text_Next").GetComponent<Text>();
        }

        public void SetAttrValue()
        {
            csvData = CSVTravellerAwakening.Instance.GetConfData(id - 1);
            csvDataNext = CSVTravellerAwakening.Instance.GetConfData(id);
            if (csvData == null)
            {
                return;
            }
            if (csvDataNext == null)
            {
                return;
            }
            attrName.text = LanguageHelper.GetTextContent(csvDataNext.show_attr_name[type]);
            currValue.text = LanguageHelper.GetTextContent(2107003,(csvData.show_attr_value[type]/ 100).ToString()); ;
            nextValue.text = LanguageHelper.GetTextContent(2107003, (csvDataNext.show_attr_value[type] / 100).ToString());
        }
    }

    public class UI_Awaken_Tips : UIBase, UI_Awaken_Tips_Layout.IListener
    {

        private UI_Awaken_Tips_Layout layout = new UI_Awaken_Tips_Layout();
        private uint currAwakenId;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }
        protected override void OnShow()
        {            
            DestoryItem();
            currAwakenId = Sys_TravellerAwakening.Instance.awakeLevel;
            SetTitle(currAwakenId - 1, layout.lastTitle);//设置上一阶称号
            SetTitle(currAwakenId, layout.nextTitle);//设置这一阶称号
            SetAttriList(currAwakenId);
        }    
        private void DestoryItem()
        {
            FrameworkTool.DestroyChildren(layout.attrItem.transform.parent.gameObject, layout.attrItem.transform.name);
            FrameworkTool.DestroyChildren(layout.tipsAwardItem.transform.parent.gameObject, layout.tipsAwardItem.transform.name);
        }
        private void SetTitle(uint indexId, Text titleName)
        {
            CSVTravellerAwakening.Data csvAwakeData;

            if (CSVTravellerAwakening.Instance.TryGetValue(indexId, out csvAwakeData) && csvAwakeData != null)
            {
                //titleName.text = string.Concat(LanguageHelper.GetTextContent(csvAwakeData.NameId), LanguageHelper.GetTextContent(csvAwakeData.StepsId));
                titleName.text = LanguageHelper.GetTextContent(csvAwakeData.NameId);
            }
        }
        private void SetAttriList(uint indexId)
        {
            CSVTravellerAwakening.Data csvAwakeData;

            csvAwakeData = CSVTravellerAwakening.Instance.GetConfData(indexId);
            layout.attrItem.SetActive(true);
            FrameworkTool.CreateChildList(layout.attrItem.transform.parent, csvAwakeData.show_attr_name.Count);
            for (int i = 0; i < csvAwakeData.show_attr_name.Count; i++)
            {
                UI_AwakenAttrItem aItem = new UI_AwakenAttrItem(indexId, i);
                aItem.Init(layout.attrItem.transform.parent.GetChild(i).transform);
                aItem.SetAttrValue();
            }

            if (csvAwakeData.Award != null)
            {
                layout.tipsAwardPanel.SetActive(true);
                SetAwardList(csvAwakeData);

            }
            else
            {
                layout.tipsAwardPanel.SetActive(false);
            }
        }

        private void SetAwardList(CSVTravellerAwakening.Data awakedata)
        {
            FrameworkTool.CreateChildList(layout.tipsAwardItem.transform.parent, awakedata.Award.Count);
            for (int i = 0; i < awakedata.Award.Count; i++)
            {
                GameObject go = layout.tipsAwardItem.transform.parent.GetChild(i).gameObject; 
                go.SetActive(true);
                PropItem propItem = new PropItem();
                propItem.BindGameObject(go);
                propItem.SetData(new MessageBoxEvt(EUIID.UI_Awaken_Tips, new PropIconLoader.ShowItemData(awakedata.Award[i][0], awakedata.Award[i][1], true, false, false, false, false,
                       _bShowCount: true, _bUseClick: true, _bShowBagCount: false)));
            }
            layout.tipsAwardItem.SetActive(false);
        }

        public void OnCloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Awaken_Tips);
        }
    }
}