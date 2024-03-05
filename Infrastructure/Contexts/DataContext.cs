﻿using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class DataContext(DbContextOptions options) : IdentityDbContext<UserEntity>(options)
{

    public DbSet<AdressEntity> Adresses { get; set; }
    public DbSet<OptionalInfoEntity> OptionalInfos { get; set; }

}
