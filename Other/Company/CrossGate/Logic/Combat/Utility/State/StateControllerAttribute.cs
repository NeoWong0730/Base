using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class StateControllerAttribute : Attribute
{
    public int m_StateCategory;
    public string m_WorkStreamDataFile;

    public StateControllerAttribute(int stateCategory, string workStreamDataFile)
    {
        m_StateCategory = stateCategory;
        m_WorkStreamDataFile = workStreamDataFile;
    }
}
