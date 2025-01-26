SELECT 
    Identificador AS Id,
    Ano AS Year,
    Modelo AS Model,
    Placa AS LicensePlate
FROM Motos
WHERE Identificador = @Id;
