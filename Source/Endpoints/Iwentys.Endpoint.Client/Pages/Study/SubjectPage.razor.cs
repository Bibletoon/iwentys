﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Iwentys.Endpoint.Client.Tools;
using Iwentys.Endpoint.Sdk.ControllerClients;
using Iwentys.Endpoint.Sdk.ControllerClients.Study;
using Iwentys.Features.Newsfeeds.Models;
using Iwentys.Features.Study.Models;
using Microsoft.AspNetCore.Components;

namespace Iwentys.Endpoint.Client.Pages.Study
{
    public partial class SubjectPage : ComponentBase
    {
        private SubjectProfileDto _subjectProfile;
        private List<NewsfeedViewModel> _newsfeeds;

        protected override async Task OnInitializedAsync()
        {
            var httpClient = await Http.TrySetHeader(LocalStorage);
            var studentControllerClient = new SubjectControllerClient(httpClient);
            var newsfeedControllerClient = new NewsfeedControllerClient(httpClient);

            _subjectProfile = await studentControllerClient.GetProfile(SubjectId);
            _newsfeeds = await newsfeedControllerClient.GetForSubject(SubjectId);
        }
    }
}