// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Models.Allocations;

namespace UnitTests.Allocations;

public class MetaAllocationTests
{
    [Fact]
    public void SetAndGetMetaAllocation_ShouldStoreAndRetrieveCorrectly()
    {
        var meta = new MetaAllocation();
        var alloc = new Allocation();
        alloc.SetAllocation("AAPL", 1.0);

        meta.SetAllocation("Strategy1", alloc, 0.5);

        var resultWeight = meta.GetAllocationWeight("Strategy1");
        var resultAlloc = meta.GetAllocation("Strategy1");

        Assert.Equal(0.5, resultWeight);
        Assert.Equal(alloc, resultAlloc);
    }

    [Fact]
    public void TotalWeight_ShouldReturnSumOfMetaWeights()
    {
        var meta = new MetaAllocation();
        var alloc1 = new Allocation();
        var alloc2 = new Allocation();

        meta.SetAllocation("Strategy1", alloc1, 0.4);
        meta.SetAllocation("Strategy2", alloc2, 0.6);

        var totalWeight = meta.TotalWeight();

        Assert.Equal(1.0, totalWeight);
    }

    [Fact]
    public void FlattenAllocation_ShouldReturnCombinedWeights()
    {
        var meta = new MetaAllocation();

        var alloc1 = new Allocation();
        alloc1.SetAllocation("AAPL", 0.6);
        alloc1.SetAllocation("MSFT", 0.4);

        var alloc2 = new Allocation();
        alloc2.SetAllocation("AAPL", 0.5);
        alloc2.SetAllocation("GOOG", 0.5);

        meta.SetAllocation("Strategy1", alloc1, 0.5);
        meta.SetAllocation("Strategy2", alloc2, 0.5);

        var flattened = meta.FlattenAllocations();

        Assert.Equal(0.55, flattened["AAPL"], 2);
        Assert.Equal(0.2, flattened["MSFT"], 2);
        Assert.Equal(0.25, flattened["GOOG"], 2);
    }
}
