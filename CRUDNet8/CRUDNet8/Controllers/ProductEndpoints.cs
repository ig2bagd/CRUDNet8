﻿using SharedLibrary.Models;
using SharedLibrary.ProductRepositories;

namespace CRUDNet8.Controllers;

// https://www.youtube.com/watch?v=gsAuFIhXz3g
// https://www.youtube.com/watch?v=EXqM-9nMIrk
public static class ProductEndpoints
{
    //public static IEndpointConventionBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        /*
        app.MapGet("/api/product/All-Products", async (IProductRepository productRepository) =>
        {
            var products = await productRepository.GetAllProductsAsync();
            //return TypedResults.Ok(products);
            return Results.Ok(products);
        });

        app.MapGet("/api/product/Single-Product/{id}", async (int id, IProductRepository productRepository) =>
        {
            var product = await productRepository.GetProductByIdAsync(id);
            return TypedResults.Ok(product);
        });

        app.MapPost("/api/product/Add-Product", async (Product model, IProductRepository productRepository) =>
        {
            var product = await productRepository.AddProductAsync(model);
            return TypedResults.Ok(product);
        });

        app.MapPut("/api/product/Update-Product", async (Product model, IProductRepository productRepository) =>
        {
            var product = await productRepository.UpdateProductAsync(model);
            return TypedResults.Ok(product);
        });

        app.MapDelete("/api/product/Delete-Product/{id}", async (int id, IProductRepository productRepository) =>
        {
            var product = await productRepository.DeleteProductAsync(id);
            return TypedResults.Ok(product);
        });
        */

        var group = app.MapGroup("api/product");
        group.MapGet("All-Products", AllProducts);
        group.MapGet("Single-Product/{id}", SingleProduct);
        group.MapPost("Add-Product", AddProduct);
        group.MapPut("Update-Product", UpdateProduct);
        group.MapDelete("Delete-Product/{id}", DeleteProduct);
    }

    public static async Task<IResult> AllProducts(IProductRepository productRepository)
    {
        var products = await productRepository.GetAllProductsAsync();
        //return Results.Ok(products);
        return TypedResults.Ok(products);
    }

    public static async Task<IResult> SingleProduct(int id, IProductRepository productRepository)
    {
        var product = await productRepository.GetProductByIdAsync(id);
        return TypedResults.Ok(product);
    }
    public static async Task<IResult> AddProduct(Product model, IProductRepository productRepository)
    {
        var product = await productRepository.AddProductAsync(model);
        return TypedResults.Ok(product);
    }

    public static async Task<IResult> UpdateProduct(Product model, IProductRepository productRepository)
    {
        var product = await productRepository.UpdateProductAsync(model);
        return TypedResults.Ok(product);
    }

    public static async Task<IResult> DeleteProduct(int id, IProductRepository productRepository)
    {
        var product = await productRepository.DeleteProductAsync(id);
        return TypedResults.Ok(product);
    }

}