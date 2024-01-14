using System.Collections.Generic;
using UnityEngine;

namespace NWFramework
{
    /// <summary>
    /// �Զ��󶨹��������ӿ�
    /// </summary>
    public interface IAutoBindRuleHelper
    {
        /// <summary>
        /// �Ƿ�Ϊ��Ч��
        /// </summary>
        /// <param name="target">Ŀ��Transform</param>
        /// <param name="fileNames">fileNames</param>
        /// <param name="componentTypeNames">componentTypeNames</param>
        /// <returns>�Ƿ�Ϊ��Ч��</returns>
        bool IsValidBind(Transform target, List<string> fileNames, List<string> componentTypeNames);
    }
}