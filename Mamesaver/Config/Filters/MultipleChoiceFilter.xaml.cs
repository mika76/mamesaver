using System.Windows;
using Mamesaver.Config.Extensions;
using Mamesaver.Config.Filters.ViewModels;

namespace Mamesaver.Config.Filters
{
    public partial class MultipleChoiceFilter
    {
        public MultipleChoiceFilter()
        {
            InitializeComponent();
            this.InitViewModel<MultipleChoiceFilterViewModel>();
        }

        /// <summary>
        ///     Registers the <c>Field</c> dependency property to indicate which field in <see cref="Models.GameViewModel"/> 
        ///     is being filtered on.
        /// </summary>
        public static readonly DependencyProperty FieldProperty = 
            DependencyProperty.Register("Field", typeof(string), typeof(MultipleChoiceFilter));

        private MultipleChoiceFilterViewModel ViewModel => (MultipleChoiceFilterViewModel) DataContext;

        public string Field
        {
            get => (string)GetValue(FieldProperty);
            set => SetValue(FieldProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ViewModel.FilterProperty = Field;
        }
    }
}