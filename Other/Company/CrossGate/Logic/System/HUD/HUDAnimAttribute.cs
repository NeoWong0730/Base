using System;

namespace Logic
{
    public class HUDAnimAttribute : Attribute
    {
        public AnimType animType;
        public HUDAnimAttribute(AnimType _animType)
        {
            animType = _animType;
        }
    }
}


