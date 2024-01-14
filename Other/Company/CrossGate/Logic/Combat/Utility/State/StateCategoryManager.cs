using Framework;
using Logic;
using System;
using System.Collections.Generic;

/// <summary>
/// 根据不同的state种类（种类自定义）生成不同的多样的StateControllerEntity控制器
/// 
/// 可以继承StateControllerEntity来多样化StateControllerEntity控制器
/// 
/// StateControllerEntity控制器用来控制一个或多个的StateMachineEntity状态机
/// 
/// （注意：）创建StateMachineEntity状态机一定要
///             设置StateTranstionComponent转换组件（用户自定义继承该组件，生成不同的状态转换）数据
/// 
/// StateMachineEntity状态机可以控制一个或多个StateBaseComponent状态组件（用户需要继承该组件来自定义创建）
/// 每个StateBaseComponent状态组件都可以拥有-个子StateMachineEntity状态机，以对状态机进行分层
/// 可以通过StateBaseComponent状态组件定义虚函数Init以便在状态组件中传递初始化数据
/// StateBaseComponent状态组件之间可以定义一个数据组件作为数据传递桥梁
/// 
/// 是通过StateComponentAttribute来关联 控制器，状态机，状态组件
/// </summary>
public class StateCategoryManager : Logic.Singleton<StateCategoryManager>
{
    public class ControllerStateCategoryInfo
    {
        public List<Type> ControllerTypeList;
        public int ControllerStateCategoryEnum;
        public string WorkStreamDataFile;
        
        public BaseWorkStreamConfigData m_BaseWorkStreamConfigData;
    }

    public class BaseWorkStreamConfigData { }

    public class SingleWorkStreamConfigData : BaseWorkStreamConfigData
    {
        public byte[] DataBytes;
        public Dictionary<uint, uint> DataBytePosDic;
        public Dictionary<uint, WorkStreamData> DataDic;
    }

    public class MultiWorkStreamConfigData : BaseWorkStreamConfigData
    {
        public Dictionary<uint, SingleWorkStreamConfigData> m_DataDic = new Dictionary<uint, SingleWorkStreamConfigData>();
    }

    //key=state种类
    private Dictionary<int, StateECManager> _stateCategoryDic;

    public List<ControllerStateCategoryInfo> m_ControllerStateCategoryInfoList;

    public void OnAwake(bool isPlaying = true)
    {
        if (_stateCategoryDic == null)
        {
            _stateCategoryDic = new Dictionary<int, StateECManager>();
            if (m_ControllerStateCategoryInfoList == null)
                m_ControllerStateCategoryInfoList = new List<ControllerStateCategoryInfo>();

            Type[] types = isPlaying ? AssemblyManager.Instance.GetTypes() : System.Reflection.Assembly.GetAssembly(GetType()).GetTypes();
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(StateComponentAttribute), false);
                if (attrs.Length > 0)
                {
                    foreach (var attr in attrs)
                    {
                        StateComponentAttribute sca = (StateComponentAttribute)attr;

                        int stateCategory = sca.m_StateCategory;
                        int defineState = sca.m_DefineState;

                        StateECManager stateECManager;
                        if (!_stateCategoryDic.TryGetValue(stateCategory, out stateECManager) || stateECManager == null)
                        {
                            stateECManager = new StateECManager();
                            _stateCategoryDic[stateCategory] = stateECManager;
                        }

                        stateECManager.m_StateToTypeDic[defineState] = type;
                    }
                }

                attrs = type.GetCustomAttributes(typeof(StateControllerAttribute), false);
                if (attrs.Length > 0)
                {
                    foreach (var attr in attrs)
                    {
                        StateControllerAttribute sca = (StateControllerAttribute)attr;

                        bool isExist = false;
                        for (int csciIndex = 0, csciCount = m_ControllerStateCategoryInfoList.Count; csciIndex < csciCount; csciIndex++)
                        {
                            ControllerStateCategoryInfo controllerStateCategoryInfo = m_ControllerStateCategoryInfoList[csciIndex];
                            if (controllerStateCategoryInfo == null)
                                continue;

                            if (controllerStateCategoryInfo.ControllerStateCategoryEnum == sca.m_StateCategory)
                            {
                                if (controllerStateCategoryInfo.ControllerTypeList == null)
                                    controllerStateCategoryInfo.ControllerTypeList = new List<Type>();

                                controllerStateCategoryInfo.ControllerTypeList.Add(type);

                                isExist = true;
                                break;
                            }
                        }

                        if (!isExist)
                        {
                            ControllerStateCategoryInfo csci = new ControllerStateCategoryInfo();
                            csci.ControllerTypeList = new List<Type>();
                            csci.ControllerTypeList.Add(type);
                            csci.ControllerStateCategoryEnum = sca.m_StateCategory;
                            csci.WorkStreamDataFile = sca.m_WorkStreamDataFile;

                            m_ControllerStateCategoryInfoList.Add(csci);
                        }
                    }
                }
            }
        }
    }

    public StateECManager GetStateECManager(int stateCategory)
    {
        StateECManager stateECManager;
        _stateCategoryDic.TryGetValue(stateCategory, out stateECManager);

        return stateECManager;
    }

    public void Clear()
    {
        _stateCategoryDic = null;
    }
}

public class StateECManager
{
    //状态分层也必须使用同一个枚举定义的state，也就是一个StateController只能有一个DefineState枚举
    public Dictionary<int, Type> m_StateToTypeDic;

    public StateECManager()
    {
        m_StateToTypeDic = new Dictionary<int, Type>();
    }

    public int GetStateId(Type type)
    {
        foreach (var kv in m_StateToTypeDic)
        {
            if (kv.Value == type)
                return kv.Key;
        }

        return 0;
    }

    public Type GetStateType(int stateId)
    {
        Type stateType;
        m_StateToTypeDic.TryGetValue(stateId, out stateType);

        return stateType;
    }
}
