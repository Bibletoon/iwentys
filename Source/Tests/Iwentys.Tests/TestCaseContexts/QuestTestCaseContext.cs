﻿using Iwentys.Database.Seeding.FakerEntities;
using Iwentys.Domain.AccountManagement;
using Iwentys.Domain.Quests.Dto;

namespace Iwentys.Tests.TestCaseContexts
{
    public class QuestTestCaseContext
    {
        private readonly TestCaseContext _context;

        public QuestTestCaseContext(TestCaseContext context)
        {
            _context = context;
        }

        public QuestInfoDto WithQuest(AuthorizedUser user, int price)
        {
            return _context.QuestService.Create(user, QuestFaker.Instance.CreateQuestRequest(price)).Result;
        }
    }
}