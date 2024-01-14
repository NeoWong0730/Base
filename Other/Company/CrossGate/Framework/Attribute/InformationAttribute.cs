using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InformationAttribute : PropertyAttribute
{
    public enum InformationType
    {
        None,
        Info,
        Warning,
        Error,
    }

#if UNITY_EDITOR

    public string Message;
    public MessageType Type;
    public bool MessageAfterProperty;

    public InformationAttribute(string message, InformationType type, bool messageAfterProperty)
    {
        this.Message = message;
        if (type == InformationType.Error) { this.Type = MessageType.Error; }
        if (type == InformationType.Info) { this.Type = MessageType.Info; }
        if (type == InformationType.Warning) { this.Type = MessageType.Warning; }
        if (type == InformationType.None) { this.Type = MessageType.None; }
    }

#endif
}



