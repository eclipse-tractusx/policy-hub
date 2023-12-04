/********************************************************************************
 * Copyright (c) 2021, 2023 Contributors to the Eclipse Foundation
 *
 * See the NOTICE file(s) distributed with this work for additional
 * information regarding copyright ownership.
 *
 * This program and the accompanying materials are made available under the
 * terms of the Apache License, Version 2.0 which is available at
 * https://www.apache.org/licenses/LICENSE-2.0.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 * SPDX-License-Identifier: Apache-2.0
 ********************************************************************************/

using Microsoft.EntityFrameworkCore;
using Org.Eclipse.TractusX.PolicyHub.Entities.Entities;
using Org.Eclipse.TractusX.PolicyHub.Entities.Enums;

namespace Org.Eclipse.TractusX.PolicyHub.Entities;

public class PolicyHubContext : DbContext
{
    public PolicyHubContext()
    {
    }

    public PolicyHubContext(DbContextOptions<PolicyHubContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AttributeKey> AttributeKeys { get; set; } = default!;
    public virtual DbSet<Policy> Policies { get; set; } = default!;
    public virtual DbSet<PolicyAttribute> PolicyAttributes { get; set; } = default!;
    public virtual DbSet<PolicyType> PolicyTypes { get; set; } = default!;
    public virtual DbSet<PolicyAssignedTypes> PolicyAssignedTypes { get; set; } = default!;
    public virtual DbSet<PolicyKind> PolicyKinds { get; set; } = default!;
    public virtual DbSet<PolicyKindConfiguration> PolicyKindConfigurations { get; set; } = default!;
    public virtual DbSet<PolicyAssignedUseCases> PolicyAssignedUseCases { get; set; } = default!;
    public virtual DbSet<UseCase> UseCases { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("Relational:Collation", "en_US.utf8");
        modelBuilder.HasDefaultSchema("hub");

        modelBuilder.Entity<AttributeKey>().HasData(
            Enum.GetValues(typeof(AttributeKeyId))
                .Cast<AttributeKeyId>()
                .Select(e => new AttributeKey(e))
        );

        modelBuilder.Entity<Policy>(entity =>
        {
            entity.Property(x => x.IsActive).HasDefaultValue(true);

            entity.HasMany(p => p.Types)
                .WithMany(pt => pt.Policies)
                .UsingEntity<PolicyAssignedTypes>(p => p
                        .HasOne(x => x.PolicyType)
                        .WithMany()
                        .HasForeignKey(x => x.PolicyTypeId)
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    p => p
                    .HasOne(x => x.Policy)
                    .WithMany()
                    .HasForeignKey(x => x.PolicyId)
                    .OnDelete(DeleteBehavior.ClientSetNull),
                    x =>
                    {
                        x.HasKey(e => new { e.PolicyId, e.PolicyTypeId });
                    });

            entity.HasMany(p => p.UseCases)
                .WithMany(pt => pt.Policies)
                .UsingEntity<PolicyAssignedUseCases>(p => p
                        .HasOne(x => x.UseCase)
                        .WithMany()
                        .HasForeignKey(x => x.UseCaseId)
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    p => p
                        .HasOne(x => x.Policy)
                        .WithMany()
                        .HasForeignKey(x => x.PolicyId)
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    x =>
                    {
                        x.HasKey(e => new { e.PolicyId, e.UseCaseId });
                    });

            entity.HasOne(p => p.AttributeKey)
                .WithMany(pt => pt.Policies)
                .HasForeignKey(p => p.AttributeKeyId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(p => p.PolicyKind)
                .WithMany(pt => pt.Policies)
                .HasForeignKey(x => x.KindId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PolicyAttribute>(entity =>
        {
            entity.Property(x => x.IsActive).HasDefaultValue(true);

            entity
                .HasKey(x => new { x.PolicyId, x.Key, x.AttributeValue });

            entity
                .HasOne(pa => pa.AttributeKey)
                .WithMany(p => p.PolicyAttributes)
                .HasForeignKey(x => x.Key);

            entity
                .HasOne(pa => pa.Policy)
                .WithMany(p => p.Attributes)
                .HasForeignKey(x => x.PolicyId);
        });

        modelBuilder.Entity<PolicyType>(entity =>
        {
            entity.Property(x => x.IsActive).HasDefaultValue(true);

            entity
                .HasData(
                    Enum.GetValues(typeof(PolicyTypeId))
                        .Cast<PolicyTypeId>()
                        .Select(e => new PolicyType(e, true))
                );
        });

        modelBuilder.Entity<UseCase>(entity =>
        {
            entity.Property(x => x.IsActive).HasDefaultValue(true);

            entity
                .HasData(
                    Enum.GetValues(typeof(UseCaseId))
                        .Cast<UseCaseId>()
                        .Select(e => new UseCase(e, true))
                );
        });

        modelBuilder.Entity<PolicyKind>(entity =>
        {
            entity.HasOne(x => x.Configuration)
                .WithOne(x => x.PolicyKind)
                .HasForeignKey<PolicyKindConfiguration>(x => x.PolicyKindId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasData(
                Enum.GetValues(typeof(PolicyKindId))
                    .Cast<PolicyKindId>()
                    .Select(e => new PolicyKind(e))
            );
        });

        modelBuilder.Entity<PolicyKindConfiguration>(entity =>
        {
            entity.HasKey(x => x.PolicyKindId);
        });
    }
}
