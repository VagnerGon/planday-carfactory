using CarFactory_Assembly;
using CarFactory_Domain;
using CarFactory_Domain.Engine;
using CarFactory_Factory;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class CarAssemblerTests
    {
        private readonly ICarAssembler _carAssembly;
        private readonly Chassis _chassis;
        private readonly Interior _interior;
        private readonly Engine _engine;

        public CarAssemblerTests()
        {
            _carAssembly = new CarAssembler();
            _chassis = new Chassis("", true);
            _interior = new Interior();
            _engine = new Engine(new EngineBlock(1), "");
        }

        [TestMethod]
        public void Factory_ThreeWheelsCarAssemblyTest()
        {
            var wheels = new List<Wheel>();

            for (int i = 0; i < 3; i++)
            {
                wheels.Add(new Wheel());
            }
            var assemble = () => _carAssembly.AssembleCar(_chassis, _engine, _interior, wheels);
            
            assemble.Should().Throw<Exception>();
        }

        [TestMethod]
        public void Factory_FourWheelsCarAssemblyTest()
        {
            var wheels = new List<Wheel>();

            for (int i = 0; i < 4; i++)
            {
                wheels.Add(new Wheel());
            }
            var car = _carAssembly.AssembleCar(_chassis, _engine, _interior, wheels);
            car.Wheels.Count().Should().Be(4);
        }
    }
}
