using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Timers;
using System.Windows;
using System.Windows.Media.Imaging;

namespace TimeJob.Models
{
    class ProcessModel : INotifyPropertyChanged
    {
        private Timer timer;
        private Schedule _schedule;

        public enum Schedule
        {
            UNSET,
            STOP,
            ALARM_CLOCK,
            TIMER
        }

        public ProcessModel(Process value)
        {
            process = value;
            id = value.Id;
            name = value.MainWindowTitle;
            time = 30;
            _schedule = Schedule.UNSET;
            responding = value.Responding; //Status
            memory = value.PrivateMemorySize64; //Memory (private working set in Bytes)

            try
            {
                path = value.MainModule.FileName;
                bitmapSource = bitmapToBitmapSource(Icon.ExtractAssociatedIcon(processModel.path).ToBitmap());
            }
            catch
            {
                bitmapSource = new BitmapImage(new Uri("pack://application:,,,/Images/app.png", UriKind.Absolute));
            }
            finally
            {
                bitmapSource.Freeze();
            }
        }

        public int id { get; set; }
        public string name { get; set; }
        public bool responding { get; set; }
        public Schedule schedule
        {
            get { return _schedule; }
            set
            {
                if (_schedule == Schedule.UNSET)
                {
                    switch (value)
                    {
                        case Schedule.ALARM_CLOCK:
                            break;
                        case Schedule.TIMER:
                            timer = new Timer(interval: 1000);
                            timer.Elapsed += (new ElapsedEventHandler(setTimeCountDown));
                            timer.Start();
                            break;
                    }
                    _schedule = value;
                }
                else if (value == Schedule.STOP)
                {
                    _schedule = Schedule.UNSET;
                    if (null != timer) timer.Stop();
                    time = 30;
                }
            }
        }
        public long memory { get; set; }
        public string path { get; set; }
        public int time { get; set; }
        public bool showPath { get { return path != null; } set { showPath = value; } }
        public BitmapSource bitmapSource { get; set; }
        public Process process { get; set; }
        public ProcessModel processModel { get { return this; } }
        public event PropertyChangedEventHandler PropertyChanged;

        public static BitmapSource bitmapToBitmapSource(Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                          source.GetHbitmap(),
                          IntPtr.Zero,
                          Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
        }

        void setTimeCountDown(object sender, System.Timers.ElapsedEventArgs e)
        {
            time -= 1;
            RaisePropertyChanged("time");
        }

        private string GetMainModuleFilepath(int processId)
        {
            string wmiQueryString = "SELECT ProcessId, ExecutablePath FROM Win32_Process WHERE ProcessId = " + processId;
            using (var searcher = new ManagementObjectSearcher(wmiQueryString))
            {
                using (var results = searcher.Get())
                {
                    ManagementObject mo = results.Cast<ManagementObject>().FirstOrDefault();
                    if (mo != null)
                    {
                        return (string)mo["ExecutablePath"];
                    }
                }
            }
            return null;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
