[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.WaitFrameAfterRecordNTimes)]
public class WS_CombatBehaveAI_WaitFrameAfterRecordNTimes_SComponent : StateBaseComponent, IUpdate
{
	public override void Init(string str)
	{
        WS_WaitFrameAfterRecordNTimesComponent wfarnt = m_CurUseEntity.GetNeedComponent<WS_WaitFrameAfterRecordNTimesComponent>();
        wfarnt.Init(int.Parse(str));
        if(!wfarnt.IsNeedWait())
            m_CurUseEntity.TranstionMultiStates(this);
    }

    public void Update()
    {
        m_CurUseEntity.TranstionMultiStates(this);
    }
}

public class WS_WaitFrameAfterRecordNTimesComponent : BaseComponent<StateMachineEntity>
{
    public int m_RecordNTimes;

    public override void Dispose()
    {
        m_RecordNTimes = 0;

        base.Dispose();
    }

    public void Init(int times)
    {
        if (m_RecordNTimes < 1)
            m_RecordNTimes = times;
    }

    public bool IsNeedWait()
    {
        --m_RecordNTimes;

        if (m_RecordNTimes < 1)
        {
            m_RecordNTimes = 0;

            return true;
        }

        return false;
    }
}