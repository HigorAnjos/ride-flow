using Application.UseCase;
using Domain.Entities;
using Domain.Enumerables;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Threading;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("entregadores")]
    public class DeliveryPersonController : ControllerBase
    {
        private readonly ICreateDeliveryPersonUseCase _createDeliveryPersonUseCase;
        private readonly IUploadLicenseImageUseCase _uploadLicenseImageUseCase;

        public DeliveryPersonController(ICreateDeliveryPersonUseCase createDeliveryPersonUseCase, IUploadLicenseImageUseCase uploadLicenseImageUseCase)
        {
            _createDeliveryPersonUseCase = createDeliveryPersonUseCase;
            _uploadLicenseImageUseCase = uploadLicenseImageUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDeliveryPerson([FromBody] CreateDeliveryPersonRequest request, CancellationToken cancellationToken)
        {
            if (request == null || !request.IsValid())
                return BadRequest(new { Mensagem = "Dados inválidos" });

            if (!Enum.TryParse<LicenseTypeEnum>(request.Tipo_Cnh, true, out var licenseType))
            {
                return BadRequest(new { Mensagem = "Dados inválidos" });
            }

            try
            {
                var deliveryPerson = new DeliveryPerson
                {
                    Id = request.Identificador,
                    Name = request.Nome,
                    CNPJ = request.CNPJ,
                    DateOfBirth = request.Data_Nascimento,
                    LicenseNumber = request.Numero_Cnh,
                    LicenseType = licenseType,
                    LicenseImage = request.Imagem_Cnh
                };

                await _createDeliveryPersonUseCase.ExecuteAsync(deliveryPerson, cancellationToken);

                return Created();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensagem = "Erro ao cadastrar entregador.", Detalhes = ex.Message });
            }
        }

        [HttpPost("{id}/cnh")]
        public async Task<IActionResult> UploadLicenseImage(string id, [FromBody] DeliveryPersonImageRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(request.Imagem_Cnh))
            {
                return BadRequest(new { Mensagem = "Dados inválidos" });
            }

            try
            {
                await _uploadLicenseImageUseCase.ExecuteAsync(id, request.Imagem_Cnh, cancellationToken);

                return Ok();
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(new { Mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensagem = "Erro ao processar a imagem da CNH.", Detalhes = ex.Message });
            }
        }
    }

    public class CreateDeliveryPersonRequest
    {
        public string Identificador { get; set; }
        public string Nome { get; set; }
        public string CNPJ { get; set; }
        public DateTime Data_Nascimento { get; set; }
        public string Numero_Cnh { get; set; }
        public string Tipo_Cnh { get; set; }
        public string Imagem_Cnh { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Identificador))
                return false;

            if (string.IsNullOrWhiteSpace(Nome))
                return false;

            if (string.IsNullOrWhiteSpace(CNPJ) || !Regex.IsMatch(CNPJ, @"^\d{14}$"))
                return false;

            if (Data_Nascimento == default || (DateTime.Now.Year - Data_Nascimento.Year) < 18)
                return false;

            if (string.IsNullOrWhiteSpace(Numero_Cnh))
                return false;

            return true;
        }
    }

    public class DeliveryPersonImageRequest
    {
        public string Imagem_Cnh { get; set; }
    }
}
