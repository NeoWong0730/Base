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
    unsafe class UnityEngine_TextGenerationSettings_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(UnityEngine.TextGenerationSettings);

            field = type.GetField("richText", flag);
            app.RegisterCLRFieldGetter(field, get_richText_0);
            app.RegisterCLRFieldSetter(field, set_richText_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_richText_0, AssignFromStack_richText_0);
            field = type.GetField("scaleFactor", flag);
            app.RegisterCLRFieldGetter(field, get_scaleFactor_1);
            app.RegisterCLRFieldSetter(field, set_scaleFactor_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_scaleFactor_1, AssignFromStack_scaleFactor_1);
            field = type.GetField("horizontalOverflow", flag);
            app.RegisterCLRFieldGetter(field, get_horizontalOverflow_2);
            app.RegisterCLRFieldSetter(field, set_horizontalOverflow_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_horizontalOverflow_2, AssignFromStack_horizontalOverflow_2);

            app.RegisterCLRMemberwiseClone(type, PerformMemberwiseClone);

            app.RegisterCLRCreateDefaultInstance(type, () => new UnityEngine.TextGenerationSettings());


        }

        static void WriteBackInstance(ILRuntime.Runtime.Enviorment.AppDomain __domain, StackObject* ptr_of_this_method, AutoList __mStack, ref UnityEngine.TextGenerationSettings instance_of_this_method)
        {
            ptr_of_this_method = ILIntepreter.GetObjectAndResolveReference(ptr_of_this_method);
            switch(ptr_of_this_method->ObjectType)
            {
                case ObjectTypes.Object:
                    {
                        __mStack[ptr_of_this_method->Value] = instance_of_this_method;
                    }
                    break;
                case ObjectTypes.FieldReference:
                    {
                        var ___obj = __mStack[ptr_of_this_method->Value];
                        if(___obj is ILTypeInstance)
                        {
                            ((ILTypeInstance)___obj)[ptr_of_this_method->ValueLow] = instance_of_this_method;
                        }
                        else
                        {
                            var t = __domain.GetType(___obj.GetType()) as CLRType;
                            t.SetFieldValue(ptr_of_this_method->ValueLow, ref ___obj, instance_of_this_method);
                        }
                    }
                    break;
                case ObjectTypes.StaticFieldReference:
                    {
                        var t = __domain.GetType(ptr_of_this_method->Value);
                        if(t is ILType)
                        {
                            ((ILType)t).StaticInstance[ptr_of_this_method->ValueLow] = instance_of_this_method;
                        }
                        else
                        {
                            ((CLRType)t).SetStaticFieldValue(ptr_of_this_method->ValueLow, instance_of_this_method);
                        }
                    }
                    break;
                 case ObjectTypes.ArrayReference:
                    {
                        var instance_of_arrayReference = __mStack[ptr_of_this_method->Value] as UnityEngine.TextGenerationSettings[];
                        instance_of_arrayReference[ptr_of_this_method->ValueLow] = instance_of_this_method;
                    }
                    break;
            }
        }


        static object get_richText_0(ref object o)
        {
            return ((UnityEngine.TextGenerationSettings)o).richText;
        }

        static StackObject* CopyToStack_richText_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.TextGenerationSettings)o).richText;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_richText_0(ref object o, object v)
        {
            UnityEngine.TextGenerationSettings ins =(UnityEngine.TextGenerationSettings)o;
            ins.richText = (System.Boolean)v;
            o = ins;
        }

        static StackObject* AssignFromStack_richText_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @richText = ptr_of_this_method->Value == 1;
            UnityEngine.TextGenerationSettings ins =(UnityEngine.TextGenerationSettings)o;
            ins.richText = @richText;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_scaleFactor_1(ref object o)
        {
            return ((UnityEngine.TextGenerationSettings)o).scaleFactor;
        }

        static StackObject* CopyToStack_scaleFactor_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.TextGenerationSettings)o).scaleFactor;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_scaleFactor_1(ref object o, object v)
        {
            UnityEngine.TextGenerationSettings ins =(UnityEngine.TextGenerationSettings)o;
            ins.scaleFactor = (System.Single)v;
            o = ins;
        }

        static StackObject* AssignFromStack_scaleFactor_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @scaleFactor = *(float*)&ptr_of_this_method->Value;
            UnityEngine.TextGenerationSettings ins =(UnityEngine.TextGenerationSettings)o;
            ins.scaleFactor = @scaleFactor;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_horizontalOverflow_2(ref object o)
        {
            return ((UnityEngine.TextGenerationSettings)o).horizontalOverflow;
        }

        static StackObject* CopyToStack_horizontalOverflow_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.TextGenerationSettings)o).horizontalOverflow;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_horizontalOverflow_2(ref object o, object v)
        {
            UnityEngine.TextGenerationSettings ins =(UnityEngine.TextGenerationSettings)o;
            ins.horizontalOverflow = (UnityEngine.HorizontalWrapMode)v;
            o = ins;
        }

        static StackObject* AssignFromStack_horizontalOverflow_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.HorizontalWrapMode @horizontalOverflow = (UnityEngine.HorizontalWrapMode)typeof(UnityEngine.HorizontalWrapMode).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            UnityEngine.TextGenerationSettings ins =(UnityEngine.TextGenerationSettings)o;
            ins.horizontalOverflow = @horizontalOverflow;
            o = ins;
            return ptr_of_this_method;
        }


        static object PerformMemberwiseClone(ref object o)
        {
            var ins = new UnityEngine.TextGenerationSettings();
            ins = (UnityEngine.TextGenerationSettings)o;
            return ins;
        }


    }
}
