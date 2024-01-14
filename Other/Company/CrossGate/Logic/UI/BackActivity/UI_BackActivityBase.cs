using Logic.Core;
using Packet;
using UnityEngine.Playables;

namespace Logic
{
    public class UI_BackActivityBase : UIComponent
    {

        public virtual void SetOpenValue(uint openValue)
        {

        }

        /// <summary>
        /// 检测功能是否开启 
        /// </summary>
        public virtual bool CheckFunctionIsOpen()
        {
            return true;
        }
        /// <summary>
        /// 检测红点是否显示
        /// </summary>
        public virtual bool CheckTabRedPoint()
        {
            return false;
        }
    }
}
