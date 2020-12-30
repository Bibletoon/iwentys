﻿using Iwentys.Features.Students.Entities;

namespace Iwentys.Features.Gamification.Entities
{
    public class StudentInterestTag
    {
        public int InterestTagId { get; init; }
        public virtual InterestTag InterestTag { get; init; }

        public int StudentId { get; init; }
        public virtual Student Student { get; init; }
    }
}