using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows;
using log4net;
using System.Windows.Media.Animation;

namespace WhiteBoard
{
    //@author U095146E
    class Toast
    {
        #region Private Fields
        private TextBlock lblToast;
        private bool fadeIn;
        private int toastDuration;
        private Storyboard storyboard;
        private DoubleAnimation fadeInAnimation, fadeOutAnimation;
        #endregion

        #region Protected Fields
        protected static readonly ILog log = LogManager.GetLogger(typeof(Toast));
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for Toast. Requires access to the Toast label declared in MainWindow
        /// </summary>
        /// <param name="lblToast">Reference to UI element from MainWindow</param>
        public Toast(TextBlock lblToast)
        {
            this.lblToast = lblToast;
        }
        #endregion

        #region Public Class Methods
        /// <summary>
        /// Method to display toast on screen
        /// </summary>
        /// <param name="toast">Message to show on toast</param>
        public void ShowToast(string toast, MainWindow main)
        {
            log.Debug("Showing toast...");
            fadeIn = true;
            toastDuration = 0;

            lblToast.Text = toast;

            // Set up toast width
            if (toast.Length * Constants.TOAST_WIDTH_PER_CHARACTER > Constants.MAX_TOAST_WIDTH)
            {
                lblToast.Width = Constants.MAX_TOAST_WIDTH;
            }
            else
            {
                lblToast.Width = toast.Length * Constants.TOAST_WIDTH_PER_CHARACTER;
            }

            lblToast.Visibility = Visibility.Visible;
            lblToast.Opacity = 0;

            storyboard = new Storyboard();

            FadeInAnimation(main);

            FadeOutAnimation(main);
        }
        #endregion

        #region Private Class Helper Methods
        /// <summary>
        /// Animation to perform fade out of Toast
        /// </summary>
        /// <param name="main"></param>
        private void FadeOutAnimation(MainWindow main)
        {
            fadeOutAnimation = new DoubleAnimation();
            fadeOutAnimation.From = 1.0;
            fadeOutAnimation.To = 0.0;
            fadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(2));
            storyboard.Children.Add(fadeOutAnimation);
            Storyboard.SetTargetName(fadeOutAnimation, "lblToast");
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(TextBlock.OpacityProperty));

            storyboard.Begin(main);
        }

        /// <summary>
        /// Animation to perform fade in Toast
        /// </summary>
        /// <param name="main"></param>
        private void FadeInAnimation(MainWindow main)
        {
            //Fade in toast
            fadeInAnimation = new DoubleAnimation();
            fadeInAnimation.From = 0.0;
            fadeInAnimation.To = 1.0;
            fadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(2));
            storyboard.Children.Add(fadeInAnimation);
            Storyboard.SetTargetName(fadeInAnimation, "lblToast");
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(TextBlock.OpacityProperty));

            storyboard.Begin(main);
        }
        #endregion
    }
}
