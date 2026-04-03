using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ShirobokovaPartnerLib.Models;
using ShirobokovaPartnerLib.Services;

namespace ShirobokovaPartnerApp
{
    public partial class SalesHistoryWindow : Window
    {
        private readonly PartnerService _partnerService;
        private readonly Partner _partner;

        public SalesHistoryWindow(PartnerService partnerService, Partner partner)
        {
            InitializeComponent();
            _partnerService = partnerService;
            _partner = partner;

            PartnerNameText.Text = $"Партнёр: {partner.Name}";
            Title = $"История продаж - {partner.Name} - Широбокова А.М.";

            LoadSalesHistory();
        }

        /// <summary>
        /// Загрузка истории продаж
        /// </summary>
        private async void LoadSalesHistory()
        {
            try
            {
                Cursor = Cursors.Wait;

                var sales = await _partnerService.GetSalesHistoryAsync(_partner.Id);
                SalesGrid.ItemsSource = sales;

                var total = 0m;
                foreach (var sale in sales)
                {
                    total += sale.TotalAmount;
                }

                TotalSalesText.Text = $"Общая сумма продаж: {total:N2} руб.";

                Cursor = Cursors.Arrow;

                if (sales.Count == 0)
                {
                    TotalSalesText.Text += "\n(Продажи отсутствуют)";
                }
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Arrow;
                MessageBox.Show($"Ошибка загрузки истории продаж: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}