# Use a base image with Node.js
FROM node:lts

# Install global dependencies that we need to build the Static Web App
# Include: Static Web Apps CLI, Angular CLI, Typescript, Azure Functions Core Tools
RUN npm install -g @azure/static-web-apps-cli
RUN npm install -g @angular/cli@13
RUN npm install -g typescript 
RUN npm install -g azure-functions-core-tools@4 --unsafe-perm true
# note - the swa cli will install azure-functions-core-tools automatically but we can speed up the build by explictly installing it here.
#      - we clean the npm cache to reduce the size of the image
#      - we combine the npm install commands to reduce the number of layers in the image

# Set the working directory
WORKDIR /app

# Copy the static web app files
COPY ./src/client/package.json /app/client/package.json
COPY ./src/functions/package.json /app/functions/package.json

# Install packages for the two parts of the app
RUN cd /app/client && npm install --force
RUN cd /app/functions && npm install --force
# note - we combine the npm install commands to reduce the number of layers in the image

# Copy the rest of the files
RUN ls -al
COPY ./src /app

# Build the Angular Client
RUN cd /app/client && npm install --force
RUN cd /app/client && ng build
# note - we should be using the --prod flag here on the angular build, but it's not working at this time either locally or inside the container.

# Build the Function App
RUN cd /app/functions && npm install && npm run build

# Clean up unnecessary files
RUN rm -rf /var/lib/apt/lists/* /tmp/* /var/tmp/*
# note - commented out to see how much we save in the image size without this step 

# Expose the port for the SWA client
EXPOSE 4280

# Start the SWA CLI to serve the static web app on running the container
CMD ["swa", "start", "/app/client/dist", "--api-location", "/app/functions"]