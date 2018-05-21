namespace CapsKey.Model
{
    public class MainModel : ModelBase
    {
        private bool _isCapsActive;
        public bool IsCapsActive
        {
            get { return _isCapsActive; }
            set
            {
                if (_isCapsActive != value)
                {
                    _isCapsActive = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
