using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Exceptions;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Mappers;
using RoadReadyAPI.Models;
using RoadReadyAPI.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoadReadyTests
{
    [TestFixture]
    public class VehicleServiceTests
    {
        private Mock<IVehicleRepository> _vehicleRepositoryMock;
        private Mock<IBrandRepository> _brandRepositoryMock;
        private IMapper _mapper;
        private Mock<ILogger<VehicleService>> _loggerMock;
        private IVehicleService _vehicleService;

        [SetUp]
        public void Setup()
        {
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _brandRepositoryMock = new Mock<IBrandRepository>();
            _mapper = new Mapper(new MapperConfiguration(cfg => { cfg.AddProfile<VehicleMappingProfile>(); }));
            _loggerMock = new Mock<ILogger<VehicleService>>();
            _vehicleService = new VehicleService(_vehicleRepositoryMock.Object, _brandRepositoryMock.Object, _mapper, _loggerMock.Object);
        }

        #region SearchVehiclesAsync Tests
        [Test]
        public async Task SearchVehiclesAsync_WhenCalled_ReturnsPaginatedResult()
        {
            // Arrange
            var criteria = new VehicleSearchCriteriaDTO { PageNumber = 1, PageSize = 10 };
            var vehicles = new List<Vehicle> { new Vehicle { Id = 1, Name = "Civic", Brand = new Brand(), Location = new Location() } };

            _vehicleRepositoryMock.Setup(r => r.GetTotalVehicleCountAsync(criteria)).ReturnsAsync(1);
            _vehicleRepositoryMock.Setup(r => r.GetPagedVehiclesAsync(criteria)).ReturnsAsync(vehicles);

            // Act
            var result = await _vehicleService.SearchVehiclesAsync(criteria);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TotalCount);
            Assert.AreEqual(1, result.Items.Count);
        }

        [Test]
        public async Task SearchVehiclesAsync_WhenSearchingByName_ReturnsMatchingVehicles()
        {
            // Arrange
            var criteria = new VehicleSearchCriteriaDTO { Name = "Innova" };
            var matchingVehicles = new List<Vehicle>
    {
        new Vehicle { Id = 1, Name = "Innova Crysta", Brand = new Brand(), Location = new Location() }
    };

            _vehicleRepositoryMock.Setup(r => r.GetTotalVehicleCountAsync(criteria)).ReturnsAsync(1);
            _vehicleRepositoryMock.Setup(r => r.GetPagedVehiclesAsync(criteria)).ReturnsAsync(matchingVehicles);

            // Act
            var result = await _vehicleService.SearchVehiclesAsync(criteria);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Items.Count);
            Assert.AreEqual("Innova Crysta", result.Items.First().Name);
        }
        #endregion

        #region AddVehicleAsync Tests
        [Test]
        public async Task AddVehicleAsync_WhenBrandExists_ReturnsVehicleDTO()
        {
            // Arrange
            var createDto = new CreateVehicleDTO { BrandId = 1 };
            var brand = new Brand { Id = 1 };
            var vehicle = new Vehicle { Id = 10 };
            _brandRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(brand);
            _vehicleRepositoryMock.Setup(r => r.Add(It.IsAny<Vehicle>())).ReturnsAsync(vehicle);
            _vehicleRepositoryMock.Setup(r => r.GetVehicleDetailsByIdAsync(10)).ReturnsAsync(new Vehicle { Brand = new Brand(), Location = new Location() });

            // Act
            var result = await _vehicleService.AddVehicleAsync(createDto);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void AddVehicleAsync_WhenBrandDoesNotExist_ThrowsNoSuchEntityException()
        {
            // Arrange
            var createDto = new CreateVehicleDTO { BrandId = 99 };
            _brandRepositoryMock.Setup(r => r.GetById(99)).ReturnsAsync((Brand)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchEntityException>(() => _vehicleService.AddVehicleAsync(createDto));
        }
        #endregion

        #region GetVehicleByIdAsync Tests
        [Test]
        public async Task GetVehicleByIdAsync_WhenVehicleExists_ReturnsVehicleDTO()
        {
            // Arrange
            var vehicle = new Vehicle { Id = 1, Brand = new Brand(), Location = new Location() };
            _vehicleRepositoryMock.Setup(r => r.GetVehicleDetailsByIdAsync(1)).ReturnsAsync(vehicle);

            // Act
            var result = await _vehicleService.GetVehicleByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetVehicleByIdAsync_WhenVehicleDoesNotExist_ThrowsNoSuchEntityException()
        {
            // Arrange
            _vehicleRepositoryMock.Setup(r => r.GetVehicleDetailsByIdAsync(99)).ReturnsAsync((Vehicle)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchEntityException>(() => _vehicleService.GetVehicleByIdAsync(99));
        }
        #endregion

        #region UpdateVehicleDetailsAsync Tests
        [Test]
        public async Task UpdateVehicleDetailsAsync_WhenVehicleExists_ReturnsUpdatedDTO()
        {
            // Arrange
            var updateDto = new UpdateVehicleDTO { PricePerDay = 100 };
            var vehicle = new Vehicle { Id = 1 };
            _vehicleRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(vehicle);
            _vehicleRepositoryMock.Setup(r => r.Update(It.IsAny<Vehicle>())).ReturnsAsync(vehicle);
            _vehicleRepositoryMock.Setup(r => r.GetVehicleDetailsByIdAsync(1)).ReturnsAsync(new Vehicle { PricePerDay = 100, Brand = new Brand(), Location = new Location() });

            // Act
            var result = await _vehicleService.UpdateVehicleDetailsAsync(1, updateDto);

            // Assert
            Assert.AreEqual(100, result.PricePerDay);
        }

        [Test]
        public void UpdateVehicleDetailsAsync_WhenVehicleDoesNotExist_ThrowsNoSuchEntityException()
        {
            // Arrange
            var updateDto = new UpdateVehicleDTO();
            _vehicleRepositoryMock.Setup(r => r.GetById(99)).ReturnsAsync((Vehicle)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchEntityException>(() => _vehicleService.UpdateVehicleDetailsAsync(99, updateDto));
        }
        #endregion

        #region DeleteVehicleAsync Tests
        [Test]
        public async Task DeleteVehicleAsync_WhenVehicleExists_ReturnsTrue()
        {
            // Arrange
            var vehicle = new Vehicle { Id = 1 };
            _vehicleRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(vehicle);
            _vehicleRepositoryMock.Setup(r => r.Delete(1)).ReturnsAsync(vehicle);

            // Act
            var result = await _vehicleService.DeleteVehicleAsync(1);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteVehicleAsync_WhenVehicleDoesNotExist_ThrowsNoSuchEntityException()
        {
            // Arrange
            _vehicleRepositoryMock.Setup(r => r.GetById(99)).ReturnsAsync((Vehicle)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchEntityException>(() => _vehicleService.DeleteVehicleAsync(99));
        }
        #endregion
    }
}