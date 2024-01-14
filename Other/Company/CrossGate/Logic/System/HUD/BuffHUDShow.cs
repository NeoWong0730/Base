using Lib.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using Logic.Core;
using DG.Tweening;
using System;

namespace Logic
{
    public class BuffHUDShow : IHUDComponent
    {
        private int side;
        public GameObject mRootGameObject;
        private RectTransform rectTransform
        {
            get
            {
                return mRootGameObject.transform as RectTransform;
            }
        }

        private Transform target;
        private GameObject icon;
        private Vector3 offest;
        private List<uint> buffids = new List<uint>();
        /// <summary> buff剩余计数 key：buffId | value：计数值 </summary>
        private Dictionary<uint, uint> dictBuffCount = new Dictionary<uint, uint>();
        private CanvasGroup mCanvasGroup;
        private float showTime;
        private float hideTime;
        private float intervel;
        private float fadeTime;
        private Timer timer;

        private List<uint> willRemoved = new List<uint>();

        private HUDPositionCorrect positionCorrect;

        public void Construct(GameObject gameObject, int _side)
        {
            if (gameObject == null)
            {
                DebugUtil.LogErrorFormat("gameObject=null");
                return;
            }
            mRootGameObject = gameObject;
            ParseCp();
            string[] val = CSVParam.Instance.GetConfData(632).str_value.Split('|');
            offest = new Vector3(float.Parse(val[0]) / 100, float.Parse(val[1]) / 100, float.Parse(val[2]) / 100);
            string[] _val = CSVParam.Instance.GetConfData(642).str_value.Split('|');
            hideTime = float.Parse(_val[0]) / 1000;
            showTime = float.Parse(_val[1]) / 1000;
            intervel = float.Parse(_val[2]) / 1000;
            fadeTime = float.Parse(_val[3]) / 1000;
            side = _side;
            positionCorrect = mRootGameObject.GetNeedComponent<HUDPositionCorrect>();
            positionCorrect.CalOffest(offest);

            rectTransform.localScale = Vector3.one;
        }

        public void SetTarget(Transform transform)
        {
            positionCorrect.SetTarget(transform);
        }


        public void UpdateBuffIcon(uint buffId, bool add, uint count = 0)
        {
            bool _add = false;
            CSVBuff.Data cSVBuffData = CSVBuff.Instance.GetConfData(buffId);
            if (cSVBuffData.hud_icon != 0)
            {
                _add = true;
            }
            if (add)
            {
                if (!buffids.Contains(buffId) && _add)
                {
                    buffids.Add(buffId);
                }
                if (cSVBuffData.hud_icon_num == 1)
                {
                    UpdateBuffCountData(buffId, count);
                }
                UpdateBuffIcon();
            }
            else
            {
                if (buffids.Contains(buffId))
                {
                    Flash(buffId, EndFlash);
                    buffids.Remove(buffId);
                }
            }
        }

        private void EndFlash(uint buffId)
        {
            UpdateBuffIcon();
        }

        public void UpdateBuffIcon()
        {
            int childCount = mRootGameObject.transform.childCount;
            int buffDataCount = buffids.Count;
            FrameworkTool.CreateChildList(mRootGameObject.transform, buffDataCount);
            for (int i = 0; i < buffDataCount; i++)
            {
                Image image = mRootGameObject.transform.GetChild(i).GetComponent<Image>();
                Color color = image.color;
                image.color = new Color(color.r, color.g, color.b, 1);
                SetIcon(image, buffids[i]);

                GameObject goBuffCount = mRootGameObject.transform.GetChild(i).transform.Find("Image").gameObject;
                if(dictBuffCount.TryGetValue(buffids[i],out uint count) && count > 0)
                {
                    goBuffCount.SetActive(true);
                    Text txtCount = mRootGameObject.transform.GetChild(i).transform.Find("Image/Num").GetComponent<Text>();
                    txtCount.text = count.ToString();
                }
                else
                {
                    goBuffCount.SetActive(false);
                }
            }
        }

        public void Flash(uint buffId, Action<uint> endFade)
        {
            int index = buffids.IndexOf(buffId);
            Image image = mRootGameObject.transform.GetChild(index).GetComponent<Image>();
            ColorFade colorFade = image.gameObject.GetNeedComponent<ColorFade>();
            Color color0 = new Color(image.color.r, image.color.g, image.color.b, 0);
            Color color1 = new Color(image.color.r, image.color.g, image.color.b, 1);

            GameObject goBuffCount = mRootGameObject.transform.GetChild(index).transform.Find("Image").gameObject;
            goBuffCount.SetActive(false);
            colorFade.StartFlash(buffId, FadeMode.Light2Dark, color0, color1, intervel, fadeTime, showTime, hideTime, endFade);
        }


        public void ShowOrHide(bool flag)
        {
            if (!flag)
            {
                rectTransform.localScale = Vector3.zero;
            }
            else
            {
                rectTransform.localScale = Vector3.one;
            }
        }


        private void SetIcon(Image image, uint buffid)
        {
            uint iconId = CSVBuff.Instance.GetConfData(buffid).hud_icon;
            ImageHelper.SetIcon(image, iconId);
        }


        private void ParseCp()
        {
            icon = mRootGameObject.transform.Find("icon").gameObject;
        }

        public void Dispose()
        {
            positionCorrect.Dispose();
            buffids.Clear();
            willRemoved.Clear();
            mRootGameObject.SetActive(false);
        }

        private void UpdateBuffCountData(uint buffId, uint count)
        {
            if(dictBuffCount.TryGetValue(buffId,out uint buffCount))
            {
                dictBuffCount[buffId] = count;
            }
            else
            {
                dictBuffCount.Add(buffId, count);
            }
        }
    }
}

