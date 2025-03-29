import { Injectable } from "@angular/core";
import { MsalService } from "@azure/msal-angular";
import { HttpClient } from "@angular/common/http";

@Injectable({
  providedIn: "root",
})
export class WeatherForecastService {
  constructor(private authService: MsalService, private http: HttpClient) {}

  getTokenAndCallApi() {
    const account = this.authService.instance.getActiveAccount();
    console.log(account);
    console.log(this.authService.instance.getConfiguration());
     if (account) {
      this.authService
        .acquireTokenSilent({
          account,
          scopes: ['api://da690e01-d4e2-4d14-b633-ded29a7e7a7d/Forecast.Read'], // Replace with the required scopes for your API
        })
        .subscribe({
          next: (response) => {
            const token = response.accessToken;
            console.log(token);
            // Use the token to call your API
            this.http
              .get("https://localhost:8081/weatherforecast", {
                headers: { Authorization: `Bearer ${token}` },
              })
              .subscribe({
                next: (data) => {
                  console.log(data);
                },
                error: (httpError) => {
                  console.error("API call failed", httpError);
                },
              });
          },
          error: (tokenError) => {
            console.error("Token acquisition failed", tokenError);
            this.authService.acquireTokenRedirect({
              scopes: ["api://da690e01-d4e2-4d14-b633-ded29a7e7a7d/Forecast.Read"], // Replace with the required scopes for your API
            }).subscribe({
              next: (data) => console.log("success redirect"),
              error: (error) => console.log(error)
            })
          },
        });
    }
  }
}
