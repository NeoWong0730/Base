using Packet;
using UnityEngine;
using Logic.Core;
using Logic;
using Lib.AssetLoader;
using System.Collections.Generic;
using UnityEngine.UI;
using Table;
using System.Collections;
using System;
using Lib.Core;
using Framework;

namespace Logic
{
    public class UI_Ring : UIBase
    {

        private Vector3 consultPosition;
        private GameObject lightPromot;
        private float offest_x = 0;
        private float offest_y = 0;

        protected override void ProcessEvents(bool toRegister)
        {
            //Sys_LittleGame.Instance.eventEmitter.Handle<float, float, float>(Sys_LittleGame.EEvents.e_TriggerStoneIcon, OnShowLightPrompt, toRegister);
        }


        protected override void OnShow()
        {
            OnShowLightPrompt();
        }

        public void OnShowLightPrompt()
        {
            float x = float.Parse(CSVParam.Instance.GetConfData(297).str_value.Split('|')[0]) / 10000;
            float y = float.Parse(CSVParam.Instance.GetConfData(297).str_value.Split('|')[1]) / 10000;
            float z = float.Parse(CSVParam.Instance.GetConfData(297).str_value.Split('|')[2]) / 10000;

            consultPosition = new Vector3(x, y, z);
            offest_x = float.Parse(CSVParam.Instance.GetConfData(296).str_value.Split('|')[0]) / 10000;
            offest_y = float.Parse(CSVParam.Instance.GetConfData(296).str_value.Split('|')[1]) / 10000;
            GameObject obj = GlobalAssets.GetAsset<GameObject>(GlobalAssets.sPrefab_LightPrompt);
            lightPromot = GameObject.Instantiate<GameObject>(obj, transform);

            Button button = lightPromot.transform.Find("Button").GetComponent<Button>();
            ImageHelper.SetIcon(button.GetComponent<Image>(), uint.Parse(CSVParam.Instance.GetConfData(298).str_value));
            button.onClick.AddListener(() =>
            {
                GameObject.Destroy(lightPromot);
                lightPromot = null;
                Sys_Task.Instance.ReqStepGoalFinishEx(Sys_LittleGame_SeekItem.Instance._TaskId);
                UIManager.CloseUI(EUIID.UI_Ring);
            }
            );
        }

        protected override void OnLateUpdate(float dt, float usdt)
        {
            if (lightPromot)
            {
                CameraManager.World2UI(lightPromot, consultPosition + new Vector3(offest_x, offest_y, 0), CameraManager.mCamera, UIManager.mUICamera);
                (lightPromot.transform as RectTransform).position = new Vector3((lightPromot.transform as RectTransform).position.x,
                    (lightPromot.transform as RectTransform).position.y, UIManager.mUICamera.transform.position.z + 100);
            }
        }
    }
}


