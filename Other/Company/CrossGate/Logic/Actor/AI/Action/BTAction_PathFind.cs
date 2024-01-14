using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTAction_PathFind : BTAction
{
    public Transform from;
    public Transform to;
    public float speed = 0.1f;

    public BTAction_PathFind(Transform from, Transform to, float speed)
    {
        this.from = from;
        this.to = to;
        this.speed = speed;
    }

    public override bool IsCompleted()
    {
        return Vector3.Distance(from.position, to.position) < 0.1f;
    }

    public override EBTStatus Tick()
    {
        EBTStatus retStatus = EBTStatus.Success;
        if (IsCompleted())
        {
            retStatus = EBTStatus.Success;
        }
        else
        {
            from.position = Vector3.MoveTowards(from.position, to.transform.position, Time.deltaTime * speed);
            retStatus = EBTStatus.Running;
        }

        return retStatus;
    }
}