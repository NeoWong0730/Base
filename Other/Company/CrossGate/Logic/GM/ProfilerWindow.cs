using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic;
using Framework;

public class ProfilerWindow : DebugWindowBase
{
    Vector2 systemInfoPos = Vector2.zero;
    public ProfilerWindow(int id) : base(id) { }

    public override void WindowFunction(int id)
    {
#if DEBUG_MODE
        systemInfoPos = GUILayout.BeginScrollView(systemInfoPos);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Name", GUILayout.Width(vSize.x / 3));
        GUILayout.Label("microsecond", GUILayout.Width(vSize.x / 8));
        GUILayout.Label("frame", GUILayout.Width(vSize.x / 8));
        GUILayout.Label("count", GUILayout.Width(vSize.x / 8));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("GameMain", GUILayout.Width(vSize.x / 3));
        GUILayout.Label(Logic.Core.GameMain.LastExecuteTime.ToString(), GUILayout.Width(vSize.x / 8));        
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Timer", GUILayout.Width(vSize.x / 3));        
        GUILayout.Label(Lib.Core.Timer.TimerManager.LastExecuteTime.ToString(), GUILayout.Width(vSize.x / 8));
        GUILayout.Label(Lib.Core.Timer.TimerManager.LastExecuteFrame.ToString(), GUILayout.Width(vSize.x / 8));
        GUILayout.Label(Lib.Core.Timer.TimerManager.LastExecuteCount.ToString(), GUILayout.Width(vSize.x / 8));
        GUILayout.EndHorizontal();

        for (int i = 0; i < Lib.Core.Timer.TimerManager.LastExecuteTimes.Count; ++i)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Lib.Core.Timer.TimerManager.LastExecuteNames[i], GUILayout.Width(vSize.x / 3));
            GUILayout.Label(Lib.Core.Timer.TimerManager.LastExecuteTimes[i].ToString(), GUILayout.Width(vSize.x / 8));
            GUILayout.Label(Lib.Core.Timer.TimerManager.LastExecuteFrames[i].ToString(), GUILayout.Width(vSize.x / 8));
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("NetDispatcher", GUILayout.Width(vSize.x / 3));
        GUILayout.Label(Net.NetClient.Instance.LastExecuteTime.ToString(), GUILayout.Width(vSize.x / 8));
        GUILayout.Label(Net.NetClient.Instance.LastExecuteFrame.ToString(), GUILayout.Width(vSize.x / 8));
        GUILayout.Label(Net.NetClient.Instance.LastDispatcherCount.ToString(), GUILayout.Width(vSize.x / 8));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("ActionCtrl", GUILayout.Width(vSize.x / 3));
        GUILayout.Label(ActionCtrl.Instance.LastExecuteTime.ToString(), GUILayout.Width(vSize.x / 8));
        GUILayout.Label(ActionCtrl.Instance.LastExecuteFrame.ToString(), GUILayout.Width(vSize.x / 8));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("CollectionCtrl", GUILayout.Width(vSize.x / 3));
        GUILayout.Label(CollectionCtrl.Instance.LastExecuteTime.ToString(), GUILayout.Width(vSize.x / 8));
        GUILayout.Label(CollectionCtrl.Instance.LastExecuteFrame.ToString(), GUILayout.Width(vSize.x / 8));
        GUILayout.EndHorizontal();

        LvPlay lvPlay = Logic.Core.LevelManager.mMainLevel as LvPlay;
        if(lvPlay != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("CombatManager", GUILayout.Width(vSize.x / 3));
            GUILayout.Label(lvPlay.LastExecuteTime.ToString(), GUILayout.Width(vSize.x / 8));
            GUILayout.Label(lvPlay.LastExecuteFrame.ToString(), GUILayout.Width(vSize.x / 8));
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Npc", GUILayout.Width(vSize.x / 3));
        GUILayout.Label(GameCenter.npcsList.Count.ToString(), GUILayout.Width(vSize.x / 8));        
        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Hero", GUILayout.Width(vSize.x / 3));
        GUILayout.Label(GameCenter.otherActorList.Count.ToString(), GUILayout.Width(vSize.x / 8));
        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Label("MPartner", GUILayout.Width(vSize.x / 3));
        GUILayout.Label(GameCenter.partners.Count.ToString(), GUILayout.Width(vSize.x / 8));
        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Label("TeleporterActor", GUILayout.Width(vSize.x / 3));
        GUILayout.Label(GameCenter.teleports.Count.ToString(), GUILayout.Width(vSize.x / 8));
        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Label("LevelSystem", GUILayout.Width(vSize.x / 3));
        GUILayout.Label("microsecond", GUILayout.Width(vSize.x / 8));
        GUILayout.Label("frame", GUILayout.Width(vSize.x / 8));
        GUILayout.Label("offset", GUILayout.Width(vSize.x / 8));
        GUILayout.Label("interval", GUILayout.Width(vSize.x / 8));
        GUILayout.EndHorizontal();

        List<LevelSystemBase> levelSystems = Logic.Core.LevelManager.mMainLevel.mLevelSystems;
        if (levelSystems != null)
        {
            for (int i = 0, len = levelSystems.Count; i < len; ++i)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(levelSystems[i].name, GUILayout.Width(vSize.x / 3));
                GUILayout.Label(levelSystems[i].LastExecuteTime.ToString(), GUILayout.Width(vSize.x / 8));
                GUILayout.Label(levelSystems[i].LastExecuteFrame.ToString(), GUILayout.Width(vSize.x / 8));
                GUILayout.Label(TimeManager.CalRealOffsetFrame(levelSystems[i].offsetFrame).ToString(), GUILayout.Width(vSize.x / 8));
                GUILayout.Label(TimeManager.CalRealIntervalFrame(levelSystems[i].intervalFrame).ToString(), GUILayout.Width(vSize.x / 8));
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Label("SystemModule", GUILayout.Width(vSize.x / 3));
        GUILayout.Label("microsecond", GUILayout.Width(vSize.x / 8));
        GUILayout.Label("frame", GUILayout.Width(vSize.x / 8));
        GUILayout.Label("offset", GUILayout.Width(vSize.x / 8));
        GUILayout.Label("interval", GUILayout.Width(vSize.x / 8));
        GUILayout.EndHorizontal();

        IReadOnlyList<Logic.Core.ISystemModuleUpdate> systemModules = Logic.Core.SystemModuleManager.GetSystemModelUpdates();
        if (systemModules != null)
        {
            for (int i = 0, len = systemModules.Count; i < len; ++i)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(systemModules[i].GetType().ToString(), GUILayout.Width(vSize.x / 3));
                GUILayout.Label(systemModules[i].GetLastExecuteTime().ToString(), GUILayout.Width(vSize.x / 8));
                GUILayout.Label(systemModules[i].GetOffsetFrame().ToString(), GUILayout.Width(vSize.x / 8));
                GUILayout.Label(TimeManager.CalRealOffsetFrame(systemModules[i].GetOffsetFrame()).ToString(), GUILayout.Width(vSize.x / 8));
                GUILayout.Label(TimeManager.CalRealIntervalFrame(systemModules[i].GetIntervalFrame()).ToString(), GUILayout.Width(vSize.x / 8));
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndScrollView();
#endif
    }
}
