using Logic.Core;
using Table;

namespace Logic
{
    /// <summary>
    /// 玩家表演动作///
    /// </summary>
    public class RoleActionComponent : Logic.Core.Component//, IUpdateCmd
    {
        public class RoleAction
        {
            public uint actionID;
            public uint direction;
            public uint startTime;
        }

        RoleAction cacheRoleAction;
        Hero hero;

        public void SetCacheRoleAction(RoleAction _roleAction)
        {
            if (cacheRoleAction == null)
            {
                cacheRoleAction = _roleAction;
                return;
            }

            if (_roleAction.startTime > cacheRoleAction.startTime)
            {
                cacheRoleAction = _roleAction;
            }
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();

            hero = actor as Hero;
        }

        public void Update()
        {
            if (cacheRoleAction != null)
            {
                if (hero.stateComponent.CurrentState == EStateType.Idle)
                {
                    PlayAction();
                    cacheRoleAction = null;
                }
            }
        }

        public void SelfPlayAction(uint actionID)
        {
            CSVActionState.Data data = CSVActionState.Instance.GetConfData(actionID);
            if (data != null)
            {
                if (!data.Loop)
                {
                    hero.animationComponent.Play(actionID);
                }
                else
                {
                    hero.animationComponent.Play(actionID, () =>
                    {
                        hero.animationComponent.Play((uint)EStateType.Idle);
                    });
                }
            }
        }

        void PlayAction()
        {
            hero.transform.localEulerAngles = new UnityEngine.Vector3(0, cacheRoleAction.direction / 1000f, 0);

            CSVActionState.Data data = CSVActionState.Instance.GetConfData(cacheRoleAction.actionID);
            if (data != null)
            {
                if (!data.Loop)
                {
                    hero.animationComponent.Play(cacheRoleAction.actionID);
                }
                else
                {
                    hero.animationComponent.Play(cacheRoleAction.actionID, () =>
                    {
                        hero.animationComponent.Play((uint)EStateType.Idle);
                    });
                }
            }
        }

        protected override void OnDispose()
        {
            cacheRoleAction = null;
            hero = null;

            base.OnDispose();
        }
    }
}
