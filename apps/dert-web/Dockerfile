# Use a base image with Node.js
FROM node:18-alpine

# Install global dependencies that we need to build the Static Web App
# Include: Static Web Apps CLI, Angular CLI, Typescript, Azure Functions Core Tools
RUN npm install -g @azure/static-web-apps-cli @angular/cli typescript azure-functions-core-tools@4 && npm cache clean --force
# note - the swa cli will install azure-functions-core-tools automatically but we can speed up the build by explictly installing it here.
#      - we clean the npm cache to reduce the size of the image
#      - we combine the npm install commands to reduce the number of layers in the image

# Set the working directory
WORKDIR /app

# Copy the static web app files
COPY ./src /app

# Install packages for the two parts of the app
RUN cd /app/client && npm install --force
RUN cd /app/functions && npm install --force
# note - we combine the npm install commands to reduce the number of layers in the image

# Build the Function App
RUN cd /app/functions && npm run build

# Build the SWA client (Angular)
RUN cd /app/client && ng build
# note - we should be using the --prod flag here on the angular build, but it's not working at this time either locally or inside the container.

# Clean up unnecessary files
RUN rm -rf /var/lib/apt/lists/* /tmp/* /var/tmp/*
# note - commented out to see how much we save in the image size without this step 

# Expose the port for the SWA client
EXPOSE 4280

# Start the SWA CLI to serve the static web app on running the container
CMD ["swa", "start", "/app/client/dist", "--api-location", "/app/functions"]