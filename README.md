# Step 1. Setup User secrets
Add user secrets for each microservice in the .Web use this template, make sure to add for all 3 microservices:
```
{
   "DbUsername" : "YourUserName",
   "DbPassword" : "YourPassword"
}
```

# Step 2. Run program once to setup Databases. 
Run all microservices
This will create the databases and all tables needed. 

Open terminal and navigate to MSTemplate For mac Users:
```./start_mac.sh ```
or for Windows: This is untested, if it dosnt work manually run each services using ```Dotnet run``` in the .Web folder
```./start_win.sh ```

# Step 3. Setup Saga table.
* Use this to create Saga Table: [Saga Table](Create_Saga_table.sql)
* The program will automatically create the 3 databases and all other tables.


# Step 4. Run the Services.

Open terminal and navigate to MSTemplate For mac Users:
```./start_mac.sh ```
or for Windows:
```./start_win.sh ```


#Below is for future implementation...
-------------------------------------------------------------------------------------------------------
# SqlServer

To set up a SqlServer Docker container using same volume mount location as the other Docker containers described in this page, run the following command, keeping in mind that if you already have an SQL Server running on your machine, then you need to use another port number (e.g. 1439) for the Docker container:

```sh
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=MyPass@word" -e "MSSQL_PID=Express" -p 1433:1433 -d --name=sql --restart always --mount source=sql-data,target=/var/opt/mssql/data -u root mcr.microsoft.com/mssql/server:2019-latest
```

Now you have an SqlServer running on `localhost,1433`

# Neo4j
To setup a Neo4j Docker container run the following command:

    docker run -d --name neo4j --restart always --publish=7474:7474 --publish=7687:7687 --network dev-bridge --mount source=neo4j-data,target=/data --env=NEO4J_AUTH=none neo4j:latest

This will create and run a Docker container named "neo4j", using latest version, set to automatically startup when Docker starts, using the dev-bridge network, creating/using a named volume for persistent data, and exposing management user interface on http://localhost:7474 with authentication disabled. If you want to enable authentication, remove the `NEO4J_AUTH` environment parameter, and default login will be **neo4j/neo4j**.

All Docker containers in the development setup should normally use the same Docker network, so that they can all "see" each other.

To set up the dev network run the following command:

    docker network create dev-bridge


# RabbitMQ
**This is setup by Boardingpass, only included here if you want/need to do it manually**

To set up a RabbitMQ Docker container run the following command:

    docker run -d --hostname rabbitmq-sbus --name rabbitmq-sbus --restart always -p 5672:5672 -p 8181:15672 --network dev-bridge --mount source=rabbitmq-sbus-data,target=/var/lib/rabbitmq rabbitmq:3-management-alpine

This will create and run a Docker container named "rabbitmq-sbus", using latest version of major version 3 of RabbitMQ (with management user interface included), set to automatically startup when Docker starts, having host name "rabbitmq-sbus", exposing management user interface on http://localhost:8181 with default login **guest/guest**, using the dev-bridge network, and creating/using a named volume for persistent data.

# MongoDB
**This is setup by Boardingpass, only included here if you want/need to do it manually**

To set up a MongoDB Docker container run the following command:

_As "direct node" - the default option in most cases_

    docker run -d --name mongodb --restart always -p 27017:27017 --network dev-bridge --mount source=mongodb-data,target=/data/db --mount source=mongodb-data-configdb,target=/data/configdb mongo:latest

_As "single node replica set", which is relevant in case the service needs to support transactions. ~~Note that connections needs to go to 127.0.0.1, the localhost alias is not supported.~~_

    docker run -d --name mongodb --restart always -p 27017:27017 --network dev-bridge --mount source=mongodb-data,target=/data/db --mount source=mongodb-data-configdb,target=/data/configdb mongo:latest bash -c "set -m ; mongod --bind_ip_all --replSet rs0 & while ! 2> /dev/null > '/dev/tcp/0.0.0.0/27017'; do sleep 1; done ; mongosh --eval 'rs.initiate()' ; fg 1" 

This will create and run a Docker container named "mongodb", using latest version, set to automatically startup when Docker starts, using the dev-bridge network, and creating/using named volumes for persistent data.

For administering your MongoDB databases there are different management tools. The official tool is "MongoDB Compass", which can be downloaded here: https://www.mongodb.com/products/compass.

Connecting MongoDB Compass to your MongoDB database, use this connection string: `mongodb://localhost:27017`
If you installed MongoDB as "single node replica set", then you need to open "Advanced Connection Options" and tick "Direct Connection" checkmark. If error: getaddrinfo ENOTFOUND "dsa2135321esafdagsa" include ?directConnection=true in connection string ```mongodb://localhost:27017/?directConnection=true```
