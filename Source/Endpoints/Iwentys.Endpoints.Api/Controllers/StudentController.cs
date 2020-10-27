﻿using System.Collections.Generic;
using Iwentys.Core.DomainModel;
using Iwentys.Core.Services;
using Iwentys.Endpoints.Api.Tools;
using Iwentys.Models.Transferable;
using Iwentys.Models.Transferable.Students;
using Microsoft.AspNetCore.Mvc;

namespace Iwentys.Endpoints.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly StudentService _studentService;

        public StudentController(StudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public ActionResult<List<StudentFullProfileDto>> Get()
        {
            return Ok(_studentService.Get());
        }

        [HttpGet("{id}")]
        public ActionResult<StudentFullProfileDto> Get(int id)
        {
            return Ok(_studentService.Get(id));
        }

        [HttpGet("for-group/{groupName}")]
        public ActionResult<List<StudentFullProfileDto>> Get(string groupName)
        {
            return Ok(_studentService.Get(groupName));
        }

        [HttpPut]
        public ActionResult<StudentFullProfileDto> Update([FromBody] StudentUpdateRequest studentUpdateRequest)
        {
            AuthorizedUser user = this.TryAuthWithToken();

            return Ok(_studentService.AddGithubUsername(user.Id, studentUpdateRequest.GithubUsername));
        }
    }
}