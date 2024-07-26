import { AzureFunction, Context, HttpRequest } from "@azure/functions"
import { AppConfigurationClient, ListConfigurationSettingsOptions } from "@azure/app-configuration"

const httpTrigger: AzureFunction = async function (context: Context, req: HttpRequest): Promise<void> {

    const appConfigurationConnectionString = process.env["AZURE_APP_CONFIG"];
    const environment = process.env["ENVIRONMENT"];

    var responseBody = {};

    if(appConfigurationConnectionString != null && appConfigurationConnectionString.length > 0) {

        const appConfigClient = new AppConfigurationClient(appConfigurationConnectionString)

        // Will retrieve the following settings from app configuration:
        // - WebClient:Api:Uri
        // - WebClient:Auth0:ClientId
        // - WebClient:Auth0:Audience
        // - WebClient:Auth0:CallbackUrl
        // - WebClient:Auth0:Domain
        // - WebClient:AppInsights:TelemetryId

        var configOptions: ListConfigurationSettingsOptions = {
            keyFilter: "WebClient:*",
            labelFilter: environment
        }

        // Call the Azure App Configuration Instance and get the settings.
        var configurationSettings = appConfigClient.listConfigurationSettings(configOptions);
        for await (const setting of configurationSettings) {
            responseBody[setting.key] = setting.value;
        }

    } else {

        // In Development get the required settings from local.settings.json
        responseBody["WebClient:Api:Uri"] = process.env["devonly_apiUrl"];
        responseBody["WebClient:Auth0:ClientId"] = process.env["devonly_auth0ClientId"];
        responseBody["WebClient:Auth0:Audience"] = process.env["devonly_auth0Audience"];
        responseBody["WebClient:Auth0:CallbackUrl"] = process.env["devonly_auth0CallbackUrl"];
        responseBody["WebClient:Auth0:Domain"] = process.env["devonly_auth0TenantDomain"];
        responseBody["WebClient:AppInsights:TelemetryId"] = process.env["devonly_appInsightsTelemetryKey"];
    }

    context.res = {
        // status: 200, /* Defaults to 200 */
        body: responseBody
    };

};

export default httpTrigger;
