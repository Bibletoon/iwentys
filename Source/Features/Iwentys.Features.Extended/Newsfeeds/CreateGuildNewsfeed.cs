﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Iwentys.Common.Databases;
using Iwentys.Domain.AccountManagement;
using Iwentys.Domain.Extended;
using Iwentys.Domain.Extended.Models;
using Iwentys.Domain.GithubIntegration;
using Iwentys.Domain.GithubIntegration.Models;
using Iwentys.Domain.Guilds;
using Iwentys.Domain.Study;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Iwentys.Features.Extended.Newsfeeds
{
    public class CreateGuildNewsfeed
    {
        public class Query : IRequest<Response>
        {
            public NewsfeedCreateViewModel CreateViewModel { get; }
            public AuthorizedUser AuthorizedUser { get; }
            public int GuildId { get; }

            public Query(NewsfeedCreateViewModel createViewModel, AuthorizedUser authorizedUser, int guildId)
            {
                CreateViewModel = createViewModel;
                AuthorizedUser = authorizedUser;
                GuildId = guildId;
            }
        }

        public class Response
        {
            public Response(NewsfeedViewModel newsfeeds)
            {
                Newsfeeds = newsfeeds;
            }

            public NewsfeedViewModel Newsfeeds { get; }
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly IGenericRepository<GuildNewsfeed> _guildNewsfeedRepository;
            private readonly IGenericRepository<SubjectNewsfeed> _subjectNewsfeedRepository;
            private readonly IGenericRepository<Newsfeed> _newsfeedRepository;
            private readonly IGenericRepository<Guild> _guildRepository;
            private readonly IGenericRepository<IwentysUser> _iwentysUserRepository;
            private readonly IGenericRepository<Student> _studentRepository;
            private readonly IGenericRepository<Subject> _subjectRepository;
            private readonly IUnitOfWork _unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;

                _studentRepository = _unitOfWork.GetRepository<Student>();
                _subjectRepository = _unitOfWork.GetRepository<Subject>();
                _guildRepository = _unitOfWork.GetRepository<Guild>();
                _subjectNewsfeedRepository = _unitOfWork.GetRepository<SubjectNewsfeed>();
                _guildNewsfeedRepository = _unitOfWork.GetRepository<GuildNewsfeed>();
                _newsfeedRepository = _unitOfWork.GetRepository<Newsfeed>();
                _iwentysUserRepository = _unitOfWork.GetRepository<IwentysUser>();
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                Student author = await _studentRepository.GetById(request.AuthorizedUser.Id);
                Guild subject = await _guildRepository.GetById(request.GuildId);

                GuildMentor mentor = await author.EnsureIsGuildMentor(_guildRepository, request.GuildId);
                var newsfeedEntity = GuildNewsfeed.Create(request.CreateViewModel, mentor, subject);

                _guildNewsfeedRepository.Insert(newsfeedEntity);
                await _unitOfWork.CommitAsync();

                NewsfeedViewModel result = await _newsfeedRepository
                    .Get()
                    .Where(n => n.Id == newsfeedEntity.NewsfeedId)
                    .Select(NewsfeedViewModel.FromEntity)
                    .SingleAsync();

                return new Response(result);
            }
        }
    }
}