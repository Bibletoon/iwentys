﻿using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Iwentys.Common.Transferable;
using Iwentys.Endpoint.Server.Source.Options;
using Microsoft.IdentityModel.Tokens;

namespace Iwentys.Endpoint.Server.Source.Tokens
{
    public static class TokenGenerator
    {
        public static IwentysAuthResponse Generate(int userId, IJwtSigningEncodingKey signingEncodingKey, JwtApplicationOptions jwtApplicationOptions)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.UserData, userId.ToString(CultureInfo.InvariantCulture))
            };

            var token = new JwtSecurityToken(
                issuer: jwtApplicationOptions.JwtIssuer,
                audience: jwtApplicationOptions.JwtIssuer,
                claims: claims,
                signingCredentials: new SigningCredentials(
                    signingEncodingKey.GetKey(),
                    signingEncodingKey.SigningAlgorithm)
            );

            var jwt = new JwtSecurityTokenHandler();
            return new IwentysAuthResponse
            {
                Token = jwt.WriteToken(token)
            };
        }
    }
}