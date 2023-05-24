using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using ViolationPublisher;

var builder = WebApplication.CreateBuilder(args);

string? seqUrl;
string? seqApiKey;
if (builder.Configuration.GetValue("UseVault", false))
{
    var secretClient = new SecretClient(
        new Uri($"https://{builder.Configuration["Vault:Name"]}.vault.azure.net/"),
        new DefaultAzureCredential());

    seqUrl = secretClient.GetSecret("Seq--Url").Value.Value;
    seqApiKey = secretClient.GetSecret("Seq--ViolationPublisherApiKey").Value.Value;

    // TODO: we shouldnt need to do both the .GetSecret above and .AddAzureKeyVault - but for some reason at this point
    // the Configuration["secret"] is not being read from the KV at this point
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{builder.Configuration["Vault:Name"]}.vault.azure.net/"),
        new DefaultAzureCredential());
}
else
{
    seqUrl = builder.Configuration.GetValue<string>("Seq:Url");
    seqApiKey = builder.Configuration.GetValue<string>("Seq:ViolationPublisherApiKey");
}

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSeq(seqUrl, seqApiKey);
});


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