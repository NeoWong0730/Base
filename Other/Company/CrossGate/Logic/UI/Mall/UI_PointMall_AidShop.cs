using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;

namespace Logic
{
    public class UI_PointMall_AidShop : UI_PointMall_NormalShop, UI_PointMall.IListener
    {
        private EPointType pointType = EPointType.Aid;

        public UI_PointMall_AidShop()
        {
            shopType = EPointShopType.Aid;
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
            uint getPoint = Sys_Attr.Instance.GetDailyCurAidPoint();
            uint maxPoint = CSVCharacterAttribute.Instance.GetConfData(Sys_Role.Instance.Role.Level).AidPointLimit;
            uint currentPoint = (uint)Sys_Bag.Instance.GetItemCount((uint)pointType);
            txtDailyGetPoint.text = getPoint + "/" + maxPoint;
            txtAllPoint.text = currentPoint.ToString();
            LayoutRebuilder.ForceRebuildLayoutImmediate(tips2);
            LayoutRebuilder.ForceRebuildLayoutImmediate(tips3);
        }
    }
}
