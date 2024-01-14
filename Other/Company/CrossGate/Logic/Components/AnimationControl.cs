using Framework;
using Lib.Core;
using Logic;
using System;
using System.Collections.Generic;
using System.Text;
using Table;
using UnityEngine;
using Logic.Core;
using System.Reflection;

public class AnimationControl
{
    protected SimpleAnimation simpleAnimation;

    string _curNeedPlayAnimaName;

    public Dictionary<uint, string> animations = new Dictionary<uint, string>();

    Dictionary<string, AnimationClipLoader> assetRequests = new Dictionary<string, AnimationClipLoader>();
    List<AnimationComponentLoader> listLoaders = new List<AnimationComponentLoader>();

    static Dictionary<string, FieldInfo> cacheFieldInfos;

    public void SetOwner(GameObject go)
    {
        simpleAnimation = go.GetNeedComponent<SimpleAnimation>();

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

    public void ClearAnimations()
    {
        _timerFirstAction?.Cancel();
        _timerLastAction?.Cancel();
        _actionLastAction = null;

        foreach (var state in animations)
        {
            try
            {
                RemoveState(state.Key);
            }
            catch (System.Exception e)
            {
                continue;
            }
        }
        animations.Clear();

        for (int i = 0; i < listLoaders.Count; ++i)
        {
            listLoaders[i].Release();
        }
        listLoaders.Clear();

        foreach (var data in assetRequests)
        {
            data.Value.Release();
        }
        assetRequests.Clear();
    }    

    /// <summary>
    /// 获取动画路径///
    /// </summary>
    /// <param name="curCharID"></param>
    /// <param name="curWeaponID"></param>
    /// <param name="stateFilter"></param>
    /// <returns></returns>
    public static List<string> GetAnimationPaths(uint curCharID, uint curWeaponID, List<uint> stateFilter = null)
    {
        List<string> paths = new List<string>();

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
                for (int i = 0, length = stateFilter.Count; i < length; ++i)
                {
                    foreach (var actionState in actionStates2)
                    {
                        if (actionState.id == stateFilter[i])
                        {
                            stringBuilder.Clear();
                            
                            string getAnimationPathsTempStr = cacheFieldInfos[actionState.action_state].GetValue(actionData).ToString();                            
                            if (!String.IsNullOrWhiteSpace(getAnimationPathsTempStr))
                            {
                                string getAnimationPathsFinalPath = stringBuilder.Append(actionData.dirPath).Append(Constants.FANXIEGANG).Append(getAnimationPathsTempStr).Append(Constants.ANIMSUFFIX).ToString();
                                paths.Add(getAnimationPathsFinalPath);
                            }
                            break;
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

        return paths;
    }

    Timer timer;

    private EStateType _startState;
    private GameObject _gameObject;
    private Timer _timerFirstAction;

    private Timer _timerLastAction;
    private Action _actionLastAction;

    /// <summary>
    /// 获取动画组///
    /// </summary>
    /// <param name="curCharID">当前角色ID</param>
    /// <param name="curWeaponID">当前武器ID</param>
    /// <param name="stateFilter">需要播的动画集合，默认全部加载</param>
    public void UpdateHoldingAnimations(uint curCharID, uint curWeaponID = Constants.UMARMEDID, HashSet<uint> stateFilter = null, EStateType startState = EStateType.Idle, GameObject go = null, Action actionLastAction = null)
    {
        ClearAnimations();

        _startState = startState;
        _gameObject = go;
        _actionLastAction = actionLastAction;

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

        uint animationKey = curCharID * Constants.CHARPARAM + weaponData.equipment_type * Constants.WEAPONTYPEPARAM + curWeaponID;
        CSVAction.Data actionData = CSVAction.Instance.GetConfData(animationKey);
        if (actionData != null)
        {
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
                    assetRequests[finalPath] = clipLoader;

                    AnimationComponentLoader comLoader = new AnimationComponentLoader();
                    comLoader.StartLoader(finalPath, clipLoader, actionState, OnLoadClipComplete);
                    listLoaders.Add(comLoader);
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
            _timerFirstAction?.Cancel();
            _timerFirstAction = Timer.Register(0.1f, OnFisrtAction);
        }

#if UNITY_EDITOR && !ILRUNTIME_MODE
        if (GameCenter.ShowSceneRootInspectorFlag)
        {
            _timerFirstAction?.Cancel();
            _timerFirstAction = Timer.Register(0.1f, OnFisrtAction);
        }
#endif

        if (!string.IsNullOrEmpty(_curNeedPlayAnimaName) && _curNeedPlayAnimaName == loader.ActionState.action_state)
        {
            Play(_curNeedPlayAnimaName);
            _curNeedPlayAnimaName = null;
        }

        if (listLoaders.Count == animations.Count)
        {
            _timerLastAction?.Cancel();
            _timerLastAction = Timer.Register(0.1f, OnLastAction);
        }
    }

    void OnFisrtAction()
    {
        if (_gameObject != null)
        {
            _gameObject.SetActive(true);
        }
        CrossFade((uint)_startState, Constants.CORSSFADETIME, () =>
        {
            Play((uint)EStateType.Idle);
        });

#if UNITY_EDITOR && !ILRUNTIME_MODE
        if (GameCenter.ShowSceneRootInspectorFlag)
        {
            CrossFade((uint)EStateType.Idle, Constants.CORSSFADETIME, () =>
            {
                Play((uint)EStateType.Idle);
            });
        }
#endif
    }

    void OnLastAction()
    {
        _actionLastAction?.Invoke();
    }

    public void SetSpeed(float speed)
    {
        simpleAnimation?.SetSpeed(speed);
    }

    public void Play(uint stateId, Action playOver = null)
    {
        if (animations == null)
            return;

        if (!animations.ContainsKey(stateId))
            return;

        simpleAnimation?.Play(animations[stateId], playOver);
    }

    public void Play(string state, Action playOver = null)
    {
        if (animations == null)
            return;

        if (!animations.ContainsValue(state))
            return;

        simpleAnimation?.Play(state, playOver);
    }

    public void CrossFade(uint stateId, float fadeLength, Action playOver = null)
    {
        if (animations == null)
            return;

        if (!animations.ContainsKey(stateId))
            return;

        simpleAnimation?.CrossFade(animations[stateId], fadeLength, playOver);
    }

    public void CrossFade(string state, float fadeLength, Action playOver = null)
    {
        simpleAnimation?.CrossFade(state, fadeLength, playOver);
    }

    public void CrossFadeAction(string state, float fadeLength, Action playOver = null, float reduceTime = 0f)
    {
        simpleAnimation?.CrossFade(state, fadeLength, playOver, reduceTime);
    }

    public bool IsPlaying(uint stateId)
    {
        if (animations == null)
            return false;

        if (simpleAnimation == null)
            return false;

        if (!animations.ContainsKey(stateId))
            return false;

        return simpleAnimation.IsPlaying(animations[stateId]);
    }

    public void Stop(uint stateId)
    {
        if (animations == null)
            return;

        if (!animations.ContainsKey(stateId))
            return;

        simpleAnimation?.Stop(animations[stateId]);
    }

    public void Stop(string stateName)
    {
        simpleAnimation?.Stop(stateName);
    }

    public void StopPlayOverTime()
    {
        simpleAnimation?.StopPlayOverTime();
    }

    public void StopAll()
    {
        simpleAnimation?.Stop();
    }

    public int GetClipCount()
    {
        if (simpleAnimation == null)
            return 0;

        return simpleAnimation.GetClipCount();
    }

    public float GetLength(string stateName)
    {
        if (simpleAnimation == null)
            return 0f;
        
        return simpleAnimation.GetState(stateName).length;
    }

    public void AddState(AnimationData data)
    {
        if (animations == null)
            return;

        if (simpleAnimation == null)
            return;

        if (!animations.ContainsKey(data.id))
            return;

        if (simpleAnimation[animations[data.id]] != null)
            return;

        if (data.clip == null)
        {
            return;
        }
        simpleAnimation.AddState(data.clip, animations[data.id]);
    }

    public void RemoveState(uint stateId)
    {
        if (animations == null)
            return;

        if (simpleAnimation == null)
            return;

        if (!animations.ContainsKey(stateId))
            return;

        if (simpleAnimation[animations[stateId]] != null)
        {
            simpleAnimation.RemoveState(animations[stateId]);
        }
    }

    public void AddStates(IEnumerable<AnimationData> datas)
    {
        foreach (var data in datas)
        {
            AddState(data);
        }
    }

    public void RemoveStates(IEnumerable<uint> eAnimationIDs)
    {
        foreach (var id in eAnimationIDs)
        {
            RemoveState(id);
        }
    }

    public int GetAnimationsCount()
    {
        return animations == null ? 0 : animations.Count;
    }
}