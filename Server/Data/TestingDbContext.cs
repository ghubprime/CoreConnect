using Microsoft.EntityFrameworkCore;

namespace CoreConnect.Server.Data;

public class TestingDbContext : AppDb
{
    public TestingDbContext(IWebHostEnvironment hostEnvironment) 
        : base(hostEnvironment)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseInMemoryDatabase("CoreConnect");
        base.OnConfiguring(options);
    }
}
