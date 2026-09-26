namespace Bank.UI
{
    //There are a few mistakes in this class
    //Figure out what those mistakes are using testing
    public class CheckingAccount : BankAccount
    {
        public CheckingAccount(string owner, double interestRate, double startingBalance = 0)
            : base(owner, interestRate, startingBalance) 
        {
        }

        public override void Withdraw(double amount)
        {
            _balance -= amount;
        }

        public override string ToString()
        {
            return "Checkingaccount as text";
        }
    }
}
