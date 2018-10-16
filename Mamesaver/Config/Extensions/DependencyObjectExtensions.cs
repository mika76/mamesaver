using System.Windows;
using System.Windows.Media;

namespace Mamesaver.Config.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static T FindVisualChildByName<T>(this DependencyObject parent, string name) where T : DependencyObject
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                var controlName = child.GetValue(FrameworkElement.NameProperty) as string;
                if (controlName == name)
                {
                    return child as T;
                }

                var result = FindVisualChildByName<T>(child, name);
                if (result != null) return result;
            }

            return null;
        }

        public static T FindVisualChild<T>(this DependencyObject current) where T : DependencyObject
        {
            if (current == null) return null;
            var childrenCount = VisualTreeHelper.GetChildrenCount(current);

            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(current, i);
                if (child is T variable) return variable;
                var result = FindVisualChild<T>(child);
                if (result != null) return result;
            }

            return null;
        }

        public static T FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            // get parent item
            var parentObject = VisualTreeHelper.GetParent(child);

            // Check if the parent matches the type we're looking for
            switch (parentObject)
            {
                case null:
                    // We've reached the end of the tree
                    return null;
                case T parent:
                    return parent;
                default:
                    return FindParent<T>(parentObject);
            }
        }
    }
}