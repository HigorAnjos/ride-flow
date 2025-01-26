using Application.UseCase;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("motos")]
    public class MotorcycleController : ControllerBase
    {
        private readonly ICreateMotorcycleUseCase _createMotorcycleUseCase;
        private readonly IGetMotorcyclesUseCase _getMotorcyclesUseCase;
        private readonly IUpdateMotorcycleLicensePlateUseCase _updateMotorcycleLicensePlateUseCase;
        private readonly IGetMotorcycleByIdUseCase _getMotorcycleByIdUseCase;
        private readonly IDeleteMotorcycleUseCase _deleteMotorcycleUseCase;


        public MotorcycleController(ICreateMotorcycleUseCase createMotorcycleUseCase, IGetMotorcyclesUseCase getMotorcyclesUseCase, IUpdateMotorcycleLicensePlateUseCase updateMotorcycleLicensePlateUseCase, IDeleteMotorcycleUseCase deleteMotorcycleUseCase, IGetMotorcycleByIdUseCase getMotorcycleByIdUseCase)
        {
            _createMotorcycleUseCase = createMotorcycleUseCase;
            _getMotorcyclesUseCase = getMotorcyclesUseCase;
            _updateMotorcycleLicensePlateUseCase = updateMotorcycleLicensePlateUseCase;
            _deleteMotorcycleUseCase = deleteMotorcycleUseCase;
            _getMotorcycleByIdUseCase = getMotorcycleByIdUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMotorcycle([FromBody] MotorcycleDto motorcycle, CancellationToken cancellationToken)
        {
            if (motorcycle == null)
                return BadRequest(new { Mensagem = "Dados inválidos" });

            try
            {
                await _createMotorcycleUseCase.ExecuteAsync(motorcycle.Identificador, motorcycle.Ano, motorcycle.Modelo, motorcycle.Placa, cancellationToken);
                return Created($"/locacoes", null);
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

        [HttpGet]
        public async Task<IActionResult> GetMotorcycles([FromQuery] string? placa, CancellationToken cancellationToken)
        {
            try
            {
                var motorcycles = await _getMotorcyclesUseCase.ExecuteAsync(cancellationToken, placa);

                if (motorcycles == null || !motorcycles.Any())
                {
                    return NotFound(new { Mensagem = "Nenhuma moto encontrada." });
                }

                var motorcycleDtos = motorcycles.Select(m => (MotorcycleDto)m).ToList();

                return Ok(motorcycleDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensagem = "Erro interno no servidor.", Detalhes = ex.Message });
            }
        }


        [HttpPut("{id}/placa")]
        public async Task<IActionResult> UpdateLicensePlate(string id, [FromBody] UpdateLicensePlateRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _updateMotorcycleLicensePlateUseCase.ExecuteAsync(id, request.Placa, cancellationToken);
                return Ok(new { Mensagem = "Placa modificada com sucesso" });
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
        public async Task<IActionResult> GetMotorcycleById(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { Mensagem = "Request mal formada" });
            }

            try
            {
                var motorcycle = await _getMotorcycleByIdUseCase.ExecuteAsync(id, cancellationToken);

                if (motorcycle == null)
                {
                    return NotFound(new { Mensagem = "Moto não encontrada" });
                }

                var motorcycleDto = (MotorcycleDto)motorcycle;

                return Ok(motorcycleDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensagem = "Erro interno no servidor.", Detalhes = ex.Message });
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMotorcycle(string id, CancellationToken cancellationToken)
        {
            try
            {
                await _deleteMotorcycleUseCase.ExecuteAsync(id, cancellationToken);
                return Ok();
            }
            catch (InvalidDataException ex)
            {
                return NotFound(new { Mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensagem = "Erro interno no servidor.", Detalhes = ex.Message });
            }
        }
    }

    public class MotorcycleDto 
    {
        public string Identificador { get; set; }
        public int Ano { get; set; }
        public string Placa { get; set; }
        public string Modelo { get; set; }

        public static explicit operator MotorcycleDto(Motorcycle motorcycle)
        {
            if (motorcycle == null) return null;

            return new MotorcycleDto
            {
                Identificador = motorcycle.Id,
                Ano = motorcycle.Year,
                Modelo = motorcycle.Model,
                Placa = motorcycle.LicensePlate
            };
        }
    }

    public class UpdateLicensePlateRequest
    {
        public string Placa { get; set; }
    }
}
