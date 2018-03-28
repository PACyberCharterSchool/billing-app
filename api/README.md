# Setting Up Development Environment
## Database

### Build Docker image
Microsoft provides a version of SQL Server that can run on a Docker Linux container - very handy for development efforts.

To stand up the container, create a docker machine using the docker-machine command with the requisite properties, start the docker machine,
attached to the docker machine, and use the provided Dockerfile to pull and build an image provided by the Docker Registry to the docker
machine:

#### Create Docker machine
To run SQL Server Linux, the Docker machine requires a minimum of 2G of RAM.

```
docker-machine create --driver virtualbox --virtualbox-memory "2048" sqlserver
```

#### Build Docker image
Microsoft provides an image for creating a Docker container that will run SQL Server 2017 on a Linux system.  Use the provided Dockerfile to 
build the image.

```
cd <project directory containing the Dockerfile>
docker build -t sqlserver .
```

The -t option provides a tag name, which will be used to run the Docker container.

### Run SQL Server Docker container
In order to stand up the Docker container the first time, the `docker run` command is required.  Note the inclusion of the options to allow port 1401 to
be exposed.

```
docker run -e 'ACCEPT_EULA=Y' -e 'MSSQL_SA_PASSWORD=Br0ken horse carrot' --name sqlserver -d sqlserver -p 1401:1433
```

## Application