FROM christianacca/mssql-server-windows-express

SHELL [ "powershell" ]

WORKDIR /

EXPOSE 1433

CMD powershell ./start.ps1 -sa_password \"$env:sa_password\" -ACCEPT_EULA \"$env:ACCEPT_EULA\" -attach_dbs '$env:attach_dbs' -Verbose
