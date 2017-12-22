using System;

namespace WindowsFormsApplication1.flags
{
    class FlagOF : FlagA
    {

        public override void setValue(int val)
        {
            this.value |= val > 0 ? (Math.Abs(val) > 127) : (Math.Abs(val) > 128);
        }



    }
}
