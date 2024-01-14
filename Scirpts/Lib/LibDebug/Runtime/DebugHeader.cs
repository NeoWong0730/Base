using System;

[Flags]
public enum ELogOption
{
    None,
    PrintConsole = 1,
    WriteFile = 2,
    AttachTime = 4,
}

public class DebugHeader
{
    public string name;
    public ELogOption option;
}
