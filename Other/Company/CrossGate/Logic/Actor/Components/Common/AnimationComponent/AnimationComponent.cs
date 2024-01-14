using Framework;
using System.Collections.Generic;
using Lib.Core;
using UnityEngine;
using System;
using Logic.Core;
using Table;
using System.Text;
using System.Reflection;

namespace Logic
{
    public class AnimationData
    {
        public uint id;
        public AnimationClip clip;

        public AnimationData(uint id, AnimationClip clip)
        {
            this.id = id;
            this.clip = clip;
        }
    }

    /// <summary>
    /// 用于将动作常驻内存，避免因为动作加载时间出现人物动作的情况
    /// 目前只针对人物角色的移动
    /// </summary>
    public class AnimationComponentLoader
    {
        public AnimationClipLoader ClipLoader;
        public CSVActionState.Data ActionState;

        private Action<AnimationComponentLoader> _action;

        private bool IsDoneLoad = false;
        public void StartLoader(string path, AnimationClipLoader loader, CSVActionState.Data actionState, Action<AnimationComponentLoader> action)
        {
            ClipLoader = loader;
            ActionState = actionState;

            _action = action;

            IsDoneLoad = false;

            ClipLoader?.Start(path, OnLoadComplete);
        }

        private void OnLoadComplete(AnimationClipLoader loader)
        {
            IsDoneLoad = true;

            _action?.Invoke(this);
        }

        public void Release()
        {
            ClipLoader?.Release();
            _action = null;
            IsDoneLoad = false;
        }

        public bool isLoadFinish()
        {
            return IsDoneLoad;
        }

        public void AddLoadCompleteListener(Action<AnimationComponentLoader> action)
        {
            _action += action;
        }
    }


    public class AnimationPool
    {
        public static Dictionary<string, AnimationComponentLoader> m_DicAnimationCache = new Dictionary<string, AnimationComponentLoader>();

        public static AnimationComponentLoader GetAnimation(string path)
        {
            AnimationComponentLoader value = null;
            m_DicAnimationCache.TryGetValue(path, out value);

            return value;
        }

        public static void PushAnimation(string path, AnimationComponentLoader animation)
        {
            if (GetAnimation(path) != null)
                return;

           // Debug.Log("push animation name : " + path);

            m_DicAnimationCache.Add(path, animation);
        }
    }
    public class AnimationComponent : Logic.Core.Component
    {
        SimpleAnimation simpleAnimation;

        string _curNeedPlayAnimaName;

        Dictionary<uint, string> animations = new Dictionary<uint, string>();

        public StateComponent stateComponent;

        //Dictionary<string, AnimationClipLoader> assetRequests = new Dictionary<string, AnimationClipLoader>();
        List<AnimationComponentLoader> listLoaders = new List<AnimationComponentLoader>();

        static Dictionary<string, FieldInfo> cacheFieldInfos;

        SceneActor sceneActor;

        public AnimationComponent() { }

        protected override void OnConstruct()
        {
            sceneActor = actor as SceneActor;

            //stateComponent = World.GetComponent<StateComponent>(actor);
            //stateComponent = sceneActor.stateComponent;
            if (stateComponent != null)
            {
                stateComponent.StateChange += OnStateChange;
            }

            //TODO: 可以外部设置
            if (((IAnimatorActor)actor).AnimatorGameObject != null)
            {
                simpleAnimation = ((IAnimatorActor)actor).AnimatorGameObject.GetNeedComponent<SimpleAnimation>();
            }

            if (cacheFieldInfos == null)
            {
                cacheFieldInfos = new Dictionary<string, FieldInfo>();
                Type type = typeof(CSVAction.Data);
                foreach (var actionState in CSVActionState.Instance.GetAll())
                {
                    FieldInfo fieldInfo = type.GetField(actionState.action_state);
                    cacheFieldInfos[actionState.action_state] = fieldInfo;
                }
            }


        }

        protected override void OnDispose()
        {
            sceneActor = null;

            ClearAnimations();

            if (stateComponent != null)
            {
                stateComponent.StateChange -= OnStateChange;
            }

            stateComponent = null;
            if (simpleAnimation != null)
                simpleAnimation.Stop();
            simpleAnimation = null;
            _curNeedPlayAnimaName = null;
        }

        public void SetSimpleAnimation(SimpleAnimation animation)
        {
            simpleAnimation = animation;
        }

        public void ClearAnimations()
        {
            //_timerFirstAction?.Cancel();

            foreach (var state in animations)
            {
                RemoveState(state.Key);
            }
            animations.Clear();

            for (int i = 0; i < listLoaders.Count; ++i)
            {
                listLoaders[i].Release();
            }
            listLoaders.Clear();

            //simpleAnimation?.UnloadAnimationClips();
        }        

        /// <summary>
        /// 获取动画路径///
        /// </summary>
        /// <param name="curCharID"></param>
        /// <param name="curWeaponID"></param>
        /// <param name="stateFilter"></param>
        /// <returns></returns>
        public static void GetAnimationPaths(uint curCharID, uint curWeaponID, out List<string> paths, HashSet<uint> stateFilter = null, IList<uint> keepSort = null)
        {
            StringBuilder stringBuilder = StringBuilderPool.GetTemporary();
            if (cacheFieldInfos == null)
            {
                cacheFieldInfos = new Dictionary<string, FieldInfo>();
                Type type = typeof(CSVAction.Data);
                foreach (var actionState in CSVActionState.Instance.GetAll())
                {
                    FieldInfo fieldInfo = type.GetField(actionState.action_state);
                    cacheFieldInfos[actionState.action_state] = fieldInfo;
                }
            }

            paths = new List<string>();
            var weaponData = CSVEquipment.Instance.GetConfData(curWeaponID);
            if (weaponData != null)
            {
                curWeaponID = (weaponData.special_action == 0) ? 0 : curWeaponID;
            }
            else
            {
                DebugUtil.LogError($"weaponData is null id:{curWeaponID}");
            }

            uint animationKey = curCharID * Constants.CHARPARAM + weaponData.equipment_type * Constants.WEAPONTYPEPARAM + curWeaponID;
            CSVAction.Data actionData = CSVAction.Instance.GetConfData(animationKey);
            if (actionData != null)
            {
                var actionStates2 = CSVActionState.Instance.GetAll();
                if (stateFilter == null || stateFilter.Count <= 0)
                {
                    foreach (var actionState in actionStates2)
                    {                   
                        stringBuilder.Clear();                       

                        string getAnimationPathsTempStr = cacheFieldInfos[actionState.action_state].GetValue(actionData).ToString();
                        if (!String.IsNullOrWhiteSpace(getAnimationPathsTempStr))
                        {
                            string getAnimationPathsFinalPath = stringBuilder.Append(actionData.dirPath).Append(Constants.FANXIEGANG).Append(getAnimationPathsTempStr).Append(Constants.ANIMSUFFIX).ToString();
                            paths.Add(getAnimationPathsFinalPath);
                        }
                    }
                }
                else
                {
                    if (keepSort == null || keepSort.Count <= 0) {
                        foreach (var kvpFilter in stateFilter) {
                            foreach (var actionState in actionStates2) {
                                if (actionState.id == kvpFilter) {
                                    stringBuilder.Clear();

                                    string getAnimationPathsTempStr = cacheFieldInfos[actionState.action_state].GetValue(actionData).ToString();
                                    if (!String.IsNullOrWhiteSpace(getAnimationPathsTempStr)) {
                                        string getAnimationPathsFinalPath = stringBuilder.Append(actionData.dirPath).Append(Constants.FANXIEGANG).Append(getAnimationPathsTempStr).Append(Constants.ANIMSUFFIX).ToString();
                                        paths.Add(getAnimationPathsFinalPath);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    else {
                        for (int i = 0, length = keepSort.Count; i < length; ++i) {
                            foreach (var actionState in actionStates2) {
                                if (actionState.id == keepSort[i]) {
                                    stringBuilder.Clear();

                                    string getAnimationPathsTempStr = cacheFieldInfos[actionState.action_state].GetValue(actionData).ToString();
                                    if (!String.IsNullOrWhiteSpace(getAnimationPathsTempStr)) {
                                        string getAnimationPathsFinalPath = stringBuilder.Append(actionData.dirPath).Append(Constants.FANXIEGANG).Append(getAnimationPathsTempStr).Append(Constants.ANIMSUFFIX).ToString();
                                        paths.Add(getAnimationPathsFinalPath);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.LogError($"actionData is null, animationKey: {animationKey}");
            }
            StringBuilderPool.ReleaseTemporary(stringBuilder);
        }

        private EStateType _startState;
        private GameObject _gameObject;
        //private Timer _timerFirstAction;

        /// <summary>
        /// 获取动画组///
        /// </summary>
        /// <param name="curCharID">当前角色ID</param>
        /// <param name="curWeaponID">当前武器ID</param>
        /// <param name="stateFilter">需要播的动画集合，默认全部加载</param>
        public void UpdateHoldingAnimations(uint curCharID, uint curWeaponID = Constants.UMARMEDID, HashSet<uint> stateFilter = null, EStateType startState = EStateType.Idle, GameObject go = null)
        {
            ClearAnimations();

            _startState = startState;
            _gameObject = go;

            if (curWeaponID == 0)
            {
                curWeaponID = Constants.UMARMEDID;
            }
            var weaponData = CSVEquipment.Instance.GetConfData(curWeaponID);
            if (weaponData != null)
            {
                curWeaponID = (weaponData.special_action == 0) ? 0 : curWeaponID;
            }
            else
            {
                Debug.LogError($"weaponData is null id:{curWeaponID}");
            }

            Hero hero = actor as Hero;
            uint animationKey;
            if (hero != null && hero.Mount != null)
            {
                //animationKey = 100490000;
                animationKey = curCharID * Constants.CHARPARAM + 9 * Constants.WEAPONTYPEPARAM + hero.Mount.csvPetData.action_id_mount;
            }
            else
            { 
                animationKey = curCharID * Constants.CHARPARAM + weaponData.equipment_type * Constants.WEAPONTYPEPARAM + curWeaponID;
            }
                
            CSVAction.Data actionData = CSVAction.Instance.GetConfData(animationKey);
            if (actionData != null)
            {
                //var characterData = CSVCharacter.Instance.GetConfData(actionData.hero_id);//反查人物表
                StringBuilder stringBuilder = StringBuilderPool.GetTemporary();
                foreach (var actionState in CSVActionState.Instance.GetAll())
                {
                    if (stateFilter != null && !stateFilter.Contains(actionState.id))
                    {
                        continue;
                    }

                    string dir = cacheFieldInfos[actionState.action_state].GetValue(actionData).ToString();
                    if (!String.IsNullOrWhiteSpace(dir))
                    {
                        stringBuilder.Clear();
                        string finalPath = stringBuilder.Append(actionData.dirPath).Append("/").Append(dir).Append(Constants.ANIMSUFFIX).ToString();

                        AnimationClipLoader clipLoader = new AnimationClipLoader();
                        //assetRequests[finalPath] = clipLoader;

                        AnimationComponentLoader comLoader = new AnimationComponentLoader();
                        comLoader.StartLoader(finalPath, clipLoader, actionState, OnLoadClipComplete);
                        listLoaders.Add(comLoader);

                        //AnimationComponentLoader poolAnimation = null;
                        //if (characterData != null && actionState.Value.id == 1003)
                        //    poolAnimation = AnimationPool.GetAnimation(finalPath);

                        //if (poolAnimation == null)
                        //{
                        //    AnimationComponentLoader comLoader = new AnimationComponentLoader();
                        //    comLoader.StartLoader(finalPath, clipLoader, actionState.Value, OnLoadClipComplete);

                        //    if (characterData != null && actionState.Value.id == 1003)
                        //        AnimationPool.PushAnimation(finalPath, comLoader);
                        //    else
                        //        listLoaders.Add(comLoader);
                        //}
                        //else
                        //{
                        //    if (poolAnimation.isLoadFinish())
                        //        OnLoadClipComplete(poolAnimation);
                        //    else
                        //        poolAnimation.AddLoadCompleteListener(OnLoadClipComplete);
                        //}


                    }
                }
                StringBuilderPool.ReleaseTemporary(stringBuilder);
            }
            else
            {
                Debug.LogError($"actionData is null, animationKey: {animationKey}");
            }
        }

        void OnLoadClipComplete(AnimationComponentLoader loader)
        {
            AnimationClip animationClip = loader.ClipLoader.Result;
            animations[loader.ActionState.id] = loader.ActionState.action_state;
            AddState(new AnimationData(loader.ActionState.id, animationClip));
            if (loader.ActionState.id == (uint)_startState)
            {
                if (_gameObject != null)
                {
                    _gameObject.SetActive(true);
                }
                CrossFade((uint)_startState, Constants.CORSSFADETIME);
            }
            else
            {
                if (stateComponent != null && stateComponent.CurrentState == (EStateType)loader.ActionState.id)
                {
                    CrossFade(loader.ActionState.id, Constants.CORSSFADETIME);
                }
            }

            if (!string.IsNullOrEmpty(_curNeedPlayAnimaName) && _curNeedPlayAnimaName == loader.ActionState.action_state)
            {
                Play(_curNeedPlayAnimaName);
                _curNeedPlayAnimaName = null;
            }
        }

        void OnFisrtAction()
        {
            if (_gameObject != null)
            {
                _gameObject.SetActive(true);
            }
            CrossFade((uint)_startState, Constants.CORSSFADETIME);
        }

        void OnStateChange(EStateType oldState, EStateType newState)
        {
            if (newState == EStateType.NormalAttack)
            {
                CrossFade((uint)newState, Constants.CORSSFADETIME, OnCrossFadeOver);
            }
            else
            {
                Hero hero = sceneActor as Hero;
                if (hero != null && hero.Mount != null && hero.Mount.csvPetData != null)
                {
                    if (newState == EStateType.Idle)
                    {
                        if (hero.Mount.csvPetData.action_id_mount == 0)
                            CrossFade((uint)EStateType.mount_1_idle, Constants.CORSSFADETIME);
                        else
                            CrossFade((uint)EStateType.mount_2_idle, Constants.CORSSFADETIME);
                    }
                    else if (newState == EStateType.UI_Show_Idle)
                    {
                        if (hero.Mount.csvPetData.action_id_mount == 0)
                            CrossFade((uint)EStateType.mount_1_show_idle, Constants.CORSSFADETIME);
                        else
                            CrossFade((uint)EStateType.mount_2_show_idle, Constants.CORSSFADETIME);
                    }
                    else if (newState == EStateType.Inquiry)
                    {
                        if (hero.Mount.csvPetData.action_id_mount == 0)
                            CrossFade((uint)EStateType.mount_1_inquiry, Constants.CORSSFADETIME);
                        else
                            CrossFade((uint)EStateType.mount_2_inquiry, Constants.CORSSFADETIME);
                    }
                    else if (newState == EStateType.Run)
                    {
                        if (hero.Mount.csvPetData.action_id_mount == 0)
                            CrossFade((uint)EStateType.mount_1_run, Constants.CORSSFADETIME);
                        else
                            CrossFade((uint)EStateType.mount_2_run, Constants.CORSSFADETIME);
                    }
                    else if (newState == EStateType.Walk)
                    {
                        if (hero.Mount.csvPetData.action_id_mount == 0)
                            CrossFade((uint)EStateType.mount_1_walk, Constants.CORSSFADETIME);
                        else
                            CrossFade((uint)EStateType.mount_2_walk, Constants.CORSSFADETIME);
                    }
                    else
                    {
                        CrossFade((uint)newState, Constants.CORSSFADETIME);
                    }
                }
                else
                {
                    CrossFade((uint)newState, Constants.CORSSFADETIME);
                }
            }

        }

        void OnCrossFadeOver()
        {
            stateComponent?.ChangeState(EStateType.Idle);
        }

        #region PublicFunc

        public void SetSpeed(float speed)
        {
            simpleAnimation.SetSpeed(speed);
        }

        public void Play(uint stateId, Action playOver = null)
        {
            if (!animations.ContainsKey(stateId))
                return;

            simpleAnimation.Play(animations[stateId], playOver);
        }

        public void Play(string state, Action playOver = null)
        {
            if (!animations.ContainsValue(state))
                return;

            simpleAnimation?.Play(state, playOver);
        }

        public void PlaySuccess(string state, bool isVerifyContain = true, Action playOver = null, bool isNotNeedStartState = true)
        {
            if (simpleAnimation != null)
            {
                if (isNotNeedStartState)
                    _startState = EStateType.None;

                if (!animations.ContainsValue(state))
                {
                    if (!isVerifyContain)
                        _curNeedPlayAnimaName = state;

                    return;
                }

                if (simpleAnimation.PlaySuccess(state, playOver))
                    _curNeedPlayAnimaName = null;
                else
                    _curNeedPlayAnimaName = state;
            }
        }

        public void CrossFade(uint stateId, float fadeLength, Action playOver = null)
        {
            if (!animations.ContainsKey(stateId))
                return;

            simpleAnimation?.CrossFade(animations[stateId], fadeLength, playOver);
        }

        public void CrossFade(string state, float fadeLength, Action playOver = null)
        {
            if (!animations.ContainsValue(state))
                return;
            
            simpleAnimation?.CrossFade(state, fadeLength, playOver);
        }

        public void CrossFadeSuccess(string state, float fadeLength, Action playOver = null, float reduceTime = 0f)
        {
            if (simpleAnimation != null)
            {
                if (!animations.ContainsValue(state))
                {
                    _curNeedPlayAnimaName = state;

                    return;
                }

                if (simpleAnimation.CrossFadeSuccess(state, fadeLength, playOver, reduceTime))
                    _curNeedPlayAnimaName = null;
                else
                    _curNeedPlayAnimaName = state;
            }
        }

        public bool IsPlaying(uint stateId)
        {
            if (!animations.ContainsKey(stateId))
                return false;
            return simpleAnimation.IsPlaying(animations[stateId]);
        }

        public void Pause(string state)
        {
            if (!animations.ContainsValue(state))
                return;

            simpleAnimation?.Pause(state);
        }

        public void RemovePause(string state)
        {
            if (!animations.ContainsValue(state))
                return;

            simpleAnimation?.RemovePause(state);
        }

        public void PauseAll()
        {
            simpleAnimation?.PauseAll();
        }

        public void RemovePauseAll()
        {
            simpleAnimation?.RemovePauseAll();
        }

        public void Stop(uint stateId)
        {
            if (!animations.ContainsKey(stateId))
                return;
            simpleAnimation?.Stop(animations[stateId]);
        }

        public void Stop(string stateName)
        {
            if (!animations.ContainsValue(stateName))
                return;
            
            simpleAnimation?.Stop(stateName);
        }

        public void StopPlayOverTime()
        {
            simpleAnimation?.StopPlayOverTime();
        }

        public void StopAll()
        {
            simpleAnimation?.Stop();
            _curNeedPlayAnimaName = null;
        }

        public void SetAnimationEnable(bool isEnable)
        {
            if (simpleAnimation != null)
            {
                simpleAnimation.enabled = isEnable;
            }
        }

        public float GetLength(string stateName)
        {
            var anim = simpleAnimation.GetState(stateName);
            if (anim != null)
            {
                return anim.length;
            }
            return 0f;
        }

        string addTempStr = string.Empty;

        public void AddState(AnimationData data)
        {
            if (simpleAnimation == null)
                return;

            if (animations.TryGetValue(data.id, out addTempStr))
            {
                if (simpleAnimation[addTempStr] == null)
                {
                    if (data.clip == null)
                    {
                        DebugUtil.LogError($"animationClip is null uid: {actor.uID} id: {data.id} name: {addTempStr}");
                        return;
                    }

                    simpleAnimation.AddState(data.clip, addTempStr);
                }
            }
            addTempStr = string.Empty;
        }

        string removeTempStr = string.Empty;

        public void RemoveState(uint stateId)
        {
            if (simpleAnimation == null)
                return;

            if (animations.TryGetValue(stateId, out removeTempStr))
            {
                if (simpleAnimation[removeTempStr] != null)
                {
                    simpleAnimation.RemoveState(removeTempStr);
                }
            }
            removeTempStr = string.Empty;
        }

        public int GetAnimationsCount()
        {
            return animations == null ? 0 : animations.Count;
        }

        public bool HadAnimation(uint stateid)
        {
            return animations.ContainsKey(stateid);
        }

        public void UpdateAni(float deltaTime)
        {
            simpleAnimation?.UpdateAni(deltaTime);
        }
        #endregion
    }
}
