using UnityEngine;

public class IniElement_Vector2Array : IniElement {
    public IniElement_Vector2Array() : base() { }
    public Vector2[] value { get; private set; }

    protected override void Parse() {
        string[] segments = this.csvString.Split('|');
        int halfLength = segments.Length / 2;
        Vector2[] ret = new Vector2[halfLength];
        for (int i = 0, length = segments.Length; i < length; i += 2) {
            int realIndex = i / 2;
            uint.TryParse(segments[i], out uint x);
            uint.TryParse(segments[i + 1], out uint y);
            ret[realIndex] = new Vector2(x, y);
        }
        // 缓存结果
        this.value = ret;
    }
}
