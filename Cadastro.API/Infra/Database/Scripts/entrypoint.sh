#!/bin/bash

echo "Aguardando SQL Server inicializar..."

for i in {1..30}; do
    /opt/mssql-tools18/bin/sqlcmd -S sqlserver -U sa -P "Senha@1234" -C -Q "SELECT 1" > /dev/null 2>&1
    if [ $? -eq 0 ]; then
        echo "SQL Server pronto!"
        break
    fi
    echo "Tentativa $i - aguardando..."
    sleep 5
done

/opt/mssql-tools18/bin/sqlcmd -S sqlserver -U sa -P "Senha@1234" -C -i /scripts/Cliente.sql

echo "Script executado com sucesso!"
