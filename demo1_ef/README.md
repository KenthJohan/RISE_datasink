# Experimental sensor database

## RISE testbed
RISE has its own server called testbed.
Subdomain is used for projects.
This project is under http://datasink.testbed.se

## Used tools
* PostgreSQL TimescaleDB
* C# dotnet aspnetcore
* Javascript for sensor GUI
* Openstreemap for geo tagged sensor data
* plotly for plotting timeseries in realtime


## Installing dotnet
https://docs.microsoft.com/en-us/dotnet/core/install/linux-ubuntu


```bash
wget https://packages.microsoft.com/config/ubuntu/21.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

sudo apt-get update
sudo apt-get install -y apt-transport-https

sudo apt-get update
sudo apt-get install -y dotnet-sdk-5.0

sudo apt-get update
sudo apt-get install -y apt-transport-https

sudo apt-get update
sudo apt-get install -y aspnetcore-runtime-5.0
```



## Installing PostgreSQL
```bash
# Create the file repository configuration:
sudo sh -c 'echo "deb http://apt.postgresql.org/pub/repos/apt $(lsb_release -cs)-pgdg main" > /etc/apt/sources.list.d/pgdg.list'

# Import the repository signing key:
wget --quiet -O - https://www.postgresql.org/media/keys/ACCC4CF8.asc | sudo apt-key add -

# Update the package lists:
sudo apt-get update

# Install the latest version of PostgreSQL.
# If you want a specific version, use 'postgresql-12' or similar instead of 'postgresql':
sudo apt-get -y install postgresql
```





### PostgreSQL start commandline
```bash
sudo -u postgres psql
```

### PostgreSQL add user
```bash
sudo -u postgres createuser -P -s -e datasink
```
```bash
Enter name of role to add:
```

### PostgreSQL create database
```bash
sudo -u postgres createdb datasink
```






