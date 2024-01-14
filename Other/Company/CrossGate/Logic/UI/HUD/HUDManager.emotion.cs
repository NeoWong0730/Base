using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using System;
using Table;
using Lib.Core;
using System.Text;

namespace Logic
{
    public partial class HUD : UIBase
    {
        public void CreateEmotion(CreateEmotionEvt createEmotionEvt)
        {
            if (createEmotionEvt.actorId != 0)
            {
                ShowOrHideActorHUD_Icon(createEmotionEvt.actorId, false);
                if (emotionhuds.TryGetValue(createEmotionEvt.gameObject,out EmotionShow _emotionShow))
                {
                    RecyleEmotionHud(_emotionShow);
                }
            }  
            GameObject go;
            EmotionShow  emotionShow;
            go = EmotionPools.Get(root_Emotion);
            go.SetActive(true);
#if UNITY_EDITOR
            go.name = createEmotionEvt.gameObject.name;
#endif
            emotionShow = HUDFactory.Get<EmotionShow>();
            emotionShow.Construct(go,createEmotionEvt.emtionId, RecyleEmotionHud,()=>
            {
                if (createEmotionEvt.actorId!=0)
                {
                    ShowOrHideActorHUD_Icon(createEmotionEvt.actorId, true);
                }
            }
            );
            emotionShow.SetTarget(createEmotionEvt.gameObject.transform);
            if (!emotionhuds.ContainsKey(createEmotionEvt.gameObject))
            {
                emotionhuds.Add(createEmotionEvt.gameObject, null);
            }
            emotionhuds[createEmotionEvt.gameObject] = emotionShow;
        }

        public void ClearEmotion()
        {
            Dictionary<GameObject, EmotionShow>.Enumerator enumerator4 = emotionhuds.GetEnumerator();
            while (enumerator4.MoveNext())
            {
                EmotionShow  emotionShow= enumerator4.Current.Value;
                emotionShow?.Dispose();
            }
            emotionhuds.Clear();
        }


        public void RecyleEmotionHud(EmotionShow emotionShow)
        {
            if (emotionShow == null)
                return;
            emotionShow.onComplete?.Invoke();
            emotionhuds[emotionShow.target.gameObject] = null;
            emotionShow.Dispose();
            HUDFactory.Recycle(emotionShow);
            EmotionPools.Recovery(emotionShow.mRootGameObject);
        }
    }
}

