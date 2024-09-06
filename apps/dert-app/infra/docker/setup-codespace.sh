# At the location of this shell script, create files for each of the secrets that we want to use. 
# For each of the following environment variables, create a secrets file for reference in docker-compose

# AUTH0_APP_CLIENT_ID - Auth0 client id
# AUTH0_MANAGEMENT_CLIENT_ID - Auth0 management client id
# AUTH0_MANAGEMENT_CLIENT_SECRET - Auth0 management client secret
# AUTH0_WEB_CLIENT_ID - Auth0 web client id
# SENDGRID_API_KEY - SendGrid API key

# Notify the user that the script is running
echo "Starting setting up the configuration & secrets for the codespace."

###################
# Setup for the API
###################

# For each of the secrets coming in from the environment, add it to the api.env file.
echo "PwaClient__Auth0__ClientId=$AUTH0_APP_CLIENT_ID" > infra/docker/api.env # replace what's there
echo "Auth0__ManagementClientId=$AUTH0_MANAGEMENT_CLIENT_ID" >> infra/docker/api.env # append to the file from now on
echo "Auth0__ManagementClientSecret=REDACTED
echo "WebClient__Auth0__ClientId=$AUTH0_WEB_CLIENT_ID" >> infra/docker/api.env
echo "SendGrid__ApiKey=$SENDGRID_API_KEY" >> infra/docker/api.env

###################
# Setup for the App
###################

echo "API_URL=https://$CODESPACE_NAME-44100.app.github.dev" > infra/docker/app.env
echo "AUTH_CALLBACK_URL=https://$CODESPACE_NAME-44300.app.github.dev" >> infra/docker/app.env

# Notify the user that the script has completed
echo "Completed setting up the configuration & secrets for the codespace."