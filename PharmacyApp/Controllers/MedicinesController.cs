using Microsoft.AspNetCore.Mvc;
using PharmacyApp.Models;
using System.Text.Json;

namespace PharmacyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicinesController : ControllerBase
    {
        private readonly string _filePath;
        public MedicinesController(IWebHostEnvironment environment)
        {
            _filePath = Path.Combine(
                environment.ContentRootPath,
                "Data",
                "medicines.json"
            );
        }

        [HttpGet]
        public IActionResult GetMedicines(string? search)
        {
            var medicines = ReadMedicines();

            if (!string.IsNullOrWhiteSpace(search))
            {
                medicines = medicines
                    .Where(m => m.FullName.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return Ok(medicines);
        }

        [HttpGet("{id}")]
        public IActionResult GetMedicine(int id)
        {
            var medicine = ReadMedicines()
                .FirstOrDefault(m => m.Id == id);

            if (medicine == null)
                return NotFound();

            return Ok(medicine);
        }

        [HttpPost]
        public IActionResult AddMedicine(Medicine medicine)
        {
            var medicines = ReadMedicines();

            medicine.Id = medicines.Count == 0
                ? 1
                : medicines.Max(m => m.Id) + 1;

            medicines.Add(medicine);

            WriteMedicines(medicines);

            return Ok(medicine);
        }

        private List<Medicine> ReadMedicines()
        {
            if (!System.IO.File.Exists(_filePath))
                return new List<Medicine>();

            var json = System.IO.File.ReadAllText(_filePath);

            return JsonSerializer.Deserialize<List<Medicine>>(json)
                   ?? new List<Medicine>();
        }

        private void WriteMedicines(List<Medicine> medicines)
        {
            var json = JsonSerializer.Serialize(
                medicines,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            System.IO.File.WriteAllText(_filePath, json);
        }
    }
}
