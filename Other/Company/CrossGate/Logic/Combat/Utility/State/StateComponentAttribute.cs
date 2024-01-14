using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class StateComponentAttribute : Attribute
{
    public int m_StateCategory;

    //状态分层也必须使用同一个枚举定义的state，也就是一个StateController只能有一个DefineState枚举
    public int m_DefineState;

    public StateComponentAttribute(int stateCategory, int defineState)
    {
        m_StateCategory = stateCategory;
        m_DefineState = defineState;
    }
}
