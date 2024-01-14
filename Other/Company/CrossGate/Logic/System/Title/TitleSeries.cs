using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using Lib.Core;

namespace Logic
{
    //称号系列数据类

    public class TitleSeries
    {
        public uint Id { get; private set; }                                //配置id
        public bool active { get; private set; }                            //该系列是否激活
        public bool IsFirstactive { get; set; }                             //该系列是否是第一次激活
        public CSVTitleSeries.Data cSVTitleSeriesData { get; private set; }  //系列表格数据

        public List<Title> titles = new List<Title>();                      //这个系列拥有的称号列表

        public List<uint> rewardState = new List<uint>();                   //系列奖励状态   1: 可领取, 2: 已领取; 0或空为未解锁不可领取

        public int activeCount
        {
            get
            {
                int count = 0;
                foreach (var item in titles)
                {
                    if (item.active)
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        public int maxActiveCount
        {
            get
            {
                if (cSVTitleSeriesData!=null)
                {
                    //int count = cSVTitleSeriesData.seriesCollect.Count;
                    //return (int)cSVTitleSeriesData.seriesCollect[count - 1][0];
                    return titles.Count;
                }
                else
                {
                    return -1;
                }
            }
        }

        public float progress
        {
            get
            {
                return (float)activeCount / (float)maxActiveCount;
            }
        }

        public bool bNewAwardAvaliable { get; private set; }

        public bool bAllAwardGet
        {
            get
            {
                bool allGet = true;
                foreach (var item in rewardState)
                {
                    if (item != 2)
                        allGet = false;
                }
                return allGet;
            }
        }

        public bool bPerformed { get; set; }


        public TitleSeries(CSVTitleSeries.Data data)//(uint id)
        {
            //Id = id;
            //cSVTitleSeriesData = CSVTitleSeries.Instance.GetConfData(id);

            cSVTitleSeriesData = data;
            Id = cSVTitleSeriesData.id;

            int configCount = cSVTitleSeriesData.seriesCollect.Count;
            for (int i = 0; i < configCount; i++)
            {
                rewardState.Add(0);
            }
        }
        

        public void AddTitle(Title title)
        {
            titles.Add(title);
        }

        public void SetActive(bool _active)
        {
            active = _active;
        }

        public void InitRewardState(List<uint> _rewardStates)
        {
            rewardState.Clear();
            foreach (var item in _rewardStates)
            {
                rewardState.Add(item);
            }
            int curCount = rewardState.Count;
            int configCount = cSVTitleSeriesData.seriesCollect.Count;
            if (curCount< configCount)
            {
                for (int i = 0; i < configCount-curCount; i++)
                {
                    rewardState.Add(0);
                }
            }
            if (rewardState.Contains(1))
            {
                SetRewardAvaliable(true);
            }
        }

        public void UpdateRewardStateAvailbale(int index)
        {
            if (rewardState[index]!=0)
            {
                DebugUtil.LogErrorFormat("{0} 索引的奖励状态不为空");
                return;
            }
            rewardState[index] = 1;
            SetRewardAvaliable(true);
        }

        public void UpdateRewardStateAfterAchieve()
        {
            int configCount = cSVTitleSeriesData.seriesCollect.Count;
            for (int i = 0; i < configCount; i++)
            {
                if (activeCount>= cSVTitleSeriesData.seriesCollect[i][0])
                {
                    rewardState[i] = 2;
                }
            }
        }


        public void UpdateActiveState(bool calFirst=false)
        {
            foreach (var item in titles)
            {
                if (!item.active)
                {
                    active = false;
                    return;
                }
            }
            active = true;
            if (calFirst)
                IsFirstactive = true;
        }

        public void SetRewardAvaliable(bool flag)
        {
            bNewAwardAvaliable = flag;
            Sys_Title.Instance.eventEmitter.Trigger<uint>(Sys_Title.EEvents.OnNewRewardAvaliable, Id);
        }

        
        public void SortTitleList()
        {
            List<Title> temp = new List<Title>(titles);
            titles.Clear();
            for (int i = temp.Count - 1; i >= 0; --i)
            {
                if (temp[i].active)
                {
                    titles.Add(temp[i]);
                    temp.RemoveAt(i);
                }
            }
            titles.Sort((x, y) => x.cSVTitleData.titleOrder.CompareTo(y.cSVTitleData.titleOrder));
            temp.Sort((x, y) => x.cSVTitleData.titleOrder.CompareTo(y.cSVTitleData.titleOrder));
            titles.AddRange(temp);
        }
    }
}


