﻿using Iwentys.Common.Databases;
using Iwentys.Common.Exceptions;
using Iwentys.Domain;
using Iwentys.Domain.Guilds;
using Iwentys.Domain.Models;
using Iwentys.Domain.Services;
using MediatR;

namespace Iwentys.Features.Guilds.Guilds
{
    public static class CreateGuild
    {
        public class Query : IRequest<Response>
        {
            public GuildCreateRequestDto Arguments { get; set; }
            public AuthorizedUser AuthorizedUser { get; set; }

            public Query(GuildCreateRequestDto arguments, AuthorizedUser authorizedUser)
            {
                Arguments = arguments;
                AuthorizedUser = authorizedUser;
            }
        }

        public class Response
        {
            public Response(GuildProfileShortInfoDto guild)
            {
                Guild = guild;
            }

            public GuildProfileShortInfoDto Guild { get; set; }
        }

        public class Handler : RequestHandler<Query, Response>
        {
            private readonly GithubIntegrationService _githubIntegrationService;

            private readonly IGenericRepository<GuildMember> _guildMemberRepository;
            private readonly IGenericRepository<GuildPinnedProject> _guildPinnedProjectRepository;
            private readonly IGenericRepository<Guild> _guildRepository;
            private readonly IGenericRepository<IwentysUser> _iwentysUserRepository;

            private readonly IUnitOfWork _unitOfWork;

            public Handler(IUnitOfWork unitOfWork, GithubIntegrationService githubIntegrationService)
            {
                _githubIntegrationService = githubIntegrationService;

                _unitOfWork = unitOfWork;
                _iwentysUserRepository = _unitOfWork.GetRepository<IwentysUser>();
                _guildRepository = _unitOfWork.GetRepository<Guild>();
                _guildMemberRepository = _unitOfWork.GetRepository<GuildMember>();
                _guildPinnedProjectRepository = _unitOfWork.GetRepository<GuildPinnedProject>();
            }

            protected override Response Handle(Query request)
            {
                IwentysUser creator = _iwentysUserRepository.GetById(request.AuthorizedUser.Id).Result;

                Guild userGuild = _guildMemberRepository.ReadForStudent(creator.Id);
                if (userGuild is not null)
                    throw new InnerLogicException("Student already in guild");

                var guildEntity = Guild.Create(creator, request.Arguments);
                _guildRepository.InsertAsync(guildEntity).Wait();
                _unitOfWork.CommitAsync().Wait();
                return new Response(new GuildProfileShortInfoDto(guildEntity));
            }
        }
    }
}