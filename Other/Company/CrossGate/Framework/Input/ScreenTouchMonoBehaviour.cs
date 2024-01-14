using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class ScreenTouchMonoBehaviour : MonoBehaviour
    {
        public Action OnScreenTouchDown;

        void Start()
        {

        }

        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                OnScreenTouchDown?.Invoke();
            }
#elif UNITY_ANDROID || UNITY_IOS || UNITY_TVOS
            if(Input.touchCount > 0)
            {
                OnScreenTouchDown?.Invoke();
            }
#endif
        }
    }
}
