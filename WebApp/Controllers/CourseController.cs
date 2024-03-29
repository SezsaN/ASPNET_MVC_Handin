﻿using Infrastructure.Entities;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WebApp.ViewModels;
using static System.Net.WebRequestMethods;

namespace WebApp.Controllers;

public class CourseController(HttpClient httpClient, UserManager<UserEntity> userManager) : Controller
{
	private readonly HttpClient _httpClient = httpClient;
    private readonly UserManager<UserEntity> _userManager = userManager;


	[HttpGet]
	public async Task<IActionResult> Index(string searchString, int? category )
	{
        try
        {
            var viewModel = new CourseViewModel();
            viewModel.Courses = await PopulateCourses(); 

            if (!String.IsNullOrEmpty(searchString))
            {
                viewModel.Courses = viewModel.Courses.Where(s => s.Title.ToLower().Contains(searchString.ToLower()));
            }

            if (category.HasValue)
            {
                switch (category)
                {
                    case 1: 
                        viewModel.Courses = viewModel.Courses.Where(c => c.IsBestSeller == true);
                        break;
                    case 2: 
                        viewModel.Courses = viewModel.Courses.Where(c => c.DiscountPrice != null);
                        break;
                    default:
                        break;
                }
            }

            return View(viewModel);
        }
        catch
        {
            return BadRequest();
        }
    }

	[HttpGet]
	public async Task<IEnumerable<CourseModel>> PopulateCourses()
	{

		string apiUrl = "https://localhost:7160/api/course";

		var response = await _httpClient.GetAsync(apiUrl);

		var json = await response.Content.ReadAsStringAsync();

		var data = JsonConvert.DeserializeObject<IEnumerable<CourseModel>>(json);
		if (data != null)
		{
			return data;
		}
		 
		else
		{
			return Enumerable.Empty<CourseModel>();
		}
	}

    [HttpPost]

    public async Task<IActionResult> SaveCourse(int CourseId)
    {
        string apiUrl = "https://localhost:7160/api/SavedCourse/";

        var user = await _userManager.GetUserAsync(User);

        if (user != null)
        {
            var saveCourse = new SaveCourseModel
            {
                UserEmail = user.Email!,
                CourseId = CourseId,
            };

            var json = JsonConvert.SerializeObject(saveCourse);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var respsonse = await _httpClient.PostAsync(apiUrl, content);

            if (respsonse.IsSuccessStatusCode)
            {
                TempData["Saved"] = "Course saved";
                return Ok(respsonse);
            }

            else
            {
                TempData["Failed"] = "Something went wrong";
                return NoContent();
            }


        }
        return BadRequest();
    }

}
 