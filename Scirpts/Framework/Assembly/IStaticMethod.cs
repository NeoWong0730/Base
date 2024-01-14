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