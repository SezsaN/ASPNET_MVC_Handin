﻿using Infrastructure.Entities;
using Infrastructure.Models;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels;


namespace WebApp.Controllers;

public class AccountController : Controller
{
	private readonly UserManager<UserEntity> _userManager;
	private readonly SignInManager<UserEntity> _signInManager;
	private readonly AddressRepository _addressRepository;
	private readonly OptionalInfoRepository _optionalInfoRepository;



	public AccountController(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager, AddressRepository addressRepository, OptionalInfoRepository optionalInfoRepository)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_addressRepository = addressRepository;
		_optionalInfoRepository = optionalInfoRepository;
	}


	[HttpGet]
	public async Task<IActionResult> Index()
	{
		var viewModel = new AccountDetailsViewModel
		{
			AccountBasic = await PopulateBasic(),
			AccountAddress = await PopulateAddress(),
			AccountOptional = await PopulateOptional()
		};

		return View(viewModel);
	}

	[HttpPost]
	public async Task<IActionResult> Basic(AccountDetailsViewModel viewmodel)
	{
		
		if (viewmodel.AccountBasic != null)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user != null)
			{
				user.FirstName = viewmodel.AccountBasic.FirstName;
				user.LastName = viewmodel.AccountBasic.LastName;
				user.Email = viewmodel.AccountBasic.Email;
				user.UserName = viewmodel.AccountBasic.Email;
				user.PhoneNumber = viewmodel.AccountBasic.Phone;
							

				if (String.IsNullOrEmpty(viewmodel.AccountBasic.Bio))
				{
					var result = await _userManager.UpdateAsync(user);
				}
				else
				{
					var optional = await _optionalInfoRepository.GetOneAsync(x => x.Id == user.OptionalInfoID);
					if (optional != null)
					{
						optional.Bio = viewmodel.AccountBasic.Bio;
						var result = await _optionalInfoRepository.UpdateAsync(x => x.Id == optional.Id, optional);
						return RedirectToAction("Index", "Account");
					}
					else
					{
						var newOptional = await _optionalInfoRepository.CreateAsync(new OptionalInfoEntity
						{
							Bio = viewmodel.AccountBasic.Bio
						});
						if (newOptional != null)
						{
							user.OptionalInfoID = newOptional.Id;
							var result = await _userManager.UpdateAsync(user);
							return RedirectToAction("Index", "Account");
						}
					}
				}
			};
		}
		return RedirectToAction("Index", "Account");
	}

	[HttpPost]
	public async Task<IActionResult> Address(AccountDetailsViewModel viewmodel)
	{
		if (viewmodel.AccountAddress != null)
		{
			var userEntity = await _userManager.GetUserAsync(User);

			if (String.IsNullOrEmpty(viewmodel.AccountAddress.AddressLine1) && String.IsNullOrEmpty(viewmodel.AccountAddress.PostalCode) && String.IsNullOrEmpty(viewmodel.AccountAddress.City))
			{
				userEntity!.AddressID = null;
				var result = await _userManager.UpdateAsync(userEntity);

				if (result.Succeeded)
				{
					return RedirectToAction("Index", "Account");
				}
			}
			else
			{
				var address = await _addressRepository.GetOneAsync(x => x.AddressLine == viewmodel.AccountAddress.AddressLine1 && x.PostalCode == viewmodel.AccountAddress.PostalCode && x.City == viewmodel.AccountAddress.City);

				if (address == null)
				{
					var newAddress = await _addressRepository.CreateAsync(new AddressEntity
					{
						AddressLine = viewmodel.AccountAddress.AddressLine1!,
						PostalCode = viewmodel.AccountAddress.PostalCode!,
						City = viewmodel.AccountAddress.City!,
					});

					if (newAddress != null)
					{
						var user = await _userManager.GetUserAsync(User);
						if (user != null)
						{
							user.AddressID = newAddress.Id;
							var result = await _userManager.UpdateAsync(user);

							if (result.Succeeded)
							{
								return RedirectToAction("Index", "Account");
							}
						}
					}
				}
				else
				{
					var user = await _userManager.GetUserAsync(User);
					if (user != null)
					{
						user.AddressID = address.Id;
						var result = await _userManager.UpdateAsync(user);
						if (result.Succeeded)
						{
							return RedirectToAction("Index", "Account");
						}
					}
				}
			}
		}
		return RedirectToAction("Index", "Account");
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

	[HttpGet]
	public IActionResult SavedItems()
	{
		var viewmodel = new AccountSavedItemsViewModel();
		return View(viewmodel);
	}

	[HttpPost]

	public IActionResult SavedItems(AccountSavedItemsViewModel viewmodel)
	{
		if (ModelState.IsValid)
		{
			return RedirectToAction("SavedItems", "Account");
		}
		return View(viewmodel);
	}



	[HttpGet]

	public async Task<IActionResult> LogOut()
	{
		await _signInManager.SignOutAsync();

		return RedirectToAction("Index", "Home");
	}


	private async Task<AccountBasic> PopulateBasic()
	{
		var user = await _userManager.GetUserAsync(User);
		if (user != null)
		{
			return new AccountBasic
			{
				UserId = user.Id,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email!,
				Phone = user.PhoneNumber,
				Bio = user.OptionalInfo?.Bio
			};
		}
		return null!;
	}

	private async Task<AccountAddress> PopulateAddress()
	{
		var user = await _userManager.GetUserAsync(User);
		if (user != null)
		{

			var address = await _addressRepository.GetOneAsync(x => x.Id == user.AddressID);
			if (address != null)
			{
				return new AccountAddress
				{
					AddressLine1 = address.AddressLine,
					PostalCode = address.PostalCode,
					City = address.City,
				};
			}
			else
			{
				return new AccountAddress
				{
					AddressLine1 = "",
					PostalCode = "",
					City = "",
				};
			}
		}
		return null!;
	}

	private async Task<AccountOptional> PopulateOptional()
	{
		var user = await _userManager.GetUserAsync(User);
		if (user != null)
		{
			var optional = await _optionalInfoRepository.GetOneAsync(x => x.Id == user.OptionalInfoID);
			if (optional != null)
			{
				return new AccountOptional
				{
					Bio = optional.Bio,
					SecAddressLine = optional.SecAddressLine,
					ProfilePictureUrl = optional.ProfilePictureUrl
				};
			}
			else
			{
				return new AccountOptional
				{
					Bio = "",
					SecAddressLine = "",
					ProfilePictureUrl = ""
				};
			}
		}
		return null!;
	}
}	

