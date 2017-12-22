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
