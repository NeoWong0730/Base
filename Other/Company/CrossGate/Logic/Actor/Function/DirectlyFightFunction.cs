using Net;
using Packet;

namespace Logic
{
    /// <summary>
    /// 交互战斗功能///
    /// </summary>
    public class DirectlyFightFunction : FunctionBase
    {
        uint languageID_ExecTip = 0;
        uint level = 0;
        protected override bool CanExecute(bool CheckVisual = true)
        {
            if (FunctionSourceType == EFunctionSourceType.Task && Table.CSVTask.Instance.TryGetValue(HandlerID, out Table.CSVTask.Data cSVTaskData))
            {
                if (cSVTaskData.taskCategory == (int)ETaskCategory.Advance)
                {
                    languageID_ExecTip = Sys_Advance.Instance.IsCanIntoAdvanceFight(out level);
                    return languageID_ExecTip == 0;
                }
            }
            return true;
        }


        protected override void OnCantExecTip()
        {
            if (languageID_ExecTip != 0)
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(languageID_ExecTip,level.ToString()));
        }

        private float lastTime = 0;
        protected override void OnExecute()
        {
            var cur = UnityEngine.Time.unscaledTime;
            if (cur < lastTime + 1.2f) {
                return;
            }
            lastTime = cur;

            base.OnExecute();

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

