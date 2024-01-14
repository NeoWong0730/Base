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
#if DEBUG && !DISABLE_ILRUNTIME_DEBUG
using AutoList = System.Collections.Generic.List<object>;
#else
using AutoList = ILRuntime.Other.UncheckedList<object>;
#endif
namespace ILRuntime.Runtime.Generated
{
    unsafe class Lib_Core_EventTrigger_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Lib.Core.EventTrigger);
            args = new Type[]{typeof(UnityEngine.GameObject)};
            method = type.GetMethod("Get", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Get_0);
            args = new Type[]{typeof(UnityEngine.Component)};
            method = type.GetMethod("Get", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Get_1);
            args = new Type[]{typeof(UnityEngine.EventSystems.EventTriggerType), typeof(UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData>)};
            method = type.GetMethod("AddEventListener", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddEventListener_2);
            args = new Type[]{};
            method = type.GetMethod("ClearEvents", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ClearEvents_3);
            args = new Type[]{typeof(UnityEngine.GameObject), typeof(UnityEngine.EventSystems.EventTriggerType), typeof(UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData>)};
            method = type.GetMethod("AddEventListener", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddEventListener_4);
            args = new Type[]{};
            method = type.GetMethod("get_IsDraging", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_IsDraging_5);

            field = type.GetField("onClick", flag);
            app.RegisterCLRFieldGetter(field, get_onClick_0);
            app.RegisterCLRFieldSetter(field, set_onClick_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onClick_0, AssignFromStack_onClick_0);
            field = type.GetField("onDragStart", flag);
            app.RegisterCLRFieldGetter(field, get_onDragStart_1);
            app.RegisterCLRFieldSetter(field, set_onDragStart_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_onDragStart_1, AssignFromStack_onDragStart_1);
            field = type.GetField("onDrag", flag);
            app.RegisterCLRFieldGetter(field, get_onDrag_2);
            app.RegisterCLRFieldSetter(field, set_onDrag_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_onDrag_2, AssignFromStack_onDrag_2);
            field = type.GetField("onDragEnd", flag);
            app.RegisterCLRFieldGetter(field, get_onDragEnd_3);
            app.RegisterCLRFieldSetter(field, set_onDragEnd_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_onDragEnd_3, AssignFromStack_onDragEnd_3);


        }


        static StackObject* Get_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GameObject @go = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = Lib.Core.EventTrigger.Get(@go);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Get_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Component @cp = (UnityEngine.Component)typeof(UnityEngine.Component).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = Lib.Core.EventTrigger.Get(@cp);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* AddEventListener_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData> @action = (UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData>)typeof(UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.EventSystems.EventTriggerType @eType = (UnityEngine.EventSystems.EventTriggerType)typeof(UnityEngine.EventSystems.EventTriggerType).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            Lib.Core.EventTrigger instance_of_this_method = (Lib.Core.EventTrigger)typeof(Lib.Core.EventTrigger).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddEventListener(@eType, @action);

            return __ret;
        }

        static StackObject* ClearEvents_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Lib.Core.EventTrigger instance_of_this_method = (Lib.Core.EventTrigger)typeof(Lib.Core.EventTrigger).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ClearEvents();

            return __ret;
        }

        static StackObject* AddEventListener_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData> @action = (UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData>)typeof(UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.EventSystems.EventTriggerType @eType = (UnityEngine.EventSystems.EventTriggerType)typeof(UnityEngine.EventSystems.EventTriggerType).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            UnityEngine.GameObject @go = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            Lib.Core.EventTrigger.AddEventListener(@go, @eType, @action);

            return __ret;
        }

        static StackObject* get_IsDraging_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Lib.Core.EventTrigger instance_of_this_method = (Lib.Core.EventTrigger)typeof(Lib.Core.EventTrigger).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsDraging;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }


        static object get_onClick_0(ref object o)
        {
            return ((Lib.Core.EventTrigger)o).onClick;
        }

        static StackObject* CopyToStack_onClick_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Lib.Core.EventTrigger)o).onClick;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onClick_0(ref object o, object v)
        {
            ((Lib.Core.EventTrigger)o).onClick = (Lib.Core.EventTrigger.VoidDelegate)v;
        }

        static StackObject* AssignFromStack_onClick_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            Lib.Core.EventTrigger.VoidDelegate @onClick = (Lib.Core.EventTrigger.VoidDelegate)typeof(Lib.Core.EventTrigger.VoidDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((Lib.Core.EventTrigger)o).onClick = @onClick;
            return ptr_of_this_method;
        }

        static object get_onDragStart_1(ref object o)
        {
            return ((Lib.Core.EventTrigger)o).onDragStart;
        }

        static StackObject* CopyToStack_onDragStart_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Lib.Core.EventTrigger)o).onDragStart;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDragStart_1(ref object o, object v)
        {
            ((Lib.Core.EventTrigger)o).onDragStart = (Lib.Core.EventTrigger.VoidDelegate)v;
        }

        static StackObject* AssignFromStack_onDragStart_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            Lib.Core.EventTrigger.VoidDelegate @onDragStart = (Lib.Core.EventTrigger.VoidDelegate)typeof(Lib.Core.EventTrigger.VoidDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((Lib.Core.EventTrigger)o).onDragStart = @onDragStart;
            return ptr_of_this_method;
        }

        static object get_onDrag_2(ref object o)
        {
            return ((Lib.Core.EventTrigger)o).onDrag;
        }

        static StackObject* CopyToStack_onDrag_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Lib.Core.EventTrigger)o).onDrag;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDrag_2(ref object o, object v)
        {
            ((Lib.Core.EventTrigger)o).onDrag = (Lib.Core.EventTrigger.VectorDelegate)v;
        }

        static StackObject* AssignFromStack_onDrag_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            Lib.Core.EventTrigger.VectorDelegate @onDrag = (Lib.Core.EventTrigger.VectorDelegate)typeof(Lib.Core.EventTrigger.VectorDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((Lib.Core.EventTrigger)o).onDrag = @onDrag;
            return ptr_of_this_method;
        }

        static object get_onDragEnd_3(ref object o)
        {
            return ((Lib.Core.EventTrigger)o).onDragEnd;
        }

        static StackObject* CopyToStack_onDragEnd_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Lib.Core.EventTrigger)o).onDragEnd;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDragEnd_3(ref object o, object v)
        {
            ((Lib.Core.EventTrigger)o).onDragEnd = (Lib.Core.EventTrigger.VoidDelegate)v;
        }

        static StackObject* AssignFromStack_onDragEnd_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            Lib.Core.EventTrigger.VoidDelegate @onDragEnd = (Lib.Core.EventTrigger.VoidDelegate)typeof(Lib.Core.EventTrigger.VoidDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((Lib.Core.EventTrigger)o).onDragEnd = @onDragEnd;
            return ptr_of_this_method;
        }



    }
}
