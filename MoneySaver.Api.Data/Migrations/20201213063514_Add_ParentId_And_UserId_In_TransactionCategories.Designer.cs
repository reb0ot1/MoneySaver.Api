﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MoneySaver.Api.Data;

namespace MoneySaver.Api.Data.Migrations
{
    [DbContext(typeof(MoneySaverApiContext))]
    [Migration("20201213063514_Add_ParentId_And_UserId_In_TransactionCategories")]
    partial class Add_ParentId_And_UserId_In_TransactionCategories
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MoneySaver.Api.Data.Budget", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedOnUtc")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<double>("LimitAmount")
                        .HasColumnType("float");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Budgets");
                });

            modelBuilder.Entity("MoneySaver.Api.Data.BudgetItem", b =>
                {
                    b.Property<int>("BudgetItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BudgetId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedOnUtc")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<double>("LimitAmount")
                        .HasColumnType("float");

                    b.Property<double>("SpentAmount")
                        .HasColumnType("float");

                    b.Property<int>("TransactionCategoryId")
                        .HasColumnType("int");

                    b.HasKey("BudgetItemId");

                    b.HasIndex("BudgetId");

                    b.HasIndex("TransactionCategoryId");

                    b.ToTable("BudgetItems");
                });

            modelBuilder.Entity("MoneySaver.Api.Data.Transaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AdditionalNote")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<DateTime>("CreateOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedOnUtc")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ModifyOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("TransactionCategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TransactionCategoryId");

                    b.ToTable("Transactions");

                    b.HasData(
                        new
                        {
                            Id = new Guid("d6f2349d-ad83-4463-8566-9cdb4805c47f"),
                            AdditionalNote = "Тест бележка",
                            Amount = 3.3999999999999999,
                            CreateOn = new DateTime(2020, 12, 13, 6, 35, 14, 26, DateTimeKind.Utc).AddTicks(1945),
                            IsDeleted = false,
                            ModifyOn = new DateTime(2020, 12, 13, 6, 35, 14, 26, DateTimeKind.Utc).AddTicks(3131),
                            TransactionCategoryId = 1,
                            TransactionDate = new DateTime(2020, 12, 13, 6, 35, 14, 26, DateTimeKind.Utc).AddTicks(4062),
                            UserId = 1
                        });
                });

            modelBuilder.Entity("MoneySaver.Api.Data.TransactionCategory", b =>
                {
                    b.Property<int>("TransactionCategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DeletedOnUtc")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TransactionCategoryId");

                    b.ToTable("TransactionCategories");

                    b.HasData(
                        new
                        {
                            TransactionCategoryId = 1,
                            IsDeleted = false,
                            Name = "Food"
                        },
                        new
                        {
                            TransactionCategoryId = 2,
                            IsDeleted = false,
                            Name = "Sport"
                        });
                });

            modelBuilder.Entity("MoneySaver.Api.Data.BudgetItem", b =>
                {
                    b.HasOne("MoneySaver.Api.Data.Budget", "Budget")
                        .WithMany("BudgetItems")
                        .HasForeignKey("BudgetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MoneySaver.Api.Data.TransactionCategory", "TransactionCategory")
                        .WithMany()
                        .HasForeignKey("TransactionCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MoneySaver.Api.Data.Transaction", b =>
                {
                    b.HasOne("MoneySaver.Api.Data.TransactionCategory", "TransactionCategory")
                        .WithMany()
                        .HasForeignKey("TransactionCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
