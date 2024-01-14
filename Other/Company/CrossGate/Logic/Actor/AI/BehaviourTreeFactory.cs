using System;
using System.Collections.Generic;
using UnityEngine;

public static class BTFactory
{
    public static BTRoot GetRoot(bool repeated) { return new BTRoot(repeated); }
    public static BTSequence Sequence() { return new BTSequence(); }
    public static BTSelector Selector() { return new BTSelector(); }

    public static BTSimpleAction Call(Action action) { return new BTSimpleAction(action); }
    public static BTAction_PathFind PathFind(Transform from, Transform to, float speed) { return new BTAction_PathFind(from, to, speed); }

    public static BTConditions If(Func<bool> func) { return new BTConditions(func); }
}
