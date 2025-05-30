﻿using Domain.Entities.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.FluentConfig
{
    public class FluentUserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(b => b.UserName);
            builder.HasQueryFilter(b => b.IsActive);
        }
    }
}
