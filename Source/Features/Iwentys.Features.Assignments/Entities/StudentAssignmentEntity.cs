﻿using Iwentys.Features.StudentFeature.Entities;

namespace Iwentys.Features.Assignments.Entities
{
    public class StudentAssignmentEntity
    {
        public int AssignmentId { get; set; }
        public AssignmentEntity Assignment { get; set; }

        public int StudentId { get; set; }
        public StudentEntity Student { get; set; }
    }
}