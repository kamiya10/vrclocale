using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace VRCTC_Installer
{
    public static class ProgressBarExtension
    {
        private static readonly TimeSpan duration = TimeSpan.FromSeconds(2);

        public static void setPercent(this ProgressBar progressBar, double percentage)
        {
            DoubleAnimation animation = new DoubleAnimation(percentage, duration);
            progressBar.BeginAnimation(RangeBase.ValueProperty, animation);
        }
    }
}