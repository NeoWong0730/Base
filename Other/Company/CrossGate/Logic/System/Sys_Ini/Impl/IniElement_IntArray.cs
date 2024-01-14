using System.Collections.Generic;
using Logic.Core;
using Table;

// 逗号分隔的一维数组
public class IniElement_IntArray : IniElement
{
    public IniElement_IntArray() : base() { }
    public int[] value { get; private set;}
    
    protected override void Parse()
    {
        string[] segments = csvString.Split('|');
        int[] ret = new int[segments.Length];
        for(int i = 0; i < segments.Length; ++i)
        {
            int t;
            int.TryParse(segments[i], out t);
            ret[i] = t;
        }

        // 缓存结果
        value = ret;
    }
}
