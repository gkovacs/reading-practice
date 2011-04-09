using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace ReadingPractice
{
    public static class Extensions
    {
        public static void AddScrollCallback(this ScrollViewer view, PropertyChangedCallback callback)
        {
            RegisterForNotification("HorizontalOffset", view, callback);
            RegisterForNotification("VerticalOffset", view, callback);
        }

        public static void RegisterForNotification(string property, FrameworkElement frameworkElement, PropertyChangedCallback OnCallBack)
        {
            Binding binding = new Binding(property)
            {
                Source = frameworkElement
            };

            var dependencyproperty = System.Windows.DependencyProperty.RegisterAttached("ListenAttached" + property,
                                     typeof(object), typeof(UserControl), new System.Windows.PropertyMetadata(OnCallBack));

            frameworkElement.SetBinding(dependencyproperty, binding);
        } 
    }
}
