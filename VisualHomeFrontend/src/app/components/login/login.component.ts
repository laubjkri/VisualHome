import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/models/user';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';
import { sleep } from "src/utils/sleep";
import { HttpErrorResponse } from '@angular/common/http';
import { DataProviderService } from 'src/app/services/data-provider.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {

  constructor(
    private authService: AuthService,
    private router: Router,
    private dataProviderService: DataProviderService
  ) { }

  showError: boolean = false;
  userLoggedIn: boolean = false;
  userName: string = "";
  showSucces: boolean = false;
  errorText: string = "";
  user = new User();

  ngOnInit(): void {
    if (this.userName = this.authService.getUsername()) {
      console.log("Decoded token: ", this.userName);
      this.userLoggedIn = true;
    }
  }


  login(user: User) {
    this.showError = false;
    this.showSucces = false;
    this.authService.login(user).subscribe({
      next: async () => {        
        this.showSucces = true;
        await sleep(1000);
        this.router.navigateByUrl("");
      },
      error: async (err: HttpErrorResponse) => {        
        this.errorText = err.error;
        this.showError = true;
        await sleep(3000);
        this.showError = false;
      }      
    });
  }

  logout() {
    this.authService.logout();
    this.userLoggedIn = false;
    this.dataProviderService.disconnect();
  }


  tokenIsPresent = () => this.authService.tokenIsPresent();

}
