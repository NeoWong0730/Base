using Framework;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickDataBinder : ValueTypeBinder<JoystickData>
{
    public override unsafe void AssignFromStack(ref JoystickData ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        ins.dis = *(float*)&v->Value;

        v = ILIntepreter.Minus(ptr, 2);
        ins.state = (JoystickState)(v->Value);

        v = ILIntepreter.Minus(ptr, 3);
        float x = *(float*)&v->Value;

        v = ILIntepreter.Minus(ptr, 4);
        float y = *(float*)&v->Value;

        ins.dir = new Vector2(x, y);
        //Lib.Core.DebugUtil.LogFormat(Lib.Core.ELogType.eTestILruntime, "yd----JoystickDataBinder------AssignFromStack------------");
    }

    public override unsafe void CopyValueTypeToStack(ref JoystickData ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        *(float*)&v->Value = ins.dis;

        v = ILIntepreter.Minus(ptr, 2);
        v->Value = (int)ins.state;

        v = ILIntepreter.Minus(ptr, 3);
        *(float*)&v->Value = ins.dir.x;

        v = ILIntepreter.Minus(ptr, 4);
        *(float*)&v->Value = ins.dir.y;

        //Lib.Core.DebugUtil.LogFormat(Lib.Core.ELogType.eTestILruntime, "yd----JoystickDataBinder------CopyValueTypeToStack------------");
    }
}