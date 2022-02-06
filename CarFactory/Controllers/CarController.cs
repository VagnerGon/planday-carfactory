using CarFactory.Controllers.ViewModel;
using CarFactory_Domain;
using CarFactory_Factory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace CarFactory.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarController : ControllerBase
    {
        private readonly ICarFactory _carFactory;
        public CarController(ICarFactory carFactory)
        {
            _carFactory = carFactory;
        }

        [ProducesResponseType(typeof(BuildCarOutputModel), StatusCodes.Status200OK)]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody][Required] BuildCarInputModel carsSpecs)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var wantedCars = TransformToDomainObjects(carsSpecs);
            
            var cars = await _carFactory.BuildCarsAsync(wantedCars);
            stopwatch.Stop();

            //Create response and return
            return Ok(new BuildCarOutputModel {
                Cars = cars,
                RunTime = stopwatch.ElapsedMilliseconds
            });
        }

        private static IEnumerable<CarSpecification> TransformToDomainObjects(BuildCarInputModel carsSpecs)
        {
            //Check and transform specifications to domain objects
            var wantedCars = new List<CarSpecification>();
            foreach (var spec in carsSpecs.Cars)
            {
                if (spec.Specification.NumberOfDoors % 2 == 0)
                    throw new ArgumentException("Must give an odd number of doors");

                PaintJob? paint = null;
                var baseColor = Color.FromName(spec.Specification.Paint.BaseColor);
                paint = spec.Specification.Paint.Type switch
                {
                    PaintType.Single => new SingleColorPaintJob(baseColor),
                    PaintType.Striped => new StripedPaintJob(baseColor, Color.FromName(spec.Specification.Paint.StripeColor)),
                    PaintType.Dotted => new DottedPaintJob(baseColor, Color.FromName(spec.Specification.Paint.DotColor)),
                    _ => throw new ArgumentException($"Unknown paint type {spec.Specification.Paint.Type}"),
                };
                var dashboardSpeakers = spec.Specification.FrontWindowSpeakers.Select(s => new CarSpecification.SpeakerSpecification { IsSubwoofer = s.IsSubwoofer });
                var doorSpeakers = new CarSpecification.SpeakerSpecification[0]; //TODO: Let people install door speakers
                var wantedCar = new CarSpecification(paint, spec.Specification.Manufacturer, spec.Specification.NumberOfDoors, doorSpeakers, dashboardSpeakers);

                for (var i = 1; i <= spec.Amount; i++)
                {
                    wantedCars.Add(wantedCar);
                }
            }
            return wantedCars;
        }
    }
}
