// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Bot.DataSources.Alpaca.Models;

public class BarsResponse
{
    [JsonPropertyName("bars")]
    public List<Bar> Bars { get; set; }

    [JsonPropertyName("next_page_token")]
    public string NextPageToken { get; set; }
}