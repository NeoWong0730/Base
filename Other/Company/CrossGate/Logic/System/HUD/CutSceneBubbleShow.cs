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
    public class CutSceneBubbleShow : IHUDComponent
    {
        public Transform target { get; private set; }
        public GameObject mRootGameobject;
        public Vector2 offset;
        private Vector3 offset_model;
        private EmojiText m_EmojiText;
        private Image m_Icon;
        private Text m_Name;
        private GameObject m_ArrowLeft;
        private GameObject m_ArrowRight;
        private GameObject m_ArrowMid;

        private Timer m_TextPlayTimer;
        private uint destroyTimer;
        private HUDPositionCorrect positionCorrect;

        private Action<CutSceneBubbleShow> m_OnRecycle;
        private Action m_OnComplete;
        private bool b_Valid = false;
        private CSVBubble.Data cSVBubbleData;

        public void Construct(GameObject gameObject)
        {
            b_Valid = true;
            mRootGameobject = gameObject;

            m_EmojiText = mRootGameobject.transform.Find("Text").GetComponent<EmojiText>();
            m_Icon = mRootGameobject.transform.Find("Image/Image_Icon").GetComponent<Image>();
            m_Name = mRootGameobject.transform.Find("Text_name").GetComponent<Text>();
            m_ArrowLeft = mRootGameobject.transform.Find("Arrow_L").gameObject;
            m_ArrowRight = mRootGameobject.transform.Find("Arrow_R").gameObject;
            m_ArrowMid = mRootGameobject.transform.Find("Arrow_M").gameObject;

            string vs = CSVParam.Instance.GetConfData(197).str_value;
            string[] s = vs.Split('|');
            offset_model = new Vector3(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]));

            positionCorrect = mRootGameobject.GetNeedComponent<HUDPositionCorrect>();
            positionCorrect.SetbaseOffest(offset_model);
        }

        public void SetData(uint bubbleId, uint _timer)
        {
            cSVBubbleData = CSVBubble.Instance.GetConfData(bubbleId);
            destroyTimer = _timer;

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
                    TextHelper.SetText(m_Name, Sys_Role.Instance.Role.Name.ToStringUtf8());
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
            target = _transform;
            positionCorrect.SetTarget(_transform);
        }

        public void SetCamera(Camera camera)
        {
            if (camera == null)
            {
                DebugUtil.LogErrorFormat("cutSceneCamera=null");
                return;
            }
            positionCorrect.SetCamera(camera);
        }

        public void CalNpcOffest(Vector3 _offest)
        {
            offset = _offest;
            positionCorrect.CalOffest(_offest);
            if (offset.x == 0)
            {
                m_ArrowMid.SetActive(true);
                m_ArrowLeft.SetActive(false);
                m_ArrowRight.SetActive(false);
            }
            if (offset.x > 0)
            {
                m_ArrowMid.SetActive(false);
                m_ArrowLeft.SetActive(false);
                m_ArrowRight.SetActive(true);
            }
            if (offset.x < 0)
            {
                m_ArrowMid.SetActive(false);
                m_ArrowLeft.SetActive(true);
                m_ArrowRight.SetActive(false);
            }
        }

        public void SetContent(string content, Action<CutSceneBubbleShow> onRecycle, Action complete = null)
        {
            m_OnRecycle = onRecycle;
            m_OnComplete = complete;
            m_EmojiText.text = content;
            m_TextPlayTimer?.Cancel();
            m_TextPlayTimer = Timer.Register(destroyTimer, OnBubbleRecycle);
        }

        private void OnBubbleRecycle()
        {
            m_OnComplete?.Invoke();
            m_OnRecycle?.Invoke(this);
        }

        public void Dispose()
        {
            if (m_Name)
            {
                m_Name.text = string.Empty;
            }
            if (m_EmojiText)
            {
                m_EmojiText.text = string.Empty;
            }
            m_TextPlayTimer?.Cancel();
            destroyTimer = 0;
            b_Valid = false;
            m_OnRecycle = null;
            m_OnComplete = null;
            positionCorrect.Dispose();
            mRootGameobject.SetActive(false);
        }
    }
}


