﻿using Iwentys.Features.GithubIntegration.Entities;

namespace Iwentys.Features.GithubIntegration.Models
{
    public record GithubRepositoryInfoDto
    {
        public GithubRepositoryInfoDto(GithubProjectEntity entity)
        : this(
            entity.Id,
            entity.Owner,
            entity.Name,
            entity.Description,
            entity.FullUrl,
            entity.StarCount)
        {
        }

        public GithubRepositoryInfoDto(long id, string owner, string name, string description, string url, int starCount)
        {
            Id = id;
            Owner = owner;
            Name = name;
            Description = description;
            Url = url;
            StarCount = starCount;
        }

        public GithubRepositoryInfoDto()
        {
        }

        public long Id { get; init; }
        public string Owner { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public string Url { get; init; }
        public int StarCount { get; init; }
    }
}