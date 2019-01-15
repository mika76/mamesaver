using System.Windows;

namespace Mamesaver.Config.Infrastructure
{
    public class BindingProxy : Freezable
    {
        /// <summary>
        ///     Use a <see cref="DependencyProperty" /> as the backing store for <see cref="Data" /> to enable animation, styling,
        ///     binding etc.
        /// </summary>
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));

        public object Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        protected override Freezable CreateInstanceCore() => new BindingProxy();
    }
}