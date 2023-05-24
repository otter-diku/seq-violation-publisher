using ViolationPublisher;

var builder = WebApplication.CreateBuilder(args);

var seqUrl = builder.Configuration.GetSection("Seq:Url").Value!;
var seqApikey = builder.Configuration.GetSection("Seq:ApiKey").Value!;

builder.Services.AddLogging(loggingBuilder =>
{
    // loggingBuilder.ClearProviders();
    loggingBuilder.AddSeq(seqUrl, seqApikey);
});

// Seq.Extensions.Logging.SelfLog.Enable(Console.Error);
// Seq.Extensions.Logging.SelfLog.Enable(message => {
//     Console.WriteLine(message);
// });

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