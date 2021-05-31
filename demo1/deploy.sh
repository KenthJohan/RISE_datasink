#!/bin/bash

sudo pkill -f dotnet
git pull
git checkout startpoint
cd backend
sudo nohup dotnet run --urls=http://datasink.testbed.se &