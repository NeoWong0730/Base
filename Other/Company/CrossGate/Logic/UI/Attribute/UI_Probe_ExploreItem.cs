using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Framework;
using UnityEngine.EventSystems;
using Table;

namespace Logic
{
    public class UI_Probe_ExploreItem : UIComponent
    {
        private Toggle toggle;
        private Button imgLock;
        private Image imgIcon;

        private uint exploreId;
        private bool isOpen;
        private IListener listener;
        private Vector2 position;

        private CSVExploringSkill.Data imgIconData;        

        protected override void Loaded()
        {
            toggle = transform.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(OnClickToggle);
            imgLock = transform.Find("Image_Lock").GetComponent<Button>();
            imgIcon = transform.Find("Image").GetComponent<Image>();
            position = RectTransformUtility.WorldToScreenPoint(UIManager.mUICamera, transform.position);
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }
        
        public override void OnDestroy()
        {
            //Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateAttr, OnUpdateAttr, false); 
        }

        private void OnClickToggle(bool isOn)
        {
            if (isOn)
            {
                if (isOpen)
                {
                    OnToggle();
                }
                else
                {
                    //TODO: 弹未解锁提示
                }
            }
        }

        public void OnToggle()
        {
            listener?.OnSelectExploreId(exploreId, position);
        }

        public void RegisterListener(IListener _listen)
        {
            listener = _listen;
        }

        public void UpdateInfo(uint typeId)
        {
            exploreId = typeId * 100;
            uint funcId = 20500 + typeId;
            imgIconData = CSVExploringSkill.Instance.GetConfData(exploreId);
            ImageHelper.SetIcon(imgIcon, imgIconData.icon);
            isOpen = Sys_FunctionOpen.Instance.IsOpen(funcId, false);
            imgLock.gameObject.SetActive(!isOpen);
        }

        public void OnSelected(bool isOn)
        {
            if (toggle != null)
            {
                if (toggle.isOn && isOn)
                {
                    OnToggle();
                }
                toggle.isOn = isOn;
            }  
        }


        #region Interface
        public interface IListener
        {
            void OnSelectExploreId(uint exploreId, Vector2 pos);
        }
        #endregion
    }



}
