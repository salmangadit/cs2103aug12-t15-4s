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
    class Toast
    {
        Label lblToast;
        bool enter;
        int toastTime;
        DispatcherTimer toastTimer, toastAnimationTimer;
        protected static readonly ILog log = LogManager.GetLogger(typeof(Toast));

        public Toast(Label lblToast)
        {
            this.lblToast = lblToast;
        }

        public void ShowToast(string toast)
        {
            log.Debug("Showing toast...");
            toastTimer = new System.Windows.Threading.DispatcherTimer();
            toastTimer.Tick += new EventHandler(toastTimer_Tick);
            toastTimer.Interval = new TimeSpan(0, 0, 3);
            toastTimer.Start();

            toastAnimationTimer = new DispatcherTimer();
            toastAnimationTimer.Tick += new EventHandler(toastAnimationTimer_Tick);
            toastAnimationTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            toastAnimationTimer.Start();
            enter = true;
            toastTime = 0;

            lblToast.Content = toast;
            lblToast.Width = toast.Length * 10;
            lblToast.Visibility = Visibility.Visible;
            lblToast.Opacity = 0;
        }
        
        private void toastTimer_Tick(object sender, EventArgs e)
        {
            lblToast.Visibility = Visibility.Collapsed;
            toastTimer.Stop();
        }

        private void toastAnimationTimer_Tick(object sender, EventArgs e)
        {
            toastTime++;
            if (lblToast.Opacity < 1 && enter)
            {
                if (lblToast.Opacity >= 0.8)
                {
                    enter = false;
                }
                lblToast.Opacity += 0.2;
            }
            else if (toastTime >= 15)
            {
                if (lblToast.Opacity <= 0.2)
                {
                    log.Debug("Hiding toast...");
                    toastAnimationTimer.Stop();
                }
                lblToast.Opacity -= 0.2;
            }
        }
    }
}
