using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;

namespace Logic
{
    public class InquiryAction : ActionBase
    {
        //public const string TypeName = "Logic.InquiryAction";

        public CSVDetect.Data CSVDetectData
        {
            get;
            set;
        }

        public enum EEvents
        {
            StartInquiry,
            InterrputInquiry,
            InquiryCompleted,
        }

        //Timer timer;

        public static bool InquiryCompleted;

        public static readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        protected override void ProcessEvents(bool toRegister)
        {
            base.ProcessEvents(toRegister);

            if (GameCenter.mainHero != null && GameCenter.mainHero.stateComponent != null)
            {
                if (toRegister)
                {
                    GameCenter.mainHero.stateComponent.StateChange += OnStateChange;
                }
                else
                {
                    GameCenter.mainHero.stateComponent.StateChange -= OnStateChange;
                }
            }
        }

        protected override void OnDispose()
        {
            InquiryCompleted = false;
            CSVDetectData = null;
            Sys_Inquiry.Instance.timer?.Cancel();
            Sys_Inquiry.Instance.timer = null;

            base.OnDispose();
        }

        void OnStateChange(EStateType oldState, EStateType newState)
        {
            if (oldState == EStateType.Inquiry)
            {
                if (newState == EStateType.Run)
                {
                    Interrupt();
                }
            }
        }

        protected override void OnExecute()
        {
            GameCenter.mainHero.movementComponent.Stop();
            GameCenter.mainHero.stateComponent.ChangeState(EStateType.Inquiry);
            InquiryCompleted = false;
            
            eventEmitter.Trigger<CSVDetect.Data>(EEvents.StartInquiry, CSVDetectData);
            Sys_Inquiry.Instance.timer?.Cancel();
            Sys_Inquiry.Instance.timer = Timer.Register(CSVDetectData.duration / 1000f, () =>
            {
                CmdNpcInvestigateReq cmdNpcInvestigateReq = new CmdNpcInvestigateReq();
                cmdNpcInvestigateReq.Id = CSVDetectData.id;
                cmdNpcInvestigateReq.NpcId = Sys_Interactive.CurInteractiveNPC.uID;

                cmdNpcInvestigateReq.TaskId = CSVDetectData.TaskID;
                cmdNpcInvestigateReq.TaskIndex = CSVDetectData.TaskTargetNum;

                DebugUtil.Log(ELogType.eNPC, "InvestigateReq");
                NetClient.Instance.SendMessage((ushort)CmdNpc.InvestigateReq, cmdNpcInvestigateReq);
            }, null, false, true);

            GameCenter.mainHero.animationComponent.StopAll();
            GameCenter.mainHero.animationComponent.Play(CSVDetectData.DetectAction, () =>
            {
                GameCenter.mainHero.animationComponent.Play((uint)EStateType.Idle);
            });

            CSVEffect.Data cSVEffectData = CSVEffect.Instance.GetConfData(CSVDetectData.Fx);
            if (cSVEffectData != null)
            {
                float yOffset = 0f;
                if (GameCenter.mainHero.heroBaseComponent.TitleId != 0)
                {
                    yOffset = 0.95f;               
                }
                if (GameCenter.mainHero.Mount != null)
                {
                    yOffset += GameCenter.mainHero.Mount.csvPetData.mountsignposition[1] / 1000f;
                }
                EffectUtil.Instance.LoadEffect(GameCenter.mainHero.uID, cSVEffectData.effects_path, GameCenter.mainHero.fxRoot.transform, EffectUtil.EEffectTag.Inquiry, cSVEffectData.fx_duration / 1000f, 1, 1, ELayerMask.Default, yOffset);
            }
        }

        protected override void OnInterrupt()
        {
            Sys_Inquiry.Instance.timer?.Cancel();
            Sys_Inquiry.Instance.timer = null;
            EffectUtil.Instance.UnloadEffectByTag(GameCenter.mainHero.uID, EffectUtil.EEffectTag.Inquiry);
            eventEmitter.Trigger(EEvents.InterrputInquiry);
        }

        protected override void OnCompleted()
        {
            eventEmitter.Trigger(EEvents.InquiryCompleted);
            GameCenter.mainHero.animationComponent.Play(CSVDetectData.EndAction, () =>
            {
                GameCenter.mainHero.animationComponent.Play((uint)EStateType.Idle);
            });
            CSVEffect.Data cSVEffectData = CSVEffect.Instance.GetConfData(CSVDetectData.EndFx);
            if (cSVEffectData != null)
            {
                float yOffset = 0f;
                if (GameCenter.mainHero.heroBaseComponent.TitleId != 0)
                {
                    yOffset = 0.95f;                 
                }
                if (GameCenter.mainHero.Mount != null)
                {
                    yOffset += GameCenter.mainHero.Mount.csvPetData.mountsignposition[1] / 1000f;
                }
                EffectUtil.Instance.LoadEffect(GameCenter.mainHero.uID, cSVEffectData.effects_path, GameCenter.mainHero.fxRoot.transform, EffectUtil.EEffectTag.Inquiry, cSVEffectData.fx_duration / 1000f, 1, 1, ELayerMask.Default, yOffset);
            }
        }

        public override bool IsCompleted()
        {
            return InquiryCompleted;
        }
    }
}
