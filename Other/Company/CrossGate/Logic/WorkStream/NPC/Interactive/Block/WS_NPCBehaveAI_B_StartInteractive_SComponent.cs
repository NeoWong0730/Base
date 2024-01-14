namespace Logic
{
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.B_StartInteractive)]
    public class WS_NPCBehaveAI_B_StartInteractive_SComponent : StateBaseComponent
    {
        public override void Init(string str)
        {
            m_CurUseEntity.TranstionMultiStates(this);
        }
    }

    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.B_EndInteractive)]
    public class WS_NPCBehaveAI_B_EndInteractive_SComponent : StateBaseComponent
    {
        public override void Init(string str)
        {
            m_CurUseEntity.TranstionMultiStates(this);
        }
    }

    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.B_WorldBossEnterFight)]
    public class WS_NPCBehaveAI_B_WorldBossEnterFight_SComponent : StateBaseComponent
    {
        public override void Init(string str)
        {
            m_CurUseEntity.TranstionMultiStates(this);
        }
    }

    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.B_WorldBossExitFight_Success)]
    public class WS_NPCBehaveAI_B_WorldBossExitFight_Success_SComponent : StateBaseComponent
    {
        public override void Init(string str)
        {
            m_CurUseEntity.TranstionMultiStates(this);
        }
    }

    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.B_WorldBossExitFight_Faild)]
    public class WS_NPCBehaveAI_B_WorldBossExitFight_Faild_SComponent : StateBaseComponent
    {
        public override void Init(string str)
        {
            m_CurUseEntity.TranstionMultiStates(this);
        }
    }

    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.B_WorldBossDead)]
    public class WS_NPCBehaveAI_B_WorldBossDead_SComponent : StateBaseComponent
    {
        public override void Init(string str)
        {
            m_CurUseEntity.TranstionMultiStates(this);
        }
    }
}
