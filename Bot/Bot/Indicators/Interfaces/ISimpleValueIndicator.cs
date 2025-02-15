using System;

namespace Bot.Indicators.Interfaces
{
	/// <summary>
	/// Helper interface for indicators which only produce a single value.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ISimpleValueIndicator<T> : IIndicator
	{
		/// <summary>
		/// Gets the value of the indicator.
		/// </summary>
		public T Value { get; }
	}
}
