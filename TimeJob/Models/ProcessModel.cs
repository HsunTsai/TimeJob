using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Timers;
using System.Windows;
using System.Windows.Media.Imaging;

namespace TimeJob.Models
{
    class ProcessModel : INotifyPropertyChanged
    {
        private Timer timer;

        public ProcessModel(Process value)
        {
            process = value;
            id = value.Id;
            name = value.MainWindowTitle;
            time = 1000;
            responding = value.Responding; //Status
            memory = value.PrivateMemorySize64; //Memory (private working set in Bytes)
            try
            {
                if (null != value.MainModule)
                {
                    path = value.MainModule.FileName;
                    bitmapSource = bitmapToBitmapSource(Icon.ExtractAssociatedIcon(processModel.path).ToBitmap());
                    bitmapSource.Freeze();
                }
            }
            catch
            {

            }
        }

        public int id { get; set; }
        public string name { get; set; }
        public bool responding { get; set; }
        public bool schedule
        {
            get { return schedule; }
            set
            {
                if (value)
                {
                    timer = new Timer(interval: 1000);
                    timer.Elapsed += (new ElapsedEventHandler(setTimeCountDown));
                    timer.Start();
                }
                else
                {
                    if (null != timer) timer.Stop();
                    time = 1000;
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
