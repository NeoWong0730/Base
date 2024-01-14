using System;
using System.Collections.Generic;
using Framework;
using Logic;
using Logic.Core;
using Packet;
using Table;
using UnityEngine;

namespace Logic
{

    public partial class UI_BattlePass_Common
    {
        public static List<ItemIdCount> GetBattlePassLevelReward(uint minLevel, uint maxLevel,bool isVip)
        {
            List<ItemIdCount> itemlist = new List<ItemIdCount>();

            uint levelcount = maxLevel - minLevel;

            for (uint i = 0; i <= levelcount; i++)
            {
                uint leveleid = minLevel + i;

                GetBattlePassLevelReward(itemlist,leveleid,isVip);

            }

            return itemlist;
        }

        public static void GetBattlePassLevelVipReward(List<ItemIdCount> itemlist, uint leveleid)
        {
            var data = Sys_BattlePass.Instance.GetLevelReward(leveleid);

            if (data != null)
            {
                itemlist.AddRange(data.VipReward);

            }
        }
        public static void GetBattlePassLevelReward(List<ItemIdCount> itemlist, uint leveleid,bool isVip)
        {
            var data = Sys_BattlePass.Instance.GetLevelReward(leveleid);

            if (data != null)
            {
                itemlist.AddRange(data.NormalReward);

                if (isVip)
                {
                    itemlist.AddRange(data.VipReward);
                }
            }
        }
    }


    public class BattlePassModelShow : ModelShow
    {
        
        public CSVBattlePassRewardDisplay.Data m_Data { get; set; }

        public int ModelIndex { get; set; } = 0;

        HeroLoader m_heroLoader;
        public override void LoadModel()
        {
            m_heroLoader = HeroLoader.Create(true);


            Dictionary<uint, List<dressData>> DressValue = new Dictionary<uint, List<dressData>>();


            DressValue.Add(m_Data.Fashionid, new List<dressData>());

            if (m_Data.Ornaments_Id != null && m_Data.Ornaments_Id.Count > 0)
            {
                int count = m_Data.Ornaments_Id.Count;

                for (int i = 0; i < count; i++)
                {
                    DressValue.Add(m_Data.Ornaments_Id[i], new List<dressData>());
                }
               
            }

            if (m_Data.Weapon_Id > 0)
            {
                DressValue.Add(m_Data.Weapon_Id, new List<dressData>());
            }

            m_heroLoader.LoadHero(m_Data.Hero_Id[ModelIndex], m_Data.equip_id, ELayerMask.ModelShow, DressValue, o =>
            {

                m_heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).SetParent(Parent, null);
            });


            m_heroLoader.heroDisplay.onLoaded += OnLoadend;
        }

        private void OnLoadend(int id)
        {
            if (m_heroLoader.heroDisplay == null || m_heroLoader.heroDisplay.bMainPartFinished == false)
                return;

            if (id == 0)
            {
                var value = m_heroLoader.heroDisplay.GetPart(EHeroModelParts.Main);

                uint charId = uint.Parse(m_Data.Show_Id[ModelIndex]);

                value.gameObject.SetActive(false);

                m_heroLoader.heroDisplay.mAnimation.UpdateHoldingAnimations(charId,0,null,EStateType.Idle,value.gameObject);
            }




           // Parent.transform.Setlayer(ELayerMask.HidingSceneActor);

        }

        public override void Dispose()
        {
            base.Dispose();

            m_heroLoader?.Dispose();

            m_heroLoader = null;

        }


    }



}
