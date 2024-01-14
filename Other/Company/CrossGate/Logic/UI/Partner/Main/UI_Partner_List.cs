using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    public class UI_Partner_List : UIParseCommon, UI_Partner_List_Top.IListener, UI_Partner_List_Left.IListener
    {
        private UI_Partner_List_Top top;
        private UI_Partner_List_Left left;
        private UI_Partner_List_Right right;

        protected override void Parse()
        {
            top = new UI_Partner_List_Top();
            top.Init(transform.Find("View_Top"));
            top.RegisterListener(this);

            left = new UI_Partner_List_Left();
            left.Init(transform.Find("View_Left"));
            left.RegisterListener(this);

            right = new UI_Partner_List_Right();
            right.Init(transform.Find("View_Right"));
            
        }

        public override void Show()
        {
            gameObject.SetActive(true);

            top.Show();
            left.Show();
            right.Show();
        }

        public override void Hide()
        {
            top.Hide();
            left.Hide();
            right.Hide();

            gameObject.SetActive(false);
        }

        public override void OnDestroy()
        {
            right?.OnDestroy();
        }

        public void OnSelectType(EPartnerType _type)
        {
            left?.OnSiftingByType(_type);
        }

        public void OnSelectListIndex(uint _typeId, List<uint> _listIds)
        {
            List<uint> listInofIds = Sys_Partner.Instance.GetPartnerListInfoIds(_typeId, _listIds);
            right?.UpdatePartner(listInofIds);
        }
    }
}
