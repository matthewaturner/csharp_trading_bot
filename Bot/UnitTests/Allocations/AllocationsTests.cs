
using Bot.Models.Allocations;

namespace UnitTests;

public class AllocationTests
{
    [Fact]
    public void SetAndGetAllocation_ShouldStoreAndRetrieveCorrectly()
    {
        var allocations = new Allocation();
        allocations.SetAllocation("AAPL", 0.5);

        var result = allocations.GetAllocation("AAPL");

        Assert.Equal(0.5, result);
    }

    [Fact]
    public void TotalWeight_ShouldReturnSumOfWeights()
    {
        var allocations = new Allocation();
        allocations.SetAllocation("AAPL", 0.4);
        allocations.SetAllocation("MSFT", 0.6);

        var totalWeight = allocations.TotalWeight();

        Assert.Equal(1.0, totalWeight);
    }
}
