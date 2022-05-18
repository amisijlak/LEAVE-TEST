using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using LEAVE.DAL;
using LEAVE.DAL.Entities;
using LEAVE.DAL.Lookups;
using LEAVE.DAL.Security;
using LEAVE.DAL.Settings;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;

namespace LEAVE.DAL
{
    public class LEAVEContext : IdentityDbContext<ApplicationUser, SecurityRole, string>
    {
        public string ConnectionString => _connectionString;
        private readonly string _connectionString;

        public LEAVEContext(DbContextOptions<LEAVEContext> options)
            : base(options)
        {
            if (options != null)
            {
                //extract connnection string
                var extension = options.FindExtension<SqlServerOptionsExtension>();
                _connectionString = extension.ConnectionString;
            }
        }

        #region Datasets

        #region Settings

        public DbSet<GenericLookup> GenericLookups { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Institution> Institutions { get; set; }
        public DbSet<PermissionType> PermissionTypes { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        #endregion

        #endregion

        private void CreateIndex<T>(ModelBuilder builder, Expression<Func<T, object>> KeyPropertyExpression, string IndexName = "IX_Unique", bool IsUnique = true)
            where T : class
        {
            var indexBuilder = builder.Entity<T>().HasIndex(KeyPropertyExpression);
            if (!string.IsNullOrEmpty(IndexName)) indexBuilder.HasName(IndexName);
            if (IsUnique) indexBuilder.IsUnique();
        }

        private void CreateCodeIndex<T>(ModelBuilder builder) where T : class, ICodedModel
        {
            CreateIndex<T>(builder, r => r.Code, "IX_Unique_Code");
        }

        private void CreateNameIndex<T>(ModelBuilder builder) where T : class, INamedModel
        {
            CreateIndex<T>(builder, r => r.Name, "IX_Unique_Name");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Call the base class first:
            base.OnModelCreating(builder);

            if (builder != null)
            {
                //settings
                CreateCodeIndex<Employee>(builder);

                //lookups
                CreateIndex<GenericLookup>(builder, r => new { r.LookupType, r.Code });

                //entities
                CreateCodeIndex<Institution>(builder);

                //security
                CreateIndex<PermissionType>(builder, r => new { r.Module, r.SubModule, r.SystemAction });
                CreateIndex<RolePermission>(builder, r => new { r.RoleId, r.PermissionId });
                CreateIndex<ApplicationUser>(builder, r => r.Code);
            }
        }
    }
}
