using System;
using System.Collections.Generic;
using System.Text;

namespace FF3LE.Undo
{
    interface Command
    {
        bool AutoRedo();
        void Execute();
    }
}
