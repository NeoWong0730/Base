using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using Net;
using Packet;

namespace Logic
{
    /// <summary>
    /// 魔法变形///
    /// </summary>
    public class Sys_MagicChange : SystemModuleBase<Sys_MagicChange>
    {
        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener((ushort)CmdRole.MagicShapeShiftReq, (ushort)CmdRole.MagicShapeShiftRes, OnMagicShapeShiftRes, CmdRoleMagicShapeShiftRes.Parser);
        }

        public void ReqMagicShapeShift(uint heroID)
        {
            CmdRoleMagicShapeShiftReq req = new CmdRoleMagicShapeShiftReq();
            req.HeroId = heroID;

            NetClient.Instance.SendMessage((ushort)CmdRole.MagicShapeShiftReq, req);
        }

        void OnMagicShapeShiftRes(NetMsg msg)
        {
            CmdRoleMagicShapeShiftRes res = NetMsgUtil.Deserialize<CmdRoleMagicShapeShiftRes>(CmdRoleMagicShapeShiftRes.Parser, msg);
            if (res != null)
            {
                void OnConform()
                {
                    Sys_Role.Instance.ExitGameReq();
                }

                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(13498);
                PromptBoxParameter.Instance.SetConfirm(true, OnConform);
                PromptBoxParameter.Instance.SetCancel(false, null);

                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
        }
    }
}