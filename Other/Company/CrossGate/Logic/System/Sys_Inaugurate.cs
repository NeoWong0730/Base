using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;

namespace Logic
{
    public class Sys_Inaugurate : SystemModuleBase<Sys_Inaugurate>
    {
        /// <summary> 事件枚举 </summary>
        public enum EEvents
        {
            OnChangeCareer,
        }
        /// <summary> 事件列表 </summary>
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener((ushort)CmdRole.SelectCareerReq, (ushort)CmdRole.SelectCareerRes, OnSelectCareerRes, CmdRoleSelectCareerRes.Parser);
        }

        private void OnSelectCareerRes(NetMsg netMsg)
        {
            CmdRoleSelectCareerRes res = NetMsgUtil.Deserialize<CmdRoleSelectCareerRes>(CmdRoleSelectCareerRes.Parser, netMsg);
            Sys_Role.Instance.Role.Career = res.CareerId;

            GameCenter.mainHero.careerComponent.UpdateCareerType((ECareerType)res.CareerId);
            CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(res.CareerId);
            //GameCenter.mainHero.weaponComponent.UpdateWeapon(cSVCareerData.weapon);
            GameCenter.mainHero.animationComponent.UpdateHoldingAnimations(GameCenter.mainHero.heroBaseComponent.HeroID, GameCenter.mainHero.weaponComponent.CurWeaponID, Constants.IdleAndRunAnimationClipHashSet);
            Sys_Inaugurate.Instance.eventEmitter.Trigger(EEvents.OnChangeCareer);
        }


        public void RoleSelectCareerReq(uint id)
        {
            CmdRoleSelectCareerReq cmdRoleSelectOccupationReq = new CmdRoleSelectCareerReq();
            cmdRoleSelectOccupationReq.CareerId = id;
            //             byte[] messagedata = NetMsgUtil.Serialize((ushort)CmdRole.SelectOccupationReq, cmdRoleSelectOccupationReq);
            //             NetClient.Instance.SendData(messagedata);
            NetClient.Instance.SendMessage((ushort)CmdRole.SelectCareerReq, cmdRoleSelectOccupationReq);
        }
    }
}


