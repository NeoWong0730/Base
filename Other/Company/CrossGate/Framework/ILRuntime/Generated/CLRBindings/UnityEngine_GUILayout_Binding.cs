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
    unsafe class UnityEngine_GUILayout_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(UnityEngine.GUILayout);
            args = new Type[]{typeof(System.Boolean), typeof(System.String), typeof(UnityEngine.GUILayoutOption[])};
            method = type.GetMethod("Toggle", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Toggle_0);
            args = new Type[]{typeof(UnityEngine.GUILayoutOption[])};
            method = type.GetMethod("BeginHorizontal", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, BeginHorizontal_1);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("Width", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Width_2);
            args = new Type[]{typeof(System.String), typeof(UnityEngine.GUILayoutOption[])};
            method = type.GetMethod("Label", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Label_3);
            args = new Type[]{};
            method = type.GetMethod("EndHorizontal", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, EndHorizontal_4);
            args = new Type[]{typeof(UnityEngine.Vector2), typeof(UnityEngine.GUILayoutOption[])};
            method = type.GetMethod("BeginScrollView", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, BeginScrollView_5);
            args = new Type[]{};
            method = type.GetMethod("EndScrollView", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, EndScrollView_6);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("Height", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Height_7);
            args = new Type[]{typeof(System.String), typeof(UnityEngine.GUILayoutOption[])};
            method = type.GetMethod("Button", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Button_8);
            args = new Type[]{typeof(UnityEngine.Rect)};
            method = type.GetMethod("BeginArea", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, BeginArea_9);
            args = new Type[]{};
            method = type.GetMethod("EndArea", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, EndArea_10);
            args = new Type[]{typeof(System.Single), typeof(System.Single), typeof(System.Single), typeof(UnityEngine.GUILayoutOption[])};
            method = type.GetMethod("HorizontalSlider", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, HorizontalSlider_11);
            args = new Type[]{typeof(System.String), typeof(UnityEngine.GUILayoutOption[])};
            method = type.GetMethod("TextField", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, TextField_12);
            args = new Type[]{typeof(UnityEngine.GUIStyle), typeof(UnityEngine.GUILayoutOption[])};
            method = type.GetMethod("BeginHorizontal", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, BeginHorizontal_13);


        }


        static StackObject* Toggle_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GUILayoutOption[] @options = (UnityEngine.GUILayoutOption[])typeof(UnityEngine.GUILayoutOption[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @text = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Boolean @value = ptr_of_this_method->Value == 1;


            var result_of_this_method = UnityEngine.GUILayout.Toggle(@value, @text, @options);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* BeginHorizontal_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GUILayoutOption[] @options = (UnityEngine.GUILayoutOption[])typeof(UnityEngine.GUILayoutOption[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            UnityEngine.GUILayout.BeginHorizontal(@options);

            return __ret;
        }

        static StackObject* Width_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @width = *(float*)&ptr_of_this_method->Value;


            var result_of_this_method = UnityEngine.GUILayout.Width(@width);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Label_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GUILayoutOption[] @options = (UnityEngine.GUILayoutOption[])typeof(UnityEngine.GUILayoutOption[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @text = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            UnityEngine.GUILayout.Label(@text, @options);

            return __ret;
        }

        static StackObject* EndHorizontal_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            UnityEngine.GUILayout.EndHorizontal();

            return __ret;
        }

        static StackObject* BeginScrollView_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GUILayoutOption[] @options = (UnityEngine.GUILayoutOption[])typeof(UnityEngine.GUILayoutOption[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.Vector2 @scrollPosition = new UnityEngine.Vector2();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.ParseValue(ref @scrollPosition, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @scrollPosition = (UnityEngine.Vector2)typeof(UnityEngine.Vector2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
                __intp.Free(ptr_of_this_method);
            }


            var result_of_this_method = UnityEngine.GUILayout.BeginScrollView(@scrollPosition, @options);

            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static StackObject* EndScrollView_6(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            UnityEngine.GUILayout.EndScrollView();

            return __ret;
        }

        static StackObject* Height_7(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @height = *(float*)&ptr_of_this_method->Value;


            var result_of_this_method = UnityEngine.GUILayout.Height(@height);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Button_8(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GUILayoutOption[] @options = (UnityEngine.GUILayoutOption[])typeof(UnityEngine.GUILayoutOption[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @text = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = UnityEngine.GUILayout.Button(@text, @options);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* BeginArea_9(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Rect @screenRect = new UnityEngine.Rect();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Rect_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Rect_Binding_Binder.ParseValue(ref @screenRect, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @screenRect = (UnityEngine.Rect)typeof(UnityEngine.Rect).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
                __intp.Free(ptr_of_this_method);
            }


            UnityEngine.GUILayout.BeginArea(@screenRect);

            return __ret;
        }

        static StackObject* EndArea_10(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            UnityEngine.GUILayout.EndArea();

            return __ret;
        }

        static StackObject* HorizontalSlider_11(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GUILayoutOption[] @options = (UnityEngine.GUILayoutOption[])typeof(UnityEngine.GUILayoutOption[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Single @rightValue = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Single @leftValue = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.Single @value = *(float*)&ptr_of_this_method->Value;


            var result_of_this_method = UnityEngine.GUILayout.HorizontalSlider(@value, @leftValue, @rightValue, @options);

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* TextField_12(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GUILayoutOption[] @options = (UnityEngine.GUILayoutOption[])typeof(UnityEngine.GUILayoutOption[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @text = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = UnityEngine.GUILayout.TextField(@text, @options);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* BeginHorizontal_13(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GUILayoutOption[] @options = (UnityEngine.GUILayoutOption[])typeof(UnityEngine.GUILayoutOption[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.GUIStyle @style = (UnityEngine.GUIStyle)typeof(UnityEngine.GUIStyle).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            UnityEngine.GUILayout.BeginHorizontal(@style, @options);

            return __ret;
        }



    }
}
