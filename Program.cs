var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

var perguntas = new List<Pergunta>();
var respostas = new List<Resposta>();

app.MapPost("/api/pergunta", (Pergunta pergunta) =>
{
    pergunta.Id = perguntas.Any() ? perguntas.Max(p => p.Id) + 1 : 1;
    perguntas.Add(pergunta);
    return Results.Created($"/api/pergunta/{pergunta.Id}", pergunta);
});

app.MapPut("/api/pergunta/{id}", (int id, Pergunta inputPergunta) =>
{
    var pergunta = perguntas.FirstOrDefault(p => p.Id == id);

    if (pergunta == null)
    {
        return Results.NotFound("Pergunta não encontrada.");
    }

    pergunta.Question = inputPergunta.Question;
    pergunta.ChemicalElement = inputPergunta.ChemicalElement;

    return Results.Ok(pergunta);
});

app.MapDelete("/api/pergunta/{id}", (int id) =>
{
    var pergunta = perguntas.FirstOrDefault(p => p.Id == id);

    if (pergunta == null)
    {
        return Results.NotFound("Pergunta não encontrada.");
    }

    perguntas.Remove(pergunta);
    return Results.Ok("Pergunta excluída com sucesso.");
});

app.MapGet("/api/pergunta", () =>
{
    return Results.Ok(perguntas);
});

app.MapGet("/api/pergunta/{id}", (int id) =>
{
    var pergunta = perguntas.FirstOrDefault(p => p.Id == id);

    if (pergunta == null)
    {
        return Results.NotFound("Pergunta não encontrada.");
    }

    return Results.Ok(pergunta);
});

app.MapPost("/api/resposta", (Resposta resposta) =>
{
    resposta.Id = respostas.Any() ? respostas.Max(r => r.Id) + 1 : 1;
    respostas.Add(resposta);
    return Results.Created($"/api/resposta/{resposta.Id}", resposta);
});

app.MapGet("/api/resposta", () =>
{
    return Results.Ok(respostas);
});

app.MapGet("/api/resposta/{id}", (int id) =>
{
    var resposta = respostas.FirstOrDefault(r => r.Id == id);

    if (resposta == null)
    {
        return Results.NotFound("Resposta não encontrada.");
    }

    return Results.Ok(resposta);
});

app.MapDelete("/api/pergunta", () =>
{
    perguntas.Clear();
    return Results.NoContent();
});

app.MapDelete("/api/resposta", () =>
{
    respostas.Clear();
    return Results.NoContent();
});

app.MapGet("/api/pergunta/maxid", () =>
{
    int maxId = perguntas.Any() ? perguntas.Max(p => p.Id) : 1;
    return Results.Ok(maxId);
});

app.MapGet("/api/resposta/maxid/{alunoId}", (int alunoId) =>
{
    var maxId = respostas.Where(r => r.AlunoId == alunoId).Any() ? respostas.Where(r => r.AlunoId == alunoId).Max(r => r.Id) : 0;
    return Results.Ok(maxId);
});

app.Run();

public class Pergunta
{
    public int Id { get; set; }
    public string Question { get; set; }
    public string ChemicalElement { get; set; }
}

public class Resposta
{
    public int Id { get; set; }
    public int Answer { get; set; }
    public int AlunoId { get; set; }
}
