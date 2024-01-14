using Lib.Core;
using Logic;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CombatModelManager : Logic.Singleton<CombatModelManager>
{
    public class PreRegisterModelStruct
    {
        public ulong Id;
        public string ModelName;
        public System.Action<GameObject, ulong> UseModelAction;

        public void Push()
        {
            Id = 0ul;
            ModelName = null;
            UseModelAction = null;

            CombatObjectPool.Instance.Push(this);
        }
    }

    public class ModelDelayStruct
    {
        public ulong Id;
        public string ModelName;
        public Transform ModelParent;
        public float DelayTime;
        public System.Action<GameObject, ulong> ModelAction;

        public void Push()
        {
            ModelName = null;
            ModelParent = null;
            ModelAction = null;

            CombatObjectPool.Instance.Push(this);
        }
    }

    public class ModelUseStateStruct
    {
        public string ModelName;
        /// <summary>
        /// =-1已经被回收; =0正在延迟过程中; =1延迟结束在加载中; =2加载结束;
        /// </summary>
        public int State;
        public GameObject Go;
        public Transform FreeParent;

        public void Push()
        {
            ModelName = null;
            Go = null;
            FreeParent = null;

            CombatObjectPool.Instance.Push(this);
        }
    }

    public class ModelFreeStruct
    {
        public ulong Id;
        public GameObject Go;

        public void Push()
        {
            Go = null;

            CombatObjectPool.Instance.Push(this);
        }
    }

    public class AssetDataClass
    {
        public AsyncOperationHandle<GameObject> OperationHandle;
        public System.Action<AsyncOperationHandle<GameObject>> OHAction;

        public void Push()
        {
            OHAction = null;

            CombatObjectPool.Instance.Push(this);
        }
    }

    public bool m_IsWork;

    public List<GameObject> m_ChangeParticleSystemScaleModelList;

    private List<PreRegisterModelStruct> _preRegisterModelStructList;

    private List<ModelDelayStruct> _modelDelayList;

    private Dictionary<ulong, ModelUseStateStruct> _useModelDic;

    private Dictionary<string, TQueue<ModelFreeStruct>> _freeModel;

    private Queue<string> _recordStrQueue;
    
    private Dictionary<ulong, AssetDataClass> _assetDic;

    private bool _init;

    private float _freeIntervalTime;

    public void OnAwake()
    {
        Init();
    }

    private void Init()
    {
        if (_init)
            return;

        _init = true;

        if (m_ChangeParticleSystemScaleModelList == null)
            m_ChangeParticleSystemScaleModelList = new List<GameObject>();

        if (_preRegisterModelStructList == null)
            _preRegisterModelStructList = new List<PreRegisterModelStruct>();

        if (_modelDelayList == null)
            _modelDelayList = new List<ModelDelayStruct>();

        if (_useModelDic == null)
            _useModelDic = new Dictionary<ulong, ModelUseStateStruct>();

        if (_freeModel == null)
            _freeModel = new Dictionary<string, TQueue<ModelFreeStruct>>();

        if (_recordStrQueue == null)
            _recordStrQueue = new Queue<string>();

        if (_assetDic == null)
            _assetDic = new Dictionary<ulong, AssetDataClass>();
    }

    public void OnEnable()
    {
        m_IsWork = true;
    }

    public void OnUpdate()
    {
        if (!_init)
            Init();

        if (_modelDelayList.Count > 0)
        {
            for (int i = _modelDelayList.Count - 1; i > -1; i--)
            {
                ModelDelayStruct mds = _modelDelayList[i];
                if (string.IsNullOrEmpty(mds.ModelName))
                {
                    _modelDelayList.RemoveAt(i);
                    mds.Push();
                    continue;
                }

                if (Time.time > mds.DelayTime)
                {
                    SetModel(mds.Id, mds.ModelName, mds.ModelParent, mds.ModelAction);

                    _modelDelayList.RemoveAt(i);
                    mds.Push();
                }
            }
        }

        if (_freeModel.Count > 0)
        {
            _freeIntervalTime += Time.deltaTime;
            if (_freeIntervalTime > 5f)
            {
                _freeIntervalTime = 0f;

                foreach (var keyValue in _freeModel)
                {
                    TQueue<ModelFreeStruct> value = keyValue.Value;
                    if (value == null || value.Count == 0)
                        continue;

                    if (value.CheckExpire())
                    {
                        while (value.Count > 0)
                        {
                            var mfs = value.Dequeue();

                            if (mfs.Go != null)
                                DestroyGo(mfs.Go);

                            RecoveryAsset(mfs.Id);
                            mfs.Push();
                        }

                        _recordStrQueue.Enqueue(keyValue.Key);
                    }
                }

                while (_recordStrQueue.Count > 0)
                {
                    string recordStr = _recordStrQueue.Dequeue();
                    _freeModel.Remove(recordStr);
                }
            }
        }
    }

    public void OnDisable()
    {
        m_IsWork = false;
#if DEBUG_MODE
        //if (_freeModel.Count > 0)
        //{
        //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //    foreach (var kv in _freeModel)
        //    {
        //        ModelFreeStruct[] list = kv.Value.GetArray();
        //        foreach (var item in list)
        //        {
        //            if (item.Go == null)
        //                continue;

        //            sb.Clear();
        //            sb.Append($"{item.Go.name}------<color=yellow>");
        //            Renderer[] pss = item.Go.GetComponentsInChildren<Renderer>();
        //            for (int i = 0, length = pss.Length; i < length; i++)
        //            {
        //                var ps = pss[i];
        //                if (ps == null)
        //                    continue;

        //                sb.Append($"    mat{i.ToString()}:{(ps.material == null ? "null" : $"{ps.material.name}【{ps.material.shader?.name}】")}");
        //            }
        //            sb.Append($"</color>");
        //            DebugUtil.Log(ELogType.CombatAILog, $"{sb.ToString()}");
        //        }
        //    }
        //}
#endif
    }

    public void Dispose()
    {
        if (null == _modelDelayList
            || null == _useModelDic
            || null == _freeModel
            || null == _assetDic
            || m_ChangeParticleSystemScaleModelList == null
            || _recordStrQueue == null)
        {
            return;
        }

        m_ChangeParticleSystemScaleModelList.Clear();

        if (_modelDelayList.Count > 0)
        {
            foreach (var md in _modelDelayList)
            {
                md.Push();
            }
            _modelDelayList.Clear();
        }
        
        foreach (var kv in _useModelDic)
        {
            ModelUseStateStruct muss = kv.Value;

            if (muss.Go != null)
                DestroyGo(muss.Go);

            RecoveryAsset(kv.Key);
            muss.Push();
        }
        _useModelDic.Clear();

        foreach (var kv in _freeModel)
        {
            var queue = kv.Value;
            if (queue == null)
                continue;

            while (queue.Count > 0)
            {
                var mfs = queue.Dequeue();

                if(mfs.Go != null)
                    DestroyGo(mfs.Go);

                RecoveryAsset(mfs.Id);
                mfs.Push();
            }
        }
        _freeModel.Clear();

        foreach (var kv in _assetDic)
        {            
            AssetDataClass adc = kv.Value;
            if (adc == null)
                continue;

            if(adc.OperationHandle.IsValid())
            {
                AddressablesUtil.Release(ref adc.OperationHandle, adc.OHAction);
            }

            adc.Push();
        }
        _assetDic.Clear();

        _recordStrQueue.Clear();

        m_IsWork = false;
        _init = false;
    }
    
    public ulong CreateModel(string modelName, Transform parent = null, System.Action<GameObject, ulong> action = null, float delayTime = 0f, bool isPreRegister = false)
    {
        if (string.IsNullOrEmpty(modelName))
            return 0ul;

        //处理预注册
        if (_preRegisterModelStructList.Count > 0)
        {
            if (isPreRegister)
            {
                for (int i = 0, preCount = _preRegisterModelStructList.Count; i < preCount; i++)
                {
                    PreRegisterModelStruct preRegisterModelStruct = _preRegisterModelStructList[i];
                    if (preRegisterModelStruct == null)
                        continue;

                    if (preRegisterModelStruct.ModelName == modelName)
                    {
                        return preRegisterModelStruct.Id;
                    }
                }
            }
            else
            {
                for (int i = 0, preCount = _preRegisterModelStructList.Count; i < preCount; i++)
                {
                    PreRegisterModelStruct preRegisterModelStruct = _preRegisterModelStructList[i];
                    if (preRegisterModelStruct == null)
                        continue;

                    if (preRegisterModelStruct.ModelName == modelName)
                    {
                        if (preRegisterModelStruct.UseModelAction == null)
                        {
                            preRegisterModelStruct.UseModelAction = action;
                            return preRegisterModelStruct.Id;
                        }

                        break;
                    }
                }
            }
        }

        ulong id = ++CombatHelp.m_StartId;
        
        if (delayTime <= 0)
        {
            SetModel(id, modelName, parent, action, isPreRegister);
        }
        else
        {
            ModelDelayStruct modelDelayStruct = CombatObjectPool.Instance.Get<ModelDelayStruct>();
            modelDelayStruct.Id = id;
            modelDelayStruct.ModelName = modelName;
            modelDelayStruct.ModelParent = parent;
            modelDelayStruct.DelayTime = delayTime + Time.time;
            modelDelayStruct.ModelAction = action;

            _modelDelayList.Add(modelDelayStruct);

            SetUseModelData(modelName, id, 0);
        }

        return id;
    }
    
    private void SetModel(ulong id, string modelName, Transform parent = null, System.Action<GameObject, ulong> action = null, bool isPreRegister = false)
    {
        TQueue<ModelFreeStruct> queue = null;
        GameObject obj = null;
        ulong oldId = 0ul;
        if (_freeModel.TryGetValue(modelName, out queue) && queue.Count > 0)
        {
            ModelFreeStruct mfs = queue.Dequeue();
            obj = mfs.Go;
            oldId = mfs.Id;
            if (obj == null)
            {
                RecoveryAsset(oldId);
            }
            mfs.Push();
        }

        if (obj == null)
        {
            if (_assetDic.ContainsKey(id))
            {
                Debug.LogError($"使用已经存在的Model id：{id.ToString()}去生成新的model");
                return;
            }

            if (!SetUseModelData(modelName, id, 1))
                return;

            if (isPreRegister)
            {
                PreRegisterModelStruct preRegisterModelStruct = CombatObjectPool.Instance.Get<PreRegisterModelStruct>();
                preRegisterModelStruct.Id = id;
                preRegisterModelStruct.ModelName = modelName;

                _preRegisterModelStructList.Add(preRegisterModelStruct);
            }

            AssetDataClass adc = CombatObjectPool.Instance.Get<AssetDataClass>();
            adc.OHAction = (cell) =>
            {
                if (!m_IsWork || !IsUseModelData(id))
                    return;

                if (cell.Result == null)
                {
                    DebugUtil.LogError($"获取模型：{modelName}的Result为null");
                    return;
                }

                obj = GameObject.Instantiate(cell.Result);
                if (obj != null)
                {
                    //DebugUtil.Log(ELogType.CombatAILog, $"CombatModelManager----<color=yellow>创建新的{obj.name}    Id:{id.ToString()}</color>");

                    if (!SetUseModelData(modelName, id, 2, obj))
                        return;

                    if (parent != null)
                    {
                        obj.transform.SetParent(parent, false);
                    }

                    obj.SetActive(true);
                    Transform trans = obj.transform;
                    trans.localPosition = Vector3.zero;
                    trans.localEulerAngles = Vector3.zero;

                    action?.Invoke(obj, id);

                    if (isPreRegister)
                    {
                        DoPreRegisterModel(modelName, id, obj);
                    }
                }
            };
            AddressablesUtil.LoadAssetAsync(ref adc.OperationHandle, modelName, adc.OHAction);
            
            _assetDic.Add(id, adc);
        }
        else
        {
            AssetDataClass adc;
            if (_assetDic.TryGetValue(oldId, out adc))
            {
                _assetDic.Remove(oldId);

                _assetDic.Add(id, adc);
            }

            //DebugUtil.Log(ELogType.CombatAILog, $"CombatModelManager----<color=yellow>复用旧的{obj.name}    oldId:{oldId.ToString()}    Id:{id.ToString()}</color>");

            if (!SetUseModelData(modelName, id, 2, obj))
                return;

            if (isPreRegister)
            {
                FreeModel(id, CombatManager.Instance.m_WorkStreamTrans);
                return;
            }

            if (parent != null)
            {
                obj.transform.SetParent(parent, false);
            }
            else if (obj.transform.parent != null)
            {
                obj.transform.SetParent(null);
            }
            obj.SetActive(true);
            Transform trans = obj.transform;
            trans.localPosition = Vector3.zero;
            trans.localEulerAngles = Vector3.zero;

            action?.Invoke(obj, id);
        }
    }    
    
    private bool SetUseModelData(string modelName, ulong id, int state, GameObject go = null)
    {
        if (_useModelDic.ContainsKey(id))
        {
            ModelUseStateStruct muss = _useModelDic[id];

            if (muss.ModelName != modelName)
            {
                DebugUtil.LogError($"创建model时发现已存在的model名字不同ModelId一样，new={modelName}，old=muss.ModelName");

                muss.State = -1;
            }

            if (muss.State == -1)
            {
                PushToFreeModel(muss.ModelName, go, id, muss.FreeParent);

                if (go != muss.Go)
                {
                    //DebugUtil.LogError($"{modelName}回收状态State{muss.State}下缓存的和要销毁的不一致muss.Go;{muss.Go?.name}    go:{go?.name}");
                    //PushToFreeModel(muss.ModelName, muss.Go, id, muss.FreeParent);
                    if(muss.Go != null)
                        DestroyGo(muss.Go);
                }

                _useModelDic.Remove(id);
                muss.Push();

                return false;
            }
            
            muss.ModelName = modelName;
            muss.State = state;
            muss.Go = go;

            _useModelDic[id] = muss;
        }
        else
        {
            ModelUseStateStruct muss = CombatObjectPool.Instance.Get<ModelUseStateStruct>();
            muss.ModelName = modelName;
            muss.State = state;
            muss.Go = go;
            muss.FreeParent = null;

            _useModelDic[id] = muss;
        }

        return true;
    }

    private bool IsUseModelData(ulong id)
    {
        if (_useModelDic.ContainsKey(id))
        {
            ModelUseStateStruct muss = _useModelDic[id];
            if (muss.State == -1)
            {
                PushToFreeModel(muss.ModelName, muss.Go, id, muss.FreeParent);

                _useModelDic.Remove(id);
                muss.Push();
                return false;
            }
        }

        return true;
    }

    public GameObject GetUseModel(ulong id)
    {
        if (_useModelDic.ContainsKey(id))
            return _useModelDic[id].Go;

        return null;
    }

    public void FreeModel(ulong id, Transform freeParent = null, bool isSetFar = false, bool isDestroy = false)
    {
        if (id <= 0ul)
            return;
        
        if (_useModelDic.ContainsKey(id))
        {
            ModelUseStateStruct muss = _useModelDic[id];
            if (muss.State == 2)
            {
                PushToFreeModel(muss.ModelName, muss.Go, id, freeParent, isSetFar, isDestroy);
                _useModelDic.Remove(id);
                muss.Push();
            }
            else
            {
                muss.State = -1;
                muss.FreeParent = freeParent;
                _useModelDic[id] = muss;
            }
        }
    }

    //不能直接调用，得和_useModelDic.Remove(id);配套使用
    private void PushToFreeModel(string modelName, GameObject obj, ulong id, Transform freeParent, bool isSetFar = false, bool isDestroy = false)
    {
        if (isDestroy || obj == null || string.IsNullOrEmpty(modelName))
        {
            if(obj != null)
                DestroyGo(obj);
            RecoveryAsset(id);
            return;
        }
        
        TQueue<ModelFreeStruct> queue = null;
        if (!_freeModel.TryGetValue(modelName, out queue) || queue == null)
        {
            queue = new TQueue<ModelFreeStruct>(modelName);
            _freeModel[modelName] = queue;
        }

        ModelFreeStruct mfs = CombatObjectPool.Instance.Get<ModelFreeStruct>();
        mfs.Go = obj;
        mfs.Id = id;

        //if (queue.IsContain(mfs))
        //{
        //    DebugUtil.LogError($"在重复回收ModelName；{modelName}");
        //    return;
        //}

        queue.Enqueue(mfs);

        if (freeParent != null)
        {
            obj.transform.SetParent(freeParent);
        }

        if (isSetFar)
            obj.transform.position = CombatHelp.FarV3;

        obj.SetActive(false);
    }

    private void RecoveryAsset(ulong modelId)
    {
        if (modelId <= 0ul)
            return;

        AssetDataClass adc;
        if (_assetDic.TryGetValue(modelId, out adc))
        {
            DebugUtil.Log(ELogType.eCombatObject, $"RecoveryAsset----<color=yellow>modelId:{modelId.ToString()}    GameObject:{adc.OperationHandle.Result?.name}</color>");
            _assetDic.Remove(modelId);
            AddressablesUtil.ReleaseInstance(ref adc.OperationHandle, adc.OHAction);

            adc.Push();
        }
    }

    public bool IsLoaded(string modelStr)
    {
        if (_freeModel.TryGetValue(modelStr, out TQueue<ModelFreeStruct> queue) && queue.Count > 0)
            return true;

        if (_preRegisterModelStructList.Count > 0)
        {
            for (int i = 0, preCount = _preRegisterModelStructList.Count; i < preCount; i++)
            {
                PreRegisterModelStruct preRegisterModelStruct = _preRegisterModelStructList[i];
                if (preRegisterModelStruct == null)
                    continue;

                if (preRegisterModelStruct.ModelName == modelStr)
                {
                    return true;
                }
            }
        }
        
        foreach (var kv in _useModelDic)
        {
            ModelUseStateStruct modelUseStateStruct = kv.Value;
            if (modelUseStateStruct == null)
                continue;

            if (modelUseStateStruct.ModelName == modelStr)
                return true;
        }

        return false;
    }

    private void DoPreRegisterModel(string modelStr, ulong id, GameObject obj)
    {
        for (int i = 0, preCount = _preRegisterModelStructList.Count; i < preCount; i++)
        {
            PreRegisterModelStruct preRegisterModelStruct = _preRegisterModelStructList[i];
            if (preRegisterModelStruct == null)
                continue;

            if (preRegisterModelStruct.ModelName == modelStr)
            {
                if (preRegisterModelStruct.Id != id)
                {
                    DebugUtil.LogError($"预注册加载CombatModel的Id不正确");
                }

                if (preRegisterModelStruct.UseModelAction == null)
                {
                    FreeModel(preRegisterModelStruct.Id, CombatManager.Instance.m_WorkStreamTrans);
                }
                else
                {
                    preRegisterModelStruct.UseModelAction.Invoke(obj, id);
                }

                _preRegisterModelStructList.RemoveAt(i);
                preRegisterModelStruct.Push();
                break;
            }
        }
    }

    private void DestroyGo(GameObject obj)
    {
        m_ChangeParticleSystemScaleModelList.Remove(obj);
        Object.DestroyImmediate(obj);
    }
}
