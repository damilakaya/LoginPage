using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using STB.MAP.UI.Models;
using System.Security.Claims;

public class AccountController : Controller
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly SignInManager<ApplicationUser> _signInManager;

	public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
	{
		_userManager = userManager;
		_signInManager = signInManager;
	}

	[HttpGet]
	public IActionResult Register()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> Register(RegisterViewModel model)
	{
		if (ModelState.IsValid)
		{
			var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
			var result = await _userManager.CreateAsync(user, model.Password);

			if (result.Succeeded)
			{
				await _signInManager.SignInAsync(user, isPersistent: false);
				return RedirectToAction(nameof(Login));
			}
			else
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}
		}

		return View(model);
	}

	[HttpGet]
	public IActionResult Login()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
	{
		if (ModelState.IsValid)
		{
			var user = await _userManager.FindByNameAsync(model.Username);

			if (user != null)
			{
				var signInResult = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

				if (signInResult.Succeeded)
				{
					// Başarılı giriş durumu
					var claims = new List<Claim>
				{
					new Claim(ClaimTypes.NameIdentifier, user.Id),
					new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
					new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                    // İsteğe bağlı: Kullanıcının rol bilgilerini ekleyebilirsiniz
                    // new Claim(ClaimTypes.Role, "User"),
                };

					var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
					var principal = new ClaimsPrincipal(identity);

					await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
					{
						IsPersistent = true,
						ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
					});

					return RedirectToAction("Index", "Home");
				}
				else if (signInResult.IsLockedOut)
				{
					// Hesap kilitlendi durumu
					ModelState.AddModelError(string.Empty, "Hesabınız kilitlendi. Lütfen bir süre sonra tekrar deneyin.");
				}
				else if (signInResult.RequiresTwoFactor)
				{
					// İki faktörlü kimlik doğrulama gerekiyor durumu
					ModelState.AddModelError(string.Empty, "İki faktörlü kimlik doğrulama gerekiyor.");
				}
				else
				{
					// Geçersiz kullanıcı adı veya şifre durumu
					ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre.");
				}
			}
			else
			{
				ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı. Lütfen doğru kullanıcı adı ve şifre girildiğinden emin olun.");
			}
		}

		// Hatalı giriş durumu
		return View(model);
	}



}












