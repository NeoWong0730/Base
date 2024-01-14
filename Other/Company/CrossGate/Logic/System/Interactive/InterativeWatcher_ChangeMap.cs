using Lib.Core;

namespace Logic
{
    //public class InteractiveChangeMapData
    //{
    //    public bool enter;
    //    public uint portID;
    //    public uint gridX;
    //    public uint gridY;
    //}

    //[InteractiveWatcher(EInteractiveAimType.ChangeMap)]
    //public class InterativeWatcher_ChangeMap : IInteractiveWatcher
    //{
    //    public void OnGridAutoExecute(InteractiveEvtData data)
    //    {
    //        DebugUtil.Log(ELogType.eNPC, "InterativeWatcher_ChangeMap.OnGridAutoExecute()");

    //        if (data == null)
    //            return;

    //        if (ActionCtrl.Instance.actionCtrlStatus == ActionCtrl.EActionCtrlStatus.Auto)
    //            return;

    //        InteractiveChangeMapData interactiveChangeMapData = data.data as InteractiveChangeMapData;

    //        if (interactiveChangeMapData != null && interactiveChangeMapData.enter)
    //        {
    //            TransmitAction transmitAction = ActionCtrl.Instance.CreateAction(TransmitAction.TypeName) as TransmitAction;
    //            if (transmitAction != null)
    //            {
    //                transmitAction.interactiveChangeMapData = interactiveChangeMapData;
    //                transmitAction.Init();
    //                ActionCtrl.Instance.ExecutePlayerCtrlAction(transmitAction);
    //            }
    //        }
    //    }

    //    public void OnClickExecute(InteractiveEvtData data)
    //    {
    //        DebugUtil.Log(ELogType.eNPC, "InterativeWatcher_ChangeMap.OnClickExecute()");
    //    }

    //    public void OnDoubleClickExecute(InteractiveEvtData data)
    //    {
    //        DebugUtil.Log(ELogType.eNPC, "InterativeWatcher_ChangeMap.OnDoubleClickExecute()");
    //    }

    //    public void OnLongPressExecute(InteractiveEvtData data)
    //    {
    //        DebugUtil.Log(ELogType.eNPC, "InterativeWatcher_ChangeMap.OnLongPressExecute()");
    //    }

    //    public void OnDistanceCheckExecute(InteractiveEvtData data)
    //    {

    //    }
    //}
}
