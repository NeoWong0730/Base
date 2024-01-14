namespace Logic
{
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.B_EnterTrigger)]
    public class WS_NPCBehaveAI_B_EnterTrigger_SComponent : StateBaseComponent
    {
        public override void Init(string str)
        {
            m_CurUseEntity.TranstionMultiStates(this);
        }
    }
}
