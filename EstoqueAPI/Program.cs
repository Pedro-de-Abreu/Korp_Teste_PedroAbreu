using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configura o banco SQLite local (vai criar um arquivo estoque.db na pasta)
builder.Services.AddDbContext<EstoqueDb>(opt => opt.UseSqlite("Data Source=estoque.db"));

// 2. Libera o CORS para o Angular conseguir acessar depois
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();
app.UseCors();

// 3. Tática de guerrilha: Cria o banco e as tabelas automaticamente ao rodar o projeto
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EstoqueDb>();
    db.Database.EnsureCreated();
}

// ==== ROTAS DA API (NOSSOS ENDPOINTS) ====

app.MapGet("/", () => "Microsserviço de Estoque Rodando!");

// Listar todos os produtos
app.MapGet("/produtos", async (EstoqueDb db) => await db.Produtos.ToListAsync());

// Cadastrar novo produto
app.MapPost("/produtos", async (Produto produto, EstoqueDb db) =>
{
    db.Produtos.Add(produto);
    await db.SaveChangesAsync();
    return Results.Created($"/produtos/{produto.Id}", produto);
});

// Baixar Estoque (Essa rota será chamada pelo Microsserviço de Faturamento)
app.MapPost("/produtos/{id}/baixar", async (int id, BaixaEstoqueRequest req, EstoqueDb db) =>
{
    var produto = await db.Produtos.FindAsync(id);
    
    if (produto is null) 
        return Results.NotFound(new { erro = "Produto não encontrado." });
    
    if (produto.Saldo < req.Quantidade) 
        return Results.BadRequest(new { erro = $"Saldo insuficiente. Saldo atual: {produto.Saldo}" });

    // Atualiza o saldo
    produto.Saldo -= req.Quantidade;
    await db.SaveChangesAsync();
    
    return Results.Ok(produto);
});

// FIXAMOS a porta 5001 para o Estoque. Assim o Faturamento sempre acha ele.
app.Run("http://localhost:5001"); 


// ==== NOSSOS MODELOS (TABELAS E REQUESTS) ====

public class Produto
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public int Saldo { get; set; }
}

public class BaixaEstoqueRequest
{
    public int Quantidade { get; set; }
}

class EstoqueDb : DbContext
{
    public EstoqueDb(DbContextOptions<EstoqueDb> options) : base(options) { }
    public DbSet<Produto> Produtos => Set<Produto>();
}