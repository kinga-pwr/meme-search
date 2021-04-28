# How to run
## Linux
1. Run command `./bin/elasticsearch` or as deamon `./bin/elasticsearch -d -p pid`
2. Run command `dotnet run`
## Windows
1. idk

# Test connection
`curl -X GET localhost:9200/memes-test/_count`

`curl -X GET "localhost:9200/?pretty"`
