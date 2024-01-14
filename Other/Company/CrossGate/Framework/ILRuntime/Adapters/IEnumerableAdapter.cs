using Google.Protobuf;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections;
using System.Collections.Generic;

public class IEnumerableAdapter : CrossBindingAdaptor
{
    public override Type BaseCLRType
    {
        get { return typeof(IEnumerable); }
    }

    public override Type AdaptorType
    {
        get { return typeof(Adaptor); }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);
    }

    internal class Adaptor : IEnumerable, CrossBindingAdaptorType
    {
        private ILTypeInstance __instance;
        private ILRuntime.Runtime.Enviorment.AppDomain mAppdomain;

        private IMethod mGetEnumeratorMethod;
        private bool mGetEnumeratorMethodGot = false;

        public Adaptor()
        {
        }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            this.mAppdomain = appdomain;
            this.__instance = instance;
        }

        public ILTypeInstance ILInstance { get { return __instance; } }

        public IEnumerator GetEnumerator()
        {
            if (!mGetEnumeratorMethodGot)
            {
                mGetEnumeratorMethod = __instance.Type.GetMethod("GetEnumerator", 0);
                mGetEnumeratorMethodGot = true;
            }

            if (mGetEnumeratorMethod != null)
            {
                var res = mAppdomain.Invoke(mGetEnumeratorMethod, __instance, null);
                return (IEnumerator)res;
            }
            else
            {
                return null;
            }
        }
    }
}

public class IMessageEnumerableAdapter : CrossBindingAdaptor
{
    public override Type BaseCLRType
    {
        get { return typeof(IEnumerable<Adapt_IMessage.Adaptor>); }
    }

    public override Type AdaptorType
    {
        get { return typeof(Adaptor); }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);
    }

    internal class Adaptor : IEnumerable<Adapt_IMessage>, CrossBindingAdaptorType
    {
        private ILTypeInstance __instance;
        private ILRuntime.Runtime.Enviorment.AppDomain mAppdomain;

        private IMethod mGetEnumeratorMethod;
        private bool mGetEnumeratorMethodGot = false;

        private IMethod mGetEnumeratorIMessageMethod;
        private bool mGetEnumeratorMessageMethodGot = false;

        public Adaptor()
        {
        }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            this.mAppdomain = appdomain;
            this.__instance = instance;
        }

        public ILTypeInstance ILInstance { get { return __instance; } }

        public IEnumerator GetEnumerator()
        {
            if (!mGetEnumeratorMethodGot)
            {
                mGetEnumeratorMethod = __instance.Type.GetMethod("GetEnumerator", 0);
                mGetEnumeratorMethodGot = true;
            }

            if (mGetEnumeratorMethod != null)
            {
                var res = mAppdomain.Invoke(mGetEnumeratorMethod, __instance, null);
                return (IEnumerator)res;
            }
            else
            {
                return null;
            }
        }

        IEnumerator<Adapt_IMessage> IEnumerable<Adapt_IMessage>.GetEnumerator()
        {
            if (!mGetEnumeratorMessageMethodGot)
            {
                mGetEnumeratorIMessageMethod = __instance.Type.GetMethod("GetEnumerator", 0);
                mGetEnumeratorMessageMethodGot = true;
            }

            if (mGetEnumeratorIMessageMethod != null)
            {
                var res = mAppdomain.Invoke(mGetEnumeratorIMessageMethod, __instance, null);
                return (IEnumerator<Adapt_IMessage>)res;
            }
            else
            {
                return null;
            }
        }
    }
}
