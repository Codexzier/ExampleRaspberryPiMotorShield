using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleRaspberryPiMotorShield
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private string _statusText;

        public string StatusText
        {
            get => this._statusText;
            set
            {
                this._statusText = value;
                this.OnPropertyChanged(nameof(this.StatusText));
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
