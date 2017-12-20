using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public class RegisterA
    {
        public sbyte value;

        public TextBox textBoxOutput_Dec { get; set; }
        public TextBox textBoxOutput_Bin { get; set; }

        public TextBox flagCF { get; set; }
        public TextBox flagZF { get; set; }
        public TextBox flagSF { get; set; }
        public TextBox flagOF { get; set; }

        //bool CF, bool ZF, bool SF, bool OF
        public bool CF = false;
        public bool ZF = false;
        public bool SF = false;
        public bool OF = false;

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


        #region<constructors>

        public RegisterA()
        {
            setValue(0);
        }

        public RegisterA(sbyte value)
        {
            setValue(value);
        }

        public RegisterA(int value)
        {
            setValue(isCorrect(value));
        }

        public RegisterA(sbyte value, TextBox textBoxOutput_Bin, TextBox textBoxOutput_Dec)
        {
            setValue(value);
            this.textBoxOutput_Bin = textBoxOutput_Bin;
            this.textBoxOutput_Dec = textBoxOutput_Dec;
        }

        #endregion

        public string getBinStringByValue()
        {
            return RegisterA._getBinByValue(this.value);
        }

        protected static string _getBinByValue(sbyte val)
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

        protected sbyte _getSByteByBinString(string value)
        {
            return Convert.ToSByte(value, BIN_SYSTEM);
        }

        protected void _bitShifterOverride(int count, bool isRight = true, bool isNeedCF = false)
        {
            string binaryString = getBinStringByValue();
            int binStrLast = binaryString.Length;
            if (isNeedCF)
            {
                binaryString += flagCF.Text;
            }

            _getMoveRL(ref binaryString, count, isRight: isRight, isNeedCF: false);

            if (isNeedCF)
            {
                CF = binaryString[binStrLast] == '1';
            }
            this.value = _getSByteByBinString(binaryString.Substring(0, binStrLast));
            setValue(isCorrect(this.getValue(), isSF: false));
        }

        protected void _getMoveRL(ref string binaryString, int count, bool isRight = true, bool isNeedCF = true)
        {
            while (count > 0)
            {
                if (count == 1 && isNeedCF)
                {
                    CF = binaryString[binaryString.Length - 1] == '1';
                }
                if (isRight)
                {
                    binaryString = binaryString[binaryString.Length - 1].ToString() +
                                   binaryString.Substring(0, binaryString.Length - 1);
                }
                else
                {
                    binaryString = binaryString.Substring(1, binaryString.Length - 1) +
                                   binaryString[0].ToString();
                }
                count--;
            }
        }

        public int updateTextBox()
        {
            int result = 0;
            if (textBoxOutput_Bin != null)
            {
                textBoxOutput_Bin.Text = getBinStringByValue();
            }
            else
                result = -1;

            if (textBoxOutput_Dec != null)
            {
                textBoxOutput_Dec.Text = Convert.ToString(this.value, DEC_SYSTEM);
            }
            else
                result = -2;

            return result;
        }

        protected sbyte _correctValue(int value)
        {
            return value > 0 ? (sbyte)(value % 128) : (sbyte)(value % 129);
        }

        public sbyte isCorrect( int value, bool isZF = true, 
                                bool isSF = true, bool isCF = true,
                                bool isOF = true)
        {
            sbyte result = _correctValue(value);

            if (isZF)
                ZF |= (result == 0);

            if(isSF)
                SF |= (result < 0);

            if (isCF)
                CF |= value > 0 ? (Math.Abs(value) > 127) : (Math.Abs(value) > 128);
            if(isOF)
                OF |= value > 0 ? (Math.Abs(value) > 127) : (Math.Abs(value) > 128);

            setFlag();

            CF = false;
            ZF = false;
            SF = false;
            OF = false;

            return result;
        }

        protected void setFlag()
        {
            if (flagCF != null)
                flagCF.Text = CF ? "1" : "0";

            if (flagZF != null)
                flagZF.Text = ZF ? "1" : "0";

            if (flagSF != null)
                flagSF.Text = SF ? "1" : "0";

            if (flagOF != null)
                flagOF.Text = OF ? "1" : "0";

        }

        public void updateFlagsWithText()
        {
            if(flagCF != null)
                CF = flagCF.Text == "1";
            if (flagOF != null)
                CF = flagOF.Text == "1";
            if (flagSF != null)
                CF = flagSF.Text == "1";
            if (flagZF != null)
                CF = flagZF.Text == "1";
        }
    }
}
