INSERT INTO Entregadores (Identificador, Nome, CNPJ, DataNascimento, NumeroCNH, TipoCNH)
VALUES (@Id, @Name, @CNPJ, @DateOfBirth, @LicenseNumber, @LicenseType)
RETURNING Identificador;
