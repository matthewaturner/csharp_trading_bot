
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
        public BackTestAccount(decimal initialCash)
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
        public decimal Cash { get; set; }

        /// <summary>
        /// Buying power.
        /// </summary>
        public decimal BuyingPower { get; set; }

        /// <summary>
        /// Total value of the account.
        /// </summary>
        public decimal TotalValue { get; set; }

        public override string ToString()
        {
            return $"AccountId: {AccountId}, Cash: {Cash}, BuyingPower: {BuyingPower}, TotalValue: {TotalValue}";
        }
    }
}
