using System.Text.Json;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WebApp.Services;

namespace WebApp.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Projects> Projects { get; set; }
    public DbSet<TestCasesJira> TestCasesJira { get; set; }
    public DbSet<Integrations> Integrations { get; set; }
    public DbSet<Cycles> Cycles { get; set; }

    public DbSet<Requirements> Requirements { get; set; }
    public DbSet<RequirementsFile> RequirementsFiles { get; set; }
    public DbSet<RequirementsSpecification> RequirementsSpecification { get; set; }

    public DbSet<TestCases> TestCases { get; set; }
    public DbSet<TestCasesFile> TestCasesFiles { get; set; }

    public DbSet<TestSteps> TestSteps { get; set; }
    public DbSet<TestPlans> TestPlans { get; set; }
    public DbSet<TestPlansFile> TestPlansFiles { get; set; }

    public DbSet<TestExecution> TestExecution { get; set; }
    public DbSet<TestCaseExecution> TestCaseExecution { get; set; }
    public DbSet<TestStepsExecution> TestStepsExecution { get; set; }
    public DbSet<TestStepsExecutionFile> TestStepsExecutionFiles { get; set; }

    public DbSet<Bugs> Bugs { get; set; }
    public DbSet<BugsFiles> BugsFiles { get; set; }
    public DbSet<BugsComments> BugsComments { get; set; }
    
    public DbSet<QAflowSettings> QAflowSettings { get; set; }
    
    //Add Logs for full traceability
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //Project Relationships
        modelBuilder.Entity<Projects>()
            .HasMany(p => p.Bugs)
            .WithOne(r => r.Projects)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Projects>()
            .HasMany(p => p.BugsFile)
            .WithOne(r => r.Projects)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Projects>()
            .HasMany(p => p.Requirements)
            .WithOne(r => r.Projects)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Projects>()
            .HasMany(p => p.RequirementsFile)
            .WithOne(r => r.Projects)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Projects>()
            .HasMany(p => p.RequirementsSpecification)
            .WithOne(p => p.Projects)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<Projects>()
            .HasMany(p => p.TestCasesFile)
            .WithOne(r => r.Projects)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Projects>()
            .HasMany(p => p.TestPlansFile)
            .WithOne(r => r.Projects)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Projects>()
            .HasMany(p => p.TestPlans)
            .WithOne(r => r.Projects)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Projects>()
            .HasMany(p => p.TestCases)
            .WithOne(tc => tc.Projects)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<Projects>()
            .HasMany(p => p.TestExecution)
            .WithOne(r => r.Projects)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Requirements>()
            .HasOne(r => r.RequirementsSpecification)
            .WithMany(rs => rs.LinkedRequirements)
            .HasForeignKey("RequirementsSpecificationId")
            .OnDelete(DeleteBehavior.Restrict); // or Cascade or SetNull depending on your logic


        modelBuilder.Entity<TestCases>()
            .HasMany(t => t.TestCasesJira)
            .WithOne(t => t.TestCases)
            .HasForeignKey(tcj => tcj.TestCasesId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TestCases>()
            .HasMany(tc => tc.LinkedTestCaseExecutions)
            .WithOne(tce => tce.TestCases)
            .HasForeignKey(tce => tce.TestCaseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TestExecution>()
            .HasOne(te => te.TestPlan)
            .WithMany()
            .HasForeignKey(te => te.TestPlanId)
            .OnDelete(DeleteBehavior
                .Restrict); // This prevents deletion of TestPlan if there are related TestExecutions

        modelBuilder.Entity<TestCaseExecution>()
            .HasOne(tce => tce.TestExecution)
            .WithMany(te => te.LinkedTestCaseExecutions)
            .HasForeignKey(tce => tce.TestExecutionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TestCaseExecution>()
            .HasOne(tce => tce.TestCases)
            .WithMany(tc => tc.LinkedTestCaseExecutions)
            .HasForeignKey(tce => tce.TestCaseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TestStepsExecution>()
            .HasOne(tse => tse.TestCaseExecution)
            .WithMany()
            .HasForeignKey(tse => tse.TestCaseExecutionIdFk)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TestStepsExecution>()
            .HasOne(tse => tse.TestSteps)
            .WithMany()
            .HasForeignKey(tse => tse.TestStepsId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Bugs>()
            .HasMany(b => b.BugFiles)
            .WithOne(bf => bf.Bugs)
            .HasForeignKey(bf => bf.BugId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Bugs>()
            .HasMany(b => b.BugComments)
            .WithOne(bc => bc.Bugs)
            .HasForeignKey(bc => bc.BugId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<TestCases>()
            .HasMany(tc => tc.LinkedTestCasesFiles)
            .WithOne(tcf => tcf.TestCases)
            .HasForeignKey(tcf => tcf.TestCaseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Requirements>()
            .HasMany<RequirementsFile>()
            .WithOne(rf => rf.Requirements)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RequirementsFile>()
            .HasOne(rf => rf.Requirements)
            .WithMany(r => r.LinkedRequirementsFiles)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TestPlans>()
            .HasMany<TestPlansFile>()
            .WithOne(tpf => tpf.TestPlans)
            .HasForeignKey(tpf => tpf.TestPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TestStepsExecution>()
            .HasOne(tse => tse.TestSteps)
            .WithMany()
            .HasForeignKey(tse => tse.TestStepsId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<TestCaseExecution>()
            .HasMany<TestStepsExecution>(tse => tse.LinkedTestStepsExecution)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TestPlans>()
            .HasMany<Cycles>()
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Cycles>()
            .HasMany<TestPlans>()
            .WithOne(t => t.Cycle)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<Bugs>()
            .HasMany(b => b.LinkedTestCases) // Specify the navigation property
            .WithOne(t => t.Bugs) // Specify the inverse navigation property
            .HasForeignKey("BugsId") // Specify the foreign key explicitly
            .OnDelete(DeleteBehavior.Cascade);
    }

    /// <summary>
    /// Save and Update with user information and timestamp automatically
    /// </summary>
    /// <param name="userService"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>


    private static string? GetPrimaryKeyString(EntityEntry entry)
    {
        var pk = entry.Metadata.FindPrimaryKey();
        if (pk == null) return null;

        var parts = pk.Properties
            .Select(p =>
            {
                var prop = entry.Property(p.Name);
                var val = prop.CurrentValue ?? prop.OriginalValue;
                return $"{p.Name}={val}";
            })
            .ToArray();

        if (parts.Length == 0) return null;
        if (parts.Length == 1) return parts[0].Split('=')[1]; // just value
        return string.Join(";", parts); // composite key: "A=1;B=2"
    }

// Heuristic: stub entities often have only key set + defaults for everything else.
    private static bool LooksLikeStub(object obj)
    {
        if (obj == null) return true;

        var props = obj.GetType().GetProperties();
        int meaningful = 0;

        foreach (var p in props)
        {
            var v = p.GetValue(obj);
            if (v == null) continue;
            if (v is string s && string.IsNullOrWhiteSpace(s)) continue;
            if (v is Guid g && g == Guid.Empty) continue;
            if (v is int i && i == 0) continue;
            if (v is long l && l == 0) continue;
            if (v is DateTime dt && dt == default) continue;

            meaningful++;
            if (meaningful >= 2) return false;
        }

        return true;
    }
    
    public async Task<int> SaveChangesAsync(UserService userService, CancellationToken cancellationToken = default)
{
    var strategy = Database.CreateExecutionStrategy();

    return await strategy.ExecuteAsync(async () =>
    {
        var now = DateTime.UtcNow;
        var user = (await userService.GetCurrentUserInfoAsync()).UserName;
        if(user is null) throw new ArgumentNullException(nameof(user));
        // single atomic transaction (now allowed because it's inside the execution strategy)
        await using var tx = await Database.BeginTransactionAsync(cancellationToken);

        var auditLogs = new List<AuditLog>();
        var addedEntries = new List<EntityEntry<BaseEntity>>();

        // Track only BaseEntity, and avoid auditing AuditLog itself if it inherits BaseEntity
        var entries = ChangeTracker.Entries<BaseEntity>().ToList();


        // ADDED
        foreach (var entry in entries.Where(e => e.State == EntityState.Added))
        {
            entry.Entity.CreatedAt = now;
            entry.Entity.CreatedBy = user;
            entry.Entity.ModifiedAt = now;
            entry.Entity.ModifiedBy = user;

            addedEntries.Add(entry); // log after save (to get DB-generated keys)
        }

        // MODIFIED
        foreach (var entry in entries.Where(e => e.State == EntityState.Modified))
        {
            entry.Entity.ModifiedAt = now;
            entry.Entity.ModifiedBy = user;
            if(user is null) throw new ArgumentNullException(nameof(user));
            auditLogs.Add(new AuditLog
            {
                EntityName = entry.Entity.GetType().Name,
                EntityId = GetPrimaryKeyString(entry),
                UserName = user,
                Action = "Modified",
                BeforeData = JsonSerializer.Serialize(entry.OriginalValues.ToObject()),
                AfterData = JsonSerializer.Serialize(entry.CurrentValues.ToObject()),
                Timestamp = now
            });
        }

        // DELETED (hard delete)
        foreach (var entry in entries.Where(e => e.State == EntityState.Deleted))
        {
            // If entity is detached/stub, OriginalValues may be empty/defaults.
            // GetDatabaseValuesAsync gives you the actual row before deletion.
            object? beforeObject = null;

            var originalObj = entry.OriginalValues.ToObject();
            if (!LooksLikeStub(originalObj))
            {
                beforeObject = originalObj;
            }
            else
            {
                var dbValues = await entry.GetDatabaseValuesAsync(cancellationToken);
                beforeObject = dbValues?.ToObject();
            }

            auditLogs.Add(new AuditLog
            {
                EntityName = entry.Entity.GetType().Name,
                EntityId = GetPrimaryKeyString(entry),
                UserName = user,
                Action = "Deleted",
                BeforeData = beforeObject == null ? null : JsonSerializer.Serialize(beforeObject),
                AfterData = null,
                Timestamp = now
            });
        }

        // Save main changes
        var result = await base.SaveChangesAsync(cancellationToken);

        // Log ADDED after save (now IDs exist)
        foreach (var entry in addedEntries)
        {
            auditLogs.Add(new AuditLog
            {
                EntityName = entry.Entity.GetType().Name,
                EntityId = GetPrimaryKeyString(entry),
                UserName = user,
                Action = "Added",
                BeforeData = null,
                AfterData = JsonSerializer.Serialize(entry.CurrentValues.ToObject()),
                Timestamp = now
            });
        }

        // Save audit logs
        if (auditLogs.Count > 0)
        {
            Set<AuditLog>().AddRange(auditLogs);
            await base.SaveChangesAsync(cancellationToken);
        }

        await tx.CommitAsync(cancellationToken);
        return result;
    });
}
}