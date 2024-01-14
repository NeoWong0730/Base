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
    unsafe class Framework_HotFixStateManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.HotFixStateManager);
            args = new Type[]{typeof(System.Action)};
            method = type.GetMethod("CheckPersistentAssetMd5", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CheckPersistentAssetMd5_0);

            field = type.GetField("CheckMD5AssetList", flag);
            app.RegisterCLRFieldGetter(field, get_CheckMD5AssetList_0);
            app.RegisterCLRFieldSetter(field, set_CheckMD5AssetList_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_CheckMD5AssetList_0, AssignFromStack_CheckMD5AssetList_0);


        }


        static StackObject* CheckPersistentAssetMd5_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action @CheckMD5Action = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Framework.HotFixStateManager instance_of_this_method = (Framework.HotFixStateManager)typeof(Framework.HotFixStateManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.CheckPersistentAssetMd5(@CheckMD5Action);

            return __ret;
        }


        static object get_CheckMD5AssetList_0(ref object o)
        {
            return Framework.HotFixStateManager.CheckMD5AssetList;
        }

        static StackObject* CopyToStack_CheckMD5AssetList_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = Framework.HotFixStateManager.CheckMD5AssetList;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_CheckMD5AssetList_0(ref object o, object v)
        {
            Framework.HotFixStateManager.CheckMD5AssetList = (System.Action)v;
        }

        static StackObject* AssignFromStack_CheckMD5AssetList_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @CheckMD5AssetList = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            Framework.HotFixStateManager.CheckMD5AssetList = @CheckMD5AssetList;
            return ptr_of_this_method;
        }



    }
}
