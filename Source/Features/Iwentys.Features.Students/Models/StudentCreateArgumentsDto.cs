﻿using Iwentys.Features.Students.Enums;

namespace Iwentys.Features.Students.Models
{
    public record StudentCreateArgumentsDto
    {
        public StudentCreateArgumentsDto(int id, string firstName, string middleName, string secondName, UserType role, string @group, string githubUsername, int barsPoints)
        {
            Id = id;
            FirstName = firstName;
            MiddleName = middleName;
            SecondName = secondName;
            Role = role;
            Group = @group;
            GithubUsername = githubUsername;
            BarsPoints = barsPoints;
        }

        public StudentCreateArgumentsDto()
        {
        }
        
        public int Id { get; init; }
        public string FirstName { get; init; }
        public string MiddleName { get; init; }
        public string SecondName { get; init; }
        public UserType Role { get; init; }
        public string Group { get; init; }
        public string GithubUsername { get; init; }
        public int BarsPoints { get; init; }
    }
}