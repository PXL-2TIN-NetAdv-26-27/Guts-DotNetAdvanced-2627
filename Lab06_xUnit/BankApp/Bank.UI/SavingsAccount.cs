namespace Bank.UI
{
    public class SavingsAccount : BankAccount
    {
        public SavingsAccount(string owner, double interestRate, double startingBalance = 0) : base(owner, interestRate, startingBalance)
        {
        }

        public override void Withdraw(double amount)
        {
            _balance -= amount;
        }

        public override void AddInterests()
        {
            _balance = _balance + _interestRate;
        }

        public override string ToString()
        {
            return "Savingsaccount as text";
        }
    }
}
