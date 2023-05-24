using Azure.Identity;
using ViolationPublisher;

var builder = WebApplication.CreateBuilder(args);

var seqUrl = builder.Configuration.GetValue<string>("Seq:Url");
var seqApikey = builder.Configuration.GetValue<string>("Seq:ApiKey");

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSeq(seqUrl, seqApikey);
});

if (builder.Configuration.GetValue("UseVault", false))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{builder.Configuration["Vault:Name"]}.vault.azure.net/"),
        new DefaultAzureCredential());
}

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<ViolationPublisherBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();