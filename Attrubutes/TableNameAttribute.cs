using System;

namespace Dippy.DDApi.Attributes
{
    /// <summary>
    /// Attribute used to define the name of the table that it corresponds to.
    /// <br>If this attribute is not applied, then the name of the table</br>
    /// <br>is assumed to be the same name as the class.</br>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute : Attribute
    {
        public string Name { get; }

        public TableNameAttribute(string name)
        {
            Name = name;
        }
    }
}
