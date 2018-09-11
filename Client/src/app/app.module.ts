import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

//fake backend, only for testing
import { fakeBackendProvider } from "./Interceptor/fakeBackend";

import { AppComponent } from './app.component';
import { AppRoutingModule } from './/app-routing.module';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { LogoutComponent } from './logout/logout.component';
import { AuthGuard } from "./guard/auth.guard";
import { JwtInterceptor } from "./Interceptor/jwt.interceptor";
import { UserService } from "./services/user.service";
import { LoginService } from "./services/login.service";
import { RegisterComponent } from './register/register.component';
import { TestComponent } from './test/test.component'; 

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    LoginComponent,
    LogoutComponent,
    RegisterComponent,
    TestComponent,

  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    ReactiveFormsModule,
  ],
  providers: [
    {provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true},
    fakeBackendProvider,
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
