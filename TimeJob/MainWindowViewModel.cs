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
using System.Timers;
using Timer = System.Timers.Timer;

namespace TimeJob
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private ICommand clickProcessTimer, clickProcessAlarmClock, removeScehduleProcess;

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
                    Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        processes.Add(new ProcessModel(process));
                    });
                }
            }
        }

        public ICommand ClickProcessTimer
        {
            get
            {
                return clickProcessTimer ?? (clickProcessTimer = new RelayCommand(x =>
                {
                    addProcessToSchedule(x as ProcessModel, ProcessModel.Schedule.TIMER);
                }));
            }
        }

        public ICommand ClickProcessAlarmClock
        {
            get
            {
                return clickProcessAlarmClock ?? (clickProcessAlarmClock = new RelayCommand(x =>
                {
                    addProcessToSchedule(x as ProcessModel, ProcessModel.Schedule.ALARM_CLOCK);
                }));
            }
        }



        private void addProcessToSchedule(ProcessModel processModel, ProcessModel.Schedule scheduleType)
        {
            foreach (ProcessModel selectProcessModel in processesSelect)
                if (processModel.id == selectProcessModel.id) return;

            processModel.schedule = scheduleType;
            processesSelect.Add(processModel);
            //MessageBox.Show(processModel.name + " element clicked");
        }

        public ICommand RemoveScehduleProcess
        {
            get
            {
                return removeScehduleProcess ?? (removeScehduleProcess = new RelayCommand(x =>
                {
                    removeProcess(x as ProcessModel);
                }));
            }
        }

        private void removeProcess(ProcessModel processModel)
        {
            processModel.schedule = ProcessModel.Schedule.STOP;
            processesSelect.Remove(processModel);
            //MessageBox.Show(processModel.name + " element clicked");
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
