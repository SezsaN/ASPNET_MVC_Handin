﻿using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class AccountController : Controller
{
	[HttpGet]
	public IActionResult Index()
	{
		var viewModel = new AccountDetailsViewModel();
		return View(viewModel);
	}

	[HttpPost]
	public IActionResult Index(AccountDetailsViewModel viewmodel)
	{
		if (ModelState.IsValid)
		{
			return RedirectToAction("Index", "Account");
		}
		return View(viewmodel);
	}

	[HttpGet]
	public IActionResult Security()
	{
		var viewModel = new AccountSecurityViewModel();
		return View(viewModel);
	}

	[HttpPost]
	public IActionResult Security(AccountSecurityViewModel viewmodel)
	{
		if (ModelState.IsValid)
		{
			return RedirectToAction("Index", "Account");
		}
		return View(viewmodel);
	}
}