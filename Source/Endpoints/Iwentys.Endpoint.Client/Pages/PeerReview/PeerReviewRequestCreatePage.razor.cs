﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Iwentys.Features.GithubIntegration.Models;
using Iwentys.Features.PeerReview.Enums;
using Iwentys.Features.PeerReview.Models;

namespace Iwentys.Endpoint.Client.Pages.PeerReview
{
    public partial class PeerReviewRequestCreatePage
    {
        private List<GithubRepositoryInfoDto> _availableForReviewProject;
        private GithubRepositoryInfoDto _selectedProject;
        private string _description;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _availableForReviewProject = await ClientHolder.PeerReview.GetAvailableForReviewProject();
            //FYI: this value is used in selector
            _availableForReviewProject.Insert(0, null);
        }

        private async Task CreateRequest()
        {
            var arguments = new ReviewRequestCreateArguments
            {
                ProjectId = _selectedProject.Id,
                Description = _description,
                Visibility = ProjectReviewVisibility.Open
            };

            await ClientHolder.PeerReview.CreateReviewRequest(arguments);
            NavigationManager.NavigateTo("/peer-review");
        }
    }
}
