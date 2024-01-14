using Logic;
using Logic.Core;
using UnityEngine;

public class UploadTransformSystem : LevelSystemBase
{
    public bool CanSendFlag = true;
    public bool NetUpdate = true;
    public bool TeamNetUpdate = false;
    public bool EnableMainHero = true;
    public float fLastTime = 0f;

    public override void OnUpdate()
    {     
        if (!NetUpdate)
            return;

        if (!EnableMainHero)
            return;

        if (!CanSendFlag)
            return;
        
        if (GameMain.Procedure == null
            || GameMain.Procedure.CurrentProcedure == null
            || GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Normal)
            return;

        if (GameCenter.mainHero == null)
            return;

        if (Time.unscaledTime < fLastTime)
        {
            return;
        }

        fLastTime = Time.unscaledTime + 0.5f;
        //Lib.Core.DebugUtil.LogWarningFormat("UpLoad===1");
        Sys_Map.Instance.ReqMove(TeamNetUpdate);
    }
}