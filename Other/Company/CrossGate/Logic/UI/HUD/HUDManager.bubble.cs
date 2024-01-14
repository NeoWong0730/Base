using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using System;
using Table;
using Lib.Core;
using Packet;

namespace Logic
{
    public partial class HUD : UIBase
    {
        private Timer CutSceneBubbleTimer;
        private Timer NpcBubbleTimer;

        private Dictionary<uint, Timer> m_BattleBubbleTimer = new Dictionary<uint, Timer>();
        private Dictionary<uint, BattleBubbleData> m_BattleBubbleDatas = new Dictionary<uint, BattleBubbleData>();

        #region CutSceneBubble
        public void CreateCutSceneBubble(TriggerCutSceneBubbleEvt triggerCutSceneBubbleEvt)
        {
            GameObject _go = triggerCutSceneBubbleEvt.gameObject;

            if (cutSceneBubblehuds.TryGetValue(triggerCutSceneBubbleEvt.gameObject, out CutSceneBubbleShow cutSceneBubbleShow) && cutSceneBubbleShow != null)
            {
                RecyleCutSceneBubble(cutSceneBubbleShow);
            }
            CSVBubble.Data cSVBubbleData = CSVBubble.Instance.GetConfData(triggerCutSceneBubbleEvt.bubbleid);

            CutSceneBubbleTimer?.Cancel();
            float delayTime = (float)cSVBubbleData.BubbleDelay / 1000f;
            DebugUtil.LogFormat(ELogType.eHUD, "CreateCutSceneBubble");
            CutSceneBubbleTimer = Timer.Register(delayTime, () => ShowCutSceneBubbleDelay(cSVBubbleData, _go, triggerCutSceneBubbleEvt.offest,
                triggerCutSceneBubbleEvt.onComplete, triggerCutSceneBubbleEvt.camera));
        }

        private void ShowCutSceneBubbleDelay(CSVBubble.Data _cSVBubbleData, GameObject _go, Vector3 _offest, Action oncomplete, Camera camera)
        {
            DebugUtil.LogFormat(ELogType.eHUD, "ShowCutSceneBubbleDelay");
            float bubbleTextInterval = (float)_cSVBubbleData.BubbleTextInterval / 1000f;
            string content = CSVLanguage.Instance.GetConfData(_cSVBubbleData.BubbleText).words;
            GameObject go;
            go = CutScenePools.Get(root_CutSceneBubble);
            go.SetActive(true);
            CutSceneBubbleShow cutSceneBubbleShow = HUDFactory.Get<CutSceneBubbleShow>();
            uint destroyDelayTimer = _cSVBubbleData.BubbleTime;

            cutSceneBubbleShow.Construct(go);
            cutSceneBubbleShow.CalNpcOffest(_offest);
            cutSceneBubbleShow.SetData(_cSVBubbleData.id, _cSVBubbleData.BubbleTime);
            cutSceneBubbleShow.SetContent(content, RecyleCutSceneBubble, () =>
            {
                oncomplete?.Invoke();
            });
            cutSceneBubbleShow.SetTarget(_go.transform);
            cutSceneBubbleShow.SetCamera(camera);
            cutSceneBubblehuds[_go] = cutSceneBubbleShow;
            DebugUtil.LogFormat(ELogType.eHUD, "_ShowCutSceneBubbleDelay");
        }

        #endregion

        #region NpcBubble
        public void CreateNpcBubble(TriggerNpcBubbleEvt triggerNpcBubbleEvt)
        {
            if (OptionManager.Instance.GetBoolean(OptionManager.EOptionID.SceneBubble))
            {
                return;
            }
            ShowOrHideActorHUD_Icon(triggerNpcBubbleEvt.npcid, false);

            GameObject _go = triggerNpcBubbleEvt.npcobj;

            if (npcBubbleHuds.TryGetValue(triggerNpcBubbleEvt.npcid, out NpcBubbleShow curNpcBubbleShow) && curNpcBubbleShow != null)
            {
                RecyleNpcBubble(curNpcBubbleShow);
            }
            CSVBubble.Data cSVBubbleData = CSVBubble.Instance.GetConfData(triggerNpcBubbleEvt.bubbleid);
            NpcBubbleTimer?.Cancel();
            float delayTime = (float)cSVBubbleData.BubbleDelay / 1000f;

            NpcBubbleTimer = Timer.Register(delayTime, () => ShowNpcBubbleDelay(triggerNpcBubbleEvt.npcid, triggerNpcBubbleEvt.ownerType,
                triggerNpcBubbleEvt.playInfoId, triggerNpcBubbleEvt.npcInfoId, cSVBubbleData, _go, triggerNpcBubbleEvt.onComplete));
        }

        private void ShowNpcBubbleDelay(ulong npcId, uint ownerType, uint playerInfoId, uint npcInfoId, CSVBubble.Data _cSVBubbleData, GameObject _go, Action oncomplete)
        {
            float bubbleTextInterval = (float)_cSVBubbleData.BubbleTextInterval / 1000f;

            string content = CSVLanguage.Instance.GetConfData(_cSVBubbleData.BubbleText).words;
            GameObject go;
            go = NpcBubblePools.Get(root_NpcBubble);
            if (go == null)
            {
                DebugUtil.LogErrorFormat("HUD.ShowNpcBubbleDelay==>go=null");
                return;
            }
            go.SetActive(true);
            NpcBubbleShow bubbleShow = HUDFactory.Get<NpcBubbleShow>();
            uint destroyDelayTimer = _cSVBubbleData.BubbleTime;
            List<int> lists = new List<int>();
            if (ownerType == 0)
            {
                for (int i = 0; i < CSVNpc.Instance.GetConfData((uint)npcInfoId).signPositionShifting.Count; i++)
                {
                    lists.Add(CSVNpc.Instance.GetConfData((uint)npcInfoId).signPositionShifting[i]);
                }
            }
            else if (ownerType == 1)
            {
                for (int i = 0; i < CSVCharacter.Instance.GetConfData((uint)playerInfoId).signPositionShifting.Count; i++)
                {
                    lists.Add((int)CSVCharacter.Instance.GetConfData((uint)playerInfoId).signPositionShifting[i]);
                }
            }
            Vector3 npcoffest = new Vector3((float)lists[0] / 10000, (float)lists[1] / 10000, (float)lists[2] / 10000);
            bubbleShow.Construct(go, templete_NormalEmojiText, _tansformWorldToScreen, destroyDelayTimer, npcId);
            bubbleShow.CalNpcOffest(npcoffest);
            bubbleShow.SetContent(content, bubbleTextInterval, RecyleNpcBubble, () =>
              {
                  ShowOrHideActorHUD_Icon(npcId, true);
                  oncomplete?.Invoke();
              });
            if (_go != null)
            {
                bubbleShow.SetTarget(_go.transform);
            }
            else
            {
                DebugUtil.LogErrorFormat("gameobject不存在");
            }
            npcBubbleHuds[npcId] = bubbleShow;
        }
        #endregion

        #region ExpressionBubble
        public void CreateExpressionBubble(TriggerExpressionBubbleEvt triggerPlayerChatBubbleEvt)
        {
            GameObject _go;
            if (triggerPlayerChatBubbleEvt.gameObject != null)
            {
                _go = triggerPlayerChatBubbleEvt.gameObject;
            }
            else if (!actorObjs.TryGetValue(triggerPlayerChatBubbleEvt.id, out _go))
            {
                DebugUtil.LogErrorFormat("场景中未能找到actor  ,id={0}", triggerPlayerChatBubbleEvt.id);
                return;
            }
            ExpressionBubbleShow curExpressionBubbleShow = null;
            if (expressionBubbleHuds.TryGetValue(triggerPlayerChatBubbleEvt.id, out curExpressionBubbleShow) && curExpressionBubbleShow != null)
            {
                RecyleExpressionBubble(curExpressionBubbleShow);
            }
            GameObject go;
            go = ExpressionBubblePools.Get(root_ExpressionBubble);
            go.SetActive(true);

            float destroyDelayTimer = triggerPlayerChatBubbleEvt.showTime;
            Vector3 offest = Vector3.zero;
            if (triggerPlayerChatBubbleEvt.ownerType == 0)                  //npc
            {
                List<int> lists = CSVNpc.Instance.GetConfData(triggerPlayerChatBubbleEvt.npcInfoId).signPositionShifting;
                offest = new Vector3((float)lists[0] / 10000, (float)lists[1] / 10000, (float)lists[2] / 10000);
            }
            else if (triggerPlayerChatBubbleEvt.ownerType == 1)             //player
            {
                string str = CSVParam.Instance.GetConfData(510).str_value;
                string[] s = str.Split('|');
                offest = new Vector3(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]));
            }
            ExpressionBubbleShow bubbleShow = HUDFactory.Get<ExpressionBubbleShow>();
            bubbleShow.Construct(go);
            bubbleShow.CalChatOffest(offest);
            bubbleShow.SetTarget(_go.transform);
            bubbleShow.SetData(triggerPlayerChatBubbleEvt.bubbleId, destroyDelayTimer, triggerPlayerChatBubbleEvt.id);
            bubbleShow.SetChatContent(triggerPlayerChatBubbleEvt.content, RecyleExpressionBubble);
            expressionBubbleHuds[triggerPlayerChatBubbleEvt.id] = bubbleShow;
        }

        #endregion

        #region PlayerChat
        public void CreatePlayerChatBubble(TriggerPlayerChatBubbleEvt triggerPlayerChatBubbleEvt)
        {
            if (OptionManager.Instance.GetBoolean(OptionManager.EOptionID.SceneBubble))
            {
                return;
            }

            GameObject _go;
            if (!actorObjs.TryGetValue(triggerPlayerChatBubbleEvt.id, out _go))
            {
                DebugUtil.LogErrorFormat("场景中未能找到actor  ,id={0}", triggerPlayerChatBubbleEvt.id);
                return;
            }
            PlayerChatBubbleShow playerChatBubbleShow = null;
            if (playerChatBubbleHuds.TryGetValue(triggerPlayerChatBubbleEvt.id, out playerChatBubbleShow) && playerChatBubbleShow != null)
            {
                RecylePlayerChatBubble(playerChatBubbleShow);
            }
            GameObject go;
            go = PlayerChatBubblePools.Get(root_PlayerChatBubble);
            go.SetActive(true);

            float destroyDelayTimer = triggerPlayerChatBubbleEvt.showTime;
            Vector3 vector3 = Vector3.zero;
            string str = CSVParam.Instance.GetConfData(510).str_value;
            string[] s = str.Split('|');
            vector3 = new Vector3(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]));

            PlayerChatBubbleShow bubbleShow = HUDFactory.Get<PlayerChatBubbleShow>();
            bubbleShow.Construct(go, templete_EmojiText, destroyDelayTimer, triggerPlayerChatBubbleEvt.id);
            bubbleShow.CalChatOffest(vector3);
            bubbleShow.SetChatContent(triggerPlayerChatBubbleEvt.content, triggerPlayerChatBubbleEvt.chatType, RecylePlayerChatBubble);
            bubbleShow.SetTarget(_go.transform);
            playerChatBubbleHuds[triggerPlayerChatBubbleEvt.id] = bubbleShow;
        }
        #endregion

        #region BattleBubble

        public void CreateBattleBubble(TriggerBattleBubbleEvt triggerBattleBubbleEvt)
        {
            if (OptionManager.Instance.GetBoolean(OptionManager.EOptionID.FightBubble))
            {
                return;
            }
            if (triggerBattleBubbleEvt.bubbleid != 0 && !Sys_HUD.Instance.battleHuds.ContainsKey(triggerBattleBubbleEvt.battleid))//表示需要冒战斗ai气泡
            {
                DebugUtil.LogErrorFormat("战斗单位{0}已被删除，无法触发气泡", triggerBattleBubbleEvt.battleid);
                return;
            }
            if (battleBubbleHuds.TryGetValue(triggerBattleBubbleEvt.battleid, out BubbleShow curbattleBubbleShow) && curbattleBubbleShow != null)
            {
                RecyleBattleBubble(curbattleBubbleShow);
            }
            CSVBubble.Data cSVBubbleData = CSVBubble.Instance.GetConfData(triggerBattleBubbleEvt.bubbleid);
            int type = 0;
            float delayTime = 0;
            float bubbleTextInterval = 0;
            string content = string.Empty;
            Vector2 offest = Vector2.zero;
            uint destroyDelayTimer = 0;
            int clientNum = triggerBattleBubbleEvt.ClientNum;
            ChatType chatType = ChatType.Local;

            if (cSVBubbleData != null)          //战斗内ai气泡
            {
                if (cSVBubbleData.Type == 2)
                {
                    CreateEmotionEvt createEmotionEvt = new CreateEmotionEvt();
                    createEmotionEvt.battleId = triggerBattleBubbleEvt.battleid;
                    createEmotionEvt.gameObject = Sys_HUD.Instance.battleHuds[triggerBattleBubbleEvt.battleid];
                    createEmotionEvt.emtionId = cSVBubbleData.MoodId;
                    Sys_HUD.Instance.eventEmitter.Trigger<CreateEmotionEvt>(Sys_HUD.EEvents.OnCreateEmotion, createEmotionEvt);
                    return;
                }
                type = 2;
                delayTime = (float)cSVBubbleData.BubbleDelay / 1000f;
                bubbleTextInterval = (float)cSVBubbleData.BubbleTextInterval / 1000f;
                destroyDelayTimer = cSVBubbleData.BubbleTime;
                offest = new Vector2(cSVBubbleData.offset[0] / 1000, cSVBubbleData.offset[1] / 1000);
                content = CSVLanguage.Instance.GetConfData(cSVBubbleData.BubbleText).words;
            }
            else   //战斗内玩家聊天气泡
            {
                type = 1;
                delayTime = 0;
                bubbleTextInterval = 0;
                offest = Vector2.zero;
                destroyDelayTimer = uint.Parse(CSVParam.Instance.GetConfData(511).str_value) / 10000;
                content = triggerBattleBubbleEvt.content;
                chatType = triggerBattleBubbleEvt.chatType;
            }

            if (!m_BattleBubbleDatas.TryGetValue(triggerBattleBubbleEvt.battleid, out BattleBubbleData battleBubbleData))
            {
                battleBubbleData = new BattleBubbleData();
                m_BattleBubbleDatas.Add(triggerBattleBubbleEvt.battleid, battleBubbleData);
            }
            battleBubbleData.SetData(delayTime, type, triggerBattleBubbleEvt.battleid, bubbleTextInterval, content, destroyDelayTimer, offest,
                     Sys_HUD.Instance.battleHuds[triggerBattleBubbleEvt.battleid], triggerBattleBubbleEvt.ClientNum, triggerBattleBubbleEvt.chatType);

            if (battleBubbleData.m_DelayTime == 0)
            {
                ShowBubbleDelay(type, triggerBattleBubbleEvt.battleid, bubbleTextInterval,
                   content, destroyDelayTimer, offest,
                  Sys_HUD.Instance.battleHuds[triggerBattleBubbleEvt.battleid], triggerBattleBubbleEvt.ClientNum, chatType);
            }
            else
            {
                if (m_BattleBubbleTimer.TryGetValue(triggerBattleBubbleEvt.battleid, out Timer timer))
                {
                    timer?.Cancel();
                    timer = Timer.Register(battleBubbleData.m_DelayTime, () => ShowBubbleDelay(battleBubbleData));
                }
                else
                {
                    timer = Timer.Register(battleBubbleData.m_DelayTime, () => ShowBubbleDelay(battleBubbleData));
                }
            }
        }

        private void ShowBubbleDelay(BattleBubbleData battleBubbleData)
        {
            if (!Sys_HUD.Instance.battleHuds.ContainsKey(battleBubbleData.m_BattleId))
            {
                DebugUtil.LogErrorFormat("战斗单位{0}已被删除，无法触发气泡", battleBubbleData.m_BattleId);
                return;
            }
            ShowBubbleDelay(battleBubbleData.m_Type, battleBubbleData.m_BattleId, battleBubbleData.m_BubbleTextInterval,
                    battleBubbleData.m_Content, battleBubbleData.m_DestroyDelayTimer, battleBubbleData.m_Offest,
                   Sys_HUD.Instance.battleHuds[battleBubbleData.m_BattleId], battleBubbleData.m_ClientNum, battleBubbleData.m_ChatType);
        }

        private void ShowBubbleDelay(int type, uint battleId, float _bubbleTextInterval, string _content, uint _destroyDelayTimer, Vector2 _offest, GameObject _go, int _clientNum, ChatType _chatType)
        {
            GameObject go;
            go = BattleBubblePools.Get(root_BattleBubble);
            if (go == null)
            {
                DebugUtil.LogErrorFormat("HUD.ShowBubbleDelay==>go=null");
                return;
            }
            if (_go == null)
            {
                DebugUtil.LogErrorFormat("HUD.ShowBubbleDelay==>_go=null");
                return;
            }
            BubbleShow bubbleShow = HUDFactory.Get<BubbleShow>();
            bubbleShow.Construct(go, templete_EmojiText, _destroyDelayTimer, _offest, battleId, _clientNum);
            bubbleShow.SetContent(type, _content, _bubbleTextInterval, _chatType, RecyleBattleBubble);
            bubbleShow.SetTarget(_go.transform);
            go.SetActive(true);
            battleBubbleHuds[battleId] = bubbleShow;
        }

        #endregion

        public void RecyleBattleBubble(BubbleShow bubbleShow)
        {
            if (bubbleShow == null)
                return;
            bubbleShow.Dispose();
            HUDFactory.Recycle(bubbleShow);
            BattleBubblePools.Recovery(bubbleShow.mRootGameobject);
            battleBubbleHuds.Remove(bubbleShow.battleId);
        }

        public void RecyleExpressionBubble(ExpressionBubbleShow bubbleShow)
        {
            if (bubbleShow == null)
                return;
            bubbleShow.Dispose();
            HUDFactory.Recycle(bubbleShow);
            ExpressionBubblePools.Recovery(bubbleShow.mRootGameobject);
            expressionBubbleHuds.Remove(bubbleShow.actorId);
        }

        public void RecylePlayerChatBubble(PlayerChatBubbleShow bubbleShow)
        {
            if (bubbleShow == null)
                return;
            bubbleShow.Dispose();
            HUDFactory.Recycle(bubbleShow);
            PlayerChatBubblePools.Recovery(bubbleShow.mRootGameobject);
            playerChatBubbleHuds.Remove(bubbleShow.actorId);
        }

        public void RecyleNpcBubble(NpcBubbleShow bubbleShow)
        {
            if (bubbleShow == null)
                return;
            bubbleShow.Dispose();
            HUDFactory.Recycle(bubbleShow);
            NpcBubblePools.Recovery(bubbleShow.mRootGameobject);
            npcBubbleHuds.Remove(bubbleShow.actorId);
        }

        public void RecyleCutSceneBubble(CutSceneBubbleShow bubbleShow)
        {
            if (bubbleShow == null)
                return;
            bubbleShow.Dispose();
            HUDFactory.Recycle(bubbleShow);
            CutScenePools.Recovery(bubbleShow.mRootGameobject);
            cutSceneBubblehuds.Remove(bubbleShow.target.gameObject);
        }


        public class BattleBubbleData
        {
            public int m_Type;
            public uint m_BattleId;
            public float m_BubbleTextInterval;
            public string m_Content;
            public uint m_DestroyDelayTimer;
            public Vector2 m_Offest;
            public GameObject m_Go;
            public int m_ClientNum;
            public ChatType m_ChatType;

            public float m_DelayTime;

            public void SetData(float _delayTime, int type, uint battleId, float _bubbleTextInterval, string _content, uint _destroyDelayTimer, Vector2 _offest, GameObject _go, int _clientNum, ChatType _chatType)
            {
                m_DelayTime = _delayTime;
                m_Type = type;
                m_BattleId = battleId;
                m_BubbleTextInterval = _bubbleTextInterval;
                m_Content = _content;
                m_DestroyDelayTimer = _destroyDelayTimer;
                m_Offest = _offest;
                m_Go = _go;
                m_ClientNum = _clientNum;
                m_ChatType = _chatType;
            }
        }
    }
}

