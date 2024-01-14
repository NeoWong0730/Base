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
    unsafe class CP_PopdownList_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::CP_PopdownList);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("Expand", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Expand_0);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("SetSelected", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetSelected_1);
            args = new Type[]{typeof(System.Boolean), typeof(System.Single)};
            method = type.GetMethod("MoveTo", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, MoveTo_2);

            field = type.GetField("optionProto", flag);
            app.RegisterCLRFieldGetter(field, get_optionProto_0);
            app.RegisterCLRFieldSetter(field, set_optionProto_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_optionProto_0, AssignFromStack_optionProto_0);
            field = type.GetField("optionParent", flag);
            app.RegisterCLRFieldGetter(field, get_optionParent_1);
            app.RegisterCLRFieldSetter(field, set_optionParent_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_optionParent_1, AssignFromStack_optionParent_1);


        }


        static StackObject* Expand_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @toShow = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::CP_PopdownList instance_of_this_method = (global::CP_PopdownList)typeof(global::CP_PopdownList).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Expand(@toShow);

            return __ret;
        }

        static StackObject* SetSelected_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @text = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::CP_PopdownList instance_of_this_method = (global::CP_PopdownList)typeof(global::CP_PopdownList).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetSelected(@text);

            return __ret;
        }

        static StackObject* MoveTo_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @normal = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Boolean @horOrVer = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::CP_PopdownList instance_of_this_method = (global::CP_PopdownList)typeof(global::CP_PopdownList).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.MoveTo(@horOrVer, @normal);

            return __ret;
        }


        static object get_optionProto_0(ref object o)
        {
            return ((global::CP_PopdownList)o).optionProto;
        }

        static StackObject* CopyToStack_optionProto_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_PopdownList)o).optionProto;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_optionProto_0(ref object o, object v)
        {
            ((global::CP_PopdownList)o).optionProto = (UnityEngine.GameObject)v;
        }

        static StackObject* AssignFromStack_optionProto_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.GameObject @optionProto = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::CP_PopdownList)o).optionProto = @optionProto;
            return ptr_of_this_method;
        }

        static object get_optionParent_1(ref object o)
        {
            return ((global::CP_PopdownList)o).optionParent;
        }

        static StackObject* CopyToStack_optionParent_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_PopdownList)o).optionParent;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_optionParent_1(ref object o, object v)
        {
            ((global::CP_PopdownList)o).optionParent = (UnityEngine.Transform)v;
        }

        static StackObject* AssignFromStack_optionParent_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Transform @optionParent = (UnityEngine.Transform)typeof(UnityEngine.Transform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::CP_PopdownList)o).optionParent = @optionParent;
            return ptr_of_this_method;
        }



    }
}
