# Base Stage
FROM node:lts AS base

# Install global dependencies and clean npm cache in a single RUN command
RUN npm install -g @azure/static-web-apps-cli \
    azure-functions-core-tools@4 --unsafe-perm true

# Builder Stage
FROM base AS builder

# Set the working directory
WORKDIR /build

# Install global dependencies and clean npm cache in a single RUN command
RUN npm install -g @angular/cli@14 typescript

# Copy the static web app files
COPY ./src/client/package.json /build/client/package.json
COPY ./src/functions/package.json /build/functions/package.json
COPY ./src/client/package-lock.json /build/client/package-lock.json
COPY ./src/functions/package-lock.json /build/functions/package-lock.json

# Install packages for the two parts of the app
RUN cd /build/client && npm install --force && \
    cd /build/functions && npm install --force

# Copy the rest of the files
COPY ./src /build

# Build the Angular Client
RUN cd /build/client && ng build

# Build the Function App
# RUN cd /build/functions && npm run build 

# Final Stage
FROM base AS final

# Set the working directory
WORKDIR /app

# Copy only the necessary files from the builder stage
COPY --from=builder /build/client /app/client 
COPY --from=builder /build/functions /app/functions
# note - on the client just copying the dist folder causes us routing errors. 
# todo - We need to come back to this to see if we can reduce the image size. 
#      - moving on as we need to get this opensourced to get some help.  

# Clean up unnecessary files
RUN rm -rf /var/lib/apt/lists/* /tmp/* /var/tmp/*
# note - commented out to see how much we save in the image size without this step 

# Expose the port for the SWA client
EXPOSE 4280

# Start the SWA CLI to serve the static web app on running the container
CMD ["swa", "start", "/app/client/dist", "--api-location", "/app/functions"]