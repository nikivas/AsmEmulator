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
            setValue(isCorrect((this.value+ int.Parse(flags["CF"].textBoxF.Text)) + value));
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
            int result = (this.value - int.Parse(flags["CF"].textBoxF.Text)) - value;
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
            var el = new Register(-1); // his mask is b_1:111_1111
            setValue(isCorrect(this.getValue() ^ el.getValue()));  // equals XOR  0XFF but they do not working
        }

        public void SHR(ref Register reg) 
        {
            var count = reg.getValue();
            flags["CF"].value = this.getValue() >> (count - 1) % 2 == 1;
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
            if(this.getValue() < 0)
            {
                var a = new Register(-128);
                this.XOR(ref a);
                setValue(isCorrect(this.getValue() << count));
                this.XOR(ref a);
            }
            else
            {
                setValue(isCorrect(this.getValue() << count));
            }
            
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

        //**************THIRD LAB****************//

        public void NEG()
        {
            var el = new Register(-128);// his mask is b_1:000_0000
            setValue(isCorrect(0-this.getValue()));
        }

        public void MOV(ref Register reg)
        {
            setValue(isCorrect(reg.getValue()));
        }

        public void CMP(ref Register reg)
        {
            Register nReg = new Register(this.getValue() - reg.getValue());

            if (nReg.getBinStringByValue()[0] == '1')
                flags["CF"].value = true;
            else if (nReg.getBinStringByValue().Where(x => x == '0').Count() == REGISTER_SIZE)
                flags["ZF"].value = true;

            if (this.getBinStringByValue()[0] == '1')
                flags["SF"].value = true;

            isCorrect(INCEREMENT_VALUE);
        }

        public void SWAP()
        {
            var reg = new Register(4);
            this.ROR(ref reg );
        }

        public void TEST(ref Register reg)
        {
            if(reg.getValue() == this.getValue())
            {
                flags["ZF"].value = true;
            }
            else
            {
                flags["SF"].value = true;
            }
            isCorrect(INCEREMENT_VALUE);
        }

        public void MOVS(ref Register reg)
        {
            var a = frm.getRegisterFromDictionary("CH");
            if (a.getValue() <= 0) return;

            var nextEl = frm.getNextMemorybyName(this.name, isBottom: flags["DF"].textBoxF.Text == "1");
            if (reg.ifRegisterName() )
            {
                a.setValue(a.isCorrect(a.getValue()-1));
                this.setValue(isCorrect(reg.getValue()));
                nextEl.MOVS(ref reg);
            }
            else
            {
                a.setValue(a.isCorrect(a.getValue() - 1));
                this.setValue(isCorrect(reg.getValue()));
                var nextRef = frm.getNextMemorybyName(reg.name);
                nextEl.MOVS(ref nextRef);
            }
        }

    }
}
