using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace Framework.Adaptor
{
    public class FUIBaseAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(Framework.Core.UI.FUIBase);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public class Adapter : Framework.Core.UI.FUIBase, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            CrossBindingMethodInfo m_DoLoaded_1 = new CrossBindingMethodInfo("_DoLoaded");
            CrossBindingMethodInfo m_DoBeginEnter_2 = new CrossBindingMethodInfo("_DoBeginEnter");
            CrossBindingMethodInfo m_DoEndEnter_3 = new CrossBindingMethodInfo("_DoEndEnter");
            CrossBindingMethodInfo m_DoBeginExit_4 = new CrossBindingMethodInfo("_DoBeginExit");
            CrossBindingMethodInfo m_DoEndExit_5 = new CrossBindingMethodInfo("_DoEndExit");
            CrossBindingMethodInfo m_CloseOrDestroy_6 = new CrossBindingMethodInfo("_CloseOrDestroy");            

            protected override void _DoLoaded()
            {
                if (m_DoLoaded_1.CheckShouldInvokeBase(this.instance))
                    base._DoLoaded();
                else
                    m_DoLoaded_1.Invoke(this.instance);
            }

            protected override void _DoBeginEnter()
            {
                if (m_DoBeginEnter_2.CheckShouldInvokeBase(this.instance))
                    base._DoBeginEnter();
                else
                    m_DoBeginEnter_2.Invoke(this.instance);
            }

            protected override void _DoEndEnter()
            {
                if (m_DoEndEnter_3.CheckShouldInvokeBase(this.instance))
                    base._DoEndEnter();
                else
                    m_DoEndEnter_3.Invoke(this.instance);
            }

            protected override void _DoBeginExit()
            {
                if (m_DoBeginExit_4.CheckShouldInvokeBase(this.instance))
                    base._DoBeginExit();
                else
                    m_DoBeginExit_4.Invoke(this.instance);
            }

            protected override void _DoEndExit()
            {
                if (m_DoEndExit_5.CheckShouldInvokeBase(this.instance))
                    base._DoEndExit();
                else
                    m_DoEndExit_5.Invoke(this.instance);
            }

            protected override void _CloseOrDestroy()
            {
                if (m_CloseOrDestroy_6.CheckShouldInvokeBase(this.instance))
                    base._CloseOrDestroy();
                else
                    m_CloseOrDestroy_6.Invoke(this.instance);
            }            

#region 生命周期
            //TODO 优化未被重写的生命周期函数 直接不调度

            IMethod mOnForeQuitMethod_1;
            bool mOnForeQuitMethodGot_1;
            public override void OnForeQuit()
            {
                if (!mOnForeQuitMethodGot_1)
                {
                    mOnForeQuitMethod_1 = instance.Type.GetMethod("OnForeQuit", 0);
                    mOnForeQuitMethodGot_1 = true;
                }

                if (mOnForeQuitMethod_1 != null)
                {
                    try
                    {
                        appdomain.Invoke(mOnForeQuitMethod_1, instance, null);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}.OnForeQuit excetion: {e.ToString()}");
                    }
                }
            }

            IMethod mOnSetDataMethod_2;
            bool mOnSetDataMethodGot_2;
            public override void OnSetData(System.Object arg)
            {
                if (!mOnSetDataMethodGot_2)
                {
                    mOnSetDataMethod_2 = instance.Type.GetMethod("OnSetData", 1);
                    mOnSetDataMethodGot_2 = true;
                }

                if (mOnSetDataMethod_2 != null)
                {
                    try
                    {
                        using (var ctx = appdomain.BeginInvoke(mOnSetDataMethod_2))
                        {
                            ctx.PushObject(instance);
                            ctx.PushObject(arg);
                            ctx.Invoke();
                        }
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}.OnSetData excetion: {e.ToString()}");
                    }
                }
            }

            IMethod mOnInitMethod_3;
            bool mOnInitMethodGot_3;
            protected override void OnInit()
            {
                if (!mOnInitMethodGot_3)
                {
                    mOnInitMethod_3 = instance.Type.GetMethod("OnInit", 0);
                    mOnInitMethodGot_3 = true;
                }

                if (mOnInitMethod_3 != null)
                {
                    try
                    {
                        appdomain.Invoke(mOnInitMethod_3, instance, null);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}.OnInit excetion: {e.ToString()}");
                    }
                }
            }

            IMethod mOnOpenMethod_4;
            bool mOnOpenMethodGot_4;
            protected override void OnOpen(System.Object arg)
            {
                if (!mOnOpenMethodGot_4)
                {
                    mOnOpenMethod_4 = instance.Type.GetMethod("OnOpen", 1);
                    mOnOpenMethodGot_4 = true;
                }

                if (mOnOpenMethod_4 != null)
                {
                    try
                    {
                        using (var ctx = appdomain.BeginInvoke(mOnOpenMethod_4))
                        {
                            ctx.PushObject(instance);
                            ctx.PushObject(arg);
                            ctx.Invoke();
                        }
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}.OnOpen excetion: {e.ToString()}");
                    }
                }
            }

            IMethod mOnLoadedMethod_5;
            bool mOnLoadedMethodGot_5;
            protected override void OnLoaded()
            {
                if (!mOnLoadedMethodGot_5)
                {
                    mOnLoadedMethod_5 = instance.Type.GetMethod("OnLoaded", 0);
                    mOnLoadedMethodGot_5 = true;
                }

                if (mOnLoadedMethod_5 != null)
                {
                    try
                    {
                        appdomain.Invoke(mOnLoadedMethod_5, instance, null);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}.OnLoaded excetion: {e.ToString()}");
                    }
                }
            }

            IMethod mOnUpdateMethod_6;
            bool mOnUpdateMethodGot_6;
            protected override void OnUpdate()
            {
                if (!mOnUpdateMethodGot_6)
                {
                    mOnUpdateMethod_6 = instance.Type.GetMethod("OnUpdate", 0);
                    mOnUpdateMethodGot_6 = true;
                }

                if (mOnUpdateMethod_6 != null)
                {
                    try
                    {
                        appdomain.Invoke(mOnUpdateMethod_6, instance, null);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}.OnUpdate excetion: {e.ToString()}");
                    }
                }
            }

            IMethod mOnLateUpdateMethod_7;
            bool mOnLateUpdateMethodGot_7;
            protected override void OnLateUpdate(System.Single dt, System.Single usdt)
            {
                if (!mOnLateUpdateMethodGot_7)
                {
                    mOnLateUpdateMethod_7 = instance.Type.GetMethod("OnLateUpdate", 2);
                    mOnLateUpdateMethodGot_7 = true;
                }

                if (mOnLateUpdateMethod_7 != null)
                {
                    try
                    {
                        using (var ctx = appdomain.BeginInvoke(mOnLateUpdateMethod_7))
                        {
                            ctx.PushObject(instance);
                            ctx.PushFloat(dt);
                            ctx.PushFloat(usdt);
                            ctx.Invoke();
                        }
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}.OnLateUpdate excetion: {e.ToString()}");
                    }
                }
            }

            IMethod mOnOpenedMethod_8;
            bool mOnOpenedMethodGot_8;
            protected override void OnOpened()
            {
                if (!mOnOpenedMethodGot_8)
                {
                    mOnOpenedMethod_8 = instance.Type.GetMethod("OnOpened", 0);
                    mOnOpenedMethodGot_8 = true;
                }

                if (mOnOpenedMethod_8 != null)
                {
                    try
                    {
                        appdomain.Invoke(mOnOpenedMethod_8, instance, null);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}.OnOpened excetion: {e.ToString()}");
                    }
                }
            }

            IMethod mOnShowMethod_9;
            bool mOnShowMethodGot_9;
            bool isInvoking;
            protected override void OnShow()
            {
                if (!mOnShowMethodGot_9)
                {
                    mOnShowMethod_9 = instance.Type.GetMethod("OnShow", 0);
                    mOnShowMethodGot_9 = true;
                }

                if (mOnShowMethod_9 != null && !isInvoking)
                {
                    isInvoking = true;
                    try
                    {                        
                        appdomain.Invoke(mOnShowMethod_9, instance, null);                        
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}.OnShow excetion: {e.ToString()}");
                    }
                    isInvoking = false;
                }
            }

            IMethod mOnShowEndMethod_10;
            bool mOnShowEndMethodGot_10;
            protected override void OnShowEnd()
            {
                if (!mOnShowEndMethodGot_10)
                {
                    mOnShowEndMethod_10 = instance.Type.GetMethod("OnShowEnd", 0);                    
                    mOnShowEndMethodGot_10 = true;
                }

                if (mOnShowEndMethod_10 != null)
                {
                    try
                    {
                        appdomain.Invoke(mOnShowEndMethod_10, instance, null);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}.OnShowEnd excetion: {e.ToString()}");
                    }
                }
            }

            IMethod mOnHideStartMethod_11;
            bool mOnHideStartMethodGot_11;
            protected override void OnHideStart()
            {
                if (!mOnHideStartMethodGot_11)
                {
                    mOnHideStartMethod_11 = instance.Type.GetMethod("OnHideStart", 0);                    
                    mOnHideStartMethodGot_11 = true;
                }

                if (mOnHideStartMethod_11 != null)
                {
                    try
                    {
                        appdomain.Invoke(mOnHideStartMethod_11, instance, null);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}.OnHideStart excetion: {e.ToString()}");
                    }
                }
            }

            IMethod mOnHideMethod_12;
            bool mOnHideMethodGot_12;
            protected override void OnHide()
            {
                if (!mOnHideMethodGot_12)
                {
                    mOnHideMethod_12 = instance.Type.GetMethod("OnHide", 0);
                    mOnHideMethodGot_12 = true;
                }

                if (mOnHideMethod_12 != null)
                {
                    try
                    {
                        appdomain.Invoke(mOnHideMethod_12, instance, null);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}.OnHide excetion: {e.ToString()}");
                    }
                }
            }


            IMethod mOnCloseMethod_13;
            bool mOnCloseMethodGot_13;
            protected override void OnClose()
            {
                if (!mOnCloseMethodGot_13)
                {
                    mOnCloseMethod_13 = instance.Type.GetMethod("OnClose", 0);
                    mOnCloseMethodGot_13 = true;
                }

                if (mOnCloseMethod_13 != null)
                {
                    try
                    {
                        appdomain.Invoke(mOnCloseMethod_13, instance, null);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}.OnClose excetion: {e.ToString()}");
                    }
                }
            }

            IMethod mOnDestroyMethod_14;
            bool mOnDestroyMethodGot_14;
            protected override void OnDestroy()
            {
                if (!mOnDestroyMethodGot_14)
                {
                    mOnDestroyMethod_14 = instance.Type.GetMethod("OnDestroy", 0);
                    mOnDestroyMethodGot_14 = true;
                }

                if (mOnDestroyMethod_14 != null)
                {
                    try
                    {
                        appdomain.Invoke(mOnDestroyMethod_14, instance, null);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}.OnDestroy excetion: {e.ToString()}");
                    }
                }
            }

            IMethod mProcessEventsMethod_15;
            bool mProcessEventsMethodGot_15;
            protected override void ProcessEvents(System.Boolean toRegister)
            {
                if (!mProcessEventsMethodGot_15)
                {
                    mProcessEventsMethod_15 = instance.Type.GetMethod("ProcessEvents", 1);
                    mProcessEventsMethodGot_15 = true;
                }

                if (mProcessEventsMethod_15 != null)
                {
                    try
                    {
                        using (var ctx = appdomain.BeginInvoke(mProcessEventsMethod_15))
                        {
                            ctx.PushObject(instance);
                            ctx.PushBool(toRegister);
                            ctx.Invoke();
                        }
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}.ProcessEvents excetion: {e.ToString()}");
                    }
                }
            }

            IMethod mProcessEventsForEnableMethod_16;
            bool mProcessEventsForEnableMethodGot_16;
            protected override void ProcessEventsForEnable(System.Boolean toRegister)
            {
                if (!mProcessEventsForEnableMethodGot_16)
                {
                    mProcessEventsForEnableMethod_16 = instance.Type.GetMethod("ProcessEventsForEnable", 1);

                    //判断是不是基类的函数 是的话就不调了
                    //if (mProcessEventsForEnableMethod_16 != null && mProcessEventsForEnableMethod_16.DeclearingType.BaseType.Equals(mProcessEventsForEnableMethod_16.DeclearingType.TypeForCLR))
                    //{
                    //    mProcessEventsForEnableMethod_16 = null;
                    //}

                    mProcessEventsForEnableMethodGot_16 = true;
                }

                if (mProcessEventsForEnableMethod_16 != null)
                {
                    try
                    {
                        using (var ctx = appdomain.BeginInvoke(mProcessEventsForEnableMethod_16))
                        {
                            ctx.PushObject(instance);
                            ctx.PushBool(toRegister);
                            ctx.Invoke();
                        }
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"sorry, {instance.Type.FullName}.ProcessEventsForEnable excetion: {e.ToString()}");
                    }
                }
            }

            #endregion 生命周期
            public override string ToString()
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
        }
    }
}

