﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShoppingList.Data;

namespace ShoppingList.Migrations
{
    [DbContext(typeof(ShoppingListDbContext))]
    [Migration("20210206105114_AddShopWaypointsEntityId_ToShoppingListEntity")]
    partial class AddShopWaypointsEntityId_ToShoppingListEntity
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("SharedTypes.Entities.ItemDataEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("ItemType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("ShoppingListEntityRefId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ShoppingListEntityRefId");

                    b.ToTable("ItemDataEntities");
                });

            modelBuilder.Entity("SharedTypes.Entities.ShopWaypointsEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("ShopWaypointsReadDtoJson")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ShopWaypointsEntities");
                });

            modelBuilder.Entity("SharedTypes.Entities.ShoppingListEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ShopWaypointsEntityId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ShopWaypointsEntityId");

                    b.ToTable("ShoppingListEntities");
                });

            modelBuilder.Entity("SharedTypes.Entities.ItemDataEntity", b =>
                {
                    b.HasOne("SharedTypes.Entities.ShoppingListEntity", "ShoppingListEntity")
                        .WithMany("ItemDataEntities")
                        .HasForeignKey("ShoppingListEntityRefId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ShoppingListEntity");
                });

            modelBuilder.Entity("SharedTypes.Entities.ShoppingListEntity", b =>
                {
                    b.HasOne("SharedTypes.Entities.ShopWaypointsEntity", "ShopWaypointsEntity")
                        .WithMany("ShoppingListEntities")
                        .HasForeignKey("ShopWaypointsEntityId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("ShopWaypointsEntity");
                });

            modelBuilder.Entity("SharedTypes.Entities.ShopWaypointsEntity", b =>
                {
                    b.Navigation("ShoppingListEntities");
                });

            modelBuilder.Entity("SharedTypes.Entities.ShoppingListEntity", b =>
                {
                    b.Navigation("ItemDataEntities");
                });
#pragma warning restore 612, 618
        }
    }
}
