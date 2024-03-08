﻿namespace Infrastructure.Entities;

public class OptionalInfoEntity
{
    public int Id { get; set; }

    public string? Bio { get; set; }

    public string? SecAddressLine { get; set; }

    public string? ProfilePictureUrl { get; set; }
     
    public UserEntity? User { get; set; }
}
