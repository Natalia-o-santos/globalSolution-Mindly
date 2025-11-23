#!/bin/bash

# Script de setup para Mindly API
# Aplica migrations e prepara o ambiente

echo "🚀 Configurando Mindly API..."

cd "$(dirname "$0")/src/Mindly.Api" || exit 1

echo "📦 Restaurando dependências..."
dotnet restore

echo "🗄️ Aplicando migrations..."
dotnet ef database update

echo "✅ Setup concluído!"
echo ""
echo "Para executar a API, use:"
echo "  cd src/Mindly.Api"
echo "  dotnet run"
echo ""
echo "Ou acesse o Swagger em: https://localhost:5001/swagger"

