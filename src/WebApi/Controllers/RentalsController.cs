using Application.UseCase;
using Domain.Entities;
using Domain.Enumerables;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("locacao")]
    public class RentalsController : ControllerBase
    {
        private readonly IRentMotorcycleUseCase _rentMotorcycleUseCase;
        private readonly IGetRentalByIdUseCase _getRentalByIdUseCase;
        private readonly IUpdateRentalReturnDateUseCase _updateRentalReturnDateUseCase;

        public RentalsController(IRentMotorcycleUseCase rentMotorcycleUseCase, IGetRentalByIdUseCase getRentalByIdUseCase, IUpdateRentalReturnDateUseCase updateRentalReturnDateUseCase)
        {
            _rentMotorcycleUseCase = rentMotorcycleUseCase;
            _getRentalByIdUseCase = getRentalByIdUseCase;
            _updateRentalReturnDateUseCase = updateRentalReturnDateUseCase;
        }

        [HttpPost]
        public IActionResult RentMotorcycle([FromBody] RentMotorcycleRequest request, CancellationToken cancellationToken)
        {
            if (request == null || !request.IsValid())
                return BadRequest(new { Mensagem = "Dados inválidos" });

            try
            {
                _rentMotorcycleUseCase.ExecuteAsync(
                    deliveryPersonId: request.Entregador_Id,
                    motorcycleId: request.Moto_Id,
                    startDate: request.Data_Inicio,
                    planType: request.Plano,
                    null,
                    cancellationToken
                );

                return Created();
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(new { Mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensagem = "Erro interno no servidor.", Detalhes = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRental(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { Mensagem = "Dados inválidos" });
            }

            try
            {
                var rental = await _getRentalByIdUseCase.ExecuteAsync(id, cancellationToken);

                if (rental == null)
                {
                    return NotFound(new { Mensagem = "Locação não encontrada." });
                }

                var rentalDto = (RentMotorcycleResponse)rental;

                return Ok(rentalDto);
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(new { Mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensagem = "Erro interno no servidor.", Detalhes = ex.Message });
            }
        }

        [HttpPut("{id}/devolucao")]
        public async Task<IActionResult> UpdateReturnDate(string id, [FromBody] RentMotorcycleReturnDateRequest request, CancellationToken cancellationToken)
        {
            if (request.Data_Devolucao == default)
            {
                return BadRequest(new { Mensagem = "Dados inválidos" });
            }

            try
            {
                var finalCost = await _updateRentalReturnDateUseCase.ExecuteAsync(id, request.Data_Devolucao, cancellationToken);

                return Ok(new
                {
                    Mensagem = "Data de devolução informada com sucesso",
                    //ValorFinal = finalCost
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Mensagem = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { Mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensagem = "Erro interno no servidor.", Detalhes = ex.Message });
            }
        }

    }
    

    public class RentMotorcycleRequest
    {
        public string Entregador_Id { get; set; }
        public string Moto_Id { get; set; }
        public DateTime Data_Inicio { get; set; }
        public DateTime Data_Termino { get; set; }
        public DateTime Data_Previsao_Termino { get; set; }
        public RentalPlanTypeEnum Plano { get; set; }
        
        public bool IsValid() 
        {
            if (string.IsNullOrWhiteSpace(Entregador_Id))
                return false;

            if (string.IsNullOrWhiteSpace(Moto_Id))
                return false;

            if (Data_Inicio == default)
                return false;

            if (!Enum.IsDefined(typeof(RentalPlanTypeEnum), Plano))
                return false;

            return true;
        }

        public static explicit operator RentMotorcycleRequest(Rental rental)
        {
            if (rental == null)
                return null;

            return new RentMotorcycleRequest
            {
                Entregador_Id = rental.DeliveryPersonId,
                Moto_Id = rental.MotorcycleId,
                Data_Inicio = rental.StartDate,
                Data_Termino = rental.EndDate,
                Data_Previsao_Termino = rental.ExpectedEndDate,
                Plano = rental.RentalPlan != null ? (RentalPlanTypeEnum)rental.RentalPlan.DurationInDays : RentalPlanTypeEnum.None
            };
        }

    }

    public class RentMotorcycleResponse
    {
        public string Identificador { get; set; }
        public decimal Valor_Diaria { get; set; }
        public string Entregador_Id { get; set; }
        public string Moto_Id { get; set; }
        public DateTime Data_Inicio { get; set; }
        public DateTime Data_Termino { get; set; }
        public DateTime Data_Previsao_Termino { get; set; }
        public DateTime? Data_Devolucao { get; set; }

        public static explicit operator RentMotorcycleResponse(Rental rental)
        {
            if (rental == null)
                return null;

            return new RentMotorcycleResponse
            {
                Identificador = rental.Id,
                Valor_Diaria = rental.RentalPlan.DailyRate,
                Entregador_Id = rental.DeliveryPersonId,
                Moto_Id = rental.MotorcycleId,

                Data_Inicio = rental.StartDate,
                Data_Termino = rental.EndDate,
                Data_Previsao_Termino = rental.ExpectedEndDate,
                Data_Devolucao = rental.ReturnDate, 
            };
        }

    }

    public class RentMotorcycleReturnDateRequest
    {
        public DateTime Data_Devolucao { get; set; }
    }
}
