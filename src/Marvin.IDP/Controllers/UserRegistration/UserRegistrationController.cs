using IdentityServer4.Services;
using Marvin.IDP.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Marvin.IDP.Controllers.UserRegistration
{
    public class UserRegistrationController : Controller
    {
        private readonly IMarvinUserRepository _marvinUserRepository;
        private readonly IIdentityServerInteractionService _identityInteractionService;

        public UserRegistrationController(IMarvinUserRepository marvinUserRepository, 
                IIdentityServerInteractionService identityInteractionService)
        {
            this._marvinUserRepository = marvinUserRepository;
            this._identityInteractionService = identityInteractionService;
        }

        [HttpGet]
        public IActionResult RegisterUser(string returnUrl)
        {
            var vm = new RegisterUserViewModel();
            vm.ReturnUrl = returnUrl;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // create user + claims
                var userToCreate = new Entities.User
                {
                    Password = model.Password,
                    Username = model.Username,
                    IsActive = true
                };
                userToCreate.Claims.Add(new Entities.UserClaim("country", model.Country));
                userToCreate.Claims.Add(new Entities.UserClaim("address", model.Address));
                userToCreate.Claims.Add(new Entities.UserClaim("given_name", model.Firstname));
                userToCreate.Claims.Add(new Entities.UserClaim("family_name", model.Lastname));
                userToCreate.Claims.Add(new Entities.UserClaim("email", model.Email));
                userToCreate.Claims.Add(new Entities.UserClaim("subscriptionlevel", "PaidUser"));
                userToCreate.Claims.Add(new Entities.UserClaim(ClaimTypes.Role, "PaidUser"));

                // if we're provisioning a user via external login, we must add the provider &
                // user id at the provider to this user's logins
                if (model.IsProvisioningFromExternal)
                {
                    userToCreate.Logins.Add(new Entities.UserLogin()
                    {
                        LoginProvider = model.Provider,
                        ProviderKey = model.ProviderUserId
                    });
                }

                // add it through the repository
                _marvinUserRepository.AddUser(userToCreate);

                if (!_marvinUserRepository.Save())
                {
                    throw new Exception($"Creating a user failed.");
                }

                if (!model.IsProvisioningFromExternal)
                {
                    AuthenticationProperties props = null;
                    await HttpContext.SignInAsync(userToCreate.SubjectId, userToCreate.Username, props);
                    //var userIdentity = new ClaimsIdentity();
                    //userIdentity.AddClaim(new Claim("sid", userToCreate.SubjectId));
                    //userIdentity.AddClaim(new Claim("sub", userToCreate.Username));
                    //var userPrincipal = new ClaimsPrincipal(userIdentity);
                    //// log the user in
                    //await HttpContext.SignInAsync(userPrincipal);
                }

                // continue with the flow     
                if (_identityInteractionService.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return Redirect("~/");
            }

            // ModelState invalid, return the view with the passed-in model
            // so changes can be made
            return View(model);
        }

    }
}