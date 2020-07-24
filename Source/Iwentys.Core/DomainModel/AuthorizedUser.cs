﻿using Iwentys.Models.Entities;

namespace Iwentys.Core.DomainModel
{
    //TODO: maybe we should return profile
    public class AuthorizedUser
    {
        public int Id { get; set; }
        public Student Profile { get; set; }

        //TODO: remove
        public static AuthorizedUser DebugAuth() => DebugAuth(0);
        public static AuthorizedUser DebugAuth(int id) => new AuthorizedUser {Id = id};
        public static AuthorizedUser DebugAuth(Student profile) => new AuthorizedUser {Id = profile.Id, Profile = profile};

        private AuthorizedUser()
        {
            
        }
    }
}