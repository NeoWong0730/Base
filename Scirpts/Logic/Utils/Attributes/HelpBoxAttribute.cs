using UnityEngine;
using System;

namespace Logic
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class HelpBoxAttribute : PropertyAttribute
    {
        public string text;
        public HelpBoxAttribute(string text, MessageType messageType = MessageType.None) { this.text = text; this.messageType = messageType; }
        public int lineSpace;

        public enum MessageType
        {
            None,
            Info,
            Warning
        }

        public MessageType messageType;
    }
}