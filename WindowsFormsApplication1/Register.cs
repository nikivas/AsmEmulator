using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public class Register
    {
        private sbyte value;

        public TextBox textBoxOutput_Dec { get; set; }
        public TextBox textBoxOutput_Bin { get; set; }

        public TextBox flagCF { get; set; }
        public TextBox flagZF { get; set; }
        public TextBox flagSF { get; set; }
        public TextBox flagOF { get; set; }

        //bool CF, bool ZF, bool SF, bool OF
        bool CF = false;
        bool ZF = false;
        bool SF = false;
        bool OF = false;

        public Form1 frm { get; set; }


        public const int BIN_SYSTEM = 2;
        public const int DEC_SYSTEM = 10;
        public const int INCEREMENT_VALUE = 1;
        public const sbyte ZNAK_BYTE = -128;

        public const sbyte REGISTER_SIZE = 8;
        

        public sbyte getValue()
        {
            return value;
        }

        public void setValue(sbyte value)
        {
            this.value = value;
            updateTextBox();
        }

        public Register()
        {
            setValue(0);
        }

        public Register(sbyte value)
        {
            setValue(value);
        }

        public Register(int value)
        {
            setValue(isCorrect(value));
        }

        public Register(sbyte value, TextBox textBoxOutput_Bin, TextBox textBoxOutput_Dec)
        {
            setValue(value);
            this.textBoxOutput_Bin = textBoxOutput_Bin;
            this.textBoxOutput_Dec = textBoxOutput_Dec;
        }

        public string getBinStringByValue()
        {
            return Register.getBinByValue(this.value);
        }

        public static string getBinByValue(sbyte val)
        {
            string str = Convert.ToString(val, BIN_SYSTEM);
            if (str.Length > 8)
                str = str.Substring(str.Length - 8, 8);
            while (str.Length < 8)
            {
                str = "0" + str;
            }
            return str;
        }

        public int updateTextBox()
        {
            int result = 0;
            if (textBoxOutput_Bin != null) { 
                textBoxOutput_Bin.Text = getBinStringByValue();
            }
            else
                result = -1;

            if (textBoxOutput_Dec != null) { 
                textBoxOutput_Dec.Text = Convert.ToString(this.value, DEC_SYSTEM);
            }
            else
                result = -2;

            return result;
        }

        public sbyte isCorrect(int value)
        {
            
            sbyte result = value > 0? (sbyte)(value % 128) : (sbyte)(value % 129);
            ZF |= (result == 0);
            SF |= (result < 0);
            if (value > 0)
            {
                CF |= (Math.Abs(value) > 127);
                OF |= (Math.Abs(value) > 127);
            }
            else
            {
                CF |= (Math.Abs(value) > 128);
                OF |= (Math.Abs(value) > 128);
            }

            setFlag();

            CF = false;
            ZF = false;
            SF = false;
            OF = false;

            return result;
        }

        public void ADD(ref Register reg) //сложение без переполнения(переноса)
        {
            
            var value = reg.getValue();
            setValue( isCorrect(this.value + value) );
        }

        public void ADC(ref Register reg) //сложение ???
        {
            var value = reg.getValue();
            setValue(isCorrect((this.value+ int.Parse(flagCF.Text)) + value));
        }

        public void MUL(ref Register reg)
        {
            var value = reg.getValue();
            setValue(isCorrect(this.value * value));
        }

        public void SUB(ref Register reg) //вычитание без переполнения(переноса)
        {
            var value = reg.getValue();
            setValue(isCorrect(this.value - value));
        }

        public void SUBB(ref Register reg) //вычитание ???
        {

            var value = reg.getValue();
            int result = (this.value - int.Parse(flagCF.Text)) - value;
            setValue(isCorrect(result));
        }

        public void DIV(ref Register reg)
        {
            var value = reg.getValue();
            if (this.frm != null)
            {
                var a = frm.getRegisterFromDictionary("AH");
                a.setValue(a.isCorrect(this.value / value));
            }
            setValue(isCorrect(this.value % value));
        }

        public void INC()
        {
            setValue(isCorrect(this.value + INCEREMENT_VALUE));
        }

        public void AND(ref Register reg)
        {
            var count = reg.getValue();
            setValue(isCorrect(this.getValue() & count));
        }

        public void OR(ref Register reg)
        {
            var count = reg.getValue();
            setValue(isCorrect(this.getValue() | count));
        }

        public void XOR(ref Register reg)
        {
            var count = reg.getValue();
            setValue(isCorrect(this.getValue() ^ count));
        }

        public void NOT()
        {
            setValue(isCorrect(this.getValue() ^ 0xFF));
        }

        public void SHR(ref Register reg) // ostorojno ! Govnocode i matesha
        {
            var count = reg.getValue();
            CF = this.getValue() >> (count - 1) % 2 == 1;
            var res = 0;
            res = this.getValue() >> (count);
            setValue(isCorrect(res));
            if (getValue() < 0) {
                this.value ^= ZNAK_BYTE;
                setValue(isCorrect(getValue()));
            }
        }

        public void SHL(ref Register reg)
        {
            var count = reg.getValue();
            setValue(isCorrect(this.getValue() << count));
        }

        public void getMoveR( ref string binaryString, int count)
        {
            while (count > 0)
            {
                if (count == 1)
                {
                    CF = binaryString[binaryString.Length - 1] == '1';
                }
                binaryString = binaryString[binaryString.Length - 1].ToString() +
                               binaryString.Substring(0, binaryString.Length - 1);
                count--;
            }
        }

        public void ROR(ref Register reg)
        {
            var count = reg.getValue();
            var binaryString = getBinStringByValue();

            getMoveR(ref binaryString, count);

            this.value = Convert.ToSByte(binaryString, 2);
            setValue(isCorrect(this.value));
        }


        public void ROL(ref Register reg) // 
        {
            var count = reg.getValue();
            string binaryString = getBinStringByValue();
            while (count > 0)
            {
                if(count == 1)
                {
                    CF = binaryString[0] == '1';
                }
                binaryString = binaryString.Substring(1, binaryString.Length - 1)+binaryString[0].ToString();
                count--;
            }
            this.value = Convert.ToSByte(binaryString, 2);
            setValue(isCorrect(this.getValue()));
        }

        public void RRC(ref Register reg) // dodelay, daun
        {


        }

        public void RLC(ref Register reg)
        {

        }

        private void setFlag()
        {
            if(flagCF != null)
                flagCF.Text = CF ? "1" : "0";

            if (flagZF != null)
                flagZF.Text = ZF ? "1" : "0";

            if (flagSF != null)
                flagSF.Text = SF ? "1" : "0";

            if (flagOF != null)
                flagOF.Text = OF ? "1" : "0";

        }
    }
}
