using UnityEngine;
using Logic.Core;
using Table;
using Lib.Core;

namespace Logic
{
    public partial class HUD : UIBase
    {
        #region Buff

        public void UpdateBuffHUD(BuffHuDUpdateEvt buffHuDUpdateEvt)
        {
            if (CSVBuff.Instance.GetConfData(buffHuDUpdateEvt.buffid).is_show == 0)
            {
                return;
            }
            if (!Sys_HUD.Instance.battleHuds.TryGetValue(buffHuDUpdateEvt.id, out GameObject gameObject))
            {
                return;
            }
            if (gameObject == null)
            {
                DebugUtil.LogErrorFormat("HUD.UpdateBuffHUD===>gameObject=null");
                return;
            }
            bool valid = true;
            if (GameCenter.mainFightHero != null)
            {
                bool isSameCamp = CombatHelp.IsSameCamp(GameCenter.mainFightHero.battleUnit, buffHuDUpdateEvt.side);
                if (isSameCamp && !CombatManager.Instance.m_BattleTypeTb.show_self_BuffHUD)
                {
                    valid = false;
                }
                else if (!CombatManager.Instance.m_BattleTypeTb.show_enemy_BuffHUD)
                {
                    valid = false;
                }
            }
            if (!valid)
            {
                return;
            }

            BuffHUDShow buffHUDShow;
            GameObject go;
            if (!buffhudShows.TryGetValue(buffHuDUpdateEvt.id, out buffHUDShow))
            {
                go = BuffHUDPools.Get(root_BuffHud);
                buffHUDShow = HUDFactory.Get<BuffHUDShow>();
                buffHUDShow.Construct(go, buffHuDUpdateEvt.side);
                buffHUDShow.SetTarget(gameObject.transform);
                //Sys_HUD.Instance.buffhudCount++;
                buffhudShows.Add(buffHuDUpdateEvt.id, buffHUDShow);
            }
            else
            {
                go = buffHUDShow.mRootGameObject;
            }
            go.SetActive(true);
            buffHUDShow.UpdateBuffIcon(buffHuDUpdateEvt.buffid, buffHuDUpdateEvt.add, buffHuDUpdateEvt.count);
        }

        public void BuffFalsh(BuffHUDFlashEvt buffHUDFlashEvt)
        {
            if (!buffhudShows.ContainsKey(buffHUDFlashEvt.id))
                return;
            BuffHUDShow buffHUDShow = buffhudShows[buffHUDFlashEvt.id];

        }

        public void ShowOrHideBuffHUD(uint id, bool show)
        {
            if (!buffhudShows.ContainsKey(id))
                return;
            BuffHUDShow buffHUDShow = buffhudShows[id];
            buffHUDShow.ShowOrHide(show);
        }
        #endregion
    }
}

