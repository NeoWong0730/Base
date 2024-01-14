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
    unsafe class Lib_AssetLoader_VersionHelper_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(Lib.AssetLoader.VersionHelper);
            args = new Type[]{};
            method = type.GetMethod("get_ZoneId", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ZoneId_0);
            args = new Type[]{};
            method = type.GetMethod("get_DirNoticeUrl", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_DirNoticeUrl_1);
            args = new Type[]{};
            method = type.GetMethod("get_LoginUrl", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_LoginUrl_2);
            args = new Type[]{};
            method = type.GetMethod("get_DirsvrUrl", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_DirsvrUrl_3);
            args = new Type[]{};
            method = type.GetMethod("get_eChannelFlags", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_eChannelFlags_4);
            args = new Type[]{};
            method = type.GetMethod("get_ChannelName", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ChannelName_5);
            args = new Type[]{};
            method = type.GetMethod("get_eHotFixMode", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_eHotFixMode_6);
            args = new Type[]{};
            method = type.GetMethod("get_StreamingBuildVersion", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_StreamingBuildVersion_7);
            args = new Type[]{};
            method = type.GetMethod("get_PersistentBuildVersion", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_PersistentBuildVersion_8);
            args = new Type[]{};
            method = type.GetMethod("get_StreamingAssetVersion", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_StreamingAssetVersion_9);
            args = new Type[]{};
            method = type.GetMethod("get_PersistentAssetVersion", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_PersistentAssetVersion_10);


        }


        static StackObject* get_ZoneId_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = Lib.AssetLoader.VersionHelper.ZoneId;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_DirNoticeUrl_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = Lib.AssetLoader.VersionHelper.DirNoticeUrl;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_LoginUrl_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = Lib.AssetLoader.VersionHelper.LoginUrl;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_DirsvrUrl_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = Lib.AssetLoader.VersionHelper.DirsvrUrl;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_eChannelFlags_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = Lib.AssetLoader.VersionHelper.eChannelFlags;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_ChannelName_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = Lib.AssetLoader.VersionHelper.ChannelName;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_eHotFixMode_6(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = Lib.AssetLoader.VersionHelper.eHotFixMode;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_StreamingBuildVersion_7(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = Lib.AssetLoader.VersionHelper.StreamingBuildVersion;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_PersistentBuildVersion_8(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = Lib.AssetLoader.VersionHelper.PersistentBuildVersion;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_StreamingAssetVersion_9(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = Lib.AssetLoader.VersionHelper.StreamingAssetVersion;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_PersistentAssetVersion_10(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = Lib.AssetLoader.VersionHelper.PersistentAssetVersion;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
