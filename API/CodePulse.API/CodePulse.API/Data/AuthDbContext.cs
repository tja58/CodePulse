﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace CodePulse.API.Data;

public class AuthDbContext : IdentityDbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var readerRoleId = "53769178-d8f0-4652-8240-549c1bf91df2"; // randomly gen Guid
        var writerRoleId = "c4361646-32ce-45d4-a754-12da0b3f7fe9"; // randomly gen Guid

        // create reader and writer role
        var roles = new List<IdentityRole>()
        {
            new IdentityRole()
            {
                Id = readerRoleId,
                Name ="Reader",
                NormalizedName = "Reader".ToUpper(),
                ConcurrencyStamp = readerRoleId
            },
            new IdentityRole()
            {
                Id=writerRoleId,
                Name = "Writer",
                NormalizedName = "Writer".ToUpper(),
                ConcurrencyStamp = writerRoleId
            }
        };

        // seed the roles
        builder.Entity<IdentityRole>().HasData(roles);

        // create admin user
        var adminUserId = "e061bbec-7627-4ebd-a22f-cf24ad5dd497"; // randomly gen Guid
        var admin = new IdentityUser()
        {
            Id = adminUserId,
            UserName = "admin@codepulse.com",
            Email = "admin@codepulse.com",
            NormalizedEmail = "admin@codepulse.com".ToUpper(),
            NormalizedUserName = "admin@codepulse.com".ToUpper(),
        };

        admin.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(admin, "Admin@123");
        builder.Entity<IdentityUser>().HasData(admin);

        // give roles to admin
        var adminRoles = new List<IdentityUserRole<string>>()
        {
            new() {
                UserId = adminUserId,
                RoleId = readerRoleId
            },
            new()
            {
                UserId = adminUserId,
                RoleId = writerRoleId
            }
        };

        builder.Entity<IdentityUserRole<string>>().HasData(adminRoles);
    }
}
