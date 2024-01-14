using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrackType
{
    None = 0,
    BezierCurve = 1,
    FlyLine = 2,
    FlyParabola_25D = 3,
}

public class BaseTrackEntity : AEntity
{
    public virtual void TrackOver() { }

    public virtual void Hit(bool isForce = false) { }
}

public class TrackEntity : BaseTrackEntity
{
    public Action TrackOverAction;

    public override void Dispose()
    {
        TrackOverAction = null;

        base.Dispose();
    }

    public override void TrackOver()
    {
        if (TrackOverAction != null)
            TrackOverAction.Invoke();

        Dispose();
    }
}
