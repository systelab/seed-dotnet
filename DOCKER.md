# Docker

To manually build the image, execute the command:

```bash
docker build -t systelab/seed-dotnet . 
```

To run the application, execute the command:

```bash
docker run -p 13080:13080 systelab/seed-dotnet
```
Once started, browse http://localhost:13080/swagger/ in order to get the API main page
