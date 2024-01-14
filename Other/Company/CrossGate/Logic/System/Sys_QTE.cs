using Logic.Core;
using Table;
using Framework;
using Lib.Core;

namespace Logic
{
    public class Sys_QTE : SystemModuleBase<Sys_QTE>
    {
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents {
            OnClose,
        }
        public void OpenQTE(uint unlockId, System.Action onFinish, CutSceneArg arg, EQTESource source = EQTESource.Cutscene)
        {
            CSVUnlock.Data csv = CSVUnlock.Instance.GetConfData(unlockId);
            if (csv != null)
            {
                bool autoFinishWhenDontOperate = csv.CompletionTime < 30f;
                UIManager.OpenUI((int)csv.qteUIId, true, new QTEConfig(autoFinishWhenDontOperate, csv.CompletionTime, onFinish, arg, csv, source));

                Sys_Task.Instance.InterruptCurrentTaskDoing();
            }
        }
        public void CloseQTE(uint unlockId) {
            CSVUnlock.Data csv = CSVUnlock.Instance.GetConfData(unlockId);
            if (csv != null) {
                UIManager.CloseUI((EUIID)csv.qteUIId);
            }
        }
    }
}