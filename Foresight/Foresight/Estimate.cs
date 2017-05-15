using System.Reflection;

namespace Foresight
{
    public class Estimate
    {
        private double _min;
        private double _max;
        private double _mode;

        public double Min
        {
            get { return _min; }
            set
            {
                _min = value;
                if (Min > Mode)
                    Mode = Min;
            }
        }

        public double Max
        {
            get { return _max; }
            set
            {
                _max = value;
                if (Max < Mode)
                    Mode = Max;
            }
        }

        public double Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                if (Mode < Min)
                    Min = Mode;
                if (Mode > Max)
                    Max = Mode;
            }
        }

        public Estimate() { }

        public Estimate(double min, double mode, double max)
        {
            Min = min;
            Mode = mode;
            Max = max;
        }

        public Estimate(double mode)
        {
            Min = mode;
            Mode = mode;
            Max = mode;
        }
    }
}