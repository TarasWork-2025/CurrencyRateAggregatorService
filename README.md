# Currency Rate Aggregator Service

A demo **.NET 8 Web API** that fetches and stores exchange rates from the **National Bank of Ukraine (NBU)** using **Clean Architecture**, EF Core (SQLite), caching, logging, and a background seeder.  

---

## How to Run
```bash
git clone <your-repo>
cd CurrencyRateAggregatorService
dotnet restore
dotnet run --project CurrencyRateAggregatorService.API
```

- API → `https://localhost:5001/swagger`  
- Logs → `Logs/app.log`  
- DB → SQLite file auto-created (`db.sqlite`) in API project. 

---

## Preconfigured
- **Seeder**: on startup, loads last **3 months** of USD→UAH rates from NBU.  
- **Database**: SQLite, migrations applied automatically.  
- **Caching**: in-memory.  
- **Logging**: Serilog to console + `Logs/app.log`.  
- **Swagger**: enabled in Development mode.  

---

## Limitations
- Only **USD→UAH** supported.  
- Stores only **last 3 months** of data.
- Older dates can be requested, but not stored in db.  
- Average endpoint returns data only for available in db days.  
- No authentication/authorization.  
- SQLite used for demo only (not production).  
