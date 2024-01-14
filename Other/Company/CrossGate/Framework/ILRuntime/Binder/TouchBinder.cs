using Framework;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchBinder : ValueTypeBinder<Touch>
{
    public override unsafe void AssignFromStack(ref Touch ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        ins.fingerId = v->Value;

        v = ILIntepreter.Minus(ptr, 3);
        float x1 = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 4);
        float y1 = *(float*)&v->Value;
        ins.position = new Vector2(x1, y1);
        
        v = ILIntepreter.Minus(ptr, 6);
        float x2 = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 7);
        float y2 = *(float*)&v->Value;
        ins.rawPosition = new Vector2(x2,y2);
        

        v = ILIntepreter.Minus(ptr, 9);
        float x3 = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 10);
        float y3 = *(float*)&v->Value;
        ins.deltaPosition = new Vector2(x3, y3);


        v = ILIntepreter.Minus(ptr, 11);
        ins.deltaTime = *(float*)&v->Value;

        v = ILIntepreter.Minus(ptr, 12);
        ins.tapCount = v->Value;

        v = ILIntepreter.Minus(ptr, 13);
        ins.phase = (TouchPhase)v->Value;

        v = ILIntepreter.Minus(ptr, 14);
        ins.pressure = *(float*)&v->Value;

        v = ILIntepreter.Minus(ptr, 15);
        ins.maximumPossiblePressure = *(float*)&v->Value;

        v = ILIntepreter.Minus(ptr, 16);
        ins.type = (TouchType)v->Value;
        
        v = ILIntepreter.Minus(ptr, 17);
        ins.altitudeAngle = *(float*)&v->Value;
        
        v = ILIntepreter.Minus(ptr, 18);
        ins.azimuthAngle = *(float*)&v->Value;

        v = ILIntepreter.Minus(ptr, 19);
        ins.radius = *(float*)&v->Value;

        v = ILIntepreter.Minus(ptr, 20);
        ins.radiusVariance = *(float*)&v->Value;
        //Lib.Core.DebugUtil.LogFormat(Lib.Core.ELogType.eTestILruntime, "yd----TouchBinder------AssignFromStack------------");
    }

    public override unsafe void CopyValueTypeToStack(ref Touch ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        v->Value = ins.fingerId;

        v = ILIntepreter.Minus(ptr, 3);
        *(float*)&v->Value = ins.position.x;
        v = ILIntepreter.Minus(ptr, 4);
        *(float*)&v->Value = ins.position.y;
        
        v = ILIntepreter.Minus(ptr, 6);
        *(float*)&v->Value = ins.rawPosition.x;
        v = ILIntepreter.Minus(ptr, 7);
        *(float*)&v->Value = ins.rawPosition.y;
        
        v = ILIntepreter.Minus(ptr, 9);
        *(float*)&v->Value = ins.deltaPosition.x;
        v = ILIntepreter.Minus(ptr, 10);
        *(float*)&v->Value = ins.deltaPosition.y;
        
        v = ILIntepreter.Minus(ptr, 11);
        *(float*)&v->Value = ins.deltaTime;

        v = ILIntepreter.Minus(ptr, 12);
        v->Value = ins.tapCount;
        
        v = ILIntepreter.Minus(ptr, 13);
        v->Value = (int)ins.phase;

        v = ILIntepreter.Minus(ptr, 14);
        *(float*)&v->Value = ins.pressure;

        v = ILIntepreter.Minus(ptr, 15);
        *(float*)&v->Value = ins.maximumPossiblePressure;

        v = ILIntepreter.Minus(ptr, 16);
        v->Value = (int)ins.type;

        v = ILIntepreter.Minus(ptr, 17);
        *(float*)&v->Value = ins.altitudeAngle;

        v = ILIntepreter.Minus(ptr, 18);
        *(float*)&v->Value = ins.azimuthAngle;
        
        v = ILIntepreter.Minus(ptr, 19);
        *(float*)&v->Value = ins.radius;

        v = ILIntepreter.Minus(ptr, 20);
        *(float*)&v->Value = ins.radiusVariance;

       // Lib.Core.DebugUtil.LogFormat(Lib.Core.ELogType.eTestILruntime, "yd----TouchBinder------CopyValueTypeToStack------------");
    }
}
