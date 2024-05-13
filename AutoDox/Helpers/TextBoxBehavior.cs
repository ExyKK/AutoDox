using System.Windows.Controls;
using System.Windows;

namespace AutoDox.UI.Helpers
{
    internal class TextBoxBehavior
    {
        public static readonly DependencyProperty AutoScrollToEndProperty =
            DependencyProperty.RegisterAttached("AutoScrollToEnd", typeof(bool),
            typeof(TextBoxBehavior), new FrameworkPropertyMetadata(false, OnAutoScrollToEndChanged));

        public static bool GetAutoScrollToEnd(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollToEndProperty);
        }

        public static void SetAutoScrollToEnd(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollToEndProperty, value);
        }

        private static void OnAutoScrollToEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = d as TextBox;
            if (textBox == null)
                return;

            var newValue = (bool)e.NewValue;
            if (newValue)
            {
                textBox.TextChanged += TextBox_TextChanged;
            }
            else
            {
                textBox.TextChanged -= TextBox_TextChanged;
            }
        }

        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
                return;

            textBox.ScrollToEnd();
        }
    }
}
