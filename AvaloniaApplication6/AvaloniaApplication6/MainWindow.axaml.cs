using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace AvaloniaApplication6
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<ServiceInfo> _allProcesses;
        public ServiceInfo _selectedService;
        public ServicesWork _serviceWorsk;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            MainListBox.SelectionChanged += MainListBox_SelectionChanged;
            Search.Click += SearchMethod;
            Delete.Click += OnDeleteClick;
            Add.Click += AddClick;

            _allProcesses = new ObservableCollection<ServiceInfo>();
            _serviceWorsk = new ServicesWork();
        }

        private void SearchMethod(object? sender, RoutedEventArgs e)
        {
            string searchText = TextInt.Text;

            MainListBox.Items.Clear();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                AddOutputProcesses(_allProcesses);
            }
            else
            {
                ObservableCollection<ServiceInfo> filteredProcesses = new ObservableCollection<ServiceInfo>
                    (_allProcesses.Where(p => p.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)));

                AddOutputProcesses(filteredProcesses);
            }
        }

        private void OnDeleteClick(object? sender, RoutedEventArgs e)
        {
            MainListBox.Items.Clear();

            if (_selectedService.StatusDownload != Status.not_found && _selectedService.StatusActive != Status.failed)
            {
                _serviceWorsk.InactivateServices(_selectedService.Name);

                _selectedService.StatusActive = Status.inactive;
                _allProcesses[_selectedService.Id] = _selectedService;
            }
            else
            {
                Warning.Content = "Error, service not found";
            }

            TextInt.Text = "";

            AddOutputProcesses(_allProcesses);
            Delete.IsEnabled = false;
        }

        private void AddClick(object? sender, RoutedEventArgs e)
        {
            MainListBox.Items.Clear();

            if (_selectedService.StatusDownload != Status.not_found && _selectedService.StatusActive != Status.failed)
            {
                _serviceWorsk.ActivateServices(_selectedService.Name);

                _selectedService.StatusActive = Status.active;
                _allProcesses[_selectedService.Id] = _selectedService;
            }
            else if (_selectedService.DopStatus == Status.dead)
            {
                Warning.Content = "Error, the service cannot be started";
            }
            else
            {
                Warning.Content = "Error, service not found";
            }

            TextInt.Text = "";

            AddOutputProcesses(_allProcesses);
            Add.IsEnabled = false;
        }

        public void AddOutputProcesses(ObservableCollection<ServiceInfo> processes)
        {
            foreach (ServiceInfo process in processes)
            {
                process.Name = ConvertName(process.Name, 20);
                MainListBox.Items.Add(process);
            }
        }

        private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Delete.IsEnabled = false; Add.IsEnabled = false;

            Warning.Content = "";

            if (MainListBox.SelectedItem != null)
            {
                _selectedService = (ServiceInfo)MainListBox.SelectedItem;
                if (_selectedService.StatusActive == Status.active)
                {
                    Delete.IsEnabled = true;
                }
                else if (_selectedService.StatusActive == Status.inactive)
                {
                    Add.IsEnabled = true;
                }
            }
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