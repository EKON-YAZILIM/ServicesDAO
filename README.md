# ServicesDAO

The ServicesDAO portal provides a platform for the service DAOs which provide services in a decentralized manner through a modular implementation of MVPR.
- The application is developed with microservice architecture.

## Prerequisites
To run the application, `Docker` should be installed and configured on your system. Necesseary information is [here](https://docs.docker.com/engine/install/).<br>
<br>
In order to access the database, mysql client is to be installed on your system. Necessary information is [here](https://dev.mysql.com/doc/mysql-shell/8.0/en/mysql-shell-install-linux-quick.html).<br>
Example for Ubuntu:
```shell
sudo apt-get update
sudo apt-get install mysql-client
```
In order to build microservices individually and run the tests from command prompt dotnet sdk 3.1 or higher should be installed on your system. Necessary information is [here](https://docs.microsoft.com/tr-tr/dotnet/core/install/linux-ubuntu).<br>
Example for Ubuntu:
```shell
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update
sudo apt install apt-transport-https
sudo apt install dotnet-sdk-3.1
```

## Install and run
After installing Docker to your environment, you can run the below command under the project directory to install and run.<br>
```shell
docker-compose up --build
```
A local network is created by docker-compose (dao_network) and all microservices communicate with each other within that network.<br>
After docker-compose is up, you can access the application from the below link.<br>
dao_webportal - http://localhost:8895<br>
<br>
Using below links, you can see the status, logs and/or erros of the belonging microservice from localhost.<br>
dao_identityservice - http://localhost:8890<br>
dao_dbservice - http://localhost:8889<br>
dao_logservice - http://localhost:8891<br>
dao_notificationservice - http://localhost:8892<br>
dao_apigateway - http://localhost:8896<br>
<br>
The applciation databases can be accessed from within their own containers or from certain ports to localhost.<br>
Example:
- Access the container
```shell
	> docker exec -it <container_name of the database> bash -l
	/# mysql -u root -p <name of the database>
	> 
```
- Access database from localhost
```shell
> mysql -u <user> -p'<user password>' -h 127.0.0.1 -P 3306(exposed port) -D <database name>
```
For dao_db:
```shell
> mysql -u root -p'<root password>' -h 127.0.0.1 -P 3309 -D daodb
```
Thera are 2 database instances of the application;<br>
- daodb (expose 3309)
- logsdb (expose 3310)


Application logs and notifications are carried/delivered by a RabbitMQ instance - dao_rabbitmq.<br>
dao_rabbitmq user interface adress is http://localhost:15672<br>

## Usage
The test application is live at here[http://3.128.180.61:8895/].

After entering the valid SMTP credentials to appsettings.json file you can complete sign up process with e-mail verification. Also a sign up process can be completed manually by updating Users table in the daodb database.<br>

The voting part of the application is a separate module and is not available in this repostory.<br>
The voting engine and reputation service can be found as a stand-alone project at this link[https://github.com/EKON-YAZILIM/ServicesDAO_VotingEngine.git].<br>

## Develop
All applications can be built and run in their own docker containers with the following command;
```shell
docker build -f "./Dockerfile" -t [IMAGE_NAME]:dev "[SolutionPath]\ServicesDAO"
docker run -p [application access port]:80 -name:[NAME]  [IMAGE_NAME]:dev --
```

For the application to work, all containers should be created in the same network also database, rabbitmq and api endpoints can be redefined.<br>

After .NET core 3.1 and .NET SDK 3.1.20 are installed on the environment, all applications can be built individually by running the commands below under the project solution folder;
```shell
 dotnet build ./DAO_ApiGateway/DAO_ApiGateway.csproj
```

```shell
 dotnet build ./DAO_DbService/DAO_DbService.csproj
```

```shell
 dotnet build ./DAO_IdentityService/DAO_IdentityService.csproj
```

```shell
 dotnet build ./DAO_LogService/DAO_LogService.csproj
```

```shell
 dotnet build ./DAO_NotificationService/DAO_NotificationService.csproj
```

```shell
 dotnet build ./DAO_WebPortal/DAO_WebPortal.csproj
```

## Information About Services
dao_db:<br>
MySQL<br>
Main database of the application.<br>

dao_logsdb:<br>
MySQL<br>
Log database of exceptions and operations of the application.<br>

dao_rabbitmq:<br>
RabbitMQ<br>
Serves as bus service, responsible for transmitting logs and notifications from every microservice to dao_logservice and dao_notificationservice.<br>

dao_dbservice:<br>
Shared data-access layer for application main database.<br>
Accessed by:<br>
Identity Service<br>
Notification Service<br>
Web Portal (Thru DAO_ApiGateway)<br>

dao_logservice:<br>
Data-access layer for dao_logsdb.<br>

dao_notificationservice:<br>
Responsible for sending notifications via email.<br>

dao_webportal:<br>
Provides application and user interactions with user interface.<br>
- Registering to the application
- Job Posting
- Voting
- Forum

Helpers Library:<br>
Contains application models, constants, application wide generic methods(MySQL connection, RabbitMQ subscription, Json Serializing, Encryption etc...) of the application.

## Testing
Application Mysql database instances should be up and running with a testing environment setup.<br>
To run tests from terminal dotnet sdk should be installed on your system.<br>

The easiest and recommended way is pulling a mysql docker image and run in a docker container with minimum parameters.<br>
```shell
docker run --detach --name=test-mysql -p 3309:3306  --env="MYSQL_ROOT_PASSWORD=mypassword" mysql
```
To access the mysql instance in the container:
```shell
docker exec -it test-mysql bash -l
```
To access the database from the mysql container terminal :
```shell
mysql -u root -p
Enter Password: **********
mysql>
```
The root password, the expose port and many other parameters can be changed optionally. The test database connection string should be written under the PlatformSettings section taking place in the \PathToSolution\ServicesDAO\<MicroserviceDirectory>\appsettings.test.json file and rebuild with command dotnet build.
Example:
```json
"PlatformSettings": {
    "DbConnectionString": "Server=localhost;Port=3313;Database=test_votingdb;Uid=root;Pwd=mypassword;",
    ...
}
```
After configuring the database, run the following commands from the test project directory \PathToSolution\UnitTests\.

dotnet test --filter DisplayName~
dotnet test --filter DisplayName~
dotnet test --filter DisplayName~
Code documentation files in format is autogenerated everytime the project is build under bin folder.

# HTTPS Configuration
The process below is explained for dao_webportal and is identical for every microservice included in this repository.
To enable HTTPS, https settings of the application should be added to the Docker Container configuration and the an ssl certificate file should be introduced if necessary.

Example of enabling HTTPS using 'docker-compose.override.yml' file:

dao_webportal:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=https://+;http://+:80
      - ASPNETCORE_HTTPS_PORT=443
https://+ is added to ASPNETCORE_URLS and 443 port is defined as https port with adding ASPNETCORE_HTTPS_PORT=443

An ssl certificate can be generated and placed in a location on the machine where the docker container is running.
One way to generate an SSL certificate is explained here.

The definition of the generated ssl certificate in the docker compose file is as follows:
```yml
dao_webportal:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=https://+;http://+:80
      - ASPNETCORE_HTTPS_PORT=443
#Password for the certificate
      - ASPNETCORE_Kestrel__Certificates__Default__Password=< password of the generated certificate >
#Path of the certificate file
      - ASPNETCORE_Kestrel__Certificates__Default__Path= < location of the ssl certificate in docker container. Example: '/https/aspnetapp.pfx' > 
    volumes:
#Mount the local volume where the certificate exists to docker container
      - < location of the ssl certificate in the host machine> : < location of the ssl certificate in docker container. Example: '~/.aspnet/https:/https:ro'>
```      


