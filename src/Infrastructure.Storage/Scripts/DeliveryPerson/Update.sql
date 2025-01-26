UPDATE Entregadores
SET
    Nome = @Name,
    CNPJ = @CNPJ,
    DataNascimento = @DateOfBirth,
    NumeroCNH = @LicenseNumber,
    TipoCNH = @LicenseType,
    LicenseImage = @LicenseImage
WHERE Identificador = @Id;
