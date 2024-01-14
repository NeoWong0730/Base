using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

unsafe class Framework_Core_UI_FUIStack_Binder
{
    public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
    {
        BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        MethodBase method;
        FieldInfo field;
        Type[] args;
        Type type = typeof(Framework.Core.UI.FUIStack);
        args = new Type[] { typeof(UnityEngine.Transform), typeof(UnityEngine.Camera) };
        method = type.GetMethod("Init", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, Init_0);
        args = new Type[] { typeof(System.Int32), typeof(Framework.Core.UI.UIConfigData) };
        method = type.GetMethod("PreloadUI", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, PreloadUI_1);
        args = new Type[] { typeof(System.Int32), typeof(Framework.Core.UI.UIConfigData), typeof(System.Boolean), typeof(System.Object), typeof(System.Int32) };
        method = type.GetMethod("OpenUI", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, OpenUI_2);
        args = new Type[] { typeof(System.Int32), typeof(Framework.Core.UI.EUIState), typeof(System.Boolean) };
        method = type.GetMethod("HideUI", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, HideUI_3);
        args = new Type[] { typeof(System.Int32), typeof(System.Object) };
        method = type.GetMethod("SendMsg", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, SendMsg_4);
        args = new Type[] { typeof(System.Boolean) };
        method = type.GetMethod("ClearStackUI", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, ClearStackUI_5);
        args = new Type[] { };
        method = type.GetMethod("UpdateState", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, UpdateState_6);
        args = new Type[] { typeof(System.Int32), typeof(Framework.Core.UI.FUIBase).MakeByRefType() };
        method = type.GetMethod("TryGetUI", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, TryGetUI_7);
        args = new Type[] { };
        method = type.GetMethod("TopUIID", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, TopUIID_8);
        args = new Type[] { typeof(System.Int32) };
        method = type.GetMethod("GetUI", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, GetUI_9);

        args = new Type[] {};
        method = type.GetMethod("Update", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, Update_10);

        args = new Type[] { typeof(System.Single), typeof(System.Single) };
        method = type.GetMethod("LateUpdate", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, LateUpdate_11);
        args = new Type[] { };
        method = type.GetMethod("get_nReadyHideMainCameraRef", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, get_nReadyHideMainCameraRef_12);
        args = new Type[] { };
        method = type.GetMethod("get_nRealHideMainCameraRef", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, get_nRealHideMainCameraRef_13);
        args = new Type[] { };
        method = type.GetMethod("get_nReduceFrameRateRef", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, get_nReduceFrameRateRef_14);
        args = new Type[] { };
        method = type.GetMethod("get_nReduceMainCameraQualityRef", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, get_nReduceMainCameraQualityRef_15);

        field = type.GetField("eventEmitter", flag);
        app.RegisterCLRFieldGetter(field, get_eventEmitter_0);
        app.RegisterCLRFieldSetter(field, set_eventEmitter_0);
        app.RegisterCLRFieldBinding(field, CopyToStack_eventEmitter_0, AssignFromStack_eventEmitter_0);

        args = new Type[] { };
        method = type.GetConstructor(flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, Ctor_0);
    }

    static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* __ret = ILIntepreter.Minus(__esp, 0);

        var result_of_this_method = new Framework.Core.UI.FUIStack();

        return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
    }


    static StackObject* Init_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 3);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        UnityEngine.Camera @camera = (UnityEngine.Camera)typeof(UnityEngine.Camera).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
        UnityEngine.Transform @root = (UnityEngine.Transform)typeof(UnityEngine.Transform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
        Framework.Core.UI.FUIStack instance_of_this_method = (Framework.Core.UI.FUIStack)typeof(Framework.Core.UI.FUIStack).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        instance_of_this_method.Init(@root, @camera);

        return __ret;
    }

    static StackObject* PreloadUI_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 3);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.UIConfigData @configData = (Framework.Core.UI.UIConfigData)typeof(Framework.Core.UI.UIConfigData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
        System.Int32 @id = ptr_of_this_method->Value;

        ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
        Framework.Core.UI.FUIStack instance_of_this_method = (Framework.Core.UI.FUIStack)typeof(Framework.Core.UI.FUIStack).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        instance_of_this_method.PreloadUI(@id, @configData);

        return __ret;
    }

    static StackObject* OpenUI_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 6);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        System.Int32 @parentID = ptr_of_this_method->Value;

        ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
        System.Object @arg = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
        System.Boolean @immediate = ptr_of_this_method->Value == 1;

        ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
        Framework.Core.UI.UIConfigData @configData = (Framework.Core.UI.UIConfigData)typeof(Framework.Core.UI.UIConfigData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
        System.Int32 @id = ptr_of_this_method->Value;

        ptr_of_this_method = ILIntepreter.Minus(__esp, 6);
        Framework.Core.UI.FUIStack instance_of_this_method = (Framework.Core.UI.FUIStack)typeof(Framework.Core.UI.FUIStack).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        instance_of_this_method.OpenUI(@id, @configData, @immediate, @arg, @parentID);

        return __ret;
    }

    static StackObject* HideUI_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 4);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        System.Boolean @immediate = ptr_of_this_method->Value == 1;

        ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
        Framework.Core.UI.EUIState @state = (Framework.Core.UI.EUIState)ptr_of_this_method->Value;
        __intp.Free(ptr_of_this_method);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
        System.Int32 @id = ptr_of_this_method->Value;

        ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
        Framework.Core.UI.FUIStack instance_of_this_method = (Framework.Core.UI.FUIStack)typeof(Framework.Core.UI.FUIStack).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        instance_of_this_method.HideUI(@id, @state, @immediate);

        return __ret;
    }

    static StackObject* SendMsg_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 3);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        System.Object @arg = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
        System.Int32 @id = ptr_of_this_method->Value;

        ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
        Framework.Core.UI.FUIStack instance_of_this_method = (Framework.Core.UI.FUIStack)typeof(Framework.Core.UI.FUIStack).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        instance_of_this_method.SendMsg(@id, @arg);

        return __ret;
    }

    static StackObject* ClearStackUI_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 2);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        System.Boolean @destroy = ptr_of_this_method->Value == 1;

        ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
        Framework.Core.UI.FUIStack instance_of_this_method = (Framework.Core.UI.FUIStack)typeof(Framework.Core.UI.FUIStack).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        instance_of_this_method.ClearStackUI(@destroy);

        return __ret;
    }

    static StackObject* UpdateState_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIStack instance_of_this_method = (Framework.Core.UI.FUIStack)typeof(Framework.Core.UI.FUIStack).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        instance_of_this_method.UpdateState();

        return __ret;
    }

    static StackObject* TryGetUI_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 3);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIBase @ui = (Framework.Core.UI.FUIBase)typeof(Framework.Core.UI.FUIBase).CheckCLRTypes(__intp.RetriveObject(ptr_of_this_method, __mStack));

        ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
        System.Int32 @id = ptr_of_this_method->Value;

        ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
        Framework.Core.UI.FUIStack instance_of_this_method = (Framework.Core.UI.FUIStack)typeof(Framework.Core.UI.FUIStack).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));

        var result_of_this_method = instance_of_this_method.TryGetUI(@id, out @ui);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        switch (ptr_of_this_method->ObjectType)
        {
            case ObjectTypes.StackObjectReference:
                {
                    var ___dst = ILIntepreter.ResolveReference(ptr_of_this_method);
                    object ___obj = @ui;
                    if (___dst->ObjectType >= ObjectTypes.Object)
                    {
                        if (___obj is CrossBindingAdaptorType)
                            ___obj = ((CrossBindingAdaptorType)___obj).ILInstance;
                        __mStack[___dst->Value] = ___obj;
                    }
                    else
                    {
                        ILIntepreter.UnboxObject(___dst, ___obj, __mStack, __domain);
                    }
                }
                break;
            case ObjectTypes.FieldReference:
                {
                    var ___obj = __mStack[ptr_of_this_method->Value];
                    if (___obj is ILTypeInstance)
                    {
                        ((ILTypeInstance)___obj)[ptr_of_this_method->ValueLow] = @ui;
                    }
                    else
                    {
                        var ___type = __domain.GetType(___obj.GetType()) as CLRType;
                        ___type.SetFieldValue(ptr_of_this_method->ValueLow, ref ___obj, @ui);
                    }
                }
                break;
            case ObjectTypes.StaticFieldReference:
                {
                    var ___type = __domain.GetType(ptr_of_this_method->Value);
                    if (___type is ILType)
                    {
                        ((ILType)___type).StaticInstance[ptr_of_this_method->ValueLow] = @ui;
                    }
                    else
                    {
                        ((CLRType)___type).SetStaticFieldValue(ptr_of_this_method->ValueLow, @ui);
                    }
                }
                break;
            case ObjectTypes.ArrayReference:
                {
                    var instance_of_arrayReference = __mStack[ptr_of_this_method->Value] as Framework.Core.UI.FUIBase[];
                    instance_of_arrayReference[ptr_of_this_method->ValueLow] = @ui;
                }
                break;
        }

        __intp.Free(ptr_of_this_method);
        ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
        __intp.Free(ptr_of_this_method);
        ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
        __intp.Free(ptr_of_this_method);
        __ret->ObjectType = ObjectTypes.Integer;
        __ret->Value = result_of_this_method ? 1 : 0;
        return __ret + 1;
    }

    static StackObject* TopUIID_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIStack instance_of_this_method = (Framework.Core.UI.FUIStack)typeof(Framework.Core.UI.FUIStack).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        var result_of_this_method = instance_of_this_method.TopUIID();

        __ret->ObjectType = ObjectTypes.Integer;
        __ret->Value = result_of_this_method;
        return __ret + 1;
    }

    static StackObject* GetUI_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 2);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        System.Int32 @id = ptr_of_this_method->Value;

        ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
        Framework.Core.UI.FUIStack instance_of_this_method = (Framework.Core.UI.FUIStack)typeof(Framework.Core.UI.FUIStack).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        var result_of_this_method = instance_of_this_method.GetUI(@id);

        object obj_result_of_this_method = result_of_this_method;
        if (obj_result_of_this_method is CrossBindingAdaptorType)
        {
            return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
        }
        return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
    }

    static StackObject* Update_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIStack instance_of_this_method = (Framework.Core.UI.FUIStack)typeof(Framework.Core.UI.FUIStack).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        instance_of_this_method.Update();

        return __ret;
    }

    static StackObject* LateUpdate_11(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 3);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        System.Single @usdt = *(float*)&ptr_of_this_method->Value;

        ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
        System.Single @dt = *(float*)&ptr_of_this_method->Value;

        ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
        Framework.Core.UI.FUIStack instance_of_this_method = (Framework.Core.UI.FUIStack)typeof(Framework.Core.UI.FUIStack).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        instance_of_this_method.LateUpdate(@dt, @usdt);

        return __ret;
    }

    static StackObject* get_nReadyHideMainCameraRef_12(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIStack instance_of_this_method = (Framework.Core.UI.FUIStack)typeof(Framework.Core.UI.FUIStack).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        var result_of_this_method = instance_of_this_method.nReadyHideMainCameraRef;

        __ret->ObjectType = ObjectTypes.Integer;
        __ret->Value = result_of_this_method;
        return __ret + 1;
    }

    static StackObject* get_nRealHideMainCameraRef_13(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIStack instance_of_this_method = (Framework.Core.UI.FUIStack)typeof(Framework.Core.UI.FUIStack).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        var result_of_this_method = instance_of_this_method.nRealHideMainCameraRef;

        __ret->ObjectType = ObjectTypes.Integer;
        __ret->Value = result_of_this_method;
        return __ret + 1;
    }

    static StackObject* get_nReduceFrameRateRef_14(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIStack instance_of_this_method = (Framework.Core.UI.FUIStack)typeof(Framework.Core.UI.FUIStack).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        var result_of_this_method = instance_of_this_method.nReduceFrameRateRef;

        __ret->ObjectType = ObjectTypes.Integer;
        __ret->Value = result_of_this_method;
        return __ret + 1;
    }

    static StackObject* get_nReduceMainCameraQualityRef_15(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIStack instance_of_this_method = (Framework.Core.UI.FUIStack)typeof(Framework.Core.UI.FUIStack).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        var result_of_this_method = instance_of_this_method.nReduceMainCameraQualityRef;

        __ret->ObjectType = ObjectTypes.Integer;
        __ret->Value = result_of_this_method;
        return __ret + 1;
    }


    static object get_eventEmitter_0(ref object o)
    {
        return ((Framework.Core.UI.FUIStack)o).eventEmitter;
    }

    static StackObject* CopyToStack_eventEmitter_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
    {
        var result_of_this_method = ((Framework.Core.UI.FUIStack)o).eventEmitter;
        return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
    }

    static void set_eventEmitter_0(ref object o, object v)
    {
        ((Framework.Core.UI.FUIStack)o).eventEmitter = (Lib.Core.EventEmitter<Framework.Core.UI.FUIStack.EUIStackEvent>)v;
    }

    static StackObject* AssignFromStack_eventEmitter_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        Lib.Core.EventEmitter<Framework.Core.UI.FUIStack.EUIStackEvent> @eventEmitter = (Lib.Core.EventEmitter<Framework.Core.UI.FUIStack.EUIStackEvent>)typeof(Lib.Core.EventEmitter<Framework.Core.UI.FUIStack.EUIStackEvent>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        ((Framework.Core.UI.FUIStack)o).eventEmitter = @eventEmitter;
        return ptr_of_this_method;
    }



}