using System;
using System.Collections.Generic;
using System.Windows;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IPrimeGenerator primeGenerator;
        public MainWindow()
        {
            InitializeComponent();
            primeGenerator = new PrimeGenerator();
        }

        private async void cmdPrimes_Click(object sender, RoutedEventArgs e)
        {
            cmdPrimes.Visibility = Visibility.Collapsed;
            spinnerWait.Visibility = Visibility.Visible;
            spinnerWait.Spin = true;

            long start = Convert.ToInt64(txtStartValue.Text);
            long end = Convert.ToInt64(txtLastValue.Text);

            int startTime = Environment.TickCount;

            List<long> primes = await primeGenerator.GetPrimesParallel(start, end);

            int stopTime = Environment.TickCount;

            double elapsedTimeInSecs = (stopTime - startTime) / 1000.0;

            lblTime.Content = string.Format("{0:#,##0.00} secs", elapsedTimeInSecs);

            lblCount.Content = primes.Count;

            lstResult.ItemsSource = primes;

            spinnerWait.Visibility = Visibility.Collapsed;
            spinnerWait.Spin = false;
            cmdPrimes.Visibility = Visibility.Visible;
        }
    }
}
