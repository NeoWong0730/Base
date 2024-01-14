using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobStateControllerEntity : StateControllerEntity
{
    public MobEntity m_MobEntity;

    public void Init(MobEntity mobEntity, int stateCategory)
    {
        m_MobEntity = mobEntity;

        OnInit(stateCategory);
    }

    public override void Dispose()
    {
        m_MobEntity = null;

        base.Dispose();
    }
}
