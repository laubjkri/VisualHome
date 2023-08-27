import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'VisualHomeFrontend';
  constructor(
    private router: Router
  ) { }


  navigate(): void {
    // this.router.navigateByUrl()    
  }

}
