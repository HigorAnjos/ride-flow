using System;
using System.Text.RegularExpressions;

namespace Domain.Entities
{
    public class Motorcycle
    {
        public string Id { get; set; } // Identificador único
        public int Year { get; set; } // Ano de fabricação
        public string Model { get; set; } // Modelo
        public string LicensePlate { get; set; } // Placa

        public bool IsFromYear(int year = 2024)
        {
            return Year == year;
        }

        public void UpdateLicensePlate(string newLicensePlate)
        {
            if (string.IsNullOrWhiteSpace(newLicensePlate))
                throw new ArgumentException("A placa não pode ser vazia.");

            if (!IsValidLicensePlate(newLicensePlate))
                throw new ArgumentException("A placa fornecida é inválida. Use o formato ABC-1234.");

            LicensePlate = newLicensePlate;
        }

        public bool IsValidLicensePlate(string licensePlate = null)
        {
            var plateToValidate = licensePlate ?? LicensePlate;
            return Regex.IsMatch(plateToValidate, @"^[A-Z]{3}-\d{4}$");
        }

        public bool IsValid()
        {
            return IsIdValid() &&
                   IsYearValid() &&
                   IsModelValid() &&
                   IsLicensePlateValid();
        }

        private bool IsIdValid()
        {
            return !string.IsNullOrWhiteSpace(Id);
        }

        private bool IsYearValid()
        {
            return Year >= 1900 && Year <= DateTime.Now.Year + 1;
        }

        private bool IsModelValid()
        {
            return !string.IsNullOrWhiteSpace(Model);
        }

        private bool IsLicensePlateValid()
        {
            return !string.IsNullOrWhiteSpace(LicensePlate) && IsValidLicensePlate();
        }
    }
}
