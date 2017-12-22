using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApplication1.flags;

namespace WindowsFormsApplication1
{
    public class RegisterA
    {
        public string name { get; set; }
        public sbyte value;

        public TextBox textBoxOutput_Dec { get; set; }
        public TextBox textBoxOutput_Bin { get; set; }

        public Form1 frm { get; set; }


        public const int BIN_SYSTEM = 2;
        public const int DEC_SYSTEM = 10;
        public const int INCEREMENT_VALUE = 1;
        public const sbyte ZNAK_BYTE = -128;

        public static readonly int MAX_VAL = 127;
        public static readonly int MIN_VAL = -128;

        public const sbyte REGISTER_SIZE = 8;


        public Dictionary<string,FlagA> flags = new Dictionary<string, FlagA>();

        public sbyte getValue()
        {
            return value;
        }

        public void setValue(sbyte value)
        {
            this.value = value;
            updateTextBox();
        }

        protected void flags_INIT()
        {
            flags.Add("CF", new FlagCF());
            flags.Add("OF", new FlagOF());
            flags.Add("SF", new FlagSF());
            flags.Add("ZF", new FlagZF());
            flags.Add("DF", new FlagDF());
        }

        #region<constructors>

        public RegisterA()
        {
            flags_INIT();
            setValue(0);
        }

        public RegisterA(sbyte value)
        {
            flags_INIT();
            setValue(value);
        }

        public RegisterA(int value)
        {
            flags_INIT();
            setValue(isCorrect(value));
        }

        public RegisterA(sbyte value, TextBox textBoxOutput_Bin, TextBox textBoxOutput_Dec)
        {
            flags_INIT();
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
                binaryString += flags["CF"].textBoxF.Text;
            }

            _getMoveRL(ref binaryString, count, isRight: isRight, isNeedCF: false);

            if (isNeedCF)
            {
                flags["CF"].value = binaryString[binStrLast] == '1';
            }
            this.value = _getSByteByBinString(binaryString.Substring(0, binStrLast));
            setValue(isCorrect(this.getValue(), notNeed: new List<string> {"SF" }));
        }

        protected void _getMoveRL(ref string binaryString, int count, bool isRight = true, bool isNeedCF = true)
        {
            while (count > 0)
            {
                if (count == 1 && isNeedCF)
                {
                    flags["CF"].value = binaryString[binaryString.Length - 1] == '1';
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

        public static sbyte getNewCorrectValue(int value)
        {
            return value > 0 ? (sbyte)(value % 128) : (sbyte)(value % 129);
        }

        public sbyte isCorrect( int value, List<string> notNeed = null)
        {
            sbyte result = getNewCorrectValue(value);

            foreach (var el in flags)
            {
                if(notNeed != null)
                    if (notNeed.Contains(el.Key))
                        continue;
                if (el.Value.textBoxF != null)
                    el.Value.setValue(value);
            }

            return result;
        }

        public bool ifRegisterName()
        {
            return name[1] < '0' || name[1] > '9';
        }
    }
}
