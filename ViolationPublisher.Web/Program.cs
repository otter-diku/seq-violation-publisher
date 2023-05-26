using ViolationPublisher.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var seqUrl = builder.Configuration.GetValue<string>("Seq:Url");
var seqApiKey = builder.Configuration.GetValue<string>("Seq:ViolationPublisherApiKey");

builder.Services.AddHostedService<ViolationPublisherBackgroundService>();

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSeq(seqUrl, seqApiKey);
});


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