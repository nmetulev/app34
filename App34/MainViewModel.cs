using App34.Helpers.RoamingSettings;
using Microsoft.Toolkit.Graph.Providers;
using Microsoft.Toolkit.Graph.Providers.Uwp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace App34
{
    public class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        Action _action;
        public DelegateCommand(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return _action != null;
        }

        public void Execute(object parameter)
        {
            _action.Invoke();
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        const string NotesFileName = "my_notes.json";

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand SaveCommand { get; }

        private string _text;
        public string Text
        {
            get => _text;
            set => Set(ref _text, value);
        }

        private RoamingSettingsHelper _roamingSettings;

        public MainViewModel()
        {
            SaveCommand = new DelegateCommand(Save);

            ProviderManager.Instance.ProviderUpdated += OnProviderUpdated;
            ProviderManager.Instance.GlobalProvider = WindowsProvider.Create("2fc98686-0464-42a2-ae3e-7f45c8c8257d", new string[] { "User.Read", "Tasks.ReadWrite", "Files.ReadWrite" });
        }

        private async void Save()
        {
            if (_roamingSettings != null)
            {
                await _roamingSettings.SaveFileAsync(NotesFileName, Text);
            }
        }

        private void OnProviderUpdated(object sender, ProviderUpdatedEventArgs e)
        {
            if (sender is ProviderManager providerManager && providerManager.GlobalProvider?.State == ProviderState.SignedIn)
            {
                Load();
            }
            else
            {
                Clear();
            }
        }

        private async void Load()
        {
            _roamingSettings = await RoamingSettingsHelper.CreateForCurrentUser(RoamingDataStore.OneDrive);

            bool notesExist = await _roamingSettings.FileExistsAsync(NotesFileName);
            if (notesExist)
            {
                Text = await _roamingSettings.ReadFileAsync(NotesFileName, string.Empty);
            }
            else
            {
                Text = string.Empty;
            }
        }

        private void Clear()
        {
            _roamingSettings = null;
            _text = null;
        }

        private void Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
