﻿using System;
using Iwentys.Models.Entities.Guilds;

namespace Iwentys.Models.Entities.Gamification
{
    public class GuildAchievementEntity
    {
        public int GuildId { get; set; }
        public GuildEntity Guild { get; set; }
        public int AchievementId { get; set; }
        public AchievementEntity Achievement { get; set; }

        public DateTime GettingTime { get; set; }
    }
}