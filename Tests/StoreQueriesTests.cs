using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using Models;

public class StoreQueriesTests
{
    // Тестовые данные
    private readonly List<Product> _products = new()
    {
        new Product { Id = 1, Barcode = "123456", Name = "Молоко", Type = 0, Price = 80, ExpiryDate = DateTime.Today.AddDays(-1) },
        new Product { Id = 2, Barcode = "789012", Name = "Хлеб", Type = 0, Price = 50, ExpiryDate = DateTime.Today.AddDays(10) },
        new Product { Id = 3, Barcode = "345678", Name = "Сыр", Type = 1, Price = 500, ExpiryDate = DateTime.Today.AddDays(30) }
    };

    private readonly List<Store> _stores = new()
    {
        new Store { Id = 1, Name = "Магазин 1", Address = "ул. Ленина, 1" },
        new Store { Id = 2, Name = "Магазин 2", Address = "ул. Пушкина, 2" }
    };

    private readonly List<Availability> _availabilities = new()
    {
        new Availability { StoreId = 1, ProductId = 1, Quantity = 10 },
        new Availability { StoreId = 1, ProductId = 2, Quantity = 5 },
        new Availability { StoreId = 2, ProductId = 1, Quantity = 8 },
        new Availability { StoreId = 2, ProductId = 3, Quantity = 15 }
    };

    private readonly List<Customer> _customers = new()
    {
        new Customer { Id = 1, FullName = "Иванов Иван", CardNumber = "C001" },
        new Customer { Id = 2, FullName = "Петров Петр", CardNumber = "C002" }
    };

    private readonly List<Sale> _sales = new()
    {
        new Sale { CustomerId = 1, ProductId = 1, SaleDate = DateTime.Today.AddDays(-5), TotalPrice = 80 },
        new Sale { CustomerId = 1, ProductId = 2, SaleDate = DateTime.Today.AddDays(-3), TotalPrice = 50 },
        new Sale { CustomerId = 2, ProductId = 1, SaleDate = DateTime.Today.AddDays(-1), TotalPrice = 160 }
    };

    [Fact]
    public void GetProductsInStore_ReturnsCorrectProducts()
    {
        // Arrange
        int storeId = 1;
        
        // Act
        var result = _availabilities
            .Where(a => a.StoreId == storeId)
            .Join(_products,
                a => a.ProductId,
                p => p.Id,
                (a, p) => p)
            .ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Name == "Молоко");
        Assert.Contains(result, p => p.Name == "Хлеб");
    }

    [Fact]
    public void GetStoresWithProduct_ReturnsCorrectStores()
    {
        // Arrange
        int productId = 1;
        
        // Act
        var result = _availabilities
            .Where(a => a.ProductId == productId && a.Quantity > 0)
            .Join(_stores,
                a => a.StoreId,
                s => s.Id,
                (a, s) => s)
            .ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.Name == "Магазин 1");
        Assert.Contains(result, s => s.Name == "Магазин 2");
    }

    [Fact]
    public void GetAveragePriceByGroup_ReturnsCorrectValues()
    {
        // Act
        var result = _products
            .GroupBy(p => p.Type)
            .Select(g => new { Type = g.Key, AvgPrice = g.Average(p => p.Price) })
            .ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(65, result.First(r => r.Type == 0).AvgPrice);
        Assert.Equal(500, result.First(r => r.Type == 1).AvgPrice);
    }

    [Fact]
    public void GetTop5Sales_ReturnsCorrectSales()
    {
        // Act
        var result = _sales
            .OrderByDescending(s => s.TotalPrice)
            .Take(5)
            .ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(160, result[0].TotalPrice);
        Assert.Equal(80, result[1].TotalPrice);
        Assert.Equal(50, result[2].TotalPrice);
    }

    [Fact]
    public void GetExpiredProducts_ReturnsCorrectItems()
    {
        // Act
        var result = _products
            .Where(p => p.ExpiryDate < DateTime.Today)
            .ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Молоко", result[0].Name);
    }

    [Fact]
    public void GetStoresWithSalesAboveThreshold_ReturnsCorrectStores()
    {
        // Arrange
        decimal threshold = 100;
        DateTime startDate = DateTime.Today.AddDays(-7);
        DateTime endDate = DateTime.Today;

        // Act
        var storeSales = _sales
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
            .Join(_availabilities,
                s => s.ProductId,
                a => a.ProductId,
                (s, a) => new { s.TotalPrice, a.StoreId })
            .GroupBy(x => x.StoreId)
            .Select(g => new 
            {
                StoreId = g.Key,
                TotalSales = g.Sum(x => x.TotalPrice)
            })
            .Where(x => x.TotalSales > threshold)
            .ToList();

        var result = storeSales
            .Join(_stores,
                x => x.StoreId,
                s => s.Id,
                (x, s) => s)
            .ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.Name == "Магазин 1");
        Assert.Contains(result, s => s.Name == "Магазин 2");

        // Проверка сумм
        var store1Total = storeSales.First(x => x.StoreId == 1).TotalSales;
        var store2Total = storeSales.First(x => x.StoreId == 2).TotalSales;
        
        Assert.True(store1Total > threshold);
        Assert.True(store2Total > threshold);
    }
}