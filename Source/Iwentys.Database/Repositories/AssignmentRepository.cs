﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Iwentys.Database.Context;
using Iwentys.Features.Assignments.Entities;
using Iwentys.Features.Assignments.Repositories;
using Iwentys.Features.Assignments.ViewModels;
using Iwentys.Features.StudentFeature.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Iwentys.Database.Repositories
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly IwentysDbContext _dbContext;

        public AssignmentRepository(IwentysDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<StudentAssignmentEntity> CreateAsync(StudentEntity creator, AssignmentCreateRequest assignmentCreateRequest)
        {
            EntityEntry<AssignmentEntity> createdAssignment = await _dbContext.Assignments.AddAsync(AssignmentEntity.Create(creator, assignmentCreateRequest));
            EntityEntry<StudentAssignmentEntity> studentAssignment = await _dbContext.StudentAssignments.AddAsync(new StudentAssignmentEntity
            {
                StudentId = creator.Id,
                Assignment = createdAssignment.Entity
            });

            await _dbContext.SaveChangesAsync();
            return studentAssignment.Entity;
        }

        public IQueryable<StudentAssignmentEntity> Read()
        {
            return _dbContext.StudentAssignments
                .Include(sa => sa.Assignment)
                .ThenInclude(a => a.Subject);
        }

        public async Task<AssignmentEntity> MarkCompleted(int assignmentId)
        {
            AssignmentEntity assignmentEntity = await _dbContext.Assignments.FindAsync(assignmentId);
            if (assignmentEntity is null)
                //TODO: meh
                throw new Exception();
            assignmentEntity.IsCompleted = true;
            EntityEntry<AssignmentEntity> result = _dbContext.Assignments.Update(assignmentEntity);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }
    }
}