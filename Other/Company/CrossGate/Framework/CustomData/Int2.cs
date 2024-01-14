using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public struct Int2 
    {
        public int X;
        public int Y;

        public Int2(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vector2 ToV2
        {
            get
            {
                return new Vector2(this.X, this.Y);
            }
            
        }

        public Vector3 ToV3
        {
            get
            {
                return new Vector3(this.X, 0f, this.Y);
            }
        }
    }
}


