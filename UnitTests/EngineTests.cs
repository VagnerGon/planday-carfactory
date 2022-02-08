using CarFactory_Domain;
using CarFactory_Domain.Engine;
using CarFactory_Engine;
using CarFactory_Factory;
using CarFactory_Storage;
using CarFactory_SubContractor;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class EngineTests
    {
        private readonly IEngineProvider _engineProvider;

        private Manufacturer manufacturer = Manufacturer.Plandrover;

        private const int pistonCount = 4;

        public EngineTests()
        {
            var pistonsMoq = new Mock<IGetPistons>();
            var steelSubContractorMoq = new Mock<ISteelSubcontractor>();
            var engineSpecsMoq = new Mock<IGetEngineSpecificationQuery>();
            
            pistonsMoq.Setup(m => m.Get(It.IsAny<int>())).Returns((int x) => x);
            steelSubContractorMoq.Setup(m => m.OrderSteel(It.IsAny<int>())).Returns((int x) =>
            {
                var amount = new SteelDelivery().Amount;
                var delivery = new List<SteelDelivery>();
                for (int i = 0; i <= x / amount; i++)
                {
                    delivery.Add(new SteelDelivery());
                }
                return delivery;
            });

            engineSpecsMoq.Setup(m => m.GetForManufacturer(manufacturer)).Returns(new CarFactory_Domain.Engine.EngineSpecifications.EngineSpecification
            {
                CylinderCount = pistonCount,
                Name = "Sigma",
                PropulsionType = Propulsion.Gasoline
            });

            _engineProvider = new EngineProvider(pistonsMoq.Object, steelSubContractorMoq.Object, engineSpecsMoq.Object);
        }

        [TestMethod]
        public void Factory_EngineBuildingTest()
        {
            Assert.IsNotNull(_engineProvider);
            var engine = _engineProvider.GetEngine(Manufacturer.Plandrover);

            engine.EngineBlock.CylinderCount.Should().Be(pistonCount);
            engine.PistonsCount.Should().Be(pistonCount);

            engine.HasSparkPlugs.Should().BeTrue();

            engine.PropulsionType.Should().Be(Propulsion.Gasoline);
        }
    }
}
