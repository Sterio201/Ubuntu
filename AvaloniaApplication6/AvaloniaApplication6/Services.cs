using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication6
{
    public static class Services
    {
        public static List<OutputProcess> GetProcesses()
        {
            List<OutputProcess> processes = new List<OutputProcess>();

            var processStartInfo = new ProcessStartInfo("systemctl", "--no-pager --no-legend list-units --all")
            {
                RedirectStandardOutput = true
            };

            var proc = new Process()
            {
                StartInfo = processStartInfo
            };

            proc.Start();

            int i = 0;

            while (!proc.StandardOutput.EndOfStream)
            {
                var output = proc.StandardOutput.ReadLine();
                //Console.WriteLine(output);

                var processFields = output.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var name = "";
                var download = "";
                var active = "";
                var dop = "";

                if (processFields[0] != "●")
                {
                    name = processFields[0];
                    download = processFields[1];
                    active = processFields[2];
                    dop = processFields[3];
                }
                else 
                {
                    name = processFields[1];
                    download = processFields[2];
                    active = processFields[3];
                    dop = "";
                }

                processes.Add(new OutputProcess
                {
                    Name = name,
                    StatusDownload = download,
                    StatusActive = active,
                    DopStatus = dop,

                    id = i
                });

                i++;
            }

            proc.WaitForExit();

            return processes;
        }

        public static void CreateProcess(string nameProcess) 
        {
            var processStartInfo = new ProcessStartInfo("systemctl")
            {
                Arguments = $"start {nameProcess}",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            };

            var proc = new Process()
            {
                StartInfo = processStartInfo
            };

            proc.Start();
            proc.WaitForExit();
        }

        public static void DeleteProcess(string nameProcess) 
        {
            var processStartInfo = new ProcessStartInfo("systemctl")
            {
                Arguments = $"stop {nameProcess}",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            };

            var proc = new Process()
            {
                StartInfo = processStartInfo
            };

            proc.Start();
            proc.WaitForExit();
        }
    }

    public class OutputProcess : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string? Name { get; set; }
        public string? StatusDownload { get; set; }

        private string statusActive;
        public string? StatusActive 
        {
            get { return statusActive; }
            set
            {
                if (statusActive != value)
                {
                    statusActive = value;
                    OnPropertyChanged(nameof(StatusActive));
                }
            }
        }
        public string? DopStatus { get; set; }

        public int id;

        public override string ToString()
        {
            return ConvertName(this.Name, 20) + " " + StatusDownload + " " + StatusActive + " " + DopStatus;
        }

        string ConvertName(string name, int maxSize) 
        {
            if (name.Length <= maxSize)
            {
                return name;
            }
            else 
            {
                return name.Substring(0, maxSize);
            }
        }
    }
}