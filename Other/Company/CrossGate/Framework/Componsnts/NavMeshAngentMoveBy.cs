using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshAngentMoveBy : MonoBehaviour
{
    public Transform mTransform;
    public NavMeshAgent mNavMeshAgent;

    public Vector3 Velocity;


    // for target
    public Vector3 TargetPos;
    public Vector3 TargetDirNormal;

    public bool IsHaveTarget = false;

    public Transform TargetTransform;
    public NavMeshAgent TargetNavMeshAgeng;
    public float KeepDistance = 0;
    public float Speed = 0;

    private bool hasTriggered = false;
    public Action OnFollow;
    public Action StopFollow;
    public Func<bool> CanFollow;

    public int Index = -1;


    public Action EnterKeepDistance;
    public Action ExitKeepDistance;

    public float LastDistance = 0;

    private void Start()
    {
        mTransform = transform;
        mNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {

        if (IsHaveTarget)
        {
            UpdateMoveToTaget();

        }
        else
        {
            var nextposition = mTransform.position + Velocity * Time.deltaTime;
            mNavMeshAgent.nextPosition = nextposition;
        }



    }

    private void UpdateMoveToTaget()
    {

        Velocity = GetSpeedByTarget();

        transform.forward = Velocity.normalized;
        
        float distance = (TargetTransform.position - mTransform.position).magnitude;

        Vector3 moveDirNormal = (TargetTransform.position - mTransform.position).normalized;

        float diff = distance - KeepDistance;

        // 追击停止的判断
        if (diff < 0) {
            if(this.CanFollow != null) {
                if (this.CanFollow.Invoke()) {
                    if (!this.hasTriggered) {
                        this.OnFollow?.Invoke();

                        this.CanFollow = null;
                        hasTriggered = true;
                        return;
                    }
                }
                else {
                    this.StopFollow?.Invoke();
                    
                    this.CanFollow = null;
                    hasTriggered = true;
                    return;
                }
            }
        }

        TargetPos = (mTransform.position + moveDirNormal * diff);

        Vector3 dir = (TargetPos - mTransform.position).normalized;

        //当下一个目标点超过目标位置时，将会不跟新下个目标点
        var nextposition = mTransform.position + Velocity * Time.deltaTime;

        Vector3 targetdir = (nextposition - mTransform.position).normalized;


//        Debug.LogError(Index.ToString() + "++++++++++++++++++++++++" + "move by --- " + "pos : " + mTransform.position.ToString() + "nexpos: " + nextposition.ToString() +

//"targetpos : " + TargetPos.ToString() + "targetdir : " + dir.ToString() + "nextdir : " + targetdir.ToString() + "dot : " + Vector3.Dot(dir, targetdir));


        LastDistance = distance;

        mNavMeshAgent.nextPosition = nextposition;

    }
    public void SetTarget(float speed, float keepdistance,Transform targettrans, 
        Func<bool> canFollow = null,
        Action onFollow = null,
        Action stopFollow = null)
    {
        TargetTransform = targettrans;

        Speed = speed;

        KeepDistance = keepdistance;

        IsHaveTarget = true;

        CanFollow = canFollow;
        OnFollow = onFollow;
        StopFollow = stopFollow;
        hasTriggered = false;

        LastDistance = (targettrans.position - transform.position).magnitude;

        //if (LastDistance <= KeepDistance)
        //    EnterKeepDistance?.Invoke();
        //else
        //    ExitKeepDistance?.Invoke();
    }

    private Vector3 GetSpeedByTarget()
    {
        Vector3 offset = TargetTransform.position - mTransform.position;

        //当前位置处于小于保持的距离，应该采取减速
        float speedscale = 1 + 2 * (offset.magnitude - KeepDistance);

        float speed = Mathf.Clamp(speedscale, 0f, 1.2f) * Speed;

        Vector3 dir = offset.normalized;
        dir.y = 0;

        Vector3 speed0 = dir * speed;

        return speed0;
    }
}
