# Backend readme

## How to run

### Linux

1. Run command `sudo -i service elasticsearch start`
2. Go to `/meme-search/backend/MemeSearch.API/`
3. Run command `dotnet run`

### Windows

1. idk

## Test connection

`curl -X GET localhost:9200/memes-test/_count`

`curl -X GET "localhost:9200/?pretty"`
