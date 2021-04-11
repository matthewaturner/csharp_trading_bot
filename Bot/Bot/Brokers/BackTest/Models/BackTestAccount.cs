
using Bot.Models.Interfaces;

namespace Bot.Brokers.BackTest.Models
{
    public class BackTestAccount : IAccount
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public BackTestAccount()
        {
            AccountId = "backtest-account";
            CashBalance = 0;
            BuyingPower = 0;
            TotalValue = 0;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="initialCash"></param>
        public BackTestAccount(double initialCash)
        {
            AccountId = "backtest-account";
            CashBalance = initialCash;
            BuyingPower = initialCash;
            TotalValue = initialCash;
        }

        /// <summary>
        /// Id of the account;
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Cash balance.
        /// </summary>
        public double CashBalance { get; set; }

        /// <summary>
        /// Buying power.
        /// </summary>
        public double BuyingPower { get; set; }

        /// <summary>
        /// Total value of the account.
        /// </summary>
        public double TotalValue { get; set; }
    }
}
