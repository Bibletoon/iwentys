﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Iwentys.Domain.AccountManagement;
using Iwentys.Domain.Extended.Models;
using Iwentys.FeatureBase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Iwentys.Features.Extended.Newsfeeds
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsfeedController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NewsfeedController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost(nameof(CreateSubjectNewsfeed))]
        public async Task<ActionResult> CreateSubjectNewsfeed(NewsfeedCreateViewModel createViewModel, int subjectId)
        {
            AuthorizedUser authorizedUser = this.TryAuthWithToken();
            CreateSubjectNewsfeed.Response response = await _mediator.Send(new CreateSubjectNewsfeed.Query(createViewModel, authorizedUser, subjectId));
            return Ok();
        }

        [HttpPost(nameof(CreateGuildNewsfeed))]
        public async Task<ActionResult> CreateGuildNewsfeed(NewsfeedCreateViewModel createViewModel, int subjectId)
        {
            AuthorizedUser authorizedUser = this.TryAuthWithToken();
            CreateGuildNewsfeed.Response response = await _mediator.Send(new CreateGuildNewsfeed.Query(createViewModel, authorizedUser, subjectId));
            return Ok();
        }

        [HttpGet(nameof(GetBySubjectId))]
        public async Task<ActionResult<List<NewsfeedViewModel>>> GetBySubjectId(int subjectId)
        {
            AuthorizedUser authorizedUser = this.TryAuthWithToken();
            GetSubjectNewsfeeds.Response response = await _mediator.Send(new GetSubjectNewsfeeds.Query(authorizedUser, subjectId));
            return Ok(response.Newsfeeds);
        }

        [HttpGet(nameof(GetByGuildId))]
        public async Task<ActionResult<List<NewsfeedViewModel>>> GetByGuildId(int guildId)
        {
            AuthorizedUser authorizedUser = this.TryAuthWithToken();
            GetGuildNewsfeeds.Response response = await _mediator.Send(new GetGuildNewsfeeds.Query(authorizedUser, guildId));
            return Ok(response.Newsfeeds);
        }
    }
}