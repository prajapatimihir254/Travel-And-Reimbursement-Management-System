# PowerShell script to setup the BizTravel database
# Run this script from the project root directory

Write-Host "Setting up BizTravel Database..." -ForegroundColor Green

# Navigate to the BizTravel project directory
Set-Location "BizTravel"

# Update the database using Entity Framework migrations
Write-Host "Running Entity Framework database update..." -ForegroundColor Yellow
dotnet ef database update

if ($LASTEXITCODE -eq 0) {
    Write-Host "Database setup completed successfully!" -ForegroundColor Green
    Write-Host "You can now run the application with: dotnet run" -ForegroundColor Cyan
} else {
    Write-Host "Database setup failed. Please check the error messages above." -ForegroundColor Red
}

# Navigate back to the root directory
Set-Location ".."