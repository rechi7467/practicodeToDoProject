using Microsoft.EntityFrameworkCore;
using TodoApi;
using ToDoDbContext = TodoApi.ToDoDbContext;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ToDoDB");

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.Parse("8.0.41-mysql")));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin() // Allow any origin
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("AllowAllOrigins"); 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

app.MapGet("/",()=>"serever-items is runing!");
app.MapGet("/items", async (ToDoDbContext db) =>{
var a= await db.Items.ToListAsync();
return Results.Ok(a);
});

app.MapPost("/items", async ( ToDoDbContext db,Item newTask) => {
    await db.Items.AddAsync(newTask);
    await db.SaveChangesAsync();
    return Results.Created($"/item/{newTask.Id}", newTask);
});

app.MapPut("/items/{id}", async (ToDoDbContext db ,int id,bool inputTask) =>
{
    var task = await db.Items.FindAsync(id);

    if (task is null) return Results.NotFound();
    task.IsComplete = !task.IsComplete;
    await db.SaveChangesAsync();
    return Results.Ok(task);
});
app.MapDelete("/items/{id}", async (ToDoDbContext db, int id) =>
{
    var task = await db.Items.FindAsync(id);

    if (task is null) return Results.NotFound();

    db.Items.Remove(task);
    await db.SaveChangesAsync();

    return Results.Ok();
});

app.Run();
