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
    unsafe class Framework_AudioEntry_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.AudioEntry);
            args = new Type[]{};
            method = type.GetMethod("Stop", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Stop_0);

            field = type.GetField("DEFAULT_BGMVOLUE_SCALE", flag);
            app.RegisterCLRFieldGetter(field, get_DEFAULT_BGMVOLUE_SCALE_0);
            app.RegisterCLRFieldSetter(field, set_DEFAULT_BGMVOLUE_SCALE_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_DEFAULT_BGMVOLUE_SCALE_0, AssignFromStack_DEFAULT_BGMVOLUE_SCALE_0);


        }


        static StackObject* Stop_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.AudioEntry instance_of_this_method = (Framework.AudioEntry)typeof(Framework.AudioEntry).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Stop();

            return __ret;
        }


        static object get_DEFAULT_BGMVOLUE_SCALE_0(ref object o)
        {
            return Framework.AudioEntry.DEFAULT_BGMVOLUE_SCALE;
        }

        static StackObject* CopyToStack_DEFAULT_BGMVOLUE_SCALE_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = Framework.AudioEntry.DEFAULT_BGMVOLUE_SCALE;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_DEFAULT_BGMVOLUE_SCALE_0(ref object o, object v)
        {
            Framework.AudioEntry.DEFAULT_BGMVOLUE_SCALE = (System.Single)v;
        }

        static StackObject* AssignFromStack_DEFAULT_BGMVOLUE_SCALE_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @DEFAULT_BGMVOLUE_SCALE = *(float*)&ptr_of_this_method->Value;
            Framework.AudioEntry.DEFAULT_BGMVOLUE_SCALE = @DEFAULT_BGMVOLUE_SCALE;
            return ptr_of_this_method;
        }



    }
}
