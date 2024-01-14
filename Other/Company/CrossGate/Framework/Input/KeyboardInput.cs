using UnityEngine;
using System;

namespace Framework
{
    public enum JoystickState
    {
        Start,
        Move,
        End,
    }

    public struct JoystickData
    {        
        public float dis;
        public JoystickState state;
        public float dirX;
        public float dirY;

        public Vector2 dir
        {
            set
            {
                dirX = value.x;
                dirY = value.y;
            }

            get
            {
                return new Vector2(dirX, dirY);
            }
        }
        //public float dis { set; get; }
        //public JoystickState state { set; get; }

        public bool Equals(JoystickData other)
        {
            return dir.Equals(other.dir) && dis.Equals(other.dis);
        }
    }

    public class KeyboardInput : MonoBehaviour
    {
        static string sMouseScrollWheel = "Mouse ScrollWheel";

        Vector2 mousePosition;

        JoystickData mJoystickData_L = new JoystickData();
        JoystickData mJoystickData_R = new JoystickData();

        public Action<JoystickData> SetLeftJoystick;
        public Action<JoystickData> SetRightJoystick;
        public Action<KeyCode> SendInput;
        public Action<float> SendScale;
        public Action SendHotKey;

        float lh = 0;
        float lv = 0;

        void Update()
        {            
            //lh = 0;
            //lv = 0;

            //if (Input.GetKey(KeyCode.W))
            //{
            //    lv += 1;
            //}
            //if (Input.GetKey(KeyCode.S))
            //{
            //    lv -= 1;
            //}

            //if (Input.GetKey(KeyCode.A))
            //{
            //    lh -= 1;
            //}
            //if (Input.GetKey(KeyCode.D))
            //{
            //    lh += 1;
            //}

            float rh = 0;
            float rv = 0;

            if (Input.GetMouseButtonDown(1))
            {
                mousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(1))
            {
                rh = Input.mousePosition.x - mousePosition.x;
                rv = Input.mousePosition.y - mousePosition.y;
                mousePosition = Input.mousePosition;
            }
            else
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    rv += 1;
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    rv -= 1;
                }

                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    rh -= 1;
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    rh += 1;
                }
            }


            Vector2 dir_L = new Vector2(lh, lv);
            float dis_L = 0f;
            if (!dir_L.Equals(Vector2.zero))
            {
                dir_L.Normalize();
                dis_L = 1f;
            }
            JoystickData joystickData_L = new JoystickData();
            joystickData_L.dir = dir_L;
            joystickData_L.dis = dis_L;
            if (!joystickData_L.Equals(mJoystickData_L))
            {
                SetLeftJoystick?.Invoke(joystickData_L);
                mJoystickData_L = joystickData_L;
            }


            Vector2 dir_R = new Vector2(rh, rv);
            float dis_R = 0f;
            if (!dir_R.Equals(Vector2.zero))
            {
                dis_R = dir_R.magnitude;
                dir_R.Normalize();
            }
            JoystickData joystickData_R = new JoystickData();
            joystickData_R.dir = dir_R;
            joystickData_R.dis = dis_R;
            if(!joystickData_R.Equals(mJoystickData_R))
            {
                SetRightJoystick?.Invoke(joystickData_R);
                mJoystickData_R = joystickData_R;
            }            



            if (Input.GetKeyDown(KeyCode.Q))
            {
                SendInput?.Invoke(KeyCode.Q);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                SendInput?.Invoke(KeyCode.E);
            }
            

            float fScale = Input.GetAxis(sMouseScrollWheel);
            if (fScale != 0)
            {
                SendScale?.Invoke(-fScale);
            }
        }

        public void KeyMove(float lh,float lv)
        {
            this.lh = lh;
            this.lv = lv;
        }

#if UNITY_STANDALONE_WIN && (OPEN_PC_KEYCODE_FUN || !UNITY_EDITOR)
        private void OnGUI()
        {
            SendHotKey?.Invoke();
        }
#endif
    }
}