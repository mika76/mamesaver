using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Mamesaver.Config.ViewModels;
using Mamesaver.Services;

namespace Mamesaver.Config.Extensions
{
    public static class FrameworkElementExtensions
    {
        public static void InitDesignMode(this FrameworkElement element)
        {
            // Clear design-mode background
            if (!DesignerProperties.GetIsInDesignMode(element)) element.ClearValue(Control.BackgroundProperty);
        }

        /// <summary>
        ///     Resolves either a <see cref="ViewModel"/> or <see cref="InitialisableViewModel"/>, optionally
        ///     initialises and sets it as the backing class's data context.
        /// </summary>
        public static T InitViewModel<T>(this FrameworkElement element) where T : ViewModel
        {
            var viewModel = ServiceResolver.GetInstance<T>();
            (viewModel as InitialisableViewModel)?.Initialise();

            element.DataContext = viewModel;

            return viewModel;
        }
    }
}
