using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Notifications.Wpf;
using System.Collections.ObjectModel;
using System.Windows;
using XmlHandler.Model;
using XmlHandler.Services.Interfaces;
using XmlHandler.Util;
using XmlHandler.Util.Enums;

namespace XmlHandler.ViewModel;

public interface IMainViewModel
{
    void OnSelectedCellChanged();
}

public class MainViewModel : BaseViewModel, IMainViewModel
{
    private readonly IXmlHandlerService _xmlHandlerServce;
    private readonly INotificationService _notificationService;
    private readonly ILogger<MainViewModel> _logger;

    private UserInformation? _userIformation;

    private ObservableCollection<User> _users;
    public ObservableCollection<User> Users
    {
        get => _users;
        set => SetProperty(ref _users, value);
    }


    private bool _isLoaded = false;
    public bool IsLoaded
    {
        get => _isLoaded;
        set => SetProperty(ref _isLoaded, value);
    }

    public IAsyncRelayCommand OpenConfigCommand { get; private set; }
    public AsyncRelayCommand SaveCommand { get; private set; }
    public IRelayCommand AddUserCommand { get; }
    public IRelayCommand DeleteSelectedCommand { get; }

    public MainViewModel(IXmlHandlerService xmlHandlerService,
        ILogger<MainViewModel> logger,
        INotificationService notificationService)
    {
        _xmlHandlerServce = xmlHandlerService;
        _notificationService = notificationService;
        _logger = logger;
        
        _users = new ObservableCollection<User>();
        OpenConfigCommand = new AsyncRelayCommand(LoadConfig);
        SaveCommand = new AsyncRelayCommand(Save, CanSave);
        AddUserCommand = new RelayCommand(AddUser);
        DeleteSelectedCommand = new RelayCommand(DeleteSelected, CanDelete);
        PropertyChanged += (_, __) =>
        {
            NotifyCanExecuteChanged();
        };
    }

    public void OnSelectedCellChanged()
    {
        NotifyCanExecuteChanged();
    }

    private async Task Save()
    {
        try
        {
            IsBusy = true;
            if(_userIformation == null)
            {
                _notificationService.Show("Error", "Could not save the values!", NotificationType.Error);
                return;
            }
           
            _userIformation.Users = [.. _users];
            bool result = await _xmlHandlerServce.SaveUserInformation(_userIformation).ConfigureAwait(false);
            if (!result)
            {
                _notificationService.Show("Error", "Could not save the user information!", NotificationType.Error);
                return;
            }
            _notificationService.Show("Info", "Userinformation Saved!", NotificationType.Success);
            await LoadConfig().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        finally
        {
            await Application.Current.Dispatcher.InvokeAsync(() => IsBusy = false);
        }
    }

    private void AddUser()
    {
        Users.Add(new User
        {
            FirstName = "",
            LastName = "",
            CivilState = (int)CivilState.Single
        });
        NotifyCanExecuteChanged();
    }

    private void DeleteSelected()
    {
        var selectedUsers = _users.Where(x => x.IsSelected);
        if (selectedUsers.Any())
        {
            if (MessageBox.Show("Are you sure that you want to delete the selected row(s)?",
                                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                foreach (var user in selectedUsers.ToList())
                    Users.Remove(user);
            }
        }
    }

    private async Task LoadConfig()
    {
        try
        {
            IsBusy = true;
            bool result = await _xmlHandlerServce.EnsureConfigFileExists().ConfigureAwait(false);
            if (!result)
            {
                _notificationService.Show("Error", "Could not find the file!", NotificationType.Error);

                if (MessageBox.Show("Could not find the file, do you want to select it manually?",
                                "Confirm Select", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    
                    return;
                }
                var path = OpenFileHelper.OpenFile();
                if(path is null)
                {
                    _notificationService.Show("Error", "Could not find the file!", NotificationType.Error);
                    return;
                }

                _userIformation = await _xmlHandlerServce.LoadUserInformation(path).ConfigureAwait(false);
            }
            else
            {
                _userIformation = await _xmlHandlerServce.LoadUserInformation().ConfigureAwait(false);
            }

            if(_userIformation == null)
            {
                _notificationService.Show("Error", "Could not read the file, users were null!", NotificationType.Error);
                return;
            }
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Users = new ObservableCollection<User>(_userIformation.Users);
                IsLoaded = true;
                _notificationService.Show("Info", "Userinformation Loaded!", NotificationType.Success);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        finally
        {
            await Application.Current.Dispatcher.InvokeAsync(() => IsBusy = false);
        }
    }

    private void NotifyCanExecuteChanged()
    {
        OpenConfigCommand.NotifyCanExecuteChanged();
        SaveCommand.NotifyCanExecuteChanged();
        AddUserCommand.NotifyCanExecuteChanged();
        DeleteSelectedCommand.NotifyCanExecuteChanged();
    }

    private bool CanSave() => 
        !_users.Any(x => string.IsNullOrEmpty(x.FirstName) || 
        x.FirstName.Any(char.IsDigit) ||
        string.IsNullOrEmpty(x.LastName) ||
        x.LastName.Any(char.IsDigit) ||
        x.CivilState == 0);

    private bool CanDelete() => _users.Where(x => x.IsSelected).Any();
}
