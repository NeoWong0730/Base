using UnityEngine;

namespace Logic
{
    public class UI_Partner_Config : UIParseCommon
    {
        private UI_Partner_Config_Left left;
        private UI_Partner_Config_Right right;

        protected override void Parse()
        {
            left = new UI_Partner_Config_Left();
            left.Init(transform.Find("View_Left"));

            right = new UI_Partner_Config_Right();
            right.Init(transform.Find("View_Right"));
        }

        public override void Show()
        {
            gameObject.SetActive(true);

            left.Show();
            right.Show();
        }

        public override void Hide()
        {
            left.Hide();
            right.Hide();
            gameObject.SetActive(false);
        }
    }
}
