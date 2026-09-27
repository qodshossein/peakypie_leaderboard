using System;

namespace _Template.Core.SaveSystem.Data
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SaveDataAttribute : Attribute
    {
        public string Key { get; }

        public SaveTarget Target { get; }

        public SaveDataAttribute(string key, SaveTarget target)
        {
            Key = key;
            Target = target;
        }
    }
}