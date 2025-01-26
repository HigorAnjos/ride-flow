UPDATE Motos
SET 
    Ano = @Year,
    Modelo = @Model,
    Placa = @LicensePlate
WHERE Identificador = @Id;
