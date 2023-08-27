import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DataProviderService } from 'src/app/services/data-provider.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  text: string = "";

  constructor(
    private http: HttpClient,
    private dataProvider: DataProviderService
  ) { }
  
  message: string = "";

  ngOnInit(): void {
    this.http.get("https://localhost:7171/api/auth", { responseType: "text" }).subscribe((response) => {
      this.text = response;
    });

    this.dataProvider.connect();

    this.dataProvider.messages$.subscribe((message) => this.message = message);

  }
}
