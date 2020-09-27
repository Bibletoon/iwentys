﻿using System.Collections.Generic;
using System.Linq;
using Iwentys.Models.Entities.Study;
using Iwentys.Models.Tools;
using Iwentys.Models.Transferable.Students;

namespace Iwentys.Models.Transferable.Study
{
    public class StudyLeaderboardRow : IResultFormat
    {
        public StudyLeaderboardRow()
        {
        }

        public StudyLeaderboardRow(IEnumerable<SubjectActivityEntity> activity)
        {
            List<SubjectActivityEntity> subjectActivityEntities = activity.ToList();
            Student = subjectActivityEntities[0].Student.To(s => new StudentPartialProfileDto(s));
            Activity = subjectActivityEntities.Sum(a => a.Points);
        }

        public StudentPartialProfileDto Student { get; set; }
        public double Activity { get; set; }

        public string Format()
        {
            return $"{Student.GetFullName()} - {Activity}";
        }
    }
}