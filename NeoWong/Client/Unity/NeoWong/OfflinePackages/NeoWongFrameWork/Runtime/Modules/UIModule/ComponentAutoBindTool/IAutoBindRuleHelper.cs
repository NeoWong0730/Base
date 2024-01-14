using System.Collections.Generic;
using UnityEngine;

namespace NWFramework
{
    /// <summary>
    /// 自动绑定规则辅助器接口
    /// </summary>
    public interface IAutoBindRuleHelper
    {
        /// <summary>
        /// 是否为有效绑定
        /// </summary>
        /// <param name="target">目标Transform</param>
        /// <param name="fileNames">fileNames</param>
        /// <param name="componentTypeNames">componentTypeNames</param>
        /// <returns>是否为有效绑定</returns>
        bool IsValidBind(Transform target, List<string> fileNames, List<string> componentTypeNames);
    }
}