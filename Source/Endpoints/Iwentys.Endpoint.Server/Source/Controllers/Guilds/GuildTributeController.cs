﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Iwentys.Endpoint.Server.Source.Tools;
using Iwentys.Features.Guilds.Services;
using Iwentys.Features.Guilds.ViewModels.Guilds;
using Iwentys.Features.Guilds.ViewModels.GuildTribute;
using Iwentys.Features.StudentFeature;
using Microsoft.AspNetCore.Mvc;

namespace Iwentys.Endpoint.Server.Source.Controllers.Guilds
{
    [Route("api/guild/tribute")]
    [ApiController]
    public class GuildTributeController : ControllerBase
    {
        private readonly GuildTributeService _guildService;

        public GuildTributeController(GuildTributeService guildService)
        {
            _guildService = guildService;
        }

        [HttpGet("pending")]
        public ActionResult<List<TributeInfoResponse>> GetPendingTributes()
        {
            AuthorizedUser user = this.TryAuthWithToken();
            return Ok(_guildService.GetPendingTributes(user));
        }

        [HttpGet("get-for-student")]
        public ActionResult<List<TributeInfoResponse>> GetStudentTributeResult()
        {
            AuthorizedUser user = this.TryAuthWithToken();
            return Ok(_guildService.GetStudentTributeResult(user));
        }

        [HttpPost("create")]
        public async Task<ActionResult<TributeInfoResponse>> SendTribute([FromBody] CreateProjectRequest createProject)
        {
            AuthorizedUser user = this.TryAuthWithToken();
            TributeInfoResponse tributes = await _guildService.CreateTribute(user, createProject);
            return Ok(tributes);
        }

        [HttpPut("cancel")]
        public async Task<ActionResult<TributeInfoResponse>> CancelTribute([FromBody] long tributeId)
        {
            AuthorizedUser user = this.TryAuthWithToken();
            TributeInfoResponse tributes = await _guildService.CancelTribute(user, tributeId);
            return Ok(tributes);
        }

        [HttpPut("complete")]
        public async Task<ActionResult<TributeInfoResponse>> CompleteTribute([FromBody] TributeCompleteRequest tributeCompleteRequest)
        {
            AuthorizedUser user = this.TryAuthWithToken();
            TributeInfoResponse tributes = await _guildService.CompleteTribute(user, tributeCompleteRequest);
            return Ok(tributes);
        }
    }
}
