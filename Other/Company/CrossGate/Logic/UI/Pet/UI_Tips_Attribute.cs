using UnityEngine.UI;
using Logic.Core;
using Table;

namespace Logic
{
    public class TipsContent
    {
        public string attrName { get; set; }
        public long InitPoint { get; set; }
        public long addPoint { get; set; }
        public long addGradePoint { get; set; }
        public float addGradePrecent { get; set; }
        public long remouldAddPoint { get; set; }

    }

    public class UI_Tips_Attribute :UIBase
    {
        private Text InitPoint;
        private Text addPoint;
        private Text addGradePoint;
        private Text remouldAddPoint;
        private Button closeBtn;
        private TipsContent tip;

        protected override void OnLoaded()
        {
            InitPoint = transform.Find("Content/Text").GetComponent<Text>();
            addPoint = transform.Find("Content/Text1").GetComponent<Text>();
            addGradePoint = transform.Find("Content/Text2").GetComponent<Text>();
            remouldAddPoint = transform.Find("Content/Text3").GetComponent<Text>();

            closeBtn = transform.Find("Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(OnCloseClicked);
        }

        protected override void OnOpen(object arg)
        {            
            tip = (TipsContent)arg;
        }

        protected override void OnShow()
        {
            InitPoint.text = LanguageHelper.GetTextContent(12036, tip.InitPoint.ToString(), tip.attrName);
            addPoint.text = LanguageHelper.GetTextContent(12037, tip.addPoint.ToString(), tip.attrName);   
            addGradePoint.text = LanguageHelper.GetTextContent(12038, tip.addGradePrecent.ToString(), tip.addGradePoint.ToString(), tip.attrName);
            remouldAddPoint.text = LanguageHelper.GetTextContent(12039, tip.remouldAddPoint.ToString(), tip.attrName);
        }        

        private void OnCloseClicked()
        {
            UIManager.CloseUI(EUIID.UI_Tips_Attribute);
        }
    }
}
