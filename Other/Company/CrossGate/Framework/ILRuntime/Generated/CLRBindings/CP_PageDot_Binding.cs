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
    unsafe class CP_PageDot_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::CP_PageDot);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("SetMax", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetMax_0);
            args = new Type[]{};
            method = type.GetMethod("Build", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Build_1);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("SetSelected", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetSelected_2);

            field = type.GetField("parent", flag);
            app.RegisterCLRFieldGetter(field, get_parent_0);
            app.RegisterCLRFieldSetter(field, set_parent_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_parent_0, AssignFromStack_parent_0);
            field = type.GetField("proto", flag);
            app.RegisterCLRFieldGetter(field, get_proto_1);
            app.RegisterCLRFieldSetter(field, set_proto_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_proto_1, AssignFromStack_proto_1);


        }


        static StackObject* SetMax_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @max = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::CP_PageDot instance_of_this_method = (global::CP_PageDot)typeof(global::CP_PageDot).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.SetMax(@max);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Build_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::CP_PageDot instance_of_this_method = (global::CP_PageDot)typeof(global::CP_PageDot).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Build();

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* SetSelected_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @index = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::CP_PageDot instance_of_this_method = (global::CP_PageDot)typeof(global::CP_PageDot).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetSelected(@index);

            return __ret;
        }


        static object get_parent_0(ref object o)
        {
            return ((global::CP_PageDot)o).parent;
        }

        static StackObject* CopyToStack_parent_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_PageDot)o).parent;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_parent_0(ref object o, object v)
        {
            ((global::CP_PageDot)o).parent = (UnityEngine.Transform)v;
        }

        static StackObject* AssignFromStack_parent_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Transform @parent = (UnityEngine.Transform)typeof(UnityEngine.Transform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::CP_PageDot)o).parent = @parent;
            return ptr_of_this_method;
        }

        static object get_proto_1(ref object o)
        {
            return ((global::CP_PageDot)o).proto;
        }

        static StackObject* CopyToStack_proto_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::CP_PageDot)o).proto;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_proto_1(ref object o, object v)
        {
            ((global::CP_PageDot)o).proto = (UnityEngine.GameObject)v;
        }

        static StackObject* AssignFromStack_proto_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.GameObject @proto = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::CP_PageDot)o).proto = @proto;
            return ptr_of_this_method;
        }



    }
}
