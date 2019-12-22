using System;
using System.Collections.Generic;
using System.Text;

namespace TimeTBot
{
    interface IHandler
    {
        string Execute(string id, string message, string commandName = null);
    }
}
