using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Drawing;
using TimeJob.Models;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Threading;

namespace TimeJob
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private ICommand processCommand;

        public ObservableCollection<ProcessModel> processes = new ObservableCollection<ProcessModel>();
        public ObservableCollection<ProcessModel> processesData { get { return processes; } }
        public ObservableCollection<ProcessModel> processesSelect = new ObservableCollection<ProcessModel>();
        public ObservableCollection<ProcessModel> processesSelectData { get { return processesSelect; } }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            //Get process
            Thread getProcessThread = new Thread(new ThreadStart(getProcess));
            getProcessThread.Start();
        }

        //public ICommand testBtnCommand { get { return new RelayCommand(param => this.testBtnClick()); } }

        //private void testBtnClick()
        //{
        //    Console.WriteLine("testBtnClick");
        //RaisePropertyChanged("processes");
        //testModel.title = process_str;
        //RaisePropertyChanged("testModel");
        //}

        private void getProcess()
        {
            List<Process> processList = Process.GetProcesses().ToList();
            foreach (Process process in processList)
            {
                if (process.MainWindowTitle.Length > 0)
                {
                    ProcessModel processModel = new ProcessModel();
                    processModel.id = process.Id;
                    processModel.name = process.MainWindowTitle;
                    processModel.responding = process.Responding; //Status
                    processModel.memory = process.PrivateMemorySize64; //Memory (private working set in Bytes)
                    try
                    {
                        if (null != process.MainModule)
                        {
                            processModel.path = process.MainModule.FileName;
                            processModel.bitmapSource = bitmapToBitmapSource(Icon.ExtractAssociatedIcon(processModel.path).ToBitmap());
                            processModel.bitmapSource.Freeze();
                        }
                    }
                    catch
                    {

                    }

                    Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        processes.Add(processModel);
                    });
                }
            }
        }

        public ICommand ProcessCommand
        {
            get
            {
                return processCommand ?? (processCommand = new RelayCommand(x =>
                {
                    DoStuff(x as ProcessModel);
                }));
            }
        }

        private void DoStuff(ProcessModel processModel)
        {
            processesSelect.Add(processModel);
            //MessageBox.Show(processModel.name + " element clicked");
        }

        public static BitmapSource bitmapToBitmapSource(Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                          source.GetHbitmap(),
                          IntPtr.Zero,
                          Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
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
