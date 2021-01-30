﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Iwentys.Features.AccountManagement.Models;
using Iwentys.Features.Quests.Models;

namespace Iwentys.Endpoint.Client.Pages.Quests
{
    public partial class QuestExecutorRating
    {
        private List<QuestRatingRow> _questExecutorRating;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _questExecutorRating = await ClientHolder.Quest.GetQuestExecutorRating();
        }

        private string LinkToProfile(IwentysUserInfoDto user) => $"student/profile/{user.Id}";

    }
}