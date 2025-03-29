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
  }
}
