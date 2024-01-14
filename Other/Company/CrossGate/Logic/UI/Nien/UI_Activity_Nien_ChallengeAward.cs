using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;
using Framework;
using System;
using System.Diagnostics.Tracing;
using Lib.Core;

namespace Logic
{
    public class UI_Activity_Nien_ChallengeAward  : UIBase
    {
        public class AwardType
        {
            private Transform transform;
            private CP_Toggle _toggle;
            private int _index;
            private Action<int> _action;
            public void Init(Transform trans)
            {
                transform = trans;
                _toggle = transform.GetComponent<CP_Toggle>();
                _toggle.onValueChanged.AddListener(OnClick);
            }

            public void SetIndex(int index)
            {
                _index = index;
            }

            public void Regsiter(Action<int> action)
            {
                _action = action;
            }

            public void OnSelect()
            {
                _toggle.SetSelected(true, true);
            }

            private void OnClick(bool isOn)
            {
                if (isOn)
                {
                    _action?.Invoke(_index);
                }
            }
        }
        
        private List<AwardType> listTypes = new List<AwardType>();
        private UI_Activity_Nien_AwardListRank _listRank;
        private UI_Activity_Nien_AwardListDmg _listDmg;
        
        protected override void OnLoaded()
        {
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(transform.Find("Animator/eventBg").GetComponent<Image>());
            eventListener.AddEventListener(EventTriggerType.PointerClick, OnClickClose);

            listTypes.Clear();
            Transform transToggle = transform.Find("Animator/ScrollView_Menu/List");
            int count = transToggle.childCount;
            for (int i = 0; i < count; ++i)
            {
                AwardType type = new AwardType();
                type.Init(transToggle.GetChild(i));
                type.SetIndex(i);
                type.Regsiter(OnSelectType);
                listTypes.Add(type);
            }
            
            _listRank = new UI_Activity_Nien_AwardListRank();
            _listRank.Init(transform.Find("Animator/Image_Bg/Scroll View01"));
            
            _listDmg = new UI_Activity_Nien_AwardListDmg();
            _listDmg.Init(transform.Find("Animator/Image_Bg/Scroll View02"));
        }

        protected override void OnOpen(object arg)
        {

        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            // Sys_MallActivity.Instance.eventEmitter.Handle(Sys_MallActivity.EEvents.OnFillShopData, OnFillShopData, toRegister);
            // Sys_MallActivity.Instance.eventEmitter.Handle(Sys_MallActivity.EEvents.OnRefreshShopData, OnRefreshShopData, toRegister);
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        protected override void OnHide()
        {

        }

        protected override void OnDestroy()
        {

        }

        private void OnSelectType(int index)
        {
            if (index == 0) //名次排行
            {
                _listDmg.OnHide();
                _listRank.OnShow();
                _listRank.UpdateInfo(1900001);
            }
            else if (index == 1) //伤害排行
            {
                _listRank.OnHide();
                _listDmg.OnShow();
                _listDmg.UpdateInfo(1900001);
            }
        }

        private void OnClickClose(BaseEventData events)
        {
            this.CloseSelf();
        }
        

        private void UpdateInfo()
        {
            listTypes[0].OnSelect();
        }
    }
}


