using Table;
using Lib.Core;
using System.Collections.Generic;
using Packet;

namespace Logic
{
    public enum ECollectionSchemeType
    {
        Normal = 1,
        OpenUI = 2,
    }

    /// <summary>
    /// 采集功能///
    /// </summary>
    public class CollectionFunction : FunctionBase
    {
        public CSVCollection.Data CSVCollectionData
        {
            get;
            private set;
        }

        public CSVCollectionProcess.Data CSVCollectionProcessData
        {
            get;
            private set;
        }

        Timer collectBirthTimer;
        Timer collectLockTimer;
        Timer collectUnLockTimer;

        public override void Init()
        {
            CSVCollectionData = CSVCollection.Instance.GetConfData(ID);
            if (CSVCollectionData != null)
            {
                CSVCollectionProcessData = CSVCollectionProcess.Instance.GetConfData(CSVCollectionData.id);
            }

            Sys_CollectItem.Instance.eventEmitter.Handle<ulong>(Sys_CollectItem.EEvents.CollectBirth, OnCollectBirth, true);
            Sys_Task.Instance.eventEmitter.Handle<TaskEntry, ETaskState, ETaskState>(Sys_Task.EEvents.OnTaskStatusChanged, OnTaskStatusChanged, true);
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, OnRefreshChangeData, true);
            Sys_Npc.Instance.eventEmitter.Handle<uint>(Sys_Npc.EEvents.OnActiveNPC, OnActiveNPC, true);
        }

        public override void InitCompleted()
        {
            if (CanExecute(false))
            {
                status = Status.UnLock;
            }
            else
            {
                status = Status.Lock;
            }
        }

        protected override bool CanExecute(bool CheckVisual = true)
        {
            if (CSVCollectionData == null)
            {
                DebugUtil.LogError($"CSVCollection.Data is Null, id: {ID}");
                return false;
            }

            if (CSVCollectionProcessData == null)
            {
                DebugUtil.LogError($"CSVCollectionProcess.Data is Null, id: {ID}");
                return false;
            }

            if (CheckVisual)
            {
                if (npc == null || npc.VisualComponent == null)
                {
                    return false;
                }
            }

            ///判断可行性条件///
            if (CSVCollectionData.CollectionLimit != 0)
            {
                var cSVCheckseq = CSVCheckseq.Instance.GetConfData(CSVCollectionData.CollectionLimit);
                if (cSVCheckseq != null)
                {
                    if (!cSVCheckseq.IsValid())
                    {
                        return false;
                    }
                }
                else
                {
                    DebugUtil.LogError($"CSVCheckseq.Data is Null, id: {CSVCollectionData.CollectionLimit}");
                    return false;
                }
            }

            return true;
        }

        protected override void OnCantExecTip()
        {
            if (CSVCollectionData.CollectionLimitTips == 0)
                return;

            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(CSVCollectionData.CollectionLimitTips));
        }

        protected override void OnExecute()
        {
            base.OnExecute();

            if (CSVCollectionProcessData.collectionScheme[0] == (uint)ECollectionSchemeType.Normal)
            {
                CollectionAction collectionAction = ActionCtrl.Instance.CreateAction(typeof(CollectionAction)) as CollectionAction;
                if (collectionAction != null)
                {
                    collectionAction.CSVCollectionData = CSVCollectionData;
                    if (FunctionSourceType == EFunctionSourceType.Task)
                    {
                        collectionAction.taskId = HandlerID;
                    }
                    collectionAction.Init();
                    if (ActionCtrl.Instance.actionCtrlStatus == ActionCtrl.EActionCtrlStatus.PlayerCtrl)
                    {
                        ActionCtrl.Instance.ExecutePlayerCtrlAction(collectionAction);
                    }
                    else if (ActionCtrl.Instance.actionCtrlStatus == ActionCtrl.EActionCtrlStatus.Auto)
                    {
                        List<ActionBase> actionBases = new List<ActionBase>();
                        actionBases.Add(collectionAction);
                        ActionCtrl.Instance.AddAutoActions(actionBases);
                    }
                }
            }
            else if (CSVCollectionProcessData.collectionScheme[0] == (uint)ECollectionSchemeType.OpenUI)
            {
                UnlockCollectionAction unlockCollectionAction = ActionCtrl.Instance.CreateAction(typeof(UnlockCollectionAction)) as UnlockCollectionAction;
                if (unlockCollectionAction != null)
                {
                    unlockCollectionAction.CSVCollectionData = CSVCollectionData;
                    UnlockCollectionAction.unlockID = CSVCollectionProcessData.collectionScheme[1];
                    unlockCollectionAction.Init();
                    if (ActionCtrl.Instance.actionCtrlStatus == ActionCtrl.EActionCtrlStatus.PlayerCtrl)
                    {
                        ActionCtrl.Instance.ExecutePlayerCtrlAction(unlockCollectionAction);
                    }
                    else if (ActionCtrl.Instance.actionCtrlStatus == ActionCtrl.EActionCtrlStatus.Auto)
                    {
                        List<ActionBase> actionBases = new List<ActionBase>();
                        actionBases.Add(unlockCollectionAction);
                        ActionCtrl.Instance.AddAutoActions(actionBases);
                    }
                }
            }
        }

        protected override void OnDispose()
        {
            CSVCollectionData = null;
            CSVCollectionProcessData = null;
            collectBirthTimer?.Cancel();
            collectBirthTimer = null;
            collectLockTimer?.Cancel();
            collectLockTimer = null;
            collectUnLockTimer?.Cancel();
            collectUnLockTimer = null;
            status = Status.Lock;
            Sys_CollectItem.Instance.eventEmitter.Handle<ulong>(Sys_CollectItem.EEvents.CollectBirth, OnCollectBirth, false);
            Sys_Task.Instance.eventEmitter.Handle<TaskEntry, ETaskState, ETaskState>(Sys_Task.EEvents.OnTaskStatusChanged, OnTaskStatusChanged, false);
            Sys_Bag.Instance.eventEmitter.Handle<int,int>(Sys_Bag.EEvents.OnRefreshChangeData, OnRefreshChangeData, false);
            Sys_Npc.Instance.eventEmitter.Handle<uint>(Sys_Npc.EEvents.OnActiveNPC, OnActiveNPC, false);
            base.OnDispose();
        }

        enum Status
        {
            Lock,
            UnLock,
        }

        Status status = Status.Lock;

        void Check()
        {
            if (CanExecute())
            {
                if (status == Status.Lock)
                {
                    EffectUtil.Instance.UnloadEffectByTag(npc.uID, EffectUtil.EEffectTag.CollectLock);
                    EffectUtil.Instance.UnloadEffectByTag(npc.uID, EffectUtil.EEffectTag.CollectUnLock);

                    CSVEffect.Data collectUnLockEffectData = CSVEffect.Instance.GetConfData(CSVCollectionProcessData.collectionMayEffect);
                    if (collectUnLockEffectData != null)
                    {
                        collectUnLockTimer?.Cancel();
                        collectUnLockTimer = Timer.Register(0.1f, () =>
                        {
                            if (npc.VisualComponent != null && npc.VisualComponent.Visiable)
                            {
                                EffectUtil.Instance.UnloadEffectByTag(npc.uID, EffectUtil.EEffectTag.CollectLock);
                                EffectUtil.Instance.LoadEffect(npc.uID, collectUnLockEffectData.effects_path, npc.fxRoot.transform, EffectUtil.EEffectTag.CollectUnLock, collectUnLockEffectData.fx_duration / 1000f);
                            }
                        });
                    }
                }
                status = Status.UnLock;
            }
            else
            {
                if (status == Status.UnLock)
                {
                    EffectUtil.Instance.UnloadEffectByTag(npc.uID, EffectUtil.EEffectTag.CollectLock);
                    EffectUtil.Instance.UnloadEffectByTag(npc.uID, EffectUtil.EEffectTag.CollectUnLock);

                    CSVEffect.Data collectLockEffectData = CSVEffect.Instance.GetConfData(CSVCollectionProcessData.collectionNoMayEffect);
                    if (collectLockEffectData != null)
                    {
                        collectLockTimer?.Cancel();
                        collectLockTimer = Timer.Register(0.1f, () =>
                        {
                            if (npc.VisualComponent != null && npc.VisualComponent.Visiable)
                            {
                                EffectUtil.Instance.LoadEffect(npc.uID, collectLockEffectData.effects_path, npc.fxRoot.transform, EffectUtil.EEffectTag.CollectLock);
                            }
                        });
                    }
                }
                status = Status.Lock;
            }
        }

        void OnActiveNPC(uint npcId)
        {
            if (CSVCollectionData.CollectionLimit != 0)
            {
                Check();
            }
        }

        void OnTaskStatusChanged(TaskEntry taskEntry, ETaskState oldState, ETaskState newState)
        {
            Check();
        }

        void OnRefreshChangeData(int changeType, int curBoxId)
        {
            Check();
        }

        void OnCollectBirth(ulong uid)
        {
            if (npc == null)
                return;

            if (npc.uID == uid)
            {
                CSVEffect.Data collectBirthEffectData = CSVEffect.Instance.GetConfData(CSVCollectionProcessData.collectionBirthEffect);
                if (collectBirthEffectData != null)
                {
                    collectBirthTimer?.Cancel();
                    collectBirthTimer = Timer.Register(collectBirthEffectData.fx_duration / 1000f, TimerCallBack, null, false, true);
                }
            }
        }

        void TimerCallBack()
        {
            if (!CanExecute())
            {
                CSVEffect.Data collectLockEffectData = CSVEffect.Instance.GetConfData(CSVCollectionProcessData.collectionNoMayEffect);
                if (collectLockEffectData != null)
                {
                    if (npc.VisualComponent.Visiable)
                    {
                        EffectUtil.Instance.UnloadEffectByTag(npc.uID, EffectUtil.EEffectTag.CollectLock);
                        EffectUtil.Instance.LoadEffect(npc.uID, collectLockEffectData.effects_path, npc.fxRoot.transform, EffectUtil.EEffectTag.CollectLock, collectLockEffectData.fx_duration / 1000f);
                    }
                }
            }
        }
    }
}
