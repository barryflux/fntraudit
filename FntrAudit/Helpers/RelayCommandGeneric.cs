using System;
using System.Windows.Input;

namespace FntrAudit.Helpers;

public sealed class RelayCommand<T> : ICommand
{
    private readonly Action<T?> execute;

    public RelayCommand(Action<T?> execute)
    {
        this.execute = execute;
    }

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        execute(parameter is T value ? value : default);
    }

    public event EventHandler? CanExecuteChanged;
}