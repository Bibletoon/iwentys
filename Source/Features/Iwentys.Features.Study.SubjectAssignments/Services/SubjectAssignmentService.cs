﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iwentys.Common.Databases;
using Iwentys.Common.Tools;
using Iwentys.Features.AccountManagement.Domain;
using Iwentys.Features.AccountManagement.Entities;
using Iwentys.Features.Assignments.Entities;
using Iwentys.Features.Assignments.Models;
using Iwentys.Features.Study.Entities;
using Iwentys.Features.Study.SubjectAssignments.Domain;
using Iwentys.Features.Study.SubjectAssignments.Entities;
using Iwentys.Features.Study.SubjectAssignments.Models;
using Microsoft.EntityFrameworkCore;

namespace Iwentys.Features.Study.SubjectAssignments.Services
{
    public class SubjectAssignmentService
    {
        private readonly IGenericRepository<GroupSubjectAssignment> _groupSubjectAssignmentRepository;
        private readonly IGenericRepository<GroupSubject> _groupSubjectRepository;

        private readonly IGenericRepository<IwentysUser> _iwentysUserRepository;
        private readonly IGenericRepository<Assignment> _assignmentRepository;
        private readonly IGenericRepository<StudentAssignment> _studentAssignmentRepository;
        private readonly IGenericRepository<SubjectAssignment> _subjectAssignmentRepository;
        private readonly IGenericRepository<SubjectAssignmentSubmit> _subjectAssignmentSubmitRepository;
        private readonly IGenericRepository<Subject> _subjectRepository;
        private readonly IGenericRepository<Student> _studentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SubjectAssignmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            _iwentysUserRepository = _unitOfWork.GetRepository<IwentysUser>();
            _subjectAssignmentRepository = _unitOfWork.GetRepository<SubjectAssignment>();
            _subjectAssignmentSubmitRepository = _unitOfWork.GetRepository<SubjectAssignmentSubmit>();
            _groupSubjectAssignmentRepository = _unitOfWork.GetRepository<GroupSubjectAssignment>();
            _groupSubjectRepository = _unitOfWork.GetRepository<GroupSubject>();
            _subjectRepository = _unitOfWork.GetRepository<Subject>();
            _studentRepository = _unitOfWork.GetRepository<Student>();
            _assignmentRepository = _unitOfWork.GetRepository<Assignment>();
            _studentAssignmentRepository = _unitOfWork.GetRepository<StudentAssignment>();
        }

        //TODO: if user is not teacher - filter submits
        public async Task<List<SubjectAssignmentDto>> GetSubjectAssignmentForGroup(AuthorizedUser user, int groupId)
        {
            return await _groupSubjectAssignmentRepository
                .Get()
                .Where(gsa => gsa.GroupId == groupId)
                .Select(gsa => gsa.SubjectAssignment)
                .Distinct()
                .Select(SubjectAssignmentDto.FromEntity)
                .ToListAsync();
        }

        public async Task<List<SubjectAssignmentDto>> GetSubjectAssignmentForSubject(AuthorizedUser user, int subjectId)
        {
            //TODO: it is not work?
            return await _subjectAssignmentRepository
                .Get()
                .Where(sa => sa.SubjectId == subjectId)
                .Select(SubjectAssignmentDto.FromEntity)
                .ToListAsync();
        }

        //TODO: here must be subject id and we need to resolve group subject
        //TODO: OR list of group subject ids
        public async Task<SubjectAssignmentDto> CreateSubjectAssignment(AuthorizedUser user, int subjectId, SubjectAssignmentCreateArguments arguments)
        {
            Subject subject = await _subjectRepository.GetById(subjectId);
            IwentysUser iwentysUser = await _iwentysUserRepository.GetById(user.Id);
            SubjectTeacher teacher = iwentysUser.EnsureIsTeacher(subject);

            List<Student> members = await _groupSubjectRepository
                .Get()
                .Where(gs => gs.SubjectId == subjectId)
                .Select(gs => gs.StudyGroup)
                .SelectMany(g => g.Students)
                .ToListAsync();

            var assignmentCreateArguments = new AssignmentCreateArguments
            {
                Title = arguments.Title,
                Description = arguments.Description,
                DeadlineTimeUtc = arguments.DeadlineUtc,
                SubjectId = subjectId,
                Link = arguments.Link
            };

            var assignment = Assignment.Create(iwentysUser, assignmentCreateArguments);
            await _assignmentRepository.InsertAsync(assignment);
            List<StudentAssignment> studentAssignments = members.SelectToList(s => new StudentAssignment
            {
                StudentId = s.Id,
                Assignment = assignment,
                LastUpdateTimeUtc = DateTime.UtcNow
            });

            var subjectAssignment = SubjectAssignment.Create(teacher, subject, assignment);

            await _subjectAssignmentRepository.InsertAsync(subjectAssignment);
            await _studentAssignmentRepository.InsertAsync(studentAssignments);

            await _unitOfWork.CommitAsync();
            return await _subjectAssignmentRepository
                .Get()
                .Where(sa => sa.Id == subjectAssignment.Id)
                .Select(SubjectAssignmentDto.FromEntity)
                .SingleAsync();
        }

        public async Task<List<SubjectAssignmentSubmitDto>> SearchSubjectAssignmentSubmits(AuthorizedUser user, SubjectAssignmentSubmitSearchArguments searchArguments)
        {
            Subject subject = await _subjectRepository.GetById(searchArguments.SubjectId);
            IwentysUser iwentysUser = await _iwentysUserRepository.GetById(user.Id);
            SubjectTeacher teacher = iwentysUser.EnsureIsTeacher(subject);

            return await SearchSubjectAssignmentSubmits(searchArguments);
        }

        public async Task<List<SubjectAssignmentSubmitDto>> GetStudentSubjectAssignmentSubmits(AuthorizedUser user, SubjectAssignmentSubmitSearchArguments searchArguments)
        {
            Subject subject = await _subjectRepository.GetById(searchArguments.SubjectId);
            IwentysUser iwentysUser = await _iwentysUserRepository.GetById(user.Id);

            return await SearchSubjectAssignmentSubmits(searchArguments);
        }

        public async Task<SubjectAssignmentSubmitDto> SendSubmit(AuthorizedUser user, int subjectAssignmentId, SubjectAssignmentSubmitCreateArguments arguments)
        {
            SubjectAssignment subjectAssignment = await _subjectAssignmentRepository.GetById(subjectAssignmentId);

            SubjectAssignmentSubmit subjectAssignmentSubmit = subjectAssignment.CreateSubmit(user, arguments);

            await _subjectAssignmentSubmitRepository.InsertAsync(subjectAssignmentSubmit);
            await _unitOfWork.CommitAsync();

            return await _subjectAssignmentSubmitRepository
                .Get()
                .Where(sas => sas.Id == subjectAssignmentSubmit.Id)
                .Select(sas => new SubjectAssignmentSubmitDto(sas))
                .SingleAsync();
        }

        public async Task<SubjectAssignmentSubmitDto> GetSubjectAssignmentSubmit(AuthorizedUser user, int subjectAssignmentSubmitId)
        {
            return await _subjectAssignmentSubmitRepository
                .Get()
                .Where(sas => sas.Id == subjectAssignmentSubmitId)
                .Select(sas => new SubjectAssignmentSubmitDto(sas))
                .SingleAsync();
        }

        public async Task SendFeedback(AuthorizedUser user, int subjectAssignmentSubmitId, SubjectAssignmentSubmitFeedbackArguments createArguments)
        {
            SubjectAssignmentSubmit subjectAssignmentSubmit = await _subjectAssignmentSubmitRepository.GetById(subjectAssignmentSubmitId);
            IwentysUser iwentysUser = await _iwentysUserRepository.GetById(user.Id);
            SubjectTeacher teacher = iwentysUser.EnsureIsTeacher(subjectAssignmentSubmit.SubjectAssignment.Subject);

            subjectAssignmentSubmit.ApplyFeedback(teacher, createArguments);

            _subjectAssignmentSubmitRepository.Update(subjectAssignmentSubmit);
            await _unitOfWork.CommitAsync();
        }

        //TODO: add pagination
        private async Task<List<SubjectAssignmentSubmitDto>> SearchSubjectAssignmentSubmits(SubjectAssignmentSubmitSearchArguments searchArguments)
        {
            return await _subjectAssignmentSubmitRepository
                .Get()
                .Where(sas => sas.SubjectAssignment.SubjectId == searchArguments.SubjectId)
                .WhereIf(searchArguments.StudentId, sas => sas.StudentId == searchArguments.StudentId)
                .Select(sas => new SubjectAssignmentSubmitDto(sas))
                .ToListAsync();
        }
    }
}