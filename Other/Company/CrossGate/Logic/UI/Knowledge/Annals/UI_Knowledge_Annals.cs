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
    public class UI_Knowledge_Annals : UIBase
    {
        private class AnnalsTemplate
        {
            private Transform transform;

            private List<UI_Knowledge_Annals_Cell> listCells = new List<UI_Knowledge_Annals_Cell>(2);
        
            public void Init(Transform trans)
            {
                transform = trans;

                int count = transform.childCount;
                for (int i = 0; i < count; ++i)
                {
                    UI_Knowledge_Annals_Cell cell = new UI_Knowledge_Annals_Cell();
                    cell.Init(transform.GetChild(i));

                    listCells.Add(cell);
                }

            }

            public void UpdateInfo(List<uint> listYears)
            {
                for (int i = 0; i < listCells.Count; ++i)
                {
                    if (i < listYears.Count)
                    {
                        listCells[i].OnShow();
                        listCells[i].UpdateInfo(listYears[i]);
                    }
                    else
                    {
                        listCells[i].OnHide();
                    }
                }
            }
        }

        private UI_CurrencyTitle currency;

        //private InfinityGridLayoutGroup gridGroup;
        //private Dictionary<GameObject, UI_Knowledge_Annals_Cell> dicCells = new Dictionary<GameObject, UI_Knowledge_Annals_Cell>();
        //private int visualGridCount;

        private GameObject goTemplateParent;
        private GameObject goTemplate;

        private Text _textCollectNum;
        private Button _btnReward;

        private UI_Knowledge_Annals_Bottom _bottom;

        
        private List<uint> _listYears = new List<uint>();

        private CSVStageReward.Data _stageData;
        private int _totalCount;
        private int _stage;
        private bool _canTakeReward;

        protected override void OnLoaded()
        {            
            Button btnClose = transform.Find("Animator/View_Title07/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(()=> { this.CloseSelf(); });

            currency = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            currency.InitUi();

            goTemplateParent = transform.Find("Animator/Scroll View/Viewport/Content").gameObject;
            goTemplate = transform.Find("Animator/Scroll View/Viewport/Content/Template").gameObject;
            goTemplate.SetActive(false);

            _textCollectNum = transform.Find("Animator/Image_Collect/Text_Num").GetComponent<Text>();
            _btnReward = transform.Find("Animator/Image_Collect/Button").GetComponent<Button>();
            _btnReward.onClick.AddListener(OnClickReward);

            _bottom = new UI_Knowledge_Annals_Bottom();
            _bottom.Init(transform.Find("Animator/ToggleGroup"));
            _bottom.OnHide();
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
            currency?.Dispose();
        }

        private void OnClickReward()
        {
            if (_canTakeReward)
            {
                Sys_Knowledge.Instance.OnTakeAwardReq(Sys_Knowledge.ETypes.Annals);
            }
            else
            {
                UIManager.OpenUI(EUIID.UI_Knowledge_Award, false, Sys_Knowledge.ETypes.Annals);
            }
        }

        private void UpdateInfo()
        {
            _stageData = CSVStageReward.Instance.GetConfData((uint)Sys_Knowledge.ETypes.Annals);
            _totalCount = Sys_Knowledge.Instance.GetEventsCount(Sys_Knowledge.ETypes.Annals);
            UpdateRewardState();

            _listYears.Clear();
            _listYears.AddRange(Sys_Knowledge.Instance.GetTotalAnnals().Keys);

            int tempCount = _listYears.Count;
            tempCount = tempCount % 2 == 0 ? tempCount / 2 : tempCount / 2 + 1;

            FrameworkTool.DestroyChildren(goTemplateParent, goTemplate.name);

            for (int i = 0;  i < tempCount; ++i)
            {
                List<uint> tempYears = new List<uint>();
                for (int j = 0; j < 2; ++j)
                {
                    int index = i * 2 + j;
                    if (index < _listYears.Count)
                        tempYears.Add(_listYears[index]);
                }

                GameObject go = GameObject.Instantiate<GameObject>(goTemplate);
                go.transform.SetParent(goTemplateParent.transform);
                go.SetActive(true);
                //rGo.name = rTemplateGo.name;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = goTemplate.transform.localScale;

                AnnalsTemplate tempClass = new AnnalsTemplate();
                tempClass.Init(go.transform);
                tempClass.UpdateInfo(tempYears);
            }

            FrameworkTool.ForceRebuildLayout(goTemplateParent);
        }

        private void UpdateRewardState()
        {
            int activeCount = Sys_Knowledge.Instance.GetUnlockEventsCount(Sys_Knowledge.ETypes.Annals);
            _textCollectNum.text = string.Format("{0}/{1}", activeCount, _totalCount);

            _stage = Sys_Knowledge.Instance.GetStageReward(Sys_Knowledge.ETypes.Annals);

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


