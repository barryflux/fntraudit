using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FntrAudit.Viewmodels;

namespace FntrAudit.Views
{
    public partial class ClientEditDialog : Window
    {
        public ClientEditDialog(CreateClientViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.RequestClose += OnRequestClose;
        }

        private void OnRequestClose(bool dialogResult)
        {
            DialogResult = dialogResult;
            Close();
        }



        private void DeleteActivity_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not CreateClientViewModel vm)
                return;

            if (sender is not Button button)
                return;

            if (button.Tag is not ClientActivityRowViewModel activity)
                return;

            vm.DeleteActivity(activity);
        }

        private void AccordionExpander_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is not Expander currentExpander)
                return;

            var parentPanel = FindParent<StackPanel>(currentExpander);
            if (parentPanel == null)
                return;

            foreach (var child in parentPanel.Children)
            {
                if (child is Expander expander && !ReferenceEquals(expander, currentExpander))
                {
                    expander.IsExpanded = false;
                }
            }
        }

        private static T? FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject? parent = VisualTreeHelper.GetParent(child);

            while (parent != null)
            {
                if (parent is T typedParent)
                    return typedParent;

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }
    }
}