#!/bin/bash
set -e

export PATH="$PATH:/opt/mssql-tools18/bin:/opt/mssql-tools/bin"

echo "[FCG] Iniciando SQL Server..."
/opt/mssql/bin/sqlservr &
SQLSERVER_PID=$!

echo "[FCG] Aguardando SQL Server ficar disponivel..."
RETRIES=30
for i in $(seq 1 $RETRIES); do
    if sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -Q "SELECT 1" -C -l 1 &>/dev/null 2>&1; then
        echo "[FCG] SQL Server esta pronto!"
        break
    fi
    if [ $i -eq $RETRIES ]; then
        echo "[FCG] ERRO: SQL Server nao ficou disponivel apos ${RETRIES} tentativas."
        kill $SQLSERVER_PID
        exit 1
    fi
    echo "[FCG] Tentativa $i/$RETRIES - aguardando..."
    sleep 2
done

DB_EXISTS=$(sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.databases WHERE name = 'FCG'" -h -1 -C 2>/dev/null | tr -d ' \r\n')

if [ "$DB_EXISTS" = "0" ]; then
    echo "[FCG] Criando banco FCG e inserindo dados de exemplo..."
    sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -i /scripts/FCG.sql -C -b
    echo "[FCG] Banco FCG inicializado com sucesso!"
else
    echo "[FCG] Banco FCG ja existe. Pulando inicializacao."
fi

wait $SQLSERVER_PID
