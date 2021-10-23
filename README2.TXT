Getting Started
------------------------------------------------------------------------------------------------------------------
Make sure you have installed and configured docker in your environment.
After that, you can run the below commands from the /ServicesDAO/ directory.
docker-compose up --build


All applications can be built and run in their own docker containers with the following commands;



All application can be built individually by running the commands below;
dotnet build ./DAO_ApiGateway/DAO_ApiGateway.csproj
dotnet build ./DAO_DbService/DAO_DbService.csproj
dotnet build ./DAO_IdentityService/DAO_IdentityService.csproj
dotnet build ./DAO_LogService/DAO_LogService.csproj
dotnet build ./DAO_NotificationService/DAO_NotificationService.csproj
dotnet build ./DAO_WebPortal/DAO_WebPortal.csproj

Endpoints :-
Direct web service endpoints

dao_db - http://localhost:3309
dao_logsdb - http://localhost:3310
dao_rabbitmq - http://localhost:15672

dao_dbservice - https://localhost:9990
dao_identityservice - https://localhost:9991
dao_logservice - https://localhost:9992
dao_notificationservice - https://localhost:9993
dao_webportal - https://localhost:9994
dao_apigateway - https://localhost:9995

dao_db:
MySQL
Main database of the application.

dao_logsdb:
MySQL
Log database of exceptions and operations of the application

dao_rabbitmq:
RabbitMQ
Serves as bus service, responsible for transmitting logs and notifications from every microservice to dao_logservice and dao_notificationservice.
