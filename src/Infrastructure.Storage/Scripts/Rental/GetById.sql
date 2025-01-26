SELECT 
    Identificador AS Id,
    EntregadorId AS DeliveryPersonId,
    MotoId AS MotorcycleId,
    DataInicio AS StartDate,
    DataTermino AS EndDate,
    DataPrevisaoTermino AS ExpectedEndDate,
    DataDevolucao AS ReturnDate,
    Plano AS RentalPlanType
FROM Locacoes
WHERE Identificador = @Id;
