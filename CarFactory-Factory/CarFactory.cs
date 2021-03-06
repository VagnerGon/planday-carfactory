using CarFactory_Domain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace CarFactory_Factory
{
    public class CarFactory : ICarFactory
    {
        private IChassisProvider _chassisProvider;
        private IEngineProvider _engineProvider;
        private IPainter _painter;
        private IInteriorProvider _interiorProvider;
        private IWheelProvider _wheelProvider;
        private ICarAssembler _carAssembler;
        private IStorageProvider _storageProvider;

        public CarFactory(
            IChassisProvider chassisProvider, 
            IEngineProvider engineProvider, 
            IPainter painter, 
            IInteriorProvider interiorProvider, 
            IWheelProvider wheelProvider, 
            ICarAssembler carAssembler,
            IStorageProvider storageProvider)
        {
            _chassisProvider = chassisProvider;
            _engineProvider = engineProvider;
            _painter = painter;
            _interiorProvider = interiorProvider;
            _wheelProvider = wheelProvider;
            _carAssembler = carAssembler;
            _storageProvider = storageProvider;
        }

        public async Task<IEnumerable<Car>> BuildCarsAsync(IEnumerable<CarSpecification> specs)
        {
            var cars = new ConcurrentQueue<Car>();
            var carBuidingTasks = specs.Select(spec => Task.Run(() =>
            {
                var chassis = _chassisProvider.GetChassis(spec.Manufacturer, spec.NumberOfDoors);
                var engine = _engineProvider.GetEngine(spec.Manufacturer);
                var interior = _interiorProvider.GetInterior(spec);
                var wheels = _wheelProvider.GetWheels();
                var car = _carAssembler.AssembleCar(chassis, engine, interior, wheels);
                var paintedCar = _painter.PaintCar(car, spec.PaintJob);
                cars.Enqueue(paintedCar);
            }));
            await Task.WhenAll(carBuidingTasks);
            return cars;
        }
    }
}