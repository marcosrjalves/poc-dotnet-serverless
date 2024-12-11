#!/bin/bash
if [ -f /etc/debian_version ]
then
  apt -qq update
  apt -qq -y install zip
fi

dotnet restore
dotnet lambda package --framework net6.0 --output-package bin/release/net6.0/hello.zip
