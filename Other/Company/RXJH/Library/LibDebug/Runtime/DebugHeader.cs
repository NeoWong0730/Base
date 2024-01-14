using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum ELogOption
{
    None = 0,
    PrintConsole = 1,
    WriteFile = 2,
    AttachTime = 4,
}

public class DebugHeader
{
    public string name;
    public ELogOption option;
}