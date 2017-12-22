using System;

namespace WindowsFormsApplication1.flags
{
    public class FlagCF : FlagA
    {
        public override void setValue(int val)
        {
            this.value |= val > 0 ? (Math.Abs(val) > 127) : (Math.Abs(val) > 128);
        }
    }
}
