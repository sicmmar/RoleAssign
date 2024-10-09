using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace RoleAssign.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private static readonly Dictionary<string, int> imageAssignments = new Dictionary<string, int>()
        {
            { "Mono P", 1 },
            { "Mono D", 2 },
            { "Mono I", 2 },
            { "Gato P", 1 },
            { "Gato D", 2 },
            { "Gato I", 2 },
            { "Perro P", 1 },
            { "Perro D", 2 },
            { "Perro I", 2 },
            { "Pato P", 1 },
            { "Pato D", 2 },
            { "Pato I", 2 }
        };

        private static readonly List<string> selectedImages = new List<string>();
        private static readonly Random random = new Random();
        private static readonly HashSet<string> registeredDevices = new HashSet<string>();

        [HttpGet]
        public IActionResult GetImage()
        {
            var deviceIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(deviceIpAddress))
            {
                return BadRequest("Unable to determine device IP.");
            }

            if (registeredDevices.Contains(deviceIpAddress))
            {
                return BadRequest("This device has already requested an image.");
            }

            registeredDevices.Add(deviceIpAddress);

            var imageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");

            var availableImages = imageAssignments
                .Where(x => x.Value > 0 && !selectedImages.Contains(x.Key))
                .Select(x => x.Key)
                .ToList();

            if (availableImages.Count > 0)
            {
                var selectedImage = availableImages[random.Next(availableImages.Count)];

                imageAssignments[selectedImage]--;

                selectedImages.Add(selectedImage);

                var imagePath = Path.Combine(imageFolderPath, $"{selectedImage}.png");

                if (System.IO.File.Exists(imagePath))
                {
                    var imageBytes = System.IO.File.ReadAllBytes(imagePath);
                    return File(imageBytes, "image/png");
                }
                else
                {
                    return NotFound("Image not found.");
                }
            }

            return NotFound("No available images.");
        }
    }
}
