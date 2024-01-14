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

public class CharacterInfoBinder : ValueTypeBinder<CharacterInfo>
{
    public override unsafe void AssignFromStack(ref CharacterInfo ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        ins.index = v->Value;

        v = ILIntepreter.Minus(ptr, 3);
        float uvx = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 4);
        float uvy = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 5);
        float uvwidth = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 6);
        float uvheight = *(float*)&v->Value;
        ins.uv = new Rect(uvx,uvy,uvwidth,uvheight);


        v = ILIntepreter.Minus(ptr, 8);
        float vertx = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 9);
        float verty = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 10);
        float vertwidth = *(float*)&v->Value;
        v = ILIntepreter.Minus(ptr, 11);
        float vertheight = *(float*)&v->Value;
        ins.uv = new Rect(vertx, verty, vertwidth, vertheight);


        v = ILIntepreter.Minus(ptr, 12);
        ins.width = *(float*)&v->Value;

        v = ILIntepreter.Minus(ptr, 13);
        ins.size = v->Value;

        v = ILIntepreter.Minus(ptr, 14);
        ins.style = *(FontStyle*)&v->Value;

        v = ILIntepreter.Minus(ptr, 15);
        ins.flipped = *(bool*)&v->Value;

    }

    public override unsafe void CopyValueTypeToStack(ref CharacterInfo ins, StackObject* ptr, IList<object> mStack)
    {
        var v = ILIntepreter.Minus(ptr, 1);
        v->Value = ins.index;

        v = ILIntepreter.Minus(ptr, 3);
        *(float*)&v->Value = ins.uv.x;
        v = ILIntepreter.Minus(ptr, 4);
        *(float*)&v->Value = ins.uv.y;
        v = ILIntepreter.Minus(ptr, 5);
        *(float*)&v->Value = ins.uv.width;
        v = ILIntepreter.Minus(ptr, 6);
        *(float*)&v->Value = ins.uv.height;

        v = ILIntepreter.Minus(ptr, 8);
        *(float*)&v->Value = ins.vert.x;
        v = ILIntepreter.Minus(ptr, 9);
        *(float*)&v->Value = ins.vert.y;
        v = ILIntepreter.Minus(ptr, 10);
        *(float*)&v->Value = ins.vert.width;
        v = ILIntepreter.Minus(ptr, 11);
        *(float*)&v->Value = ins.vert.height;
        

        v = ILIntepreter.Minus(ptr, 12);
        *(float*)&v->Value = ins.width;
        v = ILIntepreter.Minus(ptr, 13);
        *(float*)&v->Value = ins.size;
        v = ILIntepreter.Minus(ptr, 14);
        *(FontStyle*)&v->Value = ins.style;
        v = ILIntepreter.Minus(ptr, 15);
        *(bool*)&v->Value = ins.flipped;

    }
}
