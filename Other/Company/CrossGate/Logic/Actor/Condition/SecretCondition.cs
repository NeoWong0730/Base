using System.Collections.Generic;

namespace Logic
{
    public class SecretCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.Secret;
            }
        }

        uint secretID;

        public override void DeserializeObject(List<int> data)
        {
            secretID = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (Sys_SecretMessage.Instance.completedSecretMessageID.Contains(secretID))
            {
                return true;
            }
            return false;
        }

        protected override void OnDispose()
        {
            secretID = 0;
        }
    }

    public class NoSecretCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.NoSecret;
            }
        }

        uint secretID;

        public override void DeserializeObject(List<int> data)
        {
            secretID = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (!Sys_SecretMessage.Instance.completedSecretMessageID.Contains(secretID))
            {
                return true;
            }
            return false;
        }

        protected override void OnDispose()
        {
            secretID = 0;
        }
    }
}
