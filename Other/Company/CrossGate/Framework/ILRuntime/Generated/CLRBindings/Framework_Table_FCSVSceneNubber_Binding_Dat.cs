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
    unsafe class Framework_Table_FCSVSceneNubber_Binding_Data_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Framework.Table.FCSVSceneNubber.Data);
            args = new Type[]{};
            method = type.GetMethod("get_minimumMove", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_minimumMove_0);
            args = new Type[]{};
            method = type.GetMethod("get_skipTime", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_skipTime_1);
            args = new Type[]{};
            method = type.GetMethod("get_waitTime", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_waitTime_2);
            args = new Type[]{};
            method = type.GetMethod("get_openTime", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_openTime_3);
            args = new Type[]{};
            method = type.GetMethod("get_timeDelay", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_timeDelay_4);
            args = new Type[]{};
            method = type.GetMethod("get_material", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_material_5);
            args = new Type[]{};
            method = type.GetMethod("get_returnTime", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_returnTime_6);

            field = type.GetField("zozeId", flag);
            app.RegisterCLRFieldGetter(field, get_zozeId_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_zozeId_0, null);
            field = type.GetField("camera", flag);
            app.RegisterCLRFieldGetter(field, get_camera_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_camera_1, null);
            field = type.GetField("dialogueParameter", flag);
            app.RegisterCLRFieldGetter(field, get_dialogueParameter_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_dialogueParameter_2, null);
            field = type.GetField("consultPosition", flag);
            app.RegisterCLRFieldGetter(field, get_consultPosition_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_consultPosition_3, null);
            field = type.GetField("moveId", flag);
            app.RegisterCLRFieldGetter(field, get_moveId_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_moveId_4, null);


        }


        static StackObject* get_minimumMove_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVSceneNubber.Data instance_of_this_method = (Framework.Table.FCSVSceneNubber.Data)typeof(Framework.Table.FCSVSceneNubber.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.minimumMove;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_skipTime_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVSceneNubber.Data instance_of_this_method = (Framework.Table.FCSVSceneNubber.Data)typeof(Framework.Table.FCSVSceneNubber.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.skipTime;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_waitTime_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVSceneNubber.Data instance_of_this_method = (Framework.Table.FCSVSceneNubber.Data)typeof(Framework.Table.FCSVSceneNubber.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.waitTime;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_openTime_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVSceneNubber.Data instance_of_this_method = (Framework.Table.FCSVSceneNubber.Data)typeof(Framework.Table.FCSVSceneNubber.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.openTime;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_timeDelay_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVSceneNubber.Data instance_of_this_method = (Framework.Table.FCSVSceneNubber.Data)typeof(Framework.Table.FCSVSceneNubber.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.timeDelay;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_material_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVSceneNubber.Data instance_of_this_method = (Framework.Table.FCSVSceneNubber.Data)typeof(Framework.Table.FCSVSceneNubber.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.material;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_returnTime_6(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Framework.Table.FCSVSceneNubber.Data instance_of_this_method = (Framework.Table.FCSVSceneNubber.Data)typeof(Framework.Table.FCSVSceneNubber.Data).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.returnTime;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }


        static object get_zozeId_0(ref object o)
        {
            return ((Framework.Table.FCSVSceneNubber.Data)o).zozeId;
        }

        static StackObject* CopyToStack_zozeId_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVSceneNubber.Data)o).zozeId;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_camera_1(ref object o)
        {
            return ((Framework.Table.FCSVSceneNubber.Data)o).camera;
        }

        static StackObject* CopyToStack_camera_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVSceneNubber.Data)o).camera;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_dialogueParameter_2(ref object o)
        {
            return ((Framework.Table.FCSVSceneNubber.Data)o).dialogueParameter;
        }

        static StackObject* CopyToStack_dialogueParameter_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVSceneNubber.Data)o).dialogueParameter;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_consultPosition_3(ref object o)
        {
            return ((Framework.Table.FCSVSceneNubber.Data)o).consultPosition;
        }

        static StackObject* CopyToStack_consultPosition_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVSceneNubber.Data)o).consultPosition;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static object get_moveId_4(ref object o)
        {
            return ((Framework.Table.FCSVSceneNubber.Data)o).moveId;
        }

        static StackObject* CopyToStack_moveId_4(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Framework.Table.FCSVSceneNubber.Data)o).moveId;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
