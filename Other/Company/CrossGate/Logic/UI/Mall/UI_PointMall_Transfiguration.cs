using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;

namespace Logic
{
    public class UI_PointMall_Transfiguration :  UI_PointMall_NormalShop, UI_PointMall.IListener
    {
        private EPointType pointType = EPointType.TransfigurationCard;

        public UI_PointMall_Transfiguration()
        {
            shopType = EPointShopType.TransfigurationCard;
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
