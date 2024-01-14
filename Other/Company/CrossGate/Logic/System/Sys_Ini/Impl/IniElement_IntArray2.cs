using System.Collections.Generic;
using Logic.Core;
using Table;

// 高维是|分隔，低纬是逗号
public class IniElement_IntArray2 : IniElement
{
    public IniElement_IntArray2() : base() { }
    public List<List<int>> value { get; private set;}

    protected override void Parse()
    {
        string[] segments = csvString.Split('|');
        List<List<int>> ret = new List<List<int>>(segments.Length);
        for(int i = 0; i < segments.Length; ++i)
        {
            string[] selections = segments[i].Split(',');
            List<int> inner = new List<int>(selections.Length);
            for (int j = 0; j < selections.Length; ++j)
            {
                int t;
                int.TryParse(selections[j], out t);
                inner.Add(t);
            }
            ret.Add(inner);
        }
        
        // 缓存结果
        value = ret;
    }
}
