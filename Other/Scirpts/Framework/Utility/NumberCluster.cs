using UnityEngine;

namespace Framework
{
    // 做服务器列表的时候，需要倒着展示，每个页签展示GAP个
    public class NumberCluster
    {
        private readonly int GAP = 10;
        private readonly int maxCount = 21;

        public NumberCluster(int GAP, int maxCount)
        {
            this.GAP = GAP;
            this.maxCount = maxCount;
        }

        public bool TryGetArea(int clusterIndex, out int begin, out int end, bool invert = false)
        {
            begin = 0;
            end = 0;
            int totalClusterCount = (int)Mathf.Floor(1f * this.maxCount / this.GAP) + 1;
            if (clusterIndex < 0 || clusterIndex > totalClusterCount - 1)
            {
                return false;
            }

            if (invert)
            {
                clusterIndex = totalClusterCount - 1 - clusterIndex;
            }

            begin = clusterIndex * this.GAP;
            end = begin + this.GAP - 1;
            if (end >= this.maxCount)
            {
                end = this.maxCount - 1;
            }

            return true;
        }
    }
}