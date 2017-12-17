using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public class Register : RegisterA
    {

        #region<constructors>

        public Register() : base() { }

        public Register(sbyte value) :base(value) { }

        public Register(int value) : base(value) { }

        public Register(sbyte value, TextBox textBoxOutput_Bin, TextBox textBoxOutput_Dec) 
            : base(value, textBoxOutput_Bin, textBoxOutput_Dec) { }


        #endregion


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
        } // деление

        public void INC()
        {
            setValue(isCorrect(this.value + INCEREMENT_VALUE));
        } // сложение с 1


        //********************* SECOND LAB ***********************//
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

        public void SHR(ref Register reg) 
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

        public void ROR(ref Register reg)
        {
            var count = reg.getValue();
            _bitShifterOverride(count, isRight: true, isNeedCF: false);
        }

        public void ROL(ref Register reg)  
        {
            var count = reg.getValue();
            _bitShifterOverride(count , isRight: false, isNeedCF: false);
        }

        public void RRC(ref Register reg) 
        {
            int count = reg.getValue();
            _bitShifterOverride(count, isRight: true, isNeedCF: true);
        }

        public void RLC(ref Register reg)
        {
            int count = reg.getValue();
            _bitShifterOverride(count, isRight: false, isNeedCF: true);
        }

    }
}
