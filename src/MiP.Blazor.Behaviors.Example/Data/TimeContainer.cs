using System;
using System.ComponentModel;

namespace MiP.Blazor.Behaviors.Example.Data
{
    public class TimeContainer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _time;

        public string Time
        {
            get { return _time; }
            set
            {
                _time = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Time)));
            }
        }

        public void Update()
        {
            Time = DateTime.Now.TimeOfDay.ToString("hh\\:mm\\:ss");
        }
    }
}
