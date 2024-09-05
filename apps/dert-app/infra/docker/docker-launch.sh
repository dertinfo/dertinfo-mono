#!/bin/bash

# ######################################################
# We use a custom start script for this docker container
# as we need to update the location of the API and the 
# callback url for the auth thatb are set on envionment 
# variables passed to the container. 
# ######################################################

# Pickup the environment variable values of API_URL and AUTH_CALLBACK_URL and put them into variables
API_URL=${API_URL}
AUTH_CALLBACK_URL=${AUTH_CALLBACK_URL}
DEFAULT_API_URL=${DEFAULT_API_URL}
DEFAULT_AUTH_CALLBACK_URL=${DEFAULT_AUTH_CALLBACK_URL}

# Output the file stucture
ls -l ./

# Get a reference to the configuration file "app.dev.config.json" at the path src/assets
CONFIG_FILE="./dist/assets/app.dev.config.json"

# Replace the values of the PLACEHOLDER_FOR_API_URL with the environment variable value of API_URL
sed -i "s|${DEFAULT_API_URL}|${API_URL}|g" $CONFIG_FILE

# Replace the values of the PLACEHOLDER_FOR_AUTH_CALLBACK_URL with the environment variable value of AUTH_CALLBACK_URL
sed -i "s|${DEFAULT_AUTH_CALLBACK_URL}|${AUTH_CALLBACK_URL}|g" $CONFIG_FILE

# Write the contents of the configuration file to the console
cat $CONFIG_FILE

# Start the docker container
swa start ./dist
