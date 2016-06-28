using System;

namespace EnjoyCQRS.IntegrationTests.Shared
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UICommandNameAttribute : Attribute
    {
        public string Name { get; }

        public UICommandNameAttribute(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            Name = name;
        }
    }
}