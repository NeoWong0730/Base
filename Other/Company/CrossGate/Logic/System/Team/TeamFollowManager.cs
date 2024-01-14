using Logic.Core;
using System;
using System.Collections.Generic;


namespace Logic
{
    public partial class Sys_Team
    {
        public readonly float keepDiatance = 1.5f;

        private MemerFollowManager memerFollowManager = new MemerFollowManager();
        private CaptainFollowManager captainFollowManager = new CaptainFollowManager();

        private FollowManager followManager = null;

        private void UpdateFollowManager()
        {
            if (HaveTeam == false)
            {
                if (followManager != null)
                {
                    followManager.Clear();
                    followManager = null;
                }
                return;
            }

            if (isCaptain())
            {
                if (followManager == memerFollowManager)
                    followManager.Clear();

                followManager = captainFollowManager;
            }
            else
            {
                if (followManager == captainFollowManager)
                    followManager.Clear();

                followManager = memerFollowManager;
            }

            followManager.UpdateFormation();
        }

        private void ClearFollow()
        {
            if (followManager == null)
                return;

            if (followManager == memerFollowManager)
                followManager.ExitTeam(Sys_Role.Instance.RoleId);

            followManager.Clear();
            followManager = null;
        }

        public void DoFollow(ulong roleid)
        {
            if (HaveTeam == false)
                return;

            var index = MemIndex(roleid);

            if (index < 0)
                return;

            if (followManager != null)
                followManager.EnterTeam(roleid);
        }

        public void DoNotFollow(ulong roleid)
        {
            if (HaveTeam == false)
                return;

            var index = MemIndex(roleid);

            if (index < 0)
                return;

            if (followManager != null)
                followManager.ExitTeam(roleid);
        }

        public class FollowManager
        {
            public enum EFollowError
            {
                None = 0,
                /// <summary>
                /// 和目标是同一
                /// </summary>
                SameID,
                /// <summary>
                /// 目标actor不存在
                /// </summary>
                NoTargetActor,
                /// <summary>
                /// 自己的actor不存在
                /// </summary>
                NoActor
            }
            public virtual void UpdateFormation()
            {

            }

            public virtual void EnterTeam(ulong roleid)
            {

            }

            public virtual void ExitTeam(ulong roleid)
            {

            }

            public virtual void Clear()
            {

            }
        }
        public class MemerFollowManager : FollowManager
        {

            public override void UpdateFormation()
            {
               var mems = Sys_Team.Instance.getTeamMem(Sys_Role.Instance.RoleId);

                if (mems == null || mems.IsLeave())
                    ExitTeam(Sys_Role.Instance.RoleId);
                else
                   EnterTeam(Sys_Role.Instance.RoleId);
            }

            public override void EnterTeam(ulong roleid)
            {
                if (roleid == Sys_Role.Instance.RoleId )
                {
                    var mems = Sys_Team.Instance.getTeamMem(Sys_Role.Instance.RoleId);

                    if (mems.IsLeave())
                        return;

                    DisableOpertor();

                    UIManager.OpenUI(EUIID.UI_Teaming);
                }
            }

            public override void ExitTeam(ulong roleid)
            {
                if (roleid == Sys_Role.Instance.RoleId)
                {
                    EnableOpertor();

                    UIManager.CloseUI(EUIID.UI_Teaming);
                }
            }

            public override void Clear()
            {
                EnableOpertor();
            }
            public void DisableOpertor()
            {
                if(GameCenter.mLvPlay != null)
                {
                    GameCenter.mUploadTransformSystem.NetUpdate = false;

                    GameCenter.mPlayerControlSystem?.StopMove();
                }
                else
                {
                    Lib.Core.DebugUtil.LogError("MemerFollowManager.DisableOpertor() GameCenter.mLvPlay 尚未创建");
                }

                Sys_Input.Instance.bEnableJoystick = false;

            }


            public void EnableOpertor()
            {
                if (GameCenter.mLvPlay != null)
                {
                    GameCenter.mUploadTransformSystem.NetUpdate = true;

                    GameCenter.mPlayerControlSystem?.StopMove();
                }
                else
                {
                    Lib.Core.DebugUtil.LogError("MemerFollowManager.EnableOpertor() GameCenter.mLvPlay 尚未创建");
                }

                Sys_Input.Instance.bEnableJoystick = true;

            }
        }


        public class CaptainFollowManager : FollowManager
        {
            List<ulong> formationlist = new List<ulong>();
            public override void UpdateFormation()
            {
                if (Sys_Team.Instance.isCaptain())
                    GameCenter.mUploadTransformSystem.TeamNetUpdate = true;

                UpdateTeamFormation();
            }

            public override void EnterTeam(ulong roleid)
            {
                UpdateTeamFormation();
            }

            public override void ExitTeam(ulong roleid)
            {
                ExitFormation(roleid);

                formationlist.Remove(roleid);
               // UpdateTeamFormation();
            }


            public override void Clear()
            {
                int count = formationlist.Count;

                for (int i = 0; i < count; i++)
                {
                    var roleid = formationlist[i];

                    Hero actor = GameCenter.GetSceneHero(roleid);

                    if (actor != null && actor.followComponent.Follow)
                    {
                        actor.followComponent.SetTarget(null, 0);
                    }

                }

                GameCenter.mUploadTransformSystem.TeamNetUpdate = false;
            }

            public void UpdateTeamFormation()
            {
                int count = Sys_Team.Instance.TeamMemsCount;

                ulong targetid = Sys_Team.Instance.teamMems[0].MemId;

                List<ulong> lastformation = new List<ulong>(formationlist);

                formationlist.Clear();

                for (int i = 1; i < count; i++)
                {
                    if (Sys_Team.Instance.teamMems[i].IsLeave() == false/* && Sys_Team.Instance.teamMems[i].IsOffLine() == false*/)
                    {
                        var result = SetFollowTarget(Sys_Team.Instance.teamMems[i].MemId, targetid, Sys_Team.Instance.keepDiatance, i);

                        if (result == EFollowError.None)
                        {
                            formationlist.Add(Sys_Team.Instance.teamMems[i].MemId);
                            targetid = Sys_Team.Instance.teamMems[i].MemId;
                        }

                    }
                    else
                    {
                        ExitFormation(Sys_Team.Instance.teamMems[i].MemId);
                    }

                    lastformation.Remove(Sys_Team.Instance.teamMems[i].MemId);
                }

                int lastcount = lastformation.Count;

                if (lastcount > 0)
                {
                    for (int i = 0; i < lastcount; i++)
                    {
                        ExitFormation(lastformation[i]);
                    }
                }

            }


            public EFollowError SetFollowTarget(ulong roleid, ulong targetid, float keepdistance, int index)
            {
                if (roleid == targetid)
                    return EFollowError.SameID ;

                Hero targetHero = GameCenter.GetSceneHero(targetid);

                if (targetHero == null)
                    return EFollowError.NoTargetActor;

                Hero actor = GameCenter.GetSceneHero(roleid);

                if (actor == null)
                    return EFollowError.NoActor;


                actor.followComponent.Index = index;
                actor.followComponent.SetTarget(targetHero, keepdistance);


                return EFollowError.None;

            }

            /// <summary>
            /// 退出队形
            /// </summary>
            public void ExitFormation(ulong roleid)
            {

                Hero actor = GameCenter.GetSceneHero(roleid);

                if (actor == null)
                    return;

                actor.followComponent.SetTarget(null, 0);

            }

        }

        // private List<>





    }
}
