using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;

namespace Logic
{
    public class UI_PointMall_AncientCoinShop : UI_PointMall_NormalShop
    {
        private EPointType pointType = EPointType.AncientCoin;

        public UI_PointMall_AncientCoinShop()
        {
            shopType = EPointShopType.AncientCoin;
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
