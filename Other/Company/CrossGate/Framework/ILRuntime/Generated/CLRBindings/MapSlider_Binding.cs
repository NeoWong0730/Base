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
    unsafe class MapSlider_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::MapSlider);
            args = new Type[]{};
            method = type.GetMethod("get_curIndex", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_curIndex_0);
            args = new Type[]{};
            method = type.GetMethod("get_to", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_to_1);
            args = new Type[]{typeof(System.Int32), typeof(System.Boolean)};
            method = type.GetMethod("OnTabClicked", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnTabClicked_2);

            field = type.GetField("onIndexed", flag);
            app.RegisterCLRFieldGetter(field, get_onIndexed_0);
            app.RegisterCLRFieldSetter(field, set_onIndexed_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onIndexed_0, AssignFromStack_onIndexed_0);
            field = type.GetField("ctrls", flag);
            app.RegisterCLRFieldGetter(field, get_ctrls_1);
            app.RegisterCLRFieldSetter(field, set_ctrls_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_ctrls_1, AssignFromStack_ctrls_1);


        }


        static StackObject* get_curIndex_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::MapSlider instance_of_this_method = (global::MapSlider)typeof(global::MapSlider).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.curIndex;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_to_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::MapSlider instance_of_this_method = (global::MapSlider)typeof(global::MapSlider).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.to;

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* OnTabClicked_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @force = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @index = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::MapSlider instance_of_this_method = (global::MapSlider)typeof(global::MapSlider).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnTabClicked(@index, @force);

            return __ret;
        }


        static object get_onIndexed_0(ref object o)
        {
            return ((global::MapSlider)o).onIndexed;
        }

        static StackObject* CopyToStack_onIndexed_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::MapSlider)o).onIndexed;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onIndexed_0(ref object o, object v)
        {
            ((global::MapSlider)o).onIndexed = (System.Action<System.Int32, System.Int32>)v;
        }

        static StackObject* AssignFromStack_onIndexed_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<System.Int32, System.Int32> @onIndexed = (System.Action<System.Int32, System.Int32>)typeof(System.Action<System.Int32, System.Int32>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::MapSlider)o).onIndexed = @onIndexed;
            return ptr_of_this_method;
        }

        static object get_ctrls_1(ref object o)
        {
            return ((global::MapSlider)o).ctrls;
        }

        static StackObject* CopyToStack_ctrls_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::MapSlider)o).ctrls;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_ctrls_1(ref object o, object v)
        {
            ((global::MapSlider)o).ctrls = (global::MapSlider.Node[])v;
        }

        static StackObject* AssignFromStack_ctrls_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::MapSlider.Node[] @ctrls = (global::MapSlider.Node[])typeof(global::MapSlider.Node[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::MapSlider)o).ctrls = @ctrls;
            return ptr_of_this_method;
        }



    }
}
