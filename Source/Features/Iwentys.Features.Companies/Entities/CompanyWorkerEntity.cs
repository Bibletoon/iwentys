﻿using Iwentys.Features.Companies.Enums;
using Iwentys.Features.Students.Entities;

namespace Iwentys.Features.Companies.Entities
{
    public class CompanyWorkerEntity
    {
        public int CompanyId { get; set; }
        public virtual CompanyEntity CompanyEntity { get; set; }

        public int WorkerId { get; set; }
        public virtual StudentEntity Worker { get; set; }

        public CompanyWorkerType Type { get; set; }

        public static CompanyWorkerEntity NewRequest(CompanyEntity companyEntity, StudentEntity worker)
        {
            return new CompanyWorkerEntity
            {
                CompanyEntity = companyEntity,
                Worker = worker,
                Type = CompanyWorkerType.Requested
            };
        }
        
        public void Approve()
        {
            //TODO: some... more validation?
            Type = CompanyWorkerType.Accepted;
        }
    }
}