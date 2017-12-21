using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        const int MEM_COUNT = 16;
        const int REG_COUNT = 8;

        private int currentCommandIndex = 0;

        Dictionary<string, Register> regDic = new Dictionary<string, Register>();
        Dictionary<string, Register> memDic = new Dictionary<string, Register>();

        List<string> commandQueue = new List<string>();

        public Register getNextMemorybyName(string mem, bool isBottom = true)
        {
            List<string> listNames = new List<string>();
                listNames.AddRange((from el in memDic.Keys
                     select el).OrderBy(x => int.Parse(x.Substring(1))).ToArray());

            int idx;
            if(isBottom)
                idx = (listNames.IndexOf(mem) + 1) % listNames.Count;
            else
            {
                idx = listNames.IndexOf(mem);
                idx = idx == 0 ? listNames.Count - 1 : idx - 1;
            }
                
           
            return memDic[listNames[idx]];
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var regTextBoxCollection = this.groupBox2.Controls;

            bool isFillOk = true;
            isFillOk &= fillDic(this.groupBox2.Controls, ref regDic);
            isFillOk &= fillDic(this.groupBox3.Controls, ref memDic);

            if (!isFillOk)
            {
                MessageBox.Show("Ошибка инициализации программы");
            }

            textBoxSF_Flag.Leave += new EventHandler(this.flagChanged);
            textBoxCF_Flag.Leave += new EventHandler(this.flagChanged);
            textBoxOF_Flag.Leave += new EventHandler(this.flagChanged);
            textBoxZF_Flag.Leave += new EventHandler(this.flagChanged);

            getNextMemorybyName("A12", isBottom: false);
        }

        public void flagChanged(object sender, EventArgs e)
        {
            //foreach (var el in regDic)
            //{
            //    el.Value.updateFlagsWithText();
            //}
        }
        
        private bool fillDic(Control.ControlCollection collection, ref Dictionary<string,Register> dictionary)
        {
            try
            {
                foreach (Control el in collection)
                {
                    if (!(el is TextBox))
                        continue;
                    
                    string registerName = el.Name.Substring(7, 3).ToUpper();
                    if (registerName[2] == '_')
                        registerName = registerName.Substring(0, 2);

                    Register registerCounter = null;

                    if (!dictionary.ContainsKey(registerName))
                    {
                        registerCounter = new Register() { frm = this, name = registerName };
                        dictionary.Add(registerName, registerCounter);
                    }
                    
                    dictionary[registerName].flagCF = textBoxCF_Flag;
                    dictionary[registerName].flagOF = textBoxOF_Flag;
                    dictionary[registerName].flagSF = textBoxSF_Flag;
                    dictionary[registerName].flagZF = textBoxZF_Flag;
                    dictionary[registerName].flagDF = textBoxDF_Flag;

                    if (el.Name.Substring(10 + registerName.Length -2 , 1).ToUpper().Equals("B"))
                    {
                        dictionary[registerName].textBoxOutput_Bin = (TextBox)el;
                        dictionary[registerName].textBoxOutput_Bin.Text = "00000000";
                        dictionary[registerName].textBoxOutput_Bin.Leave += new EventHandler(anyTextBoxChangedBin);
                    }
                    else
                    {
                        dictionary[registerName].textBoxOutput_Dec = (TextBox)el;
                        dictionary[registerName].textBoxOutput_Dec.Text = "0";
                        dictionary[registerName].textBoxOutput_Dec.TextChanged += new EventHandler(anyTextBoxChangedDec);
                    }
                }
                return true;
            }catch (Exception e1)
            {
                return false;
            }
        }

        private void anyTextBoxChangedBin(object sender, EventArgs e)
        {
            var el = (TextBox)sender;
            string registerName = el.Name.Substring(7, 3).ToUpper();
            if (registerName[2] == '_')
                registerName = registerName.Substring(0, 2);
            var obj = getRegisterFromDictionary(registerName);
            try
            {
                var number = Convert.ToInt32(el.Text, 2);
                sbyte result = number > 0 ? (sbyte)(number % 128) : (sbyte)(number % 129);
                obj.setValue(result);
            }
            catch (Exception e11)
            { }


        }

        private void anyTextBoxChangedDec(object sender, EventArgs e)
        {
            var el = (TextBox)sender;
            string registerName = el.Name.Substring(7, 3).ToUpper();
            if (registerName[2] == '_')
                registerName = registerName.Substring(0, 2);
            var obj = getRegisterFromDictionary(registerName);
            try {
                int number = Convert.ToInt32(el.Text, 10);
                sbyte result = number > 0 ? (sbyte)(number % 128) : (sbyte)(number % 129);
                obj.setValue(result);
            } catch(Exception e1)
            {

            }

        }

        private void buttonRun_Click_1(object sender, EventArgs e)
        {
            try
            {
                runScript(textBoxAllCommands.Text);
            } catch(Exception e1)
            {
                MessageBox.Show("oops");
            }
            
            //textBoxAllCommands
        }


        // ???function return the list of numbers string's where was the error
        private void runScript(string textScript)
        {
            List<string> listCommands = new List<string>();
            List<int> listErrorStrings = new List<int>();
            int i = 0;

            listCommands.AddRange(textScript.Split(new char[] { '\r', '\n' }));
            while (listCommands.Remove("")) { }

            foreach (var el in listCommands)
            {
                try { 
                    excecuteCommand(el); // main Function
                    i++;
                }catch(Exception e1)
                {
                    listErrorStrings.Add(++i);
                    continue;
                }
            }
        }

        public Register getRegisterFromDictionary(string key) // return Register by key from Dictionarys
        {
            if (regDic.ContainsKey(key))
                return regDic[key];
            else if (memDic.ContainsKey(key))
                return memDic[key];
            return null;
        }

        private void excecuteCommand(string el)
        {
            List<string> bufferString = new List<string>();
            bufferString.AddRange(el.ToUpper().Split(new char[] { ' ', ',', ';' })); // разбиваем строку на слова
            while (bufferString.Remove("")) { } // удаляем пустые строки

            if (bufferString.Count < 2)
            {
                throw new Exception("invalid command"); // невозможная команда
            }

            var com = bufferString[0]; // команда
            var firstEl = bufferString[1]; // имя регистра к которому обращаются

            Register counter = getRegisterFromDictionary(firstEl); //  регистр назначения

            var method = typeof(Register).GetMethod(com); // reflection
            var paramsCount = method.GetParameters().Length;
            object[] param;
            if (paramsCount == 0)
            {
                param = new object[] { };
                method.Invoke(counter, param);
            }
            else if(paramsCount == 1)
            {
                if (bufferString.Count < 3) // если недостаточно аргументов
                    return;

                if (bufferString[2][0] >= '0' && bufferString[2][0] <= '9' || bufferString[2][0] == '-')
                {
                    param = new object[] { new Register(SByte.Parse(bufferString[2])) { frm = this, name = "AAA" } }; // если 2 аргумент - число
                }
                else
                {
                    Register secondArgCounter = getRegisterFromDictionary(bufferString[2]); // если 2 аргумент - другой регистр/память
                    param = new object[] { secondArgCounter };
                }
                method.Invoke(counter, param);

            }
            
        }

        private void buttonStep_Click(object sender, EventArgs e)
        {
            int nextCommandIndex = 0;
            var listCommands = new List<string>();
            listCommands.AddRange(textBoxAllCommands.Text.Split(new char[] { '\r', '\n' }));
            while (listCommands.Remove("")) { }
            
            if (listCommands.Count == 0) return;
            if (currentCommandIndex > listCommands.Count - 1)
            {
                currentCommandIndex = 0;
                nextCommandIndex = 1;
            }
            else if (currentCommandIndex == listCommands.Count - 1)
                nextCommandIndex = 0;
            else
                nextCommandIndex = currentCommandIndex + 1;

            if (nextCommandIndex > listCommands.Count - 1)
                nextCommandIndex = 0;
            var currentCommandLog = "";
            try { 
                currentCommandLog = "Номер строки: "+(currentCommandIndex+1)+"\r\n"
                                    +" Следующая Команда: \r\n"+listCommands[nextCommandIndex]+"\r\n";
            }catch(Exception e12)
            {
                currentCommandLog += "...";
            }
            try
            {
                excecuteCommand(listCommands[currentCommandIndex]);
            } catch(Exception e1)
            {
                currentCommandLog += "...";
            }
            textBoxInput.Text = currentCommandLog;
            currentCommandIndex++;
        }

        private void buttonZeroTextBoxes_Click(object sender, EventArgs e)
        {
            foreach (var el in regDic)
            {
                el.Value.setValue(0);
            }
            foreach (var el in memDic)
            {
                el.Value.setValue(0);
            }
        }


    }
}
