﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iwentys.Common.Databases;
using Iwentys.Features.AccountManagement.Domain;
using Iwentys.Features.AccountManagement.Entities;
using Iwentys.Features.Achievements.Domain;
using Iwentys.Features.GithubIntegration.Services;
using Iwentys.Features.Guilds.Domain;
using Iwentys.Features.Guilds.Entities;
using Iwentys.Features.Guilds.Repositories;
using Iwentys.Features.Guilds.Tournaments.Domain;
using Iwentys.Features.Guilds.Tournaments.Entities;
using Iwentys.Features.Guilds.Tournaments.Models;
using Microsoft.EntityFrameworkCore;

namespace Iwentys.Features.Guilds.Tournaments.Services
{
    public class TournamentService
    {
        private readonly AchievementProvider _achievementProvider;
        private readonly IGenericRepository<CodeMarathonTournament> _codeMarathonTournamentRepository;
        private readonly GithubIntegrationService _githubIntegrationService;
        private readonly IGenericRepository<GuildMember> _guildMemberRepository;
        private readonly IGenericRepository<Guild> _guildRepository;

        private readonly IGenericRepository<IwentysUser> _studentRepository;
        private readonly IGenericRepository<Tournament> _tournamentRepository;
        private readonly IGenericRepository<TournamentParticipantTeam> _tournamentTeamRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TournamentService(GithubIntegrationService githubIntegrationService, IUnitOfWork unitOfWork, AchievementProvider achievementProvider)
        {
            _unitOfWork = unitOfWork;
            _achievementProvider = achievementProvider;

            _studentRepository = _unitOfWork.GetRepository<IwentysUser>();
            _guildRepository = _unitOfWork.GetRepository<Guild>();
            _guildMemberRepository = _unitOfWork.GetRepository<GuildMember>();
            _tournamentRepository = _unitOfWork.GetRepository<Tournament>();
            _tournamentTeamRepository = _unitOfWork.GetRepository<TournamentParticipantTeam>();
            _codeMarathonTournamentRepository = _unitOfWork.GetRepository<CodeMarathonTournament>();
            _githubIntegrationService = githubIntegrationService;
        }

        public async Task<List<TournamentInfoResponse>> GetActive()
        {
            List<TournamentInfoResponse> result = await _tournamentRepository
                .Get()
                .Where(t => t.StartTime < DateTime.UtcNow && t.EndTime > DateTime.UtcNow)
                .Select(TournamentInfoResponse.FromEntity)
                .ToListAsync();

            result.ForEach(r => r.OrderByRate());
            return result;
        }

        public async Task<List<TournamentInfoResponse>> Get()
        {
            List<TournamentInfoResponse> result = await _tournamentRepository
                .Get()
                .Select(TournamentInfoResponse.FromEntity)
                .ToListAsync();

            result.ForEach(r => r.OrderByRate());
            return result;
        }

        public async Task<TournamentInfoResponse> Get(int tournamentId)
        {
            TournamentInfoResponse result = await _tournamentRepository
                .Get()
                .Select(TournamentInfoResponse.FromEntity)
                .SingleAsync(t => t.Id == tournamentId);

            return result.OrderByRate();
        }

        public async Task<TournamentInfoResponse> FindGuildActiveTournament(int guildId)
        {
            TournamentInfoResponse result = await _tournamentTeamRepository
                .Get()
                .Where(tt => tt.GuildId == guildId)
                .Select(tt => tt.Tournament)
                .Select(TournamentInfoResponse.FromEntity)
                .FirstOrDefaultAsync();

            return result.OrderByRate();
        }

        public async Task<TournamentLeaderboardDto> GetLeaderboard(int tournamentId)
        {
            Tournament tournament = await _tournamentRepository.GetById(tournamentId);
            return tournament
                .WrapToDomain(_githubIntegrationService, _unitOfWork, _achievementProvider)
                .GetLeaderboard();
        }

        public async Task<TournamentInfoResponse> CreateCodeMarathon(AuthorizedUser user, CreateCodeMarathonTournamentArguments arguments)
        {
            SystemAdminUser systemAdminUser = (await _studentRepository.GetById(user.Id)).EnsureIsAdmin();

            var codeMarathonTournamentEntity = CodeMarathonTournament.Create(systemAdminUser, arguments);

            await _tournamentRepository.InsertAsync(codeMarathonTournamentEntity.Tournament);
            await _codeMarathonTournamentRepository.InsertAsync(codeMarathonTournamentEntity);
            await _unitOfWork.CommitAsync();

            return await Get(codeMarathonTournamentEntity.Id);
        }

        public async Task RegisterToTournament(AuthorizedUser user, int tournamentId)
        {
            IwentysUser studentEntity = await _studentRepository.GetById(user.Id);
            Guild guild = _guildMemberRepository.ReadForStudent(user.Id);
            GuildMentor guildMentorUser = studentEntity.EnsureIsGuildMentor(guild);
            Tournament tournamentEntity = await _tournamentRepository.GetById(tournamentId);
            List<GuildMember> members = await _guildRepository
                .Get()
                .SelectMany(g => g.Members)
                .ToListAsync();

            TournamentParticipantTeam tournamentParticipantTeamEntity = tournamentEntity.RegisterTeam(guildMentorUser.Guild, members);

            await _tournamentTeamRepository.InsertAsync(tournamentParticipantTeamEntity);
            await _unitOfWork.CommitAsync();
        }

        public async Task FinishTournamentManually(AuthorizedUser user, int tournamentId)
        {
            IwentysUser studentEntity = await _studentRepository.GetById(user.Id);
            Tournament tournamentEntity = await _tournamentRepository.GetById(tournamentId);

            tournamentEntity.FinishManually(studentEntity);
            _tournamentRepository.Update(tournamentEntity);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateResult(int tournamentId)
        {
            Tournament tournamentEntity = await _tournamentRepository.GetById(tournamentId);
            ITournamentDomain tournamentDomain = tournamentEntity.WrapToDomain(_githubIntegrationService, _unitOfWork, _achievementProvider);

            await tournamentDomain.UpdateResult();
        }
    }
}