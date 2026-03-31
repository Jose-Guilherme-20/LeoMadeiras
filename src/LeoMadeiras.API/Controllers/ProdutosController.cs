using LeoMadeiras.Application.UseCases.Produtos.AtualizarProduto;
using LeoMadeiras.Application.UseCases.Produtos.BuscarProduto;
using LeoMadeiras.Application.UseCases.Produtos.CriarProduto;
using LeoMadeiras.Application.UseCases.Produtos.DeletarProduto;
using LeoMadeiras.Application.UseCases.Produtos.ListarProdutos;
using LeoMadeiras.Application.UseCases.Produtos.MaisVendidos;
using LeoMadeiras.Application.ViewModels.Common;
using LeoMadeiras.Application.ViewModels.Produtos.Request;
using LeoMadeiras.Application.ViewModels.Produtos.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeoMadeiras.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProdutosController : ControllerBase
{
    /// <summary>Lista produtos com filtros, ordenação e paginação.</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResultViewModel<ProdutoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        [FromQuery] ProdutoFiltroRequest filtro,
        [FromServices] IListarProdutosUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(filtro, ct);
        return Ok(result);
    }

    /// <summary>Busca produto por ID.</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Buscar(
        int id,
        [FromServices] IBuscarProdutoUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(id, ct);
        return Ok(result);
    }

    /// <summary>Cria um novo produto.</summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Criar(
        [FromBody] CriarProdutoRequest request,
        [FromServices] ICriarProdutoUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(request, ct);
        return CreatedAtAction(nameof(Buscar), new { id = result.Id }, result);
    }

    /// <summary>Atualiza produto existente.</summary>
    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(
        int id,
        [FromBody] AtualizarProdutoRequest request,
        [FromServices] IAtualizarProdutoUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(id, request, ct);
        return Ok(result);
    }

    /// <summary>Remove produto.</summary>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deletar(
        int id,
        [FromServices] IDeletarProdutoUseCase useCase,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(id, ct);
        return NoContent();
    }

    /// <summary>Retorna os produtos mais vendidos ordenados por quantidade.</summary>
    [HttpGet("mais-vendidos")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<MaisVendidoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MaisVendidos(
        [FromServices] IMaisVendidosUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(ct);
        return Ok(result);
    }
}