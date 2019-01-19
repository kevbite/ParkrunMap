$ErrorActionPreference = "Stop"

New-Item -Type Directory "./downloads" -ErrorAction SilentlyContinue

$mdbVersion = "4.0.5"

[Net.ServicePointManager]::SecurityProtocol = "tls12, tls11, tls"
$url = "https://fastdl.mongodb.org/win32/mongodb-win32-x86_64-2008plus-ssl-$mdbVersion.zip"
$output = "./downloads/mongodb-win32-x86_64-2008plus-ssl.zip"
Invoke-WebRequest -Uri $url -OutFile $output

Expand-Archive "./downloads/mongodb-win32-x86_64-2008plus-ssl.zip" -DestinationPath "./downloads/mongodb-win32-x86_64-2008plus-ssl"

New-Item -Type Directory "/data/db" -ErrorAction SilentlyContinue

Start-Process "./downloads/mongodb-win32-x86_64-2008plus-ssl/mongodb-win32-x86_64-2008plus-ssl-$mdbVersion/bin/mongod.exe"
