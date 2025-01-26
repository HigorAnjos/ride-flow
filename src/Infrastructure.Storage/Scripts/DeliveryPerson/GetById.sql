SELECT 
    Identificador AS Id,
    Nome,
    CNPJ,
    DataNascimento AS DateOfBirth,
    NumeroCNH AS LicenseNumber,
    TipoCNH AS LicenseType
FROM Entregadores
WHERE Identificador = @Id;
