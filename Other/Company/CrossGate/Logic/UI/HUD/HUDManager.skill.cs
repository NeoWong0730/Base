using UnityEngine;
using Logic.Core;
using Lib.Core;

namespace Logic
{
    public partial class HUD : UIBase
    {
        #region Skill

        public void CreateSkillShow(TriggerSkillEvt triggerSkillEvt)
        {
            GameObject go;
            go = SkillPools.Get(root_SkillShow);
            go.SetActive(true);
            //AnimData animData = new AnimData(go, Sys_HUD.Instance.huds[triggerSkillEvt.id], -1, -1, triggerSkillEvt.skillcontent);
            AnimData animData = CombatObjectPool.Instance.Get<AnimData>();
            animData.uiObj = go;
            animData.battleObj = Sys_HUD.Instance.battleHuds[triggerSkillEvt.id];
            animData.finnaldamage = -1;
            animData.floatingdamage = -1;
            animData.content = triggerSkillEvt.skillcontent;
            animData.bUseTrans = true;
            animData.pos = Vector3.zero;
            animData.clientNum = triggerSkillEvt.clientNum;

            SkillShow skillShow = HUDFactory.Get<SkillShow>();
            skillShow.Construct(animData, null, RecyleSkill);
            anim_BaseShows.Add(skillShow);

            //animData.Dispose();
            //CombatObjectPool.Instance.Push(animData);
        }

        #endregion

        #region SecondAction
        public void TriggerSecondAction(uint id)
        {
            GameObject go;
            go = SecondActionHUDPools.Get(root_SecondAction);
            if (Sys_HUD.Instance.battleHuds.ContainsKey(id))
            {
                GameObject _gameObject = Sys_HUD.Instance.battleHuds[id];
                if (_gameObject == null)
                {
                    return;
                }
                go.SetActive(true);
                SecondActionShow secondActionShow = HUDFactory.Get<SecondActionShow>();
                secondActionShow.Construct(go, RecyleSecondAction);
                secondActionShow.SetTarget(_gameObject.transform);
                secondActionShow.TriggerSecondAction();
                secondActionShows.Add(secondActionShow);
            }
            else
            {
                DebugUtil.LogErrorFormat("TriggerSecondAction : 未找到id为{0}的战斗单位", id);
            }
        }

        #endregion


        public void RecyleSkill(Anim_BaseShow skillShow)
        {
            SkillPools.Recovery(skillShow.mRootGameObject);
            skillShow.Dispose();
            HUDFactory.Recycle(skillShow);
            anim_BaseShows.Remove(skillShow);
        }

        public void RecyleSecondAction(SecondActionShow secondActionShow)
        {
            SecondActionHUDPools.Recovery(secondActionShow.mRootGameObject);
            secondActionShow.Dispose();
            HUDFactory.Recycle(secondActionShow);
            secondActionShows.Remove(secondActionShow);
        }
    }
}

