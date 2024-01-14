using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using System.Linq;
using Lib.Core;
using Framework;
using System;

namespace Logic
{
    public class UI_TrialGate_Icon : UIComponent
    {
        private Animator animator;

        protected override void Loaded()
        {
            animator = transform.Find("Animator/Root").GetComponent<Animator>();
        }

         public void PlayTransformFx()
        {
            animator.Play("Transfer", -1, 0);
        }

        public override void OnDestroy()
        {
            
        }
    }
}
