using System.Diagnostics.CodeAnalysis;
using MiddlewareDemo;
using YourProject;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();

var app = builder.Build();

app.UseSimpleError();
app.UseSimpleAuth();
app.UseSimpleLogging();

List<User>users = new()
{
  new User {Id = 1 , Name = "Sadik",Role = "Tax"},
  new User {Id = 2 , Name = "Raman", Role = "Consulting"},  
};
app.MapGet("/users",() => 
{
    
    try
    {
        return Results.Ok(users);
    }
    catch (Exception ex)
    {
        
        return Results.Problem($"Unexpected error: {ex.Message}");
    }

});

app.MapGet("/users/{id}",(int id) =>
{
   

    try
    {
        var p = users.FirstOrDefault(x => x.Id == id);
        return p is not null ? Results.Ok(p) : Results.NotFound($"User with id {id} not found.");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Unexpected error: {ex.Message}");
    }

    
});

app.MapPost("/users", (User p) =>
{
    try
    {
      
        if (string.IsNullOrWhiteSpace(p.Name))
            return Results.BadRequest("Name cannot be empty");

        if (p.Id <= 0)
            return Results.BadRequest("Id must be greater than 0");

       
        if (users.Any(x => x.Id == p.Id))
            return Results.Conflict($"A user with Id {p.Id} already exists.");
        
        if (users.Any(x => string.Equals(x.Name, p.Name, StringComparison.OrdinalIgnoreCase)))
            return Results.Conflict($"A user with name '{p.Name}' already exists.");
        
        if (string.IsNullOrWhiteSpace(p.Role))
            return Results.BadRequest("Role cannot be empty.");
        
        if (users.Any(x => x.Id == p.Id))
            return Results.Conflict($"A user with Id {p.Id} already exists.");

        if (users.Any(x => x.Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase)))
            return Results.Conflict($"A user with name '{p.Name}' already exists.");

       

        users.Add(p);
        return Results.Created($"/users/{p.Id}", p);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Unexpected error: {ex.Message}");
    }
});


app.MapPut("/users/{id}", (int id, User updated) =>
{
    try
    {
     
        var existing = users.FirstOrDefault(x => x.Id == id);
        if (existing is null)
            return Results.NotFound($"User with id {id} not found.");

    
        if (updated is null)
            return Results.BadRequest("Request body is required.");

        if (string.IsNullOrWhiteSpace(updated.Name))
            return Results.BadRequest("Name cannot be empty.");

        
        if (string.IsNullOrWhiteSpace(updated.Role))
            return Results.BadRequest("Role cannot be empty.");


        existing.Name = updated.Name.Trim();
        existing.Role = updated.Role.Trim();
        return Results.Ok(existing);
    }
    catch (Exception ex)
    {
        
        return Results.Problem($"Unexpected error: {ex.Message}");
    }
});


app.MapDelete("/users/{id}", (int id) =>
{
    try
    {
        var p = users.FirstOrDefault(x => x.Id == id);
        if (p is null)
            return Results.NotFound($"User with id {id} not found.");

        users.Remove(p);
            return Results.Ok($"User with id {id} deleted successfully.");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Unexpected error: {ex.Message}");
    }
});

app.Run();

