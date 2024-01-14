using Logic.Core;
using Packet;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    public class CheckHeroVisualSystem : LevelSystemBase
    {
        //float lastTime;
        //float cd = 1;
        //public bool Enable = true;

        private bool hasHide = true;

        List<Hero> heroSortLists = new List<Hero>();
        HashSet<ulong> teamHash = new HashSet<ulong>();
        Vector3 mainHeroPosition;

        public override void OnCreate()
        {
            intervalFrame = 120; //一秒120帧
        }

        public override void OnUpdate()
        {         
            //if (!Enable)
            //    return;
            //
            //if (Time.unscaledTime < lastTime)
            //    return;
            //
            //lastTime = Time.unscaledTime + cd;

            if (OptionManager.Instance.DisplayRoleCount >= GameCenter.otherActorList.Count)
            {
                if (hasHide)
                {
                    hasHide = false;
                    for (int i = 0; i < GameCenter.otherActorList.Count; ++i)
                    {
                        GameCenter.otherActorList[i].Show();
                    }
                }
            }
            else
            {
                hasHide = true;
                CheckHeroVisual(OptionManager.Instance.DisplayRoleCount);
            }
        }

        void CheckHeroVisual(int displayRoleCount)
        {
            if (GameCenter.mainHero == null)
                return;

            if (GameCenter.mainHero.transform == null)
                return;

            mainHeroPosition = GameCenter.mainHero.transform.position;

            teamHash.Clear();
            heroSortLists.Clear();

            List<Hero> otherActorList = GameCenter.otherActorList;
            Dictionary<ulong, Hero> otherActorDic = GameCenter.otherActorsDic;

            //算上主角，先减去主角相关的数量
            int teamCount = Sys_Team.Instance.TeamMemsCount;
            ulong mainHeroTeamID = Sys_Team.Instance.teamID;
            if (teamCount > 0)
            {
                displayRoleCount -= teamCount;
                teamHash.Add(mainHeroTeamID);
            }
            else
            {
                --displayRoleCount;
            }

            if (displayRoleCount <= 0)
            {
                //当已经达到显示上限时 只显示主角以及主角队伍                
                for (int i = 0, len = otherActorList.Count; i < len; ++i)
                {
                    Hero hero = otherActorList[i];
                    HeroBaseComponent heroBaseComponent = hero.heroBaseComponent;

                    //当主角没有队伍 则全部影藏
                    if (teamHash.Contains(heroBaseComponent.TeamID))
                    {
                        hero.Show();
                    }
                    else
                    {
                        hero.Hide();
                    }
                }
            }
            else
            {
                for (int i = 0, len = otherActorList.Count; i < len; ++i)
                {
                    Hero hero = otherActorList[i];
                    HeroBaseComponent heroBaseComponent = hero.heroBaseComponent;

                    //只将独立的角色和队长进行排序 且 不包含自己以及自己队伍
                    if (heroBaseComponent.TeamID == 0
                        || (heroBaseComponent.TeamID != mainHeroTeamID && heroBaseComponent.IsCaptain))
                    {
                        heroSortLists.Add(hero);
                    }
                }

                //距离排序
                heroSortLists.Sort(_HeroSortFunc);

                //根据距离排序筛选
                for (int index = 0, len = heroSortLists.Count; index < len; index++)
                {
                    Hero hero = heroSortLists[index];
                    if (displayRoleCount > 0)
                    {
                        if (!hero.heroBaseComponent.IsCaptain)
                        {
                            //只先处理独立角色
                            hero.Show();
                            --displayRoleCount;
                        }
                        else
                        {
                            //带组队的后面一并处理
                            displayRoleCount -= (int)hero.heroBaseComponent.TeamMemNum;
                            teamHash.Add(hero.heroBaseComponent.TeamID);
                        }
                    }
                    else
                    {
                        if (!hero.heroBaseComponent.IsCaptain)
                        {
                            //只先处理独立角色
                            hero.Hide();
                        }
                    }
                }

                //处理所有有组队的
                for (int i = 0, len = otherActorList.Count; i < len; ++i)
                {
                    Hero hero = otherActorList[i];
                    HeroBaseComponent heroBaseComponent = hero.heroBaseComponent;

                    if (heroBaseComponent.TeamID != 0)
                    {
                        if (teamHash.Contains(heroBaseComponent.TeamID))
                        {
                            hero.Show();
                        }
                        else
                        {
                            hero.Hide();
                        }
                    }
                }
            }

            teamHash.Clear();
            heroSortLists.Clear();
        }

        private int _HeroSortFunc(Hero herA, Hero heroB)
        {
            float distanceA = Vector3.SqrMagnitude(mainHeroPosition - heroB.transform.position);
            float distanceB = Vector3.SqrMagnitude(mainHeroPosition - heroB.transform.position);

            if (distanceA > distanceB)
                return 1;
            else if (distanceA < distanceB)
                return -1;
            else
                return 0;
        }
    }
}