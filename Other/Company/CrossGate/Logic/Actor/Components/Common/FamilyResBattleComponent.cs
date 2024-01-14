using Logic.Core;
using Packet;
using Table;
using pbc = global::Google.Protobuf.Collections;

namespace Logic {
    // 家族资源战
    public class FamilyResBattleComponent : Logic.Core.Component {
        public class Transfer1 {
            public ulong roleid;
            public RoleMapBuffUnit buff;

            public Transfer1(ulong roleid, RoleMapBuffUnit buff) {
                this.roleid = roleid;
                this.buff = buff;
            }
        }
        
        public class Transfer2 {
            public ulong roleid;
            public pbc::RepeatedField<uint> buffIds;

            public Transfer2(ulong roleid, pbc::RepeatedField<uint> buffIds) {
                this.roleid = roleid;
                this.buffIds = buffIds;
            }
        }
        
        public enum ECamp {
            Left = 1,
            Right = 2,
        }

        #region 数据区域

        // 左右阵营
        public ECamp camp = ECamp.Left;

        public ulong RoleId;
        public uint Camp;

        // 敌友标签
        public bool isRed {
            get { return Camp == Sys_FamilyResBattle.Instance.redCampId; }
        }

        public bool hasResource {
            get { return Sys_FamilyResBattle.Instance.HasResource(out uint resId) && resId != 0; }
        }

        // 队长头顶标签
        public uint memberCount = 0;
        public uint resource = 0;
        public uint maxCount = 0;
        public long BuffExpiretime = 0;

        public SceneActor sceneActor;

        public bool InProtecting {
            get { return BuffExpiretime != 0 && BuffExpiretime > Sys_Time.Instance.GetServerTime(); }
        }

        public long remain {
            get { return BuffExpiretime - Sys_Time.Instance.GetServerTime(); }
        }

        protected override void OnConstruct() {
            this.sceneActor = this.actor as SceneActor;
            
            this.ProcessEvents(false);
            this.ProcessEvents(true);
        }

        protected override void OnDispose() {
            this.ProcessEvents(false);
        }

        private void ProcessEvents(bool toRegist) {
            Sys_Map.Instance.eventEmitter.Handle<FamilyResBattleComponent.Transfer1>(Sys_Map.EEvents.OnProtectBuffAdd, this.OnProtectBuffAdd, toRegist);
            Sys_Map.Instance.eventEmitter.Handle<FamilyResBattleComponent.Transfer2>(Sys_Map.EEvents.OnProtectBuffRemove, this.OnProtectBuffRemove, toRegist);
        }

        private void OnProtectBuffAdd(FamilyResBattleComponent.Transfer1 transfer) {
            if (actor.uID == transfer.roleid && transfer.buff.Buffid == (uint) MapRoleBuffType.GuildBattleProtection) {
                this.BuffExpiretime = transfer.buff.Expiretime;
                this.TryAddBuff();
            }
        }

        private void OnProtectBuffRemove(FamilyResBattleComponent.Transfer2 transfer) {
            if (actor.uID == transfer.roleid && transfer.buffIds != null) {
                for (int i = 0, length = transfer.buffIds.Count; i < length; ++i) {
                    if (transfer.buffIds[i] == (uint) MapRoleBuffType.GuildBattleProtection) {
                        TryRemoveBuff();
                    }
                }
            }
        }

        #endregion

        #region 玩家自己
        public void AssignMain(uint guildTeamNum, uint guildTeamMaxCount) {
            // 主角数据
            memberCount = guildTeamNum;
            maxCount = guildTeamMaxCount;
            Camp = Sys_FamilyResBattle.Instance.redCampId;
        }

        public void AssignRes(uint res) {
            // 家族资源站系统中赋值
            resource = res;
            if (resource != 0) {
                Sys_FamilyResBattle.Instance._TryGuideNpc();
            }
        }

        public void AssignBuff(RoleMapBuffUnit mainHeroBuff) {
            // 家族资源站系统中赋值
            if (mainHeroBuff != null && mainHeroBuff.Buffid == (uint) MapRoleBuffType.GuildBattleProtection) {
                BuffExpiretime = mainHeroBuff.Expiretime;

                TryAddBuff();
            }
        }

        #endregion

        #region 其他玩家
        public void AssignOther(MapRole mapRole) {
            // 视野外进入的时候会触发
            resource = 0;
            memberCount = 0;
            maxCount = 0;
            BuffExpiretime = 0;

            if (mapRole.BattleMapData != null) {
                Camp = mapRole.BattleMapData.Camp;
                resource = mapRole.BattleMapData.Resource;

                if (mapRole.BattleMapData.Leader != null) {
                    memberCount = mapRole.BattleMapData.Leader.MemberCount;
                    maxCount = mapRole.BattleMapData.Leader.MaxCount;
                }
            }

            _TryAddBuff(mapRole);
        }

        #endregion

        private void _TryAddBuff(MapRole mapRole) {
            if (mapRole.OtherBuffData != null && mapRole.OtherBuffData.Bufflist != null) {
                var buffList = mapRole.OtherBuffData.Bufflist;
                for (int i = 0, length = buffList.Count; i < length; ++i) {
                    if (buffList[i].Buffid == (uint) MapRoleBuffType.GuildBattleProtection) {
                        BuffExpiretime = buffList[i].Expiretime;

                        TryAddBuff();
                        break;
                    }
                }
            }
        }

        private void TryAddBuff() {
            if (InProtecting) {
                CSVEffect.Data csvEffect = CSVEffect.Instance.GetConfData(57210);
                EffectUtil.Instance.LoadEffect(actor.uID, csvEffect.effects_path, this.sceneActor.fxRoot.transform, EffectUtil.EEffectTag.BattleProtect, remain);
            }
        }

        private void TryRemoveBuff() {
            EffectUtil.Instance.UnloadEffectByTag(actor.uID, EffectUtil.EEffectTag.BattleProtect);
        }
    }
}