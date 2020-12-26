﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iwentys.Common.Databases;
using Iwentys.Common.Exceptions;
using Iwentys.Common.Tools;
using Iwentys.Features.GithubIntegration.Services;
using Iwentys.Features.Guilds.Entities;
using Iwentys.Features.Guilds.Enums;
using Iwentys.Features.Guilds.Models.Guilds;
using Iwentys.Features.Guilds.Repositories;
using Iwentys.Features.Students.Domain;
using Iwentys.Features.Students.Entities;
using Iwentys.Features.Students.Models;

namespace Iwentys.Features.Guilds.Domain
{
    public class GuildDomain
    {
        public GuildEntity Profile { get; }

        private readonly GithubIntegrationService _githubIntegrationService;
        private readonly IGenericRepository<StudentEntity> _studentRepository;
        private readonly IGenericRepository<GuildMemberEntity> _guildMemberRepositoryNew;

        public GuildDomain(
            GuildEntity profile,
            GithubIntegrationService githubIntegrationService,
            IGenericRepository<StudentEntity> studentRepository,
            IGenericRepository<GuildMemberEntity> guildMemberRepositoryNew)
        {
            Profile = profile;
            _githubIntegrationService = githubIntegrationService;
            _studentRepository = studentRepository;
            _guildMemberRepositoryNew = guildMemberRepositoryNew;
        }

        public async Task<ExtendedGuildProfileWithMemberDataDto> ToExtendedGuildProfileDto(int? userId = null)
        {
            var info = new ExtendedGuildProfileWithMemberDataDto(Profile)
            {
                Leader = Profile.Members.Single(m => m.MemberType == GuildMemberType.Creator).Member.To(s => new StudentInfoDto(s)),
                //TODO; return result
                PinnedRepositories = Profile.PinnedProjects.SelectToList(p => _githubIntegrationService.GetRepository(p.RepositoryOwner, p.RepositoryName).Result),
            };

            if (userId is not null)
                info.UserMembershipState = await GetUserMembershipState(userId.Value);

            return info;
        }

        public async Task<List<GuildMemberImpactDto>> GetMemberImpacts()
        {
            //TODO: move to SQL
            List<GuildMemberImpactDto> result = new List<GuildMemberImpactDto>();
            foreach (var member in Profile.Members)
            {
                if (member.Member.GithubUsername is null)
                {
                    result.Add(new GuildMemberImpactDto(new StudentInfoDto(member.Member), member.MemberType));
                    continue;
                }

                var githubUser = await _githubIntegrationService.GetGithubUser(member.Member.GithubUsername);
                if (githubUser is null)
                {
                    result.Add(new GuildMemberImpactDto(new StudentInfoDto(member.Member), member.MemberType));
                    continue;
                }

                result.Add(new GuildMemberImpactDto(new StudentInfoDto(member.Member), member.MemberType, githubUser.ContributionFullInfo));
            }

            return result;
        }

        public async Task<GuildMemberLeaderBoardDto> GetMemberDashboard()
        {
            List<GuildMemberImpactDto> members = await GetMemberImpacts();
            return new GuildMemberLeaderBoardDto(members);
        }

        public async Task<UserMembershipState> GetUserMembershipState(Int32 userId)
        {
            StudentEntity user = await _studentRepository.FindByIdAsync(userId);
            GuildEntity userGuild = _guildMemberRepositoryNew.ReadForStudent(user.Id);
            GuildMemberType? userStatusInGuild = Profile.Members.Find(m => m.Member.Id == user.Id)?.MemberType;

            if (userStatusInGuild == GuildMemberType.Blocked)
                return UserMembershipState.Blocked;

            if (userGuild is not null &&
                userGuild.Id != Profile.Id)
                return UserMembershipState.Blocked;

            if (userGuild is not null &&
                userGuild.Id == Profile.Id)
                return UserMembershipState.Entered;

            if (_guildMemberRepositoryNew.IsStudentHaveRequest(userId) &&
                userStatusInGuild != GuildMemberType.Requested)
                return UserMembershipState.Blocked;

            if (_guildMemberRepositoryNew.IsStudentHaveRequest(userId) &&
                userStatusInGuild == GuildMemberType.Requested)
                return UserMembershipState.Requested;

            if (userGuild is null &&
                userStatusInGuild != GuildMemberType.Requested &&
                DateTime.UtcNow < user.GuildLeftTime.AddHours(24))
                return UserMembershipState.Blocked;

            if (userGuild is null && Profile.HiringPolicy == GuildHiringPolicy.Open)
                return UserMembershipState.CanEnter;

            if (userGuild is null && Profile.HiringPolicy == GuildHiringPolicy.Close)
                return UserMembershipState.CanRequest;

            return UserMembershipState.Blocked;
        }

        //TODO: use in daemon
        //public GuildDomain UpdateGuildFromGithub()
        //{
        //    Organization organizationInfo = _apiAccessor.FindOrganizationInfo(Profile.Title);
        //    if (organizationInfo is not null)
        //    {
        //        //TODO: need to fix after https://github.com/octokit/octokit.net/pull/2239
        //        //_profile.Bio = organizationInfo.Bio;
        //        Profile.LogoUrl = organizationInfo.Url;
        //        _guildRepository.UpdateAsync(Profile);
        //    }

        //    return this;
        //}

        public async Task<GuildMemberEntity> EnsureMemberCanRestrictPermissionForOther(AuthorizedUser editor, int memberToKickId)
        {
            StudentEntity editorStudentAccount = await _studentRepository.FindByIdAsync(editor.Id);
            editorStudentAccount.EnsureIsGuildEditor(Profile);

            GuildMemberEntity memberToKick = Profile.Members.Find(m => m.MemberId == memberToKickId);
            GuildMemberEntity editorMember = Profile.Members.Find(m => m.MemberId == editor.Id) ?? throw new EntityNotFoundException(nameof(GuildMemberEntity));

            //TODO: check
            //if (memberToKick is null || !memberToKick.MemberType.IsMember())
            if (memberToKick is null)
                throw InnerLogicException.Guild.IsNotGuildMember(editor.Id, Profile.Id);

            if (memberToKick.MemberType == GuildMemberType.Creator)
                throw InnerLogicException.Guild.StudentCannotBeBlocked(memberToKickId, Profile.Id);

            if (memberToKick.MemberType == GuildMemberType.Mentor && editorMember.MemberType == GuildMemberType.Mentor)
                throw InnerLogicException.Guild.StudentCannotBeBlocked(memberToKickId, Profile.Id);

            return memberToKick;
        }
    }
}