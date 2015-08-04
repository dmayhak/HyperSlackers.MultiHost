using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperSlackers.AspNet.Identity.EntityFramework
{
    /// <summary> Attribute to tell EF to save null strings as string.Empty instead of null. </summary>
    ///
    /// <seealso cref="T:System.ComponentModel.DataAnnotations.DisplayFormatAttribute"/>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ConvertNullToEmptyStringAttribute : DisplayFormatAttribute
    {
        /// <summary> Initializes a new instance of the ConvertNullToEmptyStringAttribute class. </summary>
        public ConvertNullToEmptyStringAttribute()
        {
            ConvertEmptyStringToNull = false;
        }
    }
}
