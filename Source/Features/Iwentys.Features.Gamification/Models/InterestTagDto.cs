﻿using Iwentys.Features.Gamification.Entities;

namespace Iwentys.Features.Gamification.Models
{
    public class InterestTagDto
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public InterestTagDto(InterestTagEntity interestTag) : this()
        {
            Id = interestTag.Id;
            Title = interestTag.Title;
        }

        public InterestTagDto()
        {
        }
    }
}