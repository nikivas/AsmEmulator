using System.Windows.Forms;

namespace WindowsFormsApplication1.flags
{
    public abstract class FlagA
    {
        public string name { get; set; }
        public TextBox textBoxF;

        protected bool _value;
        public bool value
        {
            get { return _value; }
            set
            {
                _value = value;
                update();
                reset(); 
            }
        }

        public abstract void setValue(int value);

        public virtual void update()
        {
            if (this.textBoxF != null)
                this.textBoxF.Text = _value ? "1" : "0";
        }

        public virtual void updateFromTextBox()
        {
            if (this.textBoxF != null)
                this._value = textBoxF.Text == "1";
        }

        public virtual void reset()
        {
            this._value = false;
        }
    }
}
