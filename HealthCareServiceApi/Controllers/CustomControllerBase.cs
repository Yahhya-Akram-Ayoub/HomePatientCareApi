using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelsRepository;
using ModelsRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HealthCareServiceApi.Controllers
{
    public class CustomControllerBase : ControllerBase
    {
        protected readonly IServiceUnit ServiceUnit;
        protected readonly User CurrentUser;
        public CustomControllerBase(IServiceUnit serviceunit)
        {
            ServiceUnit = serviceunit;
            CurrentUser = GetCurrentUser();
        }


        private User GetCurrentUser()
        {
            try
            {
                if (HttpContext?.User.Identity != null)
                {
                    var identity = HttpContext.User.Identity as ClaimsIdentity;

                    if (identity != null)
                    {
                        var userClaims = identity.Claims;
                        string UserId = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value;
                        return ServiceUnit.Users.Get(x => x.Id.ToString().Equals(UserId));
                    }
                }

            }
            catch (Exception e)
            {
                return null;
            }
            return null;
        }
    }
}
