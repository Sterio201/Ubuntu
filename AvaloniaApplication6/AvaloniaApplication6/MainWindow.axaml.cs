using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AvaloniaApplication6
{
    public partial class MainWindow : Window
    {
        // имена переменных с _
        private TextBox textBox;
        private ListBox listBox;
        private Button searchButton;
        private Button deleteButton;
        private Button addButton;
        private Label warningLabel;

        // лучше использовать ObservableCollection и очищать/заполнять её. Тогда ListBox будет сам
        // перерисовывать любые изменения
        List<OutputProcess> allProcesses;
        OutputProcess selectedProcess;

        public MainWindow()
        {
            InitializeComponent();

            // Avalonia генерирует поля для именованных элементов, поэтому можешь смело обращаться к
            // TextInt. и остальным элементам
            textBox = this.Find<TextBox>("TextInt");
            listBox = this.Find<ListBox>("MainListBox");
            searchButton = this.Find<Button>("Search");
            deleteButton = this.Find<Button>("Delete");
            addButton = this.Find<Button>("Add");
            warningLabel = this.Find<Label>("Warning");
            
            // MainListBox.SelectionChanged += ...
            listBox.SelectionChanged += MainListBox_SelectionChanged;
            searchButton.Click += SearchMethod;
            deleteButton.Click += DeleteClick;
            addButton.Click += AddClick;

            // долгоиграющее действие внутри конструктора - очень не хорошая вещь
            allProcesses = Services.GetProcesses();
            AddOutputProcesses(allProcesses);
        }

        private void SearchMethod(object? sender, RoutedEventArgs e)
        {
            // вмето двух .ToLower() мог бы использовать IgnoreCase поиск
            // и поправь кодировку комментариев. в C# весь код должен быть в UTF-8
            string searchText = textBox.Text.ToLower(); // �������� ��������� ����� � �������� � ������� ��������

            listBox.Items.Clear(); // ������� ListBox ����� ����������� ����� ���������

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // ���� ������ ������ ������, ���������� ���� ������ �����
                AddOutputProcesses(allProcesses);
            }
            else
            {
                // ��������� ������ ����� �� ���������� ������
                List<OutputProcess> filteredProcesses = allProcesses
                    // а если я хочу найти сервис по куску строки где-то посередине?
                    // лучше используй .Contains(searchText, StringComparison.Ordinal или InvariantCultureIgnoreCase)
                    .Where(p => p.Name.ToLower().StartsWith(searchText))
                    .ToList();

                AddOutputProcesses(filteredProcesses);
            }
        }

        // методы, обрабатывающие события, лучше называть OnDeleteClick или ProcessDeleteClick. Опять таки ради
        // лучшей семантики
        private void DeleteClick(object? sender, RoutedEventArgs e)
        {
            listBox.Items.Clear();

            // чтобы избежать ошибок с литералами строк, лучше статус парсить в какой-нибудь Enum и проверять затем
            // по значениям. Так ты сведешь человеческий фактор к минимуму и в случае обновлений/исправлений ошибок,
            // нужно будет исправить это только в 1 месте, там, где парсишь
            if (selectedProcess.StatusDownload != "not-found" && selectedProcess.StatusActive != "failed")
            {
                Services.DeleteProcess(selectedProcess.Name);

                selectedProcess.StatusActive = "inactive";
                allProcesses[selectedProcess.id] = selectedProcess;
            }
            else 
            {
                warningLabel.Content = "Error, service not found";
            }

            textBox.Text = "";

            AddOutputProcesses(allProcesses);
            deleteButton.IsEnabled = false;
        }

        private void AddClick(object? sender, RoutedEventArgs e)
        {
            listBox.Items.Clear();

            if (selectedProcess.StatusDownload != "not-found" && selectedProcess.StatusActive != "failed")
            {
                Services.CreateProcess(selectedProcess.Name);

                selectedProcess.StatusActive = "active";
                allProcesses[selectedProcess.id] = selectedProcess;
            }
            else if (selectedProcess.DopStatus == "death") 
            {
                // можно заменить на диалоги с ошибками
                warningLabel.Content = "Error, the service cannot be started";
            }
            else
            {
                warningLabel.Content = "Error, service not found";
            }

            textBox.Text = "";

            AddOutputProcesses(allProcesses);
            addButton.IsEnabled = false;
        }

        private void AddOutputProcesses(List<OutputProcess> processes)
        {
            foreach (OutputProcess process in processes)
            {
                listBox.Items.Add(process);
            }
        }

        private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // очень много делаешь вручную. Я бы перевел всё на свойства, которые понимают, когда кнопка должна быть
            // включена, а когда выключена 
            deleteButton.IsEnabled = false;
            addButton.IsEnabled = false;

            warningLabel.Content = "";

            if (listBox.SelectedItem != null)
            {
                selectedProcess = (OutputProcess)listBox.SelectedItem;
                if (selectedProcess.StatusActive == "active")
                {
                    deleteButton.IsEnabled = true;
                }
                else if (selectedProcess.StatusActive == "inactive") 
                {
                    addButton.IsEnabled = true;
                }
            }
            else
            {
                deleteButton.IsEnabled = false;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}