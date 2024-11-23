using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class Models : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GridSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GridName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsCompactMode = table.Column<bool>(type: "bit", nullable: false),
                    IsVirtualizationEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsFilterEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsSortingEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GridSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Integrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IntegrationType = table.Column<int>(type: "int", nullable: false),
                    BaseUrl = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApiKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UniqueKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Integrations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ArchivedStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bugs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    BugStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArchivedStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TestCaseExecutionId = table.Column<int>(type: "int", nullable: true),
                    BProjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bugs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bugs_Projects_BProjectId",
                        column: x => x.BProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Requirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArchivedStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RProjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requirements_Projects_RProjectId",
                        column: x => x.RProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestCases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ArchivedStatus = table.Column<int>(type: "int", nullable: false),
                    TestType = table.Column<int>(type: "int", nullable: false),
                    TestScope = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstimatedTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TcProjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestCases_Projects_TcProjectId",
                        column: x => x.TcProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SelectedTestCasesIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArchivedStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TPProjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestPlans_Projects_TPProjectId",
                        column: x => x.TPProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestStepsExecutionFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileContent = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TestStepExecutionId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TSEFProjectId = table.Column<int>(type: "int", nullable: false),
                    ProjectsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestStepsExecutionFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestStepsExecutionFiles_Projects_ProjectsId",
                        column: x => x.ProjectsId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BugsComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BugId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugsComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BugsComments_Bugs_BugId",
                        column: x => x.BugId,
                        principalTable: "Bugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BugsFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileContent = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BugId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BfProjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugsFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BugsFiles_Bugs_BugId",
                        column: x => x.BugId,
                        principalTable: "Bugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BugsFiles_Projects_BfProjectId",
                        column: x => x.BfProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequirementsFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileContent = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequirementId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RfProjectId = table.Column<int>(type: "int", nullable: false),
                    RequirementsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequirementsFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequirementsFiles_Projects_RfProjectId",
                        column: x => x.RfProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequirementsFiles_Requirements_RequirementId",
                        column: x => x.RequirementId,
                        principalTable: "Requirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequirementsFiles_Requirements_RequirementsId",
                        column: x => x.RequirementsId,
                        principalTable: "Requirements",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RequirementsTestCases",
                columns: table => new
                {
                    RequirementsId = table.Column<int>(type: "int", nullable: false),
                    TestCasesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequirementsTestCases", x => new { x.RequirementsId, x.TestCasesId });
                    table.ForeignKey(
                        name: "FK_RequirementsTestCases_Requirements_RequirementsId",
                        column: x => x.RequirementsId,
                        principalTable: "Requirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequirementsTestCases_TestCases_TestCasesId",
                        column: x => x.TestCasesId,
                        principalTable: "TestCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestCasesFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileContent = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TcfProjectId = table.Column<int>(type: "int", nullable: false),
                    TestCaseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCasesFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestCasesFiles_Projects_TcfProjectId",
                        column: x => x.TcfProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestCasesFiles_TestCases_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "TestCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestCasesJira",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JiraId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TestCasesJiraId = table.Column<int>(type: "int", nullable: false),
                    TestCasesId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCasesJira", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestCasesJira_TestCases_TestCasesId",
                        column: x => x.TestCasesId,
                        principalTable: "TestCases",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TestCasesJira_TestCases_TestCasesJiraId",
                        column: x => x.TestCasesJiraId,
                        principalTable: "TestCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExpectedResult = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TestCaseId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TestCasesId = table.Column<int>(type: "int", nullable: false),
                    ArchivedStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestSteps_TestCases_TestCasesId",
                        column: x => x.TestCasesId,
                        principalTable: "TestCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestCasesTestPlans",
                columns: table => new
                {
                    TestCasesId = table.Column<int>(type: "int", nullable: false),
                    TestPlansId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCasesTestPlans", x => new { x.TestCasesId, x.TestPlansId });
                    table.ForeignKey(
                        name: "FK_TestCasesTestPlans_TestCases_TestCasesId",
                        column: x => x.TestCasesId,
                        principalTable: "TestCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestCasesTestPlans_TestPlans_TestPlansId",
                        column: x => x.TestPlansId,
                        principalTable: "TestPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestExecution",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    TestPlanId = table.Column<int>(type: "int", nullable: false),
                    SelectedTestCaseIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExecutionStatus = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: false),
                    EstimatedTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ExecutionNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArchivedStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TEProjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestExecution", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestExecution_Projects_TEProjectId",
                        column: x => x.TEProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestExecution_TestPlans_TestPlanId",
                        column: x => x.TestPlanId,
                        principalTable: "TestPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestPlansFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileContent = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TestPlanId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TPFProjectId = table.Column<int>(type: "int", nullable: false),
                    TestPlansId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestPlansFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestPlansFiles_Projects_TPFProjectId",
                        column: x => x.TPFProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestPlansFiles_TestPlans_TestPlanId",
                        column: x => x.TestPlanId,
                        principalTable: "TestPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestPlansFiles_TestPlans_TestPlansId",
                        column: x => x.TestPlansId,
                        principalTable: "TestPlans",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TestCaseExecution",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestExecutionId = table.Column<int>(type: "int", nullable: false),
                    TestCaseId = table.Column<int>(type: "int", nullable: false),
                    ExecutionStatus = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: false),
                    ExecutionNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArchivedStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCaseExecution", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestCaseExecution_TestCases_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "TestCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestCaseExecution_TestExecution_TestExecutionId",
                        column: x => x.TestExecutionId,
                        principalTable: "TestExecution",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestStepsExecution",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestCaseExecutionId = table.Column<int>(type: "int", nullable: false),
                    TestStepsId = table.Column<int>(type: "int", nullable: false),
                    ExecutionStatus = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: false),
                    ExecutionNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ArchivedStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualResult = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestStepsExecutionFileId = table.Column<int>(type: "int", nullable: true),
                    TestCaseExecutionId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestStepsExecution", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestStepsExecution_TestCaseExecution_TestCaseExecutionId",
                        column: x => x.TestCaseExecutionId,
                        principalTable: "TestCaseExecution",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestStepsExecution_TestCaseExecution_TestCaseExecutionId1",
                        column: x => x.TestCaseExecutionId1,
                        principalTable: "TestCaseExecution",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TestStepsExecution_TestStepsExecutionFiles_TestStepsExecutionFileId",
                        column: x => x.TestStepsExecutionFileId,
                        principalTable: "TestStepsExecutionFiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TestStepsExecution_TestSteps_TestStepsId",
                        column: x => x.TestStepsId,
                        principalTable: "TestSteps",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bugs_BProjectId",
                table: "Bugs",
                column: "BProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_BugsComments_BugId",
                table: "BugsComments",
                column: "BugId");

            migrationBuilder.CreateIndex(
                name: "IX_BugsFiles_BfProjectId",
                table: "BugsFiles",
                column: "BfProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_BugsFiles_BugId",
                table: "BugsFiles",
                column: "BugId");

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_RProjectId",
                table: "Requirements",
                column: "RProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementsFiles_RequirementId",
                table: "RequirementsFiles",
                column: "RequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementsFiles_RequirementsId",
                table: "RequirementsFiles",
                column: "RequirementsId");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementsFiles_RfProjectId",
                table: "RequirementsFiles",
                column: "RfProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementsTestCases_TestCasesId",
                table: "RequirementsTestCases",
                column: "TestCasesId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseExecution_TestCaseId",
                table: "TestCaseExecution",
                column: "TestCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseExecution_TestExecutionId",
                table: "TestCaseExecution",
                column: "TestExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCases_TcProjectId",
                table: "TestCases",
                column: "TcProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCasesFiles_TcfProjectId",
                table: "TestCasesFiles",
                column: "TcfProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCasesFiles_TestCaseId",
                table: "TestCasesFiles",
                column: "TestCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCasesJira_TestCasesId",
                table: "TestCasesJira",
                column: "TestCasesId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCasesJira_TestCasesJiraId",
                table: "TestCasesJira",
                column: "TestCasesJiraId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCasesTestPlans_TestPlansId",
                table: "TestCasesTestPlans",
                column: "TestPlansId");

            migrationBuilder.CreateIndex(
                name: "IX_TestExecution_TEProjectId",
                table: "TestExecution",
                column: "TEProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TestExecution_TestPlanId",
                table: "TestExecution",
                column: "TestPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_TestPlans_TPProjectId",
                table: "TestPlans",
                column: "TPProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TestPlansFiles_TestPlanId",
                table: "TestPlansFiles",
                column: "TestPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_TestPlansFiles_TestPlansId",
                table: "TestPlansFiles",
                column: "TestPlansId");

            migrationBuilder.CreateIndex(
                name: "IX_TestPlansFiles_TPFProjectId",
                table: "TestPlansFiles",
                column: "TPFProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TestSteps_TestCasesId",
                table: "TestSteps",
                column: "TestCasesId");

            migrationBuilder.CreateIndex(
                name: "IX_TestStepsExecution_TestCaseExecutionId",
                table: "TestStepsExecution",
                column: "TestCaseExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestStepsExecution_TestCaseExecutionId1",
                table: "TestStepsExecution",
                column: "TestCaseExecutionId1");

            migrationBuilder.CreateIndex(
                name: "IX_TestStepsExecution_TestStepsExecutionFileId",
                table: "TestStepsExecution",
                column: "TestStepsExecutionFileId");

            migrationBuilder.CreateIndex(
                name: "IX_TestStepsExecution_TestStepsId",
                table: "TestStepsExecution",
                column: "TestStepsId");

            migrationBuilder.CreateIndex(
                name: "IX_TestStepsExecutionFiles_ProjectsId",
                table: "TestStepsExecutionFiles",
                column: "ProjectsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BugsComments");

            migrationBuilder.DropTable(
                name: "BugsFiles");

            migrationBuilder.DropTable(
                name: "GridSettings");

            migrationBuilder.DropTable(
                name: "Integrations");

            migrationBuilder.DropTable(
                name: "RequirementsFiles");

            migrationBuilder.DropTable(
                name: "RequirementsTestCases");

            migrationBuilder.DropTable(
                name: "TestCasesFiles");

            migrationBuilder.DropTable(
                name: "TestCasesJira");

            migrationBuilder.DropTable(
                name: "TestCasesTestPlans");

            migrationBuilder.DropTable(
                name: "TestPlansFiles");

            migrationBuilder.DropTable(
                name: "TestStepsExecution");

            migrationBuilder.DropTable(
                name: "Bugs");

            migrationBuilder.DropTable(
                name: "Requirements");

            migrationBuilder.DropTable(
                name: "TestCaseExecution");

            migrationBuilder.DropTable(
                name: "TestStepsExecutionFiles");

            migrationBuilder.DropTable(
                name: "TestSteps");

            migrationBuilder.DropTable(
                name: "TestExecution");

            migrationBuilder.DropTable(
                name: "TestCases");

            migrationBuilder.DropTable(
                name: "TestPlans");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
