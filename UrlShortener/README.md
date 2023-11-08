# Build your first Orleans app using ASP.NET Core 7.0

## Features

* Orleans integration using Grains and Silos.
* Persistent state to save the shortened URLs.
* Web service endpoints to create and redirect shortened URLs.

## Getting Started

### Prerequisites

- [.NET 7.0](http://dotnet.microsoft.com)

# Criar o projeto usando o Visual Studio Code
dotnet new web -o UrlShortener -f net7.0

# Adicionar o Orleans ao projeto
dotnet add package Microsoft.Orleans.Server -v 7.0.0

# Adicione using Orleans a sua classe Program
using Microsoft.AspNetCore.Http.Extensions;
using Orleans.Runtime;

# Teste App
1- Na barra de endereços do navegador, teste o ponto de extremidade shorten inserindo um caminho de URL, como {localhost}/shorten/https://microsoft.com. A página deve recarregar e fornecer uma URL abreviada. Copie a URL abreviada para sua área de transferência.

2- Cole a URL abreviada na barra de endereços e pressione Enter. A página deve recarregar e redirecionar você para https://microsoft.com