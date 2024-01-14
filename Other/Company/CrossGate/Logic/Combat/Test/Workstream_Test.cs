#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using static WorkStreamManagerEntity;

public class Workstream_Test : MonoBehaviour
{
    public WorkStreamManagerEntity m_WorkStreamManagerEntity;
    public List<WorkStreamTranstionComponent> m_WorkStreamTranstionComponentList;

    public static void AddWorkstream_Test<T>(T t, GameObject go) where T : WorkStreamManagerEntity 
    {
        Workstream_Test workstream_Test = CombatHelp.GetNeedComponent<Workstream_Test>(go);
        workstream_Test.m_WorkStreamManagerEntity = t;
    }

    public static Workstream_Test GetWorkstream_Test(GameObject go) 
    {
        if (go == null)
            return null;

        Workstream_Test workstream_Test = go.GetComponentInChildren<Workstream_Test>();
        return workstream_Test;
    }

    public void SetWorkStreamTranstionComponentList()
    {
        if (m_WorkStreamTranstionComponentList == null)
            m_WorkStreamTranstionComponentList = new List<WorkStreamTranstionComponent>();
        else
            m_WorkStreamTranstionComponentList.Clear();

        GetWorkStreamTranstionComponent(m_WorkStreamManagerEntity.m_MainStreamController, m_WorkStreamTranstionComponentList);
        BaseStreamControllerEntity[] bsceArray = m_WorkStreamManagerEntity.GetMainStreamControllerStack();
        for (int i = 0, count = bsceArray.Length; i < count; i++)
        {
            GetWorkStreamTranstionComponent(bsceArray[i], m_WorkStreamTranstionComponentList);
        }

        int parallelCount = m_WorkStreamManagerEntity.GetParallelStreamControllerDataListCount();
        if (parallelCount > 0) 
        {
            for (int pIndex = 0; pIndex < parallelCount; pIndex++)
            {
                StreamControllerData streamControllerData = m_WorkStreamManagerEntity.GetStreamControllerData(pIndex);
                if (streamControllerData == null)
                    continue;

                if(streamControllerData.BaseStreamController != null)
                    GetWorkStreamTranstionComponent(streamControllerData.BaseStreamController, m_WorkStreamTranstionComponentList);

                if (streamControllerData.BaseStreamControllerStack != null) 
                {
                    BaseStreamControllerEntity[] pbsceArray = streamControllerData.BaseStreamControllerStack.ToArray();
                    for (int scdIndex = 0, scdCount = pbsceArray.Length; scdIndex < scdCount; scdIndex++)
                    {
                        GetWorkStreamTranstionComponent(pbsceArray[scdIndex], m_WorkStreamTranstionComponentList);
                    }
                }
            }
        }
    }

    private void GetWorkStreamTranstionComponent(BaseStreamControllerEntity baseStreamControllerEntity, 
        List<WorkStreamTranstionComponent> wstcList)
    {
        if (baseStreamControllerEntity == null)
            return;

        WorkStreamTranstionComponent wstc = baseStreamControllerEntity.m_FirstMachine.GetComponent<WorkStreamTranstionComponent>();
        if (wstc != null)
            wstcList.Add(wstc);

        if (baseStreamControllerEntity.GetOtherMachineDic() != null) 
        {
            foreach (var kv in baseStreamControllerEntity.GetOtherMachineDic())
            {
                foreach (var sme in kv.Value)
                {
                    wstc = sme.GetComponent<WorkStreamTranstionComponent>();
                    if (wstc != null)
                        wstcList.Add(wstc);
                }
            }
        }
    }
}
#endif