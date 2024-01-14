using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Lib.Core;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Knowledge_Annals_Detail : UIBase, UI_Knowledge_Annals_Detail_Left.IListener
    {
        private UI_CurrencyTitle currency;

        private UI_Knowledge_Annals_Detail_Left _left;
        private UI_Knowledge_Annals_Detail_Right _right;

        private uint _eventId;

        protected override void OnLoaded()
        {            
            Button btnClose = transform.Find("Animator/View_Title07/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(()=> { this.CloseSelf(); });

            currency = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            currency.InitUi();

            _left = new UI_Knowledge_Annals_Detail_Left();
            _left.Init(transform.Find("Animator/Scroll View/"));
            _left.Register(this);

            _right = new UI_Knowledge_Annals_Detail_Right();
            _right.Init(transform.Find("Animator/Right"));
        }

        protected override void OnOpen(object arg)
        {            
            _eventId = 0u;
            if (arg != null)
                _eventId = (uint)arg;
        }        

        protected override void OnShow()
        {            
            UpdateInfo();
        }

        protected override void OnDestroy()
        {
            currency?.Dispose();
            _left?.OnDestroy();
        }

        private void UpdateInfo()
        {
            CSVChronology.Data data = CSVChronology.Instance.GetConfData(_eventId);
            if (data != null)
            {
                Sys_Knowledge.Instance.SelectYearId = data.years;
            }

            _left.UpdateInfo();
        }

        public void OnSelectYear(uint yearId)
        {
            _right.UpdateInfo(yearId, _eventId);
            _eventId = 0u;
        }
    }
}


