using UnityEngine;
using System;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;

public class MonoBehaviourAdapter : CrossBindingAdaptor
{
    public override Type BaseCLRType
    {
        get
        {
            return typeof(MonoBehaviour);
        }
    }

    public override Type AdaptorType
    {
        get
        {
            return typeof(Adaptor);
        }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);
    }

    public class Adaptor : MonoBehaviour, CrossBindingAdaptorType
    {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        public Adaptor()
        {

        }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            this.appdomain = appdomain;
            this.instance = instance;

            ToString();
        }

        public ILTypeInstance ILInstance { get { return instance; } set { instance = value; } }

        public ILRuntime.Runtime.Enviorment.AppDomain AppDomain { get { return appdomain; } set { appdomain = value; } }

        IMethod mAwakeMethod;
        bool mAwakeMethodGot;
        public void Awake()
        {
            //Unity会在ILRuntime准备好这个实例前调用Awake，所以这里暂时先不掉用
            if (instance != null)
            {
                if (!mAwakeMethodGot)
                {
                    mAwakeMethod = instance.Type.GetMethod("Awake", 0);
                    mAwakeMethodGot = true;
                }

                if (mAwakeMethod != null)
                {
                    try
                    {
                        appdomain.Invoke(mAwakeMethod, instance, null);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}这个类Awake报错 excetion: {e.ToString()}");
                    }
                }
            }
        }

        IMethod mOnEnableMethod;
        bool mOnEnableMethodGot;

        void OnEnable()
        {
            if (instance != null)
            {
                if (!mOnEnableMethodGot)
                {
                    mOnEnableMethod = instance.Type.GetMethod("OnEnable", 0);
                    mOnEnableMethodGot = true;
                }

                if (mOnEnableMethod != null)
                {
                    try
                    {
                        appdomain.Invoke(mOnEnableMethod, instance, null);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}这个类OnEnable报错 excetion: {e.ToString()}");
                    }
                }
            }
        }

        IMethod mStartMethod;
        bool mStartMethodGot;
        void Start()
        {
            if (instance != null)
            {
                if (!mStartMethodGot)
                {
                    mStartMethod = instance.Type.GetMethod("Start", 0);
                    mStartMethodGot = true;
                }

                if (mStartMethod != null)
                {
                    try
                    {
                        appdomain.Invoke(mStartMethod, instance, null);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}这个类Start报错 excetion: {e.ToString()}");
                    }
                }
            }
        }

        IMethod mFixedUpdateMethod;
        bool mFixedUpdateMethodGot;

        void FixedUpdate()
        {
            if (instance != null)
            {
                if (!mFixedUpdateMethodGot)
                {
                    mFixedUpdateMethod = instance.Type.GetMethod("FixedUpdate", 0);
                    mFixedUpdateMethodGot = true;
                }

                if (mFixedUpdateMethod != null)
                {
                    try
                    {
                        appdomain.Invoke(mFixedUpdateMethod, instance, null);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}这个类FixedUpdate报错 excetion: {e.ToString()}");
                    }
                }
            }
        }

        IMethod mUpdateMethod;
        bool mUpdateMethodGot;
        void Update()
        {
            if (instance != null)
            {
                if (!mUpdateMethodGot)
                {
                    mUpdateMethod = instance.Type.GetMethod("Update", 0);
                    mUpdateMethodGot = true;
                }

                if (mUpdateMethod != null)
                {
                    try
                    {
                        appdomain.Invoke(mUpdateMethod, instance, null);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}这个类Update报错 excetion: {e.ToString()}");
                    }
                }
            }
        }

        IMethod mLateUpdateMethod;
        bool mLateUpdateMethodGot;

        void LateUpdate()
        {
            if (instance != null)
            {
                if (!mLateUpdateMethodGot)
                {
                    mLateUpdateMethod = instance.Type.GetMethod("LateUpdate", 0);
                    mLateUpdateMethodGot = true;
                }

                if (mLateUpdateMethod != null)
                {
                    try
                    {
                        appdomain.Invoke(mLateUpdateMethod, instance, null);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}这个类LateUpdate报错 excetion: {e.ToString()}");
                    }
                }
            }
        }

        IMethod mOnGUIMethod;
        bool mOnGUIMethodGot;

        void OnGUI()
        {
            if (instance != null)
            {
                if (!mOnGUIMethodGot)
                {
                    mOnGUIMethod = instance.Type.GetMethod("OnGUI", 0);
                    mOnGUIMethodGot = true;
                }

                if (mOnGUIMethod != null)
                {
                    try
                    {
                        appdomain.Invoke(mOnGUIMethod, instance, null);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}这个类OnGUI报错 excetion: {e.ToString()}");
                    }
                }
            }
        }

        IMethod mOnDisableMethod;
        bool mOnDisableMetodGot;

        void OnDisable()
        {
            if (instance != null)
            {
                if (!mOnDisableMetodGot)
                {
                    mOnDisableMethod = instance.Type.GetMethod("OnDisable", 0);
                    mOnDisableMetodGot = true;
                }

                if (mOnDisableMethod != null)
                {
                    try
                    {
                        appdomain.Invoke(mOnDisableMethod, instance, null);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}这个类OnDisable报错 excetion: {e.ToString()}");
                    }
                }
            }
        }

        IMethod mOnDestroyMethod;
        bool mOnDestroyMethodGot;

        void OnDestroy()
        {
            if (instance != null)
            {
                if (!mOnDestroyMethodGot)
                {
                    mOnDestroyMethod = instance.Type.GetMethod("OnDestroy", 0);
                    mOnDestroyMethodGot = true;
                }

                if (mOnDestroyMethod != null)
                {
                    try
                    {
                        appdomain.Invoke(mOnDestroyMethod, instance, null);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}这个类OnDestroy报错 excetion: {e.ToString()}");
                    }
                }
            }
        }

        public override string ToString()
        {
            if (instance != null)
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return instance.ToString();
                }
                else
                    return instance.Type.FullName;
            }
            return string.Empty;
        }
    }
}
