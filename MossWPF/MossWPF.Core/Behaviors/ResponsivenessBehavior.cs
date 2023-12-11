using System.Windows;

namespace MossWPF.Core.Behaviors
{
    public class ResponsivenessBehavior
    {


        public static bool GetIsRepsonsiveProperty(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsRepsonsiveProperty);
        }

        public static void SetIsRepsonsiveProperty(DependencyObject obj, bool value)
        {
            obj.SetValue(IsRepsonsiveProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsRepsonsiveProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsRepsonsiveProperty =
            DependencyProperty.RegisterAttached("IsRepsonsive", typeof(bool), typeof(ResponsivenessBehavior), new PropertyMetadata(false, OnIsResponsiveChanged));

        

        public static double GetHorizontalBreakpoint(DependencyObject obj)
        {
            return (double)obj.GetValue(HorizontalBreakpointProperty);
        }

        public static void SetHorizontalBreakpoint(DependencyObject obj, double value)
        {
            obj.SetValue(HorizontalBreakpointProperty, value);
        }

        // Using a DependencyProperty as the backing store for HorizontalBreakpoint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalBreakpointProperty =
            DependencyProperty.RegisterAttached("HorizontalBreakpoint", typeof(double), typeof(ResponsivenessBehavior), new PropertyMetadata(double.MaxValue));



        public static SetterBaseCollection GetHorizontalBreakpointSetters(DependencyObject obj)
        {
            return (SetterBaseCollection)obj.GetValue(HorizontalBreakpointSettersProperty);
        }

        public static void SetHorizontalBreakpointSetters(DependencyObject obj, SetterBaseCollection value)
        {
            obj.SetValue(HorizontalBreakpointSettersProperty, value);
        }

        // Using a DependencyProperty as the backing store for HorizontalBreakpointSetters.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalBreakpointSettersProperty =
            DependencyProperty.RegisterAttached("HorizontalBreakpointSetters", typeof(SetterBaseCollection), typeof(ResponsivenessBehavior), new PropertyMetadata(new SetterBaseCollection()));




        public static bool GetIsHorizontalBreakpointSettersActive(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsHorizontalBreakpointSettersActiveProperty);
        }

        public static void SetIsHorizontalBreakpointSettersActive(DependencyObject obj, bool value)
        {
            obj.SetValue(IsHorizontalBreakpointSettersActiveProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsHorizontalBreakpointSettersActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsHorizontalBreakpointSettersActiveProperty =
            DependencyProperty.RegisterAttached("IsHorizontalBreakpointSettersActive", typeof(bool), typeof(ResponsivenessBehavior), new PropertyMetadata(false));

        private static void OnIsResponsiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           if(d is FrameworkElement element)
            {
                Window window = Application.Current.MainWindow;
                if ((GetIsRepsonsiveProperty(element)))
                {
                    window.SizeChanged += (s, e) => UpdateElement(window, element);
                }
                else
                {
                    window.SizeChanged -= (s, e) => UpdateElement(window, element);
                }
            }
        }

        private static void UpdateElement(Window window, FrameworkElement element)
        {
            double windowWidth = window.Width;
            double breakpointWidth = GetHorizontalBreakpoint(element);
            if (windowWidth >= breakpointWidth && !GetIsHorizontalBreakpointSettersActive(element))
            {
                SetIsHorizontalBreakpointSettersActive(element, true);
                element.Style = CreateResponsivenessStyle(element);

            }
            else if (windowWidth < breakpointWidth && GetIsHorizontalBreakpointSettersActive(element))
            {
                SetIsHorizontalBreakpointSettersActive(element, false);
                element.Style = element.Style.BasedOn;
            }
        }

        private static Style CreateResponsivenessStyle(FrameworkElement element)
        {
            Style responsivenessStyle = new Style(element.GetType(), element.Style);
            foreach(Setter setter in GetHorizontalBreakpointSetters(element))
            {
                responsivenessStyle.Setters.Add(setter);
            }
            return responsivenessStyle;
        }
    }
}
