using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace CashY.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        bool isBusy = false;
        /// <summary>
        /// when task running or someting needed to waiting 
        /// it's boolean show is busy or not.
        /// </summary>
        public bool IsBusy
        {
            get { return isBusy; }
            set 
            {
                SetProperty(ref isBusy, value);
            }
        }
        protected bool SetProperty<T>(ref T backingProperty, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingProperty, value))
                return false;

            backingProperty = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var changed = PropertyChanged;
            if (changed == null) return;
            changed(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
