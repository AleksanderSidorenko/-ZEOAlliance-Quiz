using System;
using System.IO;
using System.Media;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TestWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum SelectedShape

        { None, Circle, Rectangle, Text }

        private SelectedShape Shape1 = SelectedShape.None;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateShape1_Click(object sender, RoutedEventArgs e)
        {
            Shape1 = SelectedShape.Rectangle;
        }

        private void CreateShape2_Click(object sender, RoutedEventArgs e)
        {
            Shape1 = SelectedShape.Circle;
        }

        private void CreateShape3_Click(object sender, RoutedEventArgs e)
        {
            Shape1 = SelectedShape.Text;
        }

        private void canvasArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            switch (Shape1)
            {

                case SelectedShape.Circle:
                    int radius = 25;
                    try
                    {
                        radius = int.Parse(InputCircleRadius.Text);
                    }
                    catch { }

                    var ellipse = new Ellipse() { Height = radius, Width = radius };

                    ImageBrush brush = new ImageBrush();
                    brush.ImageSource = new BitmapImage(new Uri("Face-smile.png", UriKind.Relative));
                    ellipse.Fill = brush;
                    Canvas.SetLeft(ellipse, e.GetPosition(canvasArea).X);
                    Canvas.SetTop(ellipse, e.GetPosition(canvasArea).Y);
                    canvasArea.Children.Add(ellipse);
                    break;

                case SelectedShape.Rectangle:
                    int side = 50;
                    try
                    {
                        side = int.Parse(InputSquareSide.Text);
                    }
                    catch { }

                    var rectangle = new Rectangle() { Height = side, Width = side };
                    brush = new ImageBrush();
                    brush.ImageSource = new BitmapImage(new Uri("sponge-Bob.png", UriKind.Relative));
                    rectangle.Fill = brush;
                    Canvas.SetLeft(rectangle, e.GetPosition(canvasArea).X);
                    Canvas.SetTop(rectangle, e.GetPosition(canvasArea).Y);
                    canvasArea.Children.Add(rectangle);
                    break;

                case SelectedShape.Text:
                    int fontSize = 16;
                    try
                    {
                        fontSize = int.Parse(InputRenderingTextFontSize.Text);
                    }
                    catch { }
                    
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = InputRenderingText.Text.Trim().Equals("") ? "<empty>" : InputRenderingText.Text;
                    textBlock.FontSize = fontSize;
                    textBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
                    Canvas.SetLeft(textBlock, e.GetPosition(canvasArea).X);
                    Canvas.SetTop(textBlock, e.GetPosition(canvasArea).Y);
                    canvasArea.Children.Add(textBlock);
                    break;

                default:
                    Point pt = e.GetPosition((Canvas)sender);

                    HitTestResult result = VisualTreeHelper.HitTest(canvasArea, pt);

                    if (result != null)
                    {
                        if (result.VisualHit is TextBlock)
                        {
                            SoundPlayer s = new SoundPlayer(File.OpenRead("spongebob-disappointed-sound-effect.wav"));
                            s.Play();
                            var element = result.VisualHit as TextBlock;

                            canvasArea.Children.Remove(element);
                            if (!element.RenderTransform.HasAnimatedProperties)
                            {
                                DoubleAnimation da = new DoubleAnimation(360, 0, new Duration(TimeSpan.FromSeconds(3)));
                                RotateTransform rotateTransform = new RotateTransform(0, element.ActualHeight / 2, element.ActualWidth / 2);
                                da.RepeatBehavior = RepeatBehavior.Forever;
                                rotateTransform.BeginAnimation(RotateTransform.AngleProperty, da);
                                element.RenderTransform = rotateTransform;
                            }
                            else
                            {
                                element.RenderTransform = new RotateTransform();
                            }
                            canvasArea.Children.Add(element);
                        }
                        else
                        {
                            var element = result.VisualHit as Shape;
                            if (element != null)
                            {
                                SoundPlayer s = new SoundPlayer(File.OpenRead("spongebob-disappointed-sound-effect.wav"));
                                s.Play();
                                canvasArea.Children.Remove(element);

                                if (!element.RenderTransform.HasAnimatedProperties)
                                {
                                    DoubleAnimation da = new DoubleAnimation(360, 0, new Duration(TimeSpan.FromSeconds(3)));
                                    RotateTransform rotateTransform = new RotateTransform(0, element.ActualWidth / 2, element.ActualHeight / 2);
                                    da.RepeatBehavior = RepeatBehavior.Forever;
                                    rotateTransform.BeginAnimation(RotateTransform.AngleProperty, da);

                                    element.RenderTransform = rotateTransform;
                                }
                                else
                                {
                                    element.RenderTransform = new RotateTransform();
                                }

                                canvasArea.Children.Add(element);
                            }
                        }

                    }
                    return;
            }

            Shape1 = SelectedShape.None;
        }

        private void canvasArea_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition((Canvas)sender);

            HitTestResult result = VisualTreeHelper.HitTest(canvasArea, pt);

            if (result != null)
            {
                canvasArea.Children.Remove(result.VisualHit as FrameworkElement);
                SoundPlayer s = new SoundPlayer(File.OpenRead("Bang.wav"));
                s.Play();
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
