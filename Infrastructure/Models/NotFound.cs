﻿namespace Infrastructure.Models;

public class NotFound
{
	public string ImageUrl { get; set; } = "C:\\Education\\handins\\asp-handin\\ASPNET_MVC\\WebApp\\wwwroot\\images\\404.svg";

	public string Title { get; set; } = "Oooops!";

	public string Description { get; set; } = "The page you are looking for is not available";
}