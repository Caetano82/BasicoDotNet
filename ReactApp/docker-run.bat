@echo off
echo ========================================
echo   Building and Running React App Docker
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
echo   React App is running!
echo   Access: http://localhost:3000
echo ========================================
echo.
echo To view logs: docker-compose logs -f
echo To stop: docker-compose down
echo.
pause

