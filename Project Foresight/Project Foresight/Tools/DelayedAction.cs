using System;
using System.Windows;
using System.Windows.Threading;

namespace Project_Foresight.Tools
{
    public class DelayedAction
    {
        public static DelayedAction Execute(Action action, TimeSpan delay)
        {
            var item = new DelayedAction(action, delay);
            item.Reset();
            return item;
        }

        public static DelayedAction Execute(Action action, int milliseconds)
        {
            return DelayedAction.Execute(action, new TimeSpan(0, 0, 0, 0, milliseconds));
        }

        private Action _action;
        private TimeSpan _delay;
        private readonly DispatcherTimer _timer;

        public DelayedAction(Action action, TimeSpan delay)
        {
            _action = action;
            _delay = delay;
            _timer = new DispatcherTimer {Interval = delay};

            _timer.Tick += (sender, args) =>
            {
                action.Invoke();
                _timer.Stop();
            };

            
        }

        public void Reset()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

    }
}