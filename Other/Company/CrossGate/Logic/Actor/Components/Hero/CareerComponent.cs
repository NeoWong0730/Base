using System;

namespace Logic
{
    public interface ICareerComponent
    {
        CareerComponent GetCareerComponent();
    }

    public class CareerComponent : Logic.Core.Component
    {
        public ECareerType CurCarrerType
        {
            get;
            private set;
        }

        public Action<ECareerType, ECareerType> ChangeCareerAction;

        public void UpdateCareerType(ECareerType newCareerType)
        {
            if (CurCarrerType != newCareerType)
            {
                ECareerType oldCareerType = CurCarrerType;
                CurCarrerType = newCareerType;
                
                ChangeCareerAction?.Invoke(oldCareerType, newCareerType);
            }
        }
    }
}
