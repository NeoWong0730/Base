using UnityEngine;
using UnityEngine.UI;
using Table;
using Packet;

namespace Logic
{
    public class UI_Partner_Fetter_Detail_BondInfo
    {
        private Transform transform;

        private Image imgIcon;
        private Text txtName;
        private Text txtActive;

        public void Init(Transform trans)
        {
            transform = trans;

            imgIcon = transform.Find("Image_Icon").GetComponent<Image>();
            txtName = transform.Find("Image_Name/Text_Name").GetComponent<Text>();
            txtActive = transform.Find("Text_Activate").GetComponent<Text>();
        }

        public void UpdateInfo(uint bondId)
        {
            CSVBond.Data data = CSVBond.Instance.GetConfData(bondId);
            ImageHelper.SetIcon(imgIcon, data.icon);
            txtName.text = LanguageHelper.GetTextContent(data.name);

            uint count = 0;
            PartnerBond bond = Sys_Partner.Instance.GetBondData(bondId);
            if (bond != null)
                count = bond.Index;
            txtActive.text = LanguageHelper.GetTextContent(2006072, count.ToString(), data.group_effect.Count.ToString());
        }
    }
}
