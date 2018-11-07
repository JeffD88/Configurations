﻿namespace Configurations.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    using Configurations.Commands;
    using Configurations.Services;

    using Mastercam.Database;
    using Mastercam.Database.Types;
    using Mastercam.IO;
    using Mastercam.Operations;
    using Mastercam.Support;
    using Mastercam.Support.Events;

    public class MainViewViewModel : INotifyPropertyChanged
    {
        #region Private Fields

        private int configurationNumber;

        private int selectedConfiguration;

        private List<int> configurations = null;

        private Operation[] allOperations = null;

        private Operation[] selectedOperations = null;

        private readonly Window view;

        private readonly IConfigurationService configurationService;

        #endregion

        #region Construction

        public MainViewViewModel(Window view, IConfigurationService configurationService)
        {
            this.view = view;
            this.configurationService = configurationService;

            this.SelectOperationsCommand = new DelegateCommand(this.OnSelectOperationsCommand);
            this.SetConfigurationCommand = new DelegateCommand(this.OnSetConfigurationCommand);

            this.ConfigurationNumber = 1;

            this.allOperations = SearchManager.GetOperations();

            if (allOperations.Any())
                this.Configurations = configurationService.GetConfigurations(allOperations);

        }

        #endregion

        #region Commands

        public ICommand SelectOperationsCommand { get; }
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

        public List<int> Configurations
        {
            get => this.configurations;

            set
            {
                this.configurations = value;
                OnPropertyChanged(nameof(this.Configurations));
            }
        }

        #endregion

        #region Private Methods

        private void OnSelectOperationsCommand(object parameter)
        {
            this.view.Hide();
            using (var uiEvent = new McUIEvent())
            {
                var eventData = new UIEventData();
                var isEnter = false;
                while (!isEnter)
                {
                    uiEvent.GetEvent($"Select operations to add to Configuration {ConfigurationNumber}.{Environment.NewLine}" +
                                     $"Press Enter when done.",
                                     eventData,
                                     UIEventType.Key,
                                     new GeometryMask(),
                                     new SelectionMask()
                                    );
                    if (eventData.Key == 13)
                        isEnter = true;
                }               
            }
            PromptManager.Clear();

            this.selectedOperations = SearchManager.GetOperations(true);
            if (selectedOperations.Any())
            {
                configurationService.AddToConfiguration(ConfigurationNumber, selectedOperations);

                this.allOperations = SearchManager.GetOperations();
                this.Configurations = configurationService.GetConfigurations(allOperations);
            }
            else
                DialogManager.Error("No Operations Selected", "No Operations Selected");
            this.view.ShowDialog();
        }

        private void OnSetConfigurationCommand(object parameter)
        {
            configurationService.SetPosting(SelectedConfiguration, allOperations);
            this.view?.Close();
        }

        #endregion

        #region Notify Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion
    }
}