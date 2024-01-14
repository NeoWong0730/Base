using Logic.Core;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;
using Table;
using System.Collections.Generic;
using System;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Lib.Core;

namespace Logic
{
    public class UI_Loading : UIBase
    {
        Image loadingImage;
        Text tipText;
        Slider slider;
        Tweener fade;
        Tweener fadeTip;
        Text m_TxtProgress;

        private AsyncOperationHandle<Sprite> mHandle;

        private Timer timerTip;
        private List<uint> tipIds = new List<uint>();
        
        protected override void OnLoaded()
        {        
            loadingImage = gameObject.transform.Find("Image_Loading").GetComponent<Image>();
            tipText = gameObject.transform.Find("Text_Tips").GetComponent<Text>();
            slider = gameObject.transform.Find("Slider_Loading").GetComponent<Slider>();
            m_TxtProgress = gameObject.transform.Find("Slider_Loading/Text_Num").GetComponent<Text>();
        }        

        protected override void OnShow()
        {            
            slider.value = 0;
            TextHelper.SetText(m_TxtProgress, 10000, "0");

            loadingImage.color = Color.black;
            RefreshPanel();
            //RefreshPanel();
        }

        protected override void OnHide()
        {
            timerTip?.Cancel();
            timerTip = null;

            if (fade != null)
            {
                fade.Kill(true);
                fade = null;
            }

            if (fadeTip!= null)
            {
                fadeTip.Kill(true);
                fade = null;
            }

            loadingImage.color = Color.black;
            loadingImage.sprite = null;

            if (mHandle.IsValid())
            {
                AddressablesUtil.Release<Sprite>(ref mHandle, MHandle_Completed);
            }
        }

        protected override void OnUpdate()
        {
            float progress = LevelManager.fSwitchProgress;

            slider.value = progress;
            TextHelper.SetText(m_TxtProgress, 10000, ((int)(progress * 100)).ToString());
        }

        public void RefreshPanel()
        {
            //计算loading图
            string loadingPath = null;
            if (LevelManager.mSwitchLevelType == typeof(LvLogin))
            {
                loadingPath = CSVParam.Instance.GetConfData(10).str_value;
            }
            else if (LevelManager.mSwitchLevelType == typeof(LvCreateCharacter))
            {
                string[] strArray = CSVParam.Instance.GetConfData(11).str_value.Split('|');
                int index = UnityEngine.Random.Range(0, strArray.Length - 1);
                loadingPath = strArray[index];
            }
            else if (LevelManager.mSwitchLevelType == typeof(LvPlay))
            {
                loadingPath = CSVMapInfo.Instance.GetConfData(Sys_Map.Instance.CurMapId).picture;
            }

            if (loadingPath != null)
            {
                AddressablesUtil.LoadAssetAsync<Sprite>(ref mHandle, loadingPath, MHandle_Completed);
            }

            //计算tips
            uint id = CalTipsId();
            CSVTips.Data data = CSVTips.Instance.GetConfData(id);
            if (data != null)
            {
                tipIds.Clear();
                tipIds.AddRange(data.random_content);
                float fTime = data.random_time;
                timerTip?.Cancel();
                timerTip = Timer.Register(fTime, () => { 
                    
                    if (tipIds.Count > 0)
                    {
                        ShowTipText();
                    }
                    else
                    {
                        timerTip?.Cancel();
                    }

                }, null, true);

                ShowTipText();
            }
        }

        private void ShowTipText()
        {
            tipText.color = new Color(0, 0, 0, 0);
            int lanIndex = UnityEngine.Random.Range(0 * 100, tipIds.Count * 100) / 100;
            TextHelper.SetText(tipText, tipIds[lanIndex]);
            tipIds.RemoveAt(lanIndex);

            Color endValue = Color.white - tipText.color;
            Color to = new Color(0, 0, 0, 0);
            fadeTip?.Kill(true);
            fadeTip = DOTween.To(() => to, x =>
            {
                Color diff = x - to;
                to = x;
                tipText.color += diff;
            }, endValue, 0.5f)
           .Blendable().SetTarget(tipText);
        }

        private void MHandle_Completed(AsyncOperationHandle<Sprite> handle)
        {
            loadingImage.sprite = handle.Result;
            //fade = .DOBlendableColor(Color.white, 0.8f);
            Graphic target = loadingImage.GetComponent<Graphic>();
            Color endValue = Color.white - target.color;
            Color to = new Color(0, 0, 0, 0);
            fade = DOTween.To(() => to, x => {
                Color diff = x - to;
                to = x;
                target.color += diff;
            }, endValue, 0.5f)
           .Blendable().SetTarget(target);
        }

        private uint CalTipsId()
        {
            uint tipsId = 1;
            bool isHaveLevel = LevelManager.mSwitchLevelType == typeof(LvPlay);

            if (isHaveLevel)
            {
                uint level = Sys_Role.Instance.Role.Level;
                foreach (var data in CSVTipsLvl.Instance.GetAll())
                {
                    if (level <= data.lvl)
                    {
                        tipsId = data.lan_id;
                        break;
                    }
                }
            }

            return tipsId;
        }
    }
}
