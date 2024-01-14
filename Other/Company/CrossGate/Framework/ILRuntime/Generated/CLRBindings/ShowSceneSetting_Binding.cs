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
    unsafe class ShowSceneSetting_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::ShowSceneSetting);

            field = type.GetField("vResolution", flag);
            app.RegisterCLRFieldGetter(field, get_vResolution_0);
            app.RegisterCLRFieldSetter(field, set_vResolution_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_vResolution_0, AssignFromStack_vResolution_0);
            field = type.GetField("fScale", flag);
            app.RegisterCLRFieldGetter(field, get_fScale_1);
            app.RegisterCLRFieldSetter(field, set_fScale_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_fScale_1, AssignFromStack_fScale_1);


        }



        static object get_vResolution_0(ref object o)
        {
            return ((global::ShowSceneSetting)o).vResolution;
        }

        static StackObject* CopyToStack_vResolution_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::ShowSceneSetting)o).vResolution;
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2Int_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2Int_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static void set_vResolution_0(ref object o, object v)
        {
            ((global::ShowSceneSetting)o).vResolution = (UnityEngine.Vector2Int)v;
        }

        static StackObject* AssignFromStack_vResolution_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Vector2Int @vResolution = new UnityEngine.Vector2Int();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2Int_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2Int_Binding_Binder.ParseValue(ref @vResolution, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @vResolution = (UnityEngine.Vector2Int)typeof(UnityEngine.Vector2Int).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
            }
            ((global::ShowSceneSetting)o).vResolution = @vResolution;
            return ptr_of_this_method;
        }

        static object get_fScale_1(ref object o)
        {
            return ((global::ShowSceneSetting)o).fScale;
        }

        static StackObject* CopyToStack_fScale_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::ShowSceneSetting)o).fScale;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_fScale_1(ref object o, object v)
        {
            ((global::ShowSceneSetting)o).fScale = (System.Single)v;
        }

        static StackObject* AssignFromStack_fScale_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @fScale = *(float*)&ptr_of_this_method->Value;
            ((global::ShowSceneSetting)o).fScale = @fScale;
            return ptr_of_this_method;
        }



    }
}
