using System;

/// <summary>
/// Index必须从0开始递增+1     ;   
/// 支持class，IList<>, long, ulong, int, uint, short, ushort, float, string, bool
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public class FileDataOperationAttribute : Attribute
{
    public int DataType;
    public int Index;

    public FileDataOperationAttribute(int dataType, int index)
    {
        DataType = dataType;
        Index = index;
    }
}