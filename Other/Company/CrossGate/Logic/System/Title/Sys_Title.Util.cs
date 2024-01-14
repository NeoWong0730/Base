using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using Table;
using Net;
using Packet;
using Lib.Core;
using UnityEngine;
using System;

namespace Logic
{
    public partial class Sys_Title : SystemModuleBase<Sys_Title>, ISystemModuleUpdate
    {
        /// <summary>
        /// 登陆或者收到称号激活通知的时候都需要重新排序一下
        /// </summary>
        /// <param name="type"></param>
        private void Sort(uint type)
        {
            List<Title> temp;
            if (type == 1)
            {
                temp = new List<Title>(AchievementTitles);
                AchievementTitles.Clear();
                for (int i = temp.Count - 1; i >= 0; --i)
                {
                    if (temp[i].active)
                    {
                        AchievementTitles.Add(temp[i]);
                        temp.RemoveAt(i);
                    }
                }
                AchievementTitles.Sort((x, y) => x.cSVTitleData.titleOrder.CompareTo(y.cSVTitleData.titleOrder));
                temp.Sort((x, y) => x.cSVTitleData.titleOrder.CompareTo(y.cSVTitleData.titleOrder));
                AchievementTitles.AddRange(temp);
            }
            else if (type == 2)
            {
                temp = new List<Title>(Prestigetitles);
                Prestigetitles.Clear();
                for (int i = temp.Count - 1; i >= 0; --i)
                {
                    if (temp[i].active)
                    {
                        Prestigetitles.Add(temp[i]);
                        temp.RemoveAt(i);
                    }
                }
                Prestigetitles.Sort((x, y) => x.cSVTitleData.titleOrder.CompareTo(y.cSVTitleData.titleOrder));
                temp.Sort((x, y) => x.cSVTitleData.titleOrder.CompareTo(y.cSVTitleData.titleOrder));
                Prestigetitles.AddRange(temp);
            }
            else if (type == 3)
            {
                temp = new List<Title>(CareerTitles);
                CareerTitles.Clear();
                for (int i = temp.Count - 1; i >= 0; --i)
                {
                    if (temp[i].active)
                    {
                        CareerTitles.Add(temp[i]);
                        temp.RemoveAt(i);
                    }
                }
                CareerTitles.Sort((x, y) => x.cSVTitleData.titleOrder.CompareTo(y.cSVTitleData.titleOrder));
                temp.Sort((x, y) => x.cSVTitleData.titleOrder.CompareTo(y.cSVTitleData.titleOrder));
                CareerTitles.AddRange(temp);
            }
            else if (type == 4)
            {
                temp = new List<Title>(Specialtitles);
                Specialtitles.Clear();
                for (int i = temp.Count - 1; i >= 0; --i)
                {
                    if (temp[i].active)
                    {
                        Specialtitles.Add(temp[i]);
                        temp.RemoveAt(i);
                    }
                }
                Specialtitles.Sort((x, y) => x.cSVTitleData.titleOrder.CompareTo(y.cSVTitleData.titleOrder));
                temp.Sort((x, y) => x.cSVTitleData.titleOrder.CompareTo(y.cSVTitleData.titleOrder));
                Specialtitles.AddRange(temp);
            }
        }

        public void SortTitleSeries()
        {
            List<TitleSeries> temp = new List<TitleSeries>(titleSeries);
            List<TitleSeries> tempFirst = new List<TitleSeries>();
            List<TitleSeries> actives = new List<TitleSeries>();
            titleSeries.Clear();

            for (int i = temp.Count - 1; i >= 0; --i)
            {
                if (temp[i].IsFirstactive)
                {
                    tempFirst.Add(temp[i]);
                    temp.RemoveAt(i);
                }
            }

            for (int i = temp.Count - 1; i >= 0; --i)
            {
                if (temp[i].active)
                {
                    actives.Add(temp[i]);
                    temp.RemoveAt(i);
                }
            }

            tempFirst.Sort((x, y) => x.cSVTitleSeriesData.id.CompareTo(y.cSVTitleSeriesData.id));
            actives.Sort((x, y) => x.cSVTitleSeriesData.id.CompareTo(y.cSVTitleSeriesData.id));
            temp.Sort((x, y) => x.cSVTitleSeriesData.id.CompareTo(y.cSVTitleSeriesData.id));

            titleSeries.AddRange(tempFirst);
            titleSeries.AddRange(actives);
            titleSeries.AddRange(temp);
        }

        public Title GetTitleData(uint titleId)
        {
            CSVTitle.Data cSVTitleData = CSVTitle.Instance.GetConfData(titleId);
            if (cSVTitleData == null)
            {
                DebugUtil.LogErrorFormat("找不到id={0}的称号", titleId);
                return null;
            }
            TitleType titleType = (TitleType)cSVTitleData.titleShowClass;
            Title title = null;
            switch (titleType)
            {
                case TitleType.Prestige:
                    title = AchievementTitles.Find(x => x.Id == titleId);
                    break;
                case TitleType.Task:
                    title = Prestigetitles.Find(x => x.Id == titleId);
                    break;
                case TitleType.Achievement:
                    title = CareerTitles.Find(x => x.Id == titleId);
                    break;
                case TitleType.Special:
                    title = Specialtitles.Find(x => x.Id == titleId);
                    break;
                default:
                    break;
            }
            return title;
        }

        public string GetTitleConfigName(uint titleId)
        {
            CSVTitle.Data cSVTitleData = CSVTitle.Instance.GetConfData(titleId);
            if (cSVTitleData == null)
            {
                DebugUtil.LogErrorFormat(" GetTitleConfigName ------ 找不到id={0}的称号", titleId);
                return string.Empty;
            }
            string titleName = LanguageHelper.GetTextContent(cSVTitleData.titleLan);
            return titleName;
        }

        public void FliterCareerData()
        {
            var dataList = CSVTitle.Instance.GetAll();
            for (int i = 0, len = dataList.Count; i < len; i++)
            {
                CSVTitle.Data item = dataList[i];
                if (item.titleTypeNum > 1)
                {
                    continue;
                }
                if (Contains(item.id) || Sys_Role.Instance.Role.Career != item.professionId)
                {
                    continue;
                }
                Title title = new Title(item.id, (TitleType)item.titleShowClass);
                titles.Add(title);
                if (item.titleShowClass == 1)
                {
                    AchievementTitles.Add(title);
                }
                else if (item.titleShowClass == 2)
                {
                    Prestigetitles.Add(title);
                }
                else if (item.titleShowClass == 3)
                {
                    CareerTitles.Add(title);
                }
                else if (item.titleShowClass == 4)
                {
                    Specialtitles.Add(title);
                }

                if (title.cSVTitleData.titleSeries != null)
                {
                    foreach (var kv in title.cSVTitleData.titleSeries)
                    {
                        TitleSeries titleSerie = titleSeries.Find(x => x.Id == kv);
                        if (titleSerie != null)
                        {
                            titleSerie.AddTitle(title);
                        }
                        else
                        {
                            DebugUtil.LogErrorFormat("没有找到id={0}的系列属性", kv);
                        }
                    }
                }
            }
        }

        public bool TryActiveTitle(Title toAddTitle)
        {
            bool can = false;
            bool bFindSameType = false;
            bool replaceDress = false;

            int newTitleType = toAddTitle.cSVTitleData.titleType;
            int newTitleNum = toAddTitle.cSVTitleData.titleTypeNum;

            for (int i = titles.Count - 1; i >= 0; i--)
            {
                Title oldTitle = titles[i];
                int oldTitleType = oldTitle.cSVTitleData.titleType;
                int oldTitleNum = oldTitle.cSVTitleData.titleTypeNum;
                if (oldTitleType == newTitleType)
                {
                    bFindSameType = true;
                    if (oldTitleNum > newTitleNum)//新解锁的等级更低
                    {
                        if (!oldTitle.active)
                        {
                            can = true;
                        }
                        else
                        {
                            DebugUtil.Log(ELogType.eTitle, string.Format(" 激活新称号{0}失败 类型{1} 等级{2}---->老称号{3} 类型{4} 等级{5}",
                          toAddTitle.Id, newTitleType, newTitleNum, oldTitle.Id, oldTitleType, oldTitleNum));
                            return false;
                        }
                    }
                    else if (oldTitleNum < newTitleNum)// 新解锁的等级更高,则解锁成功,需要把老的数据从客户端移除
                    {
                        can = true;
                        RemoveTitle(oldTitle, out replaceDress);

                        DebugUtil.Log(ELogType.eTitle, string.Format(" 激活新称号{0}成功 类型{1} 等级{2}---->移除老称号{3} 类型{4} 等级{5}",
                           toAddTitle.Id, newTitleType, newTitleNum, oldTitle.Id, oldTitleType, oldTitleNum));
                    }
                    else            //相等 
                    {
                        can = true;
                        DebugUtil.Log(ELogType.eTitle, string.Format(" 激活称号{0}成功 类型{1} 等级{2}",
                         toAddTitle.Id, newTitleType, newTitleNum));
                        break;
                    }
                }
            }

            //如果没有找到相同类型的称号,就直接添加
            if (!bFindSameType)
            {
                DebugUtil.Log(ELogType.eTitle, string.Format("没有找到相同类型的称号,激活称号{0}", toAddTitle.Id));
                TryAddTitle(toAddTitle);
                return true;
            }
            //找到了
            if (can)
            {
                TryAddTitle(toAddTitle);
                if (replaceDress)
                {
                    TitleShowReq(toAddTitle.Id);
                    DebugUtil.Log(ELogType.eTitle, string.Format("显示新称号{0}", toAddTitle.Id));
                }
            }
            return can;
        }

        private void TryAddTitle(Title title)
        {
            for (int i = 0; i < titles.Count; i++)
            {
                if (titles[i].Id == title.Id)
                {
                    return;
                }
            }
            titles.Add(title);
            if (title.cSVTitleData.titleShowClass == 1)
            {
                AchievementTitles.Add(title);
            }
            else if (title.cSVTitleData.titleShowClass == 2)
            {
                Prestigetitles.Add(title);
            }
            else if (title.cSVTitleData.titleShowClass == 3)
            {
                CareerTitles.Add(title);
            }
            else if (title.cSVTitleData.titleShowClass == 4)
            {
                Specialtitles.Add(title);
            }

            if (title.cSVTitleData.titleSeries != null)
            {
                foreach (var kv in title.cSVTitleData.titleSeries)
                {
                    TitleSeries titleSerie = titleSeries.Find(x => x.Id == kv);
                    if (titleSerie != null)
                    {
                        titleSerie.AddTitle(title);
                    }
                    else
                    {
                        DebugUtil.LogErrorFormat("没有找到id={0}的系列属性", kv);
                    }
                }
            }
        }

        public bool Red()
        {
            return unReadTitles.Count > 0;
        }


        public void RemoveTitle(Title lowTitle, out bool replaceDress)
        {
            replaceDress = false;
            if (curShowTitle == lowTitle.Id && GameCenter.mainHero != null)
            {
                curShowTitle = 0;
                ClearTitleEvt clearTitleEvt = new ClearTitleEvt();
                clearTitleEvt.actorId = GameCenter.mainHero.uID;
                Sys_HUD.Instance.eventEmitter.Trigger<ClearTitleEvt>(Sys_HUD.EEvents.OnClearTitle, clearTitleEvt);
                replaceDress = true;
                DebugUtil.Log(ELogType.eTitle, string.Format("移除当前显示称号{0}", lowTitle.Id));
            }

            titles.Remove(lowTitle);
            if (AchievementTitles.Remove(lowTitle))
            {
                return;
            }
            if (Prestigetitles.Remove(lowTitle))
            {
                return;
            }
            if (CareerTitles.Remove(lowTitle))
            {
                return;
            }
            if (Specialtitles.Remove(lowTitle))
            {
                return;
            }
        }

        public int GetActiveTitleSeriesCount()
        {
            int count = 0;
            foreach (var item in titleSeries)
            {
                if (item.active)
                {
                    count++;
                }
            }
            return count;
        }

        public List<uint> GetDressedTitlePos()
        {
            List<uint> temp = new List<uint>(titlePos);
            for (int i = temp.Count - 1; i >= 0; --i)
            {
                if (temp[i] == 0)
                {
                    temp.RemoveAt(i);
                }
            }
            return temp;
        }

        public PrestigeFunction.PrestigeFunctionData GetPrestigeFunctionData()
        {
            PrestigeFunction.PrestigeFunctionData prestigeFunctionData = new PrestigeFunction.PrestigeFunctionData();

            uint curID = 0;
            uint nextID = 0;

            for (int i = 0; i < Prestigetitles.Count; i++)
            {
                if (Prestigetitles[i].active && Prestigetitles[i].cSVTitleData.titleType == 1)
                {
                    curID = Prestigetitles[i].Id;
                    break;
                }
            }
            if (curID == 0)
            {
                nextID = s_FirstPrestigeId;
            }
            else
            {
                nextID = curID + 1;
            }
            CSVTitle.Data cSVTitleData = CSVTitle.Instance.GetConfData(nextID);
            if (cSVTitleData == null)
            {
                return default(PrestigeFunction.PrestigeFunctionData);
            }
            else
            {
                if (cSVTitleData.titleShowClass == 2 && cSVTitleData.titleType == 1)
                {
                    prestigeFunctionData.nextPrestigeID = nextID;
                    prestigeFunctionData.getFlag = Sys_Reputation.Instance.reputationLevel >= cSVTitleData.titleGet[0];
                    prestigeFunctionData.name = 2020749;

                    prestigeFunctionData.successDialogueID = cSVTitleData.titleGet[2];
                    prestigeFunctionData.failDialogueID = cSVTitleData.titleGet[3];

                    return prestigeFunctionData;
                }
                else
                {
                    return default(PrestigeFunction.PrestigeFunctionData);
                }
            }
        }

        public bool IsReachMaxPrestigeLevel()
        {
            Title curActivePrestigeTitle = null;
            for (int i = 0; i < Prestigetitles.Count; i++)
            {
                if (Prestigetitles[i].active && Prestigetitles[i].cSVTitleData.titleType == 1)
                {
                    curActivePrestigeTitle = Prestigetitles[i];
                }
            }
            if (curActivePrestigeTitle == null)
            {
                return false;
            }
            return curActivePrestigeTitle.cSVTitleData.titleTypeNum == m_MaxPrestigeCount;
        }

        /// <summary>
        /// 此接口设计只针对声望系统
        /// </summary>
        /// <param name="titleId"></param>
        /// <returns></returns>
        public bool TitleGet(uint titleId)
        {
            Title curActivePrestigeTitle = null;
            for (int i = 0; i < Prestigetitles.Count; i++)
            {
                if (Prestigetitles[i].active && Prestigetitles[i].cSVTitleData.titleType == 1)
                {
                    curActivePrestigeTitle = Prestigetitles[i];
                }
            }
            if (curActivePrestigeTitle != null)
            {
                if (titleId <= curActivePrestigeTitle.Id)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public List<Title> GetActiveTitles()
        {
            List<Title> _titles = new List<Title>();
            for (int i = 0; i < titles.Count; i++)
            {
                if (titles[i].active && titles[i].Id != Sys_Title.Instance.familyTitle)
                {
                    _titles.Add(titles[i]);
                }
            }
            return _titles;
        }

        public string GetTitleFamiltyName()
        {
            string str = Sys_Family.Instance.GetFamilyName();

            if (Sys_Family.Instance.familyData.CheckMe() != null)
            {
                uint positionId = Sys_Family.Instance.familyData.CheckMe().Position;
                
                positionId = positionId % 10000;

                CSVFamilyPostAuthority.Data cSVFamilyPostAuthorityData = CSVFamilyPostAuthority.Instance.GetConfData(positionId);

                if (cSVFamilyPostAuthorityData != null)
                {
                    string postName = CSVLanguage.Instance.GetConfData(cSVFamilyPostAuthorityData.PostName).words;

                    str = LanguageHelper.GetTextContent(2005800, Sys_Family.Instance.GetFamilyName(), postName);
                }

            }
            return str;
        }

        public string GetTitleWarriorGroupName()
        {
            return Sys_WarriorGroup.Instance.GetTitle();
        }

        public string GetTitleWarriorGroupName(string name, uint pos)
        {
            string titleStr = name;
            titleStr += "-";

            if (pos == 1) 
            {
                titleStr += LanguageHelper.GetTextContent(1002820);
            }
            else
            {
                titleStr += LanguageHelper.GetTextContent(1002819);
            }

            return titleStr;
        }

        public bool Contains(uint titleId)
        {
            for (int i = 0; i < titles.Count; i++)
            {
                if (titles[i].Id == titleId)
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasTitle(uint id)
        {
            for (int i = 0; i < titles.Count; i++)
            {
                if (titles[i].Id == id)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

