@echo off
REM =============================================================
REM coverage-report.bat (sem delayed expansion) - gera:
REM1) TestResults\coverage\coverage.cobertura.xml
REM2) TestResults\CoverageReport\index.html
REM Usa somente --collect:"XPlat Code Coverage" (coverlet.collector).
REM Se aparecer caracteres '´╗┐' ao executar este arquivo, salve-o sem BOM (UTF-8 sem BOM / ANSI).
REM NOTA: Os arquivos .sh (cover.sh e coverage-report.sh) sao para Linux/Mac e NAO sao usados por este .bat
REM =============================================================

set "TEST_PROJECT=0-Tests\Bernhoeft.GRT.Teste.IntegrationTests\Bernhoeft.GRT.Teste.IntegrationTests.csproj"
set "RESULTS_DIR=TestResults"
set "XML_OUT_DIR=%RESULTS_DIR%\coverage"
set "XML_FILE=%XML_OUT_DIR%\coverage.cobertura.xml"
set "HTML_OUT_DIR=%RESULTS_DIR%\CoverageReport"

where reportgenerator >nul2>&1 || dotnet tool install --global dotnet-reportgenerator-globaltool >nul

REM Limpa diretorios antigos
if exist "%XML_OUT_DIR%" rmdir /s /q "%XML_OUT_DIR%"
if exist "%HTML_OUT_DIR%" rmdir /s /q "%HTML_OUT_DIR%"

echo Executando testes com cobertura...
REM IMPORTANTE: nao usar --no-build para permitir instrumentacao completa
dotnet test "%TEST_PROJECT%" -c Debug --collect:"XPlat Code Coverage" --results-directory "%RESULTS_DIR%" --verbosity minimal

echo Localizando arquivos coverage.cobertura.xml...
set "FOUND_LIST="
for /f "delims=" %%F in ('dir /b /s "%RESULTS_DIR%\coverage.cobertura.xml"2^>nul') do (
 if not defined FOUND_LIST (set "FOUND_LIST=%%F") else (set "FOUND_LIST=%FOUND_LIST%;%%F")
)

if not defined FOUND_LIST (
 echo ERRO: Nenhum arquivo coverage.cobertura.xml encontrado.
 echo Verifique se o pacote coverlet.collector esta no csproj de testes.
 goto :END
)

echo Arquivos encontrados:
for %%I in (%FOUND_LIST%) do echo - %%I

REM Extrai o primeiro arquivo da lista
set "FIRST_FILE="
for /f "tokens=1 delims=;" %%F in ("%FOUND_LIST%") do (
 if not defined FIRST_FILE set "FIRST_FILE=%%F"
)

REM Cria a pasta apenas se houver arquivos para copiar
if not exist "%XML_OUT_DIR%" mkdir "%XML_OUT_DIR%"

REM Copia o primeiro arquivo encontrado para a pasta de saida
copy /y "%FIRST_FILE%" "%XML_FILE%" >nul

if not exist "%XML_FILE%" (
 echo ERRO ao copiar arquivo principal para %XML_FILE%
 if exist "%XML_OUT_DIR%" rmdir /s /q "%XML_OUT_DIR%"
 goto :END
)

echo Gerando HTML...
reportgenerator -reports:"%FOUND_LIST%" -targetdir:"%HTML_OUT_DIR%" -reporttypes:HtmlSummary;Html >nul
if errorlevel1 (
 echo ERRO ao gerar HTML.
 REM Remove pasta coverage se nao foi gerado o HTML
 if exist "%XML_OUT_DIR%" rmdir /s /q "%XML_OUT_DIR%"
 goto :END
)

echo Cobertura XML: %XML_FILE%
echo Relatorio HTML: %HTML_OUT_DIR%\index.html

REM Verifica se o arquivo XML existe, caso contrario remove a pasta vazia
if not exist "%XML_FILE%" (
 if exist "%XML_OUT_DIR%" rmdir /s /q "%XML_OUT_DIR%"
)

echo Concluido.
:END
pause