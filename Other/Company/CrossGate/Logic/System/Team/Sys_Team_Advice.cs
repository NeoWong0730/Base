using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;

using Table;

namespace Logic
{
    public partial class Sys_Team
    {
       
        uint EnterTeamTimeConfig = 600;

        uint AdviceTipTime = 30;

        uint AdviceCDTime = 0;
        public void DoAdvice(uint battletypeID)
        {
            if (isCaptain() == false)
                return;

            var nottime = Sys_Time.Instance.GetServerTime();
            var nowtiemoffset = nottime - TeamInfo.TeamInfo.InviteSocialData.LastTriggerTime;

            if (nowtiemoffset <= AdviceCDTime)
                return;

            CSVBattleType.Data cSVBattleTypeData = CSVBattleType.Instance.GetConfData(battletypeID);

            if (cSVBattleTypeData.block_family_advice > 0 && cSVBattleTypeData.block_friend_advice > 0)
                return;

            var recordtimeoffset = nottime - TeamInfo.TeamInfo.InviteSocialData.LastRecordTime;
            int nowtimezero = (int)(nottime % 86400) - 18000;
            int nowrecordoffset = nowtimezero - (int)recordtimeoffset;

            if (recordtimeoffset > 86400 || (nowtimezero >= 0 && nowrecordoffset < 0)) //当前时间过了凌晨5点，清空邀请记录
            {
                TeamInfo.TeamInfo.InviteSocialData.InviteGuildMemIds.Clear();
                TeamInfo.TeamInfo.InviteSocialData.InviteFriendIds.Clear();
            }

            RepeatedField<ulong> roles = new RepeatedField<ulong>();
            int count = TeamMemsCount;

            for (int i = 1; i < count; i++)
            {
                var offsettime = nottime < teamMems[i].EnterTime ? 0 : (nottime - teamMems[i].EnterTime);

                if (teamMems[i].IsRob() == false && offsettime >= EnterTeamTimeConfig && 
                   ( HadInvitatedFamily(teamMems[i].MemId) == false || HadInvitatedFriend(teamMems[i].MemId) == false))
                {
                    roles.Add(teamMems[i].MemId);
                   
                }
            }

            if (roles.Count > 0)
            {
                SendInviteSocialReq();
            }
        }

        private bool HadInvitatedFamily(ulong roleid)
        {
            if (TeamInfo == null || TeamInfo.TeamInfo.InviteSocialData == null)
                return false;

            var list = TeamInfo.TeamInfo.InviteSocialData.InviteGuildMemIds;
            int count =list.Count;

            for (int i = 0; i < count; i++)
            {
                if (list[i] == roleid)
                    return true;
            }

            return false;
        }


        private bool HadInvitatedFriend(ulong roleid)
        {
            if (TeamInfo == null || TeamInfo.TeamInfo.InviteSocialData == null)
                return false;

            var list = TeamInfo.TeamInfo.InviteSocialData.InviteFriendIds;
            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                if (list[i] == roleid)
                    return true;
            }

            return false;
        }



        private void OpenAdviceDialog(uint type, RepeatedField<ulong> roles)
        {
            
            string bindnames = string.Empty;

            int count = roles.Count;

            if (type == 0 || count == 0)
                return;

            for (int i = 0; i < count; i++)
            {

                var mem = getTeamMem(roles[i]);

                if (i > 0)
                {
                    bindnames += ",";
                }

                bindnames += mem.Name.ToStringUtf8();

            }

            string message = string.Empty;

            if (type == 1)
                message = LanguageHelper.GetTextContent(2003101, bindnames);
            else if(type == 2)
                message = LanguageHelper.GetTextContent(2003102, bindnames);

            OpenTips(AdviceTipTime, message, () =>
            {
                SendOp(true, type, roles);
            },
            ()=>{
                SendOp(false, type, roles);
            }, null, true, 2003104u, 2003103u);

        }

        private void SendOp(bool send, uint type, RepeatedField<ulong> roles)
        {
            SendInviteSocialConfirmReq(roles, type, send);
        }
    }
}
