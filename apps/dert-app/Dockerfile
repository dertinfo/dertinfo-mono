##################################################################
# Docker File For DertInfo App
# Author: David Hall
# Optimised / Hardend: No
# Last Chnage: 
# - Initial Creation - David Hall - 2024/08/29
# Notes:
# - This file produces a very large image and should be optimised 
#   and hardened. The purpose of this file is to allow other artifacts 
#   in the dertinfo estate to be have a running version of the web app.
# - This file is not intended for production use as the cloud native SWA 
#   is a better option.
# - It is layered to help build the image faster. However these layers
#   do not particualrily reduce the size.
##################################################################

# Base Stage
FROM node:lts AS base

# Install global dependencies and clean npm cache in a single RUN command
RUN npm install -g @azure/static-web-apps-cli

# Builder Stage
FROM base AS builder

# Set the working directory
WORKDIR /build

# Install global dependencies and clean npm cache in a single RUN command
RUN npm install -g @angular/cli@14 typescript

# Copy the static web app files
COPY ./src/client/package.json /build/client/package.json
COPY ./src/client/package-lock.json /build/client/package-lock.json

# Install packages for the two parts of the app
RUN cd /build/client && npm install --force

# Copy the rest of the files
COPY ./src /build

# Build the Angular Client
RUN cd /build/client && ng build
# note - we do not pass the environment here as this is only ever a dev container.
#      - on deployment the release pipeline builds the static web app from the code.
#      - the SWA build instuction supplies the flag. 

# Final Stage
FROM base AS final

# Set the working directory
WORKDIR /app

# Copy only the necessary files from the builder stage
COPY --from=builder /build/client /app/client

# note - on the client just copying the dist folder causes us routing errors. 
# todo - We need to come back to this to see if we can reduce the image size. 
#      - moving on as we need to get this opensourced to get some help.  

# Clean up unnecessary files
RUN rm -rf /var/lib/apt/lists/* /tmp/* /var/tmp/*
# note - commented out to see how much we save in the image size without this step 

# Expose the port for the SWA client
EXPOSE 4280

# Start the SWA CLI to serve the static web app on running the container
CMD ["swa", "start", "/app/client/www"]