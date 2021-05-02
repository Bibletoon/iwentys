﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iwentys.Common.Databases;
using Iwentys.Domain.AccountManagement;
using Iwentys.Domain.Extended;
using Iwentys.Domain.Extended.Models;
using Iwentys.Domain.Guilds;
using Iwentys.Domain.Study;
using Microsoft.EntityFrameworkCore;

namespace Iwentys.Features.Extended.Services
{
    public class NewsfeedService
    {
        private readonly IGenericRepository<GuildNewsfeed> _guildNewsfeedRepository;
        private readonly IGenericRepository<SubjectNewsfeed> _subjectNewsfeedRepository;
        private readonly IGenericRepository<Newsfeed> _newsfeedRepository;
        private readonly IGenericRepository<Guild> _guildRepository;
        private readonly IGenericRepository<IwentysUser> _iwentysUserRepository;
        private readonly IGenericRepository<Student> _studentRepository;
        private readonly IGenericRepository<Subject> _subjectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public NewsfeedService(IUnitOfWork unitOfWork)
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

        public async Task<NewsfeedViewModel> CreateSubjectNewsfeed(NewsfeedCreateViewModel createViewModel, AuthorizedUser authorizedUser, int subjectId)
        {
            IwentysUser author = await _iwentysUserRepository.GetById(authorizedUser.Id);
            Subject subject = await _subjectRepository.GetById(subjectId);

            SubjectNewsfeed newsfeedEntity;
            if (author.CheckIsAdmin(out SystemAdminUser admin))
            {
                newsfeedEntity = SubjectNewsfeed.CreateAsSystemAdmin(createViewModel, admin, subject);
            }
            else
            {
                Student student = await _studentRepository.GetById(author.Id);
                newsfeedEntity = SubjectNewsfeed.CreateAsGroupAdmin(createViewModel, student.EnsureIsGroupAdmin(), subject);
            }

            _subjectNewsfeedRepository.Insert(newsfeedEntity);
            await _unitOfWork.CommitAsync();

            return await _newsfeedRepository
                .Get()
                .Where(n => n.Id == newsfeedEntity.NewsfeedId)
                .Select(NewsfeedViewModel.FromEntity)
                .SingleAsync();
        }

        public async Task<NewsfeedViewModel> CreateGuildNewsfeed(NewsfeedCreateViewModel createViewModel, AuthorizedUser authorizedUser, int guildId)
        {
            Student author = await _studentRepository.GetById(authorizedUser.Id);
            Guild subject = await _guildRepository.GetById(guildId);

            GuildMentor mentor = await author.EnsureIsGuildMentor(_guildRepository, guildId);
            var newsfeedEntity = GuildNewsfeed.Create(createViewModel, mentor, subject);

            _guildNewsfeedRepository.Insert(newsfeedEntity);
            await _unitOfWork.CommitAsync();

            return await _newsfeedRepository
                .Get()
                .Where(n => n.Id == newsfeedEntity.NewsfeedId)
                .Select(NewsfeedViewModel.FromEntity)
                .SingleAsync();
        }

        public async Task<List<NewsfeedViewModel>> GetSubjectNewsfeeds(int subjectId)
        {
            return await _subjectNewsfeedRepository
                .Get()
                .Where(sn => sn.SubjectId == subjectId)
                .Select(NewsfeedViewModel.FromSubjectEntity)
                .ToListAsync();
        }

        public async Task<List<NewsfeedViewModel>> GetGuildNewsfeeds(int guildId)
        {
            return await _guildNewsfeedRepository.Get()
                .Where(gn => gn.GuildId == guildId)
                .Select(NewsfeedViewModel.FromGuildEntity)
                .ToListAsync();
        }
    }
}