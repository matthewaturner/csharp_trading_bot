using System;

namespace Bot.Brokerages
{
    public class Position
    {
        public string Name;

        public PositionType _Type;

        public double Size;

        public double EntryPrice;

        public Position(string name, PositionType type, double size, double entryPrice)
        {
            this.Name = name != null ? name : throw new ArgumentNullException(nameof(name));
            this.Size = size;
            this.EntryPrice = entryPrice;
            this._Type = type;
        }

        public double GetPositionInitialValue()
        {
            return this.Size * this.EntryPrice;
        }

        public double GetCurrentPositionValue(double CurrentPrice)
        {
            return this.Size * CurrentPrice;
        }

    }
}
