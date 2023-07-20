using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication6
{
    public class ServicesWork
    {
        ObservableCollection<ServiceInfo> processes = new ObservableCollection<ServiceInfo>();

        public async Task<ObservableCollection<ServiceInfo>> GetProcesses()
        {
            var processStartInfo = new ProcessStartInfo("systemctl", "--no-pager --no-legend list-units --all")
            {
                RedirectStandardOutput = true
            };

            var proc = new Process
            {
                StartInfo = processStartInfo
            };

            proc.Start();

            int i = 0;

            while (!proc.StandardOutput.EndOfStream)
            {
                var output = proc.StandardOutput.ReadLine();

                var processFields = output.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Пока не дошло как тут можно Split использовать

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
                    dop = processFields[4];
                }

                processes.Add(new ServiceInfo
                {
                    Name = name,
                    StatusDownload = Parsing(download),
                    StatusActive = Parsing(active),
                    DopStatus = Parsing(dop),

                    Id = i
                });

                i++;
            }

            await proc.WaitForExitAsync();

            return processes;
        }

        void SystemctlStartProcess(string arguments)
        {
            var processStartInfo = new ProcessStartInfo("systemctl")
            {
                Arguments = arguments,
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

        public void ActivateServices(string nameService)
        {
            SystemctlStartProcess($"start {nameService}");
        }

        public void InactivateServices(string nameService)
        {
            SystemctlStartProcess($"stop {nameService}");
        }

        // Самому подобно реализация кажется черезчур сложной
        // и видимо есть способ упростить данный код, но я его
        // пока не нашел
        Status Parsing(string nameStatus) 
        {
            switch (nameStatus) 
            {
                case "loaded":
                    return Status.loaded;

                case "not-found":
                    return Status.not_found;

                case "active":
                    return Status.active;

                case "inactive":
                    return Status.inactive;

                case "failed":
                    return Status.failed;

                case "running":
                    return Status.running;

                case "plugged":
                    return Status.plugged;

                case "mounted":
                    return Status.mounted;

                case "dead":
                    return Status.dead;

                case "waiting":
                    return Status.waiting;

                case "exited":
                    return Status.exited;

                case "listening":
                    return Status.inactive;

                default:
                    return Status.failed;

            }
        }
    }
}