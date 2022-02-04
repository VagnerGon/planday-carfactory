using CarFactory_Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarFactory.Controllers.ViewModel
{
    public class BuildCarInputModel
    {
        public IEnumerable<BuildCarInputModelItem> Cars { get; set; }
    }

    public class BuildCarInputModelItem
    {
        [Required]
        public int Amount { get; set; }
        [Required]
        public CarSpecificationInputModel Specification { get; set; }
    }

    public class CarPaintSpecificationInputModel
    {
        public PaintType Type { get; set; }
        public string BaseColor { get; set; }
        public string? StripeColor { get; set; }
        public string? DotColor { get; set; }
    }

    public class CarSpecificationInputModel
    {
        public int NumberOfDoors { get; set; }
        public CarPaintSpecificationInputModel Paint { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public SpeakerSpecificationInputModel[] FrontWindowSpeakers { get; set; }
    }

    public class SpeakerSpecificationInputModel
    {
        public bool IsSubwoofer { get; set; }
    }

    public class BuildCarOutputModel
    {
        public long RunTime { get; set; }
        public IEnumerable<Car> Cars { get; set; }
    }

    public enum PaintType
    {
        Single,
        Striped,
        Dotted
    }
}
