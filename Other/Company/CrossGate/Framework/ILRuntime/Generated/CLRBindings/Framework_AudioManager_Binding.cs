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
    unsafe class Framework_AudioManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.AudioManager);
            args = new Type[]{typeof(UnityEngine.Vector2), typeof(System.Boolean)};
            method = type.GetMethod("OnGUI", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnGUI_0);
            args = new Type[]{typeof(System.String), typeof(System.UInt32), typeof(System.Boolean), typeof(System.Boolean), typeof(System.Single)};
            method = type.GetMethod("Play", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Play_1);
            args = new Type[]{};
            method = type.GetMethod("StopAll", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, StopAll_2);
            args = new Type[]{typeof(System.UInt32)};
            method = type.GetMethod("StopSingle", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, StopSingle_3);
            args = new Type[]{typeof(System.UInt32), typeof(System.Boolean)};
            method = type.GetMethod("PauseSingle", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, PauseSingle_4);
            args = new Type[]{typeof(System.UInt32), typeof(System.Single)};
            method = type.GetMethod("SetVolume", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetVolume_5);
            args = new Type[]{};
            method = type.GetMethod("Init", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Init_6);
            args = new Type[]{};
            method = type.GetMethod("UnInit", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, UnInit_7);

            field = type.GetField("use3dSound", flag);
            app.RegisterCLRFieldGetter(field, get_use3dSound_0);
            app.RegisterCLRFieldSetter(field, set_use3dSound_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_use3dSound_0, AssignFromStack_use3dSound_0);
            field = type.GetField("usePHASEInIOS", flag);
            app.RegisterCLRFieldGetter(field, get_usePHASEInIOS_1);
            app.RegisterCLRFieldSetter(field, set_usePHASEInIOS_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_usePHASEInIOS_1, AssignFromStack_usePHASEInIOS_1);


        }


        static StackObject* OnGUI_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @hideNormal = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.Vector2 @size = new UnityEngine.Vector2();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.ParseValue(ref @size, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @size = (UnityEngine.Vector2)typeof(UnityEngine.Vector2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            Framework.AudioManager instance_of_this_method = (Framework.AudioManager)typeof(Framework.AudioManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnGUI(@size, @hideNormal);

            return __ret;
        }

        static StackObject* Play_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 6);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @volumeScale = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Boolean @loop = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Boolean @isMulti = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.UInt32 @audioType = (uint)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            System.String @audioPath = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 6);
            Framework.AudioManager instance_of_this_method = (Framework.AudioManager)typeof(Framework.AudioManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Play(@audioPath, @audioType, @isMulti, @loop, @volumeScale);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* StopAll_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.AudioManager instance_of_this_method = (Framework.AudioManager)typeof(Framework.AudioManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.StopAll();

            return __ret;
        }

        static StackObject* StopSingle_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt32 @audioType = (uint)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Framework.AudioManager instance_of_this_method = (Framework.AudioManager)typeof(Framework.AudioManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.StopSingle(@audioType);

            return __ret;
        }

        static StackObject* PauseSingle_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @toPause = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.UInt32 @audioType = (uint)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            Framework.AudioManager instance_of_this_method = (Framework.AudioManager)typeof(Framework.AudioManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.PauseSingle(@audioType, @toPause);

            return __ret;
        }

        static StackObject* SetVolume_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @volume = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.UInt32 @audioType = (uint)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            Framework.AudioManager instance_of_this_method = (Framework.AudioManager)typeof(Framework.AudioManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetVolume(@audioType, @volume);

            return __ret;
        }

        static StackObject* Init_6(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.AudioManager instance_of_this_method = (Framework.AudioManager)typeof(Framework.AudioManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Init();

            return __ret;
        }

        static StackObject* UnInit_7(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.AudioManager instance_of_this_method = (Framework.AudioManager)typeof(Framework.AudioManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.UnInit();

            return __ret;
        }


        static object get_use3dSound_0(ref object o)
        {
            return ((Framework.AudioManager)o).use3dSound;
        }

        static StackObject* CopyToStack_use3dSound_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.AudioManager)o).use3dSound;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_use3dSound_0(ref object o, object v)
        {
            ((Framework.AudioManager)o).use3dSound = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_use3dSound_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @use3dSound = ptr_of_this_method->Value == 1;
            ((Framework.AudioManager)o).use3dSound = @use3dSound;
            return ptr_of_this_method;
        }

        static object get_usePHASEInIOS_1(ref object o)
        {
            return ((Framework.AudioManager)o).usePHASEInIOS;
        }

        static StackObject* CopyToStack_usePHASEInIOS_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.AudioManager)o).usePHASEInIOS;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_usePHASEInIOS_1(ref object o, object v)
        {
            ((Framework.AudioManager)o).usePHASEInIOS = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_usePHASEInIOS_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @usePHASEInIOS = ptr_of_this_method->Value == 1;
            ((Framework.AudioManager)o).usePHASEInIOS = @usePHASEInIOS;
            return ptr_of_this_method;
        }



    }
}
