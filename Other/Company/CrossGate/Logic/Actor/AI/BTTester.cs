using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTTester : MonoBehaviour
{
    public Transform aiTarget;
    public Transform another;
    private BTRoot bt = BTFactory.GetRoot(false);

    private void OnEnable()
    {
        bt.Init(
            // 判断是否存在目标
            BTFactory.If(() => { return aiTarget != null; }).Init(
                // 是否在可视范围之内
                BTFactory.Call(CheckTargetStillVisible),

                // 朝向target
                BTFactory.Call(FaceToTarget),
                BTFactory.PathFind(transform, aiTarget, 1f),
                
                // 记录目标位置
                BTFactory.Call(RememberTargetPosition),

                BTFactory.PathFind(transform, another, 1f)
            ),

            // 如果target被攻击而死
            BTFactory.If(() => { return aiTarget == null; }).Init(
                // 重新查找target
                BTFactory.Call(FindTarget)
            )
        );
    }

    private void Update()
    {
        // 驱动ai执行
        bt.Tick();
    }

    #region 测试函数
    private void CheckTargetStillVisible()
    {
        // 判断aiTarget和当前的距离，如果在范围之内，追击，否则巡逻
    }
    private void FaceToTarget()
    {
        // 面朝向玩家

        Debug.LogError("FaceToTarget");
        transform.LookAt(aiTarget.transform);
    }
    private void RememberTargetPosition()
    {
        // 记录当前玩家位置
        Debug.LogError("RememberTargetPosition");
    }
    private void FindTarget()
    {
        Debug.LogError("FindTarget");
    }
    #endregion
}