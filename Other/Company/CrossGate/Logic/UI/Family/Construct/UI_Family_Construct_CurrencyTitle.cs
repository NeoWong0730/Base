using Lib.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using System;
using Logic.Core;

namespace Logic
{

    public class UI_Family_Construct_CurrencyTitle
    {
        private GameObject mRoot;
        private Image mIconImage1;
        private Button mCurrency1;
        private Text mCountText1;
        CoroutineHandler mCoroutineHandler;

        public UI_Family_Construct_CurrencyTitle(GameObject _gameObject)
        {
            mRoot = _gameObject;
            ParseComponent();
            Sys_Family.Instance.eventEmitter.Handle<uint>(Sys_Family.EEvents.GuildStaminaChange, OnCurrencyChanged, true);
        }

        private void ParseComponent()
        {      
            mIconImage1 = mRoot.transform.Find("Image_Property01/Image_Icon").GetComponent<Image>();           
            mCurrency1 = mRoot.transform.Find("Image_Property01/Button_Add").GetComponent<Button>();            
            mCountText1 = mRoot.transform.Find("Image_Property01/Text_Number").GetComponent<Text>();           
        }

        public void InitUi()
        {
            //ImageHelper.SetIcon(mIconImage1, 910001);
            mCountText1.text = Sys_Bag.Instance.GetValueFormat(Sys_Bag.Instance.GetItemCount((uint)ECurrencyType.FamilyStamina));
        }

        private void OnCurrencyChanged(uint from)
        {            
            if (mCoroutineHandler != null)
            {
                CoroutineManager.Instance.Stop(mCoroutineHandler);
            }
            mCoroutineHandler = CoroutineManager.Instance.StartHandler(Refresh(from, Sys_Bag.Instance.GetItemCount((uint)ECurrencyType.FamilyStamina), 1, mCountText1));
        }

        private IEnumerator Refresh(long from, long to, float duration, Text text)
        {
            bool running = true;
            float accumulateTime = 0f;
            long currentValue = from;
            if (to < 10000000 && from < 10000000)
            {
                if (from >= to)
                {
                    currentValue = to;
                    text.text = currentValue.ToString();
                    yield break;
                }
                while (running)
                {
                    yield return new WaitForEndOfFrame();
                    accumulateTime += Time.deltaTime;
                    if (accumulateTime >= duration)
                    {
                        running = false;
                        currentValue = to;
                    }
                    else
                    {
                        currentValue = (long)Mathf.CeilToInt(Mathf.Lerp(from, to, accumulateTime / duration));
                    }
                    text.text = currentValue.ToString();
                }
            }
            else
            {
                text.text = Sys_Bag.Instance.GetValueFormat(to);
            }
        }

        public void Dispose()
        {
            Sys_Bag.Instance.eventEmitter.Handle<uint>(Sys_Bag.EEvents.OnCurrencyChanged, OnCurrencyChanged, false);
            if (mCoroutineHandler != null)
            {
                CoroutineManager.Instance.Stop(mCoroutineHandler);
            }
        }
    }
}


