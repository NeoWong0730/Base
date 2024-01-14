using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;

namespace Logic
{
    public class UI_PointMall_TravelShop : UI_PointMall_NormalShop
    {
        private EPointType pointType = EPointType.TravelersLog;

        public UI_PointMall_TravelShop()
        {
            shopType = EPointShopType.TravelersLog;
        }

        protected override void Loaded()
        {
            base.Loaded();
        }
        public override void Show()
        {
            base.Show();
        }

        protected override void OnDailyPointUpdate()
        {
            base.OnDailyPointUpdate();
        }
    }
}
