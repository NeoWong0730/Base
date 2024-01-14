using System;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;

public class IComparableAdapter : CrossBindingAdaptor
{
    public override Type BaseCLRType
    {
        get
        {
            return typeof(IComparable<ILTypeInstance>);
        }
    }

    public override Type AdaptorType
    {
        get
        {
            return typeof(Adaptor);
        }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);
    }

    public class Adaptor : IComparable<ILTypeInstance>, CrossBindingAdaptorType
    {
        ILTypeInstance _instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        public Adaptor()
        {

        }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            this.appdomain = appdomain;
            this._instance = instance;
        }

        public object[] data1 = new object[1];

        public ILTypeInstance ILInstance { get { return _instance; } }

        IMethod mCompareTo;
        bool mCompareToGot = false;

        public int CompareTo(ILTypeInstance other)
        {
            if (!mCompareToGot)
            {
                mCompareTo = _instance.Type.GetMethod("CompareTo", 1);
                if (mCompareTo == null)
                {
                    mCompareTo = _instance.Type.GetMethod("System.IComparable.CompareTo", 1);
                }
                mCompareToGot = true;
            }

            if (mCompareTo != null)
            {
                data1[0] = other;
                return (int)appdomain.Invoke(mCompareTo, _instance, data1);
            }
            return 0;
        }
    }
}
