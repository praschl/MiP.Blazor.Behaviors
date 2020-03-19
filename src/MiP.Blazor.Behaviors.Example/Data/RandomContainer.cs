using System;
using System.ComponentModel;

namespace MiP.Blazor.Behaviors.Example.Data
{
    public class RandomContainer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _randomId;

        public string RandomId
        {
            get => _randomId;
            set
            {
                _randomId = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RandomId)));
            }
        }

        public void Update() => RandomId = DateTime.Now.Millisecond.ToString();
    }
}
