﻿using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Iwentys.Common.Transferable;
using Iwentys.Database.Context;
using Iwentys.Endpoint.Server.Source.Auth;
using Iwentys.Endpoint.Server.Source.Tools;
using Iwentys.Features.StudentFeature;
using Iwentys.Features.StudentFeature.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Iwentys.Endpoint.Server.Source.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IsuAuthController : ControllerBase
    {
        private readonly ILogger<IsuAuthController> _logger;
        //private readonly IsuApiAccessor _isuApiAccessor;
        private readonly DatabaseAccessor _databaseAccessor;
        private readonly StudentService _studentService;
        private IAuthenticationService _authenticationService;

        public IsuAuthController(ILogger<IsuAuthController> logger, DatabaseAccessor databaseAccessor, StudentService studentService, IAuthenticationService authenticationService)
        {
            _logger = logger;
            _databaseAccessor = databaseAccessor;
            _studentService = studentService;
            _authenticationService = authenticationService;
            //_isuApiAccessor = new IsuApiAccessor(ApplicationOptions.IsuClientId, ApplicationOptions.IsuClientSecret, ApplicationOptions.IsuRedirection);
        }

        //[HttpGet]
        //public async Task<ActionResult<IsuAuthResponse>> Get(string code, [FromServices] IJwtSigningEncodingKey signingEncodingKey)
        //{
        //    _logger.LogInformation($"Get code for isu auth: {code}");

        //    AuthorizeResponse authResponse = await _isuApiAccessor.Authorize(code);
        //    if (!authResponse.IsSuccess)
        //        return BadRequest(authResponse.ErrorResponse);

        //    IsuUserDataResponse userData = await _isuApiAccessor.GetUserData(authResponse.TokenResponse.AccessToken);

        //    IwentysAuthResponse token = TokenGenerator.Generate(userData.Id, signingEncodingKey);
        //    var response = new IsuAuthResponse
        //    {
        //        Token = token.Token,
        //        User = JsonConvert.SerializeObject(userData)
        //    };

        //    return Ok(response);
        //}
        //TODO: fix

        [HttpGet("login/{userId}")]
        public ActionResult<IwentysAuthResponse> Login(int userId, [FromServices] IJwtSigningEncodingKey signingEncodingKey)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.UserData, userId.ToString(CultureInfo.InvariantCulture))
            };

            return Ok(TokenGenerator.Generate(userId, signingEncodingKey));
        }

        [HttpGet("loginOrCreate/{userId}")]
        public async Task<ActionResult<IwentysAuthResponse>> LoginOrCreate(int userId, [FromServices] IJwtSigningEncodingKey signingEncodingKey)
        {
            await _studentService.GetOrCreateAsync(userId);
            return Ok(TokenGenerator.Generate(userId, signingEncodingKey));
        }

        [HttpGet("ValidateToken")]
        public int ValidateToken()
        {
            AuthorizedUser tryAuthWithToken = this.TryAuthWithTokenOrDefault(-1);
            if (tryAuthWithToken.Id == -1)
                throw new Exception("Invalid token");
            return tryAuthWithToken.Id;
        }
    }
}