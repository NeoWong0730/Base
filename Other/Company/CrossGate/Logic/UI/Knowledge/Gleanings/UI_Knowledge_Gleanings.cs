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
    public class UI_Knowledge_Gleanings : UIBase, UI_Knowledge_Gleanings_Left.IListener
    {
        private UI_Knowledge_Gleanings_Left _left;
        private UI_Knowledge_Gleanings_Items _items;
        private UI_Knowledge_Gleanings_Geo _geo;

        private Text _textCollectNum;
        private Button _btnReward;

        private CSVStageReward.Data _stageData;
        private int _totalCount;
        private int _stage;
        private bool _canTakeReward;

        protected override void OnLoaded()
        {            
            Button btnClose = transform.Find("Animator/View_Title02/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(()=> { this.CloseSelf(); });

            _left = new UI_Knowledge_Gleanings_Left();
            _left.Init(transform.Find("Animator/Panel_detail/Image_LeftBG/Image_Bg3"));
            _left.Register(this);

            _items = new UI_Knowledge_Gleanings_Items();
            _items.Init(transform.Find("Animator/Panel_detail/Image_RightBG/Image_BG/Item"));

            _geo = new UI_Knowledge_Gleanings_Geo();
            _geo.Init(transform.Find("Animator/Panel_detail/Image_RightBG/Image_BG/Geo"));

            _textCollectNum = transform.Find("Animator/Image_Collect/Text_Num").GetComponent<Text>();
            _btnReward = transform.Find("Animator/Image_Collect/Button").GetComponent<Button>();
            _btnReward.onClick.AddListener(OnClickReward);
        }        

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Knowledge.Instance.eventEmitter.Handle(Sys_Knowledge.EEvents.OnTakeRewardNtf, OnTakeRewardNtf, toRegister);
        }

        protected override void OnShow()
        {         
            UpdateInfo();
        }        

        protected override void OnDestroy()
        {
            _left.OnDestroy();
        }

        private void OnClickReward()
        {
            if (_canTakeReward)
            {
                Sys_Knowledge.Instance.OnTakeAwardReq(Sys_Knowledge.ETypes.Gleanings);
            }
            else
            {
                UIManager.OpenUI(EUIID.UI_Knowledge_Award, false, Sys_Knowledge.ETypes.Gleanings);
            }
        }

        private void UpdateInfo()
        {
            _stageData = CSVStageReward.Instance.GetConfData((uint)Sys_Knowledge.ETypes.Gleanings);
            _totalCount = Sys_Knowledge.Instance.GetEventsCount(Sys_Knowledge.ETypes.Gleanings);
            UpdateRewardState();

            _left.OnSelect(2u);
        }

        private void UpdateRewardState()
        {
            int activeCount = Sys_Knowledge.Instance.GetUnlockEventsCount(Sys_Knowledge.ETypes.Gleanings);
            _textCollectNum.text = string.Format("{0}/{1}", activeCount, _totalCount);

            _stage = Sys_Knowledge.Instance.GetStageReward(Sys_Knowledge.ETypes.Gleanings);

            int needCount = int.MaxValue;
            if (_stageData.stage != null && _stage < _stageData.stage.Count)
                needCount = (int)_stageData.stage[_stage];

            _canTakeReward = activeCount >= needCount;

            ImageHelper.SetImageGray(_btnReward.image, !_canTakeReward, true);
        }

        private void OnTakeRewardNtf()
        {
            UpdateRewardState();
        }

        public void OnSelect(uint typeId, uint subTypeId)
        {
            _geo.OnHide();
            _items.OnHide();

            switch (typeId)
            {
                case 1:
                case 3:
                    _geo.OnShow();
                    _geo.UpdateInfo(subTypeId);
                    break;
                case 2:
                    _items.OnShow();
                    _items.UpdateInfo(subTypeId);
                    break;
            }
        }
    }
}


