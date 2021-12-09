# ServicesDAO

  The ServicesDAO portal provides a platform for the service DAOs which provide services in a decentralized manner through a modular implementation of MVPR.

## Prerequisite
Docker should be installed and configured.

## Install and run
After installing and configuring Docker in your environment, you can run the below command from the /ServicesDAO/ directory for installation and run.<br>
docker-compose up --build<br>
<br>
After docker-compose is up, you can access the application from the below link.<br>
dao_webportal - http://localhost:8895<br>
<br>
Using below links, you can see the status, logs and/or erros of the belonging api<br>
dao_dbservice - http://localhost:9990<br>
dao_identityservice - http://localhost:9991<br>
dao_logservice - http://localhost:9992<br>
dao_notificationservice - http://localhost:9993<br>
dao_apigateway - http://localhost:9995<br>
<br>
dao_db is configured to be accessed from localhost port: 3309 as default<br>
dao_logsdb is configured to be accessed from localhost port: 3310 as default<br>
dao_rabbitmq user interface adress is http://localhost:15672<br>

## Develop
<br>
All applications can be built and run in their own docker containers with the following command;

docker build -f "./Dockerfile" -t [IMAGE_NAME]:dev "[SolutionPath]\ServicesDAO"
docker run -p [application access port]:80 -name:[NAME]  [IMAGE_NAME]:dev --

For the application to work, all containers should be created in the same network also database, rabbitmq and api endpoints can be redefined.<br>

After .NET core 3.1 and .NET SDK 3.1.20 are installed on the environment, all applications can be built individually by running the commands below under the project solution folder;<br>

dotnet build ./DAO_ApiGateway/DAO_ApiGateway.csproj<br>
dotnet build ./DAO_DbService/DAO_DbService.csproj<br>
dotnet build ./DAO_IdentityService/DAO_IdentityService.csproj<br>
dotnet build ./DAO_LogService/DAO_LogService.csproj<br>
dotnet build ./DAO_NotificationService/DAO_NotificationService.csproj<br>
dotnet build ./DAO_WebPortal/DAO_WebPortal.csproj<br>

## Information About Services
dao_db:
MySQL
Main database of the application.

dao_logsdb:
MySQL
Log database of exceptions and operations of the application.

dao_rabbitmq:<br>
RabbitMQ
Serves as bus service, responsible for transmitting logs and notifications from every microservice to dao_logservice and dao_notificationservice.

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
- Registering to the application<br>
- Job Posting
- Voting
- Forum

Helpers Library:<br>
Contains application models, constants, application wide generic methods(MySQL connection, RabbitMQ subscription, Json Serializing, Encryption etc...) of the application.









