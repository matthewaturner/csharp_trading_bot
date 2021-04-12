
namespace Bot.Models.Interfaces
{
    public interface IAccount
    {
        /// <summary>
        /// Id of the account.
        /// </summary>
        public string AccountId { get; }

        /// <summary>
        /// Cash available to trade.
        /// </summary>
        public double Cash { get; }

        /// <summary>
        /// Amount that can be bought or sold currently.
        /// </summary>
        public double BuyingPower { get; }

        /// <summary>
        /// Total market value of the account. Cash + long value + short value.
        /// </summary>
        public double TotalValue { get; }
    }
}
