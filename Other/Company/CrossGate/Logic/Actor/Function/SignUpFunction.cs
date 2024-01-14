using Lib.Core;
using Logic.Core;
using Table;

namespace Logic {
    /// <summary>
    /// 报名世界boss///
    /// </summary>
    public class SignUpFunction : FunctionBase {

        public enum EReason {
            None,
            PlayerLevelNotValid,
            NoTeam,
            NotEnoughTeamMember,
            NotCaptain,
            MemberLevelNotValid,
            Batting,

            BossNotUnlock,
            DontExistBoss,
        }

        public CSVBOSSInformation.Data csv {
            get;
            private set;
        }

        public EReason reason = EReason.None;

        public override void Init() {
            this.csv = CSVBOSSInformation.Instance.GetConfData(this.ID);
        }

        protected override void OnDispose() {
            this.reason = EReason.None;
            this.csv = null;

            base.OnDispose();
        }

        // 可执行的话，才会执行 OnExecute
        // boss解锁才能执行
        protected override bool CanExecute(bool CheckVisual = true) {
            if (this.csv == null) {
                this.reason = EReason.DontExistBoss;
                return false;
            }

            bool isBattling = false;
#if false
            //WorldBossComponent wb = World.GetComponent<WorldBossComponent>(this.npc);
            WorldBossComponent wb = this.npc.worldBossComponent;
            if (wb != null) {
                isBattling = wb.BattleID != 0;
            }
#else
            WorldBossNpc worldBoss = npc as WorldBossNpc;
            if (worldBoss != null)
            {
                isBattling = worldBoss.worldBossComponent.BattleID != 0;
            }
#endif

            if (isBattling) {
                this.reason = EReason.Batting;
                return false;
            }

            bool hasTeam = Sys_Team.Instance.HaveTeam;
            if (!hasTeam) {
                this.reason = EReason.NoTeam;
                return false;
            }
            bool isCaptain = Sys_Team.Instance.isCaptain();
            if (!isCaptain) {
                this.reason = EReason.NotCaptain;
                return false;
            }

            bool teamMemberCountValid = Sys_Team.Instance.TeamMemsCountOfActive >= this.csv.minimumTeamSize;
            if (!teamMemberCountValid) {
                this.reason = EReason.NotEnoughTeamMember;
                return false;
            }

            bool canPlayerEnter = Sys_WorldBoss.Instance.CanEnter(this.ID, Sys_Role.Instance.Role.Level);
            if (!canPlayerEnter) {
                this.reason = EReason.PlayerLevelNotValid;
                return false;
            }

            CSVBOSSInformation.Data csvBoss = CSVBOSSInformation.Instance.GetConfData(this.ID);
            if (csvBoss == null) {
                return false;
            }

            //bool isUnlocked = Sys_WorldBoss.Instance.IsUnlockedBossManual(csvBoss.bossManual_id);
            //if (!isUnlocked) {
            //    this.reason = EReason.BossNotUnlock;
            //    return false;
            //}

            // var ls = Sys_Team.Instance.teamMemsOfActive;
            // for (int i = 0, length = ls.Count; i < length; ++i) {
            //     if (!Sys_WorldBoss.Instance.CanEnter(this.ID, ls[i].Level)) {
            //         this.reason = EReason.MemberLevelNotValid;
            //         return false;
            //     }
            // }

            return true;
        }
        protected override void OnCantExecTip() {
            // 提示信息
            DebugUtil.Log(ELogType.eWorldBoss, this.reason.ToString());

            switch (this.reason) {
                case EReason.PlayerLevelNotValid:
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4157000008, this.csv.limit_level[0].ToString(), this.csv.limit_level[1].ToString()));
                    break;
                case EReason.NoTeam:
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4157000009));
                    break;
                case EReason.NotEnoughTeamMember:
                    string s = LanguageHelper.GetTextContent(4157000010, csv.minimumTeamSize.ToString());
                    Sys_Hint.Instance.PushContent_Normal(s);
                    break;
                case EReason.NotCaptain:  // 不是队长
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4157000011));
                    break;
                // case EReason.MemberLevelNotValid:
                //     Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4157000012, this.csv.limit_level[0].ToString(), this.csv.limit_level[1].ToString()));
                //     break;
                case EReason.BossNotUnlock:
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4157000013));
                    break;
                case EReason.DontExistBoss:
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4157000014));
                    break;
                case EReason.Batting:
                    void OnConform() {
                        // 请求观战
                        Sys_WorldBoss.Instance.ReqWatchBattle(this.npc.UID);
                    }

                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.SetCountdown(10f, PromptBoxParameter.ECountdown.Cancel);
                    PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(4157000029).words;
                    PromptBoxParameter.Instance.SetConfirm(true, OnConform, 4157000024);
                    PromptBoxParameter.Instance.SetCancel(true, null, 4157000027);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                    break;
            }
        }

        // 不合法的话，直接不在选择列表展示
        //public override bool IsValid()
        //{
        //    return true;
        //}

        protected override void OnExecute() {
            base.OnExecute();

            ulong teamId = Sys_Team.Instance.teamID;
            ulong roleId = Sys_Role.Instance.RoleId;
            ulong guid = this.npc.UID;
            Sys_WorldBoss.Instance.ReqSignUp(this.ID, teamId, roleId, 0, guid);
        }
    }
}

