﻿using System;
using Bogus;
using Iwentys.Domain.Raids.Dto;

namespace Iwentys.Database.Seeding.FakerEntities.Raids
{
    public class RaidFaker
    {
        private readonly Faker _faker = new Faker();

        public RaidCreateArguments CreateRaidCreateArguments()
        {
            return new RaidCreateArguments
            {
                Title = _faker.Commerce.ProductName(),
                Description = _faker.Lorem.Paragraph(),
                StartTimeUtc = DateTime.UtcNow.AddDays(-1),
                EndTimeUtc = DateTime.UtcNow.AddDays(1)
            };
        }
    }
}