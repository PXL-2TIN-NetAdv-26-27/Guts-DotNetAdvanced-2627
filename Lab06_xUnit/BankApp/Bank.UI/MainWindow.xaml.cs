using System;
using System.Globalization;
using System.Windows;

namespace Bank.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UpdateAccountDisplay(_checkingAccount, CheckingBalanceText, CheckingRateText);
            UpdateAccountDisplay(_savingsAccount, SavingsBalanceText, SavingsRateText);
        }

        private readonly CheckingAccount _checkingAccount = new("Sam", 0.05, 200);
        private readonly SavingsAccount _savingsAccount = new("Sam", 0.1, 200);

        private void CheckingDeposit_Click(object sender, RoutedEventArgs e) => ApplyAmount(_checkingAccount, CheckingAmountTextBox, CheckingStatusText, isDeposit: true);

        private void CheckingWithdraw_Click(object sender, RoutedEventArgs e) => ApplyAmount(_checkingAccount, CheckingAmountTextBox, CheckingStatusText, isDeposit: false);

        private void CheckingInterest_Click(object sender, RoutedEventArgs e) => ApplyInterest(_checkingAccount, CheckingStatusText);

        private void SavingsDeposit_Click(object sender, RoutedEventArgs e) => ApplyAmount(_savingsAccount, SavingsAmountTextBox, SavingsStatusText, isDeposit: true);

        private void SavingsWithdraw_Click(object sender, RoutedEventArgs e) => ApplyAmount(_savingsAccount, SavingsAmountTextBox, SavingsStatusText, isDeposit: false);

        private void SavingsInterest_Click(object sender, RoutedEventArgs e) => ApplyInterest(_savingsAccount, SavingsStatusText);

        private void ApplyAmount(BankAccount account, System.Windows.Controls.TextBox amountTextBox, System.Windows.Controls.TextBlock statusText, bool isDeposit)
        {
            if (!double.TryParse(amountTextBox.Text, NumberStyles.Float, CultureInfo.CurrentCulture, out double amount))
            {
                statusText.Text = "Enter a valid amount.";
                return;
            }

            try
            {
                if (isDeposit)
                    account.Deposit(amount);
                else
                    account.Withdraw(amount);

                statusText.Text = string.Empty;
                UpdateAccountDisplay(account,
                    account is CheckingAccount ? CheckingBalanceText : SavingsBalanceText,
                    account is CheckingAccount ? CheckingRateText : SavingsRateText);
            }
            catch (Exception exception)
            {
                statusText.Text = exception.Message;
            }
        }

        private void ApplyInterest(BankAccount account, System.Windows.Controls.TextBlock statusText)
        {
            account.AddInterests();
            statusText.Text = string.Empty;
            UpdateAccountDisplay(account,
                account is CheckingAccount ? CheckingBalanceText : SavingsBalanceText,
                account is CheckingAccount ? CheckingRateText : SavingsRateText);
        }

        private static void UpdateAccountDisplay(BankAccount account, System.Windows.Controls.TextBlock balanceText, System.Windows.Controls.TextBlock rateText)
        {
            balanceText.Text = $"{account.Balance:N2} euro";
            rateText.Text = $"Interest rate: {account.InterestRate:P2}";
        }
    }
}
