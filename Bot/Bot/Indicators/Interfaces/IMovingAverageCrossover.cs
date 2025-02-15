using Bot.Models;
using System;

namespace Bot.Indicators.Interfaces
{
	public interface IMovingAverageCrossover : ISimpleValueIndicator<PositionType>
	{
		public double ShortMa { get; }
		public double LongMa { get; }
	}
}
