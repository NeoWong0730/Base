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

namespace Logic
{
    public class PlayerChatBubbleShow : IHUDComponent
    {
        public ulong actorId { get; private set; }

        private Transform target;

        public GameObject mRootGameobject;

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

        private Text templeteText;


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



        TextGenerator _textGenerator = null;

        TextGenerationSettings _textGenerationSettings;

        private float _templeteTextWeight;

        public Vector2 offset;
        private Vector3 offset_model;
        private Timer timer;
        private float destroyTimer;
        private HUDPositionCorrect positionCorrect;

        private Action<PlayerChatBubbleShow> m_OnRecycle;
        private bool b_Valid = false;

        private Image m_Bg;
        private Image m_Arrow;

        public void Construct(GameObject gameObject, Text templete, float _timer, ulong _actorId = 0)
        {
            b_Valid = true;
            mRootGameobject = gameObject;
            destroyTimer = _timer;
            templeteText = templete;
            actorId = _actorId;
            string vs = CSVParam.Instance.GetConfData(197).str_value;
            string[] s = vs.Split('|');
            offset_model = new Vector3(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]));

            positionCorrect = mRootGameobject.GetNeedComponent<HUDPositionCorrect>();
            positionCorrect.SetbaseOffest(offset_model);

            RectTransform templateTextRT = templeteText.transform as RectTransform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(templateTextRT);
            _textGenerator = templeteText.cachedTextGenerator;
            _textGenerationSettings = templeteText.GetGenerationSettings((templeteText.transform as RectTransform).sizeDelta);
            _textGenerationSettings.richText = true;

            _textGenerationSettings.scaleFactor = 1f;            _textGenerationSettings.horizontalOverflow = HorizontalWrapMode.Wrap;            //_textGenerationSettings.pivot = new Vector2(1, 1);

            m_Bg = mRootGameobject.transform.Find("Image").GetComponent<Image>();
            m_Arrow = mRootGameobject.transform.Find("Image/Image_Arrow").GetComponent<Image>();

            _templeteTextWeight = (templeteText.transform as RectTransform).sizeDelta.x;
        }

        public void SetTarget(Transform _transform)
        {
            positionCorrect.SetTarget(_transform);
        }

        public void CalChatOffest(Vector3 _offest)
        {
            positionCorrect.CalOffest(_offest);
        }


        public void SetChatContent(string content, ChatType chatType, Action<PlayerChatBubbleShow> onRecycle)
        {
            m_OnRecycle = onRecycle;
            SetBgColor(chatType);
            bg.sizeDelta = new Vector2(CalculateWSize(content), CalculateHSize(content));
            mEmojiText.text = content;
            timer = Timer.Register(destroyTimer, OnBubbleRecycle);
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


        private void OnBubbleRecycle()
        {
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


        public void Dispose()
        {
            m_Bg.color = Color.white;
            m_Arrow.color = Color.white;
            b_Valid = false;
            m_OnRecycle = null;
            positionCorrect.Dispose();
            if (mEmojiText)
            {
                mEmojiText.text = string.Empty;
            }
            timer?.Cancel();
            mRootGameobject?.SetActive(false);
        }
    }
}


