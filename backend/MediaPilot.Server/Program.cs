var builder = WebApplication.CreateBuilder(args);

builder.Services
  .AddGraphQLServer()
  .AddFiltering()
  .AddSorting();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.MapGraphQL();

app.Run();
