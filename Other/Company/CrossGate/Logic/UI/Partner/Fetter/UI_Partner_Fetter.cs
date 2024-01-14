using UnityEngine;

namespace Logic
{
    public class UI_Partner_Fetter : UIParseCommon
    {
        private UI_Partner_Fetter_List list;
        private UI_Partner_Fetter_Result result;

        protected override void Parse()
        {
            list = new UI_Partner_Fetter_List();
            list.Init(transform.Find("Scroll View"));

            result = new UI_Partner_Fetter_Result();
            result.Init(transform.Find("View_Result"));
        }

        public override void Show()
        {
            gameObject.SetActive(true);
            UpdateInfo();
        }

        public override void Hide()
        {
            //left.Hide();
            //right.Hide();
            gameObject.SetActive(false);
        }

        private void UpdateInfo()
        {
            list?.UpdateInfo();
            result?.UpdateInfo();
        }
    }
}
