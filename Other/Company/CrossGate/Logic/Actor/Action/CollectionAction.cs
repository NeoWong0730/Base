using Lib.Core;
using Table;
using Net;
using Packet;
using Logic.Core;
using System.Collections.Generic;

namespace Logic
{
    public class UnlockCollectionAction : CollectionAction
    {
        public new const string TypeName = "Logic.UnlockCollectionAction";

        public static uint unlockID;

        protected override void ProcessEvents(bool toRegister)
        {
            base.ProcessEvents(toRegister);

            Sys_QTE.Instance.eventEmitter.Handle<EQTESource>(Sys_QTE.EEvents.OnClose, OnQTEClose, toRegister);          
        }

        void OnQTEClose(EQTESource eQTESource)
        {
            if (eQTESource == EQTESource.Collect)
            {
                Interrupt();
            }
        }

        protected override void OnInterrupt()
        {
            Sys_QTE.Instance.CloseQTE(unlockID);
            GameCenter.mainHero.stateComponent.ChangeState(EStateType.Idle);

            base.OnInterrupt();
        }

        protected override void StartCollection()
        {
            CmdNpcResourceBeginReq beginReq = new CmdNpcResourceBeginReq();
            beginReq.UNpcId = Sys_Interactive.CurInteractiveNPC.uID;
            beginReq.PickType = (uint)ECollectionSchemeType.OpenUI;
            NetClient.Instance.SendMessage((ushort)CmdNpc.ResourceBeginReq, beginReq);
        }
    }

    /// <summary>
    /// 采集行为///
    /// </summary>
    public class CollectionAction : ActionBase
    {
        //public const string TypeName = "Logic.CollectionAction";

        public CSVCollection.Data CSVCollectionData
        {
            get;
            set;
        }

        public static CSVCollection.Data CurrentCSVCollectionData;

        public enum EEvents
        {
            StartCollect,
            InterrputCollect,
            CollectCompleted,
        }

        protected bool hasCollected;

        public static readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        protected override void ProcessEvents(bool toRegister)
        {
            base.ProcessEvents(toRegister);

            Sys_CollectItem.Instance.eventEmitter.Handle<ulong>(Sys_CollectItem.EEvents.OnCollectEnded, OnCollectEnded, toRegister);
            Sys_CollectItem.Instance.eventEmitter.Handle(Sys_CollectItem.EEvents.OnCollectFaild, OnCollectFaild, toRegister);
            Sys_Map.Instance.eventEmitter.Handle<uint>(Sys_Map.EEvents.OnTransTipStart, OnTransTipStart, toRegister);
            if (GameCenter.mainHero != null && GameCenter.mainHero.stateComponent != null)
            {
                if (toRegister)
                {
                    GameCenter.mainHero.stateComponent.StateChange += OnStateChange;
                }
                else
                {
                    GameCenter.mainHero.stateComponent.StateChange -= OnStateChange;
                }
            }
            Sys_Map.Instance.eventEmitter.Handle<uint, uint>(Sys_Map.EEvents.OnChangeMap, OnChangeMap, toRegister);
        }

        void OnChangeMap(uint lastMapId, uint curMapId)
        {
            CollectionAction.eventEmitter.Trigger(CollectionAction.EEvents.InterrputCollect);
        }

        protected override void OnDispose()
        {
            hasCollected = false;
            CSVCollectionData = null;
            //CurrentCSVCollectionData = null;

            base.OnDispose();
        }

        void OnStateChange(EStateType oldState, EStateType newState)
        {
            if (oldState == EStateType.Collection)
            {
                if (newState == EStateType.Run)
                {
                    Interrupt();
                }
            }
        }

        protected override void OnExecute()
        {            
            if (!CheckPowerValue())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13038));
                CollectionCtrl.Instance.CanCollection = false;
                CollectionCtrl.Instance.status = CollectionCtrl.ECollectionCtrlStatus.Idle;
                return;
            }

            if (!CheckCollectTime())
            {
                return;
            }

            if (!CheckSkill())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13039));
                return;
            }

            uint resID;
            if (Sys_FamilyResBattle.Instance.HasResource(out resID))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3251000008));
                return;
            }

            if (!CheckCostItems())
            {
                string tip = CSVLanguage.Instance.GetConfData(1030000001).words;

                foreach (var pair in CSVCollectionData.collectionExpendItem)
                {
                    if (pair[1] != 1)
                    {
                        tip += string.Format(CSVLanguage.Instance.GetConfData(1030000002).words, CSVLanguage.Instance.GetConfData((CSVItem.Instance.GetConfData(pair[0]).name_id)).words, pair[1].ToString());
                    }
                    else
                    {
                        tip += string.Format(CSVLanguage.Instance.GetConfData(1030000003).words, CSVLanguage.Instance.GetConfData((CSVItem.Instance.GetConfData(pair[0]).name_id)).words);
                    }
                }
                Sys_Hint.Instance.PushContent_Normal(tip);
                return;
            }
            GameCenter.mainHero.movementComponent.Stop();
            GameCenter.mainHero.stateComponent.CurrentState = EStateType.Collection;

            //UploadTransformComponent uploadTransformComponent = World.GetComponent<UploadTransformComponent>(GameCenter.mainHero);
            //UploadTransformComponent uploadTransformComponent = GameCenter.mainHero.uploadTransformComponent;
            //uploadTransformComponent.CanSendFlag = false;
            GameCenter.mUploadTransformSystem.CanSendFlag = false;

            hasCollected = false;
            CurrentCSVCollectionData = CSVCollectionData;

            StartCollection();
        }

        protected virtual void StartCollection()
        {
            if (Sys_Interactive.CurInteractiveNPC.uID != 0)
            {
                CmdNpcResourceBeginReq beginReq = new CmdNpcResourceBeginReq();
                beginReq.UNpcId = Sys_Interactive.CurInteractiveNPC.uID;
                beginReq.PickType = (uint)ECollectionSchemeType.Normal;
                NetClient.Instance.SendMessage((ushort)CmdNpc.ResourceBeginReq, beginReq);
            }
        }

        #region Check

        bool CheckPowerValue()
        {
            if (Sys_Bag.Instance.GetItemCount(5) >= CSVCollectionData.cost)
            {
                return true;
            }
            return false;
        }

        bool CheckCollectTime()
        {
            uint group = CSVCollectionData.CollectionGroup;
            if (CSVCollectionData.CollectionGroup == 0)
            {
                group = CSVCollectionData.id;
            }
            if (!Sys_CollectItem.Instance.collectItemTimes.ContainsKey(group) || Sys_CollectItem.Instance.collectItemTimes[group] < CSVCollectionData.ICollectionNum)
            {
                return true;
            }
            else
            {
                if (CSVCollectionData.ICollectionType == (uint)EICollectionLimitType.Day)
                {
                    Sys_Hint.Instance.PushContent_Normal(CSVLanguage.Instance.GetConfData(1020000001).words);
                    return false;
                }
                else if (CSVCollectionData.ICollectionType == (uint)EICollectionLimitType.Week)
                {
                    Sys_Hint.Instance.PushContent_Normal(CSVLanguage.Instance.GetConfData(1020000002).words);
                    return false;
                }
                else if (CSVCollectionData.ICollectionType == (uint)EICollectionLimitType.Month)
                {
                    Sys_Hint.Instance.PushContent_Normal(CSVLanguage.Instance.GetConfData(1020000003).words);
                    return false;
                }
                else if (CSVCollectionData.ICollectionType == (uint)EICollectionLimitType.Forever)
                {
                    Sys_Hint.Instance.PushContent_Normal(CSVLanguage.Instance.GetConfData(1020000004).words);
                    return false;
                }
                else
                {
                    return true;
                }
            }           
        }

        bool CheckSkill()
        {
            if (CSVCollectionData.level_collection == null)
            {
                return true;
            }

            if (Sys_LivingSkill.Instance.GetLifeSkillLevel(CSVCollectionData.level_collection[0]) >= CSVCollectionData.level_collection[1])
            {
                return true;
            }
            return false;
        }

        bool CheckCostItems()
        {
            if (CSVCollectionData.collectionExpendItem == null)
            {
                return true;
            }

            foreach (var pair in CSVCollectionData.collectionExpendItem)
            {
                if (Sys_Bag.Instance.GetItemCount(pair[0]) < pair[1])
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        protected override void OnInterrupt()
        {
            if (CurrentCSVCollectionData == null)
                return;

            Sys_CollectItem.Instance.collectTimer?.Cancel();

            CollectionCtrl.Instance.status = CollectionCtrl.ECollectionCtrlStatus.Idle;

            CSVCollectionProcess.Data cSVCollectionProcessData = CSVCollectionProcess.Instance.GetConfData(CurrentCSVCollectionData.id);
            if (cSVCollectionProcessData != null && cSVCollectionProcessData.collectingArms != 0)
            {
                GameCenter.mainHero.heroLoader.UnloadModelParts(EHeroModelParts.Weapon);
                GameCenter.mainHero.stateComponent.CurrentState = EStateType.Idle;
                GameCenter.mainHero.ChangeModel(true);
            }
            eventEmitter.Trigger(EEvents.InterrputCollect);

            //UploadTransformComponent uploadTransformComponent = World.GetComponent<UploadTransformComponent>(GameCenter.mainHero);
            //UploadTransformComponent uploadTransformComponent = GameCenter.mainHero.uploadTransformComponent;
            //uploadTransformComponent.CanSendFlag = true;
            GameCenter.mUploadTransformSystem.CanSendFlag = true;
        }

        protected override void OnCompleted()
        {
            CollectionCtrl.Instance.status = CollectionCtrl.ECollectionCtrlStatus.Idle;           
            eventEmitter.Trigger(EEvents.CollectCompleted);
        }

        void OnCollectFaild()
        {
            OnInterrupt();
            GameCenter.mainHero.stateComponent.ChangeState(EStateType.Idle);
            hasCollected = true;

            //OnCollectEnded(0);
        }

        void OnTransTipStart(uint id)
        {
            Interrupt();
        }

        private void OnCollectEnded(ulong guid)
        {
            CSVCollectionProcess.Data cSVCollectionProcessData = CSVCollectionProcess.Instance.GetConfData(CurrentCSVCollectionData.id);
            CSVEffect.Data cSVEffectData = CSVEffect.Instance.GetConfData(cSVCollectionProcessData.collectedEffect);
            if (cSVEffectData != null)
            {
                float yOffset = 0f;
                if (GameCenter.mainHero.heroBaseComponent.TitleId != 0)
                {
                    yOffset = 0.95f;
                }
                EffectUtil.Instance.LoadEffect(GameCenter.mainHero.uID, cSVEffectData.effects_path, GameCenter.mainHero.fxRoot.transform, EffectUtil.EEffectTag.Collection, cSVEffectData.fx_duration / 1000f, 1, 1, ELayerMask.Default, yOffset);
            }
            if (string.IsNullOrWhiteSpace(cSVCollectionProcessData.collectedAction))
            {
                GameCenter.mainHero.stateComponent.ChangeState(EStateType.Idle);
                if (cSVCollectionProcessData.collectedDialogue == 0)
                {
                    hasCollected = true;
                }
                else
                {
                    CSVDialogue.Data cSVDialogueData = CSVDialogue.Instance.GetConfData(cSVCollectionProcessData.collectedDialogue);
                    List<Sys_Dialogue.DialogueDataWrap> datas = Sys_Dialogue.GetDialogueDataWraps(cSVDialogueData);

                    ResetDialogueDataEventData resetDialogueDataEventData = PoolManager.Fetch(typeof(ResetDialogueDataEventData)) as ResetDialogueDataEventData;
                    //ResetDialogueDataEventData resetDialogueDataEventData = Logic.Core.ObjectPool<ResetDialogueDataEventData>.Fetch(typeof(ResetDialogueDataEventData));
                    resetDialogueDataEventData.Init(datas, () =>
                    {
                        UIManager.CloseUI(EUIID.UI_Dialogue);
                        hasCollected = true;
                    }, cSVDialogueData);
                    Sys_Dialogue.Instance.OpenDialogue(resetDialogueDataEventData);
                }

                if (cSVCollectionProcessData != null && cSVCollectionProcessData.collectingArms != 0)
                {
                    GameCenter.mainHero.heroLoader.UnloadModelParts(EHeroModelParts.Weapon);
                    GameCenter.mainHero.stateComponent.CurrentState = EStateType.Idle;
                    GameCenter.mainHero.ChangeModel(true);
                }
            }
            else
            {
                GameCenter.mainHero.animationComponent.Play(cSVCollectionProcessData.collectedAction, () =>
                {
                    GameCenter.mainHero.stateComponent.ChangeState(EStateType.Idle);
                    if (cSVCollectionProcessData.collectedDialogue == 0)
                    {
                        hasCollected = true;
                    }
                    else
                    {
                        CSVDialogue.Data cSVDialogueData = CSVDialogue.Instance.GetConfData(cSVCollectionProcessData.collectedDialogue);
                        List<Sys_Dialogue.DialogueDataWrap> datas = Sys_Dialogue.GetDialogueDataWraps(cSVDialogueData);

                        ResetDialogueDataEventData resetDialogueDataEventData = PoolManager.Fetch(typeof(ResetDialogueDataEventData)) as ResetDialogueDataEventData;
                        //ResetDialogueDataEventData resetDialogueDataEventData = Logic.Core.ObjectPool<ResetDialogueDataEventData>.Fetch(typeof(ResetDialogueDataEventData));
                        resetDialogueDataEventData.Init(datas, () =>
                        {
                            UIManager.CloseUI(EUIID.UI_Dialogue);
                            hasCollected = true;
                        }, cSVDialogueData);
                        Sys_Dialogue.Instance.OpenDialogue(resetDialogueDataEventData);
                    }

                    if (cSVCollectionProcessData != null && cSVCollectionProcessData.collectingArms != 0)
                    {
                        GameCenter.mainHero.heroLoader.UnloadModelParts(EHeroModelParts.Weapon);
                        GameCenter.mainHero.stateComponent.CurrentState = EStateType.Idle;
                        GameCenter.mainHero.ChangeModel(true);
                    }
                });
            }
          
            //UploadTransformComponent uploadTransformComponent = World.GetComponent<UploadTransformComponent>(GameCenter.mainHero);
            //UploadTransformComponent uploadTransformComponent = GameCenter.mainHero.uploadTransformComponent;
            //uploadTransformComponent.CanSendFlag = true;
            GameCenter.mUploadTransformSystem.CanSendFlag = true;
        }

        public override bool IsCompleted()
        {
            return hasCollected;
        }
    }
}
