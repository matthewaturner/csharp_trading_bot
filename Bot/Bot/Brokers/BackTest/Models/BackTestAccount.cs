
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
            Cash = 0;
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
            Cash = initialCash;
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
        public double Cash { get; set; }

        /// <summary>
        /// Buying power.
        /// </summary>
        public double BuyingPower { get; set; }

        /// <summary>
        /// Total value of the account.
        /// </summary>
        public double TotalValue { get; set; }

        public override string ToString()
        {
            return $"AccountId: {AccountId}, Cash: {Cash}, BuyingPower: {BuyingPower}, TotalValue: {TotalValue}";
        }
    }
}
