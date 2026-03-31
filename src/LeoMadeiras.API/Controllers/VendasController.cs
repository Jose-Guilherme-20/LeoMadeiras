using LeoMadeiras.Application.UseCases.Vendas.RegistrarVenda;
using LeoMadeiras.Application.ViewModels.Vendas.Request;
using LeoMadeiras.Application.ViewModels.Vendas.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeoMadeiras.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class VendasController : ControllerBase
{
    /// <summary>Registra uma nova venda com débito de estoque transacional.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(VendaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Registrar(
        [FromBody] CriarVendaRequest request,
        [FromServices] IRegistrarVendaUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(request, ct);
        return CreatedAtAction(nameof(Registrar), new { id = result.Id }, result);
    }
}