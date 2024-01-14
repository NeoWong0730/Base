using Logic.Core;
using Packet;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// 检测玩家显隐///
    /// </summary>
    public class CheckHeroVisualComponent : Logic.Core.Component//, IUpdateCmd
    {
        /*
        float lastTime;
        float cd;
        Dictionary<ulong, Actor> herosDics;
        List<Hero> heroLists;
        Dictionary<ulong, Hero> showHeros;

        public static bool Enable
        {
            get;
            set;
        } = true;

        protected override void OnConstruct()
        {
            herosDics = new Dictionary<ulong, Actor>();
            heroLists = new List<Hero>();
            showHeros = new Dictionary<ulong, Hero>();
            lastTime = Time.unscaledTime;
            cd = 1;
        }

        protected override void OnDispose()
        {
            herosDics = null;
            heroLists = null;
            showHeros = null;
            lastTime = 0f;
            cd = 0f;
            Enable = true;
        }

        public void Update()
        {
            if (!Enable)
                return;

            if (Time.unscaledTime - lastTime < cd)
                return;

            lastTime = Time.unscaledTime;
            if ((OptionManager.Instance.DisplayRoleCount < 30))
            {
                CheckHeroVisual(OptionManager.Instance.DisplayRoleCount);
            }
        }

        void CheckHeroVisual(int num)
        {
            showHeros.Clear();
            heroLists.Clear();

            herosDics = GameCenter.mainWorld.GetActorsByType(typeof(Hero));

            if (herosDics.ContainsKey(GameCenter.mainHero.uID))
            {
                showHeros[GameCenter.mainHero.uID] = GameCenter.mainHero;
            }

            List<TeamMem> selfTeamMems = Sys_Team.Instance.getTeamMems();
            for (int index = 0, len = selfTeamMems.Count; index < len; index++)
            {
                if (herosDics.ContainsKey(selfTeamMems[index].MemId))
                {
                    showHeros[selfTeamMems[index].MemId] = herosDics[selfTeamMems[index].MemId] as Hero;
                }
            }

            foreach (Actor hero in herosDics.Values)
            {
                heroLists.Add(hero as Hero);
            }

            heroLists.Sort((HeroA, HeroB) =>
            {
                float distanceA = (GameCenter.mainHero.transform.position - HeroA.transform.position).sqrMagnitude;
                float distanceB = (GameCenter.mainHero.transform.position - HeroB.transform.position).sqrMagnitude;

                if (distanceA > distanceB)
                    return 1;
                else if (distanceA < distanceB)
                    return -1;
                else
                    return 0;
            });

            for (int index = 0, len = heroLists.Count; index < len; index++)
            {
                if (num > 0 && !showHeros.ContainsKey(heroLists[index].uID))
                {
                    showHeros[heroLists[index].uID] = heroLists[index];
                    num--;

                    if (heroLists[index] != null && heroLists[index].heroBaseComponent != null && heroLists[index].heroBaseComponent.IsCaptain)
                    {
                        for (int index2 = index + 1, len2 = heroLists.Count; index2 < len2; index2++)
                        {
                            if (heroLists[index].heroBaseComponent.TeamID == heroLists[index2].heroBaseComponent.TeamID)
                            {
                                showHeros[heroLists[index2].uID] = heroLists[index2];
                                num--;
                            }
                        }
                    }
                }           
            }

            for (int index = 0, len = heroLists.Count; index < len; index++)
            {
                if (showHeros.ContainsKey(heroLists[index].uID))
                {
                    heroLists[index].Show();
                }
                else
                {
                    heroLists[index].Hide();
                }
            }
        }
        */
    }
}
