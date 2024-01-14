using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework;
using Logic.Core;
using DG.Tweening;

namespace Logic
{
    public class UI_Loading3 : UIBase
    {
        private bool bLoading = true;
        private float time = 0;
        
        protected override void OnOpen(object arg)
        {
            bLoading = true;
        }

        protected override void OnShow()
        {
            time = Time.unscaledTime;
        }

        protected override void OnUpdate()
        {
            if (bLoading)
            {
                if (GameCenter.nLoadStage == 3)
                {
                    time = Time.unscaledTime;
                    bLoading = false;
                }
            }
            else
            {
                if (time + 1f <= Time.unscaledTime)
                {
                    UIManager.CloseUI(EUIID.UI_Loading3);
                }
            }
        }
    }

}