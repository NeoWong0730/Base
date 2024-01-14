using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;
using UnityEngine;

namespace Logic
{
    public class Sys_Narrate : SystemModuleBase<Sys_Narrate>
    {

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents
        {
            OnNarrateEnd,
        }

        public override void Init()
        {
            Sys_Task.Instance.eventEmitter.Handle<uint>(Sys_Task.EEvents.OnSubmitedForChapter, OnNeedShowChapter, true);
            Sys_Task.Instance.eventEmitter.Handle<uint>(Sys_Task.EEvents.OnSubmitedForSubTitle, OnNeedShowSubtitle, true);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.SyncSubTitle, OnNetNeedShowSubtitle, CmdPetChangePositionRes.Parser);
        }

        private void OnNetNeedShowSubtitle(NetMsg msg)
        {
            CmdTeamSyncSubTitle dataNtf = NetMsgUtil.Deserialize<CmdTeamSyncSubTitle>(CmdTeamSyncSubTitle.Parser, msg);
            OnNeedShowSubtitle(dataNtf.SubTitleId);
        }

        private void OnNeedShowChapter(uint chapterID)
        {
            CSVChapter.Data data = CSVChapter.Instance.GetConfData(chapterID);
            if (data == null)
                return;
            GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterCutScene);
            
            Logic.Core.UIScheduler.Push(EUIID.UI_Chapter, data, null, true, UIScheduler.popTypes[EUIPopType.WhenMaininterfaceRealOpenning]);
        }

        private void OnNeedShowSubtitle(uint subtitileId)
        {
            CSVSubtitle.Data data = CSVSubtitle.Instance.GetConfData(subtitileId);
            if (data == null)
                return;
            GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterCutScene);
            Logic.Core.UIScheduler.Push((EUIID)data.subtitleModel, data, null, true, UIScheduler.popTypes[EUIPopType.WhenMaininterfaceRealOpenning]);
        }
    }
}
