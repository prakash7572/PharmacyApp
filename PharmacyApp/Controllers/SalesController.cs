using Microsoft.AspNetCore.Mvc;
using PharmacyApp.Models;
using System.Text.Json;

namespace PharmacyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly string _medicineFilePath;
        private readonly string _saleFilePath;

        public SalesController(IWebHostEnvironment environment)
        {
            _medicineFilePath = Path.Combine(
                environment.ContentRootPath,
                "Data",
                "medicines.json"
            );

            _saleFilePath = Path.Combine(
                environment.ContentRootPath,
                "Data",
                "sales.json"
            );
        }

        [HttpGet]
        public IActionResult GetSales()
        {
            var sales = ReadSales();
            return Ok(sales);
        }

        [HttpPost]
        public IActionResult CreateSale(Sale sale)
        {
            var medicines = ReadMedicines();

            var medicine = medicines
                .FirstOrDefault(m => m.Id == sale.MedicineId);

            if (medicine == null)
                return NotFound("Medicine not found.");

            if (sale.Quantity <= 0)
                return BadRequest("Quantity must be greater than 0.");

            if (sale.Quantity > medicine.Quantity)
                return BadRequest("Not enough stock available.");

            medicine.Quantity -= sale.Quantity;
            var sales = ReadSales();

            sale.Id = sales.Count == 0
                ? 1
                : sales.Max(s => s.Id) + 1;

            sale.MedicineName = medicine.FullName;
            sale.UnitPrice = medicine.Price;
            sale.TotalPrice = medicine.Price * sale.Quantity;
            sale.SaleDate = DateTime.Now;

            sales.Add(sale);

            WriteMedicines(medicines);
            WriteSales(sales);

            return Ok(sale);
        }

        private List<Medicine> ReadMedicines()
        {
            if (!System.IO.File.Exists(_medicineFilePath))
                return new List<Medicine>();

            var json = System.IO.File.ReadAllText(_medicineFilePath);

            return JsonSerializer.Deserialize<List<Medicine>>(json)
                   ?? new List<Medicine>();
        }

        private List<Sale> ReadSales()
        {
            if (!System.IO.File.Exists(_saleFilePath))
                return new List<Sale>();

            var json = System.IO.File.ReadAllText(_saleFilePath);

            return JsonSerializer.Deserialize<List<Sale>>(json)
                   ?? new List<Sale>();
        }

        private void WriteMedicines(List<Medicine> medicines)
        {
            var json = JsonSerializer.Serialize(
                medicines,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            System.IO.File.WriteAllText(_medicineFilePath, json);
        }

        private void WriteSales(List<Sale> sales)
        {
            var json = JsonSerializer.Serialize(
                sales,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            System.IO.File.WriteAllText(_saleFilePath, json);
        }
    }
}
