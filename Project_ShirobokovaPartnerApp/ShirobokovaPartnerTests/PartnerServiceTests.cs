using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShirobokovaPartnerLib.Data;
using ShirobokovaPartnerLib.Models;
using ShirobokovaPartnerLib.Services;

namespace ShirobokovaPartnerTests
{
    [TestClass]
    public class PartnerServiceTests
    {
        private AppDbContext _context;
        private PartnerService _service;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _service = new PartnerService(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var partnerType = new PartnerType { Id = 1, Name = "Дистрибьютор" };
            _context.PartnerTypes.Add(partnerType);

            var product = new Product { Id = 1, Name = "Тестовый продукт", Price = 1000 };
            _context.Products.Add(product);

            var partner = new Partner
            {
                Id = 1,
                Name = "Тестовый партнёр",
                PartnerTypeId = 1,
                Rating = 50,
                Address = "г. Тест, ул. Тестовая, 1",
                DirectorName = "Тестов Тестович",
                Phone = "+7 (123) 456-78-90",
                Email = "test@test.com"
            };
            _context.Partners.Add(partner);

            var sale1 = new Sale
            {
                PartnerId = 1,
                ProductId = 1,
                Quantity = 10,
                SaleDate = DateTime.Now.AddDays(-30),
                TotalAmount = 10000
            };
            var sale2 = new Sale
            {
                PartnerId = 1,
                ProductId = 1,
                Quantity = 20,
                SaleDate = DateTime.Now.AddDays(-15),
                TotalAmount = 20000
            };
            _context.Sales.AddRange(sale1, sale2);

            _context.SaveChanges();
        }

        [TestCleanup]
        public void TearDown()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetAllPartnersAsync_ShouldReturnPartnersWithTotalSales()
        {
            var partners = await _service.GetAllPartnersAsync();

            Assert.IsNotNull(partners);
            Assert.AreEqual(1, partners.Count);
            Assert.AreEqual("Тестовый партнёр", partners[0].Name);
            Assert.AreEqual(30000, partners[0].TotalSales);
        }

        [TestMethod]
        public async Task GetPartnerByIdAsync_ExistingId_ShouldReturnPartnerWithTotalSales()
        {
            var partner = await _service.GetPartnerByIdAsync(1);

            Assert.IsNotNull(partner);
            Assert.AreEqual(1, partner.Id);
            Assert.AreEqual("Тестовый партнёр", partner.Name);
            Assert.AreEqual(30000, partner.TotalSales);
        }

        [TestMethod]
        public async Task GetPartnerByIdAsync_NonExistingId_ShouldReturnNull()
        {
            var partner = await _service.GetPartnerByIdAsync(999);
            Assert.IsNull(partner);
        }

        [TestMethod]
        public async Task AddPartnerAsync_ValidPartner_ShouldReturnTrue()
        {
            var newPartner = new Partner
            {
                Name = "Новый партнёр",
                PartnerTypeId = 1,
                Rating = 80,
                Address = "г. Новый, ул. Новая, 1",
                DirectorName = "Новый Директор",
                Phone = "+7 (999) 123-45-67",
                Email = "new@test.com"
            };

            var result = await _service.AddPartnerAsync(newPartner);

            Assert.IsTrue(result);

            var partners = await _service.GetAllPartnersAsync();
            Assert.AreEqual(2, partners.Count);
        }

        [TestMethod]
        public async Task UpdatePartnerAsync_ExistingPartner_ShouldReturnTrue()
        {
            var partner = await _service.GetPartnerByIdAsync(1);
            partner.Name = "Обновлённое имя";
            partner.Rating = 99;

            var result = await _service.UpdatePartnerAsync(partner);

            Assert.IsTrue(result);

            var updatedPartner = await _service.GetPartnerByIdAsync(1);
            Assert.AreEqual("Обновлённое имя", updatedPartner.Name);
            Assert.AreEqual(99, updatedPartner.Rating);
        }

        [TestMethod]
        public async Task UpdatePartnerAsync_NonExistingPartner_ShouldReturnFalse()
        {
            var fakePartner = new Partner { Id = 999, Name = "Не существует" };
            var result = await _service.UpdatePartnerAsync(fakePartner);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task DeletePartnerAsync_ExistingPartner_ShouldReturnTrue()
        {
            var result = await _service.DeletePartnerAsync(1);
            Assert.IsTrue(result);

            var partners = await _service.GetAllPartnersAsync();
            Assert.AreEqual(0, partners.Count);
        }

        [TestMethod]
        public async Task DeletePartnerAsync_NonExistingPartner_ShouldReturnFalse()
        {
            var result = await _service.DeletePartnerAsync(999);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task GetSalesHistoryAsync_ExistingPartner_ShouldReturnSales()
        {
            var sales = await _service.GetSalesHistoryAsync(1);

            Assert.IsNotNull(sales);
            Assert.AreEqual(2, sales.Count);
        }

        [TestMethod]
        public async Task GetSalesHistoryAsync_NonExistingPartner_ShouldReturnEmptyList()
        {
            var sales = await _service.GetSalesHistoryAsync(999);
            Assert.IsNotNull(sales);
            Assert.AreEqual(0, sales.Count);
        }

        [TestMethod]
        public async Task GetAllPartnerTypesAsync_ShouldReturnPartnerTypes()
        {
            var types = await _service.GetAllPartnerTypesAsync();

            Assert.IsNotNull(types);
            Assert.AreEqual(1, types.Count);
            Assert.AreEqual("Дистрибьютор", types[0].Name);
        }
    }
}