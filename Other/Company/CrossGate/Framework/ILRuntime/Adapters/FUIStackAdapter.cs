using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace Framework.Adaptor
{   
    public class FUIStackAdapter : CrossBindingAdaptor
    {               
        public override Type BaseCLRType
        {
            get
            {
                return typeof(Framework.Core.UI.FUIStack);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public class Adapter : Framework.Core.UI.FUIStack, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            CrossBindingFunctionInfo<Framework.Core.UI.UIConfigData, Framework.Core.UI.FUIBase> mCreateInstance_1 = new CrossBindingFunctionInfo<Framework.Core.UI.UIConfigData, Framework.Core.UI.FUIBase>("CreateInstance");

            protected override Framework.Core.UI.FUIBase CreateInstance(Framework.Core.UI.UIConfigData configData)
            {
                if (mCreateInstance_1.CheckShouldInvokeBase(this.instance))
                    return base.CreateInstance(configData);
                else
                    return mCreateInstance_1.Invoke(this.instance, configData);
            }

            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return instance.ToString();
                }
                else
                    return instance.Type.FullName;
            }
        }
    }
}

