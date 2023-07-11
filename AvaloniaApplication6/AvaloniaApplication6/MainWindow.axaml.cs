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
        private TextBox textBox;
        private ListBox listBox;
        private Button searchButton;
        private Button deleteButton;
        private Button addButton;
        private Label warningLabel;

        List<OutputProcess> allProcesses;
        OutputProcess selectedProcess;

        public MainWindow()
        {
            InitializeComponent();

            textBox = this.Find<TextBox>("TextInt");
            listBox = this.Find<ListBox>("MainListBox");
            searchButton = this.Find<Button>("Search");
            deleteButton = this.Find<Button>("Delete");
            addButton = this.Find<Button>("Add");
            warningLabel = this.Find<Label>("Warning");

            listBox.SelectionChanged += MainListBox_SelectionChanged;
            searchButton.Click += SearchMethod;
            deleteButton.Click += DeleteClick;
            addButton.Click += AddClick;

            allProcesses = Services.GetProcesses();
            AddOutputProcesses(allProcesses);
        }

        private void SearchMethod(object? sender, RoutedEventArgs e)
        {
            string searchText = textBox.Text.ToLower(); // Получаем введенный текст и приводим к нижнему регистру

            listBox.Items.Clear(); // Очищаем ListBox перед добавлением новых элементов

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Если строка поиска пустая, отображаем весь список служб
                AddOutputProcesses(allProcesses);
            }
            else
            {
                // Фильтруем список служб по введенному тексту
                List<OutputProcess> filteredProcesses = allProcesses
                    .Where(p => p.Name.ToLower().StartsWith(searchText))
                    .ToList();

                AddOutputProcesses(filteredProcesses);
            }
        }

        private void DeleteClick(object? sender, RoutedEventArgs e)
        {
            listBox.Items.Clear();

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