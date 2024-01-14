using ILRuntime.CLR.Method;
using System.Reflection;

namespace Framework
{
    public interface IStaticMethod
    {
        object Run();
        object Run(object a);
        object Run(object a, object b);
        object Run(object a, object b, object c);
    }

    public interface IInstanceMethod
    {
        object Run();
        object Run(object a);
        object Run(object a, object b);
        object Run(object a, object b, object c);
    }

    public class ILStaticMethod : IStaticMethod
    {
        public readonly ILRuntime.Runtime.Enviorment.AppDomain appDomain;
        public readonly IMethod method;
        public readonly object[] param;

        public ILStaticMethod(ILRuntime.Runtime.Enviorment.AppDomain appDomain, string typeName, string methodName, int paramsCount = 0)
        {
            this.appDomain = appDomain;
            method = appDomain.LoadedTypes[typeName].GetMethod(methodName, paramsCount);
            param = new object[paramsCount];
        }

        public object Run()
        {
            return appDomain.Invoke(method, null, param);
        }

        public object Run(object a)
        {
            param[0] = a;
            return appDomain.Invoke(method, null, param);
        }

        public object Run(object a, object b)
        {
            param[0] = a;
            param[1] = b;
            return appDomain.Invoke(method, null, param);
        }

        public object Run(object a, object b, object c)
        {
            param[0] = a;
            param[1] = b;
            param[2] = c;
            return appDomain.Invoke(method, null, param);
        }
    }

    public class ILInstanceMethod : IInstanceMethod
    {
        public readonly ILRuntime.Runtime.Enviorment.AppDomain appDomain;
        public readonly IMethod method;
        public readonly object instance;
        public readonly object[] param;

        public ILInstanceMethod(ILRuntime.Runtime.Enviorment.AppDomain appDomain, string typeName, string methodName, ref object _instance, int paramsCount)
        {
            this.appDomain = appDomain;
            method = appDomain.LoadedTypes[typeName].GetMethod(methodName, paramsCount);
            if (_instance == null)
            {
                _instance = appDomain.Instantiate(typeName);
            }
            instance = _instance;
            param = new object[paramsCount];
        }

        public object Run()
        {
            return appDomain.Invoke(method, instance, param);
        }

        public object Run(object a)
        {
            param[0] = a;
            return appDomain.Invoke(method, instance, param);
        }

        public object Run(object a, object b)
        {
            param[0] = a;
            param[1] = b;
            return appDomain.Invoke(method, instance, param);
        }

        public object Run(object a, object b, object c)
        {
            param[0] = a;
            param[1] = b;
            param[2] = c;
            return appDomain.Invoke(method, instance, param);
        }
    }

    public class MonoStaticMethod : IStaticMethod
    {
        public readonly MethodInfo method;
        public readonly object[] param;

        public MonoStaticMethod(Assembly assembly, string typeName, string methodName)
        {
            method = assembly.GetType(typeName).GetMethod(methodName);
            param = new object[method.GetParameters().Length];
        }

        public object Run()
        {
            return method.Invoke(null, param);
        }

        public object Run(object a)
        {
            param[0] = a;
            return method.Invoke(null, param);
        }

        public object Run(object a, object b)
        {
            param[0] = a;
            param[1] = b;
            return method.Invoke(null, param);
        }

        public object Run(object a, object b, object c)
        {
            param[0] = a;
            param[1] = b;
            param[2] = c;
            return method.Invoke(null, param);
        }
    }

    public class MonoInstanceMethod : IInstanceMethod
    {
        public readonly MethodInfo method;
        public readonly object instance;
        public readonly object[] param;

        public MonoInstanceMethod(Assembly assembly, string typeName, string methodName, ref object _instance)
        {
            method = assembly.GetType(typeName).GetMethod(methodName);
            if (_instance == null)
            {
                _instance = assembly.CreateInstance(typeName);
            }
            instance = _instance;

            param = new object[method.GetParameters().Length];
        }

        public object Run()
        {
            return method.Invoke(instance, param);
        }

        public object Run(object a)
        {
            param[0] = a;
            return method.Invoke(instance, param);
        }

        public object Run(object a, object b)
        {
            param[0] = a;
            param[1] = b;
            return method.Invoke(instance, param);
        }

        public object Run(object a, object b, object c)
        {
            param[0] = a;
            param[1] = b;
            param[2] = c;
            return method.Invoke(instance, param);
        }
    }
}
