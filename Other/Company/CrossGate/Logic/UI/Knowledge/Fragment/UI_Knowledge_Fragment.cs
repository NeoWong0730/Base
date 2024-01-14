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
    public class UI_Knowledge_Fragment : UIBase
    {
        private InfinityGridLayoutGroup gridGroup;
        private Dictionary<GameObject, UI_Knowledge_Fragment_Cell> dicCells = new Dictionary<GameObject, UI_Knowledge_Fragment_Cell>();
        private int visualGridCount;

        private int CELL_COUNT = 5;
        private List<List<uint>> listFragments = new List<List<uint>>();

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

            gridGroup = transform.Find("Animator/Scroll View/Viewport/Content").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            gridGroup.minAmount = 3;
            gridGroup.updateChildrenCallback = UpdateChildrenCallback;

            for (int i = 0; i < gridGroup.transform.childCount; ++i)
            {
                Transform tran = gridGroup.transform.GetChild(i);

                UI_Knowledge_Fragment_Cell cell = new UI_Knowledge_Fragment_Cell();
                cell.Init(tran);
                dicCells.Add(tran.gameObject, cell);
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

        protected override void OnDestroy()
        {
            foreach (var data in dicCells)
                data.Value.OnDestroy();
        }

        private void OnClickReward()
        {
            if (_canTakeReward)
            {
                Sys_Knowledge.Instance.OnTakeAwardReq(Sys_Knowledge.ETypes.Fragment);
            }
            else
            {
                UIManager.OpenUI(EUIID.UI_Knowledge_Award, false, Sys_Knowledge.ETypes.Fragment);
            }
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= visualGridCount)
                return;

            if (dicCells.ContainsKey(trans.gameObject))
            {
                UI_Knowledge_Fragment_Cell cell = dicCells[trans.gameObject];
                cell.UpdateInfo(listFragments[index]);
            }
        }

        private void UpdateInfo()
        {
            _stageData = CSVStageReward.Instance.GetConfData((uint)Sys_Knowledge.ETypes.Fragment);
            _totalCount = Sys_Knowledge.Instance.GetEventsCount(Sys_Knowledge.ETypes.Fragment);
            UpdateRewardState();

            CalCellData();
            visualGridCount = listFragments.Count;
            gridGroup.SetAmount(visualGridCount);
        }

        private void UpdateRewardState()
        {
            int activeCount = Sys_Knowledge.Instance.GetUnlockEventsCount(Sys_Knowledge.ETypes.Fragment);
            _textCollectNum.text = string.Format("{0}/{1}", activeCount, _totalCount);

            _stage = Sys_Knowledge.Instance.GetStageReward(Sys_Knowledge.ETypes.Fragment);

            int needCount = int.MaxValue;
            if (_stageData.stage != null && _stage < _stageData.stage.Count)
                needCount = (int)_stageData.stage[_stage];

            _canTakeReward = activeCount >= needCount;

            ImageHelper.SetImageGray(_btnReward.image, !_canTakeReward, true);
        }

        private void CalCellData()
        {
            List<uint> temp = new List<uint>(Sys_Knowledge.Instance.GetTotalFragments().Keys);

            int count = temp.Count / CELL_COUNT;
            if (temp.Count % CELL_COUNT != 0)
                count++;

            listFragments.Clear();
            for (int i = 0; i < count; ++i)
            {
                List<uint> list = new List<uint>(CELL_COUNT);

                int start = i * CELL_COUNT;
                int end = start + CELL_COUNT;
                for (int j = start; j < end; ++j)
                {
                    if (j < temp.Count)
                    {
                        list.Add(temp[j]);
                    }
                }

                listFragments.Add(list);
            }
        }

        private void OnTakeRewardNtf()
        {
            UpdateRewardState();
        }
    }
}


