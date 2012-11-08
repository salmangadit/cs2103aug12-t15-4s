using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace WhiteBoard
{
    /// <summary>
    /// Interaction logic for Tutorial.xaml
    /// </summary>
    public partial class Tutorial : UserControl
    {
        //@author U095146E
        public Tutorial()
        {
            InitializeComponent();
        }

        private void skipTour_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void SetUpAnimation()
        {
            
        }

        private void next1_Click(object sender, RoutedEventArgs e)
        {
            Storyboard myStoryboard = new Storyboard();

            //Remove step 1
            DoubleAnimation myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "txtContent1");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "imgImage1");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(Image.OpacityProperty));

            myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "next1");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(Button.OpacityProperty));

            myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "txtIntro");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(TextBlock.OpacityProperty));


            txtContent1.Visibility = Visibility.Collapsed;
            imgImage1.Visibility = Visibility.Collapsed;
            next1.Visibility = Visibility.Collapsed;
            txtIntro.Visibility = Visibility.Collapsed;
            myStoryboard.Begin(this);

            //Show Step 2
            myStoryboard = new Storyboard();
            next2.Visibility = Visibility.Visible;
            txtContent2.Visibility = Visibility.Visible;
            stkContent2.Visibility = Visibility.Visible;
            txtAdd.Visibility = Visibility.Visible;

            DoubleAnimation myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "txtContent2");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "stkContent2");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(StackPanel.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "txtAdd");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "next2");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(Button.OpacityProperty));

            myStoryboard.Begin(this);
        }

        private void next2_Click(object sender, RoutedEventArgs e)
        {
            //Hide Step 2
            Storyboard myStoryboard = new Storyboard();
            next2.Visibility = Visibility.Collapsed;
            txtContent2.Visibility = Visibility.Collapsed;
            stkContent2.Visibility = Visibility.Collapsed;
            txtAdd.Visibility = Visibility.Collapsed;

            DoubleAnimation myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "txtContent2");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "stkContent2");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(StackPanel.OpacityProperty));

            myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "txtAdd");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "next2");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(Button.OpacityProperty));

            myStoryboard.Begin(this);

            //Show Step 3
            myStoryboard = new Storyboard();
            next3.Visibility = Visibility.Visible;
            txtContent3.Visibility = Visibility.Visible;
            stkContent3.Visibility = Visibility.Visible;
            txtEdit.Visibility = Visibility.Visible;

            DoubleAnimation myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "txtContent3");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "stkContent3");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(StackPanel.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "txtEdit");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "next3");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(Button.OpacityProperty));

            myStoryboard.Begin(this);
        }

        private void next3_Click(object sender, RoutedEventArgs e)
        {
            //Hide Step 3
            Storyboard myStoryboard = new Storyboard();
            next3.Visibility = Visibility.Collapsed;
            txtContent3.Visibility = Visibility.Collapsed;
            stkContent3.Visibility = Visibility.Collapsed;
            txtEdit.Visibility = Visibility.Collapsed;

            DoubleAnimation myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "txtContent3");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "stkContent3");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(StackPanel.OpacityProperty));

            myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "txtEdit");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "next3");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(Button.OpacityProperty));

            myStoryboard.Begin(this);

            //Show Step 4
            myStoryboard = new Storyboard();
            next4.Visibility = Visibility.Visible;
            txtContent4.Visibility = Visibility.Visible;
            stkContent4.Visibility = Visibility.Visible;
            txtView.Visibility = Visibility.Visible;

            DoubleAnimation myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "txtContent4");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "stkContent4");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(StackPanel.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "txtView");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "next4");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(Button.OpacityProperty));

            myStoryboard.Begin(this);
        }

        private void next4_Click(object sender, RoutedEventArgs e)
        {
            //Hide Step 4
            Storyboard myStoryboard = new Storyboard();
            next4.Visibility = Visibility.Collapsed;
            txtContent4.Visibility = Visibility.Collapsed;
            stkContent4.Visibility = Visibility.Collapsed;
            txtView.Visibility = Visibility.Collapsed;

            DoubleAnimation myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "txtContent4");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "stkContent4");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(StackPanel.OpacityProperty));

            myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "txtView");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "next4");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(Button.OpacityProperty));

            myStoryboard.Begin(this);

            //Show Step 5
            myStoryboard = new Storyboard();
            next5.Visibility = Visibility.Visible;
            txtContent5.Visibility = Visibility.Visible;
            stkContent5.Visibility = Visibility.Visible;
            txtDelete.Visibility = Visibility.Visible;

            DoubleAnimation myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "txtContent5");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "stkContent5");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(StackPanel.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "txtDelete");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "next5");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(Button.OpacityProperty));

            myStoryboard.Begin(this);
        }

        private void next5_Click(object sender, RoutedEventArgs e)
        {
            //Hide Step 5
            Storyboard myStoryboard = new Storyboard();
            next5.Visibility = Visibility.Collapsed;
            txtContent5.Visibility = Visibility.Collapsed;
            stkContent5.Visibility = Visibility.Collapsed;
            txtDelete.Visibility = Visibility.Collapsed;

            DoubleAnimation myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "txtContent5");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "stkContent5");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(StackPanel.OpacityProperty));

            myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "txtDelete");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "next5");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(Button.OpacityProperty));

            myStoryboard.Begin(this);

            //Show Step 6
            myStoryboard = new Storyboard();
            next6.Visibility = Visibility.Visible;
            txtContent6.Visibility = Visibility.Visible;
            stkContent6.Visibility = Visibility.Visible;
            txtArchive.Visibility = Visibility.Visible;

            DoubleAnimation myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "txtContent6");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "stkContent6");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(StackPanel.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "txtArchive");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "next6");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(Button.OpacityProperty));

            myStoryboard.Begin(this);
        }

        private void next6_Click(object sender, RoutedEventArgs e)
        {
            //Hide Step 6
            Storyboard myStoryboard = new Storyboard();
            next6.Visibility = Visibility.Collapsed;
            txtContent6.Visibility = Visibility.Collapsed;
            stkContent6.Visibility = Visibility.Collapsed;
            txtArchive.Visibility = Visibility.Collapsed;

            DoubleAnimation myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "txtContent6");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "stkContent6");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(StackPanel.OpacityProperty));

            myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "txtArchive");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "next6");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(Button.OpacityProperty));

            myStoryboard.Begin(this);

            //Show Step 7
            myStoryboard = new Storyboard();
            next7.Visibility = Visibility.Visible;
            txtContent7.Visibility = Visibility.Visible;
            stkContent7.Visibility = Visibility.Visible;
            txtUndo.Visibility = Visibility.Visible;

            DoubleAnimation myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "txtContent7");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "stkContent7");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(StackPanel.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "txtUndo");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "next7");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(Button.OpacityProperty));

            myStoryboard.Begin(this);
        }

        private void next7_Click(object sender, RoutedEventArgs e)
        {
            //Hide Step 7
            Storyboard myStoryboard = new Storyboard();
            next7.Visibility = Visibility.Collapsed;
            txtContent7.Visibility = Visibility.Collapsed;
            stkContent7.Visibility = Visibility.Collapsed;
            txtUndo.Visibility = Visibility.Collapsed;

            DoubleAnimation myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "txtContent7");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "stkContent7");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(StackPanel.OpacityProperty));

            myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "txtUndo");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "next7");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(Button.OpacityProperty));

            myStoryboard.Begin(this);

            //Show Step 8
            myStoryboard = new Storyboard();
            next8.Visibility = Visibility.Visible;
            txtContent8.Visibility = Visibility.Visible;
            stkContent8.Visibility = Visibility.Visible;
            txtSearch.Visibility = Visibility.Visible;

            DoubleAnimation myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "txtContent8");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "stkContent8");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(StackPanel.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "txtSearch");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "next8");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(Button.OpacityProperty));

            myStoryboard.Begin(this);

        }

        private void next8_Click(object sender, RoutedEventArgs e)
        {
            //Hide Step 8
            Storyboard myStoryboard = new Storyboard();
            next8.Visibility = Visibility.Collapsed;
            txtContent8.Visibility = Visibility.Collapsed;
            stkContent8.Visibility = Visibility.Collapsed;
            txtSearch.Visibility = Visibility.Collapsed;

            DoubleAnimation myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "txtContent8");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "stkContent8");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(StackPanel.OpacityProperty));

            myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "txtSearch");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeOutAnimation = new DoubleAnimation();
            myFadeOutAnimation.From = 1.0;
            myFadeOutAnimation.To = 0.0;
            myFadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeOutAnimation);
            Storyboard.SetTargetName(myFadeOutAnimation, "next8");
            Storyboard.SetTargetProperty(myFadeOutAnimation, new PropertyPath(Button.OpacityProperty));

            myStoryboard.Begin(this);

            //Show Step 9
            myStoryboard = new Storyboard();
            next9.Visibility = Visibility.Visible;
            txtContent9.Visibility = Visibility.Visible;
            stkContent9.Visibility = Visibility.Visible;
            txtFeatures.Visibility = Visibility.Visible;
            txtEnjoy.Visibility = Visibility.Visible;

            DoubleAnimation myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "txtContent9");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "stkContent9");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(StackPanel.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "txtFeatures");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "txtEnjoy");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(TextBlock.OpacityProperty));

            myFadeInAnimation = new DoubleAnimation();
            myFadeInAnimation.From = 0.0;
            myFadeInAnimation.To = 1.0;
            myFadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myStoryboard.Children.Add(myFadeInAnimation);
            Storyboard.SetTargetName(myFadeInAnimation, "next9");
            Storyboard.SetTargetProperty(myFadeInAnimation, new PropertyPath(Button.OpacityProperty));

            myStoryboard.Begin(this);
        }

        private void next9_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
