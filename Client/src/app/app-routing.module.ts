import { NgModule } from '@angular/core';
import { RouterModule, Routes} from '@angular/router';

import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { LogoutComponent } from './logout/logout.component';

const routes: Routes = [
    { path: 'home', component: HomeComponent },
    { path: 'login', component: LoginComponent },
    { path: 'logout', component: LogoutComponent },
    { path: '', redirectTo: '/home', pathMatch: 'full' },
]

@NgModule({
  exports: [RouterModule],
  imports: [RouterModule.forRoot(routes)],
})


export class AppRoutingModule {}
