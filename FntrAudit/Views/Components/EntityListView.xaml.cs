using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using FntrAudit.Viewmodels.Common;

namespace FntrAudit.Views.Components
{
    public partial class EntityListView : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private ICollectionView? _filteredItemsSource;
        public ICollectionView? FilteredItemsSource
        {
            get => _filteredItemsSource;
            private set
            {
                if (_filteredItemsSource == value) return;
                _filteredItemsSource = value;
                OnPropertyChanged(nameof(FilteredItemsSource));
                OnPropertyChanged(nameof(FilteredItemsCount));
            }
        }

        public int FilteredItemsCount => FilteredItemsSource?.Cast<object>().Count() ?? 0;

        public EntityListView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(EntityListView),
                new PropertyMetadata(null, OnItemsSourceChanged));

        public IEnumerable? ItemsSource
        {
            get => (IEnumerable?)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                nameof(SelectedItem),
                typeof(object),
                typeof(EntityListView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object? SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register(
                nameof(AddCommand),
                typeof(ICommand),
                typeof(EntityListView),
                new PropertyMetadata(null));

        public ICommand? AddCommand
        {
            get => (ICommand?)GetValue(AddCommandProperty);
            set => SetValue(AddCommandProperty, value);
        }

        public static readonly DependencyProperty EditItemCommandProperty =
            DependencyProperty.Register(
                nameof(EditItemCommand),
                typeof(ICommand),
                typeof(EntityListView),
                new PropertyMetadata(null));

        public ICommand? EditItemCommand
        {
            get => (ICommand?)GetValue(EditItemCommandProperty);
            set => SetValue(EditItemCommandProperty, value);
        }

        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register(
                nameof(EditCommand),
                typeof(ICommand),
                typeof(EntityListView),
                new PropertyMetadata(null, OnEditCommandChanged));

        public ICommand? EditCommand
        {
            get => (ICommand?)GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }

        private static void OnEditCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EntityListView view)
                view.EditItemCommand = e.NewValue as ICommand;
        }

        public static readonly DependencyProperty DeleteItemCommandProperty =
            DependencyProperty.Register(
                nameof(DeleteItemCommand),
                typeof(ICommand),
                typeof(EntityListView),
                new PropertyMetadata(null));

        public ICommand? DeleteItemCommand
        {
            get => (ICommand?)GetValue(DeleteItemCommandProperty);
            set => SetValue(DeleteItemCommandProperty, value);
        }

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register(
                nameof(DeleteCommand),
                typeof(ICommand),
                typeof(EntityListView),
                new PropertyMetadata(null, OnDeleteCommandChanged));

        public ICommand? DeleteCommand
        {
            get => (ICommand?)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }

        private static void OnDeleteCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EntityListView view)
                view.DeleteItemCommand = e.NewValue as ICommand;
        }

        public static readonly DependencyProperty ShowSearchBoxProperty =
            DependencyProperty.Register(
                nameof(ShowSearchBox),
                typeof(bool),
                typeof(EntityListView),
                new PropertyMetadata(true));

        public bool ShowSearchBox
        {
            get => (bool)GetValue(ShowSearchBoxProperty);
            set => SetValue(ShowSearchBoxProperty, value);
        }

        public static readonly DependencyProperty AddButtonTextProperty =
            DependencyProperty.Register(
                nameof(AddButtonText),
                typeof(string),
                typeof(EntityListView),
                new PropertyMetadata("Ajouter"));

        public string AddButtonText
        {
            get => (string)GetValue(AddButtonTextProperty);
            set => SetValue(AddButtonTextProperty, value);
        }

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register(
                nameof(SearchText),
                typeof(string),
                typeof(EntityListView),
                new PropertyMetadata(string.Empty, OnSearchTextChanged));

        public string SearchText
        {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not EntityListView view)
                return;

            if (e.OldValue is INotifyCollectionChanged oldCollection)
                oldCollection.CollectionChanged -= view.OnItemsCollectionChanged;

            if (e.NewValue is INotifyCollectionChanged newCollection)
                newCollection.CollectionChanged += view.OnItemsCollectionChanged;

            view.RefreshCollectionView();
        }

        private static void OnSearchTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not EntityListView view)
                return;

            view.FilteredItemsSource?.Refresh();
            view.OnPropertyChanged(nameof(FilteredItemsCount));
        }

        private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            FilteredItemsSource?.Refresh();
            OnPropertyChanged(nameof(FilteredItemsCount));
        }

        private void RefreshCollectionView()
        {
            if (ItemsSource == null)
            {
                FilteredItemsSource = null;
                return;
            }

            FilteredItemsSource = CollectionViewSource.GetDefaultView(ItemsSource);
            if (FilteredItemsSource != null)
            {
                FilteredItemsSource.Filter = FilterItem;
                FilteredItemsSource.Refresh();
            }

            OnPropertyChanged(nameof(FilteredItemsCount));
        }

        private bool FilterItem(object obj)
        {
            if (obj is not EntityRowViewModel row)
                return true;

            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            var search = SearchText.Trim();

            return Contains(row.Title, search) || Contains(row.Subtitle, search);
        }

        public static readonly DependencyProperty OpenItemCommandProperty =
            DependencyProperty.Register(
                nameof(OpenItemCommand),
                typeof(ICommand),
                typeof(EntityListView),
                new PropertyMetadata(null));

        public ICommand? OpenItemCommand
        {
            get => (ICommand?)GetValue(OpenItemCommandProperty);
            set => SetValue(OpenItemCommandProperty, value);
        }

        private static bool Contains(string? source, string search)
        {
            return !string.IsNullOrWhiteSpace(source) &&
                   source.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}