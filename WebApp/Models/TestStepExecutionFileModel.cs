using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Models;

public class TestStepExecutionFileModel(IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    private readonly ApplicationDbContext _dbContext = dbContextFactory.CreateDbContext();

}