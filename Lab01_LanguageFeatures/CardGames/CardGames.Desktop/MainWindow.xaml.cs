using System.Windows;
using CardGames.Domain;

namespace CardGames.Desktop
{
    public partial class MainWindow : Window
    {
        private readonly HigherLowerGame _game;

        public MainWindow()
        {
            InitializeComponent();
            _game = new HigherLowerGame(new CardDeck() as ICardDeck, 3, CardRank.Five);

            UpdateWindow();
        }

        private void HigherButton_Click(object sender, RoutedEventArgs e)
        {
            _game.MakeGuess(higher:true);
            UpdateWindow();
        }

        private void LowerButton_Click(object sender, RoutedEventArgs e)
        {
            _game.MakeGuess(higher: false);
            UpdateWindow();
        }

        private void UpdateWindow()
        {
            CurrentCardTextBlock.Text = _game.CurrentCard.ToString();
            if (_game.PreviousCard is not null)
            {
                PreviousCardTextBlock.Text = _game.PreviousCard.ToString();
            }

            if (_game.HasWon)
            {
                MessageTextBlock.Text = "You won!";
            }
            else if (!string.IsNullOrEmpty(_game.Motivation))
            {
                MessageTextBlock.Text = _game.Motivation;
            }
            else
            {
                MessageTextBlock.Text = "";
            }
        }
    }
}
