using Logic.Core;
using System.Collections.Generic;
using UnityEngine;
using Lib.Core;
using Table;

namespace Logic
{
    public class RewardCircle :UIComponent
    {
        public Transform lightTrans;
        public Transform[] rewardTransArray;
        private Timer timer;

        public bool isPlaying;
        private float rewardTime ;
        private int rewardIndex;
        private int curIndex;
        private int totalCount;
        private uint signId;
        private int middleTime;
        private int starSlowIndex;
        private bool hasAdd;
        private bool hasSlow;

        public List<UI_AwardItem> items = new List<UI_AwardItem>();

        protected override void Loaded()
        {
            base.Loaded();
            isPlaying = false;
        }

        public void StartCircle(uint rewardSignId)
        {
            signId = rewardSignId;
            if (items.Count == 1)
            {
                GetReward();
                return;
            }
            isPlaying = true;
            hasSlow = false;
            hasAdd = false;
            rewardTime = 0.5f;
            middleTime = 0;
            timer = Timer.Register(rewardTime, OnComplete);
            if(curIndex+1> rewardTransArray.Length)
            {
                curIndex = 0;
            }
            rewardIndex = GetShowCount(signId);
            starSlowIndex = GetStarSlowIndex();

            lightTrans.position = rewardTransArray[curIndex].position;
        }

        public void SetAwardLightTrans(int index)
        {
            lightTrans.position = rewardTransArray[index].position;
            curIndex = index;
        }

        private void OnComplete()
        {
            curIndex++;
            if (curIndex >= rewardTransArray.Length)
            {
                curIndex = 0;
            }
            lightTrans.position = rewardTransArray[curIndex].position;
            if (rewardIndex == curIndex&& hasSlow)
            {
                GetReward();
                return;
            }
            if (middleTime >= rewardTransArray.Length * 2 && starSlowIndex == curIndex)
            {
                hasSlow = true;
            }

            if (rewardTime > 0.1f&&!hasAdd)
            {
                rewardTime -= 0.1f;
                timer?.Cancel();
                timer = Timer.Register(rewardTime, OnComplete);
            }
            else 
            {
                if (hasSlow)
                {
                    rewardTime += 0.15f;
                    timer?.Cancel();
                    timer = Timer.Register(rewardTime, OnComplete);
                }
                else
                {
                    hasAdd = true;
                    middleTime++;
                    rewardTime = 0.01f;
                    timer?.Cancel();
                    timer = Timer.Register(rewardTime, OnComplete);
                }
            }
        }

        private void GetReward()
        {
            isPlaying = false;
            hasSlow = false;
            hasAdd = false;
            for (int i = 0; i < items.Count; ++i)
            {
                if (Sys_Sign.Instance.awardTakeList.Contains(items[i].signId))
                {
                    items[i].SetTakeData();
                }
            }
            PushAwardRecord();
        }

        private int GetShowCount(uint signId)
        {
            for(int i=0;i< items.Count; ++i)
            {
                if (items[i].signId == signId)
                {
                    return i;
                }
            }
            return 0;
        }

        private int GetStarSlowIndex()
        {
            if (rewardIndex - 5 >= 0)
            {
                return rewardIndex - 5;
            }
            else
            {
                int count = items.Count - (items.Count % (5 - rewardIndex)) - 1;
                if (count < 0)
                {
                    count = items.Count - rewardIndex-1;
                }
                return count;                      
            }
        }

        private void PushAwardRecord()
        {
            uint rewardItemId = CSVSign.Instance.GetConfData(signId).Reward;
            uint count = CSVSign.Instance.GetConfData(signId).Count;
            string str = LanguageHelper.GetTextContent(CSVLanguage.Instance.GetConfData(5223), LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(rewardItemId).name_id), count.ToString());
            Sys_Hint.Instance.PushContent_Normal(str);
            Sys_Sign.Instance.eventEmitter.Trigger(Sys_Sign.EEvents.CompletePlayingCircle);
            signId = 0;
        }

        public void OnHide()
        {
            timer?.Cancel();
            isPlaying = false;
            hasSlow = false;
            hasAdd = false;
            middleTime = 0;
            if (signId != 0)
            {
                PushAwardRecord();
            }
        }
    }
}