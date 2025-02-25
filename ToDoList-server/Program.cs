using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
              
    });
});

var connectionString = builder.Configuration.GetConnectionString("ToDo");

builder.Services.AddDbContext<ToDoDbContext>(options => options.UseMySql
(builder.Configuration.GetConnectionString("ToDo"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ToDo"))));


var app = builder.Build();
app.UseCors("AllowAllOrigins");

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Api is working");


app.MapGet("/ToDos", async (ToDoDbContext db) =>
{
    return await db.Items.ToListAsync();
});

app.MapGet("/ToDos/{id}", async (int id, ToDoDbContext db) =>
{
    var item = await db.Items.FindAsync(id);
    return item is not null ? Results.Ok(item) : Results.NotFound();
});
app.MapPost("/ToDos", async (Item item, ToDoDbContext db) =>
{
    await  db.Items.AddAsync(item);
    await db.SaveChangesAsync();
    return Results.Created($"/ToDos/{item.Id}", item);
});


app.MapPut("/ToDos/{id}", async ( int id, bool isComplete,ToDoDbContext db) =>{

    var item = await db.Items.FindAsync(id);
    if (item == null) return Results.NotFound("task not found.");

    item.IsComplete = isComplete;
    await db.SaveChangesAsync();
    
    return Results.Ok(item);
});


app.MapDelete("/ToDos/{id}", async (int id, ToDoDbContext db) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    db.Items.Remove(item);
    await db.SaveChangesAsync();

    return Results.NoContent();
});


app.Run();
