using System;
using System.Windows;
using System.Windows.Threading;

namespace Project_Foresight.Tools
{
    public static class DelayedAction
    {
        public static void Execute(Action action, TimeSpan delay)
        {
            var timer = new DispatcherTimer
            {
                Interval = delay
            };

            timer.Tick += (sender, args) =>
            {
                action.Invoke();
                timer.Stop();
            };

            timer.Start();
        }

        public static void Execute(Action action, int milliseconds)
        {
            DelayedAction.Execute(action, new TimeSpan(0, 0, 0, 0, milliseconds));
        }
    }
}