using UnityEngine;
using System.Collections.Generic;
using ILRuntime.Other;
using System;
using System.Reflection;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Stack;
using Net;
using Google.Protobuf;

public class Net_NetMsgBinder : ValueTypeBinder<NetMsg>
{
    public override unsafe void AssignFromStack(ref NetMsg ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        ins.evtId = *(UInt16*)&v->Value;

        v = ILIntepreter.Minus(ptr, 2);
        ins.bodyLength = *(UInt16*)&v->Value;

        v = ILIntepreter.Minus(ptr, 3);
        object obj = mStack[v->Value];
        ins.data = obj as IMessage;

       // Lib.Core.DebugUtil.LogFormat(Lib.Core.ELogType.eTestILruntime, "yd----Net_NetMsgBinder------AssignFromStack------------");
    }

    public override unsafe void CopyValueTypeToStack(ref NetMsg ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        *(UInt16*)&v->Value = ins.evtId;

        v = ILIntepreter.Minus(ptr, 2);
        *(UInt16*)&v->Value = ins.bodyLength;

        v = ILIntepreter.Minus(ptr, 3);
        mStack[v->Value] = ins.data;

       // Lib.Core.DebugUtil.LogFormat(Lib.Core.ELogType.eTestILruntime, "yd----Net_NetMsgBinder------CopyValueTypeToStack------------");
    }
}
