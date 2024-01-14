using Table;
using Lib.Core;
using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 调查功能///
    /// </summary>
    public class InquiryFunction : FunctionBase
    {
        public CSVDetect.Data CSVDetectData
        {
            get;
            private set;
        }

        public override void Init()
        {
            CSVDetectData = CSVDetect.Instance.GetConfData(ID);         
        }

        protected override bool CanExecute(bool CheckVisual = true)
        {
            if (CSVDetectData == null)
            {
                DebugUtil.LogError($"CSVDetect.Data is Null, id: {ID}");
                return false;
            }
            return true;
        }

        protected override void OnExecute()
        {
            base.OnExecute();

            InquiryAction inquiryAction = ActionCtrl.Instance.CreateAction(typeof(InquiryAction)) as InquiryAction;
            if (inquiryAction != null)
            {
                inquiryAction.CSVDetectData = CSVDetectData;
                if (FunctionSourceType == EFunctionSourceType.Task)
                {
                    inquiryAction.taskId = HandlerID;
                }
                inquiryAction.Init();
                if (ActionCtrl.Instance.actionCtrlStatus == ActionCtrl.EActionCtrlStatus.PlayerCtrl)
                {
                    ActionCtrl.Instance.ExecutePlayerCtrlAction(inquiryAction);
                }
                else if (ActionCtrl.Instance.actionCtrlStatus == ActionCtrl.EActionCtrlStatus.Auto)
                {
                    List<ActionBase> actionBases = new List<ActionBase>();
                    actionBases.Add(inquiryAction);
                    ActionCtrl.Instance.AddAutoActions(actionBases);
                }
            }
        }

        public override bool IsValid()
        {
            return Sys_FunctionOpen.Instance.IsOpen(20501, false) && !Sys_Inquiry.Instance.IsInquiryed(ID) && base.IsValid();
        }

        protected override void OnDispose()
        {
            CSVDetectData = null;

            base.OnDispose();
        }
    }
}