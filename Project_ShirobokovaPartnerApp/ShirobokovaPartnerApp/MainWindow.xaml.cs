using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ShirobokovaPartnerLib.Models;
using ShirobokovaPartnerLib.Services;

namespace ShirobokovaPartnerApp
{
    public partial class MainWindow : Window
    {
        private readonly PartnerService _partnerService;
        private readonly IServiceProvider _serviceProvider;

        public MainWindow(PartnerService partnerService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _partnerService = partnerService;
            _serviceProvider = serviceProvider;
            LoadPartners();
        }

        private async void LoadPartners()
        {
            try
            {
                StatusText.Text = "Загрузка данных...";
                Cursor = Cursors.Wait;

                var partners = await _partnerService.GetAllPartnersAsync();
                PartnersGrid.ItemsSource = partners;

                StatusText.Text = $"Загружено партнёров: {partners.Count}";
                Cursor = Cursors.Arrow;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Arrow;
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "Ошибка загрузки данных";
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new PartnerEditWindow(_partnerService, null);
            editWindow.Owner = this;
            if (editWindow.ShowDialog() == true)
            {
                LoadPartners();
                StatusText.Text = "Партнёр успешно добавлен";
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadPartners();
        }

        private void PartnersGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (PartnersGrid.SelectedItem is Partner partner)
            {
                EditPartner(partner);
            }
        }

        private void EditPartner_Click(object sender, RoutedEventArgs e)
        {
            if (PartnersGrid.SelectedItem is Partner partner)
            {
                EditPartner(partner);
            }
        }

        private void EditPartner(Partner partner)
        {
            var editWindow = new PartnerEditWindow(_partnerService, partner);
            editWindow.Owner = this;
            if (editWindow.ShowDialog() == true)
            {
                LoadPartners();
                StatusText.Text = "Данные партнёра обновлены";
            }
        }

        private void ShowSalesHistory_Click(object sender, RoutedEventArgs e)
        {
            if (PartnersGrid.SelectedItem is Partner partner)
            {
                var historyWindow = new SalesHistoryWindow(_partnerService, partner);
                historyWindow.Owner = this;
                historyWindow.ShowDialog();
            }
        }

        private async void DeletePartner_Click(object sender, RoutedEventArgs e)
        {
            if (PartnersGrid.SelectedItem is not Partner partner) return;

            var result = MessageBox.Show($"Вы действительно хотите удалить партнёра \"{partner.Name}\"?\n\nВсе связанные продажи также будут удалены!",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                Cursor = Cursors.Wait;
                var success = await _partnerService.DeletePartnerAsync(partner.Id);
                Cursor = Cursors.Arrow;

                if (success)
                {
                    LoadPartners();
                    StatusText.Text = $"Партнёр \"{partner.Name}\" удалён";
                }
                else
                {
                    MessageBox.Show("Не удалось удалить партнёра", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Arrow;
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshList_Click(object sender, RoutedEventArgs e)
        {
            LoadPartners();
        }
    }
}