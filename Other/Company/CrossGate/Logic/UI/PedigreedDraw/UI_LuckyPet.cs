using Lib.Core;
using Logic.Core;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Table;
using UnityEngine.EventSystems;
using Packet;
using System.Text;

namespace Logic
{
    public class UI_LuckyPet : UIBase
    {
        private GameObject m_AnimRoot;
        private GameObject m_InfoRoot;
        private Button m_CloseButton;
        private Image m_PetIcon;
        private Text m_PetName;
        private Transform m_LotteryCodeParent;

        private uint m_LotteryCode;
        private FinalAwardInfo m_FinalAwardInfo;
        private List<int> m_Values = new List<int>();
        private List<Slot> m_Slots = new List<Slot>();
        private Slot m_CurSlot;
        private int m_Round;
        private float m_Interval;
        private float m_SlotUpdateRecordTime;

        private Timer m_PauseToSweepTimer;
        private Timer m_StartTimer;
        private Timer m_SlotUpdateTimer;
        private Timer m_StartFx3Timer;
        private Timer m_Fx3UpdateTimer;
        private Timer m_EndPlayTimer;

        private float m_PauseToSweepTime;
        private float m_StartTime;
        private bool b_Start;
        private bool b_End;
        private float m_FxIntervalTime;
        private float m_Fx3UpdateRecodeTime;
        private float m_WaitShowPlayInfoTime;
        private int m_StartFx3PlayIndex = 0;

        private GameObject m_Fx1;
        private GameObject m_Fx2;
        private GameObject m_Fx3;

        private Text m_FinnalPlayer;
        private Image m_PlayerHead;
        private Image m_PlayerFrame;

        protected override void OnInit()
        {
            //按照30帧跑120/30
            SetIntervalFrame(4);

            m_PauseToSweepTime = uint.Parse(CSVPedigreedParameter.Instance.GetConfData(9).str_value);
            m_StartTime = uint.Parse(CSVPedigreedParameter.Instance.GetConfData(10).str_value);
            m_FxIntervalTime = float.Parse(CSVPedigreedParameter.Instance.GetConfData(11).str_value);
            m_WaitShowPlayInfoTime = (int.Parse(CSVPedigreedParameter.Instance.GetConfData(12).str_value)) / 100f;
        }

        protected override void OnOpen(object arg)
        {
            m_LotteryCode = (uint)arg;
            string value = Sys_PedigreedDraw.Instance.HandleLotteryCode(m_LotteryCode);
            
            for (int i = 0; i < value.Length; i++)
            {
                m_Values.Add((value[i] - '0'));
            }

            m_Round = 0;
        }

        protected override void OnLoaded()
        {
            m_PlayerHead = transform.Find("Animator/Type2/Head").GetComponent<Image>();
            m_PlayerFrame = transform.Find("Animator/Type2/Head/Image_Before_Frame").GetComponent<Image>();
            m_FinnalPlayer = transform.Find("Animator/Type2/Name").GetComponent<Text>();
            m_LotteryCodeParent = transform.Find("Animator/Type1/Root");
            m_CloseButton = transform.Find("Animator/Btn_Close").GetComponent<Button>();
            m_PetIcon = transform.Find("Animator/Award/PetIcon").GetComponent<Image>();
            m_PetName = transform.Find("Animator/Award/BG (1)/Name").GetComponent<Text>();
            m_AnimRoot = transform.Find("Animator/Type1").gameObject;
            m_InfoRoot = transform.Find("Animator/Type2").gameObject;
            m_Fx1 = transform.Find("Animator/RawImage/FX_UI_LuckPet_01").gameObject;
            m_Fx2 = transform.Find("Animator/RawImage/FX_UI_LuckPet_02").gameObject;
            m_Fx3 = transform.Find("Animator/RawImage/Fx_UI_LuckyPet_03").gameObject;

            int count = m_Values.Count;
            FrameworkTool.CreateChildList(m_LotteryCodeParent, count);
            for (int i = 0; i < count; i++)
            {
                Slot slot = new Slot();
                slot.BindGameObject(m_LotteryCodeParent.GetChild(i).gameObject);
                slot.SetData(i, m_Values[i]);
                m_Slots.Add(slot);
            }
            m_CurSlot = m_Slots[m_Round];
            m_SlotUpdateRecordTime = Time.time;

            m_CloseButton.onClick.AddListener(OnCloseButtonClicked);

            m_SlotUpdateTimer?.Cancel();
            m_SlotUpdateTimer = Timer.Register(100, null, Update);

            m_StartTimer?.Cancel();
            m_StartTimer = Timer.Register(m_StartTime, Start);

            m_AnimRoot.SetActive(true);
            m_InfoRoot.SetActive(false);
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            if (Sys_PedigreedDraw.Instance.cSVPedigreedDrawData != null)
            {
                uint petId = Sys_PedigreedDraw.Instance.cSVPedigreedDrawData.Show_Item;
                CSVPetNew.Data cSVPetNewData = CSVPetNew.Instance.GetConfData(petId);
                if (cSVPetNewData != null)
                {
                    ImageHelper.SetIcon(m_PetIcon, cSVPetNewData.icon_id);
                    TextHelper.SetText(m_PetName, cSVPetNewData.name);
                }
            }
        }

        private void Start()
        {
            b_Start = true;
            m_Fx1.SetActive(true);
        }

        protected override void OnClose()
        {
            m_PauseToSweepTimer?.Cancel();
            m_StartTimer?.Cancel();
            m_SlotUpdateTimer?.Cancel();
            m_StartFx3Timer?.Cancel();
            m_Fx3UpdateTimer?.Cancel();
            m_EndPlayTimer?.Cancel();
            m_Slots.Clear();
        }

        private void Update(float dt)
        {
            if (m_Slots.Count == 0 || !b_Start)
            {
                return;
            }
            for (int i = 0; i < m_Slots.Count; i++)
            {
                m_Slots[i].Update();
            }
            if (Time.time - m_SlotUpdateRecordTime > m_Interval)
            {
                if (m_CurSlot.Reach())
                {
                    m_CurSlot.Stop();
                    if (m_Slots.Count > 0)
                    {
                        m_Slots.RemoveAt(m_Round);
                        if (m_Slots.Count > 0)
                        {
                            m_CurSlot = m_Slots[m_Round];
                        }
                        else
                        {
                            EndSlotLoop();
                        }
                    }
                    m_SlotUpdateRecordTime = Time.time;
                }
            }
        }

        private void EndSlotLoop()
        {
            m_Fx1.SetActive(false);
            //停顿一段时间之后扫光
            m_PauseToSweepTimer?.Cancel();
            m_PauseToSweepTimer = Timer.Register(m_PauseToSweepTime, SweepLight);
        }

        private void SweepLight()
        {
            m_Fx2.SetActive(true);
            m_StartFx3Timer?.Cancel();
            m_StartFx3Timer = Timer.Register(m_WaitShowPlayInfoTime, ShowPlayerInfo);
        }

        private void ShowPlayerInfo()
        {
            m_Fx2.SetActive(false);
            m_AnimRoot.SetActive(false);
            m_InfoRoot.SetActive(true);

            m_Fx3.SetActive(true);
            m_Fx3UpdateTimer?.Cancel();
            m_Fx3UpdateTimer = Timer.Register(20, null, UpdateFx3Play);
            m_Fx3UpdateRecodeTime = Time.time;

            if (Sys_PedigreedDraw.Instance.finalAwardInfo != null)
            {
                m_InfoRoot.SetActive(true);
                TextHelper.SetText(m_FinnalPlayer, Sys_PedigreedDraw.Instance.finalAwardInfo.RoleName.ToStringUtf8());

                uint headID = CharacterHelper.getHeadID(Sys_PedigreedDraw.Instance.finalAwardInfo.HeroId, Sys_PedigreedDraw.Instance.finalAwardInfo.HeadId);
                ImageHelper.SetIcon(m_PlayerHead, headID);

                uint frameID = CharacterHelper.getHeadFrameID(Sys_PedigreedDraw.Instance.finalAwardInfo.HeadFrameId);
                if (frameID > 0)
                {
                    m_PlayerFrame.gameObject.SetActive(true);
                    ImageHelper.SetIcon(m_PlayerFrame, frameID);
                }
                else
                {
                    m_PlayerFrame.gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.Log("finalAwardInfo=null");
                m_InfoRoot.SetActive(false);
            }
        }

        private void UpdateFx3Play(float dt)
        {
            if (b_End)
            {
                return;
            }
            if (m_StartFx3PlayIndex == m_Fx3.transform.childCount)
            {
                EndFx3LoopPlay();
            }
            if (Time.time - m_Fx3UpdateRecodeTime > m_FxIntervalTime)
            {
                m_Fx3.transform.GetChild(m_StartFx3PlayIndex).gameObject.SetActive(true);
                m_Fx3UpdateRecodeTime = Time.time;
                m_StartFx3PlayIndex++;
            }
        }

        private void EndFx3LoopPlay()
        {
            b_End = true;
            m_EndPlayTimer?.Cancel();
            m_EndPlayTimer = Timer.Register(1, CloseSelf);
        }

        private void CloseSelf()
        {
            UIManager.CloseUI(EUIID.UI_LuckyPet);
        }

        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_LuckyPet);
        }

        public class Slot
        {
            private RawImage m_RawImage;
            private GameObject m_GameObject;
            public int index;
            public float targetValue;
            private Rect m_Rect;
            private float m_Speed;
            private float m_YOffest;
            private float k = 0.1f;
            private float b = 0.4f;
            private bool b_Perform = true;

            public void Stop()
            {
                b_Perform = false;
                m_RawImage.uvRect = new Rect(m_Rect.x, targetValue / 10f, m_Rect.width, m_Rect.height);
            }

            public void BindGameObject(GameObject gameObject)
            {
                m_GameObject = gameObject;

                m_RawImage = m_GameObject.transform.Find("Num").GetComponent<RawImage>();
                m_Rect = m_RawImage.uvRect;
            }

            public void SetData(int dataIndex, int targetValue)
            {
                this.index = dataIndex;
                this.targetValue = targetValue;
                m_Speed = k * index + b;
            }

            public void Update()
            {
                if (!b_Perform)
                {
                    return;
                }
                m_YOffest += Sys_PedigreedDraw.Instance.GetDeltaTime() * m_Speed;
                if (m_YOffest >= 10)
                {
                    m_YOffest = 0;
                }
                m_RawImage.uvRect = new Rect(m_Rect.x, m_YOffest / 10f, m_Rect.width, m_Rect.height);
            }

            public bool Reach()
            {
                if (Mathf.Abs(m_YOffest - targetValue) < 0.15f)
                {
                    m_YOffest = targetValue;
                    m_RawImage.uvRect = new Rect(m_Rect.x, m_YOffest / 10f, m_Rect.width, m_Rect.height);
                    return true;
                }
                return false;
            }
        }
    }
}


