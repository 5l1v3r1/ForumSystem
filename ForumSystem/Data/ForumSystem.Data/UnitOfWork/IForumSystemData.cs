﻿namespace ForumSystem.Data.UnitOfWork
{
    using ForumSystem.Data.Common.Models;
    using ForumSystem.Data.Models;

    public interface IForumSystemData
    {
        IDeletableEntityRepository<ApplicationUser> Users { get; }

        IDeletableEntityRepository<Section> Sections { get; }

        IDeletableEntityRepository<Category> Categories { get; }

        IDeletableEntityRepository<Post> Posts { get; }

        IDeletableEntityRepository<Answer> Answers { get; }

        IDeletableEntityRepository<Comment> Comments { get; }

        IDeletableEntityRepository<PostReport> PostReports { get; } 

        void SaveChanges();
    }
}