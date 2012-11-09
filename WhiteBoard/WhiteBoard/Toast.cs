using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows;
using log4net;

namespace WhiteBoard
{
    //@author U095146E
    class Toast
    {
        #region Private Fields
        private TextBlock lblToast;
        private bool fadeIn;
        private int toastDuration;
        private DispatcherTimer toastTimer, toastAnimationTimer;
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
        public void ShowToast(string toast)
        {
            log.Debug("Showing toast...");

            InstantiateToastTimer();
            InstantiateAnimationTimer();

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
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Event handler for hiding the toast
        /// </summary>
        private void toastTimer_Tick(object sender, EventArgs e)
        {
            lblToast.Visibility = Visibility.Collapsed;
            toastTimer.Stop();
        }

        /// <summary>
        /// Event handler for toast animation timer. This animation is handle a series of ticks and concurrent
        /// modifications to the toast opacity.
        /// </summary>
        private void toastAnimationTimer_Tick(object sender, EventArgs e)
        {
            toastDuration++;
            if (lblToast.Opacity < 1 && fadeIn)
            {
                if (lblToast.Opacity >= 0.8)
                {
                    fadeIn = false;
                }
                lblToast.Opacity += 0.2;
            }
            else if (toastDuration >= 15)
            {
                if (lblToast.Opacity <= 0.2)
                {
                    log.Debug("Hiding toast...");
                    toastAnimationTimer.Stop();
                }
                lblToast.Opacity -= 0.2;
            }
        }
        #endregion

        #region Private Class Helper Methods
        /// <summary>
        /// Instantiate timer to display toast
        /// </summary>
        private void InstantiateAnimationTimer()
        {
            toastAnimationTimer = new DispatcherTimer();
            toastAnimationTimer.Tick += new EventHandler(toastAnimationTimer_Tick);
            toastAnimationTimer.Interval = new TimeSpan(0, 0, 0, 0, Constants.TOAST_ANIMATION_MILLISECONDS);
            toastAnimationTimer.Start();
        }

        /// <summary>
        /// Instantiate timer to animate the toast
        /// </summary>
        private void InstantiateToastTimer()
        {
            toastTimer = new System.Windows.Threading.DispatcherTimer();
            toastTimer.Tick += new EventHandler(toastTimer_Tick);
            toastTimer.Interval = new TimeSpan(0, 0, Constants.TOAST_DISPLAY_DURATION_SECONDS);
            toastTimer.Start();
        }
        #endregion
    }
}
