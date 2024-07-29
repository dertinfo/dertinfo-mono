#!/bin/bash

# Check the Node Version
echo "Checking Node Version"
node -v

# Launch the Angular Application
echo "Creating the Angular Client..."
cd /app/client
ng serve

# Launch the Function App
echo "Creating the Typescipt API"
cd /app/functions
nohup func start --port 7200 &

# Combine the Angular Client and the FuncionApp
echo "Launching the Static Web App On Port 4280"
cd /
swa start http://localhost:4200 --port 4280 --api-port 7200