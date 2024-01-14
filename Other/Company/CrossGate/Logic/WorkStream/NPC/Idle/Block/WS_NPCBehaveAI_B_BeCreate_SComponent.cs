namespace Logic
{
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.B_BeCreate)]
    public class WS_NPCBehaveAI_B_BeCreate_SComponent : StateBaseComponent
    {
        public override void Init(string str)
        {
            m_CurUseEntity.TranstionMultiStates(this);
        }
    }
}
