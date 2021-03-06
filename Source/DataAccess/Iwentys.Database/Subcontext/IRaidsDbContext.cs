﻿using Iwentys.Domain.Raids;
using Iwentys.Domain.Raids.Dto;
using Microsoft.EntityFrameworkCore;

namespace Iwentys.Database.Subcontext
{
    public interface IRaidsDbContext
    {
        public DbSet<Raid> Raids { get; set; }
        public DbSet<RaidVisitor> RaidVisitors { get; set; }
        public DbSet<RaidPartySearchRequest> PartySearchRequests { get; set; }
    }
    public static class RaidsDbContextExtensions
    {
        public static void OnRaidsModelCreating(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RaidVisitor>().HasKey(rv => new { rv.RaidId, rv.VisitorId });
            modelBuilder.Entity<RaidPartySearchRequest>().HasKey(rv => new { rv.RaidId, rv.AuthorId });
        }
    }
}