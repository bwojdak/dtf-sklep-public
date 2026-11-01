# DTF Shop Manager

A web app for running a small DTF-printed longsleeve business: designs, stock, sales, profit and listing descriptions in one place. I built it from scratch to learn ASP.NET Core MVC. The interface is in Polish, as the shop operates in Poland.

![Dashboard](docs/dashboard.png)

## Features

- **Dashboard:** current and previous month's profit, low stock, top designs, recent sales
- **Designs:** library with images and print costs
- **Stock:** quantities by color and size, with low-stock warnings
- **Sales:** a register that updates stock automatically and keeps the costs from the time of sale
- **Statistics:** monthly income and profit, sales by design
- **Description generator:** listing descriptions written by OpenAI, kept different from the most recent ones
- **Vinted watcher:** checks two public profiles every 15 minutes, and AI recognizes the color and design from the photo; new listings wait for manual confirmation

![Stock](docs/magazyn.png)
![Description generator](docs/generator.png)

## Tech stack

C#, ASP.NET Core MVC (.NET 10), Entity Framework Core, SQLite, Razor, Bootstrap, AngleSharp, OpenAI API

## Getting started

You need the .NET 10 SDK and the `dotnet-ef` tool.

```bash
cd Sklep_
dotnet ef database update
dotnet user-secrets set "OpenAI:ApiKey" "your-key"
dotnet user-secrets set "Vinted:Profile1Url" "profile-url"
dotnet user-secrets set "Vinted:Profile2Url" "profile-url"
dotnet user-secrets set "Descriptions:Prompt" "prompt-text"
dotnet run
```

The generator prompt is not part of this repository. It can use the placeholders `{Rozmiar}` (size), `{Kolor}` (color), `{Wzor}` (design) and `{PoprzednieOpisy}` (previous descriptions). Without profile URLs, the Vinted watcher skips those profiles.

## Limitations

The app has no login and is meant to run locally. The Vinted watcher reads the page's HTML, so it may stop working if the site changes.