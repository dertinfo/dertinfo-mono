import { Injectable } from '@angular/core';
import { ApplicationInsights } from '@microsoft/applicationinsights-web';
import { ConfigurationService } from './configuration.service';

@Injectable({ providedIn: 'root' })
export class AppInsightsService {
  insightsEnabled: boolean = false;
  appInsights: ApplicationInsights;

  constructor(private configurationService: ConfigurationService) {
  }

  logPageView(name?: string, url?: string) { // option to call manually
    if (!this.insightsEnabled) { return; }

    this.appInsights.trackPageView({
      name: name,
      uri: url
    });
  }

  logEvent(name: string, properties?: { [key: string]: any }) {
    if (!this.insightsEnabled) { return; }

    this.appInsights.trackEvent({ name: name }, properties);
  }

  logMetric(name: string, average: number, properties?: { [key: string]: any }) {
    if (!this.insightsEnabled) { return; }

    this.appInsights.trackMetric({ name: name, average: average }, properties);
  }

  logException(exception: Error, severityLevel?: number) {
    if (!this.insightsEnabled) { return; }

    this.appInsights.trackException({ exception: exception, severityLevel: severityLevel });
  }

  logTrace(message: string, properties?: { [key: string]: any }) {
    if (!this.insightsEnabled) { return; }

    this.appInsights.trackTrace({ message: message }, properties);
  }

  public initialiseInsights() {
    if (this.configurationService.appInsightsTelemetryKey != null && this.configurationService.appInsightsTelemetryKey !== '') {
      this.insightsEnabled = true;

      this.appInsights = new ApplicationInsights({
        config: {
          instrumentationKey: this.configurationService.appInsightsTelemetryKey,
          enableAutoRouteTracking: true // option to log all route changes
        }
      });
      this.appInsights.loadAppInsights();
    }
  }
}
