using Logic.Core;
using Net;
using Packet;

namespace Logic
{
    /// <summary>
    /// 进入范围重复啊战斗///
    /// </summary>
    public class TriggerFightFunction : FunctionBase
    {
        protected override bool CanExecute(bool CheckVisual = true)
        {
            ///组队状态下如果是队员且非暂离不能执行///
            if (Sys_Team.Instance.HaveTeam && !Sys_Team.Instance.isCaptain() && !Sys_Team.Instance.isPlayerLeave())
            {
                return false;
            }

            return true;
        }

        protected override void OnExecute()
        {
            base.OnExecute();

            //ActiveMonsterComponent activeMonsterComponent = World.GetComponent<ActiveMonsterComponent>(npc);
            //ActiveMonsterComponent activeMonsterComponent = npc.activeMonsterComponent;
            //if (activeMonsterComponent != null)
            if (npc.cSVNpcData.type == (uint)ENPCType.ActiveMonster)
            {                
                if (npc.activeMonsterComponent.canFightFlag)
                {
                    if (FunctionSourceType == EFunctionSourceType.Task)
                    {
                        CmdTaskEnterBattleReq req = new CmdTaskEnterBattleReq();
                        req.BattleId = ID;
                        req.TaskId = HandlerID;
                        req.TaskIndex = HandlerIndex;
                        req.UNpcId = npc.uID;

                        NetClient.Instance.SendMessage((ushort)CmdTask.EnterBattleReq, req);                        
                    }
                    else
                    {
                        CmdNpcEnterBattleReq req = new CmdNpcEnterBattleReq();
                        req.Id = npc.uID;
                        req.FightId = ID;

                        NetClient.Instance.SendMessage((ushort)CmdNpc.EnterBattleReq, req);                        
                    }
                }
            }
            else
            {
                if (FunctionSourceType == EFunctionSourceType.Task)
                {
                    CmdTaskEnterBattleReq req = new CmdTaskEnterBattleReq();
                    req.BattleId = ID;
                    req.TaskId = HandlerID;
                    req.TaskIndex = HandlerIndex;
                    req.UNpcId = npc.uID;

                    NetClient.Instance.SendMessage((ushort)CmdTask.EnterBattleReq, req);                    
                }
                else
                {
                    CmdNpcEnterBattleReq req = new CmdNpcEnterBattleReq();
                    req.Id = npc.uID;
                    req.FightId = ID;

                    NetClient.Instance.SendMessage((ushort)CmdNpc.EnterBattleReq, req);                    
                }
            }
        }
    }
}
