using System;
using System.Collections.Generic;

using Logic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
  
    public partial class UI_BlessingResult_Layout
    {

        Button BtnClose;

        public Transform TransSuccess;
        public Transform TransFail;

        public void Load(Transform root)
        {
            BtnClose = root.Find("Image_Black").GetComponent<Button>();
            TransSuccess = root.Find("Animator/Success");
            TransFail = root.Find("Animator/Fail");
        }

        public interface IListener
        {
            void OnClickClose();

        }


        public void SetListener(IListener listener)
        {
            BtnClose.onClick.AddListener(listener.OnClickClose);
        }


    }
}
