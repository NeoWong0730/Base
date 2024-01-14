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
    unsafe class TansformWorldToScreen_Binding_WorldToScreenData_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::TansformWorldToScreen.WorldToScreenData);
            args = new Type[]{typeof(UnityEngine.Vector3)};
            method = type.GetMethod("set_positionOffset", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_positionOffset_0);
            args = new Type[]{};
            method = type.GetMethod("get_positionOffset", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_positionOffset_1);

            field = type.GetField("to", flag);
            app.RegisterCLRFieldGetter(field, get_to_0);
            app.RegisterCLRFieldSetter(field, set_to_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_to_0, AssignFromStack_to_0);
            field = type.GetField("from", flag);
            app.RegisterCLRFieldGetter(field, get_from_1);
            app.RegisterCLRFieldSetter(field, set_from_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_from_1, AssignFromStack_from_1);


        }


        static StackObject* set_positionOffset_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Vector3 @value = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @value, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @value = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::TansformWorldToScreen.WorldToScreenData instance_of_this_method = (global::TansformWorldToScreen.WorldToScreenData)typeof(global::TansformWorldToScreen.WorldToScreenData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.positionOffset = value;

            return __ret;
        }

        static StackObject* get_positionOffset_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::TansformWorldToScreen.WorldToScreenData instance_of_this_method = (global::TansformWorldToScreen.WorldToScreenData)typeof(global::TansformWorldToScreen.WorldToScreenData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.positionOffset;

            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }


        static object get_to_0(ref object o)
        {
            return ((global::TansformWorldToScreen.WorldToScreenData)o).to;
        }

        static StackObject* CopyToStack_to_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::TansformWorldToScreen.WorldToScreenData)o).to;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_to_0(ref object o, object v)
        {
            ((global::TansformWorldToScreen.WorldToScreenData)o).to = (UnityEngine.RectTransform)v;
        }

        static StackObject* AssignFromStack_to_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.RectTransform @to = (UnityEngine.RectTransform)typeof(UnityEngine.RectTransform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::TansformWorldToScreen.WorldToScreenData)o).to = @to;
            return ptr_of_this_method;
        }

        static object get_from_1(ref object o)
        {
            return ((global::TansformWorldToScreen.WorldToScreenData)o).from;
        }

        static StackObject* CopyToStack_from_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::TansformWorldToScreen.WorldToScreenData)o).from;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_from_1(ref object o, object v)
        {
            ((global::TansformWorldToScreen.WorldToScreenData)o).from = (UnityEngine.Transform)v;
        }

        static StackObject* AssignFromStack_from_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Transform @from = (UnityEngine.Transform)typeof(UnityEngine.Transform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::TansformWorldToScreen.WorldToScreenData)o).from = @from;
            return ptr_of_this_method;
        }



    }
}
