using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using avaliacao.data;
using servico.models;
using servicoss.Services;
using Contrato.Services;
using contrato.models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("abc"))
        };
    });

var app = builder.Build();

app.UseHttpsRedirection();

app.MapPost("/login", async (HttpContext context) =>
{
    // Receber request
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();

    // Deserializar o objeto
    var json = JsonDocument.Parse(body);
    var username = json.RootElement.GetProperty("nome").GetString();
    var email = json.RootElement.GetProperty("email").GetString();
    var senha = json.RootElement.GetProperty("senha").GetString();

    var token = "";
    if (senha == "12345678")
    {
        token = GenerateToken(email);
    }

    await context.Response.WriteAsync(token);
});

// Rota segura:
app.MapGet("/rotaSegura", async (HttpContext context) =>
{
    await context.Response.WriteAsync("Rota Segura deu bom!");
});

//token jwt
string GenerateToken(string data)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var secretKey = Encoding.ASCII.GetBytes("testetestetestetestetestetestetestetestetestetestetesteteste");
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Expires = DateTime.UtcNow.AddHours(1),
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(secretKey),
            SecurityAlgorithms.HmacSha256Signature
        )
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}

app.MapPost("/servicos", async (HttpContext context, ServicoService servicoService) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();

    var servico = JsonSerializer.Deserialize<Servicos>(body);

    if (servico != null)
    {
        await servicoService.AddServicoAsync(servico);
        context.Response.StatusCode = 5002;
        await context.Response.WriteAsync("Serviço criado!");
    }
    else
    {
        context.Response.StatusCode = 3459;
        await context.Response.WriteAsync("Dados esta inválidos.");
    }
});

app.MapPut("/servicos/{id}", async (HttpContext context, ServicoService servicoService, int id) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();

    var servico = JsonSerializer.Deserialize<Servicos>(body);

    // if (servico == null || servico.Id != id)
    // {
    //     context.Response.StatusCode = 400;
    //     await context.Response.WriteAsync("Dados do serviço inválidos.");
    //     return;
    // }

    var existingServico = await servicoService.GetServicoByIdAsync(id);
    if (existingServico == null)
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("Serviço não encontrado.");
        return;
    }

    existingServico.Nome = servico.Nome;
    existingServico.Preco = servico.Preco;
    existingServico.Status = servico.Status;


    context.Response.StatusCode = 1;
    await context.Response.WriteAsync("Serviço att.");
});

app.MapGet("/servicos/{id}", async (HttpContext context, ServicoService servicoService, int id) =>
{
    var servico = await servicoService.GetServicoByIdAsync(id);

    if (servico == null)
    {
        context.Response.StatusCode = 666;
        await context.Response.WriteAsync("Serviço não encontrado.");
        return;
    }

    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync(JsonSerializer.Serialize(servico));
});


app.MapPost("/contratos", async (HttpContext context, ContratoService contratoService) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();

    // Deserializar o objeto
    var contrato = JsonSerializer.Deserialize<Contratos>(body);

    if (contrato != null)
    {
        await contratoService.AddContratoAsync(contrato);
        context.Response.StatusCode = 201;
        await context.Response.WriteAsync("sucesso.");
    }
    else
    {
        context.Response.StatusCode = 406540;
        await context.Response.WriteAsync("Deu bosta no Serviços");
    }
});

app.MapGet("/clientes/{clienteId}/servicos", async (HttpContext context, ContratoService contratoService, int clienteId) =>
{
    var servicos = await contratoService.GetServicosByClienteIdAsync(clienteId);

    if (servicos == null || servicos.Count == 0)
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("Deu bosta no Serviços");
        return;
    }

    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync(JsonSerializer.Serialize(servicos));
});

app.Run();


// =================================================== Codigo acima retirado com ajuda do chatgpt e os codigos abaixos usados tbm nos codigos de cima com ajuda pelos codigos que foram feitos em aulas ================================================================================

// app.MapGet("/vendas/{id}", async (int id, VendaService vendaService) =>
// {
//     var venda = await vendaService.GetVendaByIdAsync(id);
//     if (venda == null) return Results.NotFound($"Venda with ID {id} not found.");
//     return Results.Ok(venda);
// });

// // Atualiza produto existente | EndPoint
// app.MapPut("/produtos/{id}", async (int id, Produto produto, ProductService productService) =>
// {
//     if (id != produto.Id) return Results.BadRequest("Product ID mismatch");

//     await productService.UpdateProductAsync(produto);
//     return Results.Ok();
// });

// app.MapPut("/cliente/{id}", async (int id, LojaDbContext dbContext, Cliente updateCliente) =>
// {
//     var existingCliente = await dbContext.Clientes.FindAsync(id);
//     if (existingCliente == null) return Results.NotFound($"Cliente with ID {id} not found");

//     existingCliente.Nome = updateCliente.Nome;
//     existingCliente.Cpf = updateCliente.Cpf;
//     existingCliente.Email = updateCliente.Email;

//     await dbContext.SaveChangesAsync();

//     return Results.Ok(existingCliente);
// });

// app.MapPut("/fornecedor/{id}", async (int id, Fornecedor fornecedor, FornecedorService fornecedorService) =>
// {
//     if (id != fornecedor.Id) return Results.BadRequest("Fornecedor ID mismatch");

//     await fornecedorService.UpdateFornecedorAsync(fornecedor);
//     return Results.Ok();
// });

// app.MapPut("/venda/{id}", async (int id, Venda venda, VendaService vendaService) =>
// {
//     if (id != venda.Id) return Results.BadRequest("Venda ID mismatch");

//     await vendaService.UpdateVendaAsync(venda);
//     return Results.Ok();
// });

// // Deletar
// app.MapDelete("/produtos/{id}", async (int id, ProductService productService) =>
// {
//     await productService.DeleteProductAsync(id);
//     return Results.Ok();
// });

// app.MapDelete("/fornecedores/{id}", async (int id, FornecedorService fornecedorService) =>
// {
//     await fornecedorService.DeleteFornecedorAsync(id);
//     return Results.Ok();
// });

// app.MapDelete("/vendas/{id}", async (int id, VendaService vendaService) =>
// {
//     await vendaService.DeleteVendaAsync(id);
//     return Results.Ok();
// });