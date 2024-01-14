using UnityEngine;
using System;

#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
[DisallowMultipleComponent]
public class ReplaceListener : ReplaceComponent<Apple.PHASE.PHASEListener> {
    public override Action<int> replace {
        get { return DEFAULT_REPLACE; }
    }

    public static void DEFAULT_REPLACE(int arg){
        if(arg == 1 || arg == 3)
        {
            if(CP_AudioCollector.instance == null) { return; }
            foreach (var item in CP_AudioCollector.instance.ls) {
                if (item != null && item.phaseAudioSource != null) {
                    // re active PHASESource when change PHASEListener
                    item.phaseAudioSource.enabled = false;
                    item.phaseAudioSource.enabled = true;
                }
            }
        }
    }
}
#endif