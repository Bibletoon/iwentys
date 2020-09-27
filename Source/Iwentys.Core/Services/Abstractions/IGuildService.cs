﻿using Iwentys.Core.DomainModel;
using Iwentys.Models.Entities.Github;
using Iwentys.Models.Transferable.Guilds;

namespace Iwentys.Core.Services.Abstractions
{
    public interface IGuildService
    {
        GuildProfileShortInfoDto Create(AuthorizedUser creator, GuildCreateArgumentDto arguments);
        GuildProfileShortInfoDto Update(AuthorizedUser user, GuildUpdateArgumentDto arguments);
        GuildProfileShortInfoDto ApproveGuildCreating(AuthorizedUser user, int guildId);

        GuildProfileDto[] Get();
        GuildProfilePreviewDto[] GetOverview(int skippedCount, int takenCount);
        GuildProfileDto Get(int id, int? userId);
        GuildProfileDto GetStudentGuild(int userId);

        GithubRepository AddPinnedRepository(AuthorizedUser user, int guildId, string owner, string projectName);
        void UnpinProject(AuthorizedUser user, int pinnedProjectId);
        GuildMemberLeaderBoard GetGuildMemberLeaderBoard(int guildId);
    }
}