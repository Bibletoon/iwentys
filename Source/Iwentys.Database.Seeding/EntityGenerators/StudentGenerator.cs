﻿using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Iwentys.Database.Seeding.Tools;
using Iwentys.Features.StudentFeature.Entities;
using Iwentys.Features.StudentFeature.Enums;

namespace Iwentys.Database.Seeding.EntityGenerators
{
    public class StudentGenerator
    {
        private const int StudentCount = 100;

        public StudentGenerator(List<StudyGroupEntity> studyGroups)
        {
            Faker = new Faker<StudentEntity>()
                .RuleFor(s => s.Id, f => f.IndexFaker++ + 1)
                .RuleFor(s => s.FirstName, f => f.Name.FirstName())
                .RuleFor(s => s.SecondName, f => f.Name.LastName())
                .RuleFor(s => s.Role, UserType.Common)
                .RuleFor(s => s.Type, StudentType.Budgetary)
                .RuleFor(s => s.CreationTime, DateTime.UtcNow)
                .RuleFor(s => s.LastOnlineTime, DateTime.UtcNow)
                .RuleFor(s => s.GroupId, _ => studyGroups.GetRandom().Id)
                .RuleFor(s => s.AvatarUrl, f => f.Image.PicsumUrl())
                .RuleFor(s => s.GithubUsername, f => f.Person.UserName);

            Students = Faker.Generate(StudentCount);
            Students.Add(new StudentEntity
            {
                Id = 228617,
                FirstName = "Фреди",
                MiddleName = "Кисикович",
                SecondName = "Катс",
                Role = UserType.Admin,
                GroupId = studyGroups.First(g => g.GroupName.Contains("3505")).Id,
                GithubUsername = "InRedikaWB",
                CreationTime = DateTime.UtcNow,
                LastOnlineTime = DateTime.UtcNow,
                BarsPoints = short.MaxValue,
                AvatarUrl = new Faker().Image.PicsumUrl()
            });
        }

        public Faker<StudentEntity> Faker { get; }
        public List<StudentEntity> Students { get; set; }
    }
}