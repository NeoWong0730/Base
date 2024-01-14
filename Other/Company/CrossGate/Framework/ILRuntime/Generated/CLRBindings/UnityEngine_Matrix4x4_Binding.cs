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
    unsafe class UnityEngine_Matrix4x4_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(UnityEngine.Matrix4x4);
            args = new Type[]{typeof(UnityEngine.Vector3)};
            method = type.GetMethod("MultiplyVector", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, MultiplyVector_0);
            args = new Type[]{typeof(UnityEngine.Vector3)};
            method = type.GetMethod("MultiplyPoint3x4", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, MultiplyPoint3x4_1);
            args = new Type[]{typeof(UnityEngine.Vector3)};
            method = type.GetMethod("MultiplyPoint", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, MultiplyPoint_2);

            field = type.GetField("m00", flag);
            app.RegisterCLRFieldGetter(field, get_m00_0);
            app.RegisterCLRFieldSetter(field, set_m00_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_m00_0, AssignFromStack_m00_0);
            field = type.GetField("m01", flag);
            app.RegisterCLRFieldGetter(field, get_m01_1);
            app.RegisterCLRFieldSetter(field, set_m01_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_m01_1, AssignFromStack_m01_1);
            field = type.GetField("m02", flag);
            app.RegisterCLRFieldGetter(field, get_m02_2);
            app.RegisterCLRFieldSetter(field, set_m02_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_m02_2, AssignFromStack_m02_2);
            field = type.GetField("m03", flag);
            app.RegisterCLRFieldGetter(field, get_m03_3);
            app.RegisterCLRFieldSetter(field, set_m03_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_m03_3, AssignFromStack_m03_3);
            field = type.GetField("m10", flag);
            app.RegisterCLRFieldGetter(field, get_m10_4);
            app.RegisterCLRFieldSetter(field, set_m10_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_m10_4, AssignFromStack_m10_4);
            field = type.GetField("m11", flag);
            app.RegisterCLRFieldGetter(field, get_m11_5);
            app.RegisterCLRFieldSetter(field, set_m11_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_m11_5, AssignFromStack_m11_5);
            field = type.GetField("m12", flag);
            app.RegisterCLRFieldGetter(field, get_m12_6);
            app.RegisterCLRFieldSetter(field, set_m12_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_m12_6, AssignFromStack_m12_6);
            field = type.GetField("m13", flag);
            app.RegisterCLRFieldGetter(field, get_m13_7);
            app.RegisterCLRFieldSetter(field, set_m13_7);
            app.RegisterCLRFieldBinding(field, CopyToStack_m13_7, AssignFromStack_m13_7);
            field = type.GetField("m20", flag);
            app.RegisterCLRFieldGetter(field, get_m20_8);
            app.RegisterCLRFieldSetter(field, set_m20_8);
            app.RegisterCLRFieldBinding(field, CopyToStack_m20_8, AssignFromStack_m20_8);
            field = type.GetField("m21", flag);
            app.RegisterCLRFieldGetter(field, get_m21_9);
            app.RegisterCLRFieldSetter(field, set_m21_9);
            app.RegisterCLRFieldBinding(field, CopyToStack_m21_9, AssignFromStack_m21_9);
            field = type.GetField("m22", flag);
            app.RegisterCLRFieldGetter(field, get_m22_10);
            app.RegisterCLRFieldSetter(field, set_m22_10);
            app.RegisterCLRFieldBinding(field, CopyToStack_m22_10, AssignFromStack_m22_10);
            field = type.GetField("m23", flag);
            app.RegisterCLRFieldGetter(field, get_m23_11);
            app.RegisterCLRFieldSetter(field, set_m23_11);
            app.RegisterCLRFieldBinding(field, CopyToStack_m23_11, AssignFromStack_m23_11);
            field = type.GetField("m30", flag);
            app.RegisterCLRFieldGetter(field, get_m30_12);
            app.RegisterCLRFieldSetter(field, set_m30_12);
            app.RegisterCLRFieldBinding(field, CopyToStack_m30_12, AssignFromStack_m30_12);
            field = type.GetField("m31", flag);
            app.RegisterCLRFieldGetter(field, get_m31_13);
            app.RegisterCLRFieldSetter(field, set_m31_13);
            app.RegisterCLRFieldBinding(field, CopyToStack_m31_13, AssignFromStack_m31_13);
            field = type.GetField("m32", flag);
            app.RegisterCLRFieldGetter(field, get_m32_14);
            app.RegisterCLRFieldSetter(field, set_m32_14);
            app.RegisterCLRFieldBinding(field, CopyToStack_m32_14, AssignFromStack_m32_14);
            field = type.GetField("m33", flag);
            app.RegisterCLRFieldGetter(field, get_m33_15);
            app.RegisterCLRFieldSetter(field, set_m33_15);
            app.RegisterCLRFieldBinding(field, CopyToStack_m33_15, AssignFromStack_m33_15);

            app.RegisterCLRCreateDefaultInstance(type, () => new UnityEngine.Matrix4x4());

            args = new Type[]{typeof(UnityEngine.Vector4), typeof(UnityEngine.Vector4), typeof(UnityEngine.Vector4), typeof(UnityEngine.Vector4)};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }

        static void WriteBackInstance(ILRuntime.Runtime.Enviorment.AppDomain __domain, StackObject* ptr_of_this_method, AutoList __mStack, ref UnityEngine.Matrix4x4 instance_of_this_method)
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
                        var instance_of_arrayReference = __mStack[ptr_of_this_method->Value] as UnityEngine.Matrix4x4[];
                        instance_of_arrayReference[ptr_of_this_method->ValueLow] = instance_of_this_method;
                    }
                    break;
            }
        }

        static StackObject* MultiplyVector_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Vector3 @vector = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @vector, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @vector = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.Matrix4x4 instance_of_this_method = new UnityEngine.Matrix4x4();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Matrix4x4_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Matrix4x4_Binding_Binder.ParseValue(ref instance_of_this_method, __intp, ptr_of_this_method, __mStack, false);
            } else {
                ptr_of_this_method = ILIntepreter.GetObjectAndResolveReference(ptr_of_this_method);
                instance_of_this_method = (UnityEngine.Matrix4x4)typeof(UnityEngine.Matrix4x4).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
            }

            var result_of_this_method = instance_of_this_method.MultiplyVector(@vector);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Matrix4x4_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Matrix4x4_Binding_Binder.WriteBackValue(__domain, ptr_of_this_method, __mStack, ref instance_of_this_method);
            } else {
                WriteBackInstance(__domain, ptr_of_this_method, __mStack, ref instance_of_this_method);
            }

            __intp.Free(ptr_of_this_method);
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static StackObject* MultiplyPoint3x4_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Vector3 @point = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @point, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @point = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.Matrix4x4 instance_of_this_method = new UnityEngine.Matrix4x4();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Matrix4x4_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Matrix4x4_Binding_Binder.ParseValue(ref instance_of_this_method, __intp, ptr_of_this_method, __mStack, false);
            } else {
                ptr_of_this_method = ILIntepreter.GetObjectAndResolveReference(ptr_of_this_method);
                instance_of_this_method = (UnityEngine.Matrix4x4)typeof(UnityEngine.Matrix4x4).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
            }

            var result_of_this_method = instance_of_this_method.MultiplyPoint3x4(@point);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Matrix4x4_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Matrix4x4_Binding_Binder.WriteBackValue(__domain, ptr_of_this_method, __mStack, ref instance_of_this_method);
            } else {
                WriteBackInstance(__domain, ptr_of_this_method, __mStack, ref instance_of_this_method);
            }

            __intp.Free(ptr_of_this_method);
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static StackObject* MultiplyPoint_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Vector3 @point = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @point, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @point = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.Matrix4x4 instance_of_this_method = new UnityEngine.Matrix4x4();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Matrix4x4_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Matrix4x4_Binding_Binder.ParseValue(ref instance_of_this_method, __intp, ptr_of_this_method, __mStack, false);
            } else {
                ptr_of_this_method = ILIntepreter.GetObjectAndResolveReference(ptr_of_this_method);
                instance_of_this_method = (UnityEngine.Matrix4x4)typeof(UnityEngine.Matrix4x4).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
            }

            var result_of_this_method = instance_of_this_method.MultiplyPoint(@point);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Matrix4x4_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Matrix4x4_Binding_Binder.WriteBackValue(__domain, ptr_of_this_method, __mStack, ref instance_of_this_method);
            } else {
                WriteBackInstance(__domain, ptr_of_this_method, __mStack, ref instance_of_this_method);
            }

            __intp.Free(ptr_of_this_method);
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }


        static object get_m00_0(ref object o)
        {
            return ((UnityEngine.Matrix4x4)o).m00;
        }

        static StackObject* CopyToStack_m00_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.Matrix4x4)o).m00;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_m00_0(ref object o, object v)
        {
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m00 = (System.Single)v;
            o = ins;
        }

        static StackObject* AssignFromStack_m00_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @m00 = *(float*)&ptr_of_this_method->Value;
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m00 = @m00;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_m01_1(ref object o)
        {
            return ((UnityEngine.Matrix4x4)o).m01;
        }

        static StackObject* CopyToStack_m01_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.Matrix4x4)o).m01;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_m01_1(ref object o, object v)
        {
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m01 = (System.Single)v;
            o = ins;
        }

        static StackObject* AssignFromStack_m01_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @m01 = *(float*)&ptr_of_this_method->Value;
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m01 = @m01;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_m02_2(ref object o)
        {
            return ((UnityEngine.Matrix4x4)o).m02;
        }

        static StackObject* CopyToStack_m02_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.Matrix4x4)o).m02;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_m02_2(ref object o, object v)
        {
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m02 = (System.Single)v;
            o = ins;
        }

        static StackObject* AssignFromStack_m02_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @m02 = *(float*)&ptr_of_this_method->Value;
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m02 = @m02;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_m03_3(ref object o)
        {
            return ((UnityEngine.Matrix4x4)o).m03;
        }

        static StackObject* CopyToStack_m03_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.Matrix4x4)o).m03;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_m03_3(ref object o, object v)
        {
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m03 = (System.Single)v;
            o = ins;
        }

        static StackObject* AssignFromStack_m03_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @m03 = *(float*)&ptr_of_this_method->Value;
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m03 = @m03;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_m10_4(ref object o)
        {
            return ((UnityEngine.Matrix4x4)o).m10;
        }

        static StackObject* CopyToStack_m10_4(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.Matrix4x4)o).m10;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_m10_4(ref object o, object v)
        {
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m10 = (System.Single)v;
            o = ins;
        }

        static StackObject* AssignFromStack_m10_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @m10 = *(float*)&ptr_of_this_method->Value;
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m10 = @m10;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_m11_5(ref object o)
        {
            return ((UnityEngine.Matrix4x4)o).m11;
        }

        static StackObject* CopyToStack_m11_5(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.Matrix4x4)o).m11;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_m11_5(ref object o, object v)
        {
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m11 = (System.Single)v;
            o = ins;
        }

        static StackObject* AssignFromStack_m11_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @m11 = *(float*)&ptr_of_this_method->Value;
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m11 = @m11;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_m12_6(ref object o)
        {
            return ((UnityEngine.Matrix4x4)o).m12;
        }

        static StackObject* CopyToStack_m12_6(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.Matrix4x4)o).m12;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_m12_6(ref object o, object v)
        {
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m12 = (System.Single)v;
            o = ins;
        }

        static StackObject* AssignFromStack_m12_6(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @m12 = *(float*)&ptr_of_this_method->Value;
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m12 = @m12;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_m13_7(ref object o)
        {
            return ((UnityEngine.Matrix4x4)o).m13;
        }

        static StackObject* CopyToStack_m13_7(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.Matrix4x4)o).m13;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_m13_7(ref object o, object v)
        {
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m13 = (System.Single)v;
            o = ins;
        }

        static StackObject* AssignFromStack_m13_7(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @m13 = *(float*)&ptr_of_this_method->Value;
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m13 = @m13;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_m20_8(ref object o)
        {
            return ((UnityEngine.Matrix4x4)o).m20;
        }

        static StackObject* CopyToStack_m20_8(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.Matrix4x4)o).m20;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_m20_8(ref object o, object v)
        {
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m20 = (System.Single)v;
            o = ins;
        }

        static StackObject* AssignFromStack_m20_8(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @m20 = *(float*)&ptr_of_this_method->Value;
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m20 = @m20;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_m21_9(ref object o)
        {
            return ((UnityEngine.Matrix4x4)o).m21;
        }

        static StackObject* CopyToStack_m21_9(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.Matrix4x4)o).m21;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_m21_9(ref object o, object v)
        {
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m21 = (System.Single)v;
            o = ins;
        }

        static StackObject* AssignFromStack_m21_9(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @m21 = *(float*)&ptr_of_this_method->Value;
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m21 = @m21;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_m22_10(ref object o)
        {
            return ((UnityEngine.Matrix4x4)o).m22;
        }

        static StackObject* CopyToStack_m22_10(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.Matrix4x4)o).m22;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_m22_10(ref object o, object v)
        {
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m22 = (System.Single)v;
            o = ins;
        }

        static StackObject* AssignFromStack_m22_10(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @m22 = *(float*)&ptr_of_this_method->Value;
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m22 = @m22;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_m23_11(ref object o)
        {
            return ((UnityEngine.Matrix4x4)o).m23;
        }

        static StackObject* CopyToStack_m23_11(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.Matrix4x4)o).m23;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_m23_11(ref object o, object v)
        {
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m23 = (System.Single)v;
            o = ins;
        }

        static StackObject* AssignFromStack_m23_11(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @m23 = *(float*)&ptr_of_this_method->Value;
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m23 = @m23;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_m30_12(ref object o)
        {
            return ((UnityEngine.Matrix4x4)o).m30;
        }

        static StackObject* CopyToStack_m30_12(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.Matrix4x4)o).m30;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_m30_12(ref object o, object v)
        {
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m30 = (System.Single)v;
            o = ins;
        }

        static StackObject* AssignFromStack_m30_12(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @m30 = *(float*)&ptr_of_this_method->Value;
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m30 = @m30;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_m31_13(ref object o)
        {
            return ((UnityEngine.Matrix4x4)o).m31;
        }

        static StackObject* CopyToStack_m31_13(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.Matrix4x4)o).m31;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_m31_13(ref object o, object v)
        {
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m31 = (System.Single)v;
            o = ins;
        }

        static StackObject* AssignFromStack_m31_13(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @m31 = *(float*)&ptr_of_this_method->Value;
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m31 = @m31;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_m32_14(ref object o)
        {
            return ((UnityEngine.Matrix4x4)o).m32;
        }

        static StackObject* CopyToStack_m32_14(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.Matrix4x4)o).m32;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_m32_14(ref object o, object v)
        {
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m32 = (System.Single)v;
            o = ins;
        }

        static StackObject* AssignFromStack_m32_14(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @m32 = *(float*)&ptr_of_this_method->Value;
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m32 = @m32;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_m33_15(ref object o)
        {
            return ((UnityEngine.Matrix4x4)o).m33;
        }

        static StackObject* CopyToStack_m33_15(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((UnityEngine.Matrix4x4)o).m33;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_m33_15(ref object o, object v)
        {
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m33 = (System.Single)v;
            o = ins;
        }

        static StackObject* AssignFromStack_m33_15(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @m33 = *(float*)&ptr_of_this_method->Value;
            UnityEngine.Matrix4x4 ins =(UnityEngine.Matrix4x4)o;
            ins.m33 = @m33;
            o = ins;
            return ptr_of_this_method;
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Vector4 @column3 = new UnityEngine.Vector4();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector4_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector4_Binding_Binder.ParseValue(ref @column3, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @column3 = (UnityEngine.Vector4)typeof(UnityEngine.Vector4).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.Vector4 @column2 = new UnityEngine.Vector4();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector4_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector4_Binding_Binder.ParseValue(ref @column2, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @column2 = (UnityEngine.Vector4)typeof(UnityEngine.Vector4).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            UnityEngine.Vector4 @column1 = new UnityEngine.Vector4();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector4_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector4_Binding_Binder.ParseValue(ref @column1, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @column1 = (UnityEngine.Vector4)typeof(UnityEngine.Vector4).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            UnityEngine.Vector4 @column0 = new UnityEngine.Vector4();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector4_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector4_Binding_Binder.ParseValue(ref @column0, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @column0 = (UnityEngine.Vector4)typeof(UnityEngine.Vector4).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
                __intp.Free(ptr_of_this_method);
            }


            var result_of_this_method = new UnityEngine.Matrix4x4(@column0, @column1, @column2, @column3);

            if(!isNewObj)
            {
                __ret--;
                if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Matrix4x4_Binding_Binder != null) {
                    ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Matrix4x4_Binding_Binder.WriteBackValue(__domain, __ret, __mStack, ref result_of_this_method);
                } else {
                    WriteBackInstance(__domain, __ret, __mStack, ref result_of_this_method);
                }
                return __ret;
            }

            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Matrix4x4_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Matrix4x4_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
                return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }


    }
}
