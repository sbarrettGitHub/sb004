using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SB004.Controllers
{
  using System.Web.Http.Results;

  public class AccountController : ApiController
  {
       [HttpGet]
    public HttpResponseMessage Get(string id)
    {
      return new HttpResponseMessage(HttpStatusCode.OK);
    }
  }
}
