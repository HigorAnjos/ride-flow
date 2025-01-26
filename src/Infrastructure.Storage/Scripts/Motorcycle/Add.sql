INSERT INTO Motos (Identificador, Ano, Modelo, Placa)
VALUES (@Id, @Year, @Model, @LicensePlate)
ON CONFLICT (Placa)
DO UPDATE SET
    Ano = @Year,
    Modelo = @Model,
    Placa = @LicensePlate;
