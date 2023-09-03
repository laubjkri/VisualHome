import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { MainComponent } from './components/main/main.component';
import { UsersComponent } from './components/users/users.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';

const routes: Routes = [
  { path: "", redirectTo: 'main/dashboard', pathMatch: 'full' },
  { path: "main", redirectTo: 'main/dashboard', pathMatch: 'full' },
  { path: "login", component: LoginComponent, pathMatch: 'full' },  
  {
    path: "main", component: MainComponent,
    children: [
      { path: "dashboard", component: DashboardComponent, pathMatch: 'full' },
      { path: "users", component: UsersComponent, pathMatch: 'full'},
    ]
  }  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
