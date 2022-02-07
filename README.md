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
Project consists of two solutions:
- ServicesDAO
- ServicesDAO_VotingEngine
<br>
This repository contains only ServicesDAO. <br>
<br>
VotingEngine and ReputationService microservices and their database instances should be entegrated to the project environment (daonetwork) to run the project with all its functionality. <br>
<br>
After setting up the environment, you can do the steps below to install and run. <br> 
<br>

1. Under the project directory, open terminal and run: <br>

```shell
docker-compose up –-build
```
<br>
The command will create:
<br>

- dao_webportal
- dao_identityservice 
- dao_dbservice 
- dao_logservice 
- dao_notificationservice
- dao_apigateway 
<br><br>

A local network is created by docker-compose (dao_network) and above microservices communicate with each other within that network. <br><br>

To project to be work with full functionality, VotingEngine and ReputationService microservices and their database instances should be entegrated. <br><br>

2. Go to the parent directory and clone the repository with the following command. <br>

```shell
git clone https://github.com/EKON-YAZILIM/ServicesDAO_VotingEngine
```
<br>

Enter the ServicesDAO_VotingEngine project file. In the terminal ``` code .``` when you type, the project files will open. <br><br>

The docker-compose.yml file in the ServicesDAO_VotingEngine project needs to be edited. <br><br>

- In order to avoid conflicts with Rabbitmq, some parts of the yml file should be removed. Parts that need to be commented out to exclude from the project: <br>

```shell
# dao_rabbitmq:
  #   image: rabbitmq:3-management
  #   container_name: 'dao_rabbitmq'
  #   environment:
  #       RABBITMQ_DEFAULT_USER: "daorabbit"
  #       RABBITMQ_DEFAULT_PASS: "dao2021*"
  #   ports:
  #       - 5673:5673
  #       - 5672:5672
  #       - 15672:15672
  #   volumes:
  #       - ~/.docker-conf/rabbitmq/data/:/var/lib/rabbitmq/
  #       - ~/.docker-conf/rabbitmq/log/:/var/lib/log
  #   healthcheck:
  #       test: rabbitmq-diagnostics -q status
  #       interval: 10s
  #       timeout: 30s
  #       retries: 15
    
  #   networks:
  #       - daonetwork
```

<br>
- At the same time, to try to perform RabbitMQ health checks: <br><br>

```shell
dao_votingengine:
    image: ${DOCKER_REGISTRY-}daovotingengine
    platform: linux/x86_64
    build:
      context: .
      dockerfile: DAO_VotingEngine/Dockerfile
    # depends_on:      
    #   dao_rabbitmq:
    #     condition: service_healthy
    restart: always
    networks:
      - daonetwork

dao_reputationservice:
    image: ${DOCKER_REGISTRY-}daoreputationservice
    platform: linux/x86_64
    build:
      context: .
      dockerfile: DAO_ReputationService/Dockerfile
    # depends_on:      
    #   dao_rabbitmq:
    #     condition: service_healthy
    restart: always
    networks:
      - daonetwork

```
<br>
Under ServicesDAO_VotingEngine project directory, open terminal and run: <br>

```shell
docker-compose up –-build
```
<br>
After docker-compose is up, you can access the application from the below link. 
<br><br>
dao_webportal - http://localhost:8895

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
-	To log in to the DAO, you must first sign up. <br>
-	Press the “Sign Up” button and fill in the information requested.<br>
-	Check the box “I accept the terms and conditions” and click the sign up button.<br>
-	Afterwards, an activation e-mail to sent your e-mail address.<br>
-	In order to receive this activation e-mail, you must enter the valid SMTP information in the required place in the appsettings.json file.<br>
-	If you have entered this SMTP information, you will receive an activation e-mail.<br>
-	Click on the link specified in the activation e-mail. <br>
-	You will be redirected to the application url using your default browser. <br>
-	Once the page appeared, you should see a toaster indicating that the activation was successful.<br>
-	This is required to create a user because the registration process is completed with the activation mail.<br>
-	To activate a user without email registration, the IsActive and KYCStatus columns in the daodb.Users table in the database should be set to 1 manually.<br>

### Dashboard Page
-	On this page, you can see the amount of available reputation, the number of jobs you have created, the amount of auctions and votes you have given. <br>
-	The last comments given to the jobs are displayed on this page. <br>
-	You can follow the stages of the jobs you have created and the jobs you have done. <br>

### All Jobs Page
-	It is the page where all jobs are displayed. <br>
-	You can also add a job yourself from the “Post New Job” button at the top right. (The process of adding a job is explained in detail under the heading “My Jobs”).<br>
-	You can search according to the title of the job from the search button at the top.<br>
-	You can filter according to the status of the jobs from the selectbox at the top.<br>
-	If you click on the job, you can go to the detail page of the job.<br>

### My Jobs Page
-	If you want to add a job, you click on the “Post New Job” button. <br>
-	There are some rules for adding jobs. You have to add jobs by following these rules. <br>
-	After reading the rules, you can follow the steps below to add jobs. <br>
	1- Need to define a title for the job.<br>
	2- Set a price for the job in Euros.<br>
	3- Set a timeframe for the job to be completed in days.<br>
	4- Determine the relevant coding languages and tags.<br>
	5- Add the repository address or the code file where the job will be done.<br>
	6- Need to add a detailed description of the job.<br>
-	After filling this information, you will press the “Submit Job” button to add a job.<br>
-	You will see 2 conditions. If you accept these, you can create a job by saying "continue".<br>
-	After creating the job, you can make edits again if you want.<br>
-	By clicking “Comment”, you can both comment and access previous comments. And you can reply to other comments.<br>
-	It must be approved by the admin in order to perform the transactions related to the created jobs.<br>
-	Otherwise, bids cannot be given for jobs.<br>
-	The label of the jobs not approved by the admin is “Pending”. <br><br>

_If you are an Associate, you must pay a fee to be able to job._ <br>
_If you are a VotingAssociate, you can directly recruit._ <br>

### Auction Page
-	**Active Internal Auctions:**
	-	Only VotingAssociates can bid.
	-	And you cannot bid on your own affairs.
	-	You can see and approve bids for the job you have created from the "Show Bids" button.
	-	If the job owner approves the bid, that person starts doing the job.
	-	There is a certain time. If no bid is given by the VotingAssociates within this period, this job falls to "Active Public Auctions". <br>
-	**Active Public Auctions:**
	-	Associates can bid for jobs that are not bid by VotingAssociates.<br>


### Approval of Job:
-	After the job owner approves one of the bids, the approved person starts to do the job. <br>
-	After the job is done, the person doing the job should press the “Show Informal Voting” button on the “Job Details” page of the job. In this way, the job to be voted on. <br>
-	Voting process details are also under the “Voting Page” heading. <br>


### Voting Page
-	**Active Informal Votes:**
	-	The job done comes here and is voted here only by VotingAssociates.
	-	The person cannot vote for his/her own job. 
	-	When voting, you must choose one of the two options and enter the reputation amount.
	-	If the votes here exceed the quorum, they can move to the “Active Formal Votes” section.
-	**Active Formal Votes:**
	-	Everyone can vote here.
	-	The job is approved or rejected according to the votes cast.

### Reputation History
-	On this page, you can find all the details about your reputation. <br>

### Payment History
-	On this page, you can find all the details about the payments.


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
After configuring the database, run the following commands from the test project directory. <br>

For DAO_DbService:
```shell
\PathToSolution\UnitTests\DAO_DbService.Test\ dotnet test
```
For DAO_IdentityService:
```shell
\PathToSolution\UnitTests\DAO_IdentityService.Test\ dotnet test
```
 
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


