# base image
FROM microsoft/mssql-server-linux

# Dockerfile author/maintainer
MAINTAINER William Moore <bill@riversagile.com>

# Update application repository list
RUN apt-get update

# Expose the default SQL Server port
EXPOSE 1401

# Set the default command
ENTRYPOINT ["/bin/sh", "-c", "/opt/mssql/bin/sqlservr"]