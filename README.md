# csharp_trading_bot
Simple trading bot C#.

https://m.imgur.com/a/dHfkE7O

# Requirements
 - dotnet 8

# Interesting Capabilities
 - Fluent, composable indicators that allow you to build silly things like "3-Day simple moving average of the squares of prices" with a single line
`var smaOfSquares = Ind.SMA(3).Of(x => x*x);`. These can be chained to arbitrary complexity. Check out the Indicators folder and the tests.
 - Built around the idea that a strategy should be completely separate from the portfolio containing it. Strategies simply output the _desired allocations_
for the symbols in their universe. The allocations would be aggregated (paper/live trading not yet implemented) by a separate component which handles risk
adjustments and places trades.
 - Everything is interfaced. Easily swap new data sources, strategies, etc.
 - For correctness, I have followed along with Ernest Chan's Quantitative Trading which gives data and known good values for example backtests and things
like Sharpe Ratio so I have high confidence that my math is actually correct.
  - Backtests are _intentionally_ omitting trading fees, etc. simply because that's how Quantitative Trading does it. I originally implemented trading fees
and compounded returns daily but found it was different from the book (no fees no compounding) so I scrapped it and would reimplement it again later.

# Roadmap
Eventually I want to add the following:
 - Strategies should be able to be implemented in many different languages. I have already used the event pattern for sending market data and other init,
finalize events which will make this easy later on. Can swap to GRPC or any other mechanism. This will allow me to use stats packages from python, R, etc.
without having to implement arcane stationarity tests myself.
 - Currently the bot is backtesting only, there is a lot of work needed to plug it into a real system. First would come paper trading, then live trading.
 - Store my own tick data in something like Kusto, a database that is optimized for write-once-read-many workloads. This will enable me to build what I
 would call "discovery" strategies which query the universe of stocks for things like "all stocks that dropped 10% yesterday" and reliably backtest them.
 - Parse financial data from sec website. Wouldn't want to make this very complicated, probably just feed XBRL into an AI tool to standardize the accounting
to some version of GAAP and hope for the best.

# Screenshots
Just some proof of what the bot does, roughly how it works and why it is neat.

## Example3_6 From Ernest P. Chan's Quantitative Trading

1. Calculate OLS Regression on GLD-GDX
![image](https://github.com/user-attachments/assets/1978dab0-8f2a-4a83-b9d9-c1e61d795a8b)

2. Compute unit portfolio y = GLD-m*GDX
![image](https://github.com/user-attachments/assets/4de0181e-ac45-4c43-9835-887aa69690f0)

4. Short backtest out of sample - nearly identitcal results as the book
![image](https://github.com/user-attachments/assets/b5f0e802-9552-4ce9-9c9b-7d1846c223a0)

## OLSPairsTrade Backtest on GLD+IGE
Continued playing around with the backtest settings, found one that looks nice. 
No selection bias here I swear...
![image](https://github.com/user-attachments/assets/32c68a23-514b-4ef0-94a3-ffed798e1b1b)
![image](https://github.com/user-attachments/assets/4e6ca6fe-e551-4af9-abbd-11d062162244)
