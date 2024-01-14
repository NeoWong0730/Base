using Lib.Core;
using UnityEngine;
using System.Collections.Generic;
using Table;

namespace Logic
{
    public class VisualComponent : Logic.Core.Component
    {
        public bool DefaultVisiable
        {
            get;
            set;
        }

        private bool _visiable;
        public bool Visiable
        {
            get
            {
                return _visiable;
            }
            set
            {
                if (value == true)
                {
                    npc.transform.Setlayer(ELayerMask.NPC);
                    Component collider;
                    if (npc.modelGameObject != null && npc.modelGameObject.TryGetComponent(typeof(Collider), out collider))
                    {
                        ((Collider)collider).enabled = true;
                    }
                    ShowOrHideActorHUDEvt showOrHideActorHUDEvt = new ShowOrHideActorHUDEvt();
                    showOrHideActorHUDEvt.id = npc.uID;
                    showOrHideActorHUDEvt.flag = true;
                    Sys_HUD.Instance.eventEmitter.Trigger<ShowOrHideActorHUDEvt>(Sys_HUD.EEvents.OnShowOrHideActorHUD, showOrHideActorHUDEvt);                    
                }
                else
                {
                    npc.transform.Setlayer(ELayerMask.HidingSceneActor);
                    Component collider;
                    if (npc.modelGameObject != null && npc.modelGameObject.TryGetComponent(typeof(Collider), out collider))
                    {
                        ((Collider)collider).enabled = false;
                    }
                    ShowOrHideActorHUDEvt showOrHideActorHUDEvt = new ShowOrHideActorHUDEvt();
                    showOrHideActorHUDEvt.id = npc.uID;
                    showOrHideActorHUDEvt.flag = false;
                    Sys_HUD.Instance.eventEmitter.Trigger<ShowOrHideActorHUDEvt>(Sys_HUD.EEvents.OnShowOrHideActorHUD, showOrHideActorHUDEvt);
                }
                _visiable = value;
            }
        }

        private Npc npc;

        private List<ConditionBase> listConditions = new List<ConditionBase>(8);
         //Dictionary<string, ConditionBase> conditions = new Dictionary<string, ConditionBase>();

        protected override void OnConstruct()
        {
            base.OnConstruct();
            npc = actor as Npc;

            if (npc.cSVNpcData.ShowState == 0)
            {
                DefaultVisiable = true;
                _visiable = true;
            }
            else
            {
                DefaultVisiable = false;
                _visiable = false;
            }

            ProcessEvents(true);

            //Checking();
        }

        void ProcessEvents(bool toRegister)
        {
            Sys_Task.Instance.eventEmitter.Handle<TaskEntry, ETaskState, ETaskState>(Sys_Task.EEvents.OnTaskStatusChanged, OnTaskStatusChanged, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int,int>(Sys_Bag.EEvents.OnRefreshChangeData, OnRefreshChangeData, toRegister);
            Sys_Weather.Instance.eventEmitter.Handle(Sys_Weather.EEvents.OnWeatherChange, OnWeatherChange, toRegister);
            Sys_Weather.Instance.eventEmitter.Handle(Sys_Weather.EEvents.OnDayNightChange, OnDayNightChange, toRegister);
            Sys_Weather.Instance.eventEmitter.Handle(Sys_Weather.EEvents.OnSeasonChange, OnSeasonChange, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, OnUpdateLevel, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<uint, uint, bool, bool>(Sys_Task.EEvents.OnTaskGoalStatusChanged, OnTaskGoalStatusChanged, toRegister);
            Sys_Escort.Instance.eventEmitter.Handle<uint>(Sys_Escort.EEvents.OnSyncConvoyStart, OnSyncConvoyStart, toRegister);
            Sys_Escort.Instance.eventEmitter.Handle<uint>(Sys_Escort.EEvents.OnSyncConvoyEnd, OnSyncConvoyEnd, toRegister);
            Sys_NpcFollow.Instance.eventEmitter.Handle<uint>(Sys_NpcFollow.EEvents.OnSyncNpcFollowStart, OnSyncNpcFollowStart, toRegister);
            Sys_NpcFollow.Instance.eventEmitter.Handle<uint>(Sys_NpcFollow.EEvents.OnSyncNpcFollowEnd, OnSyncNpcFollowEnd, toRegister);
            Sys_Track.Instance.eventEmitter.Handle<uint>(Sys_Track.EEvents.OnSyncTrackStart, OnSyncTrackStart, toRegister);
            Sys_Track.Instance.eventEmitter.Handle<uint>(Sys_Track.EEvents.OnSyncTrackEnd, OnSyncTrackEnd, toRegister);
            Sys_Npc.Instance.eventEmitter.Handle<uint>(Sys_Npc.EEvents.OnActiveNPC, OnActiveNPC, toRegister);
            Sys_Npc.Instance.eventEmitter.Handle(Sys_Npc.EEvents.OnCheckShowNpc, OnTimeCheck, toRegister);
        }

        void OnActiveNPC(uint npcInfoID)
        {
            if (npc == null)
                return;

            if (npc.cSVNpcData == null)
            {
                DebugUtil.LogError($"OnActiveNPC ERROR npc.cSVNpcData id null, id: {npcInfoID}");
                return;
            }

            Checking();
        }

        void OnTimeCheck()
        {
            if (marks.ContainsKey((int)EConditionType.CheckTimeInYear) || marks.ContainsKey((int)EConditionType.CheckTimeMonth) || marks.ContainsKey((int)EConditionType.CheckTimeOnDay) || marks.ContainsKey((int)EConditionType.CheckTimeOnWeek))
            {
                Checking();
            }
        }

        void OnTaskStatusChanged(TaskEntry taskEntry, ETaskState oldState, ETaskState newState)
        {
            if (marks.ContainsKey((int)EConditionType.HaveTask) || marks.ContainsKey((int)EConditionType.TaskUnReceived) || marks.ContainsKey((int)EConditionType.TaskUnCompleted) || marks.ContainsKey((int)EConditionType.TaskCompleted) || marks.ContainsKey((int)EConditionType.TaskSubmitted))
            {
                Checking();
            }
        }

        void OnRefreshChangeData(int changeType, int curBoxId)
        {
            if (marks.ContainsKey((int)EConditionType.HaveItem))
            {
                Checking();
            }
        }

        void OnWeatherChange()
        {
            if (marks.ContainsKey((int)EConditionType.Weather))
            {
                Checking();
            }
        }

        void OnDayNightChange()
        {
            if (marks.ContainsKey((int)EConditionType.Time))
            {
                Checking();
            }
        }

        void OnSeasonChange()
        {
            if (marks.ContainsKey((int)EConditionType.Season))
            {
                Checking();
            }
        }

        void OnUpdateLevel()
        {
            if (marks.ContainsKey((int)EConditionType.LessThanLv) || marks.ContainsKey((int)EConditionType.GreaterThanLv) || marks.ContainsKey((int)EConditionType.GreaterOrEqualLv) || marks.ContainsKey((int)EConditionType.EqualLv))
            {
                Checking();
            }
        }

        void OnTaskGoalStatusChanged(uint taskId, uint taskGoalId, bool oldState, bool newState)
        {
            if (marks.ContainsKey((int)EConditionType.TaskTargetCompleted) || marks.ContainsKey((int)EConditionType.TaskTargetUnCompleted))
            {
                Checking();
            }
        }

        void OnSyncConvoyStart(uint npcInfoId)
        {
            if (npc == null)
                return;

            if (npc.cSVNpcData == null)
            {
                DebugUtil.LogError($"OnSyncConvoyStart ERROR npc.cSVNpcData id null, id: {npcInfoId}");
                return;
            }

            if (npc.cSVNpcData.id == npcInfoId)
            {
                Checking();
            }
        }

        void OnSyncConvoyEnd(uint npcInfoId)
        {
            if (npc == null)
                return;

            if (npc.cSVNpcData == null)
            {
                DebugUtil.LogError($"OnSyncConvoyEnd ERROR npc.cSVNpcData id null, id: {npcInfoId}");
                return;
            }

            if (npc.cSVNpcData.id == npcInfoId)
            {
                Checking();
            }
        }

        void OnSyncNpcFollowStart(uint npcInfoId)
        {
            if (npc != null && npc.cSVNpcData.id == npcInfoId)
            {
                Checking();
            }
        }

        void OnSyncNpcFollowEnd(uint npcInfoId)
        {
            if (npc != null && npc.cSVNpcData.id == npcInfoId)
            {
                Checking();
            }
        }

        void OnSyncTrackStart(uint npcInfoId)
        {
            if (npc != null && npc.cSVNpcData.id == npcInfoId)
            {
                Checking();
            }
        }

        void OnSyncTrackEnd(uint npcInfoId)
        {
            if (npc != null && npc.cSVNpcData.id == npcInfoId)
            {
                Checking();
            }
        }

        Dictionary<int, int> marks = new Dictionary<int, int>();

        public void Checking()
        {
            ClearListConditions();
            bool preVisiable = Visiable;
            Visiable = DefaultVisiable;          

            if (npc != null && npc.cSVNpcData != null && npc.cSVNpcData.GreaterThanLvCond != 0)
            {
                ConditionBase greaterThanLvCond = ConditionManager.CreateCondition(EConditionType.GreaterOrEqualLv);

                List<int> values = new List<int>();
                values.Add((int)npc.cSVNpcData.GreaterThanLvCond);
                greaterThanLvCond.DeserializeObject(values);
                listConditions.Add(greaterThanLvCond);
                //conditions[greaterThanLvCond.FullName] = greaterThanLvCond;
            }

            if (npc.cSVNpcData.LessThanLvCond != 0)
            {
                ConditionBase lessThanLvCond = ConditionManager.CreateCondition(EConditionType.LessThanLv);

                List<int> values = new List<int>();
                values.Add((int)npc.cSVNpcData.LessThanLvCond);
                lessThanLvCond.DeserializeObject(values);
                listConditions.Add(lessThanLvCond);
                //conditions[lessThanLvCond.FullName] = lessThanLvCond;
            }

            bool NeedCheckGroup = false;
            bool groupFlag = true;
            if (npc.cSVNpcData.ConditionGroupCond != 0)
            {
                NeedCheckGroup = true;
                var cSVCheckseq = CSVCheckseq.Instance.GetConfData(npc.cSVNpcData.ConditionGroupCond);
                if (cSVCheckseq != null && !cSVCheckseq.IsValid(ref marks))
                {
                    groupFlag = false;
                }
            }
            else
            {
                NeedCheckGroup = false;
            }

            if (listConditions.Count == 0)
            {
                if (NeedCheckGroup && groupFlag)
                {
                    Visiable = !DefaultVisiable;
                }
            }
            else
            {
                if (NeedCheckGroup)
                {
                    if (CheckConditions() && groupFlag)
                    {
                        Visiable = !DefaultVisiable;
                    }
                }
                else
                {
                    if (CheckConditions())
                    {
                        Visiable = !DefaultVisiable;
                    }
                }
            }

            if (Sys_Escort.Instance.IsNpcEscorting(npc.cSVNpcData.id))
            {
                Visiable = false;
            }

            if (Sys_NpcFollow.Instance.IsNpcFollowing(npc.cSVNpcData.id))
            {
                Visiable = false;
            }

            if (Sys_Track.Instance.IsNpcTracking(npc.cSVNpcData.id))
            {
                Visiable = false;
            }

#if false
            //CollectionComponent collectionComponent = Logic.Core.World.GetComponent<CollectionComponent>(npc);
            CollectionComponent collectionComponent = npc.collectionComponent;
            if (collectionComponent != null)
            {
                if (collectionComponent.IsHide)
                {
                    Visiable = false;
                }
            }
#else
            if(npc.cSVNpcData.type == (uint)ENPCType.Collection)
            {
                CollectionNpc collectionNpc = npc as CollectionNpc;
                if(collectionNpc.collectionComponent.IsHide)
                {
                    Visiable = false;
                }
            }
#endif

            if (npc.cSVNpcData.type == (uint)ENPCType.Collection)
            {               
                ///是采集物且由隐藏变为显示///
                if (preVisiable == false && Visiable == true)
                {
                    EffectUtil.Instance.UnloadEffectByTag(npc.uID, EffectUtil.EEffectTag.CollectLock);

                    CSVCollectionProcess.Data cSVCollectionProcessData = CSVCollectionProcess.Instance.GetConfData(npc.cSVNpcData.id);
                    if (cSVCollectionProcessData != null)
                    {
                        CSVEffect.Data collectBirthEffectData = CSVEffect.Instance.GetConfData(cSVCollectionProcessData.collectionBirthEffect);
                        if (collectBirthEffectData != null)
                        {
                            EffectUtil.Instance.LoadEffect(npc.uID, collectBirthEffectData.effects_path, npc.fxRoot.transform, EffectUtil.EEffectTag.CollectBirth, collectBirthEffectData.fx_duration / 1000f);
                            Sys_CollectItem.Instance.eventEmitter.Trigger<ulong>(Sys_CollectItem.EEvents.CollectBirth, npc.uID);
                        }
                    }
                }
            }
            else if (npc.cSVNpcData.type == (uint)ENPCType.ActiveMonster)
            {
                //ActiveMonsterComponent activeMonsterComponent = Logic.Core.World.GetComponent<ActiveMonsterComponent>(npc);
                //ActiveMonsterComponent activeMonsterComponent = npc.activeMonsterComponent;
                Visiable = Visiable && npc.activeMonsterComponent.isShow;
            }
        }

        protected override void OnDispose()
        {
            npc = null;
            _visiable = true;

            ClearListConditions();
            ProcessEvents(false);

            marks.Clear();

            base.OnDispose();
        }

        void ClearListConditions()
        {
            for (int i = 0; i < listConditions.Count; ++i)
                listConditions[i].Dispose();

            listConditions.Clear();
        }

        bool CheckConditions()
        {
            for (int i = 0; i < listConditions.Count; ++i)
            {
                if (!listConditions[i].IsValid())
                {
                    return false;
                }
                else
                {
                }
            }

            return true;
        }
    }
}
