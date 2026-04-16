using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configura o banco local SQLite para as notas
builder.Services.AddDbContext<FaturamentoDb>(opt => opt.UseSqlite("Data Source=faturamento.db"));

// Habilita CORS pro Angular depois
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Habilita o "cliente" para fazermos requisições HTTP para o Microsserviço de Estoque
builder.Services.AddHttpClient();

var app = builder.Build();
app.UseCors();

// Cria o banco automaticamente
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FaturamentoDb>();
    db.Database.EnsureCreated();
}

// ==== ROTAS DA API DE FATURAMENTO ====

app.MapGet("/", () => "Microsserviço de Faturamento Rodando!");

// Consultar notas emitidas
app.MapGet("/notas", async (FaturamentoDb db) => await db.Notas.ToListAsync());

// EMITIR NOTA FISCAL (Comunicação entre microsserviços e Tratamento de Falhas)
app.MapPost("/notas/emitir", async (NotaFiscalRequest req, FaturamentoDb db, IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();
    var payload = new { Quantidade = req.Quantidade };
    
    HttpResponseMessage response;
    
    try
    {
        // 1. Tenta comunicar com o serviço de Estoque para dar a baixa
        response = await client.PostAsJsonAsync($"http://localhost:5001/produtos/{req.ProdutoId}/baixar", payload);
    }
    catch (HttpRequestException)
    {
        // REQUISITO DO TESTE (TRATAMENTO DE FALHA): Se o Estoque estiver fora do ar, cai aqui!
        return Results.Problem("O Serviço de Estoque está indisponível no momento. Não foi possível emitir a nota fiscal.", statusCode: 503);
    }

    // Se o estoque recusar (ex: saldo insuficiente), repassa o erro
    if (!response.IsSuccessStatusCode)
    {
        var erro = await response.Content.ReadAsStringAsync();
        return Results.BadRequest(new { erro = "Erro ao baixar estoque: " + erro });
    }

    // 2. Se a baixa no estoque deu sucesso, a gente salva a nota fiscal no banco
    var novaNota = new NotaFiscal
    {
        Status = "Fechada", // Requisito: após a impressão, o status muda para fechada
        ProdutoId = req.ProdutoId,
        Quantidade = req.Quantidade
    };

    db.Notas.Add(novaNota);
    await db.SaveChangesAsync();

    return Results.Ok(new { mensagem = "Nota emitida com sucesso!", nota = novaNota });
});

// Rodamos o Faturamento na porta 5002 para não dar conflito com o Estoque
app.Run("http://localhost:5002");


// ==== MODELOS ====

public class NotaFiscal
{
    public int Id { get; set; } // O Id automático já atende o requisito de "Numeração Sequencial"
    public string Status { get; set; } = "Aberta";
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
}

public class NotaFiscalRequest
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
}

class FaturamentoDb : DbContext
{
    public FaturamentoDb(DbContextOptions<FaturamentoDb> options) : base(options) { }
    public DbSet<NotaFiscal> Notas => Set<NotaFiscal>();
}