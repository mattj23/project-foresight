using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using MathNet.Numerics.Statistics;
using Project_Foresight.Annotations;

namespace Project_Foresight.ViewModels
{
    public class ProbabilityDensityData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string XLabel { get; set; }
        public string SeriesTitle { get; set; }

        public double XMax
        {
            get { return _xMax; }
            set
            {
                if (value.Equals(_xMax)) return;
                _xMax = value;
                OnPropertyChanged();
            }
        }

        public double XMin
        {
            get { return _xMin; }
            set
            {
                if (value.Equals(_xMin)) return;
                _xMin = value;
                OnPropertyChanged();
            }
        }

        public Func<double, string> ValueFormatter
        {
            get { return _valueFormatter; }
            set
            {
                if (Equals(value, _valueFormatter)) return;
                _valueFormatter = value;
                OnPropertyChanged();
            }
        }

        public SeriesCollection ChartCollection
        {
            get { return _chartCollection; }
            set
            {
                if (Equals(value, _chartCollection)) return;
                _chartCollection = value;
                OnPropertyChanged();
            }
        }

        private List<double> _rawData;
        private SeriesCollection _chartCollection;
        private double _xMin;
        private double _xMax;
        private Func<double, string> _valueFormatter;

        public ProbabilityDensityData(IEnumerable<double> rawData, string seriesTitle)
        {
            this.XMax = 1;
            _rawData = rawData.ToList();
            this.SeriesTitle = seriesTitle;
            this.Compute();
            this.ValueFormatter = x => x.ToString();
        }

        private void Compute()
        {
            double minValue = Math.Floor(_rawData.Min());
            double maxValue = Math.Ceiling(_rawData.Max());

            int binCount = (int)(maxValue - minValue);
            int binWidth = (int)Math.Round(binCount / 20.0);
            if (binWidth < 1)
                binWidth = 1;

            var histogram = new Histogram(_rawData, (binCount / binWidth) + 2, minValue - binWidth, maxValue + binWidth);
            var chartValues = new ChartValues<ObservablePoint>();

            for (int i = 0; i < histogram.BucketCount; i++)
            {
                chartValues.Add(new ObservablePoint(
                    (histogram[i].LowerBound + histogram[i].UpperBound) / 2.0,
                    histogram[i].Count / _rawData.Count));
            }
            this.ChartCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = SeriesTitle,
                    Values = chartValues
                }
            };
            
            this.XMax = histogram.UpperBound;
            this.XMin = histogram.LowerBound;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}