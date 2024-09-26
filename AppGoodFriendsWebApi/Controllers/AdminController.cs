using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models;
using Models.DTO;
using Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AppWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AdminController : Controller
    {
        loginUserSessionDto _usr = null;

        IFriendsService _friendService = null;
        ILoginService _loginService = null;
        ILogger<AdminController> _logger;

        //GET: api/admin/seed?count={count}
        [HttpGet()]
        [ActionName("Seed")]
        [ProducesResponseType(200, Type = typeof(adminInfoDbDto))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> Seed(string count)
        {
            try
            {
                int _count = int.Parse(count);

                adminInfoDbDto _info = await _friendService.SeedAsync(_usr, _count);
                return Ok(_info);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //GET: api/admin/removeseed
        [HttpGet()]
        [ActionName("RemoveSeed")]
        [ProducesResponseType(200, Type = typeof(adminInfoDbDto))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> RemoveSeed(string seeded = "true")
        {
            try
            {
                bool _seeded = bool.Parse(seeded);

                adminInfoDbDto _info = await _friendService.RemoveSeedAsync(_usr, _seeded);
                return Ok(_info);        
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //GET: api/admin/seeduser?users={count}&superusers={count}
        [HttpGet()]
        [ActionName("SeedUsers")]
        [ProducesResponseType(200, Type = typeof(usrInfoDto))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> SeedUsers(string countUsr = "32", string countSupUsr = "2")
        {
            try
            {
                int _countUsr = int.Parse(countUsr);
                int _countSupUsr = int.Parse(countSupUsr);

                return BadRequest("Not implemented");           
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }       
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }

        #region constructors
        public AdminController(IFriendsService friendService, ILogger<AdminController> logger)
        {
            _friendService = friendService;
            _logger = logger;
        }

        /*
        public AdminController(IFriendsService friendService, ILogger<AdminController> logger)
        {
            _friendService = friendService;
            _logger = logger;
        }
        */
        /*
        public AdminController(IFriendsService friendService, ILoginService loginService, ILogger<AdminController> logger)
        {
            _friendService = friendService;
            _loginService = loginService;

            _logger = logger;
        }
        */
        #endregion
    }
}

