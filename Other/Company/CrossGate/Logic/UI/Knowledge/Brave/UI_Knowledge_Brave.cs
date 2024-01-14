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
    public class UI_Knowledge_Brave : UIBase
    {
        private class BraveHero
        {
            private Transform transform;

            private Button _btnHead;
            private Image _imgHead;
            private Transform _transRed;

            private uint _braveId;

            public void Init(Transform trans)
            {
                transform = trans;

                _btnHead = transform.Find("Image_Head").GetComponent<Button>();
                _btnHead.onClick.AddListener(OnClick);

                _imgHead = transform.Find("Image_Head").GetComponent<Image>();
                _transRed = transform.Find("Image_Red");
            }

            private void OnClick()
            {
                UIManager.OpenUI(EUIID.UI_Knowledge_Brave_Detail, false, _braveId);
            }

            public void UpdateInfo(uint braveId)
            {
                _braveId = braveId;

                CSVBrave.Data data = CSVBrave.Instance.GetConfData(_braveId);
                if (data != null)
                {
                    ImageHelper.SetIcon(_imgHead, data.icon);
                }

                bool isRed = Sys_Knowledge.Instance.IsBraveRedPoint(_braveId);
                if (_transRed != null)
                    _transRed.gameObject.SetActive(isRed);
            }
        }

        private List<BraveHero> listHeros = new List<BraveHero>();

        private Text _textCollectNum;
        private Button _btnReward;

        private CSVStageReward.Data _stageData;
        private int _totalCount;
        private int _stage;
        private bool _canTakeReward;

        protected override void OnLoaded()
        {            
            Button btnClose = transform.Find("Animator/View_Title07/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(()=> { this.CloseSelf(); });

            for (int i = 0; i < 6; ++i)
            {
                Transform temp = transform.Find(string.Format("Animator/Image_BG/Brave/Brave{0}", i));

                BraveHero hero = new BraveHero();
                hero.Init(temp);

                listHeros.Add(hero);
            }

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

        private void OnClickReward()
        {
            if (_canTakeReward)
            {
                Sys_Knowledge.Instance.OnTakeAwardReq(Sys_Knowledge.ETypes.Brave);
            }
            else
            {
                UIManager.OpenUI(EUIID.UI_Knowledge_Award, false, Sys_Knowledge.ETypes.Brave);
            }
        }

        private void UpdateInfo()
        {
            _stageData = CSVStageReward.Instance.GetConfData((uint)Sys_Knowledge.ETypes.Brave);
            _totalCount = Sys_Knowledge.Instance.GetEventsCount(Sys_Knowledge.ETypes.Brave);
            UpdateRewardState();

            for (int i = 0; i < listHeros.Count; ++i)
            {
                uint braveId = (uint)(611 + i) * 10;
                CSVBrave.Data data = CSVBrave.Instance.GetConfData(braveId);
                if (data != null)
                {
                    listHeros[i].UpdateInfo(data.id);
                }
                else
                {
                    DebugUtil.LogErrorFormat("CSVBraveBiography id={0} 找不到", braveId);
                }
            }
        }

        private void UpdateRewardState()
        {
            int activeCount = Sys_Knowledge.Instance.GetUnlockEventsCount(Sys_Knowledge.ETypes.Brave);
            _textCollectNum.text = string.Format("{0}/{1}", activeCount, _totalCount);

            _stage = Sys_Knowledge.Instance.GetStageReward(Sys_Knowledge.ETypes.Brave);

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
    }
}


