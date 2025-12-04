using Microsoft.EntityFrameworkCore;
using ProdApp.Data;

public static class TestDbContextFactory
{
    public static ProdDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ProdDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ProdDbContext(options);
    }
}
