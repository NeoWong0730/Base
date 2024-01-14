using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Lib.Core;
using Logic.Core;
using Table;
using Framework;

namespace Logic
{
    public class UI_Knowledge_Brave_Detail : UIBase, UI_Knowledge_Brave_Detail_Left.IListener
    {

        private UI_Knowledge_Brave_Detail_Left _left;
        private UI_Knowledge_Brave_Detail_Center _center;
        private UI_Knowledge_Brave_Detail_Right _right;

        private uint _braveId;

        protected override void OnLoaded()
        {            
            AssetDependencies dependence = transform.GetComponent<AssetDependencies>();

            Button btnClose = transform.Find("Animator/View_Title07/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(()=> { this.CloseSelf(); });

            _left = new UI_Knowledge_Brave_Detail_Left();
            _left.Init(transform.Find("Animator/Image_BG0/Toggle_Left"));
            _left.Register(this);

            _center = new UI_Knowledge_Brave_Detail_Center();
            _center.Init(transform.Find("Animator/Image_BG1/Center"));
            _center.SceneObj = dependence.mCustomDependencies[0];

            _right = new UI_Knowledge_Brave_Detail_Right();
            _right.Init(transform.Find("Animator/Image_BG1/Right"));
        }

        protected override void OnOpen(object arg)
        {            
            _braveId = 0;
            if (arg != null)
                _braveId = (uint)arg;

            Sys_Knowledge.Instance.CurBraveId = _braveId;
        }        

        protected override void OnShow()
        {         
            _left?.UpdateInfo();
        }

        protected override void OnHide()
        {
            _center?.OnHide();
        }

        protected override void OnDestroy()
        {
            _left?.OnDestroy();
        }

        public void OnSelectBrave(uint braveId)
        {
            Sys_Knowledge.Instance.CurBraveId = braveId;

            _center?.UpdateInfo(braveId);
            _right?.UpdateInfo(braveId);
        }
    }
}


