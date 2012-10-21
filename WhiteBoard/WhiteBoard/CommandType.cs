using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhiteBoard
{
    // remove this class when you refactor, better to use
    // object is Class to check, faster and cleaner
    public enum CommandType
    {
        Add,
        Edit,
        View,
        Delete,
        Search,
        Archive,
        Undo
    }
}
