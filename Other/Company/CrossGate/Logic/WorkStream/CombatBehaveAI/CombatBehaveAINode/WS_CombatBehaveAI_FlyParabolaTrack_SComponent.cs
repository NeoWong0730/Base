[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.FlyParabolaTrack)]
public class WS_CombatBehaveAI_FlyParabolaTrack_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        var mob = (MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent;

        float[] fs = CombatHelp.GetStrParseFloat1Array(str);
        
        TrackEntity trackEntity = EntityFactory.Create<TrackEntity>();
        FlyParabolaBhComp_25D flyParabolaBhComp_25D = trackEntity.GetNeedComponent<FlyParabolaBhComp_25D>();

        float total = 1000f;
        if (fs.Length > 4)
        {
            flyParabolaBhComp_25D.m_TrackOverType = 0;
            total = fs[4];
        }
        else
        {
            flyParabolaBhComp_25D.m_TrackOverType = 1;
        }

        flyParabolaBhComp_25D.Init(mob.m_Trans, fs[0], fs[1], fs[2], fs[3], total);

        trackEntity.TrackOverAction = () =>
        {
            m_CurUseEntity.TranstionMultiStates(this);
        };
    }
}