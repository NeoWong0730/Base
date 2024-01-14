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
    unsafe class Framework_Consts_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Consts);

            field = type.GetField("persistentDataPath", flag);
            app.RegisterCLRFieldGetter(field, get_persistentDataPath_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_persistentDataPath_0, null);
            field = type.GetField("START_TIME", flag);
            app.RegisterCLRFieldGetter(field, get_START_TIME_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_START_TIME_1, null);
            field = type.GetField("ID_Schedule", flag);
            app.RegisterCLRFieldGetter(field, get_ID_Schedule_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_ID_Schedule_2, null);


        }



        static object get_persistentDataPath_0(ref object o)
        {
            return Framework.Consts.persistentDataPath;
        }

        static StackObject* CopyToStack_persistentDataPath_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = Framework.Consts.persistentDataPath;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_START_TIME_1(ref object o)
        {
            return Framework.Consts.START_TIME;
        }

        static StackObject* CopyToStack_START_TIME_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = Framework.Consts.START_TIME;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_ID_Schedule_2(ref object o)
        {
            return Framework.Consts.ID_Schedule;
        }

        static StackObject* CopyToStack_ID_Schedule_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = Framework.Consts.ID_Schedule;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }



    }
}
