using System.Collections.Generic;
using Logic.Core;
using Table;

public class IniElement_Int : IniElement
{
    public IniElement_Int() : base() { }
    public int value { get; private set; }
    
    protected override void Parse()
    {
        int.TryParse(csvString, out int ret);
        
        // 缓存结果
        value = ret;
    }
}
