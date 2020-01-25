using System;

namespace Dippy.DDApi.Attributes
{
    /// <summary>
    /// <para>Attribute for columns that shouldn't be written to and are only read.</para>
    /// <br>This is used for columns that are computed by the database,</br>
    /// <br>including autoincrement keys, timpstamps and other forms of auto-logs.</br>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ReadOnlyColumnAttribute : Attribute
    {
    }
}
