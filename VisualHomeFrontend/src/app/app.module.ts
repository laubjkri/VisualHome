import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LoginComponent } from './components/login/login.component';

import { HTTP_INTERCEPTORS, HttpClientModule } from "@angular/common/http";
import { FormsModule } from '@angular/forms';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { AuthInterceptor } from './interceptors/auth.interceptor';

import { MainComponent } from './components/main/main.component';
import { TopBarComponent } from './components/top-bar/top-bar.component';

import { IconsModule } from './modules/icons/icons.module';

// Material modules
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from "@angular/material/button"
import { MatDividerModule} from '@angular/material/divider';
import { MatCardModule } from '@angular/material/card';
import { AuthService } from './services/auth.service';
import { DataProviderService } from './services/data-provider.service';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    DashboardComponent,
    MainComponent,
    TopBarComponent    
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatDividerModule,
    IconsModule,
    MatCardModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    AuthService,
    DataProviderService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
