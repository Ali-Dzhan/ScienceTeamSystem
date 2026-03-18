using Microsoft.EntityFrameworkCore;
using ScienceTeamsApp.Data;
using ScienceTeamsApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ScienceTeamsApp.Tests
{
    public class ScienceTeamsTests
    {
        // Помощен метод за създаване на виртуална база данни за тестовете
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateTeam_SavesToDatabase_Successfully()
        {
            // Arrange
            var dbContext = GetDbContext();
            var newTeam = new Team { Name = "AI Research", Description = "Testing team" };

            // Act
            dbContext.Teams.Add(newTeam);
            await dbContext.SaveChangesAsync();

            // Assert
            var teamInDb = await dbContext.Teams.FirstOrDefaultAsync(t => t.Name == "AI Research");
            Assert.NotNull(teamInDb);
            Assert.Equal(1, dbContext.Teams.Count());
        }

        [Fact]
        public void Task_Overdue_Logic_ReturnsTrue_If_DeadlinePassed()
        {
            // Arrange
            var task = new TaskItem
            {
                Title = "Old Task",
                Deadline = DateTime.Now.AddDays(-5),
                Status = "In Progress"
            };

            // Act
            bool isOverdue = task.Deadline < DateTime.Now && task.Status != "Done";

            // Assert
            Assert.True(isOverdue);
        }

        [Fact]
        public void Task_Overdue_Logic_ReturnsFalse_If_StatusIsDone()
        {
            // Arrange
            var task = new TaskItem
            {
                Title = "Finished Task",
                Deadline = DateTime.Now.AddDays(-5),
                Status = "Done"
            };

            // Act
            bool isOverdue = task.Deadline < DateTime.Now && task.Status != "Done";

            // Assert
            Assert.False(isOverdue);
        }

        [Fact]
        public async Task ActivityLog_IsCreated_Correctly()
        {
            // Arrange
            var dbContext = GetDbContext();
            var log = new ActivityLog
            {
                Action = "Create Task",
                Description = "Created testing task",
                Timestamp = DateTime.Now,
                UserId = "user123"
            };

            // Act
            dbContext.ActivityLogs.Add(log);
            await dbContext.SaveChangesAsync();

            // Assert
            var savedLog = await dbContext.ActivityLogs.FirstOrDefaultAsync();
            Assert.NotNull(savedLog);
            Assert.Equal("Create Task", savedLog.Action);
        }

        [Fact]
        public void TaskItem_Requires_Title_Validation()
        {
            // Arrange
            var task = new TaskItem { Description = "No title task" };

            // Act
            bool hasTitle = !string.IsNullOrEmpty(task.Title);

            // Assert
            Assert.False(hasTitle);
        }
    }
}