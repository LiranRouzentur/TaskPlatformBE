using Microsoft.EntityFrameworkCore;
using TaskPlatformBE.Models;

namespace TaskPlatformBE.Data
{
    public static class SeedData
    {
        public static void Seed(ApplicationDbContext context)
        {
            // Seed Users
            if (!context.Users.Any())
            {
                var users = new List<User>
                {
                    new User { Name = "John Doe" },
                    new User { Name = "Jane Smith" },
                    new User { Name = "Bob Johnson" },
                    new User { Name = "Alice Brown" },
                    new User { Name = "Charlie Wilson" },
                    new User { Name = "Diana Miller" },
                    new User { Name = "Edward Davis" },
                    new User { Name = "Fiona Garcia" }
                };

                context.Users.AddRange(users);
                context.SaveChanges();
            }

            // Seed Task Types
            if (!context.TaskTypes.Any())
            {
                var taskTypes = new List<TaskType>
                {
                    new TaskType { Name = "Procurement" },
                    new TaskType { Name = "Development" }
                };

                context.TaskTypes.AddRange(taskTypes);
                context.SaveChanges();
            }

            // Seed Task Status Items
            if (!context.TaskStatusItems.Any())
            {
                var taskStatuses = new List<TaskStatusItem>
                {
                    // Procurement Task Statuses
                    new TaskStatusItem 
                    { 
                        Id = 1, 
                        TypeId = 1, 
                        StatusId = 1, 
                        Name = "Created", 
                        IsFinal = false, 
                        Requirement = "", 
                        RequirementDescription = "No data required",
                        TaskTypeId = 1
                    },
                    new TaskStatusItem 
                    { 
                        Id = 2, 
                        TypeId = 1, 
                        StatusId = 2, 
                        Name = "Supplier offers received", 
                        IsFinal = false, 
                        Requirement = "2 price-quote strings (Comma-separated)", 
                        RequirementDescription = "Need exactly 2 offers (Comma-separated)",
                        TaskTypeId = 1
                    },
                    new TaskStatusItem 
                    { 
                        Id = 3, 
                        TypeId = 1, 
                        StatusId = 3, 
                        Name = "Purchase completed", 
                        IsFinal = true, 
                        Requirement = "Receipt string", 
                        RequirementDescription = "Need receipt",
                        TaskTypeId = 1
                    },
                    
                    // Development Task Statuses
                    new TaskStatusItem 
                    { 
                        Id = 4, 
                        TypeId = 2, 
                        StatusId = 1, 
                        Name = "Created", 
                        IsFinal = false, 
                        Requirement = "", 
                        RequirementDescription = "No data required",
                        TaskTypeId = 2
                    },
                    new TaskStatusItem 
                    { 
                        Id = 5, 
                        TypeId = 2, 
                        StatusId = 2, 
                        Name = "Specification completed", 
                        IsFinal = false, 
                        Requirement = "Specification text", 
                        RequirementDescription = "Need specification",
                        TaskTypeId = 2
                    },
                    new TaskStatusItem 
                    { 
                        Id = 6, 
                        TypeId = 2, 
                        StatusId = 3, 
                        Name = "Development completed", 
                        IsFinal = false, 
                        Requirement = "Branch name", 
                        RequirementDescription = "Need branch name",
                        TaskTypeId = 2
                    },
                    new TaskStatusItem 
                    { 
                        Id = 7, 
                        TypeId = 2, 
                        StatusId = 4, 
                        Name = "Distribution completed", 
                        IsFinal = true, 
                        Requirement = "Version number", 
                        RequirementDescription = "Need version number",
                        TaskTypeId = 2
                    }
                };

                context.TaskStatusItems.AddRange(taskStatuses);
                context.SaveChanges();
            }

            // Seed Sample Tasks
            if (!context.Tasks.Any())
            {
                var tasks = new List<TaskItem>
                {
                    // Procurement Tasks
                    new TaskItem 
                    { 
                        TypeId = 1, 
                        UserId = 3, 
                        Status = 1, 
                        NextAssignedUserId = 4, 
                        Requirement = "", 
                        CreatedAt = DateTime.UtcNow.AddDays(-5), 
                        UpdatedAt = null, 
                        CustomFields = "" 
                    },
                    new TaskItem 
                    { 
                        TypeId = 1, 
                        UserId = 1, 
                        Status = 2, 
                        NextAssignedUserId = 2, 
                        Requirement = "Quote A: $500 for premium package, Quote B: $450 for standard package", 
                        CreatedAt = DateTime.UtcNow.AddDays(-3), 
                        UpdatedAt = DateTime.UtcNow.AddDays(-1), 
                        CustomFields = "" 
                    },
                    new TaskItem 
                    { 
                        TypeId = 1, 
                        UserId = 4, 
                        Status = 3, 
                        NextAssignedUserId = 1, 
                        Requirement = "Receipt: INV-2024-001 - Premium package purchased for $500", 
                        CreatedAt = DateTime.UtcNow.AddDays(-7), 
                        UpdatedAt = DateTime.UtcNow.AddDays(-2), 
                        CustomFields = "" 
                    },
                    
                    // Development Tasks
                    new TaskItem 
                    { 
                        TypeId = 2, 
                        UserId = 2, 
                        Status = 1, 
                        NextAssignedUserId = 5, 
                        Requirement = "", 
                        CreatedAt = DateTime.UtcNow.AddDays(-4), 
                        UpdatedAt = null, 
                        CustomFields = "" 
                    },
                    new TaskItem 
                    { 
                        TypeId = 2, 
                        UserId = 3, 
                        Status = 2, 
                        NextAssignedUserId = 2, 
                        Requirement = "Complete user management system with role-based access control", 
                        CreatedAt = DateTime.UtcNow.AddDays(-6), 
                        UpdatedAt = DateTime.UtcNow.AddDays(-3), 
                        CustomFields = "" 
                    },
                    new TaskItem 
                    { 
                        TypeId = 2, 
                        UserId = 1, 
                        Status = 3, 
                        NextAssignedUserId = 4, 
                        Requirement = "feature/user-management-v2", 
                        CreatedAt = DateTime.UtcNow.AddDays(-8), 
                        UpdatedAt = DateTime.UtcNow.AddDays(-4), 
                        CustomFields = "" 
                    },
                    new TaskItem 
                    { 
                        TypeId = 2, 
                        UserId = 4, 
                        Status = 4, 
                        NextAssignedUserId = 2, 
                        Requirement = "v2.1.0", 
                        CreatedAt = DateTime.UtcNow.AddDays(-10), 
                        UpdatedAt = DateTime.UtcNow.AddDays(-5), 
                        CustomFields = "" 
                    }
                };

                context.Tasks.AddRange(tasks);
                context.SaveChanges();
            }

            // Seed Task Status Requirements (historical data for completed status changes)
            if (!context.TaskStatusRequirements.Any())
            {
                var taskStatusRequirements = new List<TaskStatusRequirement>
                {
                    // Procurement Task 2 - Status 1 requirement (empty for Created status)
                    new TaskStatusRequirement 
                    { 
                        TaskId = 2, 
                        StatusId = 1, 
                        RequirementValue = "bla1,bla2",
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    },
                    
                    // Procurement Task 3 - Status 1 requirement (empty for Created status)
                    new TaskStatusRequirement 
                    { 
                        TaskId = 3, 
                        StatusId = 1, 
                        RequirementValue = "",
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    },
                    
                    // Procurement Task 3 - Status 2 requirement (2 price quotes for Supplier offers received)
                    new TaskStatusRequirement 
                    { 
                        TaskId = 3, 
                        StatusId = 2, 
                        RequirementValue = "Quote A: $500 for premium package, Quote B: $450 for standard package",
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    },
                    
                    // Development Task 5 - Status 1 requirement (empty for Created status)
                    new TaskStatusRequirement 
                    { 
                        TaskId = 5, 
                        StatusId = 1, 
                        RequirementValue = "",
                        CreatedAt = DateTime.UtcNow.AddDays(-3)
                    },
                    
                    // Development Task 5 - Status 2 requirement (specification text)
                    new TaskStatusRequirement 
                    { 
                        TaskId = 5, 
                        StatusId = 2, 
                        RequirementValue = "Complete user management system with role-based access control",
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    },
                    
                    // Development Task 6 - Status 1 requirement (empty for Created status)
                    new TaskStatusRequirement 
                    { 
                        TaskId = 6, 
                        StatusId = 1, 
                        RequirementValue = "",
                        CreatedAt = DateTime.UtcNow.AddDays(-4)
                    },
                    
                    // Development Task 6 - Status 2 requirement (specification text)
                    new TaskStatusRequirement 
                    { 
                        TaskId = 6, 
                        StatusId = 2, 
                        RequirementValue = "Complete user management system with role-based access control",
                        CreatedAt = DateTime.UtcNow.AddDays(-3)
                    },
                    
                    // Development Task 6 - Status 3 requirement (branch name)
                    new TaskStatusRequirement 
                    { 
                        TaskId = 6, 
                        StatusId = 3, 
                        RequirementValue = "feature/user-management-v2",
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    },
                    
                    // Development Task 7 - Status 1 requirement (empty for Created status)
                    new TaskStatusRequirement 
                    { 
                        TaskId = 7, 
                        StatusId = 1, 
                        RequirementValue = "",
                        CreatedAt = DateTime.UtcNow.AddDays(-5)
                    },
                    
                    // Development Task 7 - Status 2 requirement (specification text)
                    new TaskStatusRequirement 
                    { 
                        TaskId = 7, 
                        StatusId = 2, 
                        RequirementValue = "Complete user management system with role-based access control",
                        CreatedAt = DateTime.UtcNow.AddDays(-4)
                    },
                    
                    // Development Task 7 - Status 3 requirement (branch name)
                    new TaskStatusRequirement 
                    { 
                        TaskId = 7, 
                        StatusId = 3, 
                        RequirementValue = "feature/user-management-v2",
                        CreatedAt = DateTime.UtcNow.AddDays(-3)
                    },
                    
                    // Development Task 7 - Status 4 requirement (version number)
                    new TaskStatusRequirement 
                    { 
                        TaskId = 7, 
                        StatusId = 4, 
                        RequirementValue = "v2.1.0",
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    }
                };

                context.TaskStatusRequirements.AddRange(taskStatusRequirements);
                context.SaveChanges();
            }
        }
    }
}