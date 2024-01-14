using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;

namespace Logic
{
    public class UI_PointMall_CaptainShop : UI_PointMall_NormalShop,UI_PointMall.IListener
    {
        private EPointType pointType = EPointType.Captain;

        public UI_PointMall_CaptainShop()
        {
            shopType = EPointShopType.Captain;
        }
        protected override void Loaded()
        {
            base.Loaded();
        }
        public override void Show()
        {
            base.Show();
            Sys_Attr.Instance.GetDailyPointReq();
        }

        protected override void OnDailyPointUpdate()
        {
            base.OnDailyPointUpdate();
            uint getPoint =Sys_Attr.Instance.GetDailyCurCaptainPoint();
            uint maxPoint = CSVCharacterAttribute.Instance.GetConfData(Sys_Role.Instance.Role.Level).CaptainPointLimit;
            uint currentPoint = (uint)Sys_Bag.Instance.GetItemCount((uint)pointType);
            txtDailyGetPoint.text = getPoint + "/" + maxPoint;
            txtAllPoint.text = currentPoint.ToString();
            LayoutRebuilder.ForceRebuildLayoutImmediate(tips2);
            LayoutRebuilder.ForceRebuildLayoutImmediate(tips3);
        }
    }
}
