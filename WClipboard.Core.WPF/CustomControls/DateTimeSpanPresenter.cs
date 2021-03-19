using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WClipboard.Core.WPF.Converters.DateTimeSpan;

namespace WClipboard.Core.WPF.CustomControls
{
    public class DateTimeSpanPresenter : TextBlock
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(DateTime?), typeof(DateTimeSpanPresenter), new FrameworkPropertyMetadata(OnPropertyChanged));

        [DefaultValue(null)]
        public DateTime? Source
        {
            get => (DateTime?)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(nameof(Target), typeof(DateTime?), typeof(DateTimeSpanPresenter), new FrameworkPropertyMetadata(OnPropertyChanged));

        [DefaultValue(null)]
        public DateTime? Target
        {
            get => (DateTime?)GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }

        public static readonly DependencyProperty ConverterProperty = DependencyProperty.Register(nameof(Converter), typeof(IDateTimeSpanConverter), typeof(DateTimeSpanPresenter), new FrameworkPropertyMetadata(OnPropertyChanged));

        [DefaultValue(null)]
        public IDateTimeSpanConverter Converter
        {
            get => (IDateTimeSpanConverter)GetValue(ConverterProperty);
            set => SetValue(ConverterProperty, value);
        }

        public static readonly DependencyProperty ConverterParameterProperty = DependencyProperty.Register(nameof(ConverterParameter), typeof(object), typeof(DateTimeSpanPresenter), new FrameworkPropertyMetadata(OnPropertyChanged));

        [DefaultValue(null)]
        public object ConverterParameter
        {
            get => GetValue(ConverterParameterProperty);
            set => SetValue(ConverterParameterProperty, value);
        }

        public static readonly DependencyProperty ConverterCultureProperty = DependencyProperty.Register(nameof(ConverterCulture), typeof(CultureInfo), typeof(DateTimeSpanPresenter), new FrameworkPropertyMetadata(OnPropertyChanged));

        [DefaultValue(null)]
        [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
        public CultureInfo ConverterCulture
        {
            get => (CultureInfo)GetValue(ConverterCultureProperty);
            set => SetValue(ConverterCultureProperty, value);
        }

        private readonly DispatcherTimer timer;

        public DateTimeSpanPresenter()
        {
            timer = new DispatcherTimer(DispatcherPriority.Background);
            WeakEventManager<DispatcherTimer, EventArgs>.AddHandler(timer, nameof(DispatcherTimer.Tick), Timer_Tick);
        }

        private void Timer_Tick(object? sender, EventArgs e) => Update();
        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as DateTimeSpanPresenter)?.Update();

        private void Update()
        {
            timer.Stop();

            if ((Source == null && Target == null) || Converter == null)
            {
                Text = null;
            }
            else
            {
                var result = Converter.Convert(Source ?? DateTime.Now, Target ?? DateTime.Now, ConverterParameter, ConverterCulture ?? CultureInfo.CurrentUICulture);
                Text = result.Text;

                if (Source == null || Target == null)
                {
                    timer.Interval = result.ReUpdateOver;
                    timer.Start();
                }
            }
        }
    }
}
