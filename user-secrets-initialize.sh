ROOT="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
cd "$ROOT"

echo "Environment configuration script."

echo -n "Username : "
read -r username
echo -n "Password : "
read -r password
echo -n "Url : "
read -r url
echo -n "Seq URL"
read -r seq_url
echo -n "Seq Api Key"
read -r seq_api_key

pn="ViolationPublisher"

dotnet user-secrets set "Kafka:ConsumerSettings:SaslUsername" "$username" --project "$pn/$pn.csproj"
dotnet user-secrets set "Kafka:ConsumerSettings:SaslPassword" "$password" --project "$pn/$pn.csproj"
dotnet user-secrets set "Kafka:ConsumerSettings:BootstrapServers" "$url" --project "$pn/$pn.csproj"
dotnet user-secrets set "Seq:Url" "$seq_url" --project "$pn/$pn.csproj"
dotnet user-secrets set "Seq:ApiKey" "$seq_api_key" --project "$pn/$pn.csproj"