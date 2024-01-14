using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    public class CollectionComponent : Logic.Core.Component
    {
        public static readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents
        {
            DestroyCollection,
            CollectionCountZero
        }

        public static Dictionary<uint, int> currentEnableCounts = new Dictionary<uint, int>();

        public void AddToCtrlList()
        {
            if (CSVCollectionData.collectionPrivateFewTimes != 0)
            {
                if (CSVCollectionData.collectionUseNumRandom == 0)
                {
                    IsHide = false;
                    return;
                }

                if (!currentEnableCounts.ContainsKey(CSVCollectionData.id))
                {
                    IsHide = false;
                    currentEnableCounts[CSVCollectionData.id] = 1;
                }
                else
                {
                    UpdateWhenAddNewCollection();
                }
            }
        }

        void UpdateWhenAddNewCollection()
        {
            if (currentEnableCounts[CSVCollectionData.id] < CSVCollectionData.areaShowMinimum)
            {
                IsHide = false;
                currentEnableCounts[CSVCollectionData.id]++;
            }
            else if (currentEnableCounts[CSVCollectionData.id] >= CSVCollectionData.areaShowMaximum)
            {
                IsHide = true;
            }
            else
            {
                int value = UnityEngine.Random.Range(0, 3);
                if (value == 0)
                {
                    IsHide = false;
                    currentEnableCounts[CSVCollectionData.id]++;
                }
                else
                {
                    IsHide = true;
                }
            }
        }

        void UpdateWhenDestroyCollection(uint collectionID, ulong uid)
        {
            if (CSVCollectionData.id == collectionID && npc.uID != uid)
            {
                if (CSVCollectionData.collectionUseNumRandom == 0)
                {
                    IsHide = false;
                    return;
                }

                if (currentEnableCounts[CSVCollectionData.id] < CSVCollectionData.areaShowMinimum)
                {
                    if (IsHide)
                    {
                        currentEnableCounts[CSVCollectionData.id]++;
                        IsHide = false;
                    }
                }
                else if (currentEnableCounts[CSVCollectionData.id] >= CSVCollectionData.areaShowMaximum)
                {

                }
                else
                {
                    if (IsHide)
                    {
                        int value = UnityEngine.Random.Range(0, 2);
                        if (value == 0)
                        {
                            IsHide = false;
                            currentEnableCounts[CSVCollectionData.id]++;
                        }
                        else
                        {
                            IsHide = true;
                        }
                    }
                }
            }
        }

        void UpdateWhenCollectionCountZero(uint collectionID, ulong uid)
        {
            if (CSVCollectionData.id == collectionID)
            {
                if (npc.uID != uid)
                {
                    if (CSVCollectionData.collectionUseNumRandom == 0)
                    {
                        IsHide = false;
                        return;
                    }

                    if (currentEnableCounts[CSVCollectionData.id] < CSVCollectionData.areaShowMinimum)
                    {
                        if (IsHide)
                        {
                            currentEnableCounts[CSVCollectionData.id]++;
                            IsHide = false;
                        }
                    }
                    else if (currentEnableCounts[CSVCollectionData.id] >= CSVCollectionData.areaShowMaximum)
                    {
                    }
                    else
                    {
                        if (IsHide)
                        {
                            int value = UnityEngine.Random.Range(0, 2);
                            if (value == 0)
                            {
                                IsHide = false;
                                currentEnableCounts[CSVCollectionData.id]++;
                            }
                            else
                            {
                                IsHide = true;
                            }
                        }
                    }
                }

                npc.VisualComponent?.Checking();
            }
        }

        void UpdateWhenCollectionReset()
        {
            if (CSVCollectionData.collectionUseNumRandom == 0)
            {
                IsHide = false;
                return;
            }

            if (currentEnableCounts[CSVCollectionData.id] < CSVCollectionData.areaShowMinimum)
            {
                if (IsHide)
                {
                    IsHide = false;
                    currentEnableCounts[CSVCollectionData.id]++;
                }
            }
            else if (currentEnableCounts[CSVCollectionData.id] >= CSVCollectionData.areaShowMaximum)
            {
            }
            else
            {
                if (IsHide)
                {
                    int value = UnityEngine.Random.Range(0, 2);
                    if (value == 0)
                    {
                        IsHide = false;
                        currentEnableCounts[CSVCollectionData.id]++;
                    }
                    else
                    {
                        IsHide = true;
                    }
                }
            }
        }

        public int Count
        {
            get;
            set;
        }
        private Npc npc;
        private CSVCollection.Data CSVCollectionData;

        public bool IsPrivateCollectItem {
            get {
                return CSVCollectionData.collectionPrivateFewTimes > 0;
            } 
        }

        Timer countResetTimer;

        public bool IsHide;

        protected override void OnConstruct()
        {
            base.OnConstruct();

            npc = actor as Npc;
            CSVCollectionData = CSVCollection.Instance.GetConfData(npc.cSVNpcData.id);
            Count = (int)CSVCollectionData.collectionPrivateFewTimes;
            eventEmitter.Handle<uint, ulong>(EEvents.CollectionCountZero, UpdateWhenCollectionCountZero, true);
            eventEmitter.Handle<uint, ulong>(EEvents.DestroyCollection, UpdateWhenDestroyCollection, true);
            Sys_CollectItem.Instance.eventEmitter.Handle<ulong>(Sys_CollectItem.EEvents.OnCollectStarted, OnCollectStarted, true);
            Sys_CollectItem.Instance.eventEmitter.Handle<ulong>(Sys_CollectItem.EEvents.OnCollectEnded, OnCollectEnded, true);
        }

        protected override void OnDispose()
        {
            if (!IsHide)
            {
                if (currentEnableCounts.ContainsKey(CSVCollectionData.id))
                {
                    currentEnableCounts[CSVCollectionData.id]--;
                }
            }

            eventEmitter.Trigger<uint, ulong>(EEvents.DestroyCollection, CSVCollectionData.id, npc.uID);
            IsHide = false;
            Count = 0;
            CSVCollectionData = null;
            npc = null;
            Sys_CollectItem.Instance.eventEmitter.Handle<ulong>(Sys_CollectItem.EEvents.OnCollectStarted, OnCollectStarted, false);
            Sys_CollectItem.Instance.eventEmitter.Handle<ulong>(Sys_CollectItem.EEvents.OnCollectEnded, OnCollectEnded, false);
            countResetTimer?.Cancel();
            countResetTimer = null;
            eventEmitter.Handle<uint, ulong>(EEvents.CollectionCountZero, UpdateWhenCollectionCountZero, false);
            eventEmitter.Handle<uint, ulong>(EEvents.DestroyCollection, UpdateWhenDestroyCollection, false);

            base.OnDispose();
        }

        void OnCollectStarted(ulong uid)
        {
            if (npc.uID == uid)
            {
                CSVCollectionProcess.Data cSVCollectionProcessData = CSVCollectionProcess.Instance.GetConfData(npc.cSVNpcData.id);
                if (cSVCollectionProcessData != null)
                {
                    if (!string.IsNullOrWhiteSpace(cSVCollectionProcessData.collectingObjectAction))
                    {
                        npc.AnimationComponent.Play(cSVCollectionProcessData.collectingObjectAction, () =>
                        {
                            npc.AnimationComponent.Play((uint)EStateType.Idle);
                        });
                    }

                    CSVEffect.Data cSVEffectData = CSVEffect.Instance.GetConfData(cSVCollectionProcessData.collectObjectEffect);
                    if (cSVEffectData != null)
                    {
                        float yOffset = 0f;
                        if (GameCenter.mainHero.heroBaseComponent.TitleId != 0)
                        {
                            yOffset = 0.95f;
                        }
                        EffectUtil.Instance.LoadEffect(npc.uID, cSVEffectData.effects_path, npc.fxRoot.transform, EffectUtil.EEffectTag.Collection, cSVEffectData.fx_duration / 1000f, 1, 1, ELayerMask.Default, yOffset);
                    }
                }
            }
        }

        void ZeroCheck()
        {
            IsHide = true;
            npc.AnimationComponent.Play((uint)EStateType.Idle);
            if (currentEnableCounts.ContainsKey(CSVCollectionData.id))
            {
                currentEnableCounts[CSVCollectionData.id]--;
            }

            if (CSVCollectionData.ICollectionType == (uint)EICollectionLimitType.Forever)
            {
                npc.VisualComponent?.Checking();
                return;
            }

            if (CSVCollectionData.collectionUseNumRandom == 0)
            {
                countResetTimer?.Cancel();
                countResetTimer = Timer.Register(CSVCollectionData.collectionPrivateHitPoints / 1000f, () =>
                {
                    UpdateWhenCollectionReset();
                    Count = (int)CSVCollectionData.collectionPrivateFewTimes;
                    npc.VisualComponent?.Checking();
                }, null, false, false);
            }
            else
            {
                if (currentEnableCounts[CSVCollectionData.id] < CSVCollectionData.areaShowMinimum)
                {
                    countResetTimer?.Cancel();
                    UpdateWhenCollectionReset();
                    Count = (int)CSVCollectionData.collectionPrivateFewTimes;
                    npc.VisualComponent?.Checking();
                }
                else
                {
                    countResetTimer?.Cancel();
                    countResetTimer = Timer.Register(CSVCollectionData.collectionPrivateHitPoints / 1000f, () =>
                    {
                        UpdateWhenCollectionReset();
                        Count = (int)CSVCollectionData.collectionPrivateFewTimes;
                        npc.VisualComponent?.Checking();
                    }, null, false, false);
                }
            }

            eventEmitter.Trigger<uint, ulong>(EEvents.CollectionCountZero, CSVCollectionData.id, npc.uID);
            npc.VisualComponent?.Checking();
        }

        void OnCollectEnded(ulong uid)
        {
            if (npc.uID == uid)
            {
                Count--;
                if (Count < 0)
                {
                    Count = 0;
                }
                if (Count == 0)
                {
                    CSVCollectionProcess.Data cSVCollectionProcessData = CSVCollectionProcess.Instance.GetConfData(npc.cSVNpcData.id);
                    if (cSVCollectionProcessData != null && !string.IsNullOrWhiteSpace(cSVCollectionProcessData.collectObjectDeathAction))
                    {
                        UnityEngine.Component collider;
                        if (npc.modelGameObject != null && npc.modelGameObject.TryGetComponent(typeof(Collider), out collider))
                        {
                            ((Collider)collider).enabled = false;
                        }
                        npc.AnimationComponent.Play(cSVCollectionProcessData.collectObjectDeathAction, () =>
                        {
                            ZeroCheck();
                        });
                    }
                    else
                    {
                        ZeroCheck();
                    }

                    if (cSVCollectionProcessData != null)
                    {
                        CSVEffect.Data cSVEffectData = CSVEffect.Instance.GetConfData(cSVCollectionProcessData.collectObjectDeathEffect);
                        if (cSVEffectData != null)
                        {
                            float yOffset = 0f;
                            if (GameCenter.mainHero.heroBaseComponent.TitleId != 0)
                            {
                                yOffset = 0.95f;
                            }
                            EffectUtil.Instance.LoadEffect(npc.uID, cSVEffectData.effects_path, npc.fxRoot.transform, EffectUtil.EEffectTag.Collection, cSVEffectData.fx_duration / 1000f, 1, 1, ELayerMask.Default, yOffset);
                        }
                    }
                }
                else
                {
                    CSVCollectionProcess.Data cSVCollectionProcessData = CSVCollectionProcess.Instance.GetConfData(npc.cSVNpcData.id);
                    if (cSVCollectionProcessData != null)
                    {
                        CSVEffect.Data cSVEffectData = CSVEffect.Instance.GetConfData(cSVCollectionProcessData.collectObjectHpEffect);
                        if (cSVEffectData != null)
                        {
                            float yOffset = 0f;
                            if (GameCenter.mainHero.heroBaseComponent.TitleId != 0)
                            {
                                yOffset = 0.95f;
                            }
                            EffectUtil.Instance.LoadEffect(npc.uID, cSVEffectData.effects_path, npc.fxRoot.transform, EffectUtil.EEffectTag.Collection, cSVEffectData.fx_duration / 1000f, 1, 1, ELayerMask.Default, yOffset);
                        }

                        if (!string.IsNullOrWhiteSpace(cSVCollectionProcessData.collectObjectHpAction))
                        {
                            npc.AnimationComponent.Play(cSVCollectionProcessData.collectObjectHpAction, () =>
                            {
                                npc.AnimationComponent.Play((uint)EStateType.Idle);
                            });
                        }
                    }
                }
            }
        }
    }
}
