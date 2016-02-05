using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace superGame
{
    public struct gameMove
    {
        public string xValue;
        public string yValue;
        public string commandName;
        public string commandMode;

        public gameMove(string xValue, string yValue, string commandName, string commandMode)
        {
            this.xValue = xValue;
            this.yValue = yValue;
            this.commandName = commandName;
            this.commandMode = commandMode;
        }

    }
}
