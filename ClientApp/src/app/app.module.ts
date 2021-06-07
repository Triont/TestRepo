import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';

import {AppComponent} from './app.component';
import { AgChartsAngularModule } from "ag-charts-angular";
//wimport { HttpClient } from '@angular/common/http'
import { HttpClientModule } from '@angular/common/http';

@NgModule({
    declarations: [
        AppComponent
    ],
    imports: [
        BrowserModule,
      AgChartsAngularModule,
      HttpClientModule,
    ],
  providers: [],
    bootstrap: [AppComponent]
})

export class AppModule {
}
