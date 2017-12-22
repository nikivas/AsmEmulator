using System;
using WindowsFormsApplication1;
namespace WindowsFormsApplication1.flags
{
    class FlagSF : FlagA
    {
        public override void setValue(int value)
        {
            this.value |= RegisterA.getNewCorrectValue(value) < 0;
        }
    }
}
