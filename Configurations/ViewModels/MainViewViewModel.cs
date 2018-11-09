namespace Configurations.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    using Configurations.Commands;
    using Configurations.Services;

    using Mastercam.Database.Types;
    using Mastercam.IO;
    using Mastercam.Support;
    using Mastercam.Support.Events;

    public class MainViewViewModel : INotifyPropertyChanged
    {
        #region Private Fields

        private int configurationNumber;

        private int selectedConfiguration;

        private int index;

        private List<int> configurations = null;

        private bool hasConfigurations;

        private readonly Window view;

        private readonly IConfigurationService configurationService;

        #endregion

        #region Construction

        public MainViewViewModel(Window view, IConfigurationService configurationService)
        {
            this.view = view;
            this.configurationService = configurationService;

            this.SelectOperationsCommand = new DelegateCommand(this.OnSelectOperationsCommand);
            this.PostConfigurationCommand = new DelegateCommand(this.OnPostConfigurationCommand);
            this.SetConfigurationCommand = new DelegateCommand(this.OnSetConfigurationCommand);

            this.ConfigurationNumber = 1;
            this.Configurations = configurationService.GetConfigurations();

            if (Configurations.Any())
            {
                HasConfigurations = true;
                Index = 0;
            }
            else
                HasConfigurations = false;
        }

        #endregion

        #region Commands

        public ICommand SelectOperationsCommand { get; }

        public ICommand PostConfigurationCommand { get; }

        public ICommand SetConfigurationCommand { get; }

        #endregion

        #region Public Properties

        public int ConfigurationNumber
        {
            get => this.configurationNumber;

            set
            {
                this.configurationNumber = value;
                OnPropertyChanged(nameof(this.ConfigurationNumber));
            }
        }

        public int SelectedConfiguration
        {
            get => this.selectedConfiguration;

            set
            {
                this.selectedConfiguration = value;
                OnPropertyChanged(nameof(this.SelectedConfiguration));
            }
        }

        public int Index
        {
            get => this.index;

            set
            {
                this.index = value;
                OnPropertyChanged(nameof(this.Index));
            }
        }

        public List<int> Configurations
        {
            get => this.configurations;

            set
            {
                this.configurations = value;
                OnPropertyChanged(nameof(this.Configurations));
            }
        }

        public bool HasConfigurations
        {
            get => this.hasConfigurations;

            set
            {
                this.hasConfigurations = value;
                OnPropertyChanged(nameof(this.HasConfigurations));
            }
        }

        #endregion

        #region Private Methods

        private void OnSelectOperationsCommand(object parameter)
        {
            this.view.Hide();

            if (OperationsSelected())
            {
                var selectedOperations = SearchManager.GetOperations(true);
                if (selectedOperations.Any())
                {
                    configurationService.AddToConfiguration(ConfigurationNumber, selectedOperations);

                    this.Configurations = configurationService.GetConfigurations();

                    if (Configurations.Any())
                    {
                        HasConfigurations = true;
                        Index = 0;
                    }
                    else
                        HasConfigurations = false;
                }
                else
                    DialogManager.Error("No operations selected", "No Operations Selected");
            }

            PromptManager.Clear();
            this.view.ShowDialog();
        }
    
        private void OnPostConfigurationCommand(object parameter)
        {

            configurationService.PostConfiguration(SelectedConfiguration);
            this.view?.Close();

        }
        
        private void OnSetConfigurationCommand(object parameter)
        {
            configurationService.SetPosting(SelectedConfiguration);
            this.view?.Close();
        }

        private bool OperationsSelected()
        {
            var eventData = new UIEventData();
            McUIEvent uiEvent = null;

            while (true)
            {
                uiEvent = new McUIEvent();

                uiEvent.GetEvent($"Select operations to add to Configuration {ConfigurationNumber}.{Environment.NewLine}" +
                                 $"Press Enter when done.",
                                 eventData,
                                 UIEventType.Key,
                                 new GeometryMask(false),
                                 new SelectionMask(false)
                                );

                if (eventData.Key == 13)
                    return true;

                if (eventData.Key == 0)
                    return false;

                eventData.Key = 0;
                uiEvent.Dispose();
            }
            
        }

        #endregion

        #region Notify Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion
    }
}