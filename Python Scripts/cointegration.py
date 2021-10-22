import datetime
import numpy as np
import os
import os
import pandas as pd
import pandas as pd
import yfinance as yf

def get_yahoo_data(symbol, datadir, startdate = None, enddate = None) -> pd.DataFrame:
    """
    Gets yahoo finance data for a symbol. Pulls data from disk if data is within 1 day old.
    :param symbol: ticker to pull from yahoo finance.
    :param path: path to the csv for the symbol.
    :return: pandas dataframe
    """
    path = os.path.join(datadir, '{0}.csv'.format(symbol))
    last_modified = None

    if not os.path.exists(datadir):
        os.makedirs(datadir)

    if os.path.exists(path):
        mtime = os.stat(path).st_mtime
        last_modified = datetime.datetime.fromtimestamp(mtime)

    if not last_modified or last_modified < (datetime.datetime.now() - datetime.timedelta(hours=12)):
        data = yf.download(symbol, period='max', interval='1d')
        data.to_csv(path)

    data = pd.read_csv(path)

    # filter by date
    if startdate and enddate:
        data = data[(data['Date'] > startdate) & (data['Date'] < enddate)]

    return data

datadir = 'C:/Users/Matthew/source/repos/python_trading_bot/data/Yahoo'
outDir = 'C:/Users/Matthew/source/repos/python_trading_bot/output'

if not os.path.exists(datadir):
    os.makedirs(datadir)
if not os.path.exists(outDir):
    os.makedirs(outDir)

# pick symbols and get data
symbols = ['MJ', 'POTX', 'MSOS']
dfs = [get_yahoo_data(symbol, datadir, startdate='2021-01-01', enddate='2021-10-21') for symbol in symbols]

# clean and merge data into one data frame
dfs = [df.rename(columns={'Adj Close':symbol})[['Date', symbol]] for df, symbol in zip(dfs, symbols)]
df = dfs[0]
for idf in dfs[1:]:
    df = df.join(idf.set_index('Date'), on='Date').dropna()
df.set_index('Date', inplace=True)
df.plot()

# plot price series together
index_list = range(len(dfs))
pairs = res = [(a, b) for idx, a in enumerate(index_list) for b in index_list[idx + 1:]] 
for i, j in pairs:
    df.plot.scatter(x=symbols[i], y=symbols[j])