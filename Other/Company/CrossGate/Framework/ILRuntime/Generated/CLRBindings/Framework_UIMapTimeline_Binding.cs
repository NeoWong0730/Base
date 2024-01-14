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
    unsafe class Framework_UIMapTimeline_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.UIMapTimeline);
            args = new Type[]{typeof(System.UInt32), typeof(System.Collections.Generic.List<System.String>)};
            method = type.GetMethod("SetData", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetData_0);
            args = new Type[]{};
            method = type.GetMethod("PlayTimeline", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, PlayTimeline_1);
            args = new Type[]{};
            method = type.GetMethod("EndAnimation", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, EndAnimation_2);

            field = type.GetField("onPlayableDirectorStopped", flag);
            app.RegisterCLRFieldGetter(field, get_onPlayableDirectorStopped_0);
            app.RegisterCLRFieldSetter(field, set_onPlayableDirectorStopped_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onPlayableDirectorStopped_0, AssignFromStack_onPlayableDirectorStopped_0);


        }


        static StackObject* SetData_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Collections.Generic.List<System.String> @message = (System.Collections.Generic.List<System.String>)typeof(System.Collections.Generic.List<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.UInt32 @id = (uint)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            Framework.UIMapTimeline instance_of_this_method = (Framework.UIMapTimeline)typeof(Framework.UIMapTimeline).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetData(@id, @message);

            return __ret;
        }

        static StackObject* PlayTimeline_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.UIMapTimeline instance_of_this_method = (Framework.UIMapTimeline)typeof(Framework.UIMapTimeline).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.PlayTimeline();

            return __ret;
        }

        static StackObject* EndAnimation_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.UIMapTimeline instance_of_this_method = (Framework.UIMapTimeline)typeof(Framework.UIMapTimeline).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.EndAnimation();

            return __ret;
        }


        static object get_onPlayableDirectorStopped_0(ref object o)
        {
            return ((Framework.UIMapTimeline)o).onPlayableDirectorStopped;
        }

        static StackObject* CopyToStack_onPlayableDirectorStopped_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.UIMapTimeline)o).onPlayableDirectorStopped;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onPlayableDirectorStopped_0(ref object o, object v)
        {
            ((Framework.UIMapTimeline)o).onPlayableDirectorStopped = (System.Action)v;
        }

        static StackObject* AssignFromStack_onPlayableDirectorStopped_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @onPlayableDirectorStopped = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((Framework.UIMapTimeline)o).onPlayableDirectorStopped = @onPlayableDirectorStopped;
            return ptr_of_this_method;
        }



    }
}
