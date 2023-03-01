using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlloyDemo.Models
{
    [System.Flags]
    public enum Region
    {
        Unknown = 0,    // 0
        Europe = 10,     // 1
        UK = 20,         // 2
        USA = 30,        // 3
        RestOfWorld = 90
    }
}