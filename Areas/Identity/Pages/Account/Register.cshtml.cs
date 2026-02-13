#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SmartEventManagement_TicketingSystem.Areas.Identity.Data;

namespace SmartEventManagement_TicketingSystem.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<SmartEventManagement_TicketingSystemUser> _signInManager;
        private readonly UserManager<SmartEventManagement_TicketingSystemUser> _userManager;
        private readonly IUserStore<SmartEventManagement_TicketingSystemUser> _userStore;
        private readonly IUserEmailStore<SmartEventManagement_TicketingSystemUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<SmartEventManagement_TicketingSystemUser> userManager,
            IUserStore<SmartEventManagement_TicketingSystemUser> userStore,
            SignInManager<SmartEventManagement_TicketingSystemUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        // ================= INPUT MODEL =================
        public class InputModel
        {
            [Required]
            [Display(Name = "Full Name")]
            public string FullName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(100, MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match")]
            public string ConfirmPassword { get; set; }
        }

        // ================= GET =================
        public async Task OnGetAsync()
        {
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        // ================= POST =================
        public async Task<IActionResult> OnPostAsync()
        {
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!ModelState.IsValid)
                return Page();

            var user = CreateUser();
            user.FullName = Input.FullName;
            user.IsMember = false;

            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return Page();
            }

            _logger.LogInformation("User registered, redirecting to payment.");

            // ✅ Auto login
            await _signInManager.SignInAsync(user, isPersistent: false);

            // ✅ GO TO PAYMENT PAGE
            return RedirectToAction("Pay", "Membership");
        }

        // ================= HELPERS =================
        private SmartEventManagement_TicketingSystemUser CreateUser()
        {
            return Activator.CreateInstance<SmartEventManagement_TicketingSystemUser>();
        }

        private IUserEmailStore<SmartEventManagement_TicketingSystemUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
                throw new NotSupportedException("Email not supported.");

            return (IUserEmailStore<SmartEventManagement_TicketingSystemUser>)_userStore;
        }
    }
}
