﻿using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class AuthController : Controller
{
	private readonly UserManager<UserEntity> _userManager;
	private readonly SignInManager<UserEntity> _signInManager;

	public AuthController(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager)
	{
		_userManager = userManager;
		_signInManager = signInManager;
	}

	[Route("/signup")]
	[HttpGet]
	
	public IActionResult SignUp()
	{
		var viewModel = new SignUpViewModel();
		return View(viewModel);
	}

	[HttpPost]
	public async Task<IActionResult> SignUp(SignUpViewModel viewmodel)
	{
		if (ModelState.IsValid)
		{
			var exists = await _userManager.Users.AnyAsync(x => x.Email == viewmodel.Form.Email);
			if (exists)
			{
				ModelState.AddModelError("AlreadyExists", "User with this email already exists");
				ViewData["ErrorMessage"] = "User with this email already exists";
				return View(viewmodel);
			}
			var user = new UserEntity
			{
				FirstName = viewmodel.Form.FirstName,
				LastName = viewmodel.Form.LastName,
				Email = viewmodel.Form.Email,
				UserName = viewmodel.Form.Email,
			};

			var result = await _userManager.CreateAsync(user, viewmodel.Form.Password);
			if (result.Succeeded)
			{
				return RedirectToAction("SignIn", "Auth");
			}
		}
		return View(viewmodel);
	}

	[Route("/signin")]
	[HttpGet]
	public IActionResult SignIn()
	{
		var viewModel = new SignInViewModel();
		return View(viewModel);
	}

	[HttpPost]
	public async Task<IActionResult> SignIn(SignInViewModel viewmodel)
	{
		if (ModelState.IsValid)
		{
			var result = await _signInManager.PasswordSignInAsync(viewmodel.Form.Email, viewmodel.Form.Password, viewmodel.Form.RememberMe, false);
			if (result.Succeeded)
			{
				return RedirectToAction("Index", "Account");
			}
		}

		ModelState.AddModelError("IncorrectValues", "Incorrect email or password");
		ViewData["ErrorMessage"] = "Incorrect email or password";
		return View(viewmodel);


	}

	[HttpGet]
	public IActionResult Facebook()
	{
		var authProps = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", Url.Action("FacebookCallback"));
		return new ChallengeResult("Facebook", authProps);
	}

	[HttpGet]
	public async Task<IActionResult> FacebookCallback()
	{
		var info = await _signInManager.GetExternalLoginInfoAsync();
		if (info != null)
		{
			var userEntity = new UserEntity
			{
				FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName)!,
				LastName = info.Principal.FindFirstValue(ClaimTypes.Surname)!,
				Email = info.Principal.FindFirstValue(ClaimTypes.Email)!,
				UserName = info.Principal.FindFirstValue(ClaimTypes.Email)!,
				IsExternalAccount = true
			};

			var user = await _userManager.FindByEmailAsync(userEntity.Email);
			if (user == null)
			{
				var result = await _userManager.CreateAsync(userEntity);
				if (result.Succeeded)
				{
					user = await _userManager.FindByEmailAsync(userEntity.Email);
				}
			}
			if (user != null)
			{
				if (user.FirstName != userEntity.FirstName || user.LastName != userEntity.LastName || user.Email != userEntity.Email)
				{
					user.FirstName = userEntity.FirstName;
					user.LastName = userEntity.LastName;
					user.Email = userEntity.Email;


					await _userManager.UpdateAsync(user);
				}

				await _signInManager.SignInAsync(user, isPersistent: false);

				if (HttpContext.User != null)
				{
					return RedirectToAction("Index", "Account");
				}

			}

		}
		return RedirectToAction("SignIn", "Auth");
	}

	[HttpGet]
	public IActionResult Google()
	{
        var authProps = _signInManager.ConfigureExternalAuthenticationProperties("Google", Url.Action("GoogleCallback"));
        return new ChallengeResult("Google", authProps);
    }

    [HttpGet]
    public async Task<IActionResult> GoogleCallback()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info != null)
        {
            var userEntity = new UserEntity
            {
                FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName)!,
                LastName = info.Principal.FindFirstValue(ClaimTypes.Surname)!,
                Email = info.Principal.FindFirstValue(ClaimTypes.Email)!,
                UserName = info.Principal.FindFirstValue(ClaimTypes.Email)!,
                IsExternalAccount = true
            };

            var user = await _userManager.FindByEmailAsync(userEntity.Email);
            if (user == null)
            {
                var result = await _userManager.CreateAsync(userEntity);
                if (result.Succeeded)
                {
                    user = await _userManager.FindByEmailAsync(userEntity.Email);
                }
            }
            if (user != null)
            {
                if (user.FirstName != userEntity.FirstName || user.LastName != userEntity.LastName || user.Email != userEntity.Email)
                {
                    user.FirstName = userEntity.FirstName;
                    user.LastName = userEntity.LastName;
                    user.Email = userEntity.Email;


                    await _userManager.UpdateAsync(user);
                }

                await _signInManager.SignInAsync(user, isPersistent: false);

                if (HttpContext.User != null)
                {
                    return RedirectToAction("Index", "Account");
                }

            }

        }
        return RedirectToAction("SignIn", "Auth");
    }
}


