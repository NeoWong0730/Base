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
    unsafe class WeatherSystem_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::WeatherSystem);
            args = new Type[]{typeof(global::EWeatherType)};
            method = type.GetMethod("set_gWeatherTypeBefore", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_gWeatherTypeBefore_0);
            args = new Type[]{typeof(global::EWeatherType)};
            method = type.GetMethod("set_gWeatherType", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_gWeatherType_1);
            args = new Type[]{typeof(global::EDayStage)};
            method = type.GetMethod("set_gDayStage", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_gDayStage_2);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("set_gWeatherProgress", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_gWeatherProgress_3);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("set_gDayProgress", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_gDayProgress_4);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("set_gSeasonProgress", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_gSeasonProgress_5);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("set_gCurSeasonProgress", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_gCurSeasonProgress_6);
            args = new Type[]{};
            method = type.GetMethod("get_gUseWeather", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_gUseWeather_7);
            args = new Type[]{};
            method = type.GetMethod("get_gWeatherType", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_gWeatherType_8);

            field = type.GetField("OnFlashing", flag);
            app.RegisterCLRFieldGetter(field, get_OnFlashing_0);
            app.RegisterCLRFieldSetter(field, set_OnFlashing_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_OnFlashing_0, AssignFromStack_OnFlashing_0);


        }


        static StackObject* set_gWeatherTypeBefore_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EWeatherType @value = (global::EWeatherType)typeof(global::EWeatherType).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            __intp.Free(ptr_of_this_method);


            global::WeatherSystem.gWeatherTypeBefore = value;

            return __ret;
        }

        static StackObject* set_gWeatherType_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EWeatherType @value = (global::EWeatherType)typeof(global::EWeatherType).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            __intp.Free(ptr_of_this_method);


            global::WeatherSystem.gWeatherType = value;

            return __ret;
        }

        static StackObject* set_gDayStage_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EDayStage @value = (global::EDayStage)typeof(global::EDayStage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            __intp.Free(ptr_of_this_method);


            global::WeatherSystem.gDayStage = value;

            return __ret;
        }

        static StackObject* set_gWeatherProgress_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;


            global::WeatherSystem.gWeatherProgress = value;

            return __ret;
        }

        static StackObject* set_gDayProgress_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;


            global::WeatherSystem.gDayProgress = value;

            return __ret;
        }

        static StackObject* set_gSeasonProgress_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;


            global::WeatherSystem.gSeasonProgress = value;

            return __ret;
        }

        static StackObject* set_gCurSeasonProgress_6(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;


            global::WeatherSystem.gCurSeasonProgress = value;

            return __ret;
        }

        static StackObject* get_gUseWeather_7(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::WeatherSystem.gUseWeather;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_gWeatherType_8(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::WeatherSystem.gWeatherType;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_OnFlashing_0(ref object o)
        {
            return global::WeatherSystem.OnFlashing;
        }

        static StackObject* CopyToStack_OnFlashing_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = global::WeatherSystem.OnFlashing;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_OnFlashing_0(ref object o, object v)
        {
            global::WeatherSystem.OnFlashing = (System.Action)v;
        }

        static StackObject* AssignFromStack_OnFlashing_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @OnFlashing = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            global::WeatherSystem.OnFlashing = @OnFlashing;
            return ptr_of_this_method;
        }



    }
}
