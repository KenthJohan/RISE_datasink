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


## Install
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