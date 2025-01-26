INSERT INTO Locacoes (Identificador, EntregadorId, MotoId, DataInicio, DataTermino, DataPrevisaoTermino, DataDevolucao, Plano)
VALUES (@Id, @DeliveryPersonId, @MotorcycleId, @StartDate, @EndDate, @ExpectedEndDate, @ReturnDate, @RentalPlan)
ON CONFLICT (Identificador)
DO UPDATE SET
    EntregadorId = @DeliveryPersonId,
    MotoId = @MotorcycleId,
    DataInicio = @StartDate,
    DataTermino = @EndDate,
    DataPrevisaoTermino = @ExpectedEndDate,
    DataDevolucao = @ReturnDate,
    Plano = @RentalPlan;
