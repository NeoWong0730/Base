using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;

public static class RuntimeOnLoad
{
    /*
    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeInitializeOnLoadMethod()
    {
        Debug.Log("OnRuntimeInitializeOnLoadMethod");

        PlayerLoopSystem playerLoopSystem = PlayerLoop.GetCurrentPlayerLoop();
        for (int i = 0; i < playerLoopSystem.subSystemList.Length; ++i)
        {
            if (string.Equals("UnityEngine.PlayerLoop.Update", playerLoopSystem.subSystemList[i].type.ToString()))
            {
                PlayerLoopSystem system = new PlayerLoopSystem();
                system.type = typeof(MainUpdateHacker);
                system.updateDelegate = MainUpdate;

                List<PlayerLoopSystem> subSystem = new List<PlayerLoopSystem>(playerLoopSystem.subSystemList[i].subSystemList);
                subSystem.Insert(0, system);

                playerLoopSystem.subSystemList[i].subSystemList = subSystem.ToArray();
                PlayerLoop.SetPlayerLoop(playerLoopSystem);
                break;
            }
        }

        playerLoopSystem = PlayerLoop.GetCurrentPlayerLoop();
        foreach (var group in playerLoopSystem.subSystemList)
        {
            Debug.Log($"->{group.type.ToString()}");
            foreach (var item in group.subSystemList)
            {
                Debug.Log($"->->{item.type.ToString()}");
            }
        }
    }

    static void MainUpdate()
    {
        Debug.Log("MainUpdate");
    }

    public struct MainUpdateHacker
    {

    }
    */
}
