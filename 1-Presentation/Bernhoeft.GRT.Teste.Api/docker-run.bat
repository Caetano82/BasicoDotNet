@echo off
echo ========================================
echo   Building and Running API Docker
echo ========================================
echo.

echo Building Docker image...
docker-compose build

if %errorlevel% neq 0 (
    echo.
    echo ERROR: Failed to build Docker image
    pause
    exit /b 1
)

echo.
echo Starting container...
docker-compose up -d

if %errorlevel% neq 0 (
    echo.
    echo ERROR: Failed to start container
    pause
    exit /b 1
)

echo.
echo ========================================
echo   API is running!
echo   HTTP: http://localhost:5000
echo   HTTPS: https://localhost:5001
echo   Swagger: http://localhost:5000/
echo ========================================
echo.
echo To view logs: docker-compose logs -f
echo To stop: docker-compose down
echo.
pause

