using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Framework;
using Logic.Core;
using Packet;
using UnityEngine.Playables;

namespace Logic
{
    public class UI_OperationalActivityBase : UIComponent, UI_OperationalActivity.IListener
    {
        private bool isInit = false;
        public virtual void SetOpenValue(uint openValue)
        {

        }

        public override void Show()
        {
            if (!isInit)
            {
                isInit = true;
                InitBeforOnShow();
            }
            
            base.Show();
        }
        /// <summary>
        /// 在onshow之前初始化，不在Onload里
        /// </summary>
        protected virtual void InitBeforOnShow() {

        }
    }
}
