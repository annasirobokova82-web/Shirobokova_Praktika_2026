using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ShirobokovaPartnerLib.Models;
using ShirobokovaPartnerLib.Services;

namespace ShirobokovaPartnerApp
{
    public partial class PartnerEditWindow : Window
    {
        private readonly PartnerService _partnerService;
        private readonly Partner? _editingPartner;
        private bool _isEditMode;

        public PartnerEditWindow(PartnerService partnerService, Partner? partner = null)
        {
            InitializeComponent();
            _partnerService = partnerService;
            _editingPartner = partner;
            _isEditMode = partner != null;

            LoadPartnerTypes();
            if (_isEditMode)
            {
                Title = "Редактирование партнёра - Широбокова А.М.";
                LoadPartnerData();
            }
            else
            {
                Title = "Добавление партнёра - Широбокова А.М.";
            }
        }

        /// <summary>
        /// Загрузка типов партнёров
        /// </summary>
        private async void LoadPartnerTypes()
        {
            try
            {
                var types = await _partnerService.GetAllPartnerTypesAsync();
                CmbPartnerType.ItemsSource = types;
                if (types.Any())
                {
                    CmbPartnerType.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки типов партнёров: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Загрузка данных партнёра для редактирования
        /// </summary>
        private void LoadPartnerData()
        {
            if (_editingPartner == null) return;

            TxtName.Text = _editingPartner.Name;
            TxtRating.Text = _editingPartner.Rating.ToString();
            TxtAddress.Text = _editingPartner.Address;
            TxtDirectorName.Text = _editingPartner.DirectorName;
            TxtPhone.Text = _editingPartner.Phone;
            TxtEmail.Text = _editingPartner.Email;

            // Выбор типа партнёра
            if (CmbPartnerType.ItemsSource is System.Collections.IList list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var type = list[i] as PartnerType;
                    if (type?.Id == _editingPartner.PartnerTypeId)
                    {
                        CmbPartnerType.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Валидация введённых данных
        /// </summary>
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Наименование партнёра обязательно для заполнения",
                    "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtName.Focus();
                return false;
            }

            if (CmbPartnerType.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип партнёра",
                    "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                CmbPartnerType.Focus();
                return false;
            }

            if (!int.TryParse(TxtRating.Text, out int rating) || rating < 0)
            {
                MessageBox.Show("Рейтинг должен быть целым неотрицательным числом",
                    "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtRating.Focus();
                return false;
            }

            // Валидация телефона (простая)
            if (!string.IsNullOrWhiteSpace(TxtPhone.Text))
            {
                var phonePattern = @"^[\+\(]?\d[\d\s\-\(\)]+$";
                if (!Regex.IsMatch(TxtPhone.Text, phonePattern))
                {
                    var result = MessageBox.Show("Телефон имеет нестандартный формат. Продолжить?",
                        "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result != MessageBoxResult.Yes)
                    {
                        TxtPhone.Focus();
                        return false;
                    }
                }
            }

            // Валидация email
            if (!string.IsNullOrWhiteSpace(TxtEmail.Text))
            {
                var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(TxtEmail.Text, emailPattern))
                {
                    MessageBox.Show("Email имеет неверный формат. Пример: name@domain.com",
                        "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtEmail.Focus();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Сохранение партнёра
        /// </summary>
        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                Cursor = Cursors.Wait;

                var partner = _isEditMode ? _editingPartner! : new Partner();
                partner.PartnerTypeId = ((PartnerType)CmbPartnerType.SelectedItem).Id;
                partner.Name = TxtName.Text.Trim();
                partner.Rating = int.Parse(TxtRating.Text);
                partner.Address = TxtAddress.Text?.Trim();
                partner.DirectorName = TxtDirectorName.Text?.Trim();
                partner.Phone = TxtPhone.Text?.Trim();
                partner.Email = TxtEmail.Text?.Trim();

                bool success;
                if (_isEditMode)
                {
                    success = await _partnerService.UpdatePartnerAsync(partner);
                }
                else
                {
                    success = await _partnerService.AddPartnerAsync(partner);
                }

                Cursor = Cursors.Arrow;

                if (success)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Не удалось сохранить данные партнёра",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Arrow;
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Отмена
        /// </summary>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Валидация ввода чисел (для поля рейтинга)
        /// </summary>
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }
    }
}