using Lib.Core;

namespace Logic
{
    /// <summary>
    /// 功能管理类
    /// </summary>
    public class FunctionManager
    {
        public static FunctionBase CreateFunction(EFunctionType type)
        {
            FunctionBase function = null;

            switch(type)
            {
                case EFunctionType.Group:
                    function = PoolManager.Fetch(typeof(GroupFunction)) as FunctionBase;
                    break;
                case EFunctionType.Dialogue:
                    function = PoolManager.Fetch(typeof(DialogueFunction)) as FunctionBase;
                    break;
                case EFunctionType.Task:
                    function = PoolManager.Fetch(typeof(TaskFunction)) as FunctionBase;
                    break;
                case EFunctionType.DirectlyFight:
                    function = PoolManager.Fetch(typeof(DirectlyFightFunction)) as FunctionBase;
                    break;
                case EFunctionType.Collection:
                    function = PoolManager.Fetch(typeof(CollectionFunction)) as FunctionBase;
                    break;
                case EFunctionType.Transmit:
                    function = PoolManager.Fetch(typeof(TransmitFunction)) as FunctionBase;
                    break;
                case EFunctionType.TriggerFight:
                    function = PoolManager.Fetch(typeof(TriggerFightFunction)) as FunctionBase;
                    break;
                case EFunctionType.DialogueChoose:
                    function = PoolManager.Fetch(typeof(DialogueChooseFunction)) as FunctionBase;
                    break;
                case EFunctionType.Task_PathFindOpenUI:
                    function = PoolManager.Fetch(typeof(Task_PathFindOpenUIFunction)) as FunctionBase;
                    break;
                case EFunctionType.ExecuteSystemModule:
                    function = PoolManager.Fetch(typeof(ExecuteSystemModuleFunction)) as FunctionBase;
                    break;
                case EFunctionType.SubmitItem:
                    function = PoolManager.Fetch(typeof(SubmitItemFunction)) as FunctionBase;
                    break;
                case EFunctionType.Inquiry:
                    function = PoolManager.Fetch(typeof(InquiryFunction)) as FunctionBase;
                    break;
                case EFunctionType.PetTransformation:
                    function = PoolManager.Fetch(typeof(PetTransformationFunction)) as FunctionBase;
                    break;
                case EFunctionType.Bar:
                    function = PoolManager.Fetch(typeof(BarFunction)) as FunctionBase;
                    break;
                case EFunctionType.Visit:
                    function = PoolManager.Fetch(typeof(VisitFunction)) as FunctionBase;
                    break;
                case EFunctionType.Prestige:
                    function = PoolManager.Fetch(typeof(PrestigeFunction)) as FunctionBase;
                    break;
                case EFunctionType.SignUp:
                    function = PoolManager.Fetch(typeof(SignUpFunction)) as FunctionBase;
                    break;
                case EFunctionType.Shop:
                    function = PoolManager.Fetch(typeof(ShopFunction)) as FunctionBase;
                    break;
                case EFunctionType.Advance:
                    function = PoolManager.Fetch(typeof(AdvanceFunction)) as FunctionBase;
                    break;
                case EFunctionType.Cook:
                    function = PoolManager.Fetch(typeof(CookFunction)) as FunctionBase;
                    break;
                case EFunctionType.CutScene:
                    function = PoolManager.Fetch(typeof(CutSceneFunction)) as FunctionBase;
                    break;
                case EFunctionType.InterfaceBubble:
                    function = PoolManager.Fetch(typeof(InterfaceBubbleFunction)) as FunctionBase;
                    break;
                case EFunctionType.TaskEnterArea:
                    function = PoolManager.Fetch(typeof(TaskEnterAreaFunction)) as FunctionBase;
                    break;
                case EFunctionType.OpenTeam:
                    function = PoolManager.Fetch(typeof(OpenTeamFunction)) as FunctionBase;
                    break;
                case EFunctionType.ClassicBoss:
                    function = PoolManager.Fetch(typeof(ClassicBossFunction)) as FunctionBase;
                    break;
                case EFunctionType.SecretMessage:
                    function = PoolManager.Fetch(typeof(SecretMessageFunction)) as FunctionBase;
                    break;
                case EFunctionType.ClockIn:
                    function = PoolManager.Fetch(typeof(ClockInFunction)) as FunctionBase;
                    break;
                case EFunctionType.ActiveKnowledge:
                    function = PoolManager.Fetch(typeof(ActiveKnowledgeFunction)) as FunctionBase;
                    break;
                case EFunctionType.LearnActiveSkill:
                    function = PoolManager.Fetch(typeof(LearnActiveSkillFunction)) as FunctionBase;
                    break;
                case EFunctionType.LearnPassiveSkill:
                    function = PoolManager.Fetch(typeof(LearnPassiveSkillFunction)) as FunctionBase;
                    break;
                case EFunctionType.HpMpUp:
                    function = PoolManager.Fetch(typeof(HpMpUpFunction)) as FunctionBase;
                    break;
                case EFunctionType.ResourceSubmit:
                    function = PoolManager.Fetch(typeof(ResourceSubmitFunction)) as FunctionBase;
                    break;
                case EFunctionType.FamilyBattle:
                    function = PoolManager.Fetch(typeof(FamilyBattleFunction)) as FunctionBase;
                    break;
                case EFunctionType.ReciveFamilyTask:
                    function = PoolManager.Fetch(typeof(ReciveFamilyTaskFunction)) as FunctionBase;
                    break;
                case EFunctionType.CreateWarriorGroup:
                    function = PoolManager.Fetch(typeof(CreateWarriorGroupFunction)) as FunctionBase;
                    break;
                default:
                    DebugUtil.LogError($"CreateFunction Error {type.ToString()} 未解析");
                    break;
            }

            return function;
        }
    }
}
