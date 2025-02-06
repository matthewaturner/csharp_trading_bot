using Theo.Models;
using System;

namespace Theo.Indicators.Interfaces
{
	public interface IMovingAverageCrossover : ISimpleValueIndicator<PositionType>
	{
		public double ShortMa { get; }
		public double LongMa { get; }
	}
}
