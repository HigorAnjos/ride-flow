using Domain.Enumerables;
using System;
using System.Text.RegularExpressions;

namespace Domain.Entities
{
    public class DeliveryPerson
    {
        public string Id { get; set; } // Identificador único
        public string Name { get; set; } // Nome do entregador
        public string CNPJ { get; set; } // CNPJ do entregador
        public DateTime DateOfBirth { get; set; } // Data de nascimento
        public string LicenseNumber { get; set; } // Número da CNH
        public LicenseTypeEnum LicenseType { get; set; } // Tipo da CNH (A, B ou A+B)
        public string LicenseImage { get; set; } // Imagem da CNH (base64 string ou link para storage)

        // Método para verificar se o entregador está habilitado para alugar motos (CNH tipo "A" ou "A+B")
        public bool CanRentMotorcycle()
        {
            return LicenseType == LicenseTypeEnum.A || LicenseType == LicenseTypeEnum.AB;
        }

        // Método para calcular a idade do entregador
        public int GetAge()
        {
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }

        // Método para validar o CNPJ
        public bool IsValidCNPJ()
        {
            return Regex.IsMatch(CNPJ, @"^\d{14}$");
        }

        // Método para validar os dados do entregador
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Id))
                return false;

            if (string.IsNullOrWhiteSpace(Name))
                return false;

            if (string.IsNullOrWhiteSpace(CNPJ) || !IsValidCNPJ())
                return false;

            if (DateOfBirth == default)
                return false;

            if (GetAge() < 18)
                return false;

            if (string.IsNullOrWhiteSpace(LicenseNumber))
                return false;

            if (!CanRentMotorcycle())
                return false;
            
            return true;
        }
    }
}
