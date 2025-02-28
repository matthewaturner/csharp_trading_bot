using Bot.Models;
using System;

namespace Bot.Indicators.Interfaces
{
	public interface IMovingAverageCrossover : ISimpleValueIndicator<PositionType>
	{
		public decimal ShortMa { get; }
		public decimal LongMa { get; }
	}
}
