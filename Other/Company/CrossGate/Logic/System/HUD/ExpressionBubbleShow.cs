using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using Lib.Core;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Table;

namespace Logic
{
    public class ExpressionBubbleShow : IHUDComponent
    {
        public ulong actorId { get; private set; }
        private Transform target;
        public GameObject mRootGameobject;
        private EmojiText m_EmojiText;
        private Image m_Icon;
        private Text m_Name;

        public Vector2 offset;
        private Vector3 offset_model;
        private Timer timer;
        private float destroyTimer;
        private HUDPositionCorrect positionCorrect;

        private Action<ExpressionBubbleShow> m_OnRecycle;
        private bool b_Valid = false;
        private CSVBubble.Data cSVBubbleData;


        public void Construct(GameObject gameObject)
        {
            b_Valid = true;
            mRootGameobject = gameObject;

            m_EmojiText = mRootGameobject.transform.Find("Text").GetComponent<EmojiText>();
            m_Icon = mRootGameobject.transform.Find("Image/Image_Icon").GetComponent<Image>();
            m_Name = mRootGameobject.transform.Find("Text_name").GetComponent<Text>();
            string vs = CSVParam.Instance.GetConfData(197).str_value;
            string[] s = vs.Split('|');
            offset_model = new Vector3(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]));

            positionCorrect = mRootGameobject.GetNeedComponent<HUDPositionCorrect>();
            positionCorrect.SetbaseOffest(offset_model);
        }

        public void SetData(uint bubbleId, float _timer, ulong _actorId = 0)
        {
            cSVBubbleData = CSVBubble.Instance.GetConfData(bubbleId);
            destroyTimer = _timer;
            actorId = _actorId;

            UpdateInfo();
        }

        private void UpdateInfo()
        {
            uint iconId = 0;
            if (cSVBubbleData != null)
            {
                if (cSVBubbleData.PeopleType == 0)
                {
                    uint headId = Sys_Role.Instance.HeroId * 10 + cSVBubbleData.CharacterHead;
                    CSVCharacterHead.Data cSVCharacterHeadData = CSVCharacterHead.Instance.GetConfData(headId);
                    if (cSVCharacterHeadData != null)
                    {
                        iconId = cSVCharacterHeadData.IconId_Li;
                    }
                    TextHelper.SetText(m_Name, (uint)CSVCharacter.Instance.GetConfData(Sys_Role.Instance.HeroId).name);
                }
                else if (cSVBubbleData.PeopleType == 1)
                {
                    CSVNPCHead.Data cSVNPCHeadData = CSVNPCHead.Instance.GetConfData(cSVBubbleData.NPCHead);
                    if (cSVNPCHeadData != null)
                    {
                        iconId = cSVNPCHeadData.IconId_Li;
                    }
                    uint npcId = cSVBubbleData.NPCHead / 10;
                    TextHelper.SetText(m_Name, LanguageHelper.GetNpcTextContent(CSVNpc.Instance.GetConfData(npcId).name));
                }
            }
            ImageHelper.SetIcon(m_Icon, iconId);
        }

        public void SetTarget(Transform _transform)
        {
            positionCorrect.SetTarget(_transform);
        }

        public void CalChatOffest(Vector3 _offest)
        {
            positionCorrect.CalOffest(_offest);
        }

        public void SetChatContent(string content, Action<ExpressionBubbleShow> onRecycle)
        {
            m_OnRecycle = onRecycle;
            m_EmojiText.text = content;
            timer = Timer.Register(destroyTimer, OnBubbleRecycle);
        }

        private void OnBubbleRecycle()
        {
            m_OnRecycle?.Invoke(this);
        }

        public void Dispose()
        {
            b_Valid = false;
            m_OnRecycle = null;
            positionCorrect.Dispose();
            if (m_EmojiText)
            {
                m_EmojiText.text = string.Empty;
            }
            timer?.Cancel();
            mRootGameobject.SetActive(false);
        }
    }
}


