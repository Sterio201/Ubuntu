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
    // static-классы - плохой тон, кроме случаев, где они действительно нужны 
    public static class Services
    {
        // отдаешь наверх list с элементами
        // я бы завернул это всё в обычный нестатический класс и организовал всю работу со службами внутри, отдавая
        // во вне только массив или то, что потребует пользователь класса
        public static List<OutputProcess> GetProcesses()
        {
            // не забывай про ключевое слово var, удобно же
            List<OutputProcess> processes = new List<OutputProcess>();

            // используешь больше аргументов самого systemctl, хвалю :)
            var processStartInfo = new ProcessStartInfo("systemctl", "--no-pager --no-legend list-units --all")
            {
                RedirectStandardOutput = true
            };

            var proc = new Process
            {
                StartInfo = processStartInfo
            };

            proc.Start();

            
            // мог бы использовать for и .Split('\n')
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

                // мог бы просто убирать "● " с помощью .Replace() метода. Было бы меньше кода 
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

            // было бы здорово, если б этот и другие методы были async
            proc.WaitForExit();

            return processes;
        }

        
        // название метода не описывает что он делает
        // внутри у тебя start, а метод называется create.
        // тобишь неверная семантика 
        // и ты не процесс запускаешь, а службу, так что метод должен называться StartService 
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

        // и здесь, лучше назвать StopService
        public static void DeleteProcess(string nameProcess) 
        {
            // много кода повторяется с методами сверху
            // мог бы вынести запуск процесса в отдельный метод, и тогда бы выглядело, например так:
            // var output = await Execute("systemctl", $"stop {nameprocess}"); и т.д.
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

    // 2 класса внутри файла лучше не создавать. Неудобно искать. Java, например, вообще считает это ошибкой компиляции.
    // разнеси это в разные классы
    
    // также вопрос к имени класса. По названию непонятно, за что отвечает этот класс. Он больше похож на информацию
    // о службе. Дучще его назвать соответствующе: ServiceInfo или что-то подобное
    public class OutputProcess : INotifyPropertyChanged // смешал логику для отображения и бизнес-логику,
                                                        // лучше их разделять, почитай про паттерн MVVM,
                                                        // ты ему не следуешь, но в целом полезно про него знать.
                                                        // он касается не только GUI приложений
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string? Name { get; set; }
        // Что такое StatusDownload? systemctl отдает LOAD, тобишь загружен. Не в смысле скачан из интернета)) А именно
        // что файл службы был загружен и система о нём знает.
        public string? StatusDownload { get; set; }

        // по общим договорённостям, имена приватных полей должны начинаться с _
        // _statusActive
        private string statusActive;
        public string? StatusActive 
        {
            get { return statusActive; }
            set
            {
                // странности какие-то происходят) с помощью ReactiveUi (включен в Avalonia) мог бы использовать метод
                // this.RaiseAndSetIfChanged(ref _statusActive, value);
                if (statusActive != value)
                {
                    statusActive = value;
                    OnPropertyChanged(nameof(StatusActive));
                }
            }
        }
        
        // DopStatus - тоже непонятно за что отвечает свойство. Нужно лезть выше по коду, вчитываться, проверять вывод
        // команды systemctl и т.д.
        public string? DopStatus { get; set; }

        // имя публичного поля (непонятное почему ты отказался от свойства) должно начинаться с большой буквы
        public int id;

        public override string ToString()
        {
            // не забывай про интерполяция строк:
            // $"{ConvertName(Name, 20)} {StatusDownload} {StatusActive} {DopStatus}"
            return ConvertName(this.Name, 20) + " " + StatusDownload + " " + StatusActive + " " + DopStatus;
        }

        // если правильно понял, метод ограничивает имя службы до n-ого количества символов? 
        // такие штуки нужны исключительно для UI, поэтому это лучше выносить в класс, связанный с отображением
        // также не забывай указывать модификатор доступа private. Это хороший тон
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