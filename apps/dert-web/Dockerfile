# Use a base image with Node.js
FROM node:18

# Install the SWA CLI
RUN npm install -g @azure/static-web-apps-cli

# Install the Angular CLI
RUN npm install -g @angular/cli

# Install the Azure Functions Core Tools
RUN npm install -g azure-functions-core-tools@4 --unsafe-perm true
# note - We don't need to do this as the swa cli will install the core tools for us.
#        However doing it now improves the run time of the container as the swa will not need to install the core tools.

# Set the working directory
WORKDIR /app

# Copy the static web app files
COPY ./src /app

# Install dependencies for the static web app web client
RUN cd /app/client && npm install

# Build the static web app client that uses Angular
RUN cd /app/client && ng build
# note - we should be using the --prod flag here on the angular build, but it's not working at this time either locally or inside the container.

# Expose the port for the static web app
EXPOSE 4280

# Start the SWA CLI to serve the static web app
CMD ["swa", "start", "/app/client/www", "--api-location", "/app/functions"]