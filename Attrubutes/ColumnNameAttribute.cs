using System;

namespace Dippy.DDApi.Attributes
{
    /// <summary>
    /// Attribute used to define the name of the column that it corresponds to.
    /// <br>If this attribute is not applied, then the name of the column</br>
    /// <br>is assumed to be the same name as the property.</br>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnNameAttribute : Attribute
    {
        public string Name { get; }

        public ColumnNameAttribute(string name)
        {
            Name = name;
        }
    }
}
