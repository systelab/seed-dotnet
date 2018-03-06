# Docker

To create the container and build the application, execute the command:

```bash
docker build -t main
```

To run the application, execute the command:

```bash
docker run -it -p 13080:13080 main
```

You can create both containers with a docker-compose.yml file and the command docker-compose.

Here's a sample docker-compose.yml file:

```yaml
version: "2"
services:
  web:
    image: systelab/seed-dotnet:latest
    environment:
    ports:
      - "13080:13080"
```

In order to run this configuration use the command:

```bash
docker-compose up -d
```

Once started, browse http://localhost:13080/swagger/ in order to get the API main page

> [Diagramr](http://diagramr.inventage.com/) could help you in order to understand the docker-compose files.
