using System.Collections.Generic;
using System.Text;
using Lib;

namespace Framework
{
    //TODO ： 低内存时 清理所有缓存
    public class StringBuilderPool
    {
        const int gMaxLength = 256;
        const int gLimitCount = 1;
        private static int requestCount = 0;
        private static Stack<StringBuilder> pool = new Stack<StringBuilder>(gLimitCount);

#if DEBUG_MODE
        public static string requestStack = string.Empty;
        public static bool fromILRuntime = false;
#endif

        public static StringBuilder GetTemporary()
        {
#if DEBUG_MODE && RECODE_STACK
            if (requestCount >= gLimitCount)
            {
                DebugUtil.LogErrorFormat("看到报错请把第一条截出来发群里 上次请求的未释放 {0} {1}", requestCount.ToString(), requestStack);
                //return null;
            }

            if (!fromILRuntime)
            {
                requestStack = System.Environment.StackTrace;
            }
            //重置下标识，下次ilruntime调度的时候再标记，仅仅作为一个临时标记
            fromILRuntime = false;
#endif

            ++requestCount;

            //DebugUtil.LogFormat(ELogType.eNone, "StringBuilder requestCount {0}", requestCount);

            StringBuilder ret = null;
            if (pool.Count > 0)
            {
                ret = pool.Pop();
                ret.Clear();
            }
            else
            {
                ret = new StringBuilder();
            }
            return ret;
        }
        public static void ReleaseTemporary(StringBuilder target)
        {
            if (target == null)
                return;

            if (requestCount < 1)
            {
#if DEBUG_MODE
                DebugUtil.LogErrorFormat("释放过多的StringBuilder 请检查逻辑是否可以优化!");
#endif
                return;
            }

            if (pool.Count < gLimitCount)
            {
                if (target.Length > gMaxLength)
                {
                    target.Length = gMaxLength;
                }
                pool.Push(target);                
            }
            --requestCount;
        }

        public static string ReleaseTemporaryAndToString(StringBuilder target)
        {
            if (target == null)
                return null;

            string rlt = target.ToString();

            if (requestCount < 1)
            {
#if DEBUG_MODE
                DebugUtil.LogErrorFormat("释放过多的StringBuilder 请检查逻辑是否可以优化!");
#endif
                return rlt;
            }

            if (pool.Count < gLimitCount)
            {
                if (target.Length > gMaxLength)
                {
                    target.Length = gMaxLength;
                }
                pool.Push(target);                
            }
            --requestCount;

            return rlt;
        }
    }
}
