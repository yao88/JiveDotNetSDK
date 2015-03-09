using AutoMapper;
using Net.Pokeshot.JiveSdk.Example.Util;
using Net.Pokeshot.JiveSdk.Models;
using Net.Pokeshot.JiveSdk.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace Net.Pokeshot.JiveSdk.Example.Controllers.Api
{
    public class ExampleController : ApiController
    {
        private JiveSdkContext db = new JiveSdkContext();
        // GET: api/Example
        [SignedRequest]
        public HttpResponseMessage Get()
        {
            string ownerId = Thread.CurrentPrincipal.Identity.Name;
                     User myUser = db.Users
                    .Include("JiveInstance")
                    .Where(u => u.UserId == ownerId).First();


            List<User> allUsers =  db.Users
                .Include("JiveInstance")
                .Where(u=>u.JiveInstance.JiveInstanceId.Equals(myUser.JiveInstance.JiveInstanceId)).ToList();
            Mapper.CreateMap<User, UserDto>();
            List<UserDto> userDto = Mapper.Map<List<User>, List<UserDto>>(allUsers);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, userDto);
            return response;
        }

        // GET: api/Example/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Example
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Example/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Example/5
        public void Delete(int id)
        {
        }
    }
}
