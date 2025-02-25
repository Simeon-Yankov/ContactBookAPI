using ContactBookAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactBookAPI.Infrastructure.Data.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(e => e.Id);

        builder
            .Property(e => e.Created)
            .IsRequired();

        builder
            .Property(e => e.CreatedBy)
            .HasMaxLength(100);

        builder.Property(e => e.LastModified);

        builder
            .Property(e => e.LastModifiedBy)
            .HasMaxLength(100);

        builder
            .Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.Deleted);

        builder
            .Property(e => e.DeletedBy)
            .HasMaxLength(100);

        // Soft delete query filter
        builder.HasQueryFilter(e => !e.IsDeleted);

        builder
            .OwnsMany(
                p => p.Addresses,
                addressBuilder =>
                {
                    addressBuilder.Property(a => a.AddressLine)
                        .IsRequired()
                        .HasMaxLength(256);

                    addressBuilder.Property(a => a.AddressType)
                        .IsRequired();

                    addressBuilder
                        .OwnsMany(
                            a => a.PhoneNumbers,
                            phoneBuilder =>
                            {
                                phoneBuilder.Property(pn => pn.Number)
                                    .IsRequired()
                                    .HasMaxLength(15);
                            });
                });
    }
}
