using UnityEngine;

public class IniElement_Vector3Array : IniElement {
    public IniElement_Vector3Array() : base() { }
    public Vector3[] value { get; private set; }

    protected override void Parse() {
        string[] segments = this.csvString.Split('|');
        int halfLength = segments.Length / 3;
        Vector3[] ret = new Vector3[halfLength];
        for (int i = 0, length = segments.Length; i < length; i += 3) {
            int realIndex = i / 3;
            uint.TryParse(segments[i], out uint x);
            uint.TryParse(segments[i + 1], out uint y);
            uint.TryParse(segments[i + 2], out uint z);
            ret[realIndex] = new Vector3(x, y, z);
        }
        // 缓存结果
        this.value = ret;
    }
}
