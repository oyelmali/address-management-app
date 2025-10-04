Write-Host "Cleaning up old containers..." -ForegroundColor Yellow
docker stop addressapp-api 2>$null
docker rm addressapp-api 2>$null

Write-Host "Building Docker image..." -ForegroundColor Green
docker build -t addressapp-api .

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Starting container..." -ForegroundColor Green
docker run -d `
    --name addressapp-api `
    -p 8080:8080 `
    -e "ConnectionStrings__DefaultConnection=User Id=postgres.cztqtxivdrkibtkczdrt;Password=Meryus.2855;Server=aws-1-eu-central-1.pooler.supabase.com;Port=5432;Database=postgres" `
    addressapp-api

Start-Sleep -Seconds 5

Write-Host "Checking container status..." -ForegroundColor Yellow
docker ps | Select-String "addressapp-api"

Write-Host "Checking logs..." -ForegroundColor Yellow
docker logs addressapp-api --tail 20

Write-Host "`nTesting endpoints..." -ForegroundColor Green

# Test health endpoint
Write-Host "`n1. Health Check:" -ForegroundColor Cyan
curl http://localhost:8080/health

# Test root endpoint
Write-Host "`n2. Root Endpoint:" -ForegroundColor Cyan
curl http://localhost:8080/

# Test swagger
Write-Host "`n3. Swagger available at:" -ForegroundColor Cyan
Write-Host "http://localhost:8080/swagger" -ForegroundColor White

# Test API endpoint
Write-Host "`n4. Testing API endpoint:" -ForegroundColor Cyan
curl http://localhost:8080/api/addresses

Write-Host "`n`nIf all tests passed, API is ready!" -ForegroundColor Green
Write-Host "Access Swagger UI: http://localhost:8080/swagger" -ForegroundColor White