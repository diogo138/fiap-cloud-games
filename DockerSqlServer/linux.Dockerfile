FROM mcr.microsoft.com/mssql/server:2022-latest

USER root

RUN mkdir -p /scripts && chown mssql:mssql /scripts

COPY --chown=mssql:mssql Docs/FCG.sql /scripts/FCG.sql
COPY --chown=mssql:mssql DockerSqlServer/entrypoint.sh /scripts/entrypoint.sh

RUN chmod +x /scripts/entrypoint.sh

USER mssql

EXPOSE 1433

ENTRYPOINT ["/scripts/entrypoint.sh"]
