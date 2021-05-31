#!/bin/bash

sudo pkill -f dotnet
git pull
sudo nohup dotnet run --urls=http://datasink.testbed.se &