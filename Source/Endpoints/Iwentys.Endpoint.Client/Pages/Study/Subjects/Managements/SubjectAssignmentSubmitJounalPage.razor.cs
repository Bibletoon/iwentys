﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Iwentys.Sdk;

namespace Iwentys.Endpoint.Client.Pages.Study.Subjects.Managements
{
    public partial class SubjectAssignmentSubmitJounalPage
    {
        private ICollection<SubjectAssignmentSubmitDto> _subjectAssignmentSubmits;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _subjectAssignmentSubmits = await SubjectAssignmentSubmitClient.GetBySubjectIdAsync(SubjectId);
        }

        private void NavigateToSubmitPage(object row)
        {
            //TODO: replace exception
            if (row is not SubjectAssignmentSubmitDto submit)
                throw new Exception("Something goes wrong.");

            NavigationManager.NavigateTo($"/subject/{SubjectId}/management/assignments/submits/{submit.Id}");
        }
    }
}
