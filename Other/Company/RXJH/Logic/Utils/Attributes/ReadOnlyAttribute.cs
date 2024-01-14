using UnityEngine;

namespace Logic
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class ReadOnlyAttribute : PropertyAttribute
    {
        public enum DataType
        {
            Local,
            Server,
        }

        public DataType dataType;
        public readonly bool justInPlayMode;

        public ReadOnlyAttribute(DataType dataType = DataType.Local, bool justInPlayMode = true)
        {
            this.dataType = dataType;
            this.justInPlayMode = justInPlayMode;
        }
    }
}
