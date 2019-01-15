namespace Mamesaver.Config.ViewModels
{
    public abstract class InitialisableViewModel : ViewModel
    {
        private bool _initialised;

        public virtual void Initialise()
        {
            if (_initialised) return;
            PerformInitialise();
            _initialised = true;
        }

        protected abstract void PerformInitialise();
    }
}