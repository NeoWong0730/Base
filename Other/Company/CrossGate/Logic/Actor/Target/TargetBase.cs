using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 目标基类///
    /// </summary>
    public abstract class TargetBase
    {
        public string TargetName;

        public virtual void Init(List<int> data)
        { }

        /// <summary>
        /// 自动执行,人物系统使用///
        /// </summary>
        public virtual void AutoExecute()
        {

        }
    }
}
