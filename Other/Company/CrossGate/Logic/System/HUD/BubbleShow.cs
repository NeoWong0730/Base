using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using Lib.Core;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Table;
using Packet;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

namespace Logic
{
    public class BubbleShow : IHUDComponent
    {
        public uint battleId { get; private set; }
        private Transform target;
        public GameObject mRootGameobject;
        private Vector3 offset_model;

        private Timer m_TextPlayTimer;
        private uint destroyTimer;
        private Vector2 rootOffest;

        private HUDPositionCorrect positionCorrect;

        private Action<BubbleShow> m_OnRecycle;
        private Action m_OnComplete;
        private bool b_Valid = false;

        private Image m_Bg;
        private Image m_Arrow;

        public RectTransform bg         //bg
        {
            get
            {
                if (mRootGameobject == null)
                {
                    return null;
                }
                return mRootGameobject.transform as RectTransform;
            }
        }

        public EmojiText mEmojiText
        {
            get
            {
                if (mRootGameobject == null)
                {
                    return null;
                }
                return mRootGameobject.transform.Find("Text").GetComponent<EmojiText>();
            }
        }

        private Text templeteText;
        TextGenerator _textGenerator = null;
        TextGenerationSettings _textGenerationSettings;

        private float _templeteTextWeight;
        TweenerCore<string, string, StringOptions> _tweenerCore;


        public void Construct(GameObject gameObject, Text templete, uint _timer, Vector2 _rootOffest, uint _battleId = 0, int _clientNum = -1)
        {
            b_Valid = true;
            mRootGameobject = gameObject;
            destroyTimer = _timer;
            battleId = _battleId;

            string s = CSVParam.Instance.GetConfData(528).str_value;
            string[] s1 = s.Split('|');
            string res = s1[_clientNum];
            string res1 = res.Split('&')[1];
            offset_model = new Vector3(0, float.Parse(res1), 0);
            rootOffest = _rootOffest;

            templeteText = templete;
            if (templeteText == null)
            {
                DebugUtil.LogErrorFormat("BubbleShow.Construct==>templeteText=null");
                return;
            }

            RectTransform templateTextRT = templeteText.transform as RectTransform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(templateTextRT);
            _textGenerator = templeteText.cachedTextGenerator;
            _textGenerationSettings = templeteText.GetGenerationSettings((templeteText.transform as RectTransform).sizeDelta);
            _textGenerationSettings.richText = true;

            _textGenerationSettings.scaleFactor = 1f;            _textGenerationSettings.horizontalOverflow = HorizontalWrapMode.Wrap;
            positionCorrect = mRootGameobject.GetNeedComponent<HUDPositionCorrect>();
            positionCorrect.SetbaseOffest(offset_model);
            positionCorrect.SetuiRootOffest(rootOffest);

            m_Bg = mRootGameobject.transform.Find("Image").GetComponent<Image>();
            m_Arrow = mRootGameobject.transform.Find("Image/Image_Arrow").GetComponent<Image>();

            _templeteTextWeight = (templeteText.transform as RectTransform).sizeDelta.x;
        }


        public void SetContent(int type, string content, float bubbleTextInterval, ChatType chatType, Action<BubbleShow> onRecycle, Action complete = null)
        {
            m_OnRecycle = onRecycle;
            m_OnComplete = complete;
            bg.sizeDelta = new Vector2(CalculateWSize(content), CalculateHSize(content));
            if (type == 1)//战斗内玩家喊话
            {
                SetBgColor(chatType);
                mEmojiText.text = content;
                OnTextPlayEnded();
            }
            else if (type == 2)//ai气泡
            {
                float NeetTime = (content.Length - 1) * bubbleTextInterval;
                _tweenerCore = mEmojiText.DOText(content, NeetTime);
                _tweenerCore.onComplete = OnTextPlayEnded;
            }
        }

        private void SetBgColor(ChatType chatType)
        {
            Color color = Color.white;
            switch (chatType)
            {
                case ChatType.Local:
                    color = Sys_HUD.Instance.current_ChatColor;
                    break;

                case ChatType.World:
                    color = Sys_HUD.Instance.world_ChatColor;
                    break;

                case ChatType.Team:
                    color = Sys_HUD.Instance.team_ChatColor;
                    break;

                case ChatType.Guild:
                    color = Sys_HUD.Instance.guild_ChatColor;
                    break;

                case ChatType.Career:
                    color = Sys_HUD.Instance.career_ChatColor;
                    break;

                case ChatType.BraveGroup:
                    color = Sys_HUD.Instance.braveTeam_ChatColor;
                    break;

                default:
                    break;
            }

            m_Bg.color = color;
            m_Arrow.color = color;
        }

        private void OnTextPlayEnded()
        {
            if (b_Valid)
            {
                m_TextPlayTimer?.Cancel();
                m_TextPlayTimer = Timer.Register(destroyTimer, OnBubbleRecycle);
            }
        }

        private void OnBubbleRecycle()
        {
            m_OnComplete?.Invoke();
            m_OnRecycle?.Invoke(this);
        }


        private float CalculateHSize(string content)
        {
            float h = _textGenerator.GetPreferredHeight(content, _textGenerationSettings);
            return h;
        }

        private float CalculateWSize(string content)
        {
            float w = _textGenerator.GetPreferredWidth(content, _textGenerationSettings);
            w = Mathf.Min(w, _templeteTextWeight);
            return w + 8;
        }

        public void SetTarget(Transform _transform)
        {
            positionCorrect.SetTarget(_transform);
            target = _transform;
        }


        public void Dispose()
        {
            m_TextPlayTimer?.Cancel();
            _tweenerCore?.Kill();
            m_Bg.color = Color.white;
            m_Arrow.color = Color.white;
            b_Valid = false;
            m_OnRecycle = null;
            m_OnComplete = null;
            if (mEmojiText)
            {
                mEmojiText.text = string.Empty;
            }
            positionCorrect.Dispose();
            mRootGameobject?.SetActive(false);
        }
    }
}

