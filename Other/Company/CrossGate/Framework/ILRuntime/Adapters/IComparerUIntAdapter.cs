using System;
using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class IComparerUIntAdapter : CrossBindingAdaptor {
    public override Type BaseCLRType {
        get {
            return typeof(IComparer<uint>);
        }
    }

    public override Type AdaptorType {
        get {
            return typeof(Adaptor);
        }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance) {
        return new Adaptor(appdomain, instance);
    }

    public class Adaptor : IComparer<uint>, CrossBindingAdaptorType {
        readonly ILTypeInstance _instance;
        readonly ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        public Adaptor() {

        }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance) {
            this.appdomain = appdomain;
            this._instance = instance;
        }

        public object[] data1 = new object[2];

        public ILTypeInstance ILInstance { get { return this._instance; } }

        IMethod mCompare;
        bool mCompareGot = false;

        public int Compare(uint x, uint y) {
            if (!this.mCompareGot) {
                this.mCompare = this._instance.Type.GetMethod("Compare", 2);
                if (this.mCompare == null) {
                    this.mCompare = this._instance.Type.GetMethod("System.Collections.Generic.IComparer.Compare", 2);
                }
                this.mCompareGot = true;
            }

            if (this.mCompare != null) {
                this.data1[0] = x;
                this.data1[1] = y;
                return (int)this.appdomain.Invoke(this.mCompare, this._instance, this.data1);
            }
            return 0;
        }
    }
}
