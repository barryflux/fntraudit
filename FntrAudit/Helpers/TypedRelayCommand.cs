using System;
using System.Windows.Input;

namespace FntrAudit.Helpers;

public sealed class TypedRelayCommand<T> : ICommand
{
    private readonly Action<T?> execute;

    public TypedRelayCommand(Action<T?> execute)
    {
        this.execute = execute;
    }

    public bool CanExecute(object? parameter) => parameter is null || parameter is T;

    public void Execute(object? parameter)
    {
        execute(parameter is T value ? value : default);
    }

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
}