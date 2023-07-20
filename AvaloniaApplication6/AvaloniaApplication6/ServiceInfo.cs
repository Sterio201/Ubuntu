using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication6
{
    public class ServiceInfo
    {
        public string? Name { get; set; }
        public Status? StatusDownload { get; set; }
        public Status? StatusActive { get; set; }
        public Status? DopStatus { get; set; }

        public int Id { get; set; }

        public override string ToString()
        {
            return Name + " " + StatusDownload + " " + StatusActive + " " + DopStatus;
        }
    }

    public enum Status 
    {
        // Варианты статуса загрузки
        loaded,
        not_found,

        // Варианты статуса активности
        active,
        inactive,
        failed,

        // Варианты допстатуса
        running,
        plugged,
        mounted,
        dead,
        waiting,
        exited,
        listening
    }
}
