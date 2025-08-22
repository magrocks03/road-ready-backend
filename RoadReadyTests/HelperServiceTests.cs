using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Mappers;
using RoadReadyAPI.Models;
using RoadReadyAPI.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoadReadyTests
{
    [TestFixture]
    public class HelperServiceTests
    {
        private Mock<IBrandRepository> _brandRepositoryMock;
        private Mock<ILocationRepository> _locationRepositoryMock;
        private Mock<IExtraRepository> _extraRepositoryMock;
        private IMapper _mapper;
        private Mock<ILogger<HelperService>> _loggerMock;
        private IHelperService _helperService;

        [SetUp]
        public void Setup()
        {
            _brandRepositoryMock = new Mock<IBrandRepository>();
            _locationRepositoryMock = new Mock<ILocationRepository>();
            _extraRepositoryMock = new Mock<IExtraRepository>();
            _loggerMock = new Mock<ILogger<HelperService>>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new HelperMappingProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            _helperService = new HelperService(
                _brandRepositoryMock.Object,
                _locationRepositoryMock.Object,
                _extraRepositoryMock.Object,
                _mapper,
                _loggerMock.Object
            );
        }

        [Test]
        public async Task GetAllBrandsAsync_WhenBrandsExist_ReturnsListOfBrandDTOs()
        {
            // Arrange
            var brands = new List<Brand> { new Brand { Name = "Toyota" } }.AsQueryable();
            // This simple mock now works perfectly.
            _brandRepositoryMock.Setup(r => r.GetAll()).Returns(brands);

            // Act
            var result = await _helperService.GetAllBrandsAsync();

            // Assert
            Assert.AreEqual("Toyota", result.First().Name);
        }

        [Test]
        public async Task GetAllLocationsAsync_WhenLocationsExist_ReturnsListOfLocationDTOs()
        {
            // Arrange
            var locations = new List<Location> { new Location { StoreName = "Chennai Airport" } }.AsQueryable();
            _locationRepositoryMock.Setup(r => r.GetAll()).Returns(locations);

            // Act
            var result = await _helperService.GetAllLocationsAsync();

            // Assert
            Assert.AreEqual("Chennai Airport", result.First().StoreName);
        }

        [Test]
        public async Task GetAllExtrasAsync_WhenExtrasExist_ReturnsListOfExtraDTOs()
        {
            // Arrange
            var extras = new List<Extra> { new Extra { Name = "GPS" } }.AsQueryable();
            _extraRepositoryMock.Setup(r => r.GetAll()).Returns(extras);

            // Act
            var result = await _helperService.GetAllExtrasAsync();

            // Assert
            Assert.AreEqual("GPS", result.First().Name);
        }
    }
}