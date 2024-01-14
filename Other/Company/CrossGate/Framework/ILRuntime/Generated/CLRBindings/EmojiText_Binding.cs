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
    unsafe class EmojiText_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::EmojiText);
            args = new Type[]{};
            method = type.GetMethod("get_emojiAsset", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_emojiAsset_0);

            field = type.GetField("onHrefClick", flag);
            app.RegisterCLRFieldGetter(field, get_onHrefClick_0);
            app.RegisterCLRFieldSetter(field, set_onHrefClick_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onHrefClick_0, AssignFromStack_onHrefClick_0);


        }


        static StackObject* get_emojiAsset_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EmojiText instance_of_this_method = (global::EmojiText)typeof(global::EmojiText).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.emojiAsset;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_onHrefClick_0(ref object o)
        {
            return ((global::EmojiText)o).onHrefClick;
        }

        static StackObject* CopyToStack_onHrefClick_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::EmojiText)o).onHrefClick;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onHrefClick_0(ref object o, object v)
        {
            ((global::EmojiText)o).onHrefClick = (UnityEngine.Events.UnityAction<System.String>)v;
        }

        static StackObject* AssignFromStack_onHrefClick_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Events.UnityAction<System.String> @onHrefClick = (UnityEngine.Events.UnityAction<System.String>)typeof(UnityEngine.Events.UnityAction<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::EmojiText)o).onHrefClick = @onHrefClick;
            return ptr_of_this_method;
        }



    }
}
