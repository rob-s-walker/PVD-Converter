using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PVDConverter
{
    class DataModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChange([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }



        private int percent = 0;
        public int Percent
        {
            get { return this.percent; }
            set
            {

                this.percent = value;
                NotifyPropertyChange();
            }
        }
        private string textout = "";
        public string TextOut
        {
            get { return textout; }
            set
            {
                textout = value;

                NotifyPropertyChange();
            }
        }
    }
}
