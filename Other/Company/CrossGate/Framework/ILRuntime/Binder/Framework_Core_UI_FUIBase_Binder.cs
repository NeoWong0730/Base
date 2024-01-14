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

unsafe class Framework_Core_UI_FUIBase_Binder
{
    static MethodBase method_DoLoaded;
    static MethodBase method_DoBeginEnter;
    static MethodBase method_DoEndEnter;
    static MethodBase method_DoBeginExit;
    static MethodBase method_DoEndExit;

    static MethodBase methodOnOpened;
    static MethodBase methodOnOpen;
    static MethodBase method_CloseOrDestroy;
    static MethodBase method_SetActive;

    public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
    {
        BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        MethodBase method;
        FieldInfo field;

        Type type = typeof(Framework.Core.UI.FUIBase);

        field = type.GetField("nParentID", flag);
        app.RegisterCLRFieldGetter(field, get_nParentID_0);
        app.RegisterCLRFieldSetter(field, set_nParentID_0);
        app.RegisterCLRFieldBinding(field, CopyToStack_nParentID_0, AssignFromStack_nParentID_0);

        Type[] args;
        args = new Type[] { };

        method = type.GetConstructor(flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, Ctor_0);

        method = type.GetMethod("get_gameObject", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, get_gameObject_0);

        method = type.GetMethod("get_transform", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, get_transform_1);

        method = type.GetMethod("get_nSortingOrder", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, get_nSortingOrder_2);

        method = type.GetMethod("get_nID", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, get_nID_4);

        method = type.GetMethod("get_canvas", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, get_canvas_5);

        method = type.GetMethod("get_bLoaded", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, get_bLoaded_6);

        method = type.GetMethod("get_isOpen", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, get_isOpen_7);

        method = type.GetMethod("get_isVisibleAndOpen", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, get_isVisibleAndOpen_8);

        method = type.GetMethod("get_isVisible", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, get_isVisible_10);

        method = type.GetMethod("get_eState", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, get_eState_11);

        method = type.GetMethod("get_eOptions", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, get_eOptions_12);

        method = type.GetMethod("get_deltaTime", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, get_deltaTime);

        method = type.GetMethod("get_unscaledDeltaTime", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, get_unscaledDeltaTime);

        method_DoLoaded = type.GetMethod("_DoLoaded", flag | BindingFlags.NonPublic, null, args, null);
        app.RegisterCLRMethodRedirection(method_DoLoaded, _DoLoaded);

        method_DoBeginEnter = type.GetMethod("_DoBeginEnter", flag | BindingFlags.NonPublic, null, args, null);
        app.RegisterCLRMethodRedirection(method_DoBeginEnter, _DoBeginEnter);

        method_DoEndEnter = type.GetMethod("_DoEndEnter", flag | BindingFlags.NonPublic, null, args, null);
        app.RegisterCLRMethodRedirection(method_DoEndEnter, _DoEndEnter);

        method_DoBeginExit = type.GetMethod("_DoBeginExit", flag | BindingFlags.NonPublic, null, args, null);
        app.RegisterCLRMethodRedirection(method_DoBeginExit, _DoBeginExit);

        method_DoEndExit = type.GetMethod("_DoEndExit", flag | BindingFlags.NonPublic, null, args, null);
        app.RegisterCLRMethodRedirection(method_DoEndExit, _DoEndExit);

        method_CloseOrDestroy = type.GetMethod("_CloseOrDestroy", flag | BindingFlags.NonPublic, null, args, null);
        app.RegisterCLRMethodRedirection(method_CloseOrDestroy, _CloseOrDestroy);

        //methodOnOpened = type.GetMethod("OnOpened", flag | BindingFlags.NonPublic, null, args, null);
        //app.RegisterCLRMethodRedirection(methodOnOpened, OnOpened);        

        //args = new Type[] { typeof(System.Object) };
        //methodOnOpen = type.GetMethod("OnOpen", flag | BindingFlags.NonPublic, null, args, null);
        //app.RegisterCLRMethodRedirection(methodOnOpen, OnOpen);

        args = new Type[] { typeof(System.Int32), typeof(System.Boolean) };
        method = type.GetMethod("SetSortingOrder", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, SetSortingOrder_3);

        args = new Type[] { typeof(System.Single), typeof(System.Boolean) };
        method = type.GetMethod("Show", flag, null, args, null);
        app.RegisterCLRMethodRedirection(method, Show_9);
    }

    static StackObject* OnOpen(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 2);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        System.Object @arg = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
        Framework.Core.UI.FUIBase instance_of_this_method = (Framework.Core.UI.FUIBase)typeof(Framework.Core.UI.FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        methodOnOpen.Invoke(instance_of_this_method, new object[] { @arg });

        return __ret;
    }

    static StackObject* OnOpened(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIBase instance_of_this_method = (Framework.Core.UI.FUIBase)typeof(Framework.Core.UI.FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        methodOnOpened.Invoke(instance_of_this_method, null);
        //instance_of_this_method._DoBeginEnter();            

        return __ret;
    }

    static StackObject* _DoBeginEnter(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIBase instance_of_this_method = (Framework.Core.UI.FUIBase)typeof(Framework.Core.UI.FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        method_DoBeginEnter.Invoke(instance_of_this_method, null);
        //instance_of_this_method._DoBeginEnter();

        return __ret;
    }

    static StackObject* _DoEndEnter(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIBase instance_of_this_method = (Framework.Core.UI.FUIBase)typeof(Framework.Core.UI.FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        method_DoEndEnter.Invoke(instance_of_this_method, null);
        //instance_of_this_method._DoEndEnter();

        return __ret;
    }

    static StackObject* _DoBeginExit(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIBase instance_of_this_method = (Framework.Core.UI.FUIBase)typeof(Framework.Core.UI.FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        method_DoBeginExit.Invoke(instance_of_this_method, null);
        //instance_of_this_method._DoBeginExit();

        return __ret;
    }

    static StackObject* _DoEndExit(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIBase instance_of_this_method = (Framework.Core.UI.FUIBase)typeof(Framework.Core.UI.FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        method_DoEndExit.Invoke(instance_of_this_method, null);
        //instance_of_this_method._DoEndExit();

        return __ret;
    }

    static StackObject* _DoLoaded(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIBase instance_of_this_method = (Framework.Core.UI.FUIBase)typeof(Framework.Core.UI.FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        method_DoLoaded.Invoke(instance_of_this_method, null);
        //instance_of_this_method._DoLoaded();

        return __ret;
    }

    static StackObject* _CloseOrDestroy(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIBase instance_of_this_method = (Framework.Core.UI.FUIBase)typeof(Framework.Core.UI.FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        method_CloseOrDestroy.Invoke(instance_of_this_method, null);
        //instance_of_this_method._DoBeginEnter();            

        return __ret;
    }

    static StackObject* get_deltaTime(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIBase instance_of_this_method = (Framework.Core.UI.FUIBase)typeof(Framework.Core.UI.FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        var result_of_this_method = instance_of_this_method.deltaTime;

        __ret->ObjectType = ObjectTypes.Float;
        *(float*)&__ret->Value = result_of_this_method;
        return __ret + 1;
    }

    static StackObject* get_unscaledDeltaTime(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIBase instance_of_this_method = (Framework.Core.UI.FUIBase)typeof(Framework.Core.UI.FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        var result_of_this_method = instance_of_this_method.unscaledDeltaTime;

        __ret->ObjectType = ObjectTypes.Float;
        *(float*)&__ret->Value = result_of_this_method;
        return __ret + 1;
    }

    static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* __ret = ILIntepreter.Minus(__esp, 0);

        var result_of_this_method = new Framework.Core.UI.FUIBase();

        return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
    }

    static StackObject* get_gameObject_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIBase instance_of_this_method = (Framework.Core.UI.FUIBase)typeof(Framework.Core.UI.FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        var result_of_this_method = instance_of_this_method.gameObject;

        return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
    }

    static StackObject* get_transform_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIBase instance_of_this_method = (Framework.Core.UI.FUIBase)typeof(Framework.Core.UI.FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        var result_of_this_method = instance_of_this_method.transform;

        return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
    }

    static StackObject* get_nSortingOrder_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIBase instance_of_this_method = (Framework.Core.UI.FUIBase)typeof(Framework.Core.UI.FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        var result_of_this_method = instance_of_this_method.nSortingOrder;

        __ret->ObjectType = ObjectTypes.Integer;
        __ret->Value = result_of_this_method;
        return __ret + 1;
    }

    static StackObject* SetSortingOrder_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 3);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        System.Boolean @force = ptr_of_this_method->Value == 1;

        ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
        System.Int32 @order = ptr_of_this_method->Value;

        ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
        Framework.Core.UI.FUIBase instance_of_this_method = (Framework.Core.UI.FUIBase)typeof(Framework.Core.UI.FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        instance_of_this_method.SetSortingOrder(@order, @force);

        return __ret;
    }

    static StackObject* get_nID_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIBase instance_of_this_method = (Framework.Core.UI.FUIBase)typeof(Framework.Core.UI.FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        var result_of_this_method = instance_of_this_method.nID;

        __ret->ObjectType = ObjectTypes.Integer;
        __ret->Value = result_of_this_method;
        return __ret + 1;
    }

    static StackObject* get_canvas_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIBase instance_of_this_method = (Framework.Core.UI.FUIBase)typeof(Framework.Core.UI.FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        var result_of_this_method = instance_of_this_method.canvas;

        return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
    }

    static StackObject* get_bLoaded_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIBase instance_of_this_method = (Framework.Core.UI.FUIBase)typeof(Framework.Core.UI.FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        var result_of_this_method = instance_of_this_method.bLoaded;

        __ret->ObjectType = ObjectTypes.Integer;
        __ret->Value = result_of_this_method ? 1 : 0;
        return __ret + 1;
    }

    static StackObject* get_isOpen_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIBase instance_of_this_method = (Framework.Core.UI.FUIBase)typeof(Framework.Core.UI.FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        var result_of_this_method = instance_of_this_method.isOpen;

        __ret->ObjectType = ObjectTypes.Integer;
        __ret->Value = result_of_this_method ? 1 : 0;
        return __ret + 1;
    }

    static StackObject* get_isVisibleAndOpen_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIBase instance_of_this_method = (Framework.Core.UI.FUIBase)typeof(Framework.Core.UI.FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        var result_of_this_method = instance_of_this_method.isVisibleAndOpen;

        __ret->ObjectType = ObjectTypes.Integer;
        __ret->Value = result_of_this_method ? 1 : 0;
        return __ret + 1;
    }

    static StackObject* Show_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 3);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        System.Boolean @stackHide = ptr_of_this_method->Value == 1;

        ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
        System.Single @delay = *(float*)&ptr_of_this_method->Value;

        ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
        Framework.Core.UI.FUIBase instance_of_this_method = (Framework.Core.UI.FUIBase)typeof(Framework.Core.UI.FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        instance_of_this_method.Show(@delay, @stackHide);

        return __ret;
    }

    static StackObject* get_isVisible_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIBase instance_of_this_method = (Framework.Core.UI.FUIBase)typeof(Framework.Core.UI.FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        var result_of_this_method = instance_of_this_method.isVisible;

        __ret->ObjectType = ObjectTypes.Integer;
        __ret->Value = result_of_this_method ? 1 : 0;
        return __ret + 1;
    }

    static StackObject* get_eState_11(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIBase instance_of_this_method = (Framework.Core.UI.FUIBase)typeof(Framework.Core.UI.FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        var result_of_this_method = instance_of_this_method.eState;

        __ret->ObjectType = ObjectTypes.Integer;
        __ret->Value = (int)result_of_this_method;
        return __ret + 1;
    }

    static StackObject* get_eOptions_12(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        StackObject* ptr_of_this_method;
        StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        Framework.Core.UI.FUIBase instance_of_this_method = (Framework.Core.UI.FUIBase)typeof(Framework.Core.UI.FUIBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        __intp.Free(ptr_of_this_method);

        var result_of_this_method = instance_of_this_method.eOptions;

        __ret->ObjectType = ObjectTypes.Integer;
        __ret->Value = (int)result_of_this_method;
        return __ret + 1;
    }


    static object get_nParentID_0(ref object o)
    {
        return ((Framework.Core.UI.FUIBase)o).nParentID;
    }

    static StackObject* CopyToStack_nParentID_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
    {
        var result_of_this_method = ((Framework.Core.UI.FUIBase)o).nParentID;
        __ret->ObjectType = ObjectTypes.Integer;
        __ret->Value = result_of_this_method;
        return __ret + 1;
    }

    static void set_nParentID_0(ref object o, object v)
    {
        ((Framework.Core.UI.FUIBase)o).nParentID = (System.Int32)v;
    }

    static StackObject* AssignFromStack_nParentID_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        System.Int32 @nParentID = ptr_of_this_method->Value;
        ((Framework.Core.UI.FUIBase)o).nParentID = @nParentID;
        return ptr_of_this_method;
    }



}
