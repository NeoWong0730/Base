using Lib.Core;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Table;
using UnityEngine.EventSystems;
using Packet;
using Lib.AssetLoader;
using DG.Tweening;

namespace Logic
{
    public class UI_Skip : UIBase 
    {
        private Timer _timer;

        protected override void OnLoaded()
        {

        }

        protected override void OnOpen(object arg)
        {

        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        protected override void OnHide()
        {
            _timer?.Cancel();
            _timer = null;
        }

        protected override void OnDestroy()
        {

        }

        private void UpdateInfo()
        {
            _timer?.Cancel();
            _timer = Timer.Register(15f, () =>
            {
                this.CloseSelf();
            });
        }
    }
}


