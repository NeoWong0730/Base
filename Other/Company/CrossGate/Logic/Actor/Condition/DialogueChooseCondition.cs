using System.Collections.Generic;

namespace Logic
{
    public class DialogueChooseCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.DialogueChoose;
            }
        }

        uint DialogueChooseID;

        public override void DeserializeObject(List<int> data)
        {
            DialogueChooseID = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (DialogueChooseID == 0)
            {
                return true;
            }
            return false;
        }

        protected override void OnDispose()
        {
            DialogueChooseID = 0;
        }
    }
}
