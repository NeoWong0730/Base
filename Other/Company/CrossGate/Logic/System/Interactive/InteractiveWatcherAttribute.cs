using System;

namespace Logic
{
    public class InteractiveWatcherAttribute : Attribute
    {
        public EInteractiveAimType InteractiveAimType { get; }

        public InteractiveWatcherAttribute(EInteractiveAimType type)
        {
            InteractiveAimType = type;
        }
    }
}
