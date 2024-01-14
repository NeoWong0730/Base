using System.Collections.Generic;
using Logic.Core;
using Table;

public class IniElement_Float : IniElement {
    public IniElement_Float() : base() {
    }

    public float value { get; private set; }

    protected override void Parse() {
        float.TryParse(csvString, out float ret);

        // 缓存结果
        value = ret;
    }
}