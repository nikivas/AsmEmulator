using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApplication1;

namespace WindowsFormsApplication1.flags
{
    class FlagZF : FlagA
    {
        public override void setValue(int value)
        {
            this.value |= RegisterA.getNewCorrectValue(value) == 0;
        }
    }
}
