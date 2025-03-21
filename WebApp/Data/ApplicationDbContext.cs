using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Projects> Projects { get; set; }
    public DbSet<TestCasesJira> TestCasesJira { get; set; }
    public DbSet<Integrations> Integrations { get; set; }
    public DbSet<GridSettings> GridSettings { get; set; }


    public DbSet<Requirements> Requirements { get; set; }
    public DbSet<RequirementsFile> RequirementsFiles { get; set; }

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TestCases>()
            .HasMany(tc => tc.LinkedRequirements)
            .WithMany(r => r.LinkedTestCases)
            .UsingEntity(j => j.ToTable("RequirementsTestCases")); // Optional: Configure junction table name

        modelBuilder.Entity<TestPlans>()
            .HasMany(tp => tp.LinkedTestCases)
            .WithMany(tc => tc.TestPlans)
            .UsingEntity<Dictionary<string, object>>(
                "TestCasesTestPlans", // Name of the join table
                j => j
                    .HasOne<TestCases>()
                    .WithMany()
                    .HasForeignKey("TestCasesId")
                    .OnDelete(DeleteBehavior.Restrict), // Restrict deletion if associated
                j => j
                    .HasOne<TestPlans>()
                    .WithMany()
                    .HasForeignKey("TestPlansId")
                    .OnDelete(DeleteBehavior.Cascade) // Cascade delete for TestPlans
            );


        modelBuilder.Entity<Projects>()
            .HasMany(p => p.Requirements)
            .WithOne(r => r.Projects)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Projects>()
            .HasMany(p => p.Bugs)
            .WithOne(r => r.Projects)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Projects>()
            .HasMany(p => p.RequirementsFile)
            .WithOne(r => r.Projects)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Projects>()
            .HasMany(p => p.BugsFile)
            .WithOne(r => r.Projects)
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

        modelBuilder.Entity<Projects>()
            .HasMany(p => p.Bugs)
            .WithOne(r => r.Projects)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TestCases>()
            .HasMany<TestCasesJira>()
            .WithOne()
            .HasForeignKey(tcj => tcj.TestCasesJiraId).OnDelete(DeleteBehavior.Cascade);

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
        
        modelBuilder.Entity<TestExecution>()
            .HasMany<TestCaseExecution>(tce => tce.LinkedTestCaseExecutions)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TestCaseExecution>()
            .HasMany<TestStepsExecution>(tse => tse.LinkedTestStepsExecution)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

    }
}