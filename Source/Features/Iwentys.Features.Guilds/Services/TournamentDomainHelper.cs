﻿using Iwentys.Common.Exceptions;
using Iwentys.Domain.Guilds;
using Iwentys.Domain.Guilds.Enums;
using Iwentys.Features.GithubIntegration.GithubIntegration;

namespace Iwentys.Features.Guilds.Services
{
    public static class TournamentDomainHelper
    {
        public static ITournamentDomain WrapToDomain(
            this Tournament tournament,
            GithubIntegrationService githubIntegrationService)
        {
            return tournament.Type switch
            {
                TournamentType.CodeMarathon => new CodeMarathonTournamentDomain(tournament, githubIntegrationService.User),
                _ => throw InnerLogicException.NotSupportedEnumValue(tournament.Type)
            };
        }
    }
}