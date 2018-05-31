using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PSDGitFinal
{
    public class RippleDecorator : ContentControl
    {
        static RippleDecorator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RippleDecorator), new FrameworkPropertyMetadata(typeof(RippleDecorator)));
        }

        public Brush HighlightBackground
        {
            get { return (Brush)GetValue(HighlightBackgroundProperty); }
            set { SetValue(HighlightBackgroundProperty, value); }
        }

        public static readonly DependencyProperty HighlightBackgroundProperty =
            DependencyProperty.Register("HighlightBackground", typeof(Brush), typeof(RippleDecorator), new PropertyMetadata(Brushes.White));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Ellipse ellipse = GetTemplateChild("PART_ellipse") as Ellipse;
            Grid grid = GetTemplateChild("PART_grid") as Grid;
            Storyboard animation = grid.FindResource("PART_animation") as Storyboard;

            this.AddHandler(MouseDownEvent, new RoutedEventHandler((sender, e) =>
            {
                var targetWidth = Math.Max(ActualWidth, ActualHeight) * 2;
                var mousePosition = (e as MouseButtonEventArgs).GetPosition(this);
                var startMargin = new Thickness(mousePosition.X, mousePosition.Y, 0, 0);
                ellipse.Margin = startMargin;
                (animation.Children[0] as DoubleAnimation).To = targetWidth;
                (animation.Children[1] as ThicknessAnimation).From = startMargin;
                (animation.Children[1] as ThicknessAnimation).To = new Thickness(mousePosition.X - targetWidth / 2, mousePosition.Y - targetWidth / 2, 0, 0);
                ellipse.BeginStoryboard(animation);
            }), true);
        }
    }
}
